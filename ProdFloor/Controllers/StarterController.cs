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

        public IActionResult List(StarterListViewModel viewModel, int page = 1, int totalitemsfromlastsearch = 0)
        {
            if (viewModel.CleanFields) return RedirectToAction("List");
            IQueryable<Starter> starters = repository.Starters.AsQueryable();

            if (viewModel.FLA > 0) starters = starters.Where(m => m.FLA == viewModel.FLA);
            if (viewModel.HP > 0) starters = starters.Where(m => m.HP == viewModel.HP);
            if (!string.IsNullOrEmpty(viewModel.StarterType)) starters = starters.Where(m => m.StarterType == viewModel.StarterType);
            if (!string.IsNullOrEmpty(viewModel.Volts)) starters = starters.Where(m => m.Volts.Contains(viewModel.Volts));
            if (!string.IsNullOrEmpty(viewModel.MCPart)) starters = starters.Where(m => m.MCPart.Contains(viewModel.MCPart));
            if (!string.IsNullOrEmpty(viewModel.NewManufacturerPart)) starters = starters.Where(m => m.NewManufacturerPart.Contains(viewModel.NewManufacturerPart));
            if (!string.IsNullOrEmpty(viewModel.OverloadTable)) starters = starters.Where(m => m.OverloadTable == viewModel.OverloadTable);
            
            viewModel.TotalItems = repository.Starters.Count();

            int TotalItemsSearch = starters.Count();
            if (page == 1)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
            }
            else if (TotalItemsSearch != totalitemsfromlastsearch)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
                page = 1;
            }
            viewModel.Starters = starters.OrderBy(p => p.FLA).Skip((page - 1) * 10).Take(10).ToList();
            viewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = page,
                ItemsPerPage = 10,
                TotalItemsFromLastSearch = totalitemsfromlastsearch,
                TotalItems = starters.Count()
            };
            return View(viewModel);
        }

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