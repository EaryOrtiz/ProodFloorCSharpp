using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProdFloor.Models;
using ProdFloor.Models.ViewModels;
using ProdFloor.Models.ViewModels.Starter;

namespace ProdFloor.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StarterController : Controller
    {
        private IItemRepository repository;
        public int PageSize = 15;

        public StarterController(IItemRepository repo)
        {
            repository = repo;
        }

        public ViewResult List(int page = 1)
            => View(new StarterListViewModel
            {
                Starters = repository.Starters
                .OrderBy(p => p.StarterID)
                .Skip((page - 1) * PageSize)
                .Take(PageSize).ToList(),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = repository.Starters.Count()
                }
            });

        public ViewResult Edit(int ID) =>
            View(repository.Starters
                .FirstOrDefault(j => j.StarterID == ID));

        [HttpPost]
        public IActionResult Edit(Starter starter)
        {
            if (ModelState.IsValid)
            {
                repository.SaveStarter(starter);
                TempData["message"] = $"{starter.StarterID} has been saved..";
                return RedirectToAction("List");
            }
            else
            {
                // there is something wrong with the data values
                return View(starter);
            }
        }

        [HttpPost]
        public IActionResult Delete(int ID)
        {
            Starter deletedStarter = repository.DeleteStarter(ID);

            if (deletedStarter != null)
            {
                TempData["message"] = $"{deletedStarter.StarterID} was deleted";
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

        public ViewResult Add() => View("Edit", new Starter());
    }
}