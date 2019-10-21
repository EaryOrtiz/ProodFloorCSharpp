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
        public IActionResult List(StateListViewModel viewModel, int page = 1)
        {
            if (viewModel.CleanFields) return RedirectToAction("List");
            IQueryable<State> states = repository.States.AsQueryable();

            if (viewModel.CountryID > 0) states = states.Where(m => m.CountryID == viewModel.CountryID);
            if (!string.IsNullOrEmpty(viewModel.Name)) states = states.Where(m => m.Name.Contains(viewModel.Name));

            viewModel.States = states.OrderBy(p => p.Name).Skip((page - 1) * 5).Take(5).ToList();
            viewModel.TotalItems = repository.States.Count();
            viewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = page,
                ItemsPerPage = 5,
                TotalItems = states.Count()
            };
            return View(viewModel);
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