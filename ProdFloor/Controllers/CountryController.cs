﻿using Microsoft.AspNetCore.Mvc;
using ProdFloor.Models;
using ProdFloor.Models.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace ProdFloor.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CountryController : Controller
    {
        private IItemRepository repository;
        private ItemController itemController;
        public int PageSize = 4;

        public CountryController(IItemRepository repo,
            ItemController item)
        {
            repository = repo;
            itemController = item;
        }
        
        public ViewResult List(int page = 1)
            => View(new CountryListViewModel
            {
                Countries = repository.Countries
                .OrderBy(p => p.CountryID)
                .Skip((page - 1) * PageSize)
                .Take(PageSize).ToList(),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = repository.Countries.Count()
                }
            });

        public ViewResult Edit(int ID) =>
            View(repository.Countries
                .FirstOrDefault(j => j.CountryID == ID));

        [HttpPost]
        public IActionResult Edit(Country country)
        {
            if (ModelState.IsValid)
            {
                repository.SaveCountry(country);
                TempData["message"] = $"{country.Name} has been saved...{country.CountryID}";
                return RedirectToAction("List");
            }
            else
            {
                // there is something wrong with the data values
                return View(country);
            }
        }

        [HttpPost]
        public IActionResult Delete(int ID)
        {
            Country deletedCountry = repository.DeleteCountry(ID);

            if (deletedCountry != null)
            {
                TempData["message"] = $"{deletedCountry.Name} was deleted";
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

        public ViewResult Add() => View("Edit", new Country());
    }
}
