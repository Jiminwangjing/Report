using KEDI.Core.Premise.Models.Sync;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CKBS.Models.Services.HumanResources
{
    [Table("tbEmployee", Schema = "dbo")]
    public class Employee : ISyncEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [MaxLength(50)]
        [Required(ErrorMessage = "Please input code !")]
        public string Code { get; set; }
        [MaxLength(50)]
        [Required(ErrorMessage = "Please input name !")]
        public string Name { get; set; }
        [NotMapped]
        public string GenderDisplay { get; set; }
        public Gender Gender { get; set; }///Male,Female
       // public Dictionary<string, string> Gender { get; set; }
        [Column(TypeName = "date")]
        public DateTime Birthdate { get; set; }
        [Column(TypeName = "date")]
        public DateTime Hiredate { get; set; }
        [NotMapped]
        public string BirthdateDisplay { get; set; }
        [NotMapped]
        public string HireDateDisplay { get; set; }
        [MaxLength(220)]
        public string Address { get; set; }
        [Phone]
        [Required(ErrorMessage = "Please input phone !")]
        public string Phone { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public byte[] Image;
        public string Photo { get; set; }
        public bool Stopwork { get; set; }
        public string Position { get; set; }
        public bool IsUser { get; set; }
        public bool Delete { get; set; }
        public int CompanyID { get; set; }
        public int EMTypeID { get; set; }
        [NotMapped]
        public string EMType { get; set; }
        [NotMapped]
        public string Action { get; set; }

        //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }
    }

    public enum Gender
    {
        Male, Female
    };


}