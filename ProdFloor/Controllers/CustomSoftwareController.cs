using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProdFloor.Models;
using ProdFloor.Models.ViewModels;
using ProdFloor.Models.ViewModels.Job;

namespace ProdFloor.Controllers
{
    [Authorize(Roles = "Admin,Engineer")]
    public class CustomSoftwareController : Controller
    {
        private IJobRepository Jobrepo;
        public int PageSize = 15;

        public CustomSoftwareController(IJobRepository repo)
        {
            Jobrepo = repo;
        }

        public ViewResult List(int page = 1)
            => View(new CustomSoftwareViewModel
            {
                CustomSoftwareList = Jobrepo.CustomSoftwares
                .Skip((page - 1) * PageSize)
                .Take(PageSize).ToList(),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = Jobrepo.CustomSoftwares.Count()
                }
            });

        public ViewResult NewCustomSoftware()
        {
            return View(new CustomSoftware());
        }

        [HttpPost]
        public IActionResult NewCustomSoftware(CustomSoftware newCustomSoftware)
        {
            if (ModelState.IsValid)
            {
                Jobrepo.SaveCustomSoftware(newCustomSoftware);
                CustomSoftwareViewModel newCustomSoftwareViewModel = new CustomSoftwareViewModel
                {
                    CustomSoftware = newCustomSoftware,
                    TriggeringList = new List<TriggeringCustSoft> { new TriggeringCustSoft { CustomSoftwareID = newCustomSoftware.CustomSoftwareID } },
                    CurrentTab = "Triggering"
                };
                TempData["message"] = $"Step# {newCustomSoftwareViewModel.CustomSoftware.CustomSoftwareID} has been saved....";
                return View("NextFormCS", newCustomSoftwareViewModel);
            }
            else
            {
                TempData["message"] = $"There seems to be errors in the form. Please validate....";
                TempData["alert"] = $"alert-danger";
                return View(newCustomSoftware);
            }
        }

        [HttpPost]
        public IActionResult NextFormCS(CustomSoftwareViewModel nextViewModel)
        {

            if (nextViewModel.buttonAction == "AddSF")
            {
                nextViewModel.TriggeringList.Add(new TriggeringCustSoft { CustomSoftwareID = nextViewModel.CustomSoftware.CustomSoftwareID });
                nextViewModel.CurrentTab = "Triggering";
            }
            else
            {
                if (ModelState.IsValid)
                {
                    if (nextViewModel.TriggeringList != null)
                    {
                        Jobrepo.SaveJobCustomSoftware(nextViewModel);
                        nextViewModel.CurrentTab = "Main";
                        TempData["message"] = $"everything was saved";
                        return RedirectToAction(nameof(List));
                    }
                    else
                    {
                        Jobrepo.SaveJobCustomSoftware(nextViewModel);
                        nextViewModel.TriggeringList = new List<TriggeringCustSoft> { new TriggeringCustSoft { CustomSoftwareID = nextViewModel.CustomSoftware.CustomSoftwareID } };
                        nextViewModel.CurrentTab = "Triggering";
                        TempData["message"] = $"Step was saved";
                        return View(nextViewModel);
                    }
                }
                else
                {
                    // there is something wrong with the data values
                    TempData["message"] = $"There seems to be errors in the form. Please validate.";
                    TempData["alert"] = $"alert-danger";
                    return View(nextViewModel);
                }

            }
            return View(nextViewModel);
        }

        public IActionResult Edit(int ID)
        {
            CustomSoftware custom = Jobrepo.CustomSoftwares.FirstOrDefault(j => j.CustomSoftwareID == ID);
            if (custom == null)
            {
                TempData["message"] = $"The requested Step doesn't exist.";
                return RedirectToAction("List");
            }
            else
            {
                List<TriggeringCustSoft> SfList = Jobrepo.TriggeringCustSofts.Where(j => j.CustomSoftwareID == ID).ToList();
                CustomSoftwareViewModel viewModel = new CustomSoftwareViewModel();
                viewModel.CustomSoftware = custom;
                if (SfList != null) viewModel.TriggeringList = SfList;
                else viewModel.TriggeringList = new List<TriggeringCustSoft> { new TriggeringCustSoft() };
                viewModel.CurrentTab = "Main";
                return View(viewModel);
            }

        }

        [HttpPost]
        public IActionResult Edit(CustomSoftwareViewModel multiEditViewModel)
        {
            if (ModelState.IsValid)
            {
                Jobrepo.SaveJobCustomSoftware(multiEditViewModel);
                multiEditViewModel.CurrentTab = "Main";
                TempData["message"] = $"{multiEditViewModel.CustomSoftware.CustomSoftwareID} ID has been saved...";
                return RedirectToAction(nameof(List));
            }
            else
            {
                // there is something wrong with the data values
                TempData["message"] = $"There seems to be errors in the form. Please validate.";
                TempData["alert"] = $"alert-danger";
                return View(multiEditViewModel);
            }
        }

