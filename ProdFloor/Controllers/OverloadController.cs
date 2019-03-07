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
        public int PageSize = 4;

        public OverloadController(IItemRepository repo)
        {
            repository = repo;
        }

        public ViewResult List(int page = 1)
            => View(new OverloadListViewModel
            {
                Overloads = repository.Ovearloads
                .OrderBy(p => p.OverloadID)
                .Skip((page - 1) * PageSize)
                .Take(PageSize).ToList(),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = repository.Ovearloads.Count()
                }
            });

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

        public ViewResult Add() => View("Edit", new Overload());
    }
}