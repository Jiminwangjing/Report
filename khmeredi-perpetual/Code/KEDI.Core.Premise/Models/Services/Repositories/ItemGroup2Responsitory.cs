using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.Services.Inventory.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Responsitory
{
    public interface IItem2Group
    {
        IQueryable<ItemGroup2> ItemGroup2s();
        Task<int> AddItemGroup2(ItemGroup2 itemGroup2);
        ItemGroup2 getid(int Id);
        IEnumerable<Background> GetBackgrounds { get; }
        IEnumerable<Colors> GetColors { get; }
        Task<int> DeleteItemGroup2(int ID);

    }
    public class ItemGroup2Responsitory : IItem2Group
    {
        private readonly DataContext _contex;
        public ItemGroup2Responsitory(DataContext dataCotext)
        {
            _contex = dataCotext;
        }

        public IEnumerable<Background> GetBackgrounds => _contex.Backgrounds.Where(c => c.Delete == false).ToList();

        public IEnumerable<Colors> GetColors => _contex.Colors.Where(c => c.Delete == false).ToList();

        public async Task<int> AddItemGroup2(ItemGroup2 itemGroup2)
        {
            if (itemGroup2.ItemG2ID == 0)
            {
                if (itemGroup2.ColorID == null || itemGroup2.BackID == null || itemGroup2.ColorID==0 || itemGroup2.BackID==0)
                {
                    int BackID = _contex.Colors.Min(x => x.ColorID);
                    int ColorID = _contex.Backgrounds.Min(x => x.BackID);
                    itemGroup2.BackID = BackID;
                    itemGroup2.ColorID = ColorID;
                }
                await _contex.ItemGroup2.AddAsync(itemGroup2);
            }
            else
            {
                if (itemGroup2.ColorID == null || itemGroup2.BackID == null || itemGroup2.ColorID == 0 || itemGroup2.BackID == 0)
                {
                    int BackID = _contex.Colors.Min(x => x.ColorID);
                    int ColorID = _contex.Backgrounds.Min(x => x.BackID);
                    itemGroup2.BackID = BackID;
                    itemGroup2.ColorID = ColorID;
                }
                _contex.ItemGroup2.Update(itemGroup2);

            }
            return await _contex.SaveChangesAsync();
        }

        public async Task<int> DeleteItemGroup2(int ID)
        {
            var item2 = await _contex.ItemGroup2.FirstAsync(c => c.ItemG2ID == ID);
            item2.Delete = true;
            _contex.ItemGroup2.Update(item2);
            return await _contex.SaveChangesAsync();
        }

        public ItemGroup2 getid(int Id) => _contex.ItemGroup2.Find(Id);

        
       

        public IQueryable<ItemGroup2> ItemGroup2s()
        {
            IQueryable<ItemGroup2> list =
                from item1 in _contex.ItemGroup1.Where(i1 => i1.Delete == false)
                join
                item2 in _contex.ItemGroup2.Where(i2 => i2.Delete == false) on
                item1.ItemG1ID equals item2.ItemG1ID
                                join col in _contex.Colors.Where(c => c.Delete == false) on
                item2.ColorID equals col.ColorID
                                join bak in _contex.Backgrounds.Where(b => b.Delete == false) on
                item2.BackID equals bak.BackID
                where item2.Delete == false && item2.Name!="None"
                select new ItemGroup2
                {
                   
                    ItemG1ID = item2.ItemG1ID,
                    ItemG2ID = item2.ItemG2ID,
                    ColorID = item2.ColorID,
                    BackID = item2.BackID,
                    Name = item2.Name,
                    Images = item2.Images,
                    Background = new Background
                    {
                        Name = bak.Name
                    },
                    Colors = new Colors
                    {
                        Name = col.Name
                    },
                    ItemGroup1 = new ItemGroup1
                    {
                        Name = item1.Name
                        
                       
                    }
                };
            return list;
        }
    }
}
