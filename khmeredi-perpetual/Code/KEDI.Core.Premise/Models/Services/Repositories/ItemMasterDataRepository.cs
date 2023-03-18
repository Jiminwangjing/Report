using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Inventory;
using CKBS.Models.Services.Inventory.Category;
using CKBS.Models.Services.Inventory.PriceList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CKBS.Models.Services.Inventory.Property;
using CKBS.Models.ServicesClass.Property;
using CKBS.Models.ServicesClass.ItemMasterDataView;
using Microsoft.EntityFrameworkCore.Internal;
using KEDI.Core.Premise.Models.Services.Inventory;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using CKBS.Models.Services.Financials;
using Newtonsoft.Json;
using KEDI.Core.Premise.Models.Services.ServiceContractTemplate;
using KEDI.Core.Premise.Models.ServicesClass.TaxGroup;
using KEDI.Core.Premise.Models.Services.Banking;
using KEDI.Core.Premise.Repository;

namespace CKBS.Models.Services.Responsitory
{
    public interface IItemMasterData
    {
        Task<ItemMasterData> CreateAsync(int userId, int companyId, ItemMasterData itemMasterData,
        List<ItemAccounting> itemAccountings, string properties, ModelStateDictionary modelState);
        Task<List<ItemMasterData>> GetItemMastersAsync(int priceListId = 0);
        List<SelectListItem> SelectItemUoms(int groupUomId, int uomId = 0, bool disabled = false);

        //List<OnlyItemMasterDataViewModel> GetItemMasterData(bool InActive, int ComID);
        List<ItemMasterDataViewModel> GetItemMasterData(bool InActive, int ComID);
        IEnumerable<ItemMasterData> GetMasterDatas();
        IEnumerable<ItemMasterData> GetMasterDatasByCategory(int ID);
        ItemMasterData GetbyId(int id);
        Task AddOrEdit(ItemMasterData itemMaster, ModelStateDictionary modelState);
        Task<int> AddWarehouseDeatail(WarehouseDetail warehouseDetail);
        Task<int> AddWarehouseSummary(WarehouseSummary warehouseSummary);
        Task<int> AddPricelistDetail(PriceListDetail priceListDetail);
        Task<int> DeleteItemMaster(int id);
        IEnumerable<PrinterName> GetPrinter { get; }
        string CreateItemCode(string prefix = "IMD", int padLength = 5);
        void RemoveItmeInWarehous(int ItemID);
        Task CreateContractTemplate(ContractTemplate contract);
        IQueryable<TaxGroupViewModel> TaxGroupView();
        Task<List<SelectListItem>> SelectItemGroup1sAsync(int group1Id = 0, bool disabled = false);
        Task<List<SelectListItem>> SelectWarehousesAsync(int warehouseId = 0, bool disabled = false);
        Task<List<SelectListItem>> SelectEntitiesAsync<TEntity>(int entityKey = 0, bool disabled = false)
            where TEntity : class, ISelectListItem, new();
    }

    public class ItemMasterDataRepository : IItemMasterData
    {
        private readonly DataContext _context;
        private readonly UserManager _userModule;
        //private readonly IPropertyRepository _prop;
        public ItemMasterDataRepository(DataContext dataContext, UserManager userModule/*, IPropertyRepository propertyRepository*/)
        {
            _context = dataContext;
            _userModule = userModule;
            //_prop = propertyRepository;
        }

