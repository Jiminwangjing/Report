using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.ClientApi
{
    [Table("DataSyncSetting")]
    public class DataSyncSetting : IDataSyncSetting
    {
        [Key]
        public int ID { set; get; }
        public string TenantCode { set; get; }
        public string TenantName { set; get; }
        public string BaseUrl { set; get; }
        public string Username { set; get; }
        public string Password { set; get; }
        public string ApiKey { set; get; }
        public string SecretKey { set; get; }
        public TimeSpan StartTime { set; get; }
        public TimeSpan EndTime { set; get; }
        public double TickInterval { set; get; } //In minutes
        public bool Enabled { set; get; }
        public bool Revoked { set; get; }
    }
}