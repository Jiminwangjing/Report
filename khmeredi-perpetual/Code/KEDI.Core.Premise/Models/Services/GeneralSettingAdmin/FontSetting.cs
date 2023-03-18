using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.GeneralSettingAdmin
{
    [Table("tbFontSetting")]
    public class FontSetting
    {
        [Key]
        public int ID { get; set; }
        public int UserID { get; set; }
        public string FontName { get; set; }
        public string FontSize { get; set; }
        public Boolean Active { get; set; }
    }
    public class SkinItem
    {
        [Key]
        public int ID { get; set; }
        public int SkinID { get; set; }
        public int UserID { get; set; }
        public Boolean Unable { get; set; } = false;
        public string SkinName { get; set; }
        public string BackgroundColor { get; set; }
        public string AlphabetColor { get; set; }
        public string HoverColor { get; set; }
        public string designName { get; set; }


    }
    public class SkinUser
    {
        [Key]
        public int ID { get; set; }
        public int SkinID { get; set; }
        public int UserID { get; set; }
        public Boolean Unable { get; set; } = false;
    }

}