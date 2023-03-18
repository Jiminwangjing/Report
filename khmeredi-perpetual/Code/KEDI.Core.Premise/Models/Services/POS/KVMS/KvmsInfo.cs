using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CKBS.Models.Services.POS.KVMS
{
    public enum StatusKvmsInfo { Delete, Exist }
    [Table("tbKvmsInfo", Schema = "dbo")]

    public class KvmsInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int KvmsInfoID { get; set; }
        public string QNo { get; set; }
        public int CusID { get; set; } //BusinessPartner
        public string Code { get; set; }
        public string Name { get; set; }
        public int PriceListID { get; set; }
        public string Type { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        //AutoMobile
        public int AutoMID { get; set; }
        public string Plate { get; set; }
        public string Frame { get; set; }
        public string Engine { get; set; }
        public string TypeName { get; set; }
        public string BrandName { get; set; }
        public string ModelName { get; set; }
        public string ColorName { get; set; }
        public string Year { get; set; }

        public StatusKvmsInfo Status { get; set; }
    }
}
