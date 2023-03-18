using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.PrintList
{
    public class PrintPricelist
    {
        public int ID { get; set; }
        public string Addresskh { get; set; }
        public string AddressEng { get; set; }
        public string Currency { get; set; }
        public string Code { get; set; }
        public string Branches { get; set; }
        public string UomName { get; set; }
        public string ItemName { get; set; }
        public string EnglishName { get; set; }
        public double Price { get; set; }
        public string Logo { get; set; }
        public string Tel1 { get; set; }
        public string Tel2 { get; set; }
        public string Brand { get; set; }
    }
}
