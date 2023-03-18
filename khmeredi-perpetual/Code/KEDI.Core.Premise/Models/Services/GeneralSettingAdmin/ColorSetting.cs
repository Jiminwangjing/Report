using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.GeneralSettingAdmin
{
    [Table("ColorSetting")]
    public class ColorSetting
    {
        [Key]
        public int ID { get; set; }
        public Boolean Checked { get; set; }
        public string checkClick { get; set; }
        public string BackgroundColor { get; set; }
        public string FontColor { get; set; }
        public string BackgroundMenu { get; set; }
        public string BackOfSubmenu { get; set; }
        public string HoverBackgSubmenu { get; set; }
        public string BacksubmenuOnItem { get; set; }
        public string BackgButton { get; set; }
        public string Backgtableth { get; set; }
        public string Backgtabletd { get; set; }
        public string backgroundInput { get; set; }
        public string BackgroundCard { get; set; }
        public string BackgroundBar { get; set; }
        public string BackgroundBarItem { get; set; }
        public string BackgSlideBarTitle { get; set; }
        public string BackgroundIcon { get; set; }
        public string BackroundIconImage { get; set; }
        public string BackgBodyCard { get; set; }
        public string BackgBodyWet { get; set; }
        public string BackgBodyWetCard { get; set; }
        public string SkinName { get; set; }
        public string BackgroundColorName { get; set; }
        public string AlphabetColor { get; set; }
        public string HoverColor { get; set; }
        public string designName { get; set; }

        [NotMapped]
        public Boolean Unable { get; set; }
    }
}