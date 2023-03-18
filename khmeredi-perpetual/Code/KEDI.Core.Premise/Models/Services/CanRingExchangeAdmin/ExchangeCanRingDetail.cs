using KEDI.Core.Premise.Models.Services.Administrator.CanRingSetup;

namespace KEDI.Core.Premise.Models.Services.CanRingExchangeAdmin
{
    public class ExchangeCanRingDetail : MainCanRing
    {
        public int ID { get; set; }
        public int CanRingID { get; set; }
        public int ExchangeCanRingMasterID { get; set; }
    }
}
