using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Controllers.API.QRCodeV1.ClassView
{
    public class QRcodeViewModel
    {
        public  int TableID { get; set; }
        public string EncryptedTableID { get; set; }
        public string QrCodeString { get; set; }
    }
}
