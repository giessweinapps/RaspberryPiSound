using Ninject;
using RaspberryPiSound.Interfaces;
using RaspberryPiSound.Models;
using RaspberryPiSound.Mono;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TagLib;

namespace RaspberryPiSound.Controllers
{
    public class MusicPlayerController : ApiController
    {
        private readonly IMusicPlayer _player;
        private readonly Dictionary<string, Tag> _tagCache = new Dictionary<string, Tag>();

        public MusicPlayerController(IMusicPlayer player)
        {
            _player = player;
        }

        [HttpGet]
        public Tag GetCurrentTrack()
        {
            Tag tag = null;
            if (_player.CurrentTrack != null)
            {
                if (_tagCache.ContainsKey(_player.CurrentTrack.FullPath))
                    return _tagCache[_player.CurrentTrack.FullPath];
                try
                {
                    var fileTag = TagLib.File.Create(_player.CurrentTrack.FullPath);
                    tag = fileTag.Tag;
                    _tagCache.Add(_player.CurrentTrack.FullPath, tag);
                }
                catch
                {

                }
            }

            return tag;
        }

        [HttpPost]
        public void PostControl(PlayerControl control)
        {
            Trace.WriteLine("PostControl: " + control);
            switch (control.Action)
            {
                case PlayerControlAction.Next:
                    _player.PlayNextSong();
                    break;
                case PlayerControlAction.Stop:
                    _player.Stop();
                    break;
                case PlayerControlAction.Pause:
                    _player.Pause();
                    break;
            }
        }
    }
}
