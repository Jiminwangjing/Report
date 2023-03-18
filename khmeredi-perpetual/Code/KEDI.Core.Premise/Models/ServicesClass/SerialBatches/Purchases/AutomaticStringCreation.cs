using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Purchases
{
    public class AutomaticStringCreation
    {
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public string Name { get; set; }
        public TypeAutomaticStringCreation TypeInt { get; set; }
        public OperationAutomaticStringCreation OperationInt { get; set; }
        public List<SelectListItem> Type { get; set; }
        public List<SelectListItem> Operation { get; set; }
    }

    public enum TypeAutomaticStringCreation
    {
        String = 1,
        Number = 2
    }
    
    public enum OperationAutomaticStringCreation
    {
        [Display(Name = "No Operation")]
        NoOperation = 1,
        Increase = 2,
        Decrease = 3
    }
}
