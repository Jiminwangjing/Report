using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CKBS.Models.Services.Administrator.Tables;
using CKBS.Models.Services.POS;
using CKBS.Models.Services.POS.service;
using CKBS.Models.Services.POS.Template;
using KEDI.Core.Premise.Models.Services.POS.Templates;
using KEDI.Core.Premise.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using Telegram.Bot.Types.Payments;
using Table = CKBS.Models.Services.Administrator.Tables.Table;

namespace CKBS.Models.SignalR
{
    public class SignalRClient : Hub
    {
        private readonly ILogger<SignalRClient> _logger;
        private readonly PosRetailModule _posModule;
        public SignalRClient(ILogger<SignalRClient> logger, PosRetailModule posModule)
        {
            _logger = logger;
            _posModule = posModule;
        }

        private string SerializeObject<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            });
        }

        private T DeserializeObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            });
        }

        /**  FREE SPACE  **/
        public Task AddToGroup(string groupName)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async override Task OnConnectedAsync()
        {
            string userId = Context.UserIdentifier;
            await Clients.User(userId).SendAsync("Connected", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.User(Context.UserIdentifier).SendAsync("Disconnected", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task LoadOrderInfo(int tableId, int orderId, int customerId, bool isOrderOnly = false)
        {
            var orderInfo = await _posModule.GetOrderInfoAsync(tableId, orderId, customerId, true);
            var orderInfoJson = SerializeObject(orderInfo);
            await Clients.All.SendAsync("LoadOrderInfo", orderInfoJson, isOrderOnly);
        }

        public async Task SendOrder(string order = "{}", string table = "{}")
        {
            var _order = DeserializeObject<Order>(order);
            var _currentTable = DeserializeObject<Table>(table);
            await Clients.All.SendAsync("ReceiveOrder", _order, _currentTable);
        }

        public async Task ChangeViewMode(string userSetting = "{}")
        {
            var _setting = DeserializeObject<GeneralSetting>(userSetting);
            await Clients.All.SendAsync("ChangeViewMode", _setting);
        }

        public async Task ClearOrder()
        {
            await Clients.All.SendAsync("ClearOrder");
        }

    }
}
