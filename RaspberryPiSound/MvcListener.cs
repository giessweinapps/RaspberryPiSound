using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;
using Newtonsoft.Json;

namespace RaspberryPiSound
{
    public class MvcListener : TraceListener
    {
        private readonly string _fileName;
        public IEnumerable<string> Messages
        {
            get
            {
                try
                {
                    return JsonConvert.DeserializeObject<IEnumerable<string>>(File.ReadAllText(_fileName));
                }
                catch
                {
                    return Enumerable.Empty<string>();
                }
            }
        }

        public MvcListener(HttpConfiguration configuration)
        {
            _fileName = HttpContext.Current.Server.MapPath("MvcListener.json");
            configuration.Routes.Add("MvcListener", new ListenerHttpHandler(configuration, this));
        }

        public override void Write(string message)
        {
            WriteLine(message);
        }

        public override void WriteLine(string message)
        {
            var msg = DateTime.Now + ": " + message;
            var messages = Messages.ToList();
            messages.Insert(0, msg);
            File.WriteAllText(_fileName, JsonConvert.SerializeObject(messages, Formatting.Indented));
        }
    }

    public class ListenerHttpHandler : IHttpRoute
    {
        private readonly HttpConfiguration _configuration;
        private readonly MvcListener _mvcListener;

        public ListenerHttpHandler(HttpConfiguration configuration, MvcListener mvcListener)
        {
            _configuration = configuration;
            _mvcListener = mvcListener;
        }

        public IHttpRouteData GetRouteData(string virtualPathRoot, HttpRequestMessage request)
        {
            throw new NotImplementedException();
        }

        public IHttpVirtualPathData GetVirtualPath(HttpRequestMessage request, IDictionary<string, object> values)
        {
            throw new NotImplementedException();
        }

        public string RouteTemplate
        {
            get { return "MvcListener"; }
        }
        public IDictionary<string, object> Defaults { get; private set; }
        public IDictionary<string, object> Constraints { get; private set; }
        public IDictionary<string, object> DataTokens { get; private set; }
        public HttpMessageHandler Handler
        {
            get
            {
                return HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(_configuration), handlers: new DelegatingHandler[] { new HandlerA(_mvcListener) });
            }
        }
    }

    public class HandlerA : DelegatingHandler
    {
        private readonly MvcListener _mvcListener;

        public HandlerA(MvcListener mvcListener)
        {
            _mvcListener = mvcListener;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<link href='//maxcdn.bootstrapcdn.com/bootstrap/3.2.0/css/bootstrap.min.css' rel='stylesheet'>");
            sb.AppendLine("<table class='table'>");
            foreach (var message in _mvcListener.Messages)
            {
                sb.AppendLine("<tr>");
                sb.AppendLine("<td>" + message + "</td>");
                sb.AppendLine("</tr>");
            }
            sb.AppendLine("</table>");

            return Task.Factory.StartNew(() => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("<h1>Tracing</h1>" + sb,
                                            Encoding.UTF8,
                                            "text/html")
            }, cancellationToken);
        }
    }
}
