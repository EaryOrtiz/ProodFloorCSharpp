﻿using Microsoft.AspNetCore.Mvc;
using ProdFloor.Models;
using ProdFloor.Models.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace ProdFloor.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DoorOperatorController : Controller
    {
        private IItemRepository repository;
        private ItemController itemController;
        public int PageSize = 4;

        public DoorOperatorController(IItemRepository repo,
            ItemController item)
        {
            repository = repo;
            itemController = item;
        }

        public IActionResult List(DoorOperatorsListViewModel viewModel, int page = 1, int totalitemsfromlastsearch = 0) 
        {
            if (viewModel.CleanFields) return RedirectToAction("List");
            IQueryable<DoorOperator> doors = repository.DoorOperators.AsQueryable();

            if (!string.IsNullOrEmpty(viewModel.Brand)) doors = doors.Where(m => m.Brand.Contains(viewModel.Brand));
            if (!string.IsNullOrEmpty(viewModel.Style)) doors = doors.Where(m => m.Style.Contains(viewModel.Style));
            if (!string.IsNullOrEmpty(viewModel.Name)) doors = doors.Where(m => m.Name.Contains(viewModel.Name));

            
            viewModel.TotalItems = repository.DoorOperators.Count();


            int TotalItemsSearch = doors.Count();
            if (page == 1)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
            }
            else if (TotalItemsSearch != totalitemsfromlastsearch)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
                page = 1;
            }
            viewModel.DoorOperators = doors.OrderBy(p => p.Name).Skip((page - 1) * 5).Take(5).ToList();
            viewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = page,
                ItemsPerPage = 5,
                TotalItemsFromLastSearch = totalitemsfromlastsearch,
                TotalItems = doors.Count()
            };
            return View(viewModel);
        }

        public ViewResult Edit(int ID) =>
            View(repository.DoorOperators
                .FirstOrDefault(j => j.DoorOperatorID == ID));

        [HttpPost]
        public IActionResult Edit(DoorOperator doorOperator)
        {
            if (ModelState.IsValid)
            {
                repository.SaveDoorOperator(doorOperator);
                TempData["message"] = $"{doorOperator.Name} has been saved...{doorOperator.DoorOperatorID}";
                return RedirectToAction("List");
            }
            else
            {
                // there is something wrong with the data values
                return View(doorOperator);
            }
        }

        [HttpPost]
        public IActionResult Delete(int ID)
        {
            DoorOperator deletedDoorOperator = repository.DeleteDoorOperator(ID);

            if (deletedDoorOperator != null)
            {
                TempData["message"] = $"{deletedDoorOperator.Name} was deleted";
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

        public ViewResult Add() => View("Edit", new DoorOperator());
    }
}
