using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Administrator.SystemInitialization
{
    public class SeriesViewModel
    {
        public int LineID { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public string FirstNo { get; set; }
        public string NextNo { get; set; }
        public string LastNo { get; set; }
        public string PreFix { get; set; }
        public List<SelectListItem> PeriodIndecator { get; set; }
        public int DocuNumberingID { get; set; }
        public int PeriodIndID { get; set; }
        public bool Default { get; set; }
        public bool Lock { get; set; }
        

    }
}
