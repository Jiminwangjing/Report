using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.Services.Administrator.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Responsitory
{
    public interface IUOM {
        IQueryable<UnitofMeasure> GetUnitofMeasures();
        Task<int> AddOrEidt(UnitofMeasure unitofMeasure);
        UnitofMeasure getid(int id);
        Task<int> DeleteUOM(int id);
    }
    public class UnitofMeasureResponsitory : IUOM
    {
        private DataContext _contex;
        public UnitofMeasureResponsitory(DataContext dataCotext)
        {
            _contex = dataCotext;
        }

        public async Task<int> AddOrEidt(UnitofMeasure unitofMeasure)
        {
            if (unitofMeasure.ID == 0)
            {
                await _contex.UnitofMeasures.AddAsync(unitofMeasure);
            }
            else
            {
                
                _contex.UnitofMeasures.Update(unitofMeasure);
            }
            return await _contex.SaveChangesAsync();
        }

        public async Task<int> DeleteUOM(int id)
        {
            var unit = await _contex.UnitofMeasures.FirstAsync(c=>c.ID==id);
            unit.Delete = true;
            _contex.UnitofMeasures.Update(unit);
            return await _contex.SaveChangesAsync();
        }

        public UnitofMeasure getid(int id) => _contex.UnitofMeasures.Find(id);
     
        public IQueryable<UnitofMeasure> GetUnitofMeasures()
        {
            IQueryable<UnitofMeasure> list = _contex.UnitofMeasures.Where(u => u.Delete == false);
            
            return list;

        }
    }

}
