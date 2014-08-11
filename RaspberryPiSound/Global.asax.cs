using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using RaspberryPiSound.App_Start;

namespace RaspberryPiSound
{
    // Hinweis: Anweisungen zum Aktivieren des klassischen Modus von IIS6 oder IIS7 
    // finden Sie unter "http://go.microsoft.com/?LinkId=9394801".

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            NinjectWebCommon.CreateKernel();

            Trace.Listeners.Add(new MvcListener(GlobalConfiguration.Configuration));
            WebApiConfig.Register(GlobalConfiguration.Configuration);
        }
    }
}