using KEDI.Core.Premise.Models.Sync;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Administrator.Tables
{
    [Table("tbTable", Schema = "dbo")]
    public class Table : ISyncEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required(ErrorMessage = "Please input name !")]
        public string Name { get; set; }
        public int GroupTableID { get; set; }
        public string Image { get; set; }
        public int PriceListID { get; set; }
        public bool IsTablePriceList { get; set; }
        [Required]
        public char Status { get; set; } = 'A'; //{A: free, B: busy, P: bill}

        public string Time { get; set; }
        public bool Delete { get; set; }
        //Foreign Key
        [ForeignKey("GroupTableID")]
        public GroupTable GroupTable { get; set; }
        public DateTimeOffset StartDateTime { set; get; }
        public DateTimeOffset EndDateTime { set; get; }
        public TableType Type { get; set; }
        public double Duration
        {
            get
            {
                switch (Status)
                {
                    case 'B': return (DateTimeOffset.Now - StartDateTime).TotalSeconds;
                    case 'P': return (EndDateTime - StartDateTime).TotalSeconds;
                    default: return 0;
                }
            }
        } // in seconds

        public string DurationText
        {
            get
            {
                var duration = TimeSpan.FromSeconds(Duration);
                return $"{duration:%h}h {duration:%m}m {duration:%s}s";
                //return $"{ duration:%d}d {duration:%h}h {duration:%m}m {duration:%s}s";
            }
        }

        //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }
        [NotMapped] 
        public string RefNo {get;set;}
    }
    public enum TableType
    {
        Normal = 1,
        Delivery = 2,
        TakeAway = 3
    }
}
