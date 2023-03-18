namespace KEDI.Core.Premise.Models.GeneralSettingAdmin
{
    public class Display
    {
        public int ID { get; set; }
        public int Prices { get; set; }
        public int Amounts { get; set; }
        public int Rates { get; set; }
        public int Quantities { get; set; }
        public int Units { get; set; }
        public int Percent { get; set; }
        public string DecimalSeparator { get; set; } = ".";
        public string ThousandsSep { get; set; } = ",";
        public int DisplayCurrencyID { get; set; }
    }
}
//500290257