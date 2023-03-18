using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.HumanResources
{
    public class BusinessPartnerExport
    {
        public int No {get; set;}
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; } // Vender,Customer
        public string Group1 { get; set; }
        public string Phone { get; set; }
        public string PriceList { get; set; }
    }
    public class Customer 
    {
        public int No {get; set;}
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; } // Vender,Customer
        public string Group1 { get; set; }
        public string Phone { get; set; }
        public string PriceList { get; set; }
    }
    public class Vender
    {
        public int No {get; set;}
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; } // Vender,Customer
        public string Group1 { get; set; }
        public string Phone { get; set; }
        public string PriceList { get; set; }
    }
}