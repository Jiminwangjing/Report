using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.HumanResources
{
    [Table("ContactPerson")]
    public class ContactPerson
    {
        [Key]
        public int ID { get; set; }
        [NotMapped]
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
       public int BusinessPartnerID { get; set; }
        public string ContactID { get; set; }
        public string FirstName { get; set; }
        public string MidelName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string Position { get; set; }
        public string Address { get; set; }
        public string Tel1 { get; set; }
        public string Tel2 { get; set; }
        public string MobilePhone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Pager { get; set; }
        public string Remark1 { get; set; }
        public string Remark2 { get; set; }
        public string Parssword { get; set; }
        [NotMapped]
        public List<SelectListItem> CountriesOfBirth { get; set; }
        public int ContryOfBirth { get; set; }
        public DateTime DateOfBirth { get; set; }
        [NotMapped]
        public List<SelectListItem> Genders { get; set; }
        public int Gender { get; set; }
        public string Profession { get; set; }
        public bool SetAsDefualt { get; set; }



    }
    public enum Genders
    {
        Male = 1,
        Female = 2,

    }
    public enum ContriesOfBirth
    {
        Khmer=1,
        Thailand=2,
        China=3,
        Lao=4,
        Mayamar=5,


    }
}