        public IEnumerable<PrinterName> GetPrinter => _context.PrinterNames.Where(p => p.Delete == false).ToList();
        public string CreateItemCode(string prefix = "IMD", int padLength = 5)
        {
            return prefix + (_context.ItemMasterDatas.Count() + 1).ToString().PadLeft(padLength, '0');
        }
        public async Task<ItemMasterData> CreateAsync(int userId, int companyId, ItemMasterData itemMasterData,
        List<ItemAccounting> itemAccountings, string properties, ModelStateDictionary modelState)
        {
            using var t = _context.Database.BeginTransaction();
            var GetCom = _context.Company.FirstOrDefault(w => w.ID == companyId);
            itemMasterData.CompanyID = GetCom.ID;
            await AddOrEdit(itemMasterData, modelState);
            var currency = _context.PriceLists.FirstOrDefault(x => x.ID == itemMasterData.PriceListID && x.Delete == false);
            var companycur = (from com in _context.Company.Where(x => x.ID == GetCom.ID)
                              join cur in _context.Currency.Where(x => !x.Delete) on com.SystemCurrencyID equals cur.ID
                              select new
                              {
                                  CurrencyID = cur.ID
                              }).ToList();
            var defiendUom = _context.GroupDUoMs.Where(x => x.GroupUoMID == itemMasterData.GroupUomID && x.Delete == false).ToList();
            var SysCurrency = 0;
            foreach (var item in companycur)
            {
                SysCurrency = item.CurrencyID;
            }

            //Warenouse Summary
            if (itemMasterData.Process != "Standard")
            {
                WarehouseSummary warehouseSummary = new()
                {
                    WarehouseID = itemMasterData.WarehouseID,
                    ItemID = itemMasterData.ID,
                    InStock = 0,
                    ExpireDate = Convert.ToDateTime(DateTime.Now.ToString("2019/09/09")),
                    SyetemDate = Convert.ToDateTime(DateTime.Today),
                    UserID = userId,
                    TimeIn = Convert.ToDateTime(DateTime.Now.ToString("hh:mm:ss tt")),
                    CurrencyID = companycur.FirstOrDefault().CurrencyID,
                    UomID = Convert.ToInt32(itemMasterData.InventoryUoMID),
                    Cost = 0,
                    Available = 0,
                    Committed = 0,
                    Ordered = 0
                };
                await AddWarehouseSummary(warehouseSummary);
            }

            foreach (var item in defiendUom)
            {
                //Standard
                if (itemMasterData.Process == "Standard")
                {
                    if (item.AltUOM == itemMasterData.SaleUomID)
                    {
                        PriceListDetail priceListDetail = new()
                        {
                            ExpireDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd")),
                            SystemDate = Convert.ToDateTime(DateTime.Today),
                            TimeIn = Convert.ToDateTime(DateTime.Now.ToString("hh:mm:ss tt")),
                            UserID = userId,
                            UomID = itemMasterData.SaleUomID,
                            CurrencyID = currency.CurrencyID,
                            ItemID = itemMasterData.ID,
                            PriceListID = itemMasterData.PriceListID,
                            Cost = itemMasterData.Cost,
                            UnitPrice = itemMasterData.UnitPrice,
                            Barcode = itemMasterData.Barcode
                        };
                        AddPricelistDetail(priceListDetail).Wait();
                    }
                    else
                    {
                        PriceListDetail priceListDetail = new()
                        {

                            ExpireDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd")),
                            SystemDate = Convert.ToDateTime(DateTime.Today),
                            TimeIn = Convert.ToDateTime(DateTime.Now.ToString("hh:mm:ss tt")),
                            UserID = userId,
                            UomID = item.AltUOM,
                            CurrencyID = currency.CurrencyID,
                            ItemID = itemMasterData.ID,
                            PriceListID = itemMasterData.PriceListID,
                            Cost = 0,
                            UnitPrice = 0,
                            Barcode = itemMasterData.Barcode
                        };
                        await AddPricelistDetail(priceListDetail);
                    }

                }
                //FIFO, Average, Serial/Batch, FEFO
                else
                {

                    if (item.AltUOM == itemMasterData.SaleUomID)
                    {
                        PriceListDetail priceListDetail = new()
                        {

                            ExpireDate = Convert.ToDateTime(DateTime.Now.ToString("2019/09/09")),
                            SystemDate = Convert.ToDateTime(DateTime.Today),
                            TimeIn = Convert.ToDateTime(DateTime.Now.ToString("h:mm:ss")),
                            UserID = userId,
                            UomID = itemMasterData.SaleUomID,
                            CurrencyID = currency.CurrencyID,
                            ItemID = itemMasterData.ID,
                            PriceListID = itemMasterData.PriceListID,
                            Cost = itemMasterData.Cost,
                            UnitPrice = itemMasterData.UnitPrice,
                            Barcode = itemMasterData.Barcode
                        };
                        await AddPricelistDetail(priceListDetail);

                    }
                    else
                    {
                        PriceListDetail priceListDetail = new()
                        {

                            ExpireDate = Convert.ToDateTime(DateTime.Now.ToString("2019/09/09")),
                            SystemDate = Convert.ToDateTime(DateTime.Today),
                            TimeIn = Convert.ToDateTime(DateTime.Now.ToString("h:mm:ss")),
                            UserID = userId,
                            UomID = item.AltUOM,
                            CurrencyID = currency.CurrencyID,
                            ItemID = itemMasterData.ID,
                            PriceListID = itemMasterData.PriceListID,
                            Cost = 0,
                            UnitPrice = 0,
                            Barcode = itemMasterData.Barcode
                        };
                        await AddPricelistDetail(priceListDetail);
                    }
                    //Insert to warehoues detail

                    WarehouseDetail warehouseDetail = new()
                    {
                        WarehouseID = itemMasterData.WarehouseID,
                        ItemID = itemMasterData.ID,
                        InStock = 0,
                        ExpireDate = Convert.ToDateTime(DateTime.Now.ToString("2019/09/09")),
                        SyetemDate = Convert.ToDateTime(DateTime.Today),
                        UserID = userId,
                        TimeIn = Convert.ToDateTime(DateTime.Now.ToString("hh:mm:ss tt")),
                        CurrencyID = companycur.FirstOrDefault().CurrencyID,
                        UomID = item.AltUOM,
                        Cost = 0,
                    };
                    await AddWarehouseDeatail(warehouseDetail);
                }
            }

            var masterId = itemMasterData.ID;
            List<PropertyDetails> _properties = JsonConvert.DeserializeObject<List<PropertyDetails>>(properties,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            foreach (var itemAccounting in itemAccountings)
            {
                itemAccounting.ItemID = masterId;
                _context.ItemAccountings.Add(itemAccounting);
                _context.SaveChanges();
            }
            if (_properties != null)
            {
                foreach (var item in _properties)
                {
                    item.ItemID = masterId;
                    _context.PropertyDetails.Update(item);
                    _context.SaveChanges();
                }
            }

            t.Commit();
            return itemMasterData;
        }
        #region item master data
        //public List<OnlyItemMasterDataViewModel> GetItemMasterData(bool InActive, int ComID)
        //{
        //    #region
        //    List<OnlyItemMasterDataViewModel> list = (
        //                                      from item in _context.ItemMasterDatas.Where(d => d.Delete == InActive && d.CompanyID == ComID)
        //                                      join pl in _context.PriceLists.Where(d => d.Delete == false) on item.PriceListID equals pl.ID
        //                                      //join pld in _context.PriceListDetails on pl.ID equals pld.PriceListID
        //                                      join item1 in _context.ItemGroup1.Where(d => d.Delete == false) on item.ItemGroup1ID equals item1.ItemG1ID
        //                                      join item2 in _context.ItemGroup2.Where(d => d.Delete == false) on item.ItemGroup2ID equals item2.ItemG2ID into item_item2
        //                                      from item_2 in item_item2.DefaultIfEmpty()
        //                                      join item3 in _context.ItemGroup3.Where(d => d.Delete == false) on item.ItemGroup3ID equals item3.ID into item_item3
        //                                      from item_3 in item_item3.DefaultIfEmpty()
        //                                      join print in _context.PrinterNames.Where(d => d.Delete == false) on item.PrintToID equals print.ID
        //                                      join _uom in _context.UnitofMeasures on item.SaleUomID equals _uom.ID
        //                                      //join cur in _context.Currency on pl.CurrencyID equals cur.ID
        //                                      where item.Delete == InActive
        //                                      //let _uom = _context.UnitofMeasures.FirstOrDefault(d => d.Delete == false && d.ID == item.SaleUomID) ?? new UnitofMeasure()
        //                                      //let cur = _context.Currency.FirstOrDefault(i=> i.ID == pl.CurrencyID) ?? new Banking.Currency()
        //                                      let uom = _context.GroupDUoMs.FirstOrDefault(i => i.GroupUoMID == item.GroupUomID && i.UoMID == item.SaleUomID) ?? new GroupDUoM()
        //                                      let stock = _context.WarehouseSummary.Where(i => i.ItemID == item.ID).Sum(i => i.InStock)
        //                                      let unitprice = _context.PriceListDetails.Where(i => i.ItemID == item.ID).FirstOrDefault() ?? new PriceListDetail()
        //                                      let instock = item.StockIn * uom.Factor //== 0 || uom.Factor == 0? 0 : item.StockIn * uom.Factor
        //                                      let _unitprice = unitprice.UnitPrice == 0 || uom.Factor == 0 ? 0 : unitprice.UnitPrice * uom.Factor
        //                                      let cost = item.Cost == 0 || uom.Factor == 0 ? 0 : item.Cost * uom.Factor
        //                                      select new OnlyItemMasterDataViewModel
        //                                      {
        //                                          ID = item.ID,
        //                                          Stock = stock,
        //                                          Code = item.Code,
        //                                          KhmerName = item.KhmerName,
        //                                          KhmerName1 = $"{_unitprice} {uom.Factor}",
        //                                          EnglishName = item.EnglishName ?? "",
        //                                          StockIn = unitprice.UnitPrice,
        //                                          Cost = cost,
        //                                          UnitPrice = _unitprice,
        //                                          Barcode = item.Barcode ?? "",
        //                                          Type = item.Type,
        //                                          Description = item.Description,
        //                                          Image = item.Image,
        //                                          ItemGroup1ID = item.ItemGroup1ID,
        //                                          ItemGroup2ID = item.ItemGroup2ID,
        //                                          ItemGroup3ID = item.ItemGroup3ID,
        //                                          PriceListID = item.PriceListID,
        //                                          PrintToID = item.PrintToID,
        //                                          Process = item.Process,
        //                                          SaleUomID = item.SaleUomID,
        //                                          InventoryUoMID = item.InventoryUoMID,
        //                                          UomName = _uom.Name,
        //                                          IG1Name = item1.Name,
        //                                          UnitofMeasureInv = new UnitofMeasure
        //                                          {
        //                                              ID = _uom.ID,
        //                                              Name = _uom.Name

        //                                          },
        //                                          PriceList = new PriceLists
        //                                          {
        //                                              ID = pl.ID,
        //                                              Name = pl.Name
        //                                          },
        //                                          ItemGroup1 = new ItemGroup1
        //                                          {
        //                                              ItemG1ID = item1.ItemG1ID,
        //                                              Name = item1.Name
        //                                          },
        //                                          ItemGroup2 = new ItemGroup2
        //                                          {

        //                                              Name = item_2.Name ?? "None"
        //                                          },
        //                                          ItemGroup3 = new ItemGroup3
        //                                          {

        //                                              Name = item_3.Name ?? "None"
        //                                          },
        //                                          PrinterName = new PrinterName
        //                                          {
        //                                              ID = print.ID,
        //                                              Name = print.Name
        //                                          }
        //                                      }
        //        ).OrderBy(o => o.Code).ToList();
        //    #endregion
        //    var props = _prop.GetActivePropertiesOrdering(ComID);
        //    foreach (var item in list)
        //    {
        //        List<PropertydetailsViewModel> propDeView = new();
        //        var pds = _context.PropertyDetails.Where(i => i.ItemID == item.ID).ToList();
        //        //if(pds.Count < props.Count)
        //        //{
        //        foreach (var prop in props)
        //        {
        //            var notInProps = pds.FirstOrDefault(i => i.ProID == prop.ProID);
        //            if (notInProps == null)
        //            {
        //                pds.Add(new PropertyDetails
        //                {
        //                    ProID = prop.ProID,
        //                    ID = 0,
        //                    ItemID = 0,
        //                    Value = 0,
        //                });
        //            }
        //        }
        //        //}
        //        pds = pds.GroupBy(i => i.ProID).Select(i => i.FirstOrDefault()).ToList();
        //        foreach (var pd in pds)
        //        {
        //            var chpd = _context.ChildPreoperties.FirstOrDefault(_chpd => _chpd.ID == pd.Value) ?? new ChildPreoperty();
        //            var prop = props.FirstOrDefault(pr => pr.ProID == pd.ProID);
        //            if (prop != null)
        //            {
        //                propDeView.Add(new PropertydetailsViewModel
        //                {
        //                    ID = pd.ID,
        //                    ValueName = chpd.Name ?? "",
        //                    ItemID = pd.ItemID,
        //                    ProID = pd.ProID,
        //                    Value = pd.Value
        //                });
        //            }
        //        }
        //        item.Props = props;
        //        item.PropWithName = propDeView.OrderBy(p => p.ProID).ToList();
        //    }
        //    return list;
        //}
        public List<ItemMasterDataViewModel> GetItemMasterData(bool InActive, int ComID)
        {
            #region
            var items = _context.ItemMasterDatas.Where(d => d.Delete == InActive && d.CompanyID == ComID).ToList();
            var list = (from item in items
                        join pl in _context.PriceLists.Where(d => d.Delete == false) on item.PriceListID equals pl.ID
                        join item1 in _context.ItemGroup1.Where(d => d.Delete == false) on item.ItemGroup1ID equals item1.ItemG1ID
                        join _uom in _context.UnitofMeasures on item.SaleUomID equals _uom.ID
                        let uom = _context.GroupDUoMs.FirstOrDefault(i => i.GroupUoMID == item.GroupUomID && i.UoMID == item.SaleUomID) ?? new GroupDUoM()
                        let stock = _context.WarehouseSummary.Where(i => i.ItemID == item.ID).Sum(i => i.InStock)
                        let unitprice = _context.PriceListDetails.Where(i => i.ItemID == item.ID).FirstOrDefault() ?? new PriceListDetail()
                        let instock = item.StockIn * uom.Factor
                        let _unitprice = unitprice.UnitPrice == 0 || uom.Factor == 0 ? 0 : unitprice.UnitPrice * uom.Factor
                        let cost = item.Cost == 0 || uom.Factor == 0 ? 0 : item.Cost * uom.Factor
                        let pd = _context.PropertyDetails.Where(i => i.ItemID == item.ID).ToList()
                        select new ItemMasterDataViewModel
                        {
                            ItemMasterData = new OnlyItemMasterDataViewModel
                            {
                                ID = item.ID,
                                Stock = stock,
                                Code = item.Code,
                                KhmerName = item.KhmerName,
                                KhmerName1 = $"{_unitprice} {uom.Factor}",
                                EnglishName = item.EnglishName ?? "",
                                StockIn = unitprice.UnitPrice,
                                Cost = cost,
                                UnitPrice = _unitprice,
                                Barcode = item.Barcode ?? "",
                                Type = item.Type,
                                Description = item.Description,
                                Image = item.Image,
                                ItemGroup1ID = item.ItemGroup1ID,
                                ItemGroup2ID = item.ItemGroup2ID,
                                ItemGroup3ID = item.ItemGroup3ID,
                                PriceListID = item.PriceListID,
                                PrintToID = item.PrintToID,
                                Process = item.Process,
                                SaleUomID = item.SaleUomID,
                                UnitofMeasureInv = new UnitofMeasure
                                {
                                    ID = _uom.ID,
                                    Name = _uom.Name
                                },
                                ItemGroup1 = new ItemGroup1
                                {
                                    ItemG1ID = item1.ItemG1ID,
                                    Name = item1.Name
                                },
                            },
                            PropertyDetails = pd,
                        }).ToList();
            #endregion
            var props = _context.Property.Where(i => i.Active).ToList();
            if (props.Count > 0)
            {
                List<PropertydetailsViewModel> propDeView = new();
                foreach (var item in list)
                {
                    item.PropertyDetails.ForEach(i =>
                    {
                        var chpd = _context.ChildPreoperties.FirstOrDefault(_chpd => _chpd.ID == i.Value) ?? new ChildPreoperty();
                        var prop = props.FirstOrDefault(pr => pr.ID == i.ProID);
                        if (prop != null)
                        {
                            propDeView.Add(new PropertydetailsViewModel
                            {
                                ID = i.ID,
                                ValueName = chpd.Name ?? "",
                                ItemID = i.ItemID,
                                ProID = i.ProID,
                                Value = i.Value
                            });
                        }
                    });
                    item.PropWithName = propDeView.OrderBy(p => p.ProID).ToList();
                    propDeView = new List<PropertydetailsViewModel>();
                }
            }
            return list.OrderBy(o => o.ItemMasterData.Code).ToList();
        }
        #endregion
        public async Task AddOrEdit(ItemMasterData itemMaster, ModelStateDictionary modelState)
        {
            if (!modelState.IsValid) { return; }

            if (itemMaster.ID <= 0)
            {

                var count_2 = _context.ItemGroup2.Count();
                var count_3 = _context.ItemGroup3.Count();
                var ItemGroup2 = _context.ItemGroup2.FirstOrDefault(x => x.Name == "None");
                var ItemGroup3 = _context.ItemGroup3.FirstOrDefault(x => x.Name == "None");
                if (ItemGroup2 == null)
                {
                    var countBack = _context.Backgrounds.Count();
                    var color = _context.Colors.Count();
                    if (countBack == 0)
                    {
                        Background back = new()
                        {
                            Delete = false,
                            Name = "None"

                        };
                        _context.Backgrounds.Add(back);
                        _context.SaveChanges();
                    }
                    else if (color == 0)
                    {
                        Colors col = new()
                        {
                            Delete = false,
                            Name = "None"
                        };
                        _context.Colors.Add(col);
                        _context.SaveChanges();
                    }
                    else
                    {
                        int BackID = _context.Colors.Min(x => x.ColorID);
                        int ColorID = _context.Backgrounds.Min(x => x.BackID);
                        ItemGroup2 item_2 = new()
                        {
                            BackID = BackID,
                            ColorID = ColorID,
                            Images = "null",
                            ItemG1ID = itemMaster.ItemGroup1ID,
                            Name = "None",
                            Delete = false,
                        };
                        _context.ItemGroup2.Add(item_2);
                        _context.SaveChanges();
                    }
                }
                if (ItemGroup3 == null)
                {
                    var countBack = _context.Backgrounds.Count();
                    var color = _context.Colors.Count();
                    if (countBack == 0)
                    {
                        Background back = new()
                        {
                            Delete = false,
                            Name = "None"

                        };
                        _context.Backgrounds.Add(back);
                        _context.SaveChanges();
                    }
                    else if (color == 0)
                    {
                        Colors col = new()
                        {
                            Delete = false,
                            Name = "None"
                        };
                        _context.Colors.Add(col);
                        _context.SaveChanges();
                    }
                    else
                    {
                        var group2 = _context.ItemGroup2.FirstOrDefault(x => x.Name == "None");
                        int BackID = _context.Colors.Min(x => x.ColorID);
                        int ColorID = _context.Backgrounds.Min(x => x.BackID);
                        ItemGroup3 item_3 = new()
                        {
                            BackID = BackID,
                            ColorID = ColorID,
                            Images = "null",
                            ItemG1ID = itemMaster.ItemGroup1ID,
                            ItemG2ID = group2.ItemG2ID,
                            Name = "None",
                            Delete = false,

                        };
                        _context.ItemGroup3.Add(item_3);
                        _context.SaveChanges();
                    }
                }

                if (count_2 == 0)
                {
                    var countBack = _context.Backgrounds.Count();
                    var color = _context.Colors.Count();
                    if (countBack == 0)
                    {
                        Background back = new()
                        {
                            Delete = false,
                            Name = "None"

                        };
                        _context.Backgrounds.Add(back);
                        _context.SaveChanges();
                    }
                    else if (color == 0)
                    {
                        Colors col = new()
                        {
                            Delete = false,
                            Name = "None"
                        };
                        _context.Colors.Add(col);
                        _context.SaveChanges();
                    }
                    else
                    {
                        int BackID = _context.Colors.Min(x => x.ColorID);
                        int ColorID = _context.Backgrounds.Min(x => x.BackID);
                        ItemGroup2 item_2 = new()
                        {

                            BackID = BackID,
                            ColorID = ColorID,
                            Images = "null",
                            ItemG1ID = itemMaster.ItemGroup1ID,
                            Name = "None",
                            Delete = false,

                        };
                        _context.ItemGroup2.Add(item_2);
                        _context.SaveChanges();
                    }

                }
                if (count_3 == 0)
                {
                    var countBack = _context.Backgrounds.Count();
                    var color = _context.Colors.Count();
                    if (countBack == 0)
                    {
                        Background back = new()
                        {
                            Delete = false,
                            Name = "None"

                        };
                        _context.Backgrounds.Add(back);
                        _context.SaveChanges();
                    }
                    else if (color == 0)
                    {
                        Colors col = new()
                        {
                            Delete = false,
                            Name = "None"
                        };
                        _context.Colors.Add(col);
                        _context.SaveChanges();
                    }
                    else
                    {
                        var group2 = _context.ItemGroup2.FirstOrDefault(x => x.Name == "None");
                        int BackID = _context.Colors.Min(x => x.ColorID);
                        int ColorID = _context.Backgrounds.Min(x => x.BackID);
                        ItemGroup3 item_3 = new()
                        {
                            BackID = BackID,
                            ColorID = ColorID,
                            Images = "null",
                            ItemG1ID = itemMaster.ItemGroup1ID,
                            ItemG2ID = group2.ItemG2ID,
                            Name = "None",
                            Delete = false,

                        };
                        _context.ItemGroup3.Add(item_3);
                        _context.SaveChanges();
                    }
                }

                var item2 = _context.ItemGroup2.FirstOrDefault(x => x.Name == "None");
                var item3 = _context.ItemGroup3.FirstOrDefault(x => x.Name == "None");

                if (itemMaster.ItemGroup2ID == null)
                {
                    itemMaster.ItemGroup2ID = item2.ItemG2ID;
                }
                if (itemMaster.ItemGroup3ID == null)
                {
                    itemMaster.ItemGroup3ID = item3.ID;
                }
                await _context.ItemMasterDatas.AddAsync(itemMaster);
            }
            else
            {
                var count_2 = _context.ItemGroup2.Count();
                var count_3 = _context.ItemGroup3.Count();
                var ItemGroup2 = _context.ItemGroup2.FirstOrDefault(x => x.Name == "None");
                var ItemGroup3 = _context.ItemGroup3.FirstOrDefault(x => x.Name == "None");
                if (ItemGroup2 == null)
                {
                    var countBack = _context.Backgrounds.Count();
                    var color = _context.Colors.Count();
                    if (countBack == 0)
                    {
                        Background back = new()
                        {
                            Delete = false,
                            Name = "None"

                        };
                        _context.Backgrounds.Add(back);
                        _context.SaveChanges();
                    }
                    else if (color == 0)
                    {
                        Colors col = new()
                        {
                            Delete = false,
                            Name = "None"
                        };
                        _context.Colors.Add(col);
                        _context.SaveChanges();
                    }
                    else
                    {
                        int BackID = _context.Colors.Min(x => x.ColorID);
                        int ColorID = _context.Backgrounds.Min(x => x.BackID);
                        ItemGroup2 item_2 = new()
                        {
                            BackID = BackID,
                            ColorID = ColorID,
                            Images = "null",
                            ItemG1ID = itemMaster.ItemGroup1ID,
                            Name = "None",
                            Delete = false,
                        };
                        _context.ItemGroup2.Add(item_2);
                        _context.SaveChanges();
                    }
                }
                if (ItemGroup3 == null)
                {
                    var countBack = _context.Backgrounds.Count();
                    var color = _context.Colors.Count();
                    if (countBack == 0)
                    {
                        Background back = new()
                        {
                            Delete = false,
                            Name = "None"

                        };
                        _context.Backgrounds.Add(back);
                        _context.SaveChanges();
                    }
                    else if (color == 0)
                    {
                        Colors col = new()
                        {
                            Delete = false,
                            Name = "None"
                        };
                        _context.Colors.Add(col);
                        _context.SaveChanges();
                    }
                    else
                    {
                        var group2 = _context.ItemGroup2.FirstOrDefault(x => x.Name == "None");
                        int BackID = _context.Colors.Min(x => x.ColorID);
                        int ColorID = _context.Backgrounds.Min(x => x.BackID);
                        ItemGroup3 item_3 = new()
                        {
                            BackID = BackID,
                            ColorID = ColorID,
                            Images = "null",
                            ItemG1ID = itemMaster.ItemGroup1ID,
                            ItemG2ID = group2.ItemG2ID,
                            Name = "None",
                            Delete = false,

                        };
                        _context.ItemGroup3.Add(item_3);
                        _context.SaveChanges();
                    }
                }

                if (count_2 == 0)
                {
                    var countBack = _context.Backgrounds.Count();
                    var color = _context.Colors.Count();
                    if (countBack == 0)
                    {
                        Background back = new()
                        {
                            Delete = false,
                            Name = "None"

                        };
                        _context.Backgrounds.Add(back);
                        _context.SaveChanges();
                    }
                    else if (color == 0)
                    {
                        Colors col = new()
                        {
                            Delete = false,
                            Name = "None"
                        };
                        _context.Colors.Add(col);
                        _context.SaveChanges();
                    }
                    else
                    {
                        int BackID = _context.Colors.Min(x => x.ColorID);
                        int ColorID = _context.Backgrounds.Min(x => x.BackID);
                        ItemGroup2 item_2 = new()
                        {

                            BackID = BackID,
                            ColorID = ColorID,
                            Images = "null",
                            ItemG1ID = itemMaster.ItemGroup1ID,
                            Name = "None",
                            Delete = false,

                        };
                        _context.ItemGroup2.Add(item_2);
                        _context.SaveChanges();
                    }

                }
                if (count_3 == 0)
                {
                    var countBack = _context.Backgrounds.Count();
                    var color = _context.Colors.Count();
                    if (countBack == 0)
                    {
                        Background back = new()
                        {
                            Delete = false,
                            Name = "None"

                        };
                        _context.Backgrounds.Add(back);
                        _context.SaveChanges();
                    }
                    else if (color == 0)
                    {
                        Colors col = new()
                        {
                            Delete = false,
                            Name = "None"
                        };
                        _context.Colors.Add(col);
                        _context.SaveChanges();
                    }
                    else
                    {
                        var group2 = _context.ItemGroup2.FirstOrDefault(x => x.Name == "None");
                        int BackID = _context.Colors.Min(x => x.ColorID);
                        int ColorID = _context.Backgrounds.Min(x => x.BackID);
                        ItemGroup3 item_3 = new()
                        {
                            BackID = BackID,
                            ColorID = ColorID,
                            Images = "null",
                            ItemG1ID = itemMaster.ItemGroup1ID,
                            ItemG2ID = group2.ItemG2ID,
                            Name = "None",
                            Delete = false,

                        };
                        _context.ItemGroup3.Add(item_3);
                        _context.SaveChanges();
                    }
                }

                var item2 = _context.ItemGroup2.FirstOrDefault(x => x.Name == "None");
                var item3 = _context.ItemGroup3.FirstOrDefault(x => x.Name == "None");

                if (itemMaster.ItemGroup2ID == null)
                {
                    itemMaster.ItemGroup2ID = item2.ItemG2ID;
                }
                if (itemMaster.ItemGroup3ID == null)
                {
                    itemMaster.ItemGroup3ID = item3.ID;
                }
                // Update barcode in price list detail //
                var pldetail = _context.PriceListDetails.FirstOrDefault(i => i.ItemID == itemMaster.ID && i.UomID == itemMaster.InventoryUoMID);
                if (pldetail != null)
                {
                    pldetail.Barcode = itemMaster.Barcode;
                    _context.PriceListDetails.Update(pldetail);
                    _context.SaveChanges();
                }
                _context.ItemMasterDatas.Update(itemMaster);
                //update               
            }
            await _context.SaveChangesAsync();
        }

        public async Task<int> AddPricelistDetail(PriceListDetail priceListDetail)
        {
            await _context.PriceListDetails.AddAsync(priceListDetail);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> AddWarehouseDeatail(WarehouseDetail warehouseDetail)
        {
            await _context.WarehouseDetails.AddAsync(warehouseDetail);
            return await _context.SaveChangesAsync();
        }
        public async Task<int> AddWarehouseSummary(WarehouseSummary warehouseSummarry)
        {
            await _context.WarehouseSummary.AddAsync(warehouseSummarry);
            return await _context.SaveChangesAsync();
        }
        public async Task<int> DeleteItemMaster(int id)
        {
            var item = await _context.ItemMasterDatas.FirstAsync(i => i.ID == id);
            item.Delete = true;
            _context.ItemMasterDatas.Update(item);
            return await _context.SaveChangesAsync();
        }

        public ItemMasterData GetbyId(int id) => _context.ItemMasterDatas.Find(id);

        public IEnumerable<ItemMasterData> GetMasterDatas()
        {
            IEnumerable<ItemMasterData> list = (
                                               from item in _context.ItemMasterDatas.Where(d => d.Delete == false)
                                               join pl in _context.PriceLists.Where(d => d.Delete == false) on item.PriceListID equals pl.ID
                                               join item1 in _context.ItemGroup1.Where(d => d.Delete == false) on item.ItemGroup1ID equals item1.ItemG1ID
                                               join item2 in _context.ItemGroup2.Where(d => d.Delete == false) on item.ItemGroup2ID equals item2.ItemG2ID into item_item2
                                               from item_2 in item_item2.DefaultIfEmpty()
                                               join item3 in _context.ItemGroup3.Where(d => d.Delete == false) on item.ItemGroup3ID equals item3.ID into item_item3
                                               from item_3 in item_item3.DefaultIfEmpty()
                                               join print in _context.PrinterNames.Where(d => d.Delete == false) on item.PrintToID equals print.ID
                                               join uom in _context.UnitofMeasures.Where(d => d.Delete == false) on item.InventoryUoMID equals uom.ID
                                               where item.Delete == false
                                               select new ItemMasterData
                                               {
                                                   ID = item.ID,
                                                   Code = item.Code,
                                                   KhmerName = item.KhmerName,
                                                   EnglishName = item.EnglishName,
                                                   StockIn = item.StockIn,
                                                   Cost = item.Cost,
                                                   UnitPrice = item.UnitPrice,
                                                   Barcode = item.Barcode,
                                                   Type = item.Type,
                                                   Description = item.Description,
                                                   Image = item.Image,
                                                   ItemGroup1ID = item.ItemGroup1ID,
                                                   ItemGroup2ID = item.ItemGroup2ID,
                                                   ItemGroup3ID = item.ItemGroup3ID,
                                                   PriceListID = item.PriceListID,
                                                   PrintToID = item.PrintToID,
                                                   Process = item.Process,
                                                   UnitofMeasureInv = new UnitofMeasure
                                                   {
                                                       ID = uom.ID,
                                                       Name = uom.Name

                                                   },
                                                   PriceList = new PriceLists
                                                   {
                                                       ID = pl.ID,
                                                       Name = pl.Name
                                                   },
                                                   ItemGroup1 = new ItemGroup1
                                                   {
                                                       ItemG1ID = item1.ItemG1ID,
                                                       Name = item1.Name
                                                   },
                                                   ItemGroup2 = new ItemGroup2
                                                   {
                                                       Name = item_2.Name ?? "None"
                                                   },
                                                   ItemGroup3 = new ItemGroup3
                                                   {
                                                       Name = item_3.Name ?? "None"
                                                   },
                                                   PrinterName = new PrinterName
                                                   {
                                                       ID = print.ID,
                                                       Name = print.Name
                                                   }


                                               }
                );
            return list;
        }

        public IEnumerable<ItemMasterData> GetMasterDatasByCategory(int ID)
        {
            IEnumerable<ItemMasterData> list = (
                                              from item in _context.ItemMasterDatas.Where(d => d.Delete == false)
                                              join pl in _context.PriceLists.Where(d => d.Delete == false) on item.PriceListID equals pl.ID
                                              join item1 in _context.ItemGroup1.Where(d => d.Delete == false) on item.ItemGroup1ID equals item1.ItemG1ID
                                              join item2 in _context.ItemGroup2.Where(d => d.Delete == false) on item.ItemGroup2ID equals item2.ItemG2ID into item_item2
                                              from item_2 in item_item2.DefaultIfEmpty()
                                              join item3 in _context.ItemGroup3.Where(d => d.Delete == false) on item.ItemGroup3ID equals item3.ID into item_item3
                                              from item_3 in item_item3.DefaultIfEmpty()
                                              join print in _context.PrinterNames.Where(d => d.Delete == false) on item.PrintToID equals print.ID
                                              join uom in _context.UnitofMeasures.Where(d => d.Delete == false) on item.InventoryUoMID equals uom.ID
                                              where item.Delete == false && item1.ItemG1ID == ID
                                              select new ItemMasterData
                                              {
                                                  ID = item.ID,
                                                  Code = item.Code,
                                                  KhmerName = item.KhmerName,
                                                  EnglishName = item.EnglishName,
                                                  StockIn = item.StockIn,
                                                  Cost = item.Cost,
                                                  UnitPrice = item.UnitPrice,
                                                  Barcode = item.Barcode,
                                                  Type = item.Type,
                                                  Description = item.Description,
                                                  Image = item.Image,
                                                  ItemGroup1ID = item.ItemGroup1ID,
                                                  ItemGroup2ID = item.ItemGroup2ID,
                                                  ItemGroup3ID = item.ItemGroup3ID,
                                                  PriceListID = item.PriceListID,
                                                  PrintToID = item.PrintToID,
                                                  Process = item.Process,
                                                  UnitofMeasureInv = new UnitofMeasure
                                                  {
                                                      ID = uom.ID,
                                                      Name = uom.Name

                                                  },
                                                  PriceList = new PriceLists
                                                  {
                                                      ID = print.ID,
                                                      Name = print.Name
                                                  },
                                                  ItemGroup1 = new ItemGroup1
                                                  {
                                                      ItemG1ID = item1.ItemG1ID,
                                                      Name = item1.Name
                                                  },
                                                  ItemGroup2 = new ItemGroup2
                                                  {
                                                      Name = item_2.Name ?? "None"
                                                  },
                                                  ItemGroup3 = new ItemGroup3
                                                  {
                                                      Name = item_3.Name ?? "None"
                                                  },
                                                  PrinterName = new PrinterName
                                                  {
                                                      ID = print.ID,
                                                      Name = print.Name
                                                  }
                                              }
               );
            return list;
        }

        public void RemoveItmeInWarehous(int ItemID)
        {
            _context.Database.ExecuteSqlCommand("sp_RemoveItemInWarehous @ItemID={0}",
                parameters: new[] {
                    ItemID.ToString()
                });
        }

        public async Task CreateContractTemplate(ContractTemplate contract)
        {
            _context.Contracts.Update(contract);
            await _context.SaveChangesAsync();
        }

        //Update by maksokmanh
        public async Task<List<ItemMasterData>> GetItemMastersAsync(int priceListId = 0)
        {
            var items = _context.ItemMasterDatas.Where(i => !i.Delete).ToList();
            if (priceListId > 0)
            {
                items = (from im in items
                         join pl in _context.PriceLists.Where(p => !p.Delete)
                         on im.PriceListID equals pl.ID
                         select im).ToList();
            }

            return await Task.FromResult(items);
        }

        public List<SelectListItem> SelectItemUoms(int groupUomId, int uomId = 0, bool disabled = false)
        {
            var itemUoms = (
                            from gdu in _context.GroupDUoMs.Where(gd => gd.GroupUoMID == groupUomId)
                            join um in _context.UnitofMeasures on gdu.AltUOM equals um.ID
                            select new SelectListItem
                            {
                                Value = um.ID.ToString(),
                                Text = um.Name,
                                Selected = um.ID == uomId,
                                Disabled = disabled
                            }).ToList();
            return itemUoms;
        }

        public IQueryable<TaxGroupViewModel> TaxGroupView()
        {
            var tagGroup = (from tg in _context.TaxGroups.Where(i => i.Active && i.CompanyID == _userModule.CurrentUser.CompanyID)
                            let tgd = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == tg.ID)
                            .OrderByDescending(i => i.EffectiveFrom).FirstOrDefault() ?? new TaxGroupDefinition()
                            select new TaxGroupViewModel
                            {
                                ID = tg.ID,
                                Name = tg.Name,
                                Code = tg.Code,
                                Effectivefrom = tgd.EffectiveFrom,
                                Rate = tgd.Rate,
                                Type = (int)tg.Type
                            });
            return tagGroup;
        }

        public async Task<List<SelectListItem>> SelectWarehousesAsync(int warehouseId = 0, bool disabled = false)
        {
            var whs = await _context.Warehouses
                .Where(wh => !wh.Delete && wh.BranchID == _userModule.CurrentUser.BranchID)
                .Select(wh => new SelectListItem
                {
                    Value = wh.ID.ToString(),
                    Text = wh.Name,
                    Selected = warehouseId == wh.ID,
                    Disabled = disabled
                }).ToListAsync();
            return whs;
        }

        public async Task<List<SelectListItem>> SelectEntitiesAsync<TEntity>(int entityKey = 0, bool disabled = false) 
            where TEntity : class, ISelectListItem, new()
        {
            var entities = _context.Set<TEntity>()
                .Where(o => !o.Delete)
                .Select(o => new SelectListItem
                {
                    Value = o.ID.ToString(),
                    Text = o.Name,
                    Selected = entityKey == o.ID,
                    Disabled = disabled
                });
            return await entities.ToListAsync();
        }

        public object GetKey(object obj, string propName)
        {
            var propValue = obj.GetType().GetProperty(propName).GetValue(obj, null);
            return propValue;
        }

        public async Task<List<SelectListItem>> SelectItemGroup1sAsync(int group1Id = 0, bool disabled = false)
        {
            var selectListItems = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = "0",
                    Text = "---Select---"
                }
            };
            var itemGroup1s = await (_context.ItemGroup1.Where(g => !g.Delete)
                .Select(g => new SelectListItem
                {
                    Value = g.ItemG1ID.ToString(),
                    Text = g.Name,
                    Selected = g.ItemG1ID == group1Id,
                    Disabled = disabled
                })).ToListAsync();
            selectListItems.AddRange(itemGroup1s);
            return selectListItems;
        }
    }
}
