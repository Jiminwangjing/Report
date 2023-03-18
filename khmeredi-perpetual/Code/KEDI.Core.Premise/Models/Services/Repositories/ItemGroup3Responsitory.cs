using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.Services.Inventory.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Responsitory
{
    public interface IItemGroup3
    {
        IQueryable<ItemGroup3> ItemGroup3s();
        Task<int> AddItemGroup3(ItemGroup3 itemGroup3);
        ItemGroup3 GetByID(int Id);
        IEnumerable<Background> GetBackgrounds { get; }
        IEnumerable<Colors> GetColors { get; }
        Task<int> DeleteItemGroup3(int ID);
    }
    public class ItemGroup3Responsitory: IItemGroup3
    {
        private readonly DataContext _context;
        public ItemGroup3Responsitory(DataContext dataCotext)
        {
            _context = dataCotext;
        }

        public IEnumerable<Background> GetBackgrounds => _context.Backgrounds.Where(b => b.Delete == false).ToList();

        public IEnumerable<Colors> GetColors => _context.Colors.Where(c => c.Delete == false).ToList();
        public async Task<int> AddItemGroup3(ItemGroup3 itemGroup3)
        {
            if (itemGroup3.ID == 0)
            {
                if (itemGroup3.ColorID == 0 || itemGroup3.BackID == 0 || itemGroup3.BackID==null)
                {
                    int BackID = _context.Colors.Min(x => x.ColorID);
                    int ColorID = _context.Backgrounds.Min(x => x.BackID);
                    itemGroup3.BackID = BackID;
                    itemGroup3.ColorID = ColorID;
                }
                await _context.ItemGroup3.AddAsync(itemGroup3);
            }
            else
            {
                if (itemGroup3.ColorID == 0 || itemGroup3.BackID == 0 || itemGroup3.BackID == null)
                {

                    int BackID = _context.Colors.Min(x => x.ColorID);
                    int ColorID = _context.Backgrounds.Min(x => x.BackID);
                    itemGroup3.BackID = BackID;
                    itemGroup3.ColorID = ColorID;
                }
                _context.ItemGroup3.Update(itemGroup3);

            }
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteItemGroup3(int ID)
        {
            var item3 = await _context.ItemGroup3.FirstAsync(c => c.ID ==ID);
            item3.Delete = true;
            _context.ItemGroup3.Update(item3);
            return await _context.SaveChangesAsync();
        }

        public ItemGroup3 GetByID(int Id) => _context.ItemGroup3.Find(Id);
        

        public IQueryable<ItemGroup3> ItemGroup3s()
        {
            IQueryable<ItemGroup3> list = from item3 in _context.ItemGroup3.Where(i3 => i3.Delete == false)
                                          join
                                         item1 in _context.ItemGroup1.Where(i1 => i1.Delete == false) on
                                         item3.ItemG1ID equals item1.ItemG1ID
                                                                              join
                                         item2 in _context.ItemGroup2.Where(i2 => i2.Delete == false) on
                                         item3.ItemG2ID equals item2.ItemG2ID
                                                                              join
                                         col in _context.Colors.Where(c => c.Delete == false) on
                                         item3.ColorID equals col.ColorID
                                                                              join
                                             bak in _context.Backgrounds.Where(b => b.Delete == false) on
                                             item3.BackID equals bak.BackID
                                          where item3.Delete == false && item3.Name!="None"
                                          select new ItemGroup3
                                          {
                                              ID = item3.ID,
                                              ItemG1ID = item3.ItemG1ID,
                                              ItemG2ID = item3.ItemG2ID,
                                              Name = item3.Name,
                                              ColorID = item3.ColorID,
                                              BackID = item3.BackID,
                                              Images = item3.Images,
                                              ItemGroup1 = new ItemGroup1
                                              {
                                                  Name = item1.Name
                                              },
                                              ItemGroup2 = new ItemGroup2
                                              {
                                                  Name = item2.Name
                                              },
                                              Colors = new Colors
                                              {
                                                  Name = col.Name
                                              },
                                              Background = new Background
                                              {
                                                  Name = bak.Name
                                              }
                                          };
            return list;
        }
    }
}
