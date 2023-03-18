using CKBS.AppContext;
using KEDI.Core.Premise.Models.Services.RemarkDiscount;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Repository
{
    public interface IRemarkDiscountRepository
    {
        Task<RemarkDiscountItem> GetRemarkDiscountAsync(int id);
        Task<List<RemarkDiscountItem>> GetRemarkDiscountsAsync();
        Task UpdateAsync(RemarkDiscountItem data);
        Task SaveAllAsync(List<RemarkDiscountItem> data);
    }

    public class RemarkDiscountRepository : IRemarkDiscountRepository
    {
        private readonly DataContext _context;

        public RemarkDiscountRepository(DataContext context)
        {
            _context = context;
        }

        public async Task UpdateAsync(RemarkDiscountItem data)
        {
            _context.RemarkDiscounts.Update(data);
            await _context.SaveChangesAsync();
        }
        public async Task<List<RemarkDiscountItem>> GetRemarkDiscountsAsync()
        {
            var data = await _context.RemarkDiscounts.Select(i =>
             new RemarkDiscountItem
             {
                 ID = i.ID,
                 Remark = i.Remark,
                 Active = i.Active
             }).ToListAsync();
            return data;
        }
        public async Task<RemarkDiscountItem> GetRemarkDiscountAsync(int id)
        {
            var data = await _context.RemarkDiscounts.FirstOrDefaultAsync(i => i.ID == id);
            return data;
        }
        public async Task SaveAllAsync(List<RemarkDiscountItem> data)
        {
            _context.UpdateRange(data);
            await _context.SaveChangesAsync();
        }
    }
}
