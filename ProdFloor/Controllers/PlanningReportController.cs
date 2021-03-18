using Microsoft.AspNetCore.Mvc;
using ProdFloor.Models;
using ProdFloor.Models.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using Microsoft.AspNetCore.Hosting;
using System.Drawing;
using Microsoft.AspNetCore.Identity;
using Spire.Doc;
using System.Diagnostics;

namespace ProdFloor.Controllers
{
    public class PlanningReportController : Controller
    {
        private IItemRepository itemRepository;
        private UserManager<AppUser> userManager;
        private IHostingEnvironment _env;
        public int PageSize = 4;
        private object missing = System.Reflection.Missing.Value;

        public PlanningReportController(IItemRepository repo, UserManager<AppUser> userMgr, IHostingEnvironment env)
        {
            itemRepository = repo;
            userManager = userMgr;
            _env = env;
        }


        public ViewResult Index()
        {
            PlanningReportListViewModel viewModel = new PlanningReportListViewModel();

            bool isAnyPlanningInDb = itemRepository.PlanningReports.Any();

            if (isAnyPlanningInDb)
            {
                viewModel.planningReport = itemRepository.PlanningReports.FirstOrDefault();
                viewModel.planningReportRows = itemRepository.PlanningReportRows
                                                             .Where(m => m.PlanningReportID == viewModel.planningReport.PlanningReportID)
                                                             .ToList();

                return View(viewModel);
            }

            PlanningReport planning = new PlanningReport
            {
                PlanningDate = DateTime.Now,
                DateTimeLoad = DateTime.Now,
                Busy = true,
            };

            itemRepository.SavePlanningReport(planning);

            PlanningReport planningReport = itemRepository.PlanningReports.FirstOrDefault();
            List<PlanningReportRow> reportRows = GetPlanningReportTable();

            if (reportRows.Count == 0)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Planning Schedule Report file doesnt exists or must be renamed";

                return View();
            }

            foreach (PlanningReportRow row in reportRows)
            {
                row.PlanningReportID = planning.PlanningReportID;
                row.Custom = false;
                itemRepository.SavePlanningReportRow(row);
            }

            planningReport.Busy = false;
            itemRepository.SavePlanningReport(planning);

            viewModel.planningReport = planningReport;
            viewModel.planningReportRows = reportRows;

            return View(viewModel);


        }

        [HttpPost]
        public IActionResult Update(PlanningReportListViewModel viewModel)
        {
            PlanningReport planningReport = itemRepository.PlanningReports.FirstOrDefault();
            planningReport.Busy = true;
            itemRepository.SavePlanningReport(planningReport);

            List<PlanningReportRow> oldReportRows = new List<PlanningReportRow>();
            List<PlanningReportRow> NewReportRows = GetPlanningReportTable();
            if (NewReportRows.Count == 0)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Planning Schedule Report file doesnt exists or must be renamed";

                return RedirectToAction(nameof(Index));
            }

            try
            {
                oldReportRows = viewModel.planningReportRows
                                          .Where(m => m.Custom == true)
                                          .ToList();
            }
            catch { }

            

            foreach (PlanningReportRow row in NewReportRows)
            {
                if (oldReportRows.Any(m => m.PO == row.PO))
                {
                    row.Custom = true;
                }

            }

            itemRepository.DeleteAllPlanningRowsTable();

            foreach (PlanningReportRow row in NewReportRows)
            {
                row.PlanningReportID = planningReport.PlanningReportID;
                itemRepository.SavePlanningReportRow(row);
            }

            planningReport.Busy = false;
            planningReport.DateTimeLoad = DateTime.Now;
            planningReport.PlanningDate = DateTime.Now;
            itemRepository.SavePlanningReport(planningReport);

