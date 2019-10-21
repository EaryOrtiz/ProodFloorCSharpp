using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProdFloor.Models;
using ProdFloor.Models.ViewModels;
using ProdFloor.Models.ViewModels.Stations;

namespace ProdFloor.Controllers
{
    [Authorize(Roles = "Admin,TechAdmin")]
    public class StationController : Controller
    {
        private IItemRepository itemprepo;
        private ITestingRepository testingrepo;
        public int PageSize = 7;

        public StationController(ITestingRepository repo, IItemRepository repo2)
        {
            testingrepo = repo;
            itemprepo = repo2;
        }

        public ViewResult Add()
        {
            return View("Edit", new Station());
        }

        public IActionResult List(StationListViewModel viewModel, int page = 1)
        {
            if (viewModel.CleanFields) return RedirectToAction("List");
            IQueryable<Station> stations = testingrepo.Stations.AsQueryable();

            if (viewModel.JobTypeID > 0) stations = stations.Where(m => m.JobTypeID == viewModel.JobTypeID);
            if (!string.IsNullOrEmpty(viewModel.Label)) stations = stations.Where(m => m.Label.Contains(viewModel.Label));

            viewModel.Stations = stations.OrderBy(p => p.Label).Skip((page - 1) * 5).Take(5).ToList();
            viewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = page,
                ItemsPerPage = 5,
                TotalItems = stations.Count()
            };
            return View(viewModel);
        }

        public ViewResult Edit(int ID) =>
            View(testingrepo.Stations
                .FirstOrDefault(j => j.StationID == ID));

        [HttpPost]
        public IActionResult Edit(Station station)
        {
            if (ModelState.IsValid)
            {
                testingrepo.SaveStation(station);
                TempData["message"] = $"{station.Label} has been saved...{station.StationID}";
                return RedirectToAction("List");
            }
            else
            {
                // there is something wrong with the data values
                return View(station);
            }
        }

        [HttpPost]
        public IActionResult Delete(int ID)
        {
            Station deletedStation = testingrepo.DeleteStation(ID);

            if (deletedStation != null)
            {
                TempData["message"] = $"{deletedStation.Label} was deleted";
            }
            return RedirectToAction("List");
        }

        [HttpPost]
        public IActionResult SeedXML(string buttonImportXML)
        {
            string resp = buttonImportXML;
            ItemController.ImportXML(HttpContext.RequestServices, resp);
            return RedirectToAction(nameof(List));
        }
    }
}