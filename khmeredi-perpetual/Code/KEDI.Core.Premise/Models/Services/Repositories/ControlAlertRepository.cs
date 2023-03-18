using CKBS.AlertManagementsServices.Repositories;
using CKBS.AppContext;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.AlertManagement;
using CKBS.Models.ServicesClass.ControlAlertView;
using CKBS.Models.ServicesClass.Notification;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CKBS.Models.ServicesClass.AlertViewClass;
using KEDI.Core.Premise.Models.Services.AlertManagement;
using KEDI.Core.Helpers.Enumerations;

namespace CKBS.Models.Services.Responsitory
{
    public interface IControlAlertRepository
    {
        Task<IEnumerable<AlertMasterViewModel>> GetAlertMastersAsync(int comId);
        Task<AlertMaster> GetAlertMasterAsync(int? id);
        Task AddAsync(AlertMaster alertMaster);
        Company GetCompany(int userId);
        Task<AlertDetail> GetWHUserAsync(int comId, int alertdid = 0, int alertM = 0);
        Task UpdateAlertDetailsAsync(AlertDetail alertDetail);
        Task<IEnumerable<AlertDetail>> GetAlertDetailsAsync(int comId);
        Task<IEnumerable<AlertMaster>> GetAlertMastersAsyncCheck(int comId);
        Task<AlertDetail> GetAlertDetailAsync(int id);
        Task<AlertMaster> CheckTypeAlertAsync(TypeOfAlert typeOfAlert);
        Task<IEnumerable<AlertDetailViewModel>> GetAlertDetailsAsync(int id, int comId);
        Task CheckActiveDeltailAsync(int id, bool active);
        Task<bool> UpdateStockAlertAsync(int id);
        Task<bool> UpdateDueDateAlertAsync(int id);
        Task<bool> UpdateCashOutAlertAsync(int id);
        Task<bool> UpdateExpirationItemAlertAsync(int id);
        Task<CashOutAlertViewModel> GetCashOutAsync(int id);
        Task<ExpirationStockItem> GetExpirationStockItemAsync(int id);
        Task<NotificationViewModel> GetNotificationAsync(int userId);
        Task<NotificationViewModel> GetDueDateAlertAsync(string typeRead, string typeOrder, int userId, bool isClear = false);
        Task<NotificationViewModel> GetCashOutAlertAsync(string typeRead, string typeOrder, int userId, bool isClear = false);
        Task<NotificationViewModel> GetStockAlertAsync(string typeRead, string typeOrder, int userId, bool isClear = false);
        Task<NotificationViewModel> GetExpirationStockItemAlertAsync(string typeRead, string typeOrder, int userId, bool isClear = false);
    }
    public class ControlAlertRepository : IControlAlertRepository
    {
        private readonly DataContext _context;
        private readonly ICheckFrequently _check;

