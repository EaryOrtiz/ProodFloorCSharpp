using Microsoft.AspNetCore.Mvc;
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
        public int PageSize = 4;

        public DoorOperatorController(IItemRepository repo)
        {
            repository = repo;
        }
        
        public ViewResult List(string brand, int page = 1)
            => View(new DoorOperatorsListViewModel
            {
                DoorOperators = repository.DoorOperators
                .Where(j => brand == null || j.Brand == brand)
                .OrderBy(p => p.DoorOperatorID)
                .Skip((page - 1) * PageSize)
                .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems =brand == null ?
                    repository.DoorOperators.Count() :
                    repository.DoorOperators.Where(e =>
                    e.Brand == brand).Count()
                },
                CurrentBrand = brand
            });

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

        public ViewResult Add() => View("Edit", new DoorOperator());
    }
}
