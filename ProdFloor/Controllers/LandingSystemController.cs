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

        
        public ViewResult List(string jobType,int page = 1)
            => View(new LandingSystemsListViewModel
            {
                LandingSystems = repository.LandingSystems
                .Where(j => jobType == null || j.UsedIn == jobType)
                .OrderBy(p => p.LandingSystemID)
                .Skip((page - 1) * PageSize)
                .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = jobType == null ?
                    repository.LandingSystems.Count() :
                    repository.LandingSystems.Where(e =>
                    e.UsedIn == jobType).Count()
                }
            });

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

        public ViewResult Add() => View("Edit", new LandingSystem());
    }
}
