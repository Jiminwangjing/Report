using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.ChartOfAccounts  
{
    public class GLAccountViewModel
    {
        public GLAccount GLAccount { get; set; }
        public SelectList Currencies { get; set; }
        public IEnumerable<GLAccount> Categories { get;set; }
        public IEnumerable<GLAccount> Details { get; set; }
    }
}
