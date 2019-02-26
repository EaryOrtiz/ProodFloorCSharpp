using Microsoft.AspNetCore.Mvc;
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
        public int PageSize = 4;

        public CountryController(IItemRepository repo)
        {
            repository = repo;
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

        public FileStreamResult ExportToXML()
        {
            MemoryStream ms = new MemoryStream();
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.OmitXmlDeclaration = true;
            xws.Indent = true;

            List<Country> countries = new List<Country>();
            countries = repository.Countries.ToList();

                
                using (XmlWriter xw = XmlWriter.Create(ms, xws))
                {
                xw.WriteStartDocument();
                xw.WriteStartElement("Countries");

                foreach (Country country in countries)
                {
                    xw.WriteStartElement("Country");

                    xw.WriteElementString("ID", country.CountryID.ToString());   
                    xw.WriteElementString("Name", country.Name);
                    xw.WriteEndElement();
                }

                xw.WriteEndElement();
                xw.WriteEndDocument();
            }
            ms.Position = 0;

            
            return File(ms, "text/xml", "Sample.xml");
        }

        public ViewResult Add() => View("Edit", new Country());
    }
}
