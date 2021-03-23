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
        private ItemController itemController;
        public int PageSize = 6;

        public StateController(IItemRepository repo,
            ItemController item)
        {
            repository = repo;
            itemController = item;
        }

        public IActionResult List(StateListViewModel viewModel, int page = 1, int totalitemsfromlastsearch = 0)
        {
            if (viewModel.CleanFields) return RedirectToAction("List");
            IQueryable<State> states = repository.States.AsQueryable();

            if (viewModel.CountryID > 0) states = states.Where(m => m.CountryID == viewModel.CountryID);
            if (!string.IsNullOrEmpty(viewModel.Name)) states = states.Where(m => m.Name.Contains(viewModel.Name));


            viewModel.TotalItems = repository.States.Count();

            int TotalItemsSearch = states.Count();
            if (page == 1)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
            }
            else if (TotalItemsSearch != totalitemsfromlastsearch)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
                page = 1;
            }
            viewModel.States = states.OrderBy(p => p.Name).Skip((page - 1) * 5).Take(5).ToList();
            viewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = page,
                ItemsPerPage = 5,
                TotalItemsFromLastSearch = totalitemsfromlastsearch,
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
            itemController.ImportXML(HttpContext.RequestServices, resp);
            return RedirectToAction(nameof(List));
        }

        public ViewResult Add()
        {
            return View("Edit", new State());
        }
    }
}