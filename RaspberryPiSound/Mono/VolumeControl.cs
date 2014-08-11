using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using RaspberryPiSound.Interfaces;

namespace RaspberryPiSound.Mono
{
    public class VolumeControl : IVolumeControl
    {
        public int Volume
        {
            get { return CallVolume(string.Empty); }
        }

        public void ChangeVolume(string direction)
        {
            CallVolume(direction);
        }

        private int CallVolume(string arguments)
        {
            ProcessStartInfo info = new ProcessStartInfo("vol", arguments);
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;
            info.CreateNoWindow = true;

            var proc = Process.Start(info);
            string line = proc.StandardOutput.ReadLine();
            return int.Parse(line);
        }
    }
}