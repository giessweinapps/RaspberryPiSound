using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;
using Ninject;

namespace RaspberryPiSound.App_Start
{
    internal class LocalNinjectDependencyResolver : IDependencyResolver
    {
        private readonly IKernel _kernel;
        public LocalNinjectDependencyResolver(IKernel kernel)
        {
            _kernel = kernel;
        }

        public void Dispose()
        {
            
        }

        public object GetService(Type serviceType)
        {
            return _kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _kernel.GetAll(serviceType);
        }

        public IDependencyScope BeginScope()
        {
            return this;
        }
    }
}