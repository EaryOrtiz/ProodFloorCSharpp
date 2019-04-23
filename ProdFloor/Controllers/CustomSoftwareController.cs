using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult DeleteCS(int fieldID, string returnUrl, CustomSoftwareViewModel viewModel)
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
            return Redirect(returnUrl);
        }

        /*
        [HttpPost]
        public IActionResult SeedXML(string buttonImportXML)
        {
            string resp = buttonImportXML;
            ItemController.ImportXML(HttpContext.RequestServices, resp);
            return RedirectToAction(nameof(List));
        }
        */
    }
}