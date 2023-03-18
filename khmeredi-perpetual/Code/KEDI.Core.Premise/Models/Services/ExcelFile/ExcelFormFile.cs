using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.ExcelFile
{
    public class ExcelFormFile
    {
        public IFormFile FormFile { get; set; }
        public int SheetIndex { get; set; }
        public bool BulkImport { get; set; }
    }
}
