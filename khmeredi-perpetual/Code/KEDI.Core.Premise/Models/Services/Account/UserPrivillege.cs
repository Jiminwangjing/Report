using CKBS.Models.Services.Account;
using CKBS.Models.Services.HumanResources;
using KEDI.Core.Premise.Models.Sync;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Administrator.General
{
    [Table("tbUserPrivillege", Schema ="dbo")]
    public class UserPrivillege : ISyncEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int UserID { get; set; }
        public int FunctionID { get; set; }
        public bool Used { get; set; }
        public bool Delete { get; set; }
        public string Code { get; set; }
        [ForeignKey("UserID")]
        public List<UserAccount> UserAccount { get; set; }
        [ForeignKey("FunctionID")]
        public Function Function { get; set; }

        //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }

    }
}
