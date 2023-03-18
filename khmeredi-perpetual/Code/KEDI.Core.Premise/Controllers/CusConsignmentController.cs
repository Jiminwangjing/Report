using System.Linq;
using System.Threading.Tasks;
using CKBS.AppContext;
using KEDI.Core.Premise.Models.Services.CustomerConsignments;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CKBS.Models.Services.POS;
using System.Collections.Generic;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.ServicesClass.GoodsIssue;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using CKBS.Models.Services.Administrator.SystemInitialization;
using KEDI.Core.Premise.Models.Services.POS;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.ChartOfAccounts;
using CKBS.Models.Services.HumanResources;
using CKBS.Models.Services.Inventory;

namespace KEDI.Core.Premise.Controllers
{
    public class CusConsignmentController : Controller
    {
        private readonly DataContext _context;

        public CusConsignmentController(DataContext context)
        {
            _context = context;
        }

        public IActionResult GetSeries()
        {
            var list = _context.Series.Where(s => s.DocuTypeID == 16).ToList();
            return Ok(list);
        }

        public IActionResult GetNumberSeries()
        {
            var docType = _context.DocumentTypes.FirstOrDefault(s => s.Code == "GR") ?? new DocumentType();
            var number = _context.Series.FirstOrDefault(s => s.DocuTypeID == docType.ID) ?? new Series();
            return Ok(number.NextNo);
        }

        [HttpGet]
        public IActionResult FindReceipt(string number, int seriesid)
        {
            var master = _context.Receipt.Where(s => s.ReceiptNo == number && s.SeriesID == seriesid).ToList();

            if (master.Count > 0)
            {
                var list = (from r in master
                            join rd in _context.ReceiptDetail on r.ReceiptID equals rd.ReceiptID

                            let cus = _context.BusinessPartners.FirstOrDefault(s => s.ID == r.CustomerID) ?? new BusinessPartner()
                            let w = _context.Warehouses.FirstOrDefault(s => s.ID == r.WarehouseID) ?? new Warehouse()
                            let item = _context.ItemMasterDatas.FirstOrDefault(s => s.ID == rd.ItemID) ?? new ItemMasterData()
                            select new
                            {
                                LineID = rd.ID,
                                ItemID = rd.ItemID,
                                Code = rd.Code,
                                Name = rd.KhmerName,
                                Uom = (from GDU in _context.GroupDUoMs.Where(i => i.GroupUoMID == item.GroupUomID)
                                       join UNM in _context.UnitofMeasures on GDU.AltUOM equals UNM.ID
                                       select new UOMSViewModel
                                       {
                                           BaseUoMID = GDU.BaseUOM,
                                           Factor = GDU.Factor,
                                           ID = UNM.ID,
                                           Name = UNM.Name
                                       }).Select(c => new SelectListItem
                                       {
                                           Value = c.ID.ToString(),
                                           Text = c.Name,
                                           Selected = c.ID == rd.UomID
                                       }).ToList(),
                                UomID = rd.UomID,
                                Qty = rd.Qty,
                                DisplayQty = rd.Qty,
                                Status = StatusSelect.Open,
                                Warehouse = w.Name,
                                Customer = cus.Name,
                                ReceiptNo = r.ReceiptNo,
                            }).ToList();
                return Ok(list);
            }
            return Ok();
        }

        [HttpGet]
        public IActionResult GetCustomerItemDetails(int customer, int warehouse)
        {
            var master = _context.CustomerConsignments.Where(s => s.CustomerID == customer && s.WarehouseID == warehouse && s.Status == StatusSelect.Open).ToList();

            if (master.Count > 0)
            {
                var list = (from cn in master
                            join cns in _context.CustomerConsignmentDetails.Where(s => s.OpenQty > 0) on cn.CustomerConsignmentID equals cns.CustomerConsignmentID
                            join uom in _context.UnitofMeasures on cns.UomID equals uom.ID
                            join item in _context.ItemMasterDatas on cns.ItemID equals item.ID

                            let r = _context.Receipt.FirstOrDefault(s => s.ReceiptID == cn.InvoiceID) ?? new Receipt()
                            let rd = _context.ReceiptDetail.FirstOrDefault(s => s.ItemID == cns.ItemID && s.ReceiptID == cn.InvoiceID) ?? new ReceiptDetail()
                            let mp = _context.MultiPaymentMeans.FirstOrDefault(s => s.ReceiptID == r.ReceiptID) ?? new MultiPaymentMeans()
                            let pment = _context.PaymentMeans.FirstOrDefault(s => s.ID == mp.PaymentMeanID) ?? new PaymentMeans()
                            let acc = _context.GLAccounts.FirstOrDefault(s => s.ID == pment.AccountID) ?? new GLAccount()
                            let cur = _context.Currency.FirstOrDefault(s => s.ID == r.SysCurrencyID) ?? new Currency()
                            let docType = _context.DocumentTypes.FirstOrDefault(s => s.Code == "GR") ?? new DocumentType()
                            let series = _context.Series.FirstOrDefault(s => s.DocuTypeID == docType.ID) ?? new Series()
                            select new
                            {
                                DetailID = cns.CustomerConsignmentDetailID,
                                ID = cns.CustomerConsignmentID,
                                LineID = item.ID,
                                Code = rd.Code,
                                KhmerName = rd.KhmerName,
                                ExpireDate = cn.ValidDate.Date.ToString("MM/dd/yyyy"),
                                UomName = uom.Name,
                                UomID = uom.ID,
                                Qty = cns.OpenQty,
                                Date = DateTime.Now.Date.ToString("MM/dd/yyyy"),
                                Status = StatusSelect.Open,
                                DisplayQty = cns.OpenQty,
                                ReceiptID = cn.InvoiceID,
                                UserID = r.UserOrderID,
                                BranchID = r.BranchID,
                                PostingDate = r.DateOut,
                                SeriseID = series.ID,
                                EnglishName = rd.EnglishName,
                                WarehouseID = cn.WarehouseID,
                                ReceiptNo = r.ReceiptNo,
                                GLID = acc.ID,
                                Currency = cur.Description,
                                Quantity = cns.OpenQty,
                                CurrencyID = r.SysCurrencyID,
                                ItemID = cns.ItemID,
                                Isselect = false,
                            }).ToList();
                return Ok(list);
            }

            return Ok();
        }