        [HttpPost]
        public IActionResult Delete(int ID)
        {
            CustomSoftware deletedCust = Jobrepo.DeleteJobCustomSoftware(ID);
            if (deletedCust != null)
            {
                TempData["message"] = $"{deletedCust.CustomSoftwareID} was deleted";
            }
            else
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"There was an error with your request";
            }
            return RedirectToAction("List");
        }

        [HttpPost]
        public IActionResult DeleteCS(int fieldID, int customID)
        {
            TriggeringCustSoft deletedField = Jobrepo.DeleteTriggeringCustSoft(fieldID);
            if (deletedField != null)
            {
                TempData["message"] = $"{deletedField.TriggeringCustSoftID} was deleted";
            }
            else
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"There was an error with your request{fieldID}";
            }
            return RedirectToAction("Edit", new { id = customID });
        }

        [HttpPost]
        public FileStreamResult ExportToXML()
        {
            MemoryStream ms = new MemoryStream();
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.OmitXmlDeclaration = true;
            xws.Indent = true;

            List<CustomSoftware> customs = Jobrepo.CustomSoftwares.ToList();
            List<TriggeringCustSoft> triggerings = Jobrepo.TriggeringCustSofts.ToList();

            using (XmlWriter xw = XmlWriter.Create(ms, xws))
            {
                xw.WriteStartDocument();
                xw.WriteStartElement("AllCustoms");

                xw.WriteStartElement("Customs");
                foreach (CustomSoftware custom in customs)
                {
                    xw.WriteStartElement("Custom");
                    xw.WriteElementString("CustomSoftwareID", custom.CustomSoftwareID.ToString());
                    xw.WriteElementString("Description", custom.Description.ToString());
                    xw.WriteEndElement();
                }
                xw.WriteEndElement();
                xw.WriteStartElement("TriggeringCustSofts");
                string aux;
                foreach (TriggeringCustSoft triggering in triggerings)
                {
                    xw.WriteStartElement("TriggeringCustSoft");
                    xw.WriteElementString("ID", triggering.TriggeringCustSoftID.ToString());
                    xw.WriteElementString("CustomSoftwareID", triggering.CustomSoftwareID.ToString());
                    aux = !string.IsNullOrEmpty(triggering.Name) ? triggering.Name : "Nulo";
                    xw.WriteElementString("Name", aux);
                    xw.WriteElementString("IsSelected", triggering.isSelected.ToString());
                    xw.WriteElementString("itemToMatch", triggering.itemToMatch.ToString());
                    xw.WriteEndElement();

                }
                xw.WriteEndElement();
                xw.WriteEndElement();
                xw.WriteEndDocument();
            }

            ms.Position = 0;
            return File(ms, "text/xml", "CustomSoftware.xml");
        }

        public static void ImportXML(IServiceProvider services)
        {
            ApplicationDbContext context = services.GetRequiredService<ApplicationDbContext>();


            HtmlDocument doc = new HtmlDocument();
            doc.Load(@"C:\Users\eary.ortiz\Documents\GitHub\ProodFloorCSharpp\ProdFloor\wwwroot\AppData\CustomSoftware.xml");

            var ALLSteps = doc.DocumentNode.SelectSingleNode("//allcustoms");

            var ALLXMLobs = ALLSteps.SelectSingleNode("//customs");
            var XMLobs = ALLXMLobs.SelectNodes("//custom");

            var ALLtriggers = ALLSteps.SelectSingleNode("//triggeringcustsofts");
            var triggers = ALLtriggers.SelectNodes("//triggeringcustsoft");

            foreach (var XMLob in XMLobs)
            {
                var customsoftwareID = XMLob.SelectSingleNode(".//customsoftwareid").InnerText;
                var description = XMLob.SelectSingleNode(".//description").InnerText;

                context.CustomSoftwares.Add(new CustomSoftware
                {
                    CustomSoftwareID = Int32.Parse(customsoftwareID),
                    Description = description
                });
                context.Database.OpenConnection();
                try
                {
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.CustomSoftwares ON");
                    context.SaveChanges();
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.CustomSoftwares OFF");
                }
                finally
                {
                    context.Database.CloseConnection();
                }
            }
            foreach (var po in triggers)
            {
                var id = po.SelectSingleNode(".//id").InnerText;
                var customsoftwareID = po.SelectSingleNode(".//customsoftwareid").InnerText;
                var name = po.SelectSingleNode(".//name").InnerText;
                var isselected = po.SelectSingleNode(".//isselected").InnerText;
                var itemtomatch = po.SelectSingleNode(".//itemtomatch").InnerText;
                context.TriggeringCustSofts.Add(new TriggeringCustSoft
                {
                    TriggeringCustSoftID = Int32.Parse(id),
                    CustomSoftwareID = Int32.Parse(customsoftwareID),
                    Name = name == "Nulo" ? null : name,
                    isSelected = Boolean.Parse(isselected),
                    itemToMatch = itemtomatch

                });
                context.Database.OpenConnection();
                try
                {
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.TriggeringCustSofts ON");
                    context.SaveChanges();
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.TriggeringCustSofts OFF");
                }
                finally
                {
                    context.Database.CloseConnection();
                }
            }

        }

        
        [HttpPost]
        public IActionResult SeedXML(string buttonImportXML)
        {
            string resp = buttonImportXML;
            CustomSoftwareController.ImportXML(HttpContext.RequestServices);
            return RedirectToAction(nameof(List));
        }
        
    }
}