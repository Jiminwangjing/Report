using Microsoft.AspNetCore.Mvc.Rendering;

namespace CKBS.Models.Services.Administrator.SystemInitialization
{
    public class SeriesCreateViewModel
    {
        public int LineID { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public string FirstNo { get; set; }
        public string NextNo { get; set; }
        public string LastNo { get; set; }
        public string PreFix { get; set; }
        public SelectList PeriodIndecator { get; set; }
        public int DocuTypeID { get; set; }
        public int PeriodIndID { get; set; }
        public bool Default { get; set; }
        public bool Lock { get; set; }
    }
}