        public async Task<IActionResult> CreateCus(CustomerConsignment data)
        {
            var recipt = _context.Receipt.FirstOrDefault(s => s.ReceiptNo == data.ReceiptNo && s.SeriesID == data.SeriesID);
            var Warehouse = _context.Warehouses.FirstOrDefault(s => s.ID == recipt.WarehouseID);
            var customer = _context.BusinessPartners.FirstOrDefault(s => s.ID == recipt.CustomerID);
            var checkcus = _context.CustomerConsignments.AsNoTracking().FirstOrDefault(s => s.InvoiceID == recipt.ReceiptID && s.Status == StatusSelect.Open);

            data.CustomerConsignmentID = 0;
            data.InvoiceID = recipt.ReceiptID;
            data.InvocieDate = recipt.DateOut;
            data.WarehouseID = Warehouse.ID;
            data.CustomerID = customer.ID;
            data.Status = StatusSelect.Open;

            if (checkcus != null)
            {
                data.CustomerConsignmentID = checkcus.CustomerConsignmentID;
                _context.CustomerConsignments.Update(data);
            }
            else
            {
                _context.CustomerConsignments.Add(data);
            }

            await _context.SaveChangesAsync();


            foreach (var item in data.ItemDetail)
            {
                var itemmaster = _context.ItemMasterDatas.FirstOrDefault(s => s.ID == item.ItemID);

                CustomerConsignmentDetail customerDetail = new();
                customerDetail.CustomerConsignmentDetailID = 0;
                customerDetail.CustomerConsignmentID = data.CustomerConsignmentID;
                customerDetail.ItemID = item.ItemID;
                customerDetail.Qty = item.Qty;
                customerDetail.OpenQty = item.Qty;
                customerDetail.UomID = item.UomID;
                customerDetail.GrpUomID = itemmaster.GroupUomID;
                customerDetail.Status = StatusItem.None;

                if (checkcus != null)
                {
                    var cusDetail = _context.CustomerConsignmentDetails.AsNoTracking().FirstOrDefault(s => s.CustomerConsignmentID == checkcus.CustomerConsignmentID && s.ItemID == item.ItemID);

                    customerDetail.CustomerConsignmentDetailID = cusDetail.CustomerConsignmentDetailID;
                    _context.Update(customerDetail);
                }
                else
                {
                    _context.Add(customerDetail);
                }

                await _context.SaveChangesAsync();
            }

            return Ok();
        }

        [HttpPost]

        public async Task<IActionResult> SaveWithdrawOrReturn(CustomerConsignment data, string Status)
        {
            foreach (var detail in data.ItemDetail)
            {
                var checkstatus = _context.CustomerConsignments.FirstOrDefault(s => s.CustomerConsignmentID == detail.ID && s.Status == StatusSelect.Open);

                if (checkstatus != null)
                {
                    var itemDetail = _context.CustomerConsignmentDetails.FirstOrDefault(s => s.CustomerConsignmentDetailID == detail.DetailID && s.OpenQty > 0);

                    itemDetail.OpenQty = itemDetail.OpenQty - detail.Qty;
                    if (Status == "withdraw")
                    {
                        itemDetail.Status = StatusItem.Withdraw;
                    }
                    else
                    {
                        itemDetail.Status = StatusItem.Return;
                    }

                    _context.Update(itemDetail);
                    await _context.SaveChangesAsync();
                }
            }
            checkItemDetail(data);

            return Ok();
        }

        public void checkItemDetail(CustomerConsignment data)
        {
            var cusMaster = _context.CustomerConsignments.Where(s => data.ItemDetail.Any(k => k.ID == s.CustomerConsignmentID) && s.Status == StatusSelect.Open).ToList();

            foreach (var master in cusMaster)
            {
                var cusDetail = _context.CustomerConsignmentDetails.Where(s => s.CustomerConsignmentID == master.CustomerConsignmentID && s.OpenQty > 0).ToList();

                if (cusDetail.Count <= 0)
                {
                    master.Status = StatusSelect.Close;
                    _context.Update(master);
                    _context.SaveChanges();
                }
            }
        }

        // [HttpPost]
        // public async Task<IActionResult> CheckItemsReturn(CustomerConsignment data)
        // {
        //     var cusDetail = await _context.CustomerConsignmentDetails.Where(s => data.ItemDetail.Any(k => k.DetailID == s.CustomerConsignmentDetailID) && s.OpenQty > 0).ToListAsync();

        //     foreach (var item in data.ItemDetail)
        //     {
        //         item.Status = StatusItem.Return;

        //         _context.Update(item);
        //         await _context.SaveChangesAsync();
        //     }
        //     checkItemDetail(data);

        //     return Ok();
        // }


    }
}