﻿using Microsoft.AspNetCore.Mvc;
using ProdFloor.Models;
using ProdFloor.Models.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace ProdFloor.Controllers
{
    [Authorize(Roles = "Admin")]
    public class FireCodeController : Controller
    {
        private IItemRepository repository;
        private ItemController itemController;
        public int PageSize = 4;

        public FireCodeController(IItemRepository repo,
            ItemController item)
        {
            repository = repo;
            itemController = item;
        }

        public ViewResult List(string filtrado, string Sort = "default", int page = 1, int totalitemsfromlastsearch = 0)
        {   
            if (filtrado != null) Sort = filtrado;

            List<FireCode> fireCodes = repository.FireCodes
                .OrderBy(p => p.FireCodeID).ToList();

            if (Sort != "default")
            {
                fireCodes = repository.FireCodes
                 .Where(m => m.Name.Contains(Sort))
                 .OrderBy(p => p.FireCodeID).ToList();

            }

            int TotalItemsSearch = fireCodes.Count();
            if (page == 1)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
            }
            else if (TotalItemsSearch != totalitemsfromlastsearch)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
                page = 1;
            }
            FireCodesListViewModel fireCodesListView = new FireCodesListViewModel
            {
                FireCodes = fireCodes.Skip((page - 1) * PageSize)
                .Take(PageSize).ToList(),
                TotalItems = repository.FireCodes.Count(),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    sort = Sort != "default" ? Sort : "default",
                    ItemsPerPage = PageSize,
                    TotalItemsFromLastSearch = totalitemsfromlastsearch,
                    TotalItems = fireCodes.Count()
                }
            };
            return View(fireCodesListView);
        }

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

        [HttpPost]
        public IActionResult SeedXML(string buttonImportXML)
        {
            string resp = buttonImportXML;
            itemController.ImportXML(HttpContext.RequestServices, resp);
            return RedirectToAction(nameof(List));
        }

        public ViewResult Add() => View("Edit", new FireCode());


        public IActionResult SearchFireCode()
        {
            try
            {
                string term = HttpContext.Request.Query["term"].ToString();
                var names = repository.FireCodes.Where(p => p.Name.Contains(term)).Select(p => p.Name).Distinct().ToList();
                return Ok(names);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
