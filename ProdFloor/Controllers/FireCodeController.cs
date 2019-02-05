using Microsoft.AspNetCore.Mvc;
using ProdFloor.Models;
using ProdFloor.Models.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace ProdFloor.Controllers
{
    [Authorize(Roles = "Admin")]
    public class FireCodeController : Controller
    {
        private IItemRepository repository;
        public int PageSize = 4;

        public FireCodeController(IItemRepository repo)
        {
            repository = repo;
        }
        
        public ViewResult List(int page = 1)
            => View(new FireCodesListViewModel
            {
                FireCodes = repository.FireCodes
                .OrderBy(p => p.FireCodeID)
                .Skip((page - 1) * PageSize)
                .Take(PageSize).ToList(),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = repository.FireCodes.Count()
                }
            });

        public ViewResult Edit(int ID) =>
            View(repository.FireCodes
                .FirstOrDefault(j => j.FireCodeID == ID));

        [HttpPost]
        public IActionResult Edit(FireCode fireCode)
        {
            if (ModelState.IsValid)
            {
                repository.SaveFireCode(fireCode);
                TempData["message"] = $"{fireCode.Name} has been saved...{fireCode.FireCodeID}";
                return RedirectToAction("List");
            }
            else
            {
                // there is something wrong with the data values
                return View(fireCode);
            }
        }

        [HttpPost]
        public IActionResult Delete(int ID)
        {
            FireCode deletedFireCode = repository.DeleteFireCode(ID);

            if (deletedFireCode != null)
            {
                TempData["message"] = $"{deletedFireCode.Name} was deleted";
            }
            return RedirectToAction("List");
        }

        public ViewResult Add() => View("Edit", new FireCode());
    }
}
