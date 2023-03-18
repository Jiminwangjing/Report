using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.HumanResources;
using KEDI.Core.Premise.Models.ClientApi.UserAccount;
using KEDI.Core.Premise.Models.Sync;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CKBS.Models.Services.Account
{
    [Table("tbUserAccount", Schema = "dbo")]
    public class UserAccount : ISyncEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int? EmployeeID { get; set; }
        public int? CompanyID { get; set; }
        public int BranchID { get; set; }
        [StringLength(50)]
        [MinLength(4, ErrorMessage = "Username must have at least 4 characters.")]
        [Required(ErrorMessage = "Username cannot be empty.")]
        public string Username { get; set; }
        //[MinLength(4, ErrorMessage = "Password must have at least 4 characters.")]
        //[Required(ErrorMessage = "Password cannot be empty.")]
        [DataType(DataType.Password)]
        [JsonIgnore]
        public string PasswordHash { get; set; }
        [DataType(DataType.Password)]
        [JsonIgnore]
        public string Password { get; set; }
        [MinLength(4, ErrorMessage = "Password must have at least 4 characters.")]
        [DataType(DataType.Password)]
        [JsonIgnore]
        public string ComfirmPassword { get; set; }
        public string Email { set; get; }
        public bool UserPos { get; set; }
        public string Language { get; set; }
        public bool Status { get; set; }
        public bool Delete { get; set; }
        [ForeignKey("EmployeeID")]
        public Employee Employee { get; set; }
        [ForeignKey("CompanyID")]
        public Company Company { get; set; }
        [ForeignKey("BranchID")]
        public Branch Branch { get; set; }
        public string TelegramUserID { get; set; }
        public bool IsUserOrder { get; set; }
        public string PasswordStamp { set; get; }
        public string SecurityStamp { set; get; }
        public string PublicKey { set; get; }
        public string Signature { set; get; }
        [JsonIgnore]
        [ForeignKey("UserID")]
        public List<RefreshToken> RefreshTokens { get; set; }


        //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }

        public Boolean Skin { get; set; }
    }
     public class MultiBrand
    {
       [Key] 
        public int ID { get; set; } 
         [NotMapped] 
          public string LineID { get; set; } 
        [NotMapped] 
          public string Name { get; set; } 
          [NotMapped] 
          public string UserName { get; set; } 
          [NotMapped] 
          public string Location { get; set; } 
          [NotMapped] 
          public string Address { get; set; } 
         public int BranchID { get; set; } 
        public int UserID { get; set; } 
        public bool Active { get; set; } 
        [NotMapped]
        public  bool Defualt { get; set; }

    }
}
