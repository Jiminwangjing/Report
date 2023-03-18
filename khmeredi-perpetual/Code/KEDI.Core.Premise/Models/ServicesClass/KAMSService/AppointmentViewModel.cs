using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.ServicesClass.KAMSService
{
    public class AppointmentViewModel
    {
        public int ID { get; set; }
        public int CustomerID { get; set; }
        public int VehicleID { get; set; }
        public string CusName { get; set; }
        public string PhoneCus { get; set; }
        public string Plate { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Notification { get; set; }
        public DateTime PostingDate { get; set; }
        public DateTime ClosingDate { get; set; }
        public string Status { get; set; }

        public List<AppointmentService> AppointmentServices { get; set; }
    }

    public class AppointmentService
    {
        public int ID { get; set; }
        public int AppointmentID { get; set; }
        public string ServiceName { get; set; }
        public string ServiceUom { get; set; }
        public string ServiceDate { get; set; }
        public string Status { get; set; }
    }
}
