using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.Services.Administrator.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Responsitory
{
    public interface ITable
    {
        List<Table> GetTables();
        Table GetById(int id);
        Task<int> AddOrEdit(Table table);
        Task<int> DeleteTable(int id);
    }
    public class TableRepository : ITable
    {
        private readonly DataContext _context;
        public TableRepository(DataContext dataContext)
        {
            _context = dataContext;
        }

        public async Task<int> AddOrEdit(Table table)
        {
            if (table.ID == 0)
            {
                table.Time = "00:00:00";
                await _context.Tables.AddAsync(table);
            }
            else
            {
                if (table.Image == null)
                {
                    var tables = _context.Tables.Find(table.ID);
                    tables.GroupTableID = table.GroupTableID;
                    tables.Image = tables.Image;
                    tables.Type = table.Type;
                    tables.Name = table.Name;
                    tables.Status = table.Status;
                    tables.Delete = table.Delete;
                    tables.Time = table.Time;
                    tables.IsTablePriceList = table.IsTablePriceList;
                    tables.PriceListID = table.PriceListID;
                    _context.Tables.Update(tables);
                }
                else
                {
                    _context.Tables.Update(table);
                }

            }
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteTable(int id)
        {
            var tab = await _context.Tables.FirstAsync(t => t.ID == id);
            tab.Delete = true;
            _context.Tables.Update(tab);
            return await _context.SaveChangesAsync();
        }

        public Table GetById(int id) => _context.Tables.Find(id);

        public List<Table> GetTables()
        {
            List<Table> list = (from tab in _context.Tables.Where(t => t.Delete == false)
                                join grou in _context.GroupTables.Where(g => g.Delete == false) on tab.GroupTableID equals grou.ID
                                select new Table
                                {
                                    ID = tab.ID,
                                    Name = tab.Name,
                                    GroupTableID = tab.GroupTableID,
                                    Image = tab.Image,
                                    Status = tab.Status,
                                    Time = tab.Time,
                                    Type = tab.Type,
                                    GroupTable = new GroupTable
                                    {
                                        Name = grou.Name
                                    }

                                }).ToList();
            return list;
        }


    }
}
