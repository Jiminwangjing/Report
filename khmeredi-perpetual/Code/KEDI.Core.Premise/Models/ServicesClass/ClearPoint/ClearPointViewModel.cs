
namespace KEDI.Core.Premise.Models.ServicesClass.ClearPoint
{
    public class ClearPointViewModel
    {
        public int ID { get; set; }
        public string LineID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal TotalPoint { get; set; }
        public decimal ClearPoints { get; set; }
        public decimal OutstandPoint { get; set; }
        public decimal AfterClear { get; set; }

    }
}
