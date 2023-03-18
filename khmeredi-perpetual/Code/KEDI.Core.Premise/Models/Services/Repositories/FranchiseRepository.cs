using CKBS.AppContext;
using KEDI.Core.Premise.Models.ServicesClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.Responsitory
{
    public interface IPushSchedule
    {
        IEnumerable<PushSchedule> GetPushSchedules();
    }
    public class FranchiseRepository
    {
        private readonly DataContext _context;
        public FranchiseRepository(DataContext context)
        {
            _context = context;
        }
        public IEnumerable<PushSchedule> GetPushSchedules()
        {
            var receipt = _context.Receipt.ToList();
            var list = from r in receipt
                       group new { r } by new { r.DateOut } into r
                       let data = r.FirstOrDefault()
                       select new PushSchedule
                       {
                           Date = data.r.DateOut,
                           INVCount = r.Count(),
                           Amount = (decimal)r.Sum(s=>s.r.GrandTotal_Sys),
                       };
            return list;
        }
    }
}
