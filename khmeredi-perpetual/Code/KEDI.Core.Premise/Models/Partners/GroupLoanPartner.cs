using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Partners
{
    public enum Grouploan{Group1= 1,Group2=2}
     
    public class GroupLoanPartner
    {
        public int ID{get;set;}
        public string Name{get;set;}
        public Grouploan Grouploan{get;set;}
        public int Group1ID{get;set;}
        public bool Delete{get;set;}
    }
}