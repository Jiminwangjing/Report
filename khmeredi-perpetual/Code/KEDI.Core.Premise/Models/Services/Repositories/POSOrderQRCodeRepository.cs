using CKBS.AppContext;
using CKBS.Controllers.API.QRCodeV1.ClassView;
using CKBS.Models.Services.Account;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.POS;
using CKBS.Models.Services.POS.service;
using CKBS.Models.Services.POS.Template;
using KEDI.Core.Premise.Repository;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Responsitory
{
    public interface IPOSOrderQRCodeRepository
    {
        Task<OrderInfo> FetchOrdersQR(int decryptedIntId, int orderId = 0, bool setDefaultOrder = false);
        Task<List<ServiceItemSales>> GetGroupItemsAsync(int group1, int group2, int group3, int priceListId, int level = 0, bool onlyAddon = false);
        Task<ItemsReturnObj> Send(Order order, string printType);
        string GenerateQRCode(QRcodeViewModel qRcode);
    }
    public class POSOrderQRCodeRepository : IPOSOrderQRCodeRepository
    {
        private readonly DataContext _context;
        private readonly IPOS _pos;
        private readonly PosRetailModule _posRetail;

        public POSOrderQRCodeRepository(DataContext context, IPOS pos, PosRetailModule posRetail)
        {
            _context = context;
            _pos = pos;
            _posRetail = posRetail;
        }

        public async Task<OrderInfo> FetchOrdersQR(int decryptedIntId, int orderId = 0, bool setDefaultOrder = false)
        {
            var orderInfo = await _posRetail.GetOrderInfoQrAsync(decryptedIntId, orderId, 0, setDefaultOrder);
            return orderInfo;
        }

        public string GenerateQRCode(QRcodeViewModel qRcode)
        {
            QRCodeGenerator _qrCode = new();
            QRCodeData _qrCodeData = _qrCode.CreateQrCode(qRcode.QrCodeString, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new(_qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            string data = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(BitmapToBytesCode(qrCodeImage)));
            return data;
        }

        public async Task<List<ServiceItemSales>> GetGroupItemsAsync(int group1, int group2, int group3, int priceListId, int level = 0, bool onlyAddon = false)
        {
            var itemInfos = await _posRetail.GetGroupItemsAsync(group1, group2, group3, priceListId, GetCompany().ID, level, onlyAddon);
            return itemInfos;
        }

        public async Task<ItemsReturnObj> Send(Order order, string printType)
        {
            using var t = _context.Database.BeginTransaction();
            order.UserOrderID = GetUserID().ID;
            var returnItem = await _pos.SubmitOrderQrAsync(CheckedOrder(order), printType);
            t.Commit();
            return returnItem;
        }

        private static byte[] BitmapToBytesCode(Bitmap image)
        {
            using MemoryStream stream = new();
            image.Save(stream, ImageFormat.Png);
            return stream.ToArray();
        }
        private UserAccount GetUserID()
        {
            var user = _context.UserAccounts.FirstOrDefault(i => i.IsUserOrder) ?? new UserAccount();
            return user;
        }
        private Order CheckedOrder(Order order)
        {
            double total = 0;
            order.OrderDetail.ForEach(od =>
            {
                od.DiscountValue = (od.Qty * od.UnitPrice) * od.DiscountRate / 100;
                od.Total = ((od.Qty * od.UnitPrice) * (1 - (od.DiscountRate / 100)));
                od.Total_Sys = od.Total * order.PLRate;
                total += od.Total;
            });
            order.Sub_Total = total;
            order.DiscountValue = total * (order.DiscountRate / 100);
            order.GrandTotal = total * (1 - order.DiscountRate / 100);
            order.GrandTotal_Sys = order.GrandTotal * order.PLRate;
            order.Change = order.Received - order.GrandTotal;
            if (Setting.VatAble)
            {
                order.TaxValue = order.GrandTotal * (order.TaxRate / 100);
            }
            return order;
        }
        private GeneralSetting Setting
        {
            get
            {
                return _context.GeneralSettings.FirstOrDefault(g => g.UserID == GetUserID().ID) ?? CreateSetting();
            }
        }
        private GeneralSetting CreateSetting(int userId = 0)
        {
            var setting = new GeneralSetting
            {
                UserID = userId <= 0 ? GetUserID().ID : userId,
                BranchID = GetBranchID(),
                CompanyID = GetCompany().ID
            };
            var company = _context.Company.Find(GetCompany().ID);
            if (company != null)
            {
                setting.LocalCurrencyID = company.LocalCurrencyID;
                setting.SysCurrencyID = company.SystemCurrencyID;
            }
            return setting;
        }
        private Company GetCompany()
        {
            var com = (from us in _context.UserAccounts.Where(w => w.ID == GetUserID().ID)
                       join co in _context.Company on us.CompanyID equals co.ID
                       select co
                       ).FirstOrDefault();
            return com;
        }
        private int GetBranchID()
        {
            UserAccount user = _context.UserAccounts.FirstOrDefault(u => u.ID == GetUserID().ID);
            return user.BranchID;
        }
    }
}
