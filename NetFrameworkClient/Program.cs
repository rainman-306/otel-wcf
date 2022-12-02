using System;
using System.Diagnostics;
using System.Net.Http;
using System.ServiceModel;
using System.Threading.Tasks;
using Contract;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace NetFrameworkClient
{
    public class Program
    {

        public const string serviceName = "WcfClient.Test";

        public const int HTTP_PORT = 8088;
        public const int HTTPS_PORT = 8443;
        public const int NETTCP_PORT = 8089;

        static async Task Main(string[] args)
        {
            // Define some important constants to initialize tracing with
            var serviceVersion = "1.0.0";

            // Configure important OpenTelemetry settings and the console exporter
            using (var tracerProvider = Sdk.CreateTracerProviderBuilder()
                .AddSource(serviceName)
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService(serviceName: serviceName, serviceVersion: serviceVersion))
                .AddWcfInstrumentation()
                .AddOtlpExporter( o=> 
                {
                    o.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                    o.HttpClientFactory = () => 
                    {
                        HttpClient client = new HttpClient();

                        // Add any code here for setting up the http client.
                        
                        return client;

                    };
                })
                .AddConsoleExporter()
                .Build())
            {

                Console.Title = "WCF .Net Framework Client";

                // await CallBasicHttpBinding($"http://localhost:{HTTP_PORT}");
                // await CallBasicHttpBinding($"https://localhost:{HTTPS_PORT}");
                //await CallWsHttpBinding($"http://localhost:{HTTP_PORT}");
               // await CallWsHttpBinding($"https://localhost:{HTTPS_PORT}");
                //await CallNetTcpBinding($"net.tcp://localhost:{NETTCP_PORT}");
                await CallNamedPipeBinding("");
            }
        }

        private static async Task CallBasicHttpBinding(string hostAddr)
        {
            var MyActivitySource = new ActivitySource(serviceName);
            using (var activity = MyActivitySource.StartActivity("CallBasicHttpBinding"))
            {
                IClientChannel channel = null;
                var binding = new BasicHttpBinding(IsHttps(hostAddr) ? BasicHttpSecurityMode.Transport : BasicHttpSecurityMode.None);
                var factory = new ChannelFactory<IEchoService>(binding, new EndpointAddress($"{hostAddr}/basicHttp"));

                await Task.Factory.FromAsync(factory.BeginOpen, factory.EndOpen, null);
                try
                {
                    IEchoService client = factory.CreateChannel();
                    channel = client as IClientChannel;
                    await Task.Factory.FromAsync(channel.BeginOpen, channel.EndOpen, null);
                    var result = await client.Echo("Hello World!");
                    await Task.Factory.FromAsync(channel.BeginClose, channel.EndClose, null);
                    Console.WriteLine(result);
                }
                finally
                {
                    await Task.Factory.FromAsync(factory.BeginClose, factory.EndClose, null);
                }
            }
        }

        private static async Task CallWsHttpBinding(string hostAddr)
        {
            var MyActivitySource = new ActivitySource(serviceName);
            using (var activity = MyActivitySource.StartActivity("CallWsHttpBinding"))
            {
                IClientChannel channel = null;

                var binding = new WSHttpBinding(IsHttps(hostAddr) ? SecurityMode.Transport : SecurityMode.None);

                var factory = new ChannelFactory<IEchoService>(binding, new EndpointAddress($"{hostAddr}/wsHttp"));
                await Task.Factory.FromAsync(factory.BeginOpen, factory.EndOpen, null);
                try
                {
                    IEchoService client = factory.CreateChannel();
                    channel = client as IClientChannel;
                    await Task.Factory.FromAsync(channel.BeginOpen, channel.EndOpen, null);
                    var result = await client.Echo("Hello World!");
                    await Task.Factory.FromAsync(channel.BeginClose, channel.EndClose, null);
                    Console.WriteLine(result);
                }
                finally
                {
                    await Task.Factory.FromAsync(factory.BeginClose, factory.EndClose, null);
                }
            }
        }

        private static async Task CallNetTcpBinding(string hostAddr)
        {
            var MyActivitySource = new ActivitySource(serviceName);
            using (var activity = MyActivitySource.StartActivity("CallNetTcpBinding"))
            {

                IClientChannel channel = null;

                var binding = new NetTcpBinding();

                var factory = new ChannelFactory<IEchoService>(binding, new EndpointAddress($"{hostAddr}/nettcp"));
                await Task.Factory.FromAsync(factory.BeginOpen, factory.EndOpen, null);
                try
                {
                    IEchoService client = factory.CreateChannel();
                    channel = client as IClientChannel;
                    await Task.Factory.FromAsync(channel.BeginOpen, channel.EndOpen, null);
                    var result = await client.Echo("Hello World!");
                    await Task.Factory.FromAsync(channel.BeginClose, channel.EndClose, null);
                    Console.WriteLine(result);
                }
                finally
                {
                    await Task.Factory.FromAsync(factory.BeginClose, factory.EndClose, null);
                }
            }
        }


        private static async Task CallNamedPipeBinding(string hostAddr)
        {
            var MyActivitySource = new ActivitySource(serviceName);
            using (var activity = MyActivitySource.StartActivity("CallNamedPipeBinding"))
            {
                IClientChannel channel = null;

                var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);

                var factory = new ChannelFactory<IEchoService>(binding, new EndpointAddress($"net.pipe://localhost/myservice"));
                await Task.Factory.FromAsync(factory.BeginOpen, factory.EndOpen, null);
                try
                {
                    IEchoService client = factory.CreateChannel();
                    channel = client as IClientChannel;
                    await Task.Factory.FromAsync(channel.BeginOpen, channel.EndOpen, null);
                    var result = await client.Echo("Hello World!");
                    await Task.Factory.FromAsync(channel.BeginClose, channel.EndClose, null);
                    Console.WriteLine(result);
                }
                finally
                {
                    await Task.Factory.FromAsync(factory.BeginClose, factory.EndClose, null);
                }
            }
        }

        private static bool IsHttps(string url)
        {
            return url.ToLower().StartsWith("https://");
        }
    }
}
