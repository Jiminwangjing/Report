using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.Services.Inventory.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Responsitory
{
    public interface IItemGroup
    {
        IQueryable<ItemGroup1> GetItemGroup1s();
        Task<int> AddItemGroup(ItemGroup1 itemGroup1);
        ItemGroup1 GetbyID(int ID);
        IEnumerable<Colors> GetColors { get; }
        IEnumerable<Background> Backgrounds { get; }
        Task<int> DeleteItemGroup(int ID);
    }
    public class ItemGorup1Responsitory:IItemGroup
    {
        public readonly DataContext _contex;
        public ItemGorup1Responsitory(DataContext dataCotext)
        {
            _contex = dataCotext;
        }
        public IEnumerable<Colors> GetColors => _contex.Colors.Where(c => c.Delete == false).ToList();

        public IEnumerable<Background> Backgrounds => _contex.Backgrounds.Where(c => c.Delete == false).ToList();

        public async Task<int> AddItemGroup (ItemGroup1 itemGroup1)
        {
            if (itemGroup1.ItemG1ID == 0)
            {
                var countBack = _contex.Backgrounds.Count();
                var color = _contex.Colors.Count();
                if (countBack == 0)
                {
                    Background back = new ()
                    {
                        Delete = false,
                        Name = "None"

                    };
                    _contex.Backgrounds.Add(back);
                    _contex.SaveChanges();
                }
                else if (color == 0)
                {
                    Colors col = new ()
                    {
                        Delete = false,
                        Name = "None"
                    };
                    _contex.Colors.Add(col);
                    _contex.SaveChanges();
                }
                if (itemGroup1.ColorID == 0 || itemGroup1.BackID == 0 || itemGroup1.ColorID == null || itemGroup1.BackID == null)
                { 
                    int BackID= _contex.Colors.Min(x => x.ColorID);
                    int ColorID =_contex.Backgrounds.Min(x => x.BackID);
                    itemGroup1.BackID = BackID;
                    itemGroup1.ColorID = ColorID;
                }
               await _contex.ItemGroup1.AddAsync(itemGroup1);
            }
            else
            { 
                    var countBack = _contex.Backgrounds.Count();
                    var color = _contex.Colors.Count();
                    if (countBack == 0)
                    {
                        Background back = new ()
                        {
                            Delete = false,
                            Name = "None"

                        };
                        _contex.Backgrounds.Add(back);
                        _contex.SaveChanges();
                    }
                    else if (color == 0)
                    {
                        Colors col = new ()
                        {
                            Delete = false,
                            Name = "None"
                        };
                        _contex.Colors.Add(col);
                        _contex.SaveChanges();
                    }
                if (itemGroup1.ColorID == 0 || itemGroup1.BackID == 0 || itemGroup1.ColorID == null || itemGroup1.BackID == null)
                {
                    int BackID = _contex.Colors.Min(x => x.ColorID);
                    int ColorID = _contex.Backgrounds.Min(x => x.BackID);
                    itemGroup1.BackID = BackID;
                    itemGroup1.ColorID = ColorID;
                }
                _contex.ItemGroup1.Update(itemGroup1);
            }
            return await _contex.SaveChangesAsync();
        }

        public async Task<int> DeleteItemGroup(int ID)
        {
            if (ID != 0)
            {
                var cat = await _contex.ItemGroup1.FirstAsync(c => c.ItemG1ID == ID);
                cat.Delete = true;
                _contex.ItemGroup1.Update(cat);

            }
            return await _contex.SaveChangesAsync();
        }

        public IQueryable<ItemGroup1> GetItemGroup1s()
        {
            IQueryable<ItemGroup1> list =
            from s in _contex.ItemGroup1.Where(cate => cate.Delete == false)
            join c in _contex.Colors.Where(col => col.Delete == false) on s.ColorID equals c.ColorID
            join bak in _contex.Backgrounds.Where(b => b.Delete == false) on s.BackID equals bak.BackID
            where s.Delete == false
            select new ItemGroup1
            {
                ItemG1ID=s.ItemG1ID,
                Name=s.Name,
                BackID = s.BackID,
                ColorID = s.ColorID,
                Images = s.Images,
                Colors = new Colors
                {
                    Name=c.Name
                },
                Background = new Background
                {
                    Name=bak.Name
                },
            };
            return list;
        }

        public ItemGroup1 GetbyID(int ID) => _contex.ItemGroup1.Find(ID);

        
    }
}
