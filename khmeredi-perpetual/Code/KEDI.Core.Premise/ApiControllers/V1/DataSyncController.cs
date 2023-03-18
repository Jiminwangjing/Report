
using CKBS.Models.Services.POS;
using KEDI.Core.Premise.Authorization;
using KEDI.Core.Premise.Models.Sync;
using KEDI.Core.Premise.Models.Sync.Customs.Clients;
using KEDI.Core.Premise.Models.Sync.Customs.Server;
using KEDI.Core.Premise.Repositories.Sync;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.ApiControllers.V1
{ 
    //[ApiController]
    [Privilege(AuthenticationSchemes = "Bearer")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/v1/[Controller]")]
    public class DataSyncController : ControllerBase
    {
        private readonly ISyncServerWorker _syncServerRepo;
        private readonly IPosSyncRepo _posSync;
        public DataSyncController(ISyncServerWorker syncServerRepo, IPosSyncRepo posSync)
        {
            _syncServerRepo = syncServerRepo;
            _posSync = posSync;
        }

        [HttpPost("pullEntityContainer")]
        public async Task<ServerSyncContainer> GetServerSyncContainer()
        {
            var posRefEntities = await _syncServerRepo.GetServerSyncContainerAsync();
            return posRefEntities;
        }

        [HttpPost("pushBackEntityContainer")]
        public async Task PostBackServerSyncContainer([FromBody] Dictionary<Type, IEnumerable<SyncEntity>> entities)
        {
            await _syncServerRepo.UpdateRangeEntityHistoryAsync(entities);
        }

        [HttpPost("pushReceipts")]
        public async Task<IEnumerable<ReceiptContainer>> PushReceipts(
            [FromBody] IEnumerable<ReceiptContainer> receipts
        ){
            await _posSync.AddRangeReceiptAsync(ModelState, receipts);
            return receipts;
        }

        [HttpPost("pushReceiptMemos")]
        public async Task<IEnumerable<ReceiptMemoContainer>> PushReceiptMemos(
            [FromBody] IEnumerable<ReceiptMemoContainer> receiptMemos
        )
        {
            await _posSync.AddRangeReceiptMemoAsync(ModelState, receiptMemos);
            return receiptMemos;
        }

        [HttpPost("pushVoidOrders")]
        public async Task<IEnumerable<VoidOrderContainer>> PushVoidOrders(
            [FromBody] IEnumerable<VoidOrderContainer> voidOrders
        )
        {
            await _posSync.AddRangeVoidOrderAsync(ModelState, voidOrders);
            return voidOrders;
        }

        [HttpPost("pushVoidItems")]
        public async Task<IEnumerable<VoidItemContainer>> PushVoidItems(
            [FromBody] IEnumerable<VoidItemContainer> voidItems
        )
        {
            await _posSync.AddRangeVoidItemAsync(ModelState, voidItems);
            return voidItems;
        }

        [HttpPost("pushOpenShifts")]
        public async Task<IEnumerable<EntryMap<OpenShift>>> PushOpenShifts(
            [FromBody] IEnumerable<EntryMap<OpenShift>> openShifts
        )
        {
            await _posSync.AddRangeOpenShiftAsync(ModelState, openShifts);
            return openShifts;
        }

        [HttpPost("pushCloseShifts")]
        public async Task<IEnumerable<EntryMap<CloseShift>>> PushCloseShifts(
            [FromBody] IEnumerable<EntryMap<CloseShift>> closeShifts
        )
        {
            await _posSync.AddRangeCloseShiftAsync(ModelState, closeShifts);
            return closeShifts;
        }

    }
}
