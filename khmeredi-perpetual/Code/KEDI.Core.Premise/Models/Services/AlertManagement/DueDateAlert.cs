using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.AlertManagement
{
    public class DueDateAlert
    {
        public int ID { get; set; }
        public int BPID { get; set; }
        public int InvoiceID { get; set; }
        public int SeriesDID { get; set; }
        public string InvoiceNumber { get; set; }
        public string Name { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime DueDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DueDateType DueDateType { get; set; }
        public string Type { get; set; }
        public string TimeLeft { get; set; }
        public int CompanyID { get; set; }
        public int UserID { get; set; }

    }
    public enum DueDateType
    {
        Null,
        Vendor,
        Customer
    }
}
