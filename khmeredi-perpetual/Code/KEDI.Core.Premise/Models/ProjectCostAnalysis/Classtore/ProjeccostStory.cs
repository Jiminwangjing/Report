namespace KEDI.Core.Premise.Models.ProjectCostAnalysis
{
    public enum CopyTypeSQ
    {
        SQ = 1,
        SDM = 2
    }
    public class ProjeccostStory
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int SeriesID { get; set; }
        public int CompanyID { get; set; }
        public string InvoiceNumber { get; set; }
        public string PostingDate { get; set; }
        public string ValidUntilDate { get; set; }
        public string DocumentDate { get; set; }
        public int BaseOnID { get; set; }
        public CopyTypeSQ CopyTypeSQ { get; set; }
        public string KeyCopy { get; set; }
       
    }
   
}
