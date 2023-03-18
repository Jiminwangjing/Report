using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using KEDI.Core.Premise.Repository;
using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace KEDI.Core.Premise.Models.SignalR
{
    public interface IReceiver : IClientProxy
    {
        Task SignOut();
        Task Demo(object obj);
    }

    public class HubSystem : Hub<IReceiver>
    {
        readonly UserManager _userModule;
        readonly IHttpContextAccessor _httpAccessor;
        public HubSystem(UserManager userModule, IHttpContextAccessor httpAccessor)
        {
            _userModule = userModule;
            _httpAccessor = httpAccessor;
        }

        public async Task SignOutAsync(string userId)
        {
            await Clients.User(userId).SignOut();
        }

        public async Task DemoAsync()
        {
            await Clients.All.Demo(DateTime.Now.ToString());
        }

        public override async Task OnConnectedAsync()
        {
            string userId = _userModule.GetUserId().ToString();
            if (int.Parse(userId) > 0)
            {
                _userModule.AddLoggedUser(userId, userId);
            } 
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                string userId = _userModule.GetUserId().ToString();
                _userModule.RemoveLoggedUser(userId);
            }
            
            await base.OnDisconnectedAsync(exception);
        }

    }
}
