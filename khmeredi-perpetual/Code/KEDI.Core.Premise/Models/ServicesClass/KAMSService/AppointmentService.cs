using CKBS.Models.Services.Appointment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.ServicesClass.KAMSService
{
    public class AppointmentServiceClass
    {
        public Appointment Appointment { get; set; }
        public List<AppointmentService> AppointmentServices { get; set; }
    }

    public class VehicleServiceClass
    {
        public int AutoMID { get; set; }
        public string Plate { get; set; }
        public string Frame { get; set; }
        public string Engine { get; set; }
        public string VehiTypes { get; set; }
        public string VehiBrands { get; set; }
        public string VehiModels { get; set; }
        public string VehiColors { get; set; }
        public string Year { get; set; }
    }
}
