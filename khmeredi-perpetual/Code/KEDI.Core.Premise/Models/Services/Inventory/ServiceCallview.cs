using KEDI.Core.Premise.Models.Services.ChartOfAccounts;
using System.Collections.Generic;

namespace KEDI.Core.Premise.Models.Services.Inventory
{
    public class ServiceCallview
    {
        public int ID { get; set; }
        public int CallID { get; set; }
        public string Number { get; set; }
        public int BPID { get; set; }
        public string BPCode { get; set; }
        public string BName { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Priority { get; set; }
        public string CallStatus { get; set; }
        public string Handleby { get; set; }
        public string Technician { get; set; }
        public string ItemName { get; set; }
        public string MfrSerialNo { get; set; }
        public string SerialNumber { get; set; }

        public int ItemID { get; set; }
        public int SeriesID { get; set; }
        public int ItemGroupID { get; set; }



        public string CreatedOnDate { get; set; }
        public string CreatedOnTime { get; set; }
        public string ClosedOnDate { get; set; }
        public string ClosedOnTime { get; set; }
        public string Resolvedondate { get; set; }
        public string Resolvedontime { get; set; }
        public double AvgResolutionTime { set; get; }
        public string ItemCode { get; set; }
        public int ChannelID { get; set; }//F
        public string ChannelName { get; set; }//F

        public string GName { get; set; }
        public double CountData { get; set; }
        public string Resolution { get; set; }
    }
    public class INComeStatementView
    {
        public int ID { get; set; }
        public string Titile { get; set; }
        public decimal Balance { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal SubTypeAcountID { get; set; }
        public decimal Revenue { get; set; }
        public decimal COGS { get; set; }
        public decimal GrossProfit { get; set; }
        public decimal Opex { get; set; }
        public decimal OperatingProfit { get; set; }
        public decimal InterestTax { get; set; }
        public decimal NetProfit { get; set; }
        public string Currency { get; set; }
        public int Format { get; set; }
        public string Date { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }
        public List<Opex> OpexItem { get; set; }
    }
    public class BarchartView
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Name2 { get; set; }
        public string Name3 { get; set; }
        public decimal Revenue1 { get; set; }
        public decimal Revenue2 { get; set; }
        public decimal Revenue3 { get; set; }
        public decimal Debit1 { get; set; }
        public decimal Crebit1 { get; set; }
        public decimal Debit2 { get; set; }
        public decimal Crebit2 { get; set; }
        public decimal Debit3 { get; set; }
        public decimal Crebit3 { get; set; }
        public decimal Total { get; set; }
        public int Format { get; set; }
        public string Date { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }
        public List<Opex> OpexItem { get; set; }
    }
}
