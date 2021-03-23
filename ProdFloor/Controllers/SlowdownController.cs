using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProdFloor.Models;
using ProdFloor.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using ProdFloor.Models.ViewModels.SlowDown;

namespace ProdFloor.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SlowdownController : Controller
    {
        private IItemRepository repository;
        private ItemController itemController;
        public int PageSize = 15;

        public SlowdownController(IItemRepository repo,
            ItemController item)
        {
            repository = repo;
            itemController = item;
        }

        public IActionResult List(SlowdownListViewModel viewModel, int page = 1, int totalitemsfromlastsearch = 0)
        {
            if (viewModel.CleanFields) return RedirectToAction("List");
            IQueryable<Slowdown> slowdowns = repository.Slowdowns.AsQueryable();

            if (viewModel.CarSpeedFPM > 0) slowdowns = slowdowns.Where(m => m.CarSpeedFPM == viewModel.CarSpeedFPM);
            if (viewModel.Distance > 0) slowdowns = slowdowns.Where(m => m.Distance == viewModel.Distance);
            if (viewModel.A > 0) slowdowns = slowdowns.Where(m => m.A == viewModel.A);
            if (viewModel.SlowLimit > 0) slowdowns = slowdowns.Where(m => m.SlowLimit == viewModel.SlowLimit);
            if (viewModel.MiniumFloorHeight > 0) slowdowns = slowdowns.Where(m => m.MiniumFloorHeight == viewModel.MiniumFloorHeight);

            viewModel.TotalItems = repository.Slowdowns.Count();

            int TotalItemsSearch = slowdowns.Count();
            if (page == 1)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
            }
            else if (TotalItemsSearch != totalitemsfromlastsearch)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
                page = 1;
            }
            viewModel.Slowdowns = slowdowns.OrderBy(p => p.CarSpeedFPM).Skip((page - 1) * 10).Take(10).ToList();
            viewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = page,
                ItemsPerPage = 10,
                TotalItemsFromLastSearch = totalitemsfromlastsearch,
                TotalItems = slowdowns.Count()
            };
            return View(viewModel);
        }

        public ViewResult Edit(int ID) =>
            View(repository.Slowdowns
                .FirstOrDefault(j => j.SlowdownID == ID));

        [HttpPost]
        public IActionResult Edit(Slowdown slowdown)
        {
            if (ModelState.IsValid)
            {
                repository.SaveSlowdown(slowdown);
                TempData["message"] = $"{slowdown.SlowdownID} has been saved..";
                return RedirectToAction("List");
            }
            else
            {
                // there is something wrong with the data values
                return View(slowdown);
            }
        }

        [HttpPost]
        public IActionResult Delete(int ID)
        {
            Slowdown deletedSlowdown = repository.DeleteSlowdown(ID);

            if (deletedSlowdown != null)
            {
                TempData["message"] = $"{deletedSlowdown.SlowdownID} was deleted";
            }
            return RedirectToAction("List");
        }

        [HttpPost]
        public IActionResult SeedXML(string buttonImportXML)
        {
            string resp = buttonImportXML;
            itemController.ImportXML(HttpContext.RequestServices, resp);
            return RedirectToAction(nameof(List));
        }

        public ViewResult Add() => View("Edit", new Slowdown());
    }
}