            return RedirectToAction(nameof(Index));
        }

        public ViewResult NewPrintable()
        {
            //GenerateWord();
            return View(new PlanningReportListViewModel());
        }

        [HttpPost]
        public IActionResult NewPrintable(PlanningReportListViewModel viewModel)
        {
            PlanningReportRow reportRow = itemRepository.PlanningReportRows
                                                        .FirstOrDefault(m => m.PO == viewModel.POSearch);

            if(reportRow == null)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"El job que esta buscando no existe";
            }
            else
            {
                viewModel.Custom = reportRow.Custom;
                viewModel.ReportRow = reportRow;
                viewModel.DueDate = DateTime.Now;
            }

             return View(viewModel);
        }

        [HttpPost]
        public IActionResult Printables(PlanningReportListViewModel viewModel)
        {
            PlanningReportRow reportRow = itemRepository.PlanningReportRows
                                                        .FirstOrDefault(m => m.PO == viewModel.POSearch);

            if (reportRow == null)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"El job que esta buscando no existe";

                return RedirectToAction("NewPrintable");
            }

            PlanningReport planning = itemRepository.PlanningReports
                                                    .FirstOrDefault(m => m.PlanningReportID == reportRow.PlanningReportID);
            if (planning.Busy)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Planning en mantenimiento, regrese despues";

                return RedirectToAction("NewPrintable");
            }

            reportRow.Custom = viewModel.Custom;
            itemRepository.SavePlanningReportRow(reportRow);

            viewModel.ReportRow = reportRow;

            return View(viewModel);
        }

        public List<PlanningReportRow> GetPlanningReportTable()
        {
            string fileName = @"wwwroot\resources\PlanningScheduleReport" + DateTime.Now.ToString("MM-dd-yyy") + ".xlsx";

            if (!System.IO.File.Exists(Path.Combine(fileName)))
            {
                return new List<PlanningReportRow>();
            }

            List<PlanningReportRow> planningReportRowTable = new List<PlanningReportRow>();
            List<string> rowList = new List<string>();
            ISheet sheet;
            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                stream.Position = 0;
                XSSFWorkbook xssWorkbook = new XSSFWorkbook(stream);
                sheet = xssWorkbook.GetSheetAt(3);
                IRow headerRow = sheet.GetRow(0);
                int cellCount = headerRow.LastCellNum;
                for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                {
                    PlanningReportRow planningRow = new PlanningReportRow();
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;
                    if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                    for (int j = row.FirstCellNum; j < cellCount; j++)
                    {
                        if (row.GetCell(j) != null)
                        {
                            if (string.IsNullOrWhiteSpace(row.GetCell(j).ToString()))
                                row.GetCell(j).SetCellValue("N/A");

                            rowList.Add(row.GetCell(j).ToString());
                        }
                    }
                    if (rowList.ElementAt(3) == "MRP")
                    {
                        rowList.Clear();
                        continue;
                    }

                    planningRow.JobNumber = rowList.ElementAt(0);
                    planningRow.LineNumber = int.Parse(rowList.ElementAt(1));
                    planningRow.Material = rowList.ElementAt(2);
                    planningRow.MRP = rowList.ElementAt(3);
                    planningRow.PO = int.Parse(rowList.ElementAt(4));
                    planningRow.SoldTo = rowList.ElementAt(6);
                    planningRow.Consecutive = int.Parse(rowList.ElementAt(7));
                    planningRow.JobName = rowList.ElementAt(8);
                    planningRow.PreviousWorkCenter = rowList.ElementAt(9);
                    planningRow.WorkCenter = rowList.ElementAt(10);
                    planningRow.Notes = rowList.ElementAt(11);
                    planningRow.Priority = rowList.ElementAt(12);

                    DateTime shippingDate = DateTime.Parse(rowList.ElementAt(13));
                    planningRow.ShippingDate = shippingDate.ToShortDateString();


                    planningReportRowTable.Add(planningRow);
                    rowList.Clear();
                }
            }

            return planningReportRowTable;
        }

        public IActionResult GenerateJobTraveler(PlanningReportListViewModel viewModel)
        {

            PlanningReportRow reportRow = itemRepository.PlanningReportRows
                                                        .FirstOrDefault(m => m.PO == viewModel.POSearch);

            viewModel.ReportRow = reportRow; 
            string EngName = "HUNG L.";
            string EngNumberString = reportRow.MRP.Remove(0, 1);
            string EngNameAUx = "";

            int EngNumber = int.Parse(EngNumberString);

            try
            {
                EngNameAUx = userManager.Users.FirstOrDefault(m => m.EngID == EngNumber).ShortFullName.ToUpper();

                if (!string.IsNullOrEmpty(EngNameAUx))
                    EngName = EngNameAUx;
            }
            catch { }

            
            string rootFolder = _env.WebRootPath.ToString();
            string filename = "JobTravelerV6-" + DateTime.Now.ToString("MM-dd-yyyy") + ".docx";
            byte[] toArray = null;
            var memoryStream = new MemoryStream();

            Document doc = new Document();
            try
            {
                
                doc.LoadFromFile(rootFolder + @"\resources\JobTravelerV6-Template.docx");
            }
            catch
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error al leer la plantilla";

                return View("Printables", viewModel);
            }
            


            try
            {
                doc.Replace("JobName", reportRow.JobName.ToUpper(), true, true);
                doc.Replace("Item", reportRow.JobName.ToUpper(), true, true);
                doc.Replace("JobNum", reportRow.JobNumber.ToString().ToUpper(), true, true);
                doc.Replace("PONum", reportRow.PO.ToString(), true, true);
                doc.Replace("ShippingDate", reportRow.ShippingDate, true, true);
                doc.Replace("MATERIAL", reportRow.Material.ToUpper(), true, true);
                doc.Replace("DueDate", viewModel.DueDate.ToShortDateString(), true, true);
                doc.Replace("CARNUMBER", viewModel.CarNumber.ToUpper(), true, true);
                doc.Replace("ConfigGuy", viewModel.ConfigGuy.ToUpper(), true, true);
                doc.Replace("EngName", EngName.ToUpper(), true, true); 

                using (memoryStream)
                {
                    doc.SaveToStream(memoryStream, FileFormat.Docx2013);
                    toArray = memoryStream.ToArray();
                    memoryStream.Position = 0;
                }
            }
            catch
            {
                doc.Close();

                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error al generar el documento";

                return View("Printables", viewModel);
            }

           
            
            return File(toArray, "application/msword", filename);
        }

        /*
        public IActionResult GenerateJobTraveler(PlanningReportListViewModel viewModel)
        {
            PlanningReportRow reportRow = itemRepository.PlanningReportRows
                                                        .FirstOrDefault(m => m.PO == viewModel.POSearch);

            viewModel.ReportRow = reportRow;
            string EngName = "HUNG L.";
            string EngNumberString = reportRow.MRP.Remove(0,1);
            string EngNameAUx = "";

            int EngNumber = int.Parse(EngNumberString);

            try
            {
                EngNameAUx = userManager.Users.FirstOrDefault(m => m.EngID == EngNumber).ShortFullName.ToUpper();

                if (!string.IsNullOrEmpty(EngNameAUx))
                    EngName = EngNameAUx;
            }
            catch { }
            

            Application application = new Application();
            string rootFolder = _env.WebRootPath.ToString();

            Document document = application.Documents.Add(rootFolder + @"\resources\JobTravelerV6-Template.docx");
            string folderDesktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            try
            {
             
                ReplaceAllParagraphs(application.Selection.Find, "JobName", reportRow.JobName.ToUpper());
                ReplaceAllParagraphs(application.Selection.Find, "Item", reportRow.LineNumber.ToString());
                ReplaceAllParagraphs(application.Selection.Find, "JobNum", reportRow.JobNumber.ToString().ToUpper());
                ReplaceAllParagraphs(application.Selection.Find, "PONum", reportRow.PO.ToString());
                ReplaceAllParagraphs(application.Selection.Find, "ShippingDate",reportRow.ShippingDate);
                ReplaceAllParagraphs(application.Selection.Find, "MATERIAL", reportRow.Material.ToUpper());

                ReplaceAllParagraphs(application.Selection.Find, "DueDate", viewModel.DueDate.ToShortDateString());
                ReplaceAllParagraphs(application.Selection.Find, "CARNUMBER", viewModel.CarNumber.ToUpper());
                ReplaceAllParagraphs(application.Selection.Find, "ConfigGuy", viewModel.ConfigGuy.ToUpper());
                ReplaceAllParagraphs(application.Selection.Find, "EngName", EngName.ToUpper());

                object filename = folderDesktop + @"\JobTravelerV6-"+DateTime.Now.ToShortDateString()+".docx";

            
                document.SaveAs2(filename);
            }
            catch(Exception e) {
                string exeption = e.ToString();
                document.Close(false, ref missing, ref missing);
                document = null;
                application.Quit(false, ref missing, ref missing);
                application = null;

                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error al generar el documento";

                return View("Printables", viewModel);
            }
            


            document.Close(ref missing, ref missing, ref missing);
            document = null;
            application.Quit(ref missing, ref missing, ref missing);
            application = null;

            TempData["message"] = $"Documento generado y guardado en el escritorio";
            return View("Printables", viewModel);
        }

        public void ReplaceAllParagraphs(Find findObject, string text,string renplacementText)
        {
            findObject.ClearFormatting();
            findObject.Text = text;
            findObject.Replacement.ClearFormatting();
            findObject.Replacement.Text = renplacementText;

            object replaceAll = WdReplace.wdReplaceAll;
            findObject.Execute(ref missing, ref missing, ref missing, ref missing, ref missing,
                ref missing, ref missing, ref missing, ref missing, ref missing,
                ref replaceAll, ref missing, ref missing, ref missing, ref missing);
        }
        */



    }
}
