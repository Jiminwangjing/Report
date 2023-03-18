using System;
using System.ComponentModel.DataAnnotations;

namespace KEDI.Core.Premise.Models.Sync
{
    public interface ITransactionHistory
    {
        public Guid RowId { set; get; }
        public byte[] RowVersion { set; get; }
        public DateTimeOffset ChangeLog { set; get; } //In Utc
    }
}