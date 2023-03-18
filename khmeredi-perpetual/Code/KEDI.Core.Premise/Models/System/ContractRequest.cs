namespace KEDI.Core.Hosting.Models
{
    public class ContractRequest<TPayload>
    {
        public TPayload Payload { set; get; }
        public string SessionToken { set; get; }
    }
}
