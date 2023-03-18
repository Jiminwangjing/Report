using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Appointment
{
    public enum StatusAppoint { open, close, cancel }
    [Table("tbAppointment", Schema = "dbo")]
    public class Appointment
    {
        [Key]
        public int ID { get; set; }
        public int CustomerID { get; set; }
        public int VehicleID { get; set; }
        public int UserID { get; set; }
        public int CompanyID { get; set; }
        public DateTime PostingDate { get; set; }
        public DateTime ClosingDate { get; set; }
        public StatusAppoint Status { get; set; }
        public string Notification { get; set; }
        //Class
        public List<AppointmentService> AppointmentServices { get; set; }
    }

    [Table("tbAppointmentService", Schema = "dbo")]
    public class AppointmentService
    {
        [Key]
        public int ID { get; set; }
        public int AppointmentID { get; set; }
        public string ServiceName { get; set; }
        public DateTime ServiceDate { get; set; }
        public string ServiceUom { get; set; }
        public StatusAppoint Status { get; set; }
        public bool TimelyService { get; set; }

    }
}
