using CKBS.AppContext;
using CKBS.Models.Services.Promotions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Responsitory
{
    public interface ICardType
    {
        IQueryable<CardType> GetCardTypes { get; }
        Task AddorEdit(CardType cardType);
        Task DeleteCardType(int ID);
        CardType GetId(int ID);
    }
    public class CardTypeResponsitory : ICardType
    {
        private readonly DataContext _context;
        public CardTypeResponsitory(DataContext dataContext)
        {
            _context = dataContext;
        }
        public IQueryable<CardType> GetCardTypes => _context.CardTypes.Where(x => x.Delete == false);

        public async Task AddorEdit(CardType cardType)
        {
            if (cardType.ID == 0)
            {
                _context.CardTypes.Add(cardType);
            }
            else
            {
                _context.CardTypes.Update(cardType);
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCardType(int ID)
        {
            var card = _context.CardTypes.FirstOrDefault(x => x.ID == ID);
            card.Delete = true;
             _context.CardTypes.Update(card);
            await _context.SaveChangesAsync();
        }

        public CardType GetId(int ID) => _context.CardTypes.Find(ID);
        
    }
}
