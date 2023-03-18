using System.ComponentModel.DataAnnotations;

namespace CKBS.Models.Services.Inventory
{
    public enum ManagementMethod
    {
        [Display(Name = "None")]
        None = 0,
        [Display(Name = "On Every Transation")]
        OnEveryTransation = 1,
        // [Display(Name = "On Release Only")]
        // OnReleaseOnly = 2
    }
}
