using System.Collections.Generic;

namespace PayWayIntegration.Controllers
{
    public class GetTransactionEntity
    {
        public int status { get; set; }
        public string description { get; set; }
        public int total { get; set; }
        public List<Transaction> transactions { get; set; }
    }
}