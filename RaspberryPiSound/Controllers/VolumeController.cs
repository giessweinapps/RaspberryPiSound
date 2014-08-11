using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using Ninject;
using RaspberryPiSound.Interfaces;
using RaspberryPiSound.Mono;

namespace RaspberryPiSound.Controllers
{
    public class VolumeController : ApiController
    {
        private readonly IVolumeControl _control;
        private readonly object _lock = new object();

        public VolumeController(IVolumeControl control)
        {
            _control = control;
        }

        [HttpGet]
        public int GetVolume()
        {
            return _control.Volume;
        }

        [HttpPost]
        public int ChangeVolume([FromUri] bool up)
        {
            if (Monitor.TryEnter(_lock, 0))
            {
                var direction = up ? "+" : "-";
                _control.ChangeVolume(direction);
            }
            return _control.Volume;
        }
    }
}
