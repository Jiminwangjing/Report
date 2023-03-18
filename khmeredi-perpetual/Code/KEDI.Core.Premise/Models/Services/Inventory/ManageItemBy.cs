using System.ComponentModel.DataAnnotations;

namespace CKBS.Models.Services.Inventory
{
    public enum ManageItemBy
    {
        [Display(Name = "None")]
        None = 0,
        [Display(Name = "Serial Numbers")]
        SerialNumbers = 1,
        Batches = 2
    }
}
