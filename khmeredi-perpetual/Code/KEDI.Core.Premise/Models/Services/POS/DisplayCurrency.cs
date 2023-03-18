using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CKBS.Models.Services.POS
{
    [Table("tbDisplayCurrency",Schema ="dbo")]
    public class DisplayCurrency
    {
        [Key]
        public int ID { get; set; }
        [NotMapped]
        public string LineID { get; set; } = $"{DateTime.Now.Ticks}";
        public int PriceListID  { get; set; }
        public int AltCurrencyID { get; set; }
        [NotMapped]
        public string PLCurrencyName { set; get; }
        [NotMapped]
        public string ALCurrencyName { set; get; }
        [Column(TypeName = "decimal(36,18)")]
        public decimal PLDisplayRate { set; get; }
        [NotMapped]
        public List<SelectListItem> AltCurrencies { set; get; }
        [Required]
        [Column(TypeName = "decimal(36,18)")]
        public decimal DisplayRate { get; set; }
        [Required]
        public double DecimalPlaces { get; set; }
        [NotMapped]
        public int CurPLID { get; set; }
        public bool IsActive { get; set; }
        public bool IsShowCurrency{ get; set; }
        public bool IsShowOtherCurrency{ get; set; }
    }
}
