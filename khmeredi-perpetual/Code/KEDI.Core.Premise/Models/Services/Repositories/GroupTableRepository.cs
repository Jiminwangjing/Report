using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.Services.Administrator.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Responsitory
{
    public interface IGroupTable
    {
        IQueryable<GroupTable> GroupTables();
        GroupTable GetbyId(int id);
        Task<int> AddOrEdit(GroupTable groupTable);
        Task<int> Delete(int id);
        void InsertGroupTable(GroupTable groupTable);
    }
    public class GroupTableRepository : IGroupTable
    {
        private readonly DataContext _Context;
        public GroupTableRepository(DataContext dataContext)
        {
            _Context = dataContext;
        }
        public async Task<int> AddOrEdit(GroupTable groupTable)
        {
            if (groupTable.ID == 0)
            {
                await _Context.GroupTables.AddAsync(groupTable);
            }
            else
            {
                if (groupTable.Image == null)
                {
                    var table = _Context.GroupTables.Find(groupTable.ID);
                    table.Delete = groupTable.Delete;
                    table.Image = table.Image;
                    table.Name = groupTable.Name;
                    table.Types = groupTable.Types;

                    _Context.GroupTables.Update(table);
                }
                else
                {
                    _Context.GroupTables.Update(groupTable);
                }
             
            }
            return await _Context.SaveChangesAsync();
        }

        public async Task<int> Delete(int id)
        {
            var group = await _Context.GroupTables.FirstAsync(g => g.ID == id);
            group.Delete = true;
            _Context.GroupTables.Update(group);
            return  await _Context.SaveChangesAsync();
        }

        public GroupTable GetbyId(int id) => _Context.GroupTables.Find(id);
      
        public IQueryable<GroupTable> GroupTables()
        {
            IQueryable<GroupTable> list = _Context.GroupTables.Where(g => g.Delete == false);
            return list;
        }

        public  void InsertGroupTable(GroupTable groupTable)
        {
             _Context.GroupTables.Add(groupTable);
            
              _Context.SaveChangesAsync();
        }
    }
}
