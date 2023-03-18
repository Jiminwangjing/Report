using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Promotions
{
    [Table("tbPointCard",Schema ="dbo")]
    public class PointCard
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int PointID { get; set; }
        [Column(TypeName ="nvarchar(25)")]
        public string Ref_No { get; set; }//phone or ID ...
        public string Name { get; set; }
        public double Remain { get; set; }
        public int Point { get; set; }
        public bool Delete { get; set; }
        public string Description { get; set; }
        [DataType(DataType.Date)]
      
        public DateTime ExpireDate { get; set; }
        public string Approve { get; set; }
        [DataType(DataType.Date)]
       
        public DateTime DateCreate { get; set; }
        public DateTime DataApprove { get; set; }

        [ForeignKey("PointID")]
        public Point Points { get; set; }
    


    }
}
