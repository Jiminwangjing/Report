using CKBS.AppContext;
using CKBS.Models.Services.AlertManagement;
using KEDI.Core.Premise.Models.Services.GeneralSettingAdmin;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.AlertManagementsServices.Repositories
{
    public interface ICheckFrequently
    {
        Task<AlertMaster> AlertManagementAsync(TypeOfAlert typeOfAlert);
        Task<List<AlertDetail>> AlertDetailsAsync(int id);
        Task<int> CheckFrequentAsync(TypeOfAlert typeOfAlert);
        Task<int> CountNotiAsync(int userId);
        Task<int> CheckBFAsync(TypeOfAlert typeOfAlert);
        Task<GeneralServiceSetup> GetGeneralServiceSetup(string code);
        int Interval { get; set; }
    }

    public class CheckFrequently : ICheckFrequently
    {
        private readonly DataContext _context;

        public int Interval { get; set; } = 180000;

        public CheckFrequently(DataContext context)
        {
            _context = context;
        }

        public async Task<List<AlertDetail>> AlertDetailsAsync(int id)
        {
            var data = await _context.AlertDetails.Where(i => i.AlertMasterID == id && i.Active).Select(i => new AlertDetail
            {
                Active = i.Active,
                AlertMasterID = i.AlertMasterID,
                AlertWarehouses = _context.AlertWarehouse.Where(wh => wh.AlertDetailID == i.ID && wh.IsAlert).ToList(),
                ID = i.ID,
                IsAllUsers = i.IsAllUsers,
                IsAllWh = i.IsAllWh,
                UserAlerts = _context.UserAlerts.Where(user => user.AlertDetailID == i.ID && user.IsAlert).ToList(),
            }).ToListAsync();
            return data;
        }

        public async Task<AlertMaster> AlertManagementAsync(TypeOfAlert typeOfAlert)
        {
            var alertMasters = await _context.AlertMasters.Where(c => c.TypeOfAlert == typeOfAlert && c.Active).ToListAsync();
            var alertMaster = alertMasters.Select(i => new AlertMaster
            {
                ID = i.ID,
                Frequently = i.Frequently,
                Active = i.Active,
                BeforeAppDate = i.BeforeAppDate,
                TypeBeforeAppDate = i.TypeBeforeAppDate,
                TypeFrequently = i.TypeFrequently,
            }).FirstOrDefault();
            return alertMaster;
        }

        public async Task<int> CheckFrequentAsync(TypeOfAlert typeOfAlert)
        {
            var Interval = 0;
            var AppID = await AlertManagementAsync(typeOfAlert);
            if (AppID == null)
                return 600000; //1h
            var Frequently = (int)AppID.Frequently;
            var typeFrequently = AppID.TypeFrequently;

            switch (typeFrequently)
            {
                case TypeFrequently.Minutes:
                    Interval = Frequently;
                    break;
                case TypeFrequently.Hours:
                    Interval = Frequently * 60;
                    break;
                case TypeFrequently.Days:
                    Interval = Frequently * 1440;
                    break;
                case TypeFrequently.Weeks:
                    Interval = Frequently * 10080;
                    break;
                case TypeFrequently.Months:
                    Interval = Frequently * 40320;
                    break;
                case TypeFrequently.Years:
                    Interval = Frequently * 483840;
                    break;
            }
            return Interval <= 0 ? 1000 * 60 : Interval * 1000 * 60;
        }

        public async Task<int> CheckBFAsync(TypeOfAlert typeOfAlert)
        {
            var alertMs = await AlertManagementAsync(TypeOfAlert.DueDate);
            var bADate = alertMs.BeforeAppDate;
            var _alertTimeStampMin = 0;
            switch (alertMs.TypeBeforeAppDate)
            {
                case TypeFrequently.Minutes:
                    _alertTimeStampMin = (int)bADate;
                    break;
                case TypeFrequently.Hours:
                    _alertTimeStampMin = (int)bADate * 60;
                    break;
                case TypeFrequently.Days:
                    _alertTimeStampMin = (int)bADate * 1440;
                    break;
                case TypeFrequently.Weeks:
                    _alertTimeStampMin = (int)bADate * 10080;
                    break;
                case TypeFrequently.Months:
                    _alertTimeStampMin = (int)bADate * 40320;
                    break;
                case TypeFrequently.Years:
                    _alertTimeStampMin = (int)bADate * 483840;
                    break;
            }
            return _alertTimeStampMin;
        }
        public async Task<int> CountNotiAsync(int userId)
        {
            var countNotiDueDate = await _context.DueDateAlerts.AsNoTracking().Where(i => !i.IsRead && i.UserID == userId).CountAsync();
            var countNotiStockAlert = await _context.StockAlerts.AsNoTracking().Where(i => !i.IsRead && i.UserID == userId).CountAsync();
            var countNotiCashOutAlert = await _context.CashOutAlerts.AsNoTracking().Where(i => !i.IsRead && i.UserID == userId).CountAsync();
            var countNotiExpirationStockItemsAlert = await _context.ExpirationStockItems.AsNoTracking().Where(i => !i.IsRead && i.UserID == userId).CountAsync();
            var count = countNotiDueDate + countNotiStockAlert + countNotiCashOutAlert + countNotiExpirationStockItemsAlert;
            return count;
        }

        public async Task<GeneralServiceSetup> GetGeneralServiceSetup(string code)
        {
            using var context = new DataContext();
            var data = await context.GeneralServiceSetups.FirstOrDefaultAsync(i => i.Code == code);
            return data;
        }
    }
}
