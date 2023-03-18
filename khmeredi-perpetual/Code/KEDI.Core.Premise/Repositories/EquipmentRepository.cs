using CKBS.AppContext;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.ChartOfAccounts;
using CKBS.Models.Services.HumanResources;
using CKBS.Models.Services.Inventory;
using CKBS.Models.Services.POS;
using CKBS.Models.ServicesClass;
using KEDI.Core.Premise.Models.GeneralSettingAdmin;
using KEDI.Core.Premise.Models.Sale;
using KEDI.Core.Premise.Models.Services.Activity;
using KEDI.Core.Premise.Models.Services.Administrator.Inventory;
using KEDI.Core.Premise.Models.Services.ChartOfAccounts;
using KEDI.Core.Premise.Models.Services.Inventory;
using KEDI.Core.Premise.Models.Services.ServiceContractTemplate;
using KEDI.Core.Premise.Models.ServicesClass.EquipmentCard;
using KEDI.Core.Premise.Models.ServicesClass.ServiceCall;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NPOI.HPSF;
using NPOI.POIFS.Crypt.Dsig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Repository
{
    public interface IEquipmentRepository
    {
        Task<EquipmentMasterViewModel> GetEquipmentMasterAsync(string serialNumber, string type);
        Task UpdateServiceCallAsync(ServiceCall serviceCall);
        List<SeriesInPurchasePoViewModel> GetSeries();
        Task<List<BusinessPartner>> GetCustomersAsync();
        Task<List<SerialItemsViewModel>> GetSerialsAsync(int bpid);
        Task<List<ServiceCall>> GetServiceAsync();
        Task<List<ServiceCallview>> GetServiceCallAsync();
        ServiceCall FindServiceCall(string number, int seriesid);
        List<ServiceItem> BindRowsServiceItem(string lineMTID = "");
        Task<List<ServiceCallview>> GetPie1Async();
        Task<List<ServiceCallview>> GetPieOriginAsync();
        Task<List<ServiceCallview>> GetAcountAsync();
        Task<List<ServiceCallview>> GetResolutionAsync();
        Task<List<ServiceCallview>> CloseByAcountAsyc();
        Task<List<ServiceCallview>> AvgCloseTimeAsync();
        Task<List<INComeStatementView>> ChartOfAccountAsync();
        Task<List<INComeStatementView>> BarchartRevenue_COGSAsync();
        Task<List<INComeStatementView>> ChartbarEBITAsync();
        Task<List<BarchartView>> BarchartOpexsAsync();
    }

    public class EquipmentRepository : IEquipmentRepository
    {
        private readonly DataContext _context;
        private readonly ILogger<EquipmentRepository> _logger;
        public EquipmentRepository(ILogger<EquipmentRepository> logger, DataContext context)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<EquipmentMasterViewModel> GetEquipmentMasterAsync(string serialNumber, string type)
        {
            EquipmentMasterViewModel data = new();
            var stockout = type == "MfrSerial" ? await _context.StockOuts.Where(i => i.MfrSerialNumber.ToLower() == serialNumber.ToLower() && i.InStock > 0 && i.Contract > 0).FirstOrDefaultAsync() ?? new StockOut()
                         : type == "serialnumber" ? await _context.StockOuts.Where(i => i.SerialNumber.ToLower() == serialNumber.ToLower() && i.InStock > 0 && i.Contract > 0).FirstOrDefaultAsync() ?? new StockOut()
                                                    : await _context.StockOuts.Where(i => i.PlateNumber.ToLower() == serialNumber.ToLower() && i.InStock > 0 && i.Contract > 0).FirstOrDefaultAsync() ?? new StockOut();
            var item = _context.ItemMasterDatas.FirstOrDefault(i => i.ID == stockout.ItemID);
            if (item != null)
            {
                if (stockout.TransType == TransTypeWD.POS)
                {
                    var dataPos = await _context.Receipt.FirstOrDefaultAsync(i => i.ReceiptID == stockout.TransID) ?? new Receipt();
                    data = await GetEquipmentDataAsync(dataPos, stockout, item, "CustomerID", serialNumber);
                }
                if (stockout.TransType == TransTypeWD.Delivery)
                {
                    var dataPos = await _context.SaleDeliveries.FirstOrDefaultAsync(i => i.SDID == stockout.TransID) ?? new SaleDelivery();
                    data = await GetEquipmentDataAsync(dataPos, stockout, item, "CusID", serialNumber);
                }
                if (stockout.TransType == TransTypeWD.AR)
                {
                    var dataPos = await _context.SaleARs.FirstOrDefaultAsync(i => i.SARID == stockout.TransID) ?? new SaleAR();
                    data = await GetEquipmentDataAsync(dataPos, stockout, item, "CusID", serialNumber);
                }
                return await Task.FromResult(data);
            }
            else
            {
                return await Task.FromResult(new EquipmentMasterViewModel
                {
                    IsError = true,
                    Error = new Error
                    {
                        Message = "No data found!"
                    }
                });
            }

        }
        private async Task<EquipmentMasterViewModel> GetEquipmentDataAsync<T>(T data, StockOut stockOut, ItemMasterData item, string bp, string serialNumber)
        {
            int bpid = (int)GetValue(data, bp);
            var _bp = _context.BusinessPartners.Find(bpid) ?? new BusinessPartner();
            var contract = _context.Contracts.Find(item.ContractID) ?? new Models.Services.ServiceContractTemplate.ContractTemplate();
            List<ETransactionViewModel> transactionWhsViewModels = new();
            List<ETransactionViewModel> transactionStockOutViewModels = new();
            var whds = _context.WarehouseDetails.Where(i => i.SerialNumber == stockOut.SerialNumber).ToList();
            foreach (var whd in whds)
            {
                if (whd.TransType == TransTypeWD.GRPO)
                {
                    var grpos = _context.GoodsReciptPOs.Where(i => i.ID == whd.InStockFrom).ToList();
                    transactionWhsViewModels = await GetTransactionWhsAsync(grpos, stockOut.SerialNumber, "ID", "VendorID");
                }
                if (whd.TransType == TransTypeWD.PurAP)
                {
                    var aps = _context.Purchase_APs.Where(i => i.PurchaseAPID == whd.InStockFrom).ToList();
                    transactionWhsViewModels = await GetTransactionWhsAsync(aps, stockOut.SerialNumber, "PurchaseAPID", "VendorID");
                }
            }
            if (stockOut.TransType == TransTypeWD.POS)
            {
                var dataPos = _context.Receipt.Where(i => i.ReceiptID == stockOut.TransID).ToList();
                transactionStockOutViewModels = await GetTransactionStockOutAsync(dataPos, stockOut.SerialNumber, "ReceiptID", "CustomerID", "ReceiptNo");
            }
            if (stockOut.TransType == TransTypeWD.AR)
            {
                var dataPos = _context.SaleARs.Where(i => i.SARID == stockOut.TransID).ToList();
                transactionStockOutViewModels = await GetTransactionStockOutAsync(dataPos, stockOut.SerialNumber, "SARID", "CusID", "InvoiceNumber");
            }
            if (stockOut.TransType == TransTypeWD.Delivery)
            {
                var dataPos = _context.SaleDeliveries.Where(i => i.SDID == stockOut.TransID).ToList();
                transactionStockOutViewModels = await GetTransactionStockOutAsync(dataPos, stockOut.SerialNumber, "SDID", "CusID", "InvoiceNumber");
            }
            List<ETransactionViewModel> transactionViewModels = new(transactionWhsViewModels.Count + transactionStockOutViewModels.Count);
            transactionViewModels.AddRange(transactionWhsViewModels);
            transactionViewModels.AddRange(transactionStockOutViewModels);
            EServiceContractViewModel eServiceContractViewModels = new()
            {
                ContractName = contract.Name,
                EndDate = stockOut.AdmissionDate?.AddMonths((int)contract.Duration).ToShortDateString(),
                ServiceType = contract.ContracType == (int)ContractType.SerialNumber ? "Warranty" : "",
                TerminationDate = "",
                StartDate = stockOut.AdmissionDate?.ToShortDateString(),
            };

            var serviceCalls = (from svd in _context.ServiceDatas.Where(i => i.SerialNo == stockOut.SerialNumber)
                                join sc in _context.ServiceCalls on svd.ServiceCallID equals sc.ID
                                join technician in _context.Employees on sc.TechnicianID equals technician.ID
                                join cus in _context.BusinessPartners on sc.BPID equals cus.ID
                                join __item in _context.ItemMasterDatas on svd.ItemID equals __item.ID

                                select new EServiceCallViewModel
                                {
                                    CallName = sc.Name,
                                    Creation = sc.CreatedOnDate.ToShortDateString(),
                                    CustomerName = cus.Name,
                                    Subject = sc.Subject,
                                    ItemCode = __item.Code,
                                    ItemName = __item.KhmerName,
                                    Technician = technician.Name,
                                }).ToList();

            EquipmentMasterViewModel _data = new()
            {
                BPCode = _bp.Code,
                BPName = _bp.Name,
                ItemCode = item.Code,
                MfrSerialNo = stockOut.MfrSerialNumber,
                PlateNumber = stockOut.PlateNumber,
                NewSN = "",
                PreviousSN = "",
                SerialNumber = stockOut.SerialNumber,
                ServiceCalls = serviceCalls == null ? new List<EServiceCallViewModel>() : serviceCalls,
                ServiceContract = eServiceContractViewModels == null ? new EServiceContractViewModel() : eServiceContractViewModels,
                Status = StatusEquipmentCard.Active,
                Technician = "",// serviceCalls[0].Technician==null?"":serviceCalls[0].Technician,
                TelephoneNo = _bp.Phone == null || _bp.Phone == "" ? "" : _bp.Phone,
                Territory = "",
                Transactions = transactionViewModels == null ? new List<ETransactionViewModel>() : transactionViewModels,

            };
            return await Task.FromResult(_data);
        }

        private async Task<List<ETransactionViewModel>> GetTransactionWhsAsync<T>(List<T> dataType, string serial, string dataTypeId, string bpId)
        {
            var whdTrans = await (from whd in _context.WarehouseDetails.Where(i => i.SerialNumber == serial)
                                  join _dt in dataType on whd.InStockFrom equals (int)GetValue(_dt, dataTypeId)
                                  join bp in _context.BusinessPartners on (int)GetValue(_dt, bpId) equals bp.ID
                                  join doc in _context.DocumentTypes on (int)GetValue(_dt, "DocumentTypeID") equals doc.ID
                                  join series in _context.Series on (int)GetValue(_dt, "SeriesID") equals series.ID
                                  join wh in _context.Warehouses on whd.WarehouseID equals wh.ID
                                  select new ETransactionViewModel
                                  {
                                      Date = whd.AdmissionDate.ToString(),
                                      Direction = "In",
                                      DocumentNo = doc.Code,
                                      GLAccBPCode = bp.Code,
                                      GLAccBPName = bp.Name,
                                      Source = $"{series.Name}-{GetValue(_dt, "Number")}",
                                      WhsName = wh.Name
                                  }).ToListAsync();
            return whdTrans;
        }
        private async Task<List<ETransactionViewModel>> GetTransactionStockOutAsync<T>(List<T> dataType, string serial, string dataTypeId, string bpId, string number)
        {
            var StockOutTrans = await (from whd in _context.StockOuts.Where(i => i.SerialNumber == serial)
                                       join _dt in dataType on whd.TransID equals (int)GetValue(_dt, dataTypeId)
                                       join bp in _context.BusinessPartners on (int)GetValue(_dt, bpId) equals bp.ID
                                       join series in _context.Series on (int)GetValue(_dt, "SeriesID") equals series.ID
                                       join doc in _context.DocumentTypes on series.DocuTypeID equals doc.ID
                                       join wh in _context.Warehouses on whd.WarehouseID equals wh.ID
                                       select new ETransactionViewModel
                                       {
                                           Date = whd.AdmissionDate.ToString(),
                                           Direction = "Out",
                                           DocumentNo = doc.Code,
                                           GLAccBPCode = bp.Code,
                                           GLAccBPName = bp.Name,
                                           Source = $"{series.Name}-{GetValue(_dt, number)}",
                                           WhsName = wh.Name
                                       }).ToListAsync();
            return StockOutTrans;
        }
        private static object GetValue<T>(T obj, string prop)
        {
            try
            {
                var data = typeof(T).GetProperty(prop).GetValue(obj);
                return data;
            }
            catch
            {
                return string.Empty;
            }

        }

        public async Task UpdateServiceCallAsync(ServiceCall serviceCall)
        {
            using var t = _context.Database.BeginTransaction();
            SeriesDetail seriesDetail = new();
            var Series = _context.Series.Find(serviceCall.SeriesID);
            seriesDetail.Number = Series.NextNo;
            seriesDetail.SeriesID = Series.ID;
            if (serviceCall.ID == 0)
            {
                serviceCall.Number = Series.NextNo;
                // insert Series Detail
                _context.SeriesDetails.Update(seriesDetail);
                _context.SaveChanges();
                var seriesDetailID = seriesDetail.ID;
                string Sno = Series.NextNo;
                long No = long.Parse(Sno);
                Series.NextNo = Convert.ToString(No + 1);
                serviceCall.SeriesDID = seriesDetailID;
            }




            _context.ServiceCalls.Update(serviceCall);
            await _context.SaveChangesAsync();
            t.Commit();
        }

        public List<SeriesInPurchasePoViewModel> GetSeries()
        {
            var data = (from dt in _context.DocumentTypes.Where(i => i.Code == "SC")
                        join sr in _context.Series.Where(w => !w.Lock) on dt.ID equals sr.DocuTypeID
                        select new SeriesInPurchasePoViewModel
                        {
                            ID = sr.ID,
                            Name = sr.Name,
                            Default = sr.Default,
                            NextNo = sr.NextNo,
                            DocumentTypeID = sr.DocuTypeID,
                            SeriesDetailID = _context.SeriesDetails.Where(i => i.SeriesID == sr.ID).FirstOrDefault() == null ? 0 :
                            _context.SeriesDetails.Where(i => i.SeriesID == sr.ID).FirstOrDefault().ID
                        }).ToList();
            return data;
        }

        public async Task<List<BusinessPartner>> GetCustomersAsync()
        {
            var data = await _context.BusinessPartners.Where(i => i.Type == "Customer").Select(i => new BusinessPartner
            {
                ID = i.ID,
                Code = i.Code,
                Name = i.Name,
                Phone = i.Phone,
                Address = i.Address
            }).ToListAsync();
            return data;
        }

        public async Task<List<SerialItemsViewModel>> GetSerialsAsync(int bpid)
        {
            var receipts = _context.Receipt.Where(i => i.CustomerID == bpid).Select(i => new BindARDeliveryPOS
            {
                CusId = i.CustomerID,
                TransType = TransTypeWD.POS,
                TransTypeId = i.ReceiptID
            }).ToList();
            var ar = _context.SaleARs.Where(i => i.CusID == bpid).Select(i => new BindARDeliveryPOS
            {
                CusId = i.CusID,
                TransType = TransTypeWD.AR,
                TransTypeId = i.SARID
            }).ToList();
            var delivery = _context.SaleDeliveries.Where(i => i.CusID == bpid).Select(i => new BindARDeliveryPOS
            {
                CusId = i.CusID,
                TransType = TransTypeWD.Delivery,
                TransTypeId = i.SDID
            }).ToList();

            List<BindARDeliveryPOS> bindARDeliveryPOS = new(receipts.Count + ar.Count + delivery.Count);
            bindARDeliveryPOS.AddRange(receipts);
            bindARDeliveryPOS.AddRange(ar);
            bindARDeliveryPOS.AddRange(delivery);

            var data = (from allData in bindARDeliveryPOS
                        join stockOut in _context.StockOuts on allData.TransTypeId equals stockOut.TransID
                        join bp in _context.BusinessPartners on allData.CusId equals bp.ID
                        join item in _context.ItemMasterDatas on stockOut.ItemID equals item.ID
                        join itemGroup in _context.ItemGroup1 on item.ItemGroup1ID equals itemGroup.ItemG1ID
                        where stockOut.TransType == allData.TransType && !string.IsNullOrEmpty(stockOut.SerialNumber)
                        select new SerialItemsViewModel
                        {
                            CustomerName = bp.Name,
                            ItemCode = item.Code,
                            ItemName = item.KhmerName,
                            LineID = DateTime.Now.Ticks.ToString(),
                            MfrSerialNo = stockOut.MfrSerialNumber,
                            Serial = stockOut.SerialNumber,
                            PlateNumber = stockOut.PlateNumber,
                            Status = "",
                            ItemID = item.ID,
                            ItemGroupName = itemGroup.Name,
                            GroupItemID = item.ItemGroup1ID
                        }).ToList();
            return await Task.FromResult(data);
        }

        public async Task<List<ServiceCall>> GetServiceAsync()
        {
            var list = _context.ServiceCalls.ToList();
            return await Task.FromResult(list);
        }
        public async Task<List<ServiceCallview>> GetServiceCallAsync()
        {
            var list = (from s in _context.ServiceCalls
                        join bp in _context.BusinessPartners on s.BPID equals bp.ID

                        join tech in _context.Employees on s.TechnicianID equals tech.ID
                        let handleby = _context.Employees.FirstOrDefault(e => e.ID == s.HandledByID) ?? new Employee()
                        select new ServiceCallview
                        {
                            ID = s.ID,
                            CallID = s.CallID,
                            Number = s.Number,
                            BPCode = bp.Code,
                            BName = bp.Name,
                            Subject = s.Subject,
                            Priority = s.Priority.ToString(),
                            CallStatus = s.CallStatus.ToString(),

                            Handleby = handleby.Name == null ? "" : handleby.Name,
                            Technician = tech.Name,
                            CreatedOnDate = s.CreatedOnDate.ToString("dd-MM-yyyy"),
                            CreatedOnTime = s.CreatedOnTime,
                            SerialNumber = s.SerialNumber,


                            SeriesID = s.SeriesID,
                        }).OrderByDescending(s => s.ID).ToList();

            return await Task.FromResult(list);
        }

        public ServiceCall FindServiceCall(string number, int seriesid)
        {
            var activity = (from setact in _context.SetupTypes

                            select new Activytyview
                            {
                                ID = setact.ID,
                                Name = setact.Name,
                            }).ToList();
            activity.Insert(0, new Activytyview
            {
                ID = 0,
                Name = "-- Select --",
            });
            var obj = (from s in _context.ServiceCalls.Where(s => s.Number == number && s.SeriesID == seriesid)
                       join cus in _context.BusinessPartners on s.BPID equals cus.ID

                       join technical in _context.Employees on s.TechnicianID equals technical.ID
                       let handle = _context.Employees.FirstOrDefault(x => x.ID == s.HandledByID) ?? new Employee()
                       select new ServiceCall
                       {
                           ID = s.ID,
                           Name = s.Name,
                           BPID = s.BPID,
                           BCode = cus.Code,
                           BName = cus.Name,
                           BPhone = cus.Phone,
                           BPRefNo = s.BPRefNo,
                           ChannelID = s.ChannelID,
                           HandledByID = s.HandledByID,
                           TechnicianID = s.TechnicianID,
                           HandleName = handle.Name == null ? "" : handle.Name,
                           TectnicalName = technical.Name,
                           MfrSerialNo = s.MfrSerialNo,
                           CallID = s.CallID,
                           CallStatus = s.CallStatus,
                           ClosedOnDate = s.ClosedOnDate,
                           ClosedOnTime = s.ClosedOnTime,
                           ContractNo = s.ContractNo,
                           CreatedOnDate = s.CreatedOnDate,
                           CreatedOnTime = s.CreatedOnTime,
                           DocTypeID = s.DocTypeID,
                           EndDate = s.EndDate,
                           ItemGroupID = s.ItemGroupID,
                           ItemID = s.ItemID,
                           Number = s.Number,
                           Priority = s.Priority,
                           Resolution = s.Resolution,
                           SerialNumber = s.SerialNumber,
                           SeriesDID = s.SeriesDID,
                           SeriesID = s.SeriesID,
                           Subject = s.Subject,
                           ServiceDatas = (from sd in _context.ServiceDatas.Where(x => x.ServiceCallID == s.ID)
                                           join IM in _context.ItemMasterDatas.Where(s => s.Delete == false) on sd.ItemID equals IM.ID
                                           join wh in _context.Warehouses on IM.WarehouseID equals wh.ID
                                           let whd = _context.WarehouseDetails.Where(s => s.WarehouseID == wh.ID).ToList()
                                           select new ServiceData
                                           {
                                               ID = sd.ID,
                                               ItemID = sd.ItemID,
                                               ItemCode = IM.Code,
                                               ItemName = IM.EnglishName,
                                               MfrSerialNo = sd.MfrSerialNo, //whd.FirstOrDefault().MfrSerialNumber,
                                               SerialNo = sd.SerialNo, //whd.FirstOrDefault().SerialNumber,
                                               PlateNumber = sd.PlateNumber,
                                               Qty = sd.Qty,
                                               ServiceCallID = sd.ServiceCallID,
                                               LineMTID = DateTime.Now.Ticks.ToString(),
                                               ServiceItems = (from si in _context.ServiceItems.Where(i => i.ServiceDataID == sd.ID)
                                                               join hd in _context.Employees on si.HandledByID equals hd.ID
                                                               join tch in _context.Employees on si.TechnicianID equals tch.ID
                                                               //join act in _context.Activites.Where(s => s.Activities == Activities.Services) on si.LinkActivytyID equals act.ID
                                                               //join setty in _context.SetupTypes on act.TypeID equals setty.ID
                                                               //into data
                                                               //from ac in data.DefaultIfEmpty()
                                                               //let act = _context.Activites.Where(s => s.Activities == Activities.Services && s.ID== si.LinkActivytyID).FirstOrDefault()??new Activity()
                                                               //let ac =_context.Activites.Where(s => s.Activities == Activities.Services && s.ID== si.LinkActivytyID).DefaultIfEmpty().FirstOrDefault()??new Activity()
                                                               //let se = _context.SetupTypes.Where(s=>s.ID==ac.TypeID).DefaultIfEmpty().FirstOrDefault()??new SetupType()
                                                               select new ServiceItem
                                                               {
                                                                   ID = si.ID,
                                                                   ActivityID = si.ActivityID,
                                                                   HandledByID = si.HandledByID,
                                                                   TechnicianID = si.TechnicianID,
                                                                   LineID = DateTime.Now.Ticks.ToString() + si.ID,
                                                                   LineMTID = DateTime.Now.Ticks.ToString() + si.ID,// sd.LineMTID,
                                                                   LinkActivytyID = si.LinkActivytyID,
                                                                   Activitys = activity.Select(i => new SelectListItem
                                                                   {
                                                                       Text = i.Name,
                                                                       Value = i.ID.ToString(),
                                                                       Selected = i.ID == si.ActivityID,
                                                                   }).ToList(),
                                                                   Acitivity = si.Acitivity,
                                                                   ActivityName = si.ActivityName,
                                                                   Completed = si.Completed,
                                                                   EndDate = si.EndDate,
                                                                   EndTime = si.EndTime,
                                                                   Finnish = si.Finnish,
                                                                   HandledByName = hd.Name,
                                                                   TechnicianName = tch.Name,
                                                                   Remark = si.Remark,
                                                                   ServiceDataID = si.ServiceDataID,
                                                                   StartDate = si.StartDate,
                                                                   StartTime = si.StartTime,

                                                               }).ToList(),
                                           }).ToList() ?? new List<ServiceData>(),
                       }).FirstOrDefault() ?? new ServiceCall();

            if (obj != null)
            {
                int count = obj.ServiceDatas == null ? 0 : obj.ServiceDatas.Count;
                int counsi;
                // int countsi=obj.ServiceDatas.
                obj.ServiceDatas = obj.ServiceDatas == null ? new List<ServiceData>() : obj.ServiceDatas;
                foreach (var z in obj.ServiceDatas)
                {
                    counsi = z.ServiceItems.Count;
                    List<ServiceItem> SI = new List<ServiceItem>();
                    SI.AddRange(z.ServiceItems);
                    for (var j = 0; j < 10 - counsi; j++)
                    {

                        string lineId = DateTime.Now.Ticks.ToString();
                        ServiceItem siobj = new ServiceItem
                        {

                            LineID = lineId,
                            LineMTID = z.LineMTID,
                            Activitys = activity.Select(i => new SelectListItem
                            {
                                Text = i.Name,
                                Value = i.ID.ToString(),
                            }).ToList(),

                            HandledByName = "",
                            Remark = "",
                            TechnicianName = "",
                            ActivityName = "",

                        };
                        SI.Add(siobj);
                    }
                    z.ServiceItems = SI;
                }
                List<ServiceData> SD = new List<ServiceData>();
                SD.AddRange(obj.ServiceDatas);
                for (var i = 0; i < 10 - count; i++)
                {
                    string lineMTId = DateTime.Now.Ticks.ToString();

                    ServiceData sdobj = new ServiceData
                    {
                        LineMTID = lineMTId,
                        ItemCode = "",
                        ItemName = "",
                        MfrSerialNo = "",
                        SerialNo = "",
                        ServiceItems = BindRowsServiceItem(lineMTId),
                    };
                    SD.Add(sdobj);
                }
                obj.ServiceDatas = SD;
            }
            var itemobj = _context.ItemMasterDatas.Where(s => s.ID == obj.ItemID).FirstOrDefault() ?? new ItemMasterData();
            obj.ItemCode = itemobj.Code;
            obj.ItemName = itemobj.KhmerName;
            var g = _context.ItemGroup1.Where(s => s.ItemG1ID == obj.ItemGroupID).FirstOrDefault() ?? new CKBS.Models.Services.Inventory.Category.ItemGroup1();
            obj.GName = g.Name;
            return obj;


        }
        public List<ServiceItem> BindRowsServiceItem(string lineMTID = "")
        {
            var activity = (from setact in _context.SetupTypes

                            select new Activytyview
                            {
                                ID = setact.ID,
                                Name = setact.Name,
                            }).ToList();
            activity.Insert(0, new Activytyview
            {
                ID = 0,
                Name = "-- Select --",
            });
            List<ServiceItem> SI = new List<ServiceItem>();
            for (var j = 0; j < 10; j++)
            {
                string lineId = DateTime.Now.Ticks.ToString();
                ServiceItem siobj = new ServiceItem
                {

                    LineID = lineId,
                    LineMTID = lineMTID,
                    Activitys = activity.Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.ID.ToString(),
                    }).ToList(),

                    HandledByName = "",
                    Remark = "",
                    TechnicianName = "",
                    ActivityName = "",

                };
                SI.Add(siobj);
            }
            return SI;
        }
        public async Task<List<ServiceCallview>> GetPie1Async()
        {
            var list = (from s in _context.ServiceCalls.Where(s => s.CreatedOnDate.Year == DateTime.Now.Year && s.CallStatus == CallStatusType.Open)

                        group new { s } by new { s.Priority } into datas
                        let data = datas.Count()
                        orderby data descending
                        select new ServiceCallview
                        {
                            ID = datas.FirstOrDefault().s.ID,
                            Priority = ((int)(datas.FirstOrDefault().s.Priority)) == 1 ? "Low" : ((int)(datas.FirstOrDefault().s.Priority)) == 2 ? "Medium" : ((int)(datas.FirstOrDefault().s.Priority)) == 3 ? "High" : "Critical",
                            CountData = data,
                        }).OrderByDescending(r => r.CountData).ToList();
            return await Task.FromResult(list);
        }

        public async Task<List<ServiceCallview>> GetPieOriginAsync()
        {
            var list = (from s in _context.ServiceCalls.Where(s => s.CreatedOnDate.Year == DateTime.Now.Year && s.CallStatus == CallStatusType.Open)
                        join chan in _context.Channels on s.ChannelID equals chan.ID
                        group new { s, chan } by new { s.ChannelID, chan.Name } into datas
                        let data = datas.Count()
                        orderby data descending
                        select new ServiceCallview
                        {
                            ID = datas.FirstOrDefault().s.ID,
                            ChannelID = datas.FirstOrDefault().s.ChannelID,
                            ChannelName = datas.FirstOrDefault().chan.Name,
                            CountData = data,
                        }
                      ).OrderByDescending(r => r.CountData).ToList();
            return await Task.FromResult(list);
        }

        public async Task<List<ServiceCallview>> GetAcountAsync()
        {

            var list = (from s in _context.ServiceCalls.Where(s => s.CallStatus == CallStatusType.Open && s.CreatedOnDate.Year == DateTime.Now.Year)
                        join em in _context.Employees on s.TechnicianID equals em.ID
                        group new { s, em } by new { em.Name } into datas
                        let data = datas.Count()
                        orderby data descending
                        select new ServiceCallview
                        {
                            ID = datas.FirstOrDefault().s.ID,
                            ChannelID = datas.FirstOrDefault().s.TechnicianID,
                            ChannelName = datas.FirstOrDefault().em.Name,
                            CountData = data,
                        }
                     ).OrderByDescending(r => r.CountData).ToList();
            return await Task.FromResult(list);
        }

        public async Task<List<ServiceCallview>> CloseByAcountAsyc()
        {
            var list = (from s in _context.ServiceCalls.Where(s => s.CallStatus == CallStatusType.Closed && s.CreatedOnDate.Year == DateTime.Now.Year)
                        join em in _context.Employees on s.TechnicianID equals em.ID
                        group new { s, em } by new { s.TechnicianID } into datas
                        let data = datas.Count()
                        orderby data descending
                        select new ServiceCallview
                        {
                            ID = datas.First().s.ID,
                            CallStatus = datas.First().em.Name,//((int)(datas.First().s.CallStatus)) == 1 ? "Open" : ((int)(datas.First().s.CallStatus)) == 2 ? "Close" : ((int)(datas.First().s.CallStatus)) == 3 ? "Pending" : "Critical",
                            CountData = data,

                        }).OrderByDescending(r => r.CountData).ToList();
            return await Task.FromResult(list);
        }
        private static TimeSpan GetAvgTime(string timeString)
        {
            _ = TimeSpan.TryParse(timeString, out TimeSpan _ts);
            return _ts;
        }

        private static TimeSpan GetAvgResolution(List<ServiceCall> serviceCalls)
        {
            double closetime = (double)serviceCalls.Average(s => s.ClosedOnDate?.Add(GetAvgTime(s.ClosedOnTime))
            .Subtract(s.CreatedOnDate.Add(GetAvgTime(s.CreatedOnTime))).TotalSeconds);
            return TimeSpan.FromSeconds(closetime);
        }

        public async Task<List<ServiceCallview>> GetResolutionAsync()
        {
            try
            {
                var list = (from s in _context.ServiceCalls.Where(s => s.CallStatus == CallStatusType.Closed && s.CreatedOnDate.Year == DateTime.Now.Year)
                            join em in _context.Employees on s.TechnicianID equals em.ID
                            group new { s, em } by new { s.TechnicianID, em.Name } into datas
                            let data = datas.Count()
                            let avgTime = GetAvgResolution(datas.Select(d => d.s).ToList())
                            orderby data descending
                            select new ServiceCallview
                            {
                                ChannelName = datas.FirstOrDefault().em.Name,
                                // AvgResolutionTime = avgTime.TotalHours,
                                Resolvedontime = avgTime.TotalHours.ToString("N2"),
                                // = $"{avgTime.Hours}h : {avgTime.Minutes}m : {avgTime.Seconds}s"
                            }
                   ).OrderByDescending(r => r.CountData).ToList();
                return await Task.FromResult(list);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex.Message);
                return await Task.FromResult(new List<ServiceCallview>());
            }
        }


        public async Task<List<ServiceCallview>> AvgCloseTimeAsync()
        {
            var serviceCalls = _context.ServiceCalls.Where(s => s.CallStatus == CallStatusType.Closed && s.CreatedOnDate.Year == DateTime.Now.Year).ToList();
            var list = (from s in serviceCalls
                        join em in _context.Employees on s.TechnicianID equals em.ID
                        group new { s } by new { s.CallStatus } into datas
                        let data = datas.Count()
                        let avgTime = GetAvgResolution(serviceCalls.ToList())
                        orderby data descending
                        select new ServiceCallview
                        {
                            ID = datas.First().s.ID,
                            CallStatus = ((int)(datas.First().s.CallStatus)) == 1 ? "Open" : ((int)(datas.First().s.CallStatus)) == 2 ? "Close" : ((int)(datas.First().s.CallStatus)) == 3 ? "Pending" : "Critical",
                            CountData = data,
                            Resolvedondate = $"{string.Format("{0:N2}", avgTime.TotalHours)} h",
                            AvgResolutionTime = avgTime.TotalHours,
                        }
                    ).ToList();
            return await Task.FromResult(list);
        }
        //..............................Start Dashboard Income Statement ....................................

        private async Task<List<INComeStatementView>> GLAccount(CategoryType type)
        {
            string year = DateTime.Now.Year.ToString();

            var obj = await _context.GLAccounts.FirstOrDefaultAsync(s => s.CategoryType == type) ?? new CKBS.Models.Services.ChartOfAccounts.GLAccount();
            var lists = await (from gl in _context.GLAccounts.Where(s => s.IsActive == true && s.MainParentId == obj.ID)
                               join acb in _context.AccountBalances on gl.ID equals acb.GLAID
                               select new INComeStatementView
                               {
                                   ID = Convert.ToInt32(acb.PostingDate.ToString("MM")),//turnchild.ID,
                                   Titile = gl.Name,
                                   Balance = 0,
                                   Debit = acb.Debit,
                                   Credit = acb.Credit,
                                   SubTypeAcountID = gl.SubTypeAccountID,
                                   Date = acb.PostingDate.ToString("MMM yyyy"),
                                   Year = acb.PostingDate.ToString("yyyy"),
                                   Month = acb.PostingDate.ToString("MM"),
                               }

                       ).ToListAsync();



            lists = lists.Where(s => s.Year == year).ToList();
            return lists;
        }
        private async Task<List<INComeStatementView>> GLAccount_COGS(CategoryType type, int parametertype)
        {
            string year = DateTime.Now.Year.ToString();
            var obj = await _context.GLAccounts.FirstOrDefaultAsync(s => s.CategoryType == type) ?? new CKBS.Models.Services.ChartOfAccounts.GLAccount();
            var list = await (from gl in _context.GLAccounts.Where(s => s.IsActive == true && s.MainParentId == obj.ID)
                              join acb in _context.AccountBalances.Where(i => i.PostingDate.ToString("yyyy") == year) on gl.ID equals acb.GLAID
                              select new INComeStatementView
                              {
                                  ID = Convert.ToInt32(acb.PostingDate.ToString("MM")),//turnchild.ID,
                                  Titile = gl.Name,
                                  Debit = parametertype == 1 ? acb.Debit : 0,
                                  Credit = parametertype == 1 ? acb.Credit : 0,
                                  Revenue = parametertype == 1 ? 0 : acb.Debit, // ខ្ខី​ Propery ប្រើ
                                  Balance = parametertype == 1 ? 0 : acb.Credit,// ខ្ខី​ Propery ប្រើ
                                  SubTypeAcountID = gl.SubTypeAccountID,
                                  Date = acb.PostingDate.ToString("MMM yyyy"),
                                  Year = acb.PostingDate.ToString("yyyy"),
                                  Month = acb.PostingDate.ToString("MM"),
                              }).ToListAsync();
            list = list.Where(s => s.Year == year).ToList();
            return list;
        }
        private async Task<List<INComeStatementView>> GLAccountGroupMonth(List<INComeStatementView> list, int sumgroup)
        {
            // noted 1= sum Revenue
            //       2= sum COG
            //       3= sum OperatingProfit
            var lists = (from all in list
                         group new { all } by new { all.Date, } into g
                         let data = g.FirstOrDefault()
                         select new INComeStatementView
                         {
                             ID = data.all.ID,
                             Titile = data.all.Titile,
                             Revenue = sumgroup == 1 ? g.Sum(s => s.all.Debit - s.all.Credit) < 0 ? g.Sum(s => s.all.Debit - s.all.Credit) * (-1) : g.Sum(s => s.all.Debit - s.all.Credit) : 0,
                             COGS = sumgroup == 2 ? g.Sum(s => s.all.Debit - s.all.Credit) < 0 ? g.Sum(s => s.all.Debit - s.all.Credit) * (-1) : g.Sum(s => s.all.Debit - s.all.Credit) : 0,
                             Opex = sumgroup == 3 ? g.Sum(s => s.all.Debit - s.all.Credit) < 0 ? g.Sum(s => s.all.Debit - s.all.Credit) * (-1) : g.Sum(s => s.all.Debit - s.all.Credit) : 0,
                             SubTypeAcountID = data.all.SubTypeAcountID,
                             Date = data.all.Date,
                             Year = data.all.Year,
                             Month = data.all.Month,
                         }).ToList();

            return await Task.FromResult(lists);
        }
        private async Task<List<BarchartView>> OpexChart(int id = 0)
        {
            var OperatingCosts = await GLAccount(CategoryType.OperatingCosts);
            var lists = await (from obj in _context.SubTypeAcounts.Where(s => !string.IsNullOrWhiteSpace(s.Name) && s.ID == id && !s.Delete)
                               join opt in OperatingCosts on obj.ID equals opt.SubTypeAcountID
                               select new BarchartView
                               {

                                   Name = id == 1 ? obj.Name : "",
                                   Name2 = id == 2 ? obj.Name : "",
                                   Name3 = id == 3 ? obj.Name : "",
                                   Debit1 = id == 1 ? opt.Debit : 0,
                                   Crebit1 = id == 1 ? opt.Credit : 0,
                                   Debit2 = id == 2 ? opt.Debit : 0,
                                   Crebit2 = id == 2 ? opt.Credit : 0,
                                   Debit3 = id == 3 ? opt.Debit : 0,
                                   Crebit3 = id == 3 ? opt.Credit : 0,
                                   Date = opt.Date,
                                   Month = opt.Month,
                                   Year = opt.Year,
                               }
                 ).ToListAsync();

            return lists;
        }

        public async Task<List<INComeStatementView>> ChartOfAccountAsync()
        {
            var obj = await _context.GLAccounts.FirstOrDefaultAsync(s => s.CategoryType == CategoryType.Turnover) ?? new GLAccount();
            var compay = await _context.Company.FirstOrDefaultAsync(s => s.ID == obj.CompanyID) ?? new Company();

            var curName = await _context.Currency.FindAsync(compay.SystemCurrencyID) ?? new Currency();
            var displayCurr = await _context.DisplayCurrencies.FirstOrDefaultAsync(c => c.AltCurrencyID == curName.ID) ?? new DisplayCurrency();
            var disformat = await _context.Displays.FirstOrDefaultAsync(c => c.DisplayCurrencyID == displayCurr.AltCurrencyID) ?? new Display();
            var turnover = await GLAccount(CategoryType.Turnover);
            var CostofSales = await GLAccount(CategoryType.CostofSales);
            var InterestAndTax = await GLAccount(CategoryType.TaxationExtraordinary);
            var OperatingCosts = await GLAccount(CategoryType.OperatingCosts);


            decimal turnovervalue = turnover.Sum(i => ((i.Debit - i.Credit) * (-1)));
            decimal CostofSalesvalue = CostofSales.Sum(i => ((i.Debit - i.Credit)));
            decimal opexamount = OperatingCosts.Sum(i => ((i.Debit - i.Credit)));
            List<INComeStatementView> list = new();
            var subtype = await _context.SubTypeAcounts.Where(s => !string.IsNullOrWhiteSpace(s.Name) && !s.Delete).ToListAsync();
            var opex = (from sub in subtype
                        select new Opex
                        {
                            Name = sub.Name,
                            Amount = OperatingCosts.Where(x => x.SubTypeAcountID == sub.ID).Sum(i => ((i.Debit - i.Credit)))
                        }
                     ).ToList();

            list.Add(new INComeStatementView
            {
                Format = disformat.Amounts,
                Currency = curName.Description == "USD" ? "$" : curName.Description == "KHR" ? "៛" : curName.Description,
                Revenue = turnovervalue,
                COGS = CostofSalesvalue,
                GrossProfit = turnovervalue - CostofSalesvalue,
                InterestTax = InterestAndTax.Sum(i => ((i.Debit - i.Credit))),
                Opex = opexamount,
                OpexItem = opex,
                OperatingProfit = ((turnovervalue - CostofSalesvalue) - opexamount),
                NetProfit = ((turnovervalue - CostofSalesvalue) - opexamount) - InterestAndTax.Sum(i => ((i.Debit - i.Credit))),

            });
            return list;
        }

        public async Task<List<INComeStatementView>> BarchartRevenue_COGSAsync()
        {
            var turnover = await GLAccount_COGS(CategoryType.Turnover, 1);// 1= Turnover
            var costofsale = await GLAccount_COGS(CategoryType.CostofSales, 2); // 2= CostofSales
            List<INComeStatementView> all_list = new(turnover.Count + costofsale.Count);
            all_list.AddRange(turnover);
            all_list.AddRange(costofsale);
            var list = (from all in all_list
                        group new { all } by new { all.Month, } into g
                        let data = g.FirstOrDefault()
                        select new INComeStatementView
                        {
                            ID = Convert.ToInt32(data.all.Month),
                            Revenue = g.Sum(i => ((i.all.Debit - i.all.Credit) * (-1))),
                            COGS = g.Sum(i => ((i.all.Revenue - i.all.Balance))),
                            Date = data.all.Date,
                            Month = data.all.Month,
                        }).OrderBy(s => s.ID).ToList();
            return list;
        }

        public async Task<List<INComeStatementView>> ChartbarEBITAsync()
        {
            var turnover = await GLAccountGroupMonth(await GLAccount(CategoryType.Turnover), 1);
            var CostofSales = await GLAccountGroupMonth(await GLAccount(CategoryType.CostofSales), 2);
            var OperatingCosts = await GLAccountGroupMonth(await GLAccount(CategoryType.OperatingCosts), 3);
            List<INComeStatementView> all_list = new(turnover.Count + CostofSales.Count + OperatingCosts.Count);
            all_list.AddRange(turnover);
            all_list.AddRange(CostofSales);
            all_list.AddRange(OperatingCosts);
            var list = (from all in all_list
                        group new { all } by new { all.Date, } into g
                        let data = g.FirstOrDefault()
                        select new INComeStatementView
                        {
                            ID = Convert.ToInt32(data.all.Month),
                            Revenue = g.Sum(s => s.all.Revenue),
                            COGS = g.Sum(s => s.all.COGS),
                            Opex = g.Sum(s => s.all.Opex),
                            OperatingProfit = (((g.Sum(s => s.all.Revenue) - g.Sum(s => s.all.COGS)) - g.Sum(s => s.all.Opex))),
                            Date = data.all.Date,
                            Month = data.all.Month
                        }).OrderBy(s => s.ID).ToList();
            return list;
        }

        public async Task<List<BarchartView>> BarchartOpexsAsync()
        {
            var list1 = await OpexChart(1);// not 1= condition is relat with subtype
            var list2 = await OpexChart(2); // not 2=> condition  is relat with subtype
            var list3 = await OpexChart(3);  // not 3=> condition is relat with subtype
            List<BarchartView> all_list = new(list1.Count + list2.Count + list3.Count);
            all_list.AddRange(list1);
            all_list.AddRange(list2);
            all_list.AddRange(list3);
            var obj = await _context.SubTypeAcounts.Where(s => !string.IsNullOrWhiteSpace(s.Name) && s.ID <= 3 && !s.Delete).ToListAsync();// select 3 item first
            var list = (from all in all_list
                        group new { all } by new { all.Month } into g
                        let data = g.FirstOrDefault()
                        select new BarchartView
                        {
                            ID = Convert.ToInt32(data.all.Month),
                            Name = obj[0].Name,
                            Name2 = obj[1].Name,
                            Name3 = obj[2].Name,
                            Revenue1 = g.Sum(x => x.all.Debit1 - x.all.Crebit1),
                            Revenue2 = g.Sum(x => x.all.Debit2 - x.all.Crebit2),
                            Revenue3 = g.Sum(x => x.all.Debit3 - x.all.Crebit3),
                            Total = g.Sum(x => x.all.Debit1 - x.all.Crebit1) + g.Sum(x => x.all.Debit2 - x.all.Crebit2) + g.Sum(x => x.all.Debit3 - x.all.Crebit3),
                            Date = data.all.Date,
                            Month = data.all.Month,
                            Year = data.all.Year,
                        }).OrderBy(s => s.ID).ToList();
            return list;
        }
    }
}


