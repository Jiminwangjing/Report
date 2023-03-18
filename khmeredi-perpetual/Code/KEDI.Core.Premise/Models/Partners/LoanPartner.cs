using System.Security.Cryptography.X509Certificates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Partners
{
    public class LoanPartner
    {
        public int ID{get;set;}
        public string Code{get;set;}
        public string Name1{get;set;}
        public string Name2{get;set;}
        public int Group1ID{get;set;}
        public int Group2ID{get;set;}
        public int EmpID{get;set;}
        public string EmloyeeName{get;set;}
         public string Phone{get;set;}
         public string Email{get;set;}
         public string Address{get;set;}
         public string VatNumber{get;set;}
         public string GPSLink{get;set;}
        public List<LoanContactPerson> LoanContactPeople { get; set; }
         //ISyncEntity
        public Guid RowId { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }
         public bool Delete {get;set;}
    }
}