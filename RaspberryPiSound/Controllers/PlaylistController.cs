using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Ninject;
using RaspberryPiSound.Interfaces;
using RaspberryPiSound.Models;
using RaspberryPiSound.Mono;

namespace RaspberryPiSound.Controllers
{
    public class PlaylistController : ApiController
    {
        private IMusicPlayer _player;

        public PlaylistController(IMusicPlayer player)
        {
            _player = player;
        }

        [HttpGet]
        public IEnumerable<PathDescription> Status()
        {
            return _player.Playlist;
        }

        [HttpPost]
        public IEnumerable<PathDescription> Add(IEnumerable<PathDescription> paths)
        {
            _player.Stop();
            foreach (var path in paths)
            {
                _player.Add(path);
            }
            return _player.Playlist;
        }
    }
}