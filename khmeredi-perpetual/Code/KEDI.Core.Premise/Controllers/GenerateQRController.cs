using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KEDI.Core.Premise.Authorization;

namespace CKBS.Controllers
{
    [Privilege]
    public class GenerateQRController : Controller
    {
        public IActionResult Index(string inputText)
        {
            if (inputText == null)
            {
                inputText = "ABA10022711048.0012038401320pay-Qi4sH2i0XN3bZMPs1409BOOKMEBUS150409b4c62b86270f48dc3f96f8d438010aa4ef19d28";
            }
            using (MemoryStream ms = new MemoryStream())
            {
                QRCodeGenerator qRCodeGenerator = new QRCodeGenerator();
                QRCodeData qRCodeData = qRCodeGenerator.CreateQrCode(inputText, QRCodeGenerator.ECCLevel.Q);
                QRCode qRCode = new QRCode(qRCodeData);
                using (Bitmap bitmap = qRCode.GetGraphic(20))
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    ViewBag.QRCode = "data:image/pag;base64," + Convert.ToBase64String(ms.ToArray());
                }
            }
            return View();            
        }
    }
}
