using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProdFloor.Models;
using ProdFloor.Models.ViewModels;
using ProdFloor.Models.ViewModels.Testing;

namespace ProdFloor.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StepController : Controller
    {
        private IItemRepository itemprepo;
        private ITestingRepository testingrepo;
        public int PageSize = 15;

        public StepController(ITestingRepository repo, IItemRepository repo2)
        {
            testingrepo = repo;
            itemprepo = repo2;
        }

        public ViewResult List(int page = 1)
            => View(new StepViewModel
            {
                StepList = testingrepo.Steps
                .OrderBy(p => p.JobTypeID)
                .Skip((page - 1) * PageSize)
                .Take(PageSize).ToList(),
                JobTypesList = itemprepo.JobTypes.ToList(),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = testingrepo.Steps.Count()
                }
            });

        public ViewResult NewStep()
        {
            return View(new Step());
        }

        [HttpPost]
        public IActionResult NewStep(Step newStep)
        {
            if (ModelState.IsValid)
            {
                testingrepo.SaveStep(newStep);
                StepViewModel newStepViewModel = new StepViewModel
                {
                    Step = newStep,
                    TriggeringList = new List<TriggeringFeature> { new TriggeringFeature { StepID = newStep.StepID } },
                    CurrentTab = "Triggering"
                };
                TempData["message"] = $"Step# {newStepViewModel.Step.StepID} has been saved....";
                return View("NextForm", newStepViewModel);
            }
            else
            {
                TempData["message"] = $"There seems to be errors in the form. Please validate....";
                TempData["alert"] = $"alert-danger";
                return View(newStep);
            }
        }

        [HttpPost]
        public IActionResult NextForm(StepViewModel nextViewModel)
        {

            if (nextViewModel.buttonAction == "AddSF")
            {
                nextViewModel.TriggeringList.Add(new TriggeringFeature { StepID = nextViewModel.Step.StepID });
                nextViewModel.CurrentTab = "Triggering";
            }
            else
            {
                if (ModelState.IsValid)
                {
                    if (nextViewModel.TriggeringList != null)
                    {
                        testingrepo.SaveTestStep(nextViewModel);
                        nextViewModel.CurrentTab = "Main";
                        TempData["message"] = $"everything was saved";
                        return RedirectToAction(nameof(List));
                    }
                    else
                    {
                        testingrepo.SaveTestStep(nextViewModel);
                        nextViewModel.TriggeringList = new List<TriggeringFeature> { new TriggeringFeature { StepID = nextViewModel.Step.StepID } };
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
            Step step = testingrepo.Steps.FirstOrDefault(j => j.StepID == ID);
            if (step == null)
            {
                TempData["message"] = $"The requested Step doesn't exist.";
                return RedirectToAction("List");
            }
            else
            {
                List<TriggeringFeature> SfList = testingrepo.TriggeringFeatures.Where(j => j.StepID == ID).ToList();
                StepViewModel viewModel = new StepViewModel();
                viewModel.Step = step;
                if (SfList != null) viewModel.TriggeringList = SfList;
                else viewModel.TriggeringList = new List<TriggeringFeature> { new TriggeringFeature() };
                viewModel.CurrentTab = "Main";
                return View(viewModel);
            }

        }

        [HttpPost]
        public IActionResult Edit(StepViewModel multiEditViewModel)
        {
            if (ModelState.IsValid)
            {
                testingrepo.SaveTestStep(multiEditViewModel);
                multiEditViewModel.CurrentTab = "Main";
                TempData["message"] = $"{multiEditViewModel.Step.StepID} ID has been saved...";
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
            Step deletedStep = testingrepo.DeleteTestStep(ID);
            if (deletedStep != null)
            {
                TempData["message"] = $"{deletedStep.StepID} was deleted";
            }
            else
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"There was an error with your request";
            }
            return RedirectToAction("List");
        }

        [HttpPost]
        public IActionResult DeleteSF(int fieldID, string returnUrl, StepViewModel viewModel)
        {
            TriggeringFeature deletedField = testingrepo.DeleteTriggeringFeature(fieldID);
            if (deletedField != null)
            {
                TempData["message"] = $"{deletedField.TriggeringFeatureID} was deleted";
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