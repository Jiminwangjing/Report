using System.ComponentModel.DataAnnotations;

namespace KEDI.Core.Premise.Models.ServicesClass.EquipmentCard
{
    public enum StatusEquipmentCard
    {
        Active = 1,
        Returned = 2,
        Terminated = 3,
        Loaded = 4,
        [Display(Name = "In Repair Lab")]
        InRepairLab = 5
    }
}
