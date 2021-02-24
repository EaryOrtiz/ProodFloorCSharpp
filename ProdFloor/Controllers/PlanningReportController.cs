using Microsoft.AspNetCore.Mvc;
using ProdFloor.Models;
using ProdFloor.Models.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace ProdFloor.Controllers
{
    public class PlanningReportController : Controller
    {
        private IItemRepository repository;
        public int PageSize = 4;

        public PlanningReportController(IItemRepository repo)
        {
            repository = repo;
        }
        /*

        public ViewResult List(int page = 1)
            => View(new PlanningReportListViewModel
            {
                planningReport = repository.PlanningReports.FirstOrDefault(),
                PlanningReportRows = repository.PlanningReportRows
                .OrderBy(p => p.PlanningReportRowID)
                .Skip((page - 1) * PageSize)
                .Take(PageSize).ToList(),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = repository.Countries.Count()
                }
            });

        public ViewResult Edit(int ID) =>
            View(repository.PlanningReports
                .FirstOrDefault(j => j.PlanningReportID == ID));

        */
    }
}
