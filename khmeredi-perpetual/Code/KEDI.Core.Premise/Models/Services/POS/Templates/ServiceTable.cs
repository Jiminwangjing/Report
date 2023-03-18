using CKBS.Models.Services.Administrator.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.POS.service
{
    public class ServiceTable
    {
        public List<GroupTable> GroupTables { get; set; }
        public List<Table> Tables { get; set; }
    }
}
