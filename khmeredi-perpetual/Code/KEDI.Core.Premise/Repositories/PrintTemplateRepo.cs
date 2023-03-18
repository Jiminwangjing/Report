using CKBS.Models.Services.POS;
using CKBS.Models.Services.POS.service;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Repository
{
    public interface IPrintTemplateRepo
    {
        Task<List<PrintBill>> PrintBillAsync(List<PrintBill> printBills);
    }

    public class PrintTemplateRepo : IPrintTemplateRepo
    {
        readonly ILogger<PrintTemplateRepo> _logger;
        readonly HttpClient _httpClient;
        public PrintTemplateRepo(ILogger<PrintTemplateRepo> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<List<PrintBill>> PrintBillAsync(List<PrintBill> printBill)
        {
            _httpClient.BaseAddress = new Uri("http://localhost:1105");
            var response = await _httpClient.PostAsJsonAsync("/POS/PreviewReceipt", printBill);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<List<PrintBill>>();
        }
    }
}
