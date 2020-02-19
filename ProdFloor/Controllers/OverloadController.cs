using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProdFloor.Models;
using ProdFloor.Models.ViewModels;
using ProdFloor.Models.ViewModels.Overload;

namespace ProdFloor.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OverloadController : Controller
    {
        private IItemRepository repository;
        public int PageSize = 15;

        public OverloadController(IItemRepository repo)
        {
            repository = repo;
        }

        public IActionResult List(OverloadListViewModel viewModel, int page = 1, int totalitemsfromlastsearch = 0)
        {
            if (viewModel.CleanFields) return RedirectToAction("List");
            IQueryable<Overload> overloads = repository.Ovearloads.AsQueryable();

            if (viewModel.AMPMin > 0) overloads = overloads.Where(m => m.AMPMin == viewModel.AMPMin);
            if (viewModel.AMPMax > 0) overloads = overloads.Where(m => m.AMPMax == viewModel.AMPMax);
            if (viewModel.OverTableNum > 0) overloads = overloads.Where(m => m.OverTableNum == viewModel.OverTableNum);
            if (!string.IsNullOrEmpty(viewModel.MCPart)) overloads = overloads.Where(m => m.MCPart.Contains(viewModel.MCPart));
            if (!string.IsNullOrEmpty(viewModel.SiemensPart)) overloads = overloads.Where(m => m.SiemensPart.Contains(viewModel.SiemensPart));

            viewModel.TotalItems = repository.Ovearloads.Count();

            int TotalItemsSearch = overloads.Count();
            if (page == 1)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
            }
            else if (TotalItemsSearch != totalitemsfromlastsearch)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
                page = 1;
            }
            viewModel.Overloads = overloads.OrderBy(p => p.AMPMin).Skip((page - 1) * 10).Take(10).ToList();
            viewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = page,
                ItemsPerPage = 10,
                TotalItemsFromLastSearch = totalitemsfromlastsearch,
                TotalItems = overloads.Count()
            };
            return View(viewModel);
        }

        public ViewResult Edit(int ID) =>
            View(repository.Ovearloads
                .FirstOrDefault(j => j.OverloadID == ID));

        [HttpPost]
        public IActionResult Edit(Overload overload)
        {
            if (ModelState.IsValid)
            {
                repository.SaveOverload(overload);
                TempData["message"] = $"{overload.OverloadID} has been saved..";
                return RedirectToAction("List");
            }
            else
            {
                // there is something wrong with the data values
                return View(overload);
            }
        }

        [HttpPost]
        public IActionResult Delete(int ID)
        {
            Overload deletedOverload = repository.DeleteOverload(ID);

            if (deletedOverload != null)
            {
                TempData["message"] = $"{deletedOverload.OverloadID} was deleted";
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

        public ViewResult Add() => View("Edit", new Overload());
    }
}