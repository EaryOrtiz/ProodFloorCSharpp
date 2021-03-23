using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProdFloor.Models;
using ProdFloor.Models.ViewModels;
using ProdFloor.Models.ViewModels.WireTypesSize;

namespace ProdFloor.Controllers
{
    [Authorize(Roles = "Admin")]
    public class WireTypesSizeController : Controller
    {
        private IItemRepository repository;
        private ItemController itemController;
        public int PageSize = 15;

        public WireTypesSizeController(IItemRepository repo,
            ItemController item)
        {
            repository = repo;
            itemController = item;
        }

        public IActionResult List(WireTypesSizeListViewModel viewModel, int page = 1, int totalitemsfromlastsearch = 0)
        {
            if (viewModel.CleanFields) return RedirectToAction("List");
            IQueryable<WireTypesSize> wires = repository.WireTypesSizes.AsQueryable();

            if (viewModel.AMPRating > 0) wires = wires.Where(m => m.AMPRating == viewModel.AMPRating);
            if (!string.IsNullOrEmpty(viewModel.Type)) wires = wires.Where(m => m.Type.Contains(viewModel.Type));
            if (!string.IsNullOrEmpty(viewModel.Size)) wires = wires.Where(m => m.Size.Contains(viewModel.Size));

            
            viewModel.TotalItems = repository.WireTypesSizes.Count();

            int TotalItemsSearch = wires.Count();
            if (page == 1)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
            }
            else if (TotalItemsSearch != totalitemsfromlastsearch)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
                page = 1;
            }
            viewModel.WireTypes = wires.OrderBy(p => p.Size).Skip((page - 1) * 10).Take(10).ToList();
            viewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = page,
                ItemsPerPage = 10,
                TotalItemsFromLastSearch = totalitemsfromlastsearch,
                TotalItems = wires.Count()
            };
            return View(viewModel);
        }

        public ViewResult Edit(int ID) =>
            View(repository.WireTypesSizes
                .FirstOrDefault(j => j.WireTypesSizeID == ID));

        [HttpPost]
        public IActionResult Edit(WireTypesSize wireTypesSize)
        {
            if (ModelState.IsValid)
            {
                repository.SaveWireTypesSize(wireTypesSize);
                TempData["message"] = $"{wireTypesSize.WireTypesSizeID} has been saved..";
                return RedirectToAction("List");
            }
            else
            {
                // there is something wrong with the data values
                return View(wireTypesSize);
            }
        }

        [HttpPost]
        public IActionResult Delete(int ID)
        {
            WireTypesSize deletedWireTypeSize = repository.DeleteWireTypeSize(ID);

            if (deletedWireTypeSize != null)
            {
                TempData["message"] = $"{deletedWireTypeSize.WireTypesSizeID} was deleted";
            }
            return RedirectToAction("List");
        }

        [HttpPost]
        public IActionResult SeedWire(string buttonImportXML)
        {
            string resp = buttonImportXML;
            itemController.ImportXML(HttpContext.RequestServices, resp);
            return RedirectToAction(nameof(List));
        }

        public ViewResult Add() => View("Edit", new WireTypesSize());
    }
}