using CKBS.AppContext;
using CKBS.Models.Services.CostOfAccounting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Responsitory
{
    public interface ICostOfAccounting
    {
        IEnumerable<dynamic> GetEmps(int comId);
        IEnumerable<dynamic> GetDimensions(int comId);
        IEnumerable<dynamic> GetCCT();
        IEnumerable<dynamic> GetCostOfCenter(int comId);
        IEnumerable<dynamic> GetDimensionsItSelf(int comId);
        dynamic GetNone(int comId);
        dynamic GetGroup(int Id);
        dynamic CreateDetailbyCategory(int Id);
        dynamic GetLatestCCT();
        void CreateOrUpdate(CostOfCenter costOfCenter);
        void CreateCCT(CostOfAccountingType costOfCenterType);
        void GenerateDynamics(int comId);
        void UpdateDimensions(string dimensions);
    }
    public class CostOfAccountingRepository : ICostOfAccounting
    {
        private readonly DataContext _context;

        public CostOfAccountingRepository(DataContext dataContext)
        {
            _context = dataContext;
        }

        public void CreateOrUpdate(CostOfCenter costOfCenter)
        {
            _context.CostOfCenter.Update(costOfCenter);
            _context.SaveChanges();
        }

        public IEnumerable<dynamic> GetCostOfCenter(int comId)
        {
            var costOfCenter = (from cc in _context.CostOfCenter.Where(i => !i.IsDimension && !i.ActiveDimension && i.Active && i.CompanyID == comId && !i.None)
                                join cct in _context.CostOfAccountingTypes on cc.CostOfAccountingTypeID equals cct.ID
                                into g
                                from ccts in g.DefaultIfEmpty()
                                join emp in _context.Employees.Where(i => !i.Delete) on cc.OwnerEmpID equals emp.ID
                                into em 
                                from emps in em.DefaultIfEmpty()
                                select new CostOfCenter
                                {
                                    ID = cc.ID,
                                    ParentID = cc.ParentID,
                                    MainParentID = cc.MainParentID,
                                    OwnerEmpID = cc.OwnerEmpID,
                                    CostOfAccountingTypeID = cc.CostOfAccountingTypeID,
                                    CompanyID = cc.CompanyID,
                                    Level = cc.Level,
                                    OwnerName = emps.Name,
                                    CostOfAccountingType = ccts.CACodeName,
                                    CostCenter = cc.CostCenter,
                                    Name = cc.Name,
                                    ShortCode = cc.ShortCode,
                                    EffectiveFrom = cc.EffectiveFrom,
                                    EffectiveTo = cc.EffectiveTo,
                                    ActiveDimension = cc.ActiveDimension,
                                    IsDimension = cc.IsDimension,
                                    Active = cc.Active,
                                    None = cc.None,
                                }).ToList();
            return costOfCenter;
        }

        public IEnumerable<dynamic> GetDimensions(int comId)
        {
            var dimensions = _context.CostOfCenter.Where(i => i.IsDimension && i.ActiveDimension && i.CompanyID == comId).ToList();
            return dimensions;
        }

        public IEnumerable<dynamic> GetEmps(int comId)
        {
            var emps = _context.Employees.Where(i => !i.Delete && i.CompanyID == comId).ToList();
            return emps;
        }

        public dynamic GetNone(int comId)
        {
            var none = _context.CostOfCenter.FirstOrDefault(i => i.None) ?? new CostOfCenter();
            return none;
        }
        public dynamic GetGroup(int Id)
        {
            var group = _context.CostOfCenter.Find(Id);
            var emp = _context.Employees.Find(group.OwnerEmpID);
            var costCenter = _context.CostOfAccountingTypes.Find(group.CostOfAccountingTypeID);
            group.OwnerName = emp != null ? emp.Name : "";
            group.CostOfAccountingType = costCenter != null ? costCenter.CACodeName : "";
            return group;
        }
        public dynamic CreateDetailbyCategory(int id)
        {
            var category = _context.CostOfCenter.Find(id);
            var __costCenter = new CostOfCenter
            {
                ParentID = category.ID,
                Level = category.Level + 1
            };
            return __costCenter;
        }

        public void CreateCCT(CostOfAccountingType costOfCenterType)
        {
            _context.CostOfAccountingTypes.Update(costOfCenterType);
            _context.SaveChanges();
        }
        public dynamic GetLatestCCT()
        {
            var lastedCCt = _context.CostOfAccountingTypes.OrderByDescending(i => i.ID).FirstOrDefault();
            return lastedCCt;
        }

        public IEnumerable<dynamic> GetCCT()
        {
            var cct = _context.CostOfAccountingTypes.ToList();
            return cct;
        }
        public void GenerateDynamics(int comId)
        {
            var maxLevel = _context.CostOfCenter.Where(i => i.Active && i.CompanyID == comId).Max(i => i.Level);
            var maxdimensionLevel = _context.Dimensions.Max(i=> i.Level);
            var costCenter = _context.CostOfCenter.Where(i => i.Active && i.CompanyID == comId).ToList();
            List<Dimension> dimensions = new List<Dimension>();
            for(var i = 1; i <= maxLevel; i++)
            {
                if (i > maxdimensionLevel)
                {
                    dimensions.Add(new Dimension
                    {
                        Description = $"Dimension {i}",
                        DimensionName = $"Dimension {i}",
                        Level = i,
                    });
                }                
            }
            _context.Dimensions.UpdateRange(dimensions);
            _context.SaveChanges();
        }

        public IEnumerable<dynamic> GetDimensionsItSelf(int comId)
        {
            GenerateDynamics(comId);
            var dimensions = _context.Dimensions.ToList();
            return dimensions;
        }

        public void UpdateDimensions(string dimensions)
        {
            List<Dimension> dimension = JsonConvert.DeserializeObject<List<Dimension>>(dimensions, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            _context.Dimensions.UpdateRange(dimension);
            _context.SaveChanges();
        }
    }
}
