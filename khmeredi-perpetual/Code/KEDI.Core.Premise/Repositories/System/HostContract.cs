using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Hosting
{
    public class HostContract
    {
        public string Referrer { set; get; }       
        public string Issuer { set; get; }  
    }
}
