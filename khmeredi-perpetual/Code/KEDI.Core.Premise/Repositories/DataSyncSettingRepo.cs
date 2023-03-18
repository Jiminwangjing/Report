using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CKBS.AppContext;
using KEDI.Core.Cryptography;
using KEDI.Core.Premise.Models.ClientApi;
using KEDI.Core.Premise.Models.Integrations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace KEDI.Core.Premise.Repositories
{
    public interface IDataSyncSettingRepo {
        Task<int> SaveAsync(ModelStateDictionary modelState, DataSyncSetting setting);
        Task<List<DataSyncSetting>> GetDataSyncSettingsAsync();
        Task<DataSyncSetting> FindAsync(int id);
        Task<DataSyncSetting> RevokedAsync(int id);

    }

    public class DataSyncSettingRepo : IDataSyncSettingRepo
    {
        private readonly DataContext _context;
        public DataSyncSettingRepo(DataContext context){
            _context = context;
        }

        public async Task<int> SaveAsync(ModelStateDictionary modelState, DataSyncSetting setting) {
            if(modelState.IsValid){
                setting.Password = AesSecurity.Encrypt(setting.Password, string.Empty);
                _context.DataSyncSettings.Update(setting);
                await _context.SaveChangesAsync();
            }
            return 0;
        }
        public async Task<List<DataSyncSetting>> GetDataSyncSettingsAsync(){
           
            return await _context.DataSyncSettings.Where(t => !t.Revoked).ToListAsync();
        }
        public async Task<DataSyncSetting> FindAsync(int id){
            var data = await _context.DataSyncSettings.FindAsync(id) ?? new DataSyncSetting();
            return data;
        }

        public async Task<DataSyncSetting> RevokedAsync(int id){
            var data = await _context.DataSyncSettings.FindAsync(id);
            data.Revoked = true;
            await _context.SaveChangesAsync();
            return data;
        }
    }
}