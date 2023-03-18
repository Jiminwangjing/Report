using CKBS.AppContext;
using KEDI.Core.Premise.Models.Services.Responsitory;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using IoFile = System.IO.File;
using KEDI.Core.Premise.Models.Services.ServiceContractTemplate;
using Newtonsoft.Json;
using KEDI.Core.Models.Validation;
using KEDI.Core.Helpers.Enumerations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics.Contracts;

namespace KEDI.Core.Premise.Controllers
{
    public class SetupContractTemplateController : Controller
    {
        private readonly DataContext _context;
        private readonly ISale _isale;
        private readonly IWebHostEnvironment _env;
        public SetupContractTemplateController(DataContext context, ISale sale, IWebHostEnvironment env)
        {
            _context = context;
            _isale = sale;
            _env = env;
        }
        public Dictionary<int, string> Times => EnumHelper.ToDictionary(typeof(Times));
        public IActionResult Index(int id)
        {
            var data = (from con in _context.Contracts.Where(x => x.ID == id)
                        let conver = _context.Converages.FirstOrDefault(x => x.ID == con.ConverageID) ?? new Converage()
                        let re = _context.Remarks.FirstOrDefault(x => x.ID == con.RemarksID) ?? new Remark()
                        let setup = _context.SetupContractTypes.FirstOrDefault(x => x.ID == con.ContracType) ?? new SetupContractType()
                        select new ContractTemplate
                        {
                            ID = con.ID,
                            RemarksID = re.ID,
                            ConverageID = conver.ID,

                            Name = con.Name,
                            ContracType = con.ContracType,
                            ContractName = setup.ContractType,
                            ResponseTime = con.ResponseTime,
                            ResponseTimeDH = con.ResponseTimeDH,
                            ResultionTime = con.ResultionTime,
                            ResultionTimeDH = con.ResultionTimeDH,
                            Description = con.Description,
                            Expired = con.Expired,
                            //=======Remark======
                            Remarks = re.Remarks,
                            //======Covnerage====
                            Part = conver.Part,
                            Labor = conver.Labor,
                            Travel = conver.Travel,
                            Holiday = conver.Holiday,
                            Monthday = conver.Monthday,
                            Thuesday = conver.Thuesday,
                            Wednesday = conver.Wednesday,
                            Thursday = conver.Thursday,
                            Friday = conver.Friday,
                            Saturday = conver.Saturday,
                            Sunday = conver.Sunday,
                            StarttimeMon = conver.StarttimeMon,
                            StarttimeThu = conver.StarttimeThu,
                            StarttimeWed = conver.StarttimeWed,
                            StarttimeThur = conver.StarttimeThur,
                            StarttimeFri = conver.StarttimeFri,
                            StarttimeSat = conver.StarttimeSat,
                            StarttimeSun = conver.StarttimeSun,
                            EndtimeMon = conver.EndtimeMon,
                            EndtimeThu = conver.EndtimeThu,
                            EndtimeWed = conver.EndtimeWed,
                            EndtimeThur = conver.EndtimeThur,
                            EndtimeFri = conver.EndtimeFri,
                            EndtimeSat = conver.EndtimeSat,
                            EndtimeSun = conver.EndtimeSun,
                            AttachmentFiles = (from att in _context.AttachmentFileOfContractTemplates.Where(x => x.ContractTemplateID == con.ID)
                                               select new AttachmentFileOfContractTemplate
                                               {
                                                   LineID = DateTime.Now.Ticks.ToString() + att.ID,
                                                   ID = att.ID,
                                                   ContractTemplateID = att.ContractTemplateID,
                                                   TargetPath = att.TargetPath,
                                                   FileName = att.FileName,
                                                   AttachmentDate = att.AttachmentDate,
                                               }
                                               ).ToList() ?? new List<AttachmentFileOfContractTemplate>()


                        }).FirstOrDefault();
            if (data != null)
            {
                List<AttachmentFileOfContractTemplate> par = new();
                par.AddRange(data.AttachmentFiles);
                for (int i = 1; i <= 5; i++)
                {
                    par.Add(EmpDatapartner());
                }
                data.AttachmentFiles = par;
            }
            ViewBag.ContractType = new SelectList(_context.SetupContractTypes, "ID", "ContractType");
            ViewBag.SetupContractTemplate = "highlight";
            return View(data);

        }
        public IActionResult CreateDefaultRowAttachmetDetailOfContractTemplate(int num, int number)
        {
            var list = _isale.CreateDefaultRowAttachmetDetailOfContractTemplate(num, number);

            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> SaveAttachment()
        {
            AttachmentFileOfContractTemplate obj = new();
            var files = HttpContext.Request.Form.Files;

            foreach (var f in files)
            {
                var savePath = Path.Combine(_env.WebRootPath + "\\js\\Sale\\UploadFileContractTemplate\\" + f.FileName);
                obj.TargetPath = savePath;
                obj.FileName = f.FileName;
                obj.AttachmentDate = DateTime.Now.ToString("yyyy'-'MM'-'dd");
                using Stream fs = IoFile.Create(savePath);
                await f.CopyToAsync(fs);
            }
            return Ok(obj);
        }
        public async Task<IActionResult> DowloadFile(int AttachID)
        {

            AttachmentFileOfContractTemplate attachment = _context.AttachmentFileOfContractTemplates.Find(AttachID) ?? new AttachmentFileOfContractTemplate();

            if (attachment.ID > 0)
            {
                string fullPath = Path.GetFullPath(string.Format("{0}{1}", _env.WebRootPath + "\\js\\Sale\\UploadFileContractTemplate\\", attachment.FileName));
                byte[] bytes = await IoFile.ReadAllBytesAsync(fullPath);
                //Send the File to Download.
                return File(bytes, "application/octet-stream", attachment.FileName);
            }
            return NotFound();
        }
        [HttpPost]
        public IActionResult RemoveFileFromFolderMastre(string file)
        {
            string fullPath = Path.GetFullPath(string.Format("{0}{1}", _env.WebRootPath + "\\js\\Sale\\UploadFileContractTemplate\\", file));
            if (IoFile.Exists(fullPath))
            {
                IoFile.Delete(fullPath);
            }
            return Ok();
        }
        [HttpPost]
        public IActionResult DeleteFileFromDatabase(int id, int psid, string key)
        {
            AttachmentFileOfContractTemplate attachment = _context.AttachmentFileOfContractTemplates.Find(id) ?? new AttachmentFileOfContractTemplate();
            string fullPath = Path.GetFullPath(string.Format("{0}{1}", _env.WebRootPath + "\\js\\Sale\\UploadFileContractTemplate\\", attachment.FileName));
            if (IoFile.Exists(fullPath))
            {
                IoFile.Delete(fullPath);
                _context.AttachmentFileOfContractTemplates.RemoveRange(attachment);
                _context.SaveChanges();
            }
            var list = _isale.GetAttachmentAsync(psid, key);
            return Ok(list);
        }
        [HttpGet]
        public IActionResult GetEmptyTableSetupContractTypeDefalut()
        {
            List<SetupContractType> setupContractTypes = new();
            var data = _context.SetupContractTypes.ToList();
            setupContractTypes.AddRange(data);
            for (var i = 1; i <= 10; i++)
            {
                SetupContractType contracttype = new()
                {
                    LineID = DateTime.Now.Ticks.ToString(),
                    ID = 0,
                    ContractType = ""
                };
                setupContractTypes.Add(contracttype);
            }
            return Ok(setupContractTypes);

        }

        [HttpPost]
        public IActionResult InsertContractType(List<SetupContractType> contractype)
        {
            ModelMessage msg = new();
            List<SetupContractType> list = new();
            foreach (var i in contractype)
            {
                if (!string.IsNullOrWhiteSpace(i.ContractType))
                    list.Add(i);
            }
            if (list.Count == 0)
            {
                ModelState.AddModelError("", "please input Contract Type ...!");
            }
            else
            {
                foreach (var li in list)
                {
                    if (string.IsNullOrWhiteSpace(li.ContractType))
                        ModelState.AddModelError("Contract Type", "please input Description ...!");
                }
            }
            if (ModelState.IsValid)
            {
                _context.SetupContractTypes.UpdateRange(list);
                _context.SaveChanges();
                ModelState.AddModelError("success", "Setup Contract Types created succussfully!");
                msg.Approve();
            }
            var data = _context.SetupContractTypes.ToList();
            return Ok(new { Model = msg.Bind(ModelState), Data = data });
        }
        //public List<AttachmentFileOfContractTemplate> CreateDefaultRowAttachmetDetailOfContractTemplate(int num, int number = 0)
        //{
        //    List<AttachmentFileOfContractTemplate> lstatm = new List<AttachmentFileOfContractTemplate>();
        //    for (var i = 0; i < num - number; i++)
        //    {
        //        string lineId = DateTime.Now.Ticks.ToString();
        //        AttachmentFileOfContractTemplate objatm = new AttachmentFileOfContractTemplate
        //        {
        //            LineID = lineId,
        //            TargetPath = "",
        //            FileName = "",
        //            AttachmentDate = "",

        //        };
        //        lstatm.Add(objatm);
        //    }
        //    return lstatm;
        //}
        private static AttachmentFileOfContractTemplate EmpDatapartner()
        {
            var partner = new AttachmentFileOfContractTemplate
            {
                LineID = DateTime.Now.Ticks.ToString(),
                ID = 0,
                TargetPath = "",
                FileName = "",
                AttachmentDate = ""
            };
            return partner;
        }

        public IActionResult FindContractTemplate(string name)
        {

            var data = (from con in _context.Contracts.Where(x => x.Name == name)
                        let conver = _context.Converages.FirstOrDefault(x => x.ID == con.ConverageID) ?? new Converage()
                        let re = _context.Remarks.FirstOrDefault(x => x.ID == con.RemarksID) ?? new Remark()
                        let setup = _context.SetupContractTypes.FirstOrDefault(x => x.ID == con.ContracType) ?? new SetupContractType()
                        select new ContractTemplate
                        {
                            ID = con.ID,
                            RemarksID = re.ID,
                            ConverageID = conver.ID,
                            Name = con.Name,
                            ContracType = con.ContracType,
                            ContractName = setup.ContractType,
                            ResponseTime = con.ResponseTime,
                            ResponseTimeDH = con.ResponseTimeDH,
                            ResultionTime = con.ResultionTime,
                            ResultionTimeDH = con.ResultionTimeDH,
                            Description = con.Description,
                            Expired = con.Expired,
                            //=======Remark======
                            Remarks = re.Remarks,
                            //======Covnerage====
                            Part = conver.Part,
                            Labor = conver.Labor,
                            Travel = conver.Travel,
                            Holiday = conver.Holiday,
                            Monthday = conver.Monthday,
                            Thuesday = conver.Thuesday,
                            Wednesday = conver.Wednesday,
                            Thursday = conver.Thursday,
                            Friday = conver.Friday,
                            Saturday = conver.Saturday,
                            Sunday = conver.Sunday,
                            StarttimeMon = conver.StarttimeMon,
                            StarttimeThu = conver.StarttimeThu,
                            StarttimeWed = conver.StarttimeWed,
                            StarttimeThur = conver.StarttimeThur,
                            StarttimeFri = conver.StarttimeFri,
                            StarttimeSat = conver.StarttimeSat,
                            StarttimeSun = conver.StarttimeSun,
                            EndtimeMon = conver.EndtimeMon,
                            EndtimeThu = conver.EndtimeThu,
                            EndtimeWed = conver.EndtimeWed,
                            EndtimeThur = conver.EndtimeThur,
                            EndtimeFri = conver.EndtimeFri,
                            EndtimeSat = conver.EndtimeSat,
                            EndtimeSun = conver.EndtimeSun,
                            AttachmentFiles = (from att in _context.AttachmentFileOfContractTemplates.Where(x => x.ContractTemplateID == con.ID)
                                               select new AttachmentFileOfContractTemplate
                                               {
                                                   LineID = DateTime.Now.Ticks.ToString() + att.ID,
                                                   ID = att.ID,
                                                   ContractTemplateID = att.ContractTemplateID,
                                                   TargetPath = att.TargetPath,
                                                   FileName = att.FileName,
                                                   AttachmentDate = att.AttachmentDate,
                                               }
                                               ).ToList() ?? new List<AttachmentFileOfContractTemplate>()

                        }).FirstOrDefault();
            if (data != null)
            {
                List<AttachmentFileOfContractTemplate> par = new();
                par.AddRange(data.AttachmentFiles);
                for (int i = 1; i <= 5; i++)
                {
                    par.Add(EmpDatapartner());
                }
                data.AttachmentFiles = par;
            }
            return Ok(data);
        }

        public IActionResult GetListContractTemplate()
        {
            var data = (from con in _context.Contracts
                        let conver = _context.Converages.FirstOrDefault(x => x.ID == con.ConverageID) ?? new Converage()
                        let cont = _context.SetupContractTypes.FirstOrDefault(w => w.ID == con.ContracType) ?? new SetupContractType()
                        let att = _context.AttachmentFileOfContractTemplates.FirstOrDefault(x => x.ContractTemplateID == con.ID) ?? new AttachmentFileOfContractTemplate()
                        select new ContractTemplate
                        {
                            ID = con.ID,
                            Name = con.Name,
                            Description = con.Description,
                            ContracOfType = cont.ContractType,
                        }
                      ).ToList();
            return Ok(data);
        }
        //private void ValidateSummary(ContractTemplate contracttemplate)
        //{
        //    List<StageDetail> stages = new();


        //    //stage detail
        //    foreach (var i in stageDetail)
        //    {
        //        if (i.Percent != 0 || i.StagesID != 0 || i.DocNo != 0 || i.OwnerID != 0 || i.DocTypeID != 0 || i.SaleEmpselectID != 0)
        //            stages.Add(i);
        //    }
        //    if (stages.Count == 0)
        //    {
        //        ModelState.AddModelError("stages", "please input Data In Stage Detail");
        //    }
        //    else
        //    {
        //        foreach (var li in stages)
        //        {

        //            if (li.SaleEmpselectID == 0)
        //                ModelState.AddModelError("SaleEmpselect", "please select Sale Employee in Stage");
        //            if (li.StagesID == 0)
        //                ModelState.AddModelError("StagesID", "please select Stage in Stage");


        //        }
        //    }

        //}

        public IActionResult SubmmitData(string _data)
        {
            ContractTemplate data = JsonConvert.DeserializeObject<ContractTemplate>(_data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            data.AttachmentFiles = data.AttachmentFiles.Where(i => i.FileName != "").ToList();
            //ValidateSummary(data, data.AttachmentFiles);
            ModelMessage msg = new();
            if (ModelState.IsValid)
            {
                _context.Contracts.Update(data);
                _context.SaveChanges();
                ModelState.AddModelError("success", "Setup Contract Template created succussfully!");
                msg.Approve();
            }
            return Ok(msg.Bind(ModelState));
        }
    }

}
