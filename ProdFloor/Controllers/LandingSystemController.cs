using Microsoft.AspNetCore.Mvc;
using ProdFloor.Models;
using ProdFloor.Models.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace ProdFloor.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LandingSystemController : Controller
    {
        private IItemRepository repository;
        public int PageSize = 4;

        public LandingSystemController(IItemRepository repo)
        {
            repository = repo;
        }

        
        public IActionResult List(LandingSystemsListViewModel viewModel, int page = 1)
        {
            if (viewModel.CleanFields) return RedirectToAction("List");
            IQueryable<LandingSystem> landings = repository.LandingSystems.AsQueryable();

            if (viewModel.JobTypeID != 0) viewModel.UsedIn = repository.JobTypes.FirstOrDefault(m => m.JobTypeID == viewModel.JobTypeID).Name;

            if (!string.IsNullOrEmpty(viewModel.UsedIn)) landings = landings.Where(m => m.UsedIn.Contains(viewModel.UsedIn));
            if (!string.IsNullOrEmpty(viewModel.Name)) landings = landings.Where(m => m.Name.Contains(viewModel.Name));

            viewModel.LandingSystems = landings.OrderBy(p => p.Name).Skip((page - 1) * 5).Take(5).ToList();
            viewModel.TotalItems = repository.LandingSystems.Count();
            viewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = page,
                ItemsPerPage = 5,
                TotalItems = landings.Count()
            };
            return View(viewModel);
        }

        public ViewResult Edit(int ID) =>
            View(repository.LandingSystems
                .FirstOrDefault(j => j.LandingSystemID == ID));

        [HttpPost]
        public IActionResult Edit(LandingSystem landingSystem)
        {
            if (ModelState.IsValid)
            {
                repository.SaveLandingSystem(landingSystem);
                TempData["message"] = $"{landingSystem.Name} has been saved...{landingSystem.LandingSystemID}";
                return RedirectToAction("List");    
            }
            else
            {
                // there is something wrong with the data values
                return View(landingSystem);
            }
        }

        [HttpPost]
        public IActionResult Delete(int ID)
        {
            LandingSystem deletedLandingSystem = repository.DeleteLandingSystem(ID);

            if (deletedLandingSystem != null)
            {
                TempData["message"] = $"{deletedLandingSystem.Name} was deleted";
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

        public ViewResult Add() => View("Edit", new LandingSystem());
    }
}
