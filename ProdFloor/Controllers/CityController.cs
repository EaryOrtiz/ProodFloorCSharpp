using Microsoft.AspNetCore.Mvc;
using ProdFloor.Models;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProdFloor.Models.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace ProdFloor.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CityController : Controller
    {
        private IItemRepository repository;
        public int PageSize = 4;

        public CityController(IItemRepository repo)
        {
            repository = repo;

        }
        //Se debe arreglar con jions!!!!!!!!
        public ViewResult List(int separator, int page = 1)
            => View(new CityListViewModel
            {
                Cities = repository.Cities
                .Where(j => separator == null || j.StateID == separator)
                .OrderBy(p => p.CityID)
                .Skip((page - 1) * PageSize)
                .Take(PageSize).ToList(),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = separator == null ?
                    repository.Cities.Count() :
                    repository.Cities.Where(e =>
                    e.StateID == separator).Count()
                },
                CurrentSeparator = separator.ToString()
            });

        public ViewResult Edit(int ID)
        {
            ViewData["Countries"] = repository.Countries;
            ViewData["States"] = repository.States;
            ViewData["FireCodes"] = repository.FireCodes;
            return View(repository.Cities
                .FirstOrDefault(j => j.CityID == ID));
        }


        [HttpPost]
        public IActionResult Edit(City city)
        {
            if (ModelState.IsValid)
            {
                repository.SaveCity(city);
                TempData["message"] = $"{city.Name},, has been saved...";
                return RedirectToAction("List");
            } 
            else
            {
                // there is something wrong with the data values
                return View(city);
            }
        }
        
        [HttpPost]
        public IActionResult Delete(int ID)
        {
            City deletedCity = repository.DeleteCity(ID);
        
            if (deletedCity != null)
            {
                TempData["message"] = $"{deletedCity.Name} was deleted";
            }
            return RedirectToAction("List");
        }
        
        public ViewResult Add()
        {
            ViewData["Countries"] = repository.Countries;
            ViewData["States"] = repository.States;
            ViewData["FireCodes"] = repository.FireCodes;

            return View("Edit", new City());
        }
    }
}
