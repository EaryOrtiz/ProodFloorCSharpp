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
        public int PageSize = 7;

        public CityController(IItemRepository repo)
        {
            repository = repo;

        }
        public IActionResult List(CityListViewModel viewModel, int page = 1)
        {
            if (viewModel.CleanFields) return RedirectToAction("List");
            IQueryable<City> cities = repository.Cities.AsQueryable();
            IQueryable<State> states = repository.States.AsQueryable();

            if (viewModel.CountryID > 0) 
            {
                states = repository.States.Where(m => m.CountryID == viewModel.CountryID);
                cities = repository.Cities.Where(m => states.Any(n => n.StateID == m.StateID));
            }
            if (viewModel.StateID > 0) cities = cities.Where(m => m.StateID == viewModel.StateID);

            if (!string.IsNullOrEmpty(viewModel.Name)) cities = cities.Where(m => m.Name.Contains(viewModel.Name));

            viewModel.Cities = cities.OrderBy(p => p.Name).Skip((page - 1) * 5).Take(5).ToList();
            viewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = page,
                ItemsPerPage = 5,
                TotalItems = cities.Count()
            };
            return View(viewModel);
        }
            

        public ViewResult Edit(int ID)
        {

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
            return View("Edit", new City());
        }

        public JsonResult GetJobState(int CountryID)
        {
            List<State> JobStatelist = new List<State>();
            JobStatelist = (from state in repository.States where state.CountryID == CountryID select state).ToList();
            return Json(new SelectList(JobStatelist, "StateID", "Name"));
        }

        [HttpPost]
        public IActionResult SeedXML(string buttonImportXML)
        {
            string resp = buttonImportXML;
            ItemController.ImportXML(HttpContext.RequestServices, resp);
            return RedirectToAction(nameof(List));
        }
    }
}
