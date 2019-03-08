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
        public int PageSize = 15;

        public SlowdownController(IItemRepository repo)
        {
            repository = repo;
        }

        public ViewResult List(int page = 1)
            => View(new SlowdownListViewModel
            {
                Slowdowns = repository.Slowdowns
                .OrderBy(p => p.SlowdownID)
                .Skip((page - 1) * PageSize)
                .Take(PageSize).ToList(),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = repository.Slowdowns.Count()
                }
            });

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



        public ViewResult Add() => View("Edit", new Slowdown());
    }
}
