using System.Runtime.Serialization;
using System.ServiceModel;
using OpenTelemetry.Instrumentation.Wcf;

namespace Contract
{
    [DataContract]
    public class EchoFault
    {
        private string text;

        [DataMember]
        public string Text
        {
            get { return text; }
            set { text = value; }
        }
    }

    [ServiceContract]
    [TelemetryContractBehavior]
    public interface IEchoService
    {
        [OperationContract]
        string Echo(string text);

        [OperationContract]
        string ComplexEcho(EchoMessage text);

        [OperationContract]
        [FaultContract(typeof(EchoFault))]
        string FailEcho(string text);
    }

    [DataContract]
    public class EchoMessage
    {
        [DataMember]
        public string Text { get; set; }
    }
}
