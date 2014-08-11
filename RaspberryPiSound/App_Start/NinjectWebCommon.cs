using System.Reflection;
using System.Web.Http;
using Moq;
using RaspberryPiSound.Interfaces;
using RaspberryPiSound.Models;
using RaspberryPiSound.Mono;

namespace RaspberryPiSound.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;

    public static class NinjectWebCommon
    {
        public static void CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Load(Assembly.GetExecutingAssembly());
                GlobalConfiguration.Configuration.DependencyResolver = new LocalNinjectDependencyResolver(kernel);

                RegisterServices(kernel);
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        private static void RegisterServices(IKernel kernel)
        {
            if (IsLinux)
            {
                kernel.Bind<IVolumeControl>().To<VolumeControl>().InSingletonScope();
                kernel.Bind<IMusicPlayer>().To<MusicPlayer>().InSingletonScope();
            }
            else
            {
                var rnd = new Random();
                var mockVolume = new Mock<IVolumeControl>();
                mockVolume.SetupGet(x => x.Volume).Returns(rnd.Next(0, 100));
                kernel.Bind<IVolumeControl>().ToConstant(mockVolume.Object).InSingletonScope();

                var mockPlayer = new Mock<IMusicPlayer>();
                mockPlayer.SetupGet(x => x.CurrentTrack).Returns(new PathDescription() { FullPath = @"\\server\D\Musik\05 - Why You'd Want To Live Here.mp3", Name = "Hallo" });
                kernel.Bind<IMusicPlayer>().ToConstant(mockPlayer.Object).InSingletonScope();
            }
        }

        public static bool IsLinux
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }
    }
}
