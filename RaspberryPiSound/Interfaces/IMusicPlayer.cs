using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RaspberryPiSound.Models;

namespace RaspberryPiSound.Interfaces
{
    public interface IMusicPlayer
    {
        IEnumerable<PathDescription> Playlist { get; }
        PathDescription CurrentTrack { get; }
        void Add(PathDescription file);
        void PlayNextSong();
        void Pause();
        void Stop();
    }
}