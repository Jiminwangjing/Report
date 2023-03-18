using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.Services.Banking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CKBS.Models.Services.Administrator.General;
using KEDI.Core.Premise.Models.Services.POS;

namespace CKBS.Models.Services.Responsitory
{
    public interface IPaymentMean
    {
        IQueryable<PaymentMeans> PaymentMeans { get; }
        IEnumerable<PaymentMeans> PaymentMean { get; }
        PaymentMeans GetId(int id);
        Task AddOrEdit(PaymentMeans payment, int companyID);
        Task Delete(int id);
       // Task SetDefault(int id);
    }
    public class PaymentMeanRepository : IPaymentMean
    {
        private readonly DataContext _context;
        public PaymentMeanRepository(DataContext context)
        {
            _context = context;
        }
        
        public IQueryable<PaymentMeans> PaymentMeans => _context.PaymentMeans.Where(d=>d.Delete==false);

        public IEnumerable<PaymentMeans> PaymentMean => _context.PaymentMeans.Where(d => d.Delete == false);

        public async Task AddOrEdit(PaymentMeans payment, int companyID)
        {
            if (payment.ID == 0)
            {
                payment.CompanyID = companyID;
                await _context.AddAsync(payment);
                
            }
            else
            {
                payment.CompanyID = companyID;
                _context.Update(payment);
            }
            await _context.SaveChangesAsync();
          
        }

        public async Task Delete(int id)
        {
            PaymentMeans payment = await _context.PaymentMeans.FirstAsync(w => w.ID == id);
            payment.Delete = true;
            _context.Update(payment);
            await _context.SaveChangesAsync();
        }

        public PaymentMeans GetId(int id)
        {
            PaymentMeans payment = _context.PaymentMeans.Find(id);
            return payment;
        }
        //public async Task SetDefault(int id)
        //{
        //    List<PaymentMeans> warehouses = _context.PaymentMeans.Where(w => w.Delete == false).ToList();
        //    foreach (var item in warehouses)
        //    {
        //        if (item.ID == id)
        //        {
        //            item.Default = true;
        //        }
        //        else
        //        {
        //            item.Default = false;
        //        }
        //        _context.Update(item);
        //        await _context.SaveChangesAsync();
        //    }

        //}
    }
}
