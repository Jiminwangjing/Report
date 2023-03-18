
using CKBS.Models.Services.ChartOfAccounts;
using CKBS.Models.Services.Inventory.PriceList;
using KEDI.Core.Premise.Models.Services.HumanResources;
using KEDI.Core.Premise.Models.Sync;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CKBS.Models.Services.HumanResources
{
    [Table("tbBusinessPartner", Schema = "dbo")]
    public class BusinessPartner : ISyncEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int Group1ID { get; set; }
        public int Group2ID { get; set; }
        public int SaleEMID { get; set; }
        public string VatNumber { get; set; }
        public int TerritoryID { get; set; }
        public int CustomerSourceID { get; set; }
        public int PaymentTermsID { get; set; }
        public string GPSink { get; set; }
        public decimal CreditLimit { get; set; }
        public int GLAccID { get; set; }
        public int GLAccDepositID { get; set; }
        [NotMapped]
        public string TerrName { get; set; }
        [NotMapped]
        public string SaleEmpName { get; set; }
        [NotMapped]
        public string Territory { get; set; }

        [NotMapped]
        public string Group1Name { get; set; }
        [NotMapped]
        public string Group1 { get; set; }
        [NotMapped]
        public string PhoneNumber { get; set; }
        [NotMapped]
        public string PriceListName { get; set; }

        public string PaymentCode { get; set; }
        [NotMapped]
        public string CustomerSourceName { get; set; }
        [NotMapped]
        public int InstaillmentID { get; set; }
        [NotMapped]
        public string Group2Name { get; set; }
        [Required(ErrorMessage = "Please input code !")]
        public string Code { get; set; }
        [Required(ErrorMessage = "Please input name !")]
        public string Name { get; set; }
        public string Name2 { get; set; }
        public string StoreName { get; set; }
        public decimal CumulativePoint { set; get; }


        public int CardMemberID { get; set; }
        public decimal Balance { get; set; }
        public decimal RedeemedPoint { set; get; }
        [NotMapped]
        public string LineID { get; set; }
        [NotMapped]
        public decimal TotalPoint { get; set; }
        [NotMapped]
        public decimal ClearPoints { get; set; }
        [NotMapped]
        public decimal AfterClear { get; set; }
        public decimal OutstandPoint { set; get; }
        public DateTimeOffset BirthDate { set; get; }
        public string Type { get; set; } // Vender,Customer
        [NotMapped]
        public string ContactPerson { get; set; }
        [NotMapped]
        public string DaysInstail { get; set; }
        [NotMapped]
        public List<InstaillmentDetail> Instiallment { get; set; }
        [NotMapped]
        public int Tel { get; set; }

        [NotMapped]
        public string GLCode { get; set; }
        [NotMapped]
        public int InstaillID { get; set; }
        [NotMapped]
        public string GLName { get; set; }
        [NotMapped]
        public int ActID { get; set; }
        [NotMapped]
        public string PriName { get; set; }
        public int PriceListID { get; set; }
        public string Phone { get; set; }
        [NotMapped]
        public int Months { get; set; }
        [NotMapped]
        public int Days { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public bool Point { get; set; } = false;
        public bool Delete { get; set; } = false;
        public List<AutoMobile> AutoMobile { get; set; }
        public int GroupID { set; get; }
        [NotMapped]
        public int Activities { get; set; }
        //Foreign Key
        [ForeignKey("PriceListID")]
        public PriceLists PriceList { get; set; }
        public List<ContactPerson> ContactPeople { get; set; }
        public List<ContractBiling> ContractBilings { get; set; }
        public List<BPBranch> BPBranches { get; set; }

        //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }
    }

    public class ContactPersonViewModel
    {
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public int BusinessPartnerID { get; set; }
        public int ID { get; set; }
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
        public int CountryOfBirth { set; get; }
        public List<SelectListItem> CountriesOfBirths { get; set; }
        public DateTime DateOfBirth { get; set; }
        public List<SelectListItem> Genders { get; set; }
        public int Gender { get; set; }
        public string Profession { get; set; }
        public bool SetAsDefualt { get; set; }

    }
}
