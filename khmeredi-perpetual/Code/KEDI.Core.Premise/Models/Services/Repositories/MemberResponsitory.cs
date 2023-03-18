using CKBS.AppContext;
using CKBS.Models.Services.Promotions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Responsitory
{
    public interface IMemberCard
    {
        IEnumerable<MemberCard> GetMemberCards();
        void AddorEdit(MemberCard memberCard);
        void DeleteMember(int ID);
    }
    public class MemberResponsitory : IMemberCard
    {
        private readonly DataContext _context;
        public MemberResponsitory(DataContext dataContext)
        {
            _context = dataContext;
        }
        public void AddorEdit(MemberCard memberCard)
        {
            if (memberCard.ID == 0)
            {
                _context.MemberCards.Add(memberCard);
            }
            else
            {
                _context.MemberCards.Update(memberCard);
            }
            _context.SaveChanges();

        }

        public void DeleteMember(int ID)
        {
            var mem = _context.MemberCards.FirstOrDefault(x => x.ID == ID);
            mem.Delete = true;
            _context.MemberCards.Update(mem);
            _context.SaveChanges();
        }

        public IEnumerable<MemberCard> GetMemberCards()
        {
            IEnumerable<MemberCard> list = (
                 from men in _context.MemberCards.Where(x => x.Delete == false)
                 join
                 type in _context.CardTypes.Where(x => x.Delete == false) on
                 men.CardTypeID equals type.ID
                 where men.Delete == false
                 select new MemberCard
                 {
                     ID = men.ID,
                     Ref_No = men.Ref_No,
                     Name = men.Name,
                     Description = men.Description,
                     ExpireDate = men.ExpireDate,
                     CardTypeID = men.CardTypeID,
                     Approve=men.Approve,
                     DateApprove=men.DateApprove,
                     DateCreate=men.DateCreate,
                     CardType = new CardType
                     {
                         Name = type.Name,
                         TypeDis = type.TypeDis,
                         Discount = type.Discount
                     }
                 }
                 ).ToList();
            return list;
        }
    }
}
