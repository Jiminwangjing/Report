using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Sales
{
    [Table("TmpBatchNoSelectedDetail")]
    public class BatchNoSelectedDetail : BatchNoDetailMasterClass
    {
        public int Id { get; set; }
        public int BatchNoSelectedID { get; set; }
    }
}
