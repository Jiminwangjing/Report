using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Sync
{
    [Table("ClientSyncHistory")]
    public class ClientSyncHistory : ITransactionHistory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { set; get; }
        public Guid RowId { set; get; }
        [Timestamp]
        public byte[] RowVersion { set; get; }
        public DateTimeOffset ChangeLog { get; set; }
        public string TableName { set; get; }
    }
}