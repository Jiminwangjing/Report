using KEDI.Core.Premise.Models.GeneralSettingAdmin;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.ServicesClass.InterestRange
{
    public class preditedclosingViewModel
    {
        public List<SelectListItem> PreditedClosing { get; set; }
        public Display GeneralSetting { get; set; }

    }
    public class SetUpViewModel
    {
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public int ID { get; set; }      
        public string DescriptionLevel { get; set; }
    }
    public class SetUpLevelViewModel
    {
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public int ID { get; set; }
        public string Description { get; set; }
    }

    public class DescriptionPotentailViewModel
    {
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public int ID { get; set; }
        public int interestID { get; set; }
        public List<SelectListItem> selectDescription { get; set; }

    }
    public class PotentialViewModel
    {
        public int ID { get; set; }
        public int PredictedClosingInTime { get; set; }
        public int PredictedClosingInNum { get; set; }
        public DateTime PredictedClosingDate { get; set; }
        public decimal PotentailAmount { get; set; }
        public decimal WeightAmount { get; set; }
        public decimal GrossProfit { get; set; }
        public decimal GrossProfitTotal { get; set; }
        public int LevelID { get; set; }
        public List<SelectListItem> DescriptionPotentialDetail { get; set; }
    }
    public class SaleEmployeeViewModel
    {
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public int ID { get; set; }

        public string Name { get; set; }
    }
    public class StageViewModel
    {
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public int ID { get; set; }
        public string SaleEmp { get; set; }
        public int StagesID { get; set; }
        public int OwnerID { get; set; }
        public int DoctypeID { get; set; }
        public int SaleEmpselectID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime CloseDate { get; set; }
        public List<SelectListItem> SaleEmpselect { get; set; }
        public List<SelectListItem> Nameselect { get; set; }
        public string SetupStage { get; set; }
        public float Percent { get; set; }
        public decimal PotentailAmount { get; set; }
        public decimal WeightAmount { get; set; }
        public bool ShowBpsDoc { get; set; }
        public List<SelectListItem> Doctypeselect { get; set; }


        public int DocNo { get; set; }
        public string Activety { get; set; }
        public int ActivetyID { get; set; }
        public string Owner { get; set; }
        //public List<DocTypeViewModel> ListDoctype { get; set; }


    }
    public class DocTypeViewModel {
        public int ID { get; set; }
        public string Name { get; set; }
    }
    public class SetUpStageViewModel
    {
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public int ID { get; set; }
        public string Name { get; set; }
        public int StageNo { get; set; }
        public float ClosingPercentTage { get; set; }
    }
    public class PartnerViewModel
    {
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public int RelationshipID { get; set; }
        public int NamePartnerID { get; set; }
        public int ID { get; set; }

     public List<SelectListItem> Nameselect { get; set; }

        public string SetupPartner { get; set; }
        public List<SelectListItem> Relationshipselect { get; set; }
        public string SetupRelationship { get; set; }
        public string RelatedBp { get; set; }
        public string Remark { get; set; }
    }
    public class SetupPartnerViewModel
    {
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public int ID { get; set; }
        public string Name { get; set; }
        public List<SelectListItem> DFRelationshipselect { get; set; }
        public string RelatedBp { get; set; }
        [NotMapped]
        public string RelatedBpName { get; set; }
        public int DFRelationship { get; set; }
        public string Detail { get; set; }
    }

    public class SetupRelationshipViewModel
    {
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public int ID { get; set; }
        public string RelationshipDscription { get; set; }
    }
    public class CompetitorViewModel
    {
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public int ID { get; set; }
        public int ThreaLevelID { get; set; }
        public int NameCompetitorID { get; set; }
        public List<SelectListItem> Nameselect { get; set; }

        public string SetupCompetitor { get; set; }
        public List<SelectListItem> ThreaLevel { get; set; }
        public string Remark { get; set; }
      
    }
    public class SetupCompetitorViewModel
    {
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();

        public int ID { get; set; }
        public string Name { get; set; }
        public List<SelectListItem> ThreaLevel { get; set; }
        public int ThreaLevelID { get; set; }
        public string Detail { get; set; }
    }
    public class DescriptionSummaryViewModel{
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public int ID { get; set; }
        public int ReasonsID { get; set; }
        public List<SelectListItem> Descriptionselect { get; set; }

    }
    public class SetupReasonsViewModel
    {
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public int ID { get; set; }
        public string Description { get; set; }
    }
}
