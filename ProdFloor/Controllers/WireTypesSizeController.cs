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
        public int PageSize = 15;

        public WireTypesSizeController(IItemRepository repo)
        {
            repository = repo;
        }

        public ViewResult List(int page = 1)
            => View(new WireTypesSizeListViewModel
            {
                WireTypes = repository.WireTypesSizes
                .OrderBy(p => p.WireTypesSizeID)
                .Skip((page - 1) * PageSize)
                .Take(PageSize).ToList(),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = repository.WireTypesSizes.Count()
                }
            });

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



        public ViewResult Add() => View("Edit", new WireTypesSize());
    }
}