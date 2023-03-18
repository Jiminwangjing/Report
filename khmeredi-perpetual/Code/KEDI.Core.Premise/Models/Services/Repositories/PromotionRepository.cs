
using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.Inventory;
using CKBS.Models.Services.Promotions;
using CKBS.Models.ServicesClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Responsitory
{
    public interface IPromotion
    {
        IQueryable<Promotion> Promotions { get; }
        Promotion GetID(int id);
        Task AddOrEdit(Promotion promotion);
        Task Delete(int id);
        IEnumerable<Point> GetPoints();
        Point GetidPoint(int id);
        void AddorEditPoint(PointService pointService);
        Task DeletePoint(int id);
        IEnumerable<ServicePointDetail> ServicePointDetails { get; }
        IEnumerable<PointDetail> GetPointDetails();
        void DeletePointDelail(int ID);
        Task SetActive(int ID);
        
    }
    public class PromotionRepository : IPromotion
    {
        private readonly DataContext _context;
        public PromotionRepository(DataContext dataContext)
        {
            _context = dataContext;
        }
        public IQueryable<Promotion> Promotions => _context.Promotions;

        public IEnumerable<ServicePointDetail> ServicePointDetails => _context.ServicePointDetails.FromSql("sp_GetPointDetail");

       

        public async Task AddOrEdit(Promotion promotion)
        {
            promotion.Active = true;
            if (promotion.ID == 0)
            {
                await _context.Promotions.AddAsync(promotion);
            }
            else
            {
                _context.Promotions.Update(promotion);
            }
            await _context.SaveChangesAsync();
        }

        public void AddorEditPoint(PointService pointService)
        {
            Point Addpoint;
            PointDetail Addpointdetail;
            if (pointService.PointID == 0)
            {
                Addpoint = new Point();
                Addpoint.Amount = pointService.Amount;
                Addpoint.Points = pointService.Point;
                Addpoint.Quantity = pointService.SetPoint;
            
               _context.Points.Add(Addpoint);                
               _context.SaveChanges();
                int ID = Addpoint.ID;
                foreach (var item in pointService.ServicePointDetails)
                {
                    Addpointdetail = new PointDetail();
                    Addpointdetail.PointID = ID;
                    Addpointdetail.ItemID = item.ItemID;
                    Addpointdetail.Price = item.UnitPrice;
                    Addpointdetail.Qty = item.Qty;
                    Addpointdetail.UomID = item.UomID;
                    Addpointdetail.CurrencyID = item.CurrencyID;
                    _context.PointDetails.Add(Addpointdetail);
                   _context.SaveChanges();
                }
            }
            else
            {
                var point = _context.Points.FirstOrDefault(x => x.ID == pointService.PointID);
                point.Amount = pointService.Amount;
                point.Points = pointService.Point;
                point.Quantity = pointService.SetPoint;
                _context.Points.Update(point);
                _context.SaveChanges();
                int ID = point.ID;
                foreach (var item in pointService.ServicePointDetails)
                {
                    var detail = _context.PointDetails.FirstOrDefault(x => x.ID == item.ID);
                    if (detail != null)
                    {
                        detail.PointID = ID;
                        detail.ItemID = item.ItemID;
                        detail.Qty = item.Qty;
                        detail.Price = item.UnitPrice;
                        detail.CurrencyID = item.CurrencyID;
                        detail.UomID = item.UomID;
                        _context.PointDetails.Update(detail);
                        _context.SaveChanges();
                    }
                }
            }
           
        }

        public async Task Delete(int id)
        {
            var promo = _context.Promotions.FirstOrDefault(x => x.ID == id);
            if (promo.Active == true)
            {
                promo.Active = false;
                _context.Update(promo);
            }
            else
            {
                promo.Active = true;
                _context.Update(promo);
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeletePoint(int id)
        {
            var po = _context.Points.FirstOrDefault(x => x.ID == id);
            po.Delete = true;
            _context.Update(po);
            await _context.SaveChangesAsync();
        }

        public void DeletePointDelail(int ID)
        {
            var pointdetail = _context.PointDetails.FirstOrDefault(x => x.ID == ID);
            if (pointdetail != null)
            {
                pointdetail.Delete = true;
                _context.PointDetails.Update(pointdetail);
                _context.SaveChanges();
            }
        }

        public Promotion GetID(int id) => _context.Promotions.Find(id);

        public Point GetidPoint(int id) => _context.Points.Find(id);

        public IEnumerable<PointDetail> GetPointDetails()
        {
            IEnumerable<PointDetail> list = (from pd in _context.PointDetails.Where(x => x.Delete == false)
                                             join p in _context.Points.Where(x => x.Delete == false) on
                                             pd.PointID equals p.ID
                                             join cr in _context.Currency.Where(x => x.Delete == false) on
                                             pd.CurrencyID equals cr.ID
                                             join item in _context.ItemMasterDatas.Where(x => x.Delete == false) on
                                             pd.ItemID equals item.ID
                                             join uom in _context.UnitofMeasures.Where(x => x.Delete == false) on
                                             item.SaleUomID equals uom.ID
                                             where pd.Delete == false && p.ID==pd.PointID
                                             select new PointDetail
                                             {
                                                 ID = pd.ID,
                                                 PointID = pd.PointID,
                                                 CurrencyID = pd.CurrencyID,
                                                 ItemID = pd.ItemID,
                                                 UomID = pd.UomID,
                                                 Price = pd.Price,
                                                 Qty = pd.Qty,
                                                 ItemMasterData = new ItemMasterData
                                                 {
                                                     Code = item.Code,
                                                     KhmerName = item.KhmerName,
                                                     EnglishName = item.EnglishName,
                                                     Barcode = item.Barcode
                                                 },
                                                 Currency = new Currency
                                                 {
                                                     Description = cr.Description,
                                                     Symbol = cr.Symbol
                                                 },
                                                 Point = new Point
                                                 {
                                                     Quantity = p.Quantity
                                                 },
                                                 UnitofMeasure = new UnitofMeasure
                                                 {
                                                     Name = uom.Name
                                                 }

                                             }
                                           );
            return list;
        }

        public IEnumerable<Point> GetPoints() => _context.Points.Where(x => x.Delete == false);

        public async Task SetActive(int ID)
        {
            var promotion = _context.Promotions.FirstOrDefault(x => x.ID == ID);
            if (promotion != null)
            {
                if (promotion.Active == true)
                {
                    promotion.Active = false;
                }
                else
                {
                    promotion.Active = true;
                }
                _context.Promotions.Update(promotion);
               await _context.SaveChangesAsync();

            }
        }
    }
}
