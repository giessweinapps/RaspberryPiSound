using System.Threading.Tasks;
using RaspberryPiSound.Interfaces;
using RaspberryPiSound.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace RaspberryPiSound.Mono
{
    public class MusicPlayer : IMusicPlayer
    {
        public IEnumerable<PathDescription> Playlist { get { return _playlist.AsEnumerable(); } }
        public PathDescription CurrentTrack { get; private set; }
        private readonly Queue<PathDescription> _playlist = new Queue<PathDescription>();
        private bool _isPlaying;
        private Process _proc;
        private Task _outputParseTask;

        public MusicPlayer()
        {
            CheckIfMpg321IsRunning();
        }

        private void CheckIfMpg321IsRunning()
        {
            if (_proc != null && !_proc.HasExited)
                return;
            TryToKillAllRunningInstances();

            var info = new ProcessStartInfo("mpg321", "-R abcd")
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                RedirectStandardInput = true
            };

            _proc = Process.Start(info);
            _outputParseTask = Task.Factory.StartNew(ParseOutput, TaskCreationOptions.LongRunning);
        }

        private void ParseOutput()
        {
            var output = _proc.StandardOutput;
            while (!output.EndOfStream)
            {
                var line = output.ReadLine();
                if (line != null && line.StartsWith("@P 3"))
                {
                    _isPlaying = false;
                    PlayNextSong();
                }
            }
        }

        public void Add(PathDescription file)
        {
            _playlist.Enqueue(file);
            if (!_isPlaying)
                PlayNextSong();
        }
       
        public void Stop()
        {
            _isPlaying = false;
            Command("STOP");
            _playlist.Clear();
        }

        public void Pause()
        {
            Command("PAUSE");
        }

        private void Command(string cmd)
        {
            _proc.StandardInput.WriteLine(cmd);
        }

        public void PlayNextSong()
        {
            CheckIfMpg321IsRunning();
            bool playlistIsNotEmpty = _playlist.Count > 0;
            Trace.WriteLine("PlayNextSong > playlistIsNotEmpty:" + playlistIsNotEmpty);

            if (playlistIsNotEmpty)
            {
                var file = _playlist.Dequeue();
                CurrentTrack = file;
                Trace.WriteLine("PlayNextSong > Dequeued an trying to play: " + file);
                Command("LOAD " + file.FullPath);
                _isPlaying = true;
            }
        }
        
        private void TryToKillAllRunningInstances()
        {
            var running = Process.GetProcessesByName("mpg321");
            Trace.WriteLine("TryToKillAllRunningInstances > Kill a total of: " + running.Length);

            foreach (var proc in running)
            {
                proc.Kill();
            }
        }

    }
}