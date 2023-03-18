namespace KEDI.Core.Models.ControlCenter.ApiManagement
{
    public class ClientAppDisplay
    {
        public string ClientId { set; get; }
        public string ClientCode { set; get; }
        public string ClientName { set; get; }
        public string IpAddress { set; get; }
        public string Signature { set; get; }
        public string PublicKey { set; get; }
        public string CreatedDate { set; get; }
        public string CreatorName { set; get; }
        public string StrictIpAddress { set; get; }
        public string Revoked { set; get; }
    }
}
