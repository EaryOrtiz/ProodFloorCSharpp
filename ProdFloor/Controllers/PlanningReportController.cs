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

namespace ProdFloor.Controllers
{
    public class PlanningReportController : Controller
    {
        private IItemRepository itemRepository;
        public int PageSize = 4;

        public PlanningReportController(IItemRepository repo)
        {
            itemRepository = repo;
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

            if(reportRows.Count == 0)
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
            
        public IActionResult Update()
        {
            PlanningReport planningReport = itemRepository.PlanningReports.FirstOrDefault();
            planningReport.Busy = true;
            itemRepository.SavePlanningReport(planningReport);

            List<PlanningReportRow> NewReportRows = GetPlanningReportTable();
            if (NewReportRows.Count == 0)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Planning Schedule Report file doesnt exists or must be renamed";

                return RedirectToAction(nameof(Index));
            }

            List<PlanningReportRow> oldReportRows = itemRepository.PlanningReportRows
                                                         .Where(m => m.PlanningReportID == planningReport.PlanningReportID)
                                                         .Where(m => m.Custom == true)
                                                         .ToList();

            foreach (PlanningReportRow row in NewReportRows)
            {
                if(oldReportRows.Any(m => m.PO == row.PO))
                {
                    row.Custom = true;
                }
                
            }

            if (itemRepository.DeleteAllPlanningRowsTable())
            {
                foreach (PlanningReportRow row in NewReportRows)
                {
                    row.PlanningReportID = planningReport.PlanningReportID;
                    itemRepository.SavePlanningReportRow(row);
                }
            }

            planningReport.Busy = false;
            planningReport.DateTimeLoad = DateTime.Now;
            planningReport.PlanningDate = DateTime.Now;
            itemRepository.SavePlanningReport(planningReport);

            return RedirectToAction(nameof(Index));
        }

         public IActionResult Edit(PlanningReportListViewModel viewModel)
        {
            viewModel.planningReport.Busy = true;
            itemRepository.SavePlanningReport(viewModel.planningReport);

            foreach (PlanningReportRow row in viewModel.planningReportRows)
            {
                itemRepository.SavePlanningReportRow(row);
            }

            viewModel.planningReport.Busy = false;
            itemRepository.SavePlanningReport(viewModel.planningReport);

            return RedirectToAction(nameof(Index));
        }

        public List<PlanningReportRow> GetPlanningReportTable()
        {
            string fileName = @"wwwroot\resources\PlanningScheduleReport" + DateTime.Now.ToString("MM-dd-yyy") + ".xlsx";

            if(!System.IO.File.Exists(Path.Combine(fileName)))
            {
                return new List<PlanningReportRow>();
            }

            List < PlanningReportRow > planningReportRowTable = new List<PlanningReportRow>();
            List<string> rowList = new List<string>();
            ISheet sheet;
            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                stream.Position = 0;
                XSSFWorkbook xssWorkbook = new XSSFWorkbook(stream);
                sheet = xssWorkbook.GetSheetAt(3);
                IRow headerRow = sheet.GetRow(0);
                int cellCount = /.LastCellNum;
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
                            if (!string.IsNullOrEmpty(row.GetCell(j).ToString()) & !string.IsNullOrWhiteSpace(row.GetCell(j).ToString()))
                            {
                                rowList.Add(row.GetCell(j).ToString());
                            }
                        }
                    }
                    if (rowList.ElementAt(3) == "MRP") continue;

   
                    planningRow.JobNumber = rowList.ElementAt(0);
                    planningRow.Material = rowList.ElementAt(2);
                    planningRow.MRP = rowList.ElementAt(3);
                    planningRow.PO = int.Parse(rowList.ElementAt(4));
                    planningRow.SoldTo = rowList.ElementAt(6);
                    planningRow.Consecutive =  int.Parse(rowList.ElementAt(7));
                    planningRow.JobName = rowList.ElementAt(8);
                    planningRow.PreviousWorkCenter = rowList.ElementAt(9);
                    planningRow.WorkCenter = rowList.ElementAt(10);
                    planningRow.Notes = rowList.ElementAt(11);
                    planningRow.Priority = rowList.ElementAt(12);


                    planningReportRowTable.Add(planningRow);
                    rowList.Clear();
                }
            }

            return planningReportRowTable;
        }


    }
}
