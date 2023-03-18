
using CKBS.AppContext;
using CKBS.Models.Services.Administrator.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Responsitory
{
    public interface IReceiptInformation
    {
        IQueryable<ReceiptInformation> GetReceiptInformation();
        IEnumerable<ReceiptInformation> Receipts { get; }
        ReceiptInformation GetID(int id);
        Task AddOrEdit(ReceiptInformation receiptInformation);
        Task Delete(int id);
    }
    public class ReceiptInformationRepository : IReceiptInformation
    {
        private readonly DataContext _Context;
        public ReceiptInformationRepository(DataContext dataContext)
        {
            _Context = dataContext;
        }
        public IQueryable<ReceiptInformation> GetReceiptInformation() {
            IQueryable<ReceiptInformation> list = (
                 from re in _Context.ReceiptInformation
                 join
b in _Context.Branches.Where(x => x.Delete == false) on
re.BranchID equals b.ID
                 select new ReceiptInformation
                 {
                     ID = re.ID,
                     BranchID = re.BranchID,
                     Address = re.Address,
                     EnglishDescription = re.EnglishDescription,
                     KhmerDescription = re.KhmerDescription,
                 
                     Logo = re.Logo,
                     PowerBy = re.PowerBy,
                     Tel1 = re.Tel1,
                     Tel2 = re.Tel2,
                     Title = re.Title,
                     Branch = new Branch
                     {
                         Name = b.Name
                     }

                 }
                );
            return list;
        }

        public IEnumerable<ReceiptInformation> Receipts => throw new NotImplementedException();

        public Task AddOrEdit(ReceiptInformation receiptInformation)
        {
            if (receiptInformation.ID == 0)
            {
                _Context.ReceiptInformation.AddAsync(receiptInformation);
            }
            else
            {
                _Context.ReceiptInformation.Update(receiptInformation);

            }
            return  _Context.SaveChangesAsync();
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public ReceiptInformation GetID(int id)
        {
            throw new NotImplementedException();
        }
    }
}
