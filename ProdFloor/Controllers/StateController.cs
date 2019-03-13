using Microsoft.AspNetCore.Mvc;
using ProdFloor.Models;
using ProdFloor.Models.ViewModels;
using System.Linq;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProdFloor.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StateController : Controller
    {
        private IItemRepository repository;
        public int PageSize = 6;

        public StateController(IItemRepository repo)
        {
            repository = repo;

        }
        public ViewResult List(int country, int page = 1)
        {

            var StatesCount = repository.States.Count();

            return View(new StateListViewModel
            {
                States = repository.States
               .OrderBy(p => p.Name)
               .Skip((page - 1) * PageSize)
               .Take(PageSize).ToList(),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = StatesCount
                },
                CurrentCountry = country.ToString()
            });
        }
            

        public ViewResult Edit(int ID)
        {

            return View(repository.States
                 .FirstOrDefault(j => j.StateID == ID));
        }

        [HttpPost]
        public IActionResult Edit(State state)
        {
            if (ModelState.IsValid)
            {
                repository.SaveState(state);
                TempData["message"] = $"{state.Name},, has been saved...";
                return RedirectToAction("List");
            }
            else
            {
                // there is something wrong with the data values
                return View(state);
            }
        }

        [HttpPost]
        public IActionResult Delete(int ID)
        {
            State deletedState = repository.DeleteState(ID);

            if (deletedState != null)
            {
                TempData["message"] = $"{deletedState.Name} was deleted";
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

        public ViewResult Add(){
            return View("Edit", new State());
        } 
    }
}