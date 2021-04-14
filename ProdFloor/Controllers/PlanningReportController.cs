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
            List<PlanningReportRow> planningReportRows = GetPlanningReportTable("PlanningReport");
            List<PlanningReportRow> NFPRows = GetPlanningReportTable("NFP");

            if (planningReportRows.Count == 0)
            {
                viewModel.planningReport = planningReport;
                viewModel.planningReportRows = planningReportRows;

                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Planning Schedule Report file doesnt exists or must be renamed";

                return View(viewModel);
            }

            foreach (PlanningReportRow row in NFPRows)
            {
                if (planningReportRows.Any(m => m.PO == row.PO))
                    continue;

                planningReportRows.Add(row);
            }

            foreach (PlanningReportRow row in planningReportRows)
            {
                row.PlanningReportID = planning.PlanningReportID;
                row.Custom = false;
                itemRepository.SavePlanningReportRow(row);
            }

            planningReport.Busy = false;
            itemRepository.SavePlanningReport(planning);

            viewModel.planningReport = planningReport;
            viewModel.planningReportRows = planningReportRows;

            return View(viewModel);


        }

        [HttpPost]
        public IActionResult Update(PlanningReportListViewModel viewModel)
        {
            PlanningReport planningReport = itemRepository.PlanningReports.FirstOrDefault();
            planningReport.Busy = true;
            itemRepository.SavePlanningReport(planningReport);

            List<PlanningReportRow> oldReportRows = new List<PlanningReportRow>();
            List<PlanningReportRow> planningReportRows = GetPlanningReportTable("PlanningReport");
            List<PlanningReportRow> NFPRows = GetPlanningReportTable("NFP");

            if (planningReportRows.Count == 0)
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

            foreach (PlanningReportRow row in NFPRows)
            {
                if (planningReportRows.Any(m => m.PO == row.PO))
                    continue;

                planningReportRows.Add(row);
            }



            foreach (PlanningReportRow row in planningReportRows)
            {
                if (oldReportRows.Any(m => m.PO == row.PO))
                {
                    row.Custom = true;
                }

            }

            itemRepository.DeleteAllPlanningRowsTable();

            foreach (PlanningReportRow row in planningReportRows)
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
                return View(viewModel);

            }
            
            PlanningReport report = itemRepository.PlanningReports
                                                  .FirstOrDefault(m => m.PlanningReportID == reportRow.PlanningReportID);
            if (report.Busy)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"El planning esta siendo actualizado";
                return View(viewModel);
            }
            
            viewModel.Custom = reportRow.Custom;
            viewModel.ReportRow = reportRow;
            viewModel.DueDate = DateTime.Now;
     

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

        public List<PlanningReportRow> GetPlanningReportTable(string sheetName)
        {
            string fileName = @"wwwroot\resources\Planning\PlanningScheduleReport" + DateTime.Now.ToString("MM-dd-yyy") + ".xlsx";

            if (!System.IO.File.Exists(Path.Combine(fileName)))
            {
                return new List<PlanningReportRow>();
            }

            int JobNumber = 0;
            int LineNumber = 1;
            int Material = 2;
            int MRP = 3;
            int PO = 4;
            int SoldTo = 6;
            int Consecutive = 7;
            int JobName = 8;
            int PreviousWorkCenter = 9;
            int WorkCenter = 10;
            int Notes = 11;
            int Priority = 12;
            int shippingNumber = 13;

            int sheetNumber = 3;
            int lastRow = 14;

            if (sheetName == "NFP")
            {
                JobNumber = 0;
                LineNumber = 1;
                WorkCenter = 2;
                Material = 3;
                MRP = 5;
                PO = 6;
                SoldTo = 8;
                JobName = 9;
                Priority = 11;
                shippingNumber = 12;

                lastRow = 13;
                sheetNumber = 0;
            }

            List<PlanningReportRow> planningReportRowTable = new List<PlanningReportRow>();
            List<string> rowList = new List<string>();
            ISheet sheet;
            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                stream.Position = 0;
                XSSFWorkbook xssWorkbook = new XSSFWorkbook(stream);
                sheet = xssWorkbook.GetSheetAt(sheetNumber);
                IRow headerRow = sheet.GetRow(0);
                int cellCount = lastRow;
                for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                {
                    PlanningReportRow planningRow = new PlanningReportRow();
                    if (sheetName == "NFP")
                    {
                        planningRow.Consecutive = 999;
                        planningRow.PreviousWorkCenter = "N/A";
                        planningRow.Notes = "N/A";
                    }

                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;
                    if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                    for (int j = row.FirstCellNum; j < cellCount; j++)
                    {
                        if (row.GetCell(j) != null)
                        {
                            if (string.IsNullOrWhiteSpace(row.GetCell(j).ToString()) || row.GetCell(j).ToString() == "DROPSHIP")
                            {
                                if(sheetName != "NFP")
                                {
                                    row.GetCell(j).SetCellValue("N/A");
                                }
                                else
                                {
                                    rowList.Clear();
                                    break;
                                }
                                
                            }
                                

                            rowList.Add(row.GetCell(j).ToString());
                        }
                    }

                    if (rowList.Count == 0)
                    {
                        continue;
                    }else if (rowList.ElementAt(MRP) == "MRP")
                    {
                        rowList.Clear();
                        continue;
                    }

                    if(sheetName != "NFP")
                    {
                        planningRow.Consecutive = int.Parse(rowList.ElementAt(Consecutive));
                        planningRow.PreviousWorkCenter = rowList.ElementAt(PreviousWorkCenter);
                        planningRow.Notes = rowList.ElementAt(Notes);
                    }
                    

                    planningRow.JobNumber = rowList.ElementAt(JobNumber);
                    planningRow.LineNumber = int.Parse(rowList.ElementAt(LineNumber));
                    planningRow.Material = rowList.ElementAt(Material);
                    planningRow.MRP = rowList.ElementAt(MRP);
                    planningRow.PO = int.Parse(rowList.ElementAt(PO));
                    planningRow.SoldTo = rowList.ElementAt(SoldTo);
                    planningRow.JobName = rowList.ElementAt(JobName);
                    planningRow.WorkCenter = rowList.ElementAt(WorkCenter);
                    
                    planningRow.Priority = rowList.ElementAt(Priority);

                    DateTime shippingDate = DateTime.Parse(rowList.ElementAt(shippingNumber));
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
            string EngName = "HUNG L";
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
                doc.Replace("Item", reportRow.LineNumber.ToString(), true, true);
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

            string filename = "JobTravelerV6-" + reportRow.PO.ToString() + ".docx";


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