        public ControlAlertRepository(DataContext context, ICheckFrequently check)
        {
            _context = context;
            _check = check;
        }
        private static Dictionary<int, string> TypeOfAlert => EnumHelper.ToDictionary(typeof(TypeOfAlert));
        public async Task AddAsync(AlertMaster alertMaster)
        {
            _context.AlertMasters.Update(alertMaster);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<AlertMasterViewModel>> GetAlertMastersAsync(int comId)
        {
            //var data = await _context.AlertMasters.ToListAsync();
            var data = await (from am in _context.AlertMasters.Where(i => i.CompanyID == comId)
                              select new AlertMasterViewModel
                              {
                                  CompanyID = am.CompanyID,
                                  Code = am.Code,
                                  Active = am.Active,
                                  BeforeAppDate = am.BeforeAppDate,
                                  DeleteAlert = am.DeleteAlert,
                                  Frequently = am.Frequently,
                                  ID = am.ID,
                                  TypeBeforeAppDate = am.TypeBeforeAppDate,
                                  TypeFrequently = am.TypeFrequently,
                                  TypeOfAlert = am.TypeOfAlert,
                                  TypeOfAlerts = TypeOfAlert.Select(i => new SelectListItem
                                  {
                                      Disabled = true,
                                      Selected = i.Key == (int)am.TypeOfAlert,
                                      Text = i.Value,
                                      Value = i.Key.ToString(),
                                  }).ToList(),
                              }).ToListAsync();
            return data;
        }
        public async Task<AlertDetail> GetWHUserAsync(int comId, int alertdid = 0, int alertM = 0)
        {
            //var data = await (from ad in _);
            var wh = await _context.Warehouses.Where(i => !i.Delete).Select(i => new AlertWarehouses
            {
                CompanyID = comId,
                Code = i.Code,
                ID = 0,
                IsAlert = false,
                Location = i.Location,
                Name = i.Name,
                AlertDetailID = alertdid,
                WarehouseID = i.ID
            }).ToListAsync();
            var users = await _context.UserAccounts.Where(i => !i.Delete && i.CompanyID == comId).Select(i => new UserAlert
            {
                ID = 0,
                AlertDetailID = alertdid,
                CompanyID = comId,
                IsAlert = false,
                UserAccountID = i.ID,
                UserName = i.Username,
                TelegramUserID = i.TelegramUserID,
            }).ToListAsync();

            var alertDetail = new AlertDetail
            {
                ID = 0,
                Active = false,
                AlertMasterID = alertM,
                AlertWarehouses = wh,
                Code = "",
                CompanyID = comId,
                Name = "",
                UserAlerts = users,
            };
            return alertDetail;
        }
        public Company GetCompany(int userId)
        {
            var com = (from us in _context.UserAccounts.Where(w => w.ID == userId)
                       join co in _context.Company on us.CompanyID equals co.ID
                       select co
                       ).FirstOrDefault();
            return com;
        }
        public async Task UpdateAlertDetailsAsync(AlertDetail alertDetail)
        {
            _context.AlertDetails.Update(alertDetail);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<AlertDetail>> GetAlertDetailsAsync(int comId)
        {
            var data = await _context.AlertDetails.AsNoTracking().Where(i => i.CompanyID == comId).ToListAsync();
            return data;
        }
        public async Task<AlertDetail> GetAlertDetailAsync(int id)
        {
            var data = await _context.AlertDetails.AsNoTracking().FirstOrDefaultAsync(i => i.ID == id);
            return data;
        }
        public async Task<AlertMaster> GetAlertMasterAsync(int? id)
        {
            var data = await _context.AlertMasters.AsNoTracking().FirstOrDefaultAsync(i => i.ID == id);
            return data;
        }
        public async Task<AlertMaster> CheckTypeAlertAsync(TypeOfAlert typeOfAlert)
        {
            var data = await _context.AlertMasters.FirstOrDefaultAsync(i => i.TypeOfAlert == typeOfAlert) ?? new AlertMaster();
            return data;
        }
        public async Task<IEnumerable<AlertMaster>> GetAlertMastersAsyncCheck(int comId)
        {
            var data = await _context.AlertMasters.Where(i => i.CompanyID == comId).ToListAsync();
            return data;
        }
        public async Task<IEnumerable<AlertDetailViewModel>> GetAlertDetailsAsync(int id, int comId)
        {
            var master = await _context.AlertMasters.FindAsync(id) ?? new AlertMaster();
            var data = await (from ad in _context.AlertDetails.Where(i => i.CompanyID == comId && i.AlertMasterID == id)
                              let wh = _context.AlertWarehouse.Where(i => i.CompanyID == comId && i.AlertDetailID == ad.ID).ToList()
                              let user = _context.UserAlerts.Where(i => i.CompanyID == comId && i.AlertDetailID == ad.ID).ToList()
                              select new AlertDetailViewModel
                              {
                                  UserAlerts = user,
                                  ID = ad.ID,
                                  Active = ad.Active,
                                  AlertMasterID = id,
                                  AlertWarehouses = wh,
                                  Code = ad.Code,
                                  CompanyID = ad.CompanyID,
                                  IsAllUsers = ad.IsAllUsers,
                                  IsAllWh = ad.IsAllWh,
                                  Name = ad.Name,
                                  TypeOfAlert = master.TypeOfAlert
                              }).ToListAsync();
            return data;
        }

        public async Task CheckActiveDeltailAsync(int id, bool active)
        {
            var data = await GetAlertDetailAsync(id);
            data.Active = active;
            _context.AlertDetails.Update(data);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateStockAlertAsync(int id)
        {
            var data = _context.StockAlerts.Find(id);
            if (data != null)
            {
                if (!data.IsRead)
                {
                    data.IsRead = true;
                    _context.StockAlerts.Update(data);
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            else return false;
        }

        public async Task<bool> UpdateDueDateAlertAsync(int id)
        {
            var data = _context.DueDateAlerts.Find(id);
            if (data != null)
            {
                if (!data.IsRead)
                {
                    data.IsRead = true;
                    _context.DueDateAlerts.Update(data);
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            else return false;
        }

        public async Task<NotificationViewModel> GetNotificationAsync(int userId)
        {
            var noti = new NotificationViewModel();
            var stock = new List<StockAlertViewModel>();
            var dueDate = new List<DueDateAlertViewModel>();
            var cashout = new List<CashOutAlertViewModel>();
            var expirationStockItems = new List<ExpirationStockItem>();
            stock = await (from sa in _context.StockAlerts.AsNoTracking().Where(i => i.UserID == userId)
                           join item in _context.ItemMasterDatas.AsNoTracking().Where(i => !i.Delete) on sa.ItemID equals item.ID
                           select new StockAlertViewModel
                           {
                               ID = sa.ID,
                               ItemID = sa.ItemID,
                               CreatedAt = sa.CreatedAt,
                               Image = item.Image ?? "",
                               InStock = sa.InStock,
                               IsRead = sa.IsRead,
                               ItemName = item.KhmerName,
                               Type = sa.Type,
                               WarehouseName = sa.WarehouseName
                           }).OrderBy(i => i.CreatedAt).ToListAsync();
            noti.CountStock = stock.Where(i => !i.IsRead).Count();
            noti.Stock = stock;

            dueDate = await (from dd in _context.DueDateAlerts.AsNoTracking().Where(i => i.UserID == userId)
                             join bp in _context.BusinessPartners.AsNoTracking().Where(i => !i.Delete) on dd.BPID equals bp.ID
                             select new DueDateAlertViewModel
                             {
                                 ID = dd.ID,
                                 BPID = dd.BPID,
                                 CreatedAt = dd.CreatedAt,
                                 IsRead = dd.IsRead,
                                 Name = bp.Name,
                                 Type = dd.Type,
                                 DueDate = dd.DueDate,
                                 DueDateType = dd.DueDateType,
                                 InvoiceNumber = dd.InvoiceNumber,
                                 TimeLeft = dd.TimeLeft,
                                 InvoiceID = dd.InvoiceID,
                             }).OrderBy(i => i.CreatedAt).ToListAsync();
            noti.CountDueDate = dueDate.Where(i => !i.IsRead).Count();
            noti.DueDate = dueDate;
            cashout = await (from co in _context.CashOutAlerts.AsNoTracking().Where(i => i.UserID == userId)
                             join branch in _context.Branches on co.BrandID equals branch.ID
                             join emp in _context.Employees on co.EmpID equals emp.ID
                             join cur in _context.Currency on co.CurrencyID equals cur.ID
                             select new CashOutAlertViewModel
                             {
                                 ID = co.ID,
                                 Currency = cur.Description,
                                 CreatedAt = co.CreatedAt,
                                 BrandName = branch.Name,
                                 CashInAmountSys = co.CashInAmountSys,
                                 DateIn = co.DateIn,
                                 DateOut = co.DateOut,
                                 EmpName = emp.Name,
                                 GrandTotal = co.GrandTotal,
                                 IsRead = co.IsRead,
                                 SaleAmountSys = co.SaleAmountSys,
                                 TimeIn = co.TimeIn,
                                 TimeOut = co.TimeOut,

                             }).ToListAsync();
            noti.CashOuts = cashout;
            noti.CountCashOut = cashout.Where(i => !i.IsRead).Count();


            expirationStockItems = await (from exSI in _context.ExpirationStockItems.AsNoTracking().Where(i => i.UserID == userId)
                                          join whd in _context.WarehouseDetails on exSI.WareDId equals whd.ID
                                          join item in _context.ItemMasterDatas on exSI.ItemId equals item.ID
                                          join uom in _context.UnitofMeasures on exSI.UomID equals uom.ID
                                          join ware in _context.Warehouses on exSI.WarehouseId equals ware.ID
                                          select new ExpirationStockItem
                                          {
                                              ID = exSI.ID,
                                              UomID = uom.ID,
                                              AddmissionDate = whd.AdmissionDate,
                                              BatchAttribute1 = whd.BatchAttr1,
                                              BatchAttribute2 = whd.BatchAttr2,
                                              BatchNo = whd.BatchNo,
                                              CompanyID = exSI.CompanyID,
                                              CreatedDate = exSI.CreatedDate,
                                              ExpirationDate = whd.ExpireDate,
                                              ImageItem = item.Image ?? "",
                                              Instock = (decimal)whd.InStock,
                                              IsRead = exSI.IsRead,
                                              ItemBarcode = item.Barcode,
                                              ItemCode = item.Code,
                                              ItemId = item.ID,
                                              ItemName = item.KhmerName,
                                              MfrDate = whd.MfrDate,
                                              Type = exSI.Type,
                                              UomName = uom.Name,
                                              UserID = exSI.UserID,
                                              WareDId = whd.ID,
                                              WarehouseId = ware.ID,
                                              WarehouseName = ware.Name
                                          }).ToListAsync();
            noti.ExpirationItems = expirationStockItems;
            noti.CountExpirationItem = expirationStockItems.Where(i => !i.IsRead).Count();
            noti.CountNoti = await _check.CountNotiAsync(userId);
            return noti;
        }

        public async Task<bool> UpdateCashOutAlertAsync(int id)
        {
            var data = _context.CashOutAlerts.Find(id);
            if (data != null)
            {
                if (!data.IsRead)
                {
                    data.IsRead = true;
                    _context.CashOutAlerts.Update(data);
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            else return false;
        }
        public async Task<bool> UpdateExpirationItemAlertAsync(int id)
        {
            var data = _context.ExpirationStockItems.Find(id);
            if (data != null)
            {
                if (!data.IsRead)
                {
                    data.IsRead = true;
                    _context.ExpirationStockItems.Update(data);
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            else return false;
        }
        public async Task<CashOutAlertViewModel> GetCashOutAsync(int id)
        {
            CashOutAlertViewModel cashOutAlert = await (from co in _context.CashOutAlerts.AsNoTracking().Where(i => i.ID == id)
                                                        join branch in _context.Branches on co.BrandID equals branch.ID
                                                        join emp in _context.Employees on co.EmpID equals emp.ID
                                                        join cur in _context.Currency on co.CurrencyID equals cur.ID
                                                        select new CashOutAlertViewModel
                                                        {
                                                            ID = co.ID,
                                                            Currency = cur.Description,
                                                            CreatedAt = co.CreatedAt,
                                                            BrandName = branch.Name,
                                                            CashInAmountSys = co.CashInAmountSys,
                                                            DateIn = co.DateIn,
                                                            DateOut = co.DateOut,
                                                            EmpName = emp.Name,
                                                            GrandTotal = co.GrandTotal,
                                                            IsRead = co.IsRead,
                                                            SaleAmountSys = co.SaleAmountSys,
                                                            TimeIn = co.TimeIn,
                                                            TimeOut = co.TimeOut,
                                                        }).FirstOrDefaultAsync();
            return cashOutAlert;
        }

        public async Task<ExpirationStockItem> GetExpirationStockItemAsync(int id)
        {
            ExpirationStockItem expirationStockItem = await (from exSI in _context.ExpirationStockItems.AsNoTracking().Where(i => i.ID == id)
                                                             join whd in _context.WarehouseDetails on exSI.WareDId equals whd.ID
                                                             join item in _context.ItemMasterDatas on exSI.ItemId equals item.ID
                                                             join uom in _context.UnitofMeasures on exSI.UomID equals uom.ID
                                                             join ware in _context.Warehouses on exSI.WarehouseId equals ware.ID
                                                             select new ExpirationStockItem
                                                             {
                                                                 ID = exSI.ID,
                                                                 UomID = uom.ID,
                                                                 AddmissionDate = whd.AdmissionDate,
                                                                 BatchAttribute1 = whd.BatchAttr1,
                                                                 BatchAttribute2 = whd.BatchAttr2,
                                                                 BatchNo = whd.BatchNo,
                                                                 CompanyID = exSI.CompanyID,
                                                                 CreatedDate = exSI.CreatedDate,
                                                                 ExpirationDate = whd.ExpireDate,
                                                                 ImageItem = item.Image ?? "",
                                                                 Instock = (decimal)whd.InStock,
                                                                 IsRead = exSI.IsRead,
                                                                 ItemBarcode = item.Barcode,
                                                                 ItemCode = item.Code,
                                                                 ItemId = item.ID,
                                                                 ItemName = item.KhmerName,
                                                                 MfrDate = whd.MfrDate,
                                                                 Type = exSI.Type,
                                                                 UomName = uom.Name,
                                                                 UserID = exSI.UserID,
                                                                 WareDId = whd.ID,
                                                                 WarehouseId = ware.ID,
                                                                 WarehouseName = ware.Name
                                                             }).FirstOrDefaultAsync();
            return expirationStockItem;
        }

        public async Task<NotificationViewModel> GetDueDateAlertAsync(string typeRead, string typeOrder, int userId, bool isClear = false)
        {
            var noti = new NotificationViewModel();
            var dueDate = new List<DueDateAlertViewModel>();
            bool isread = typeRead == "read";
            dueDate = await (from dd in _context.DueDateAlerts.AsNoTracking().Where(i => i.UserID == userId)
                             join bp in _context.BusinessPartners.AsNoTracking().Where(i => !i.Delete) on dd.BPID equals bp.ID
                             select new DueDateAlertViewModel
                             {
                                 ID = dd.ID,
                                 BPID = dd.BPID,
                                 CreatedAt = dd.CreatedAt,
                                 IsRead = dd.IsRead,
                                 Name = bp.Name,
                                 Type = dd.Type,
                                 DueDate = dd.DueDate,
                                 DueDateType = dd.DueDateType,
                                 InvoiceNumber = dd.InvoiceNumber,
                                 TimeLeft = dd.TimeLeft,
                                 InvoiceID = dd.InvoiceID,
                             }).OrderBy(i => i.CreatedAt).ToListAsync();
            noti.CountDueDate = dueDate.Where(i => !i.IsRead).Count();
            noti.CountNoti = await _check.CountNotiAsync(userId);
            if (!isClear)
            {
                if (typeOrder == "asorder")
                {
                    dueDate = dueDate.Where(i => i.IsRead == isread).OrderBy(i => i.CreatedAt).ToList();
                }
                else if (typeOrder == "deasorder")
                {
                    dueDate = dueDate.Where(i => i.IsRead == isread).OrderByDescending(i => i.CreatedAt).ToList();
                }
                noti.DueDate = dueDate;
                return noti;
            }
            else
            {
                noti.DueDate = dueDate;
                return noti;
            }

        }
        public async Task<NotificationViewModel> GetStockAlertAsync(string typeRead, string typeOrder, int userId, bool isClear = false)
        {
            var noti = new NotificationViewModel();
            var stock = new List<StockAlertViewModel>();
            bool isread = typeRead == "read";
            stock = await (from sa in _context.StockAlerts.AsNoTracking().Where(i => i.UserID == userId)
                           join item in _context.ItemMasterDatas.AsNoTracking().Where(i => !i.Delete) on sa.ItemID equals item.ID
                           select new StockAlertViewModel
                           {
                               ID = sa.ID,
                               ItemID = sa.ItemID,
                               CreatedAt = sa.CreatedAt,
                               Image = item.Image ?? "",
                               InStock = sa.InStock,
                               IsRead = sa.IsRead,
                               ItemName = item.KhmerName,
                               Type = sa.Type,
                               WarehouseName = sa.WarehouseName
                           }).OrderBy(i => i.CreatedAt).ToListAsync();
            noti.CountStock = stock.Where(i => !i.IsRead).Count();
            noti.CountNoti = await _check.CountNotiAsync(userId);
            if (!isClear)
            {
                if (typeOrder == "asorder")
                {
                    stock = stock.Where(i => i.IsRead == isread).OrderBy(i => i.CreatedAt).ToList();
                }
                else if (typeOrder == "deasorder")
                {
                    stock = stock.Where(i => i.IsRead == isread).OrderByDescending(i => i.CreatedAt).ToList();
                }
                noti.Stock = stock;
                return noti;
            }
            else
            {
                noti.Stock = stock;
                return noti;
            }
        }
        public async Task<NotificationViewModel> GetCashOutAlertAsync(string typeRead, string typeOrder, int userId, bool isClear = false)
        {
            var noti = new NotificationViewModel();
            var cashout = new List<CashOutAlertViewModel>();
            bool isread = typeRead == "read";
            cashout = await (from co in _context.CashOutAlerts.AsNoTracking().Where(i => i.UserID == userId)
                             join branch in _context.Branches on co.BrandID equals branch.ID
                             join emp in _context.Employees on co.EmpID equals emp.ID
                             join cur in _context.Currency on co.CurrencyID equals cur.ID
                             select new CashOutAlertViewModel
                             {
                                 ID = co.ID,
                                 Currency = cur.Description,
                                 CreatedAt = co.CreatedAt,
                                 BrandName = branch.Name,
                                 CashInAmountSys = co.CashInAmountSys,
                                 DateIn = co.DateIn,
                                 DateOut = co.DateOut,
                                 EmpName = emp.Name,
                                 GrandTotal = co.GrandTotal,
                                 IsRead = co.IsRead,
                                 SaleAmountSys = co.SaleAmountSys,
                                 TimeIn = co.TimeIn,
                                 TimeOut = co.TimeOut,

                             }).ToListAsync();
            noti.CountCashOut = cashout.Where(i => !i.IsRead).Count();
            noti.CountNoti = await _check.CountNotiAsync(userId);
            if (!isClear)
            {
                if (typeOrder == "asorder")
                {
                    cashout = cashout.Where(i => i.IsRead == isread).OrderBy(i => i.CreatedAt).ToList();
                }
                else if (typeOrder == "deasorder")
                {
                    cashout = cashout.Where(i => i.IsRead == isread).OrderByDescending(i => i.CreatedAt).ToList();
                }
                noti.CashOuts = cashout;
                return noti;
            }
            else
            {
                noti.CashOuts = cashout;
                return noti;
            }
        }

        public async Task<NotificationViewModel> GetExpirationStockItemAlertAsync(string typeRead, string typeOrder, int userId, bool isClear = false)
        {
            var noti = new NotificationViewModel();
            var expirationStockItems = new List<ExpirationStockItem>();
            bool isread = typeRead == "read";
            expirationStockItems = await (from exSI in _context.ExpirationStockItems.AsNoTracking().Where(i => i.UserID == userId)
                                          join whd in _context.WarehouseDetails on exSI.WareDId equals whd.ID
                                          join item in _context.ItemMasterDatas on exSI.ItemId equals item.ID
                                          join uom in _context.UnitofMeasures on exSI.UomID equals uom.ID
                                          join ware in _context.Warehouses on exSI.WarehouseId equals ware.ID
                                          select new ExpirationStockItem
                                          {
                                              ID = exSI.ID,
                                              UomID = uom.ID,
                                              AddmissionDate = whd.AdmissionDate,
                                              BatchAttribute1 = whd.BatchAttr1,
                                              BatchAttribute2 = whd.BatchAttr2,
                                              BatchNo = whd.BatchNo,
                                              CompanyID = exSI.CompanyID,
                                              CreatedDate = exSI.CreatedDate,
                                              ExpirationDate = whd.ExpireDate,
                                              ImageItem = item.Image ?? "",
                                              Instock = (decimal)whd.InStock,
                                              IsRead = exSI.IsRead,
                                              ItemBarcode = item.Barcode,
                                              ItemCode = item.Code,
                                              ItemId = item.ID,
                                              ItemName = item.KhmerName,
                                              MfrDate = whd.MfrDate,
                                              Type = exSI.Type,
                                              UomName = uom.Name,
                                              UserID = exSI.UserID,
                                              WareDId = whd.ID,
                                              WarehouseId = ware.ID,
                                              WarehouseName = ware.Name
                                          }).OrderBy(i => i.CreatedDate).ToListAsync();
            noti.CountExpirationItem = expirationStockItems.Where(i => !i.IsRead).Count();
            noti.CountNoti = await _check.CountNotiAsync(userId);
            if (!isClear)
            {
                if (typeOrder == "asorder")
                {
                    expirationStockItems = expirationStockItems.Where(i => i.IsRead == isread).OrderBy(i => i.CreatedDate).ToList();
                }
                else if (typeOrder == "deasorder")
                {
                    expirationStockItems = expirationStockItems.Where(i => i.IsRead == isread).OrderByDescending(i => i.CreatedDate).ToList();
                }
                noti.ExpirationItems = expirationStockItems;
                return noti;
            }
            else
            {
                noti.ExpirationItems = expirationStockItems;
                return noti;
            }
        }
        //public async Task<dynamic> GetDueDateAsync(int invoiceId, DueDateType dueDateType)
        //{
        //    if(dueDateType == DueDateType.Vendor)
        //    {
        //        var minvoice = _context.Pur
        //    }
        //}
    }
}
