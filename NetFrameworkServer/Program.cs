using System;
using System.ServiceModel;
using OpenTelemetry.Resources;
using OpenTelemetry;
using OpenTelemetry.Trace;
using System.Net.Http;

namespace DesktopServer
{
    class Program
    {
        public const string serviceName = "WcfServer.Test";

        static void Main()
        {
            var serviceVersion = "1.0.0";

            using (var tracerProvider = Sdk.CreateTracerProviderBuilder()
                .AddSource(serviceName)
                .SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService(serviceName: serviceName, serviceVersion: serviceVersion))
                    .AddConsoleExporter()
                    .AddWcfInstrumentation()
                    .AddOtlpExporter(o =>
                    {
                        o.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                        o.HttpClientFactory = () =>
                        {
                            HttpClient client = new HttpClient();

                            // Add any code here for setting up the http client.

                            return client;

                        };
                    })
                    .Build())
            {
                var httpUrl = "http://localhost:8088";
                var httpsUrl = "https://localhost:8443";
                var netTcpUrl = "net.tcp://localhost:8089";

                Uri[] baseUriList = new Uri[] { new Uri(httpUrl), new Uri(httpsUrl), new Uri(netTcpUrl) };

                Type contract = typeof(Contract.IEchoService);
                var host = new ServiceHost(typeof(EchoService), baseUriList);

                //host.AddServiceEndpoint(contract, new BasicHttpBinding(BasicHttpSecurityMode.None), "/basichttp");
                //host.AddServiceEndpoint(contract, new BasicHttpsBinding(BasicHttpsSecurityMode.Transport), "/basichttp");
                //host.AddServiceEndpoint(contract, new WSHttpBinding(SecurityMode.None), "/wsHttp");
                //host.AddServiceEndpoint(contract, new WSHttpBinding(SecurityMode.Transport), "/wsHttp");
                //host.AddServiceEndpoint(contract, new NetTcpBinding(), "/nettcp");
                host.AddServiceEndpoint(contract, new NetNamedPipeBinding(NetNamedPipeSecurityMode.None), "net.pipe://localhost/myservice");

                host.Open();

                LogHostUrls(host);

                Console.WriteLine("Hit enter to close");
                Console.ReadLine();

                host.Close();
            }
        }

        private static void LogHostUrls(ServiceHost host)
        {
            foreach (System.ServiceModel.Description.ServiceEndpoint endpoint in host.Description.Endpoints)
            {
                Console.WriteLine("Listening on " + endpoint.ListenUri.ToString());
            }
        }

    }
}
