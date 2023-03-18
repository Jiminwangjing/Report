
using CKBS.Models.Services.Administrator.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Responsitory
{
    public interface IUserPrivillege
    {
        IQueryable<UserPrivillege> UserPrivilleges { get; }
        IEnumerable<UserPrivillege> UserPrivillege { get; }
        UserPrivillege GetbyId(int id);
        Task Add(UserPrivillege user);
        Task Update(int? id, UserPrivillege user);
        Task Delete(int? id);
    }
    public class UserPrivillegeRepository : IUserPrivillege
    {
        public IQueryable<UserPrivillege> UserPrivilleges => throw new NotImplementedException();

        public IEnumerable<UserPrivillege> UserPrivillege => throw new NotImplementedException();

        public Task Add(UserPrivillege user)
        {
            throw new NotImplementedException();
        }

        public Task Delete(int? id)
        {
            throw new NotImplementedException();
        }

        public UserPrivillege GetbyId(int id)
        {
            throw new NotImplementedException();
        }

        public Task Update(int? id, UserPrivillege user)
        {
            throw new NotImplementedException();
        }
    }
}
