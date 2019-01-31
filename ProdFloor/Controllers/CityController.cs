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
        public ViewResult List(int separator, int page = 1)
        {
            var CityCount = repository.Cities.Count();

            return View(new CityListViewModel
            {
                Cities = repository.Cities
                .OrderBy(p => p.CityID)
                .Skip((page - 1) * PageSize)
                .Take(PageSize).ToList(),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = CityCount
                },
                CurrentSeparator = separator.ToString()
            });
        }
            

        public ViewResult Edit(int ID)
        {

            List<State> StateList = new List<State>();
            //Getting Data
            StateList = (from state in repository.States select state).ToList();
            //Insert Select item in list
            StateList.Insert(0, new State { StateID = 0, Name = "Select" });
            //Assigning categorlist to viewbag
            ViewBag.StateList = StateList;

            List<FireCode> FireCodeList = new List<FireCode>();
            //Getting Data
            FireCodeList = (from firecode in repository.FireCodes select firecode).ToList();
            //Insert Select item in list
            FireCodeList.Insert(0, new FireCode { FireCodeID = 0, Name = "Select" });
            //Assigning categorlist to viewbag
            ViewBag.FireCodeList = FireCodeList;

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

            List<State> StateList = new List<State>();
            //Getting Data
            StateList = (from state in repository.States select state).ToList();
            //Insert Select item in list
            StateList.Insert(0, new State { StateID = 0, Name = "Select" });
            //Assigning categorlist to viewbag
            ViewBag.StateList = StateList;

            List<FireCode> FireCodeList = new List<FireCode>();
            //Getting Data
            FireCodeList = (from firecode in repository.FireCodes select firecode).ToList();
            //Insert Select item in list
            FireCodeList.Insert(0, new FireCode { FireCodeID = 0, Name = "Select" });
            //Assigning categorlist to viewbag
            ViewBag.FireCodeList = FireCodeList;

            return View("Edit", new City());
        }
    }
}
