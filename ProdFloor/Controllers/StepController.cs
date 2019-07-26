﻿using System;
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
using ProdFloor.Models.ViewModels.Testing;
using ProdFloor.Models.ViewModels.TestJob;

namespace ProdFloor.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StepController : Controller
    {
        private IItemRepository itemprepo;
        private ITestingRepository testingrepo;
        public int PageSize = 10;

        public StepController(ITestingRepository repo, IItemRepository repo2)
        {
            testingrepo = repo;
            itemprepo = repo2;
        }

        public ViewResult List(string filtrado,string JobTypeName = "Traction", int ElmHydroPage = 1, int ElmTractionPage = 1, int M2000Page = 1, int M4000Page = 1)
        {
            if (filtrado != null ) JobTypeName = filtrado;
            List<Step> StepList = testingrepo.Steps.ToList();

            switch (JobTypeName)
            {
                case "Hydro":
                    StepList = testingrepo.Steps.Where(s => s.JobTypeID == 1).OrderBy(p => p.Order)
                        .Skip((ElmHydroPage - 1) * PageSize).Take(PageSize).ToList(); break;
                case "Traction":
                        StepList = testingrepo.Steps.Where(s => s.JobTypeID == 5).OrderBy(p => p.Order)
                        .Skip((ElmTractionPage - 1) * PageSize).Take(PageSize).ToList(); break;
                case "M2000":
                    StepList = testingrepo.Steps.Where(s => s.JobTypeID == 2).OrderBy(p => p.Order)
                    .Skip((M2000Page - 1) * PageSize).Take(PageSize).ToList();break;
                case "M4000":
                    StepList = testingrepo.Steps.Where(s => s.JobTypeID == 4).OrderBy(p => p.Order)
                        .Skip((M4000Page - 1) * PageSize).Take(PageSize).ToList();break;
            }

            StepViewModel stepView = new StepViewModel
            {
                StepList = StepList,
                ElmHydroPagingInfo = new PagingInfo
                {
                    CurrentPage = ElmHydroPage,
                    JobTypeName = JobTypeName,
                    ItemsPerPage = PageSize,
                    TotalItems = testingrepo.Steps
                        .Where(s => s.JobTypeID == 1).Count()
                },
                ElmTractionPagingInfo = new PagingInfo
                {
                    CurrentPage = ElmTractionPage,
                    ItemsPerPage = PageSize,
                    JobTypeName = JobTypeName,
                    TotalItems = testingrepo.Steps
                        .Where(s => s.JobTypeID == 5).Count()
                },
                M2000PagingInfo = new PagingInfo
                {
                    CurrentPage = M2000Page,
                    ItemsPerPage = PageSize,
                    JobTypeName = JobTypeName,
                    TotalItems = testingrepo.Steps
                        .Where(s => s.JobTypeID == 2).Count()
                },
                M4000PagingInfo = new PagingInfo
                {
                    CurrentPage = M4000Page,
                    ItemsPerPage = PageSize,
                    JobTypeName = JobTypeName,
                    TotalItems = testingrepo.Steps
                        .Where(s => s.JobTypeID == 4).Count()
                },
                TriggeringList = testingrepo.TriggeringFeatures.ToList(),
                JobTypeSelected = JobTypeName,
                JobTypesList = itemprepo.JobTypes.ToList(),
            };
            return View(stepView);
        }

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

        public ViewResult EditStepsForJob(int ID)
        {
            StepsForJob stepsForJob = testingrepo.StepsForJobs.FirstOrDefault(m => m.StepsForJobID == ID);
            if (stepsForJob == null)
            {
                return View(NotFound());
            }
            TestJobViewModel viewModel = new TestJobViewModel
            {
                StepsForJob = stepsForJob,
                TestJob = testingrepo.TestJobs.FirstOrDefault(m => m.TestJobID == stepsForJob.TestJobID),
                Step = testingrepo.Steps.FirstOrDefault(m => m.StepID == stepsForJob.StepID)
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult EditStepsForJob(TestJobViewModel viewModel)
        {
            TimeSpan elapsed = viewModel.StepsForJob.Stop - viewModel.StepsForJob.Start;
            viewModel.StepsForJob.Elapsed += elapsed;
            testingrepo.SaveStepsForJob(viewModel.StepsForJob);
                TempData["message"] = $"{viewModel.StepsForJob.StepsForJobID} has been saved...";
                return RedirectToAction("List");
        }

        [HttpPost]
        public FileStreamResult ExportToXML()
        {
            MemoryStream ms = new MemoryStream();
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.OmitXmlDeclaration = true;
            xws.Indent = true;

            List<Step> steps = testingrepo.Steps.ToList();
            List<TriggeringFeature> triggerings = testingrepo.TriggeringFeatures.ToList();

            using (XmlWriter xw = XmlWriter.Create(ms, xws))
            {
                xw.WriteStartDocument();
                xw.WriteStartElement("AllSteps");

                xw.WriteStartElement("Steps");
                foreach (Step step in steps)
                {
                    xw.WriteStartElement("Step");
                    xw.WriteElementString("StepID", step.StepID.ToString());
                    xw.WriteElementString("JobTypeID", step.JobTypeID.ToString());
                    xw.WriteElementString("Stage", step.Stage.ToString());
                    xw.WriteElementString("ExpectedTime", step.ExpectedTime.ToString());
                    xw.WriteElementString("Description", step.Description);
                    xw.WriteElementString("Order", step.Order.ToString());
                    xw.WriteEndElement();
                }
                xw.WriteEndElement();
                xw.WriteStartElement("TriggerFeatures");
                string aux;
                foreach (TriggeringFeature triggering in triggerings)
                {
                    xw.WriteStartElement("TriggerFeature");
                    xw.WriteElementString("ID", triggering.TriggeringFeatureID.ToString());
                    xw.WriteElementString("StepID", triggering.StepID.ToString());
                    aux = !string.IsNullOrEmpty(triggering.Name) ? triggering.Name : "Nulo";
                    xw.WriteElementString("Name", aux);
                    xw.WriteElementString("IsSelected", triggering.IsSelected.ToString());
                    xw.WriteEndElement();

                }
                xw.WriteEndElement();
                xw.WriteEndElement();
                xw.WriteEndDocument();
            }

            ms.Position = 0;
            return File(ms, "text/xml", "Steps.xml");
        }

        public static void ImportXML(IServiceProvider services)
        {
            ApplicationDbContext context = services.GetRequiredService<ApplicationDbContext>();


            HtmlDocument doc = new HtmlDocument();
            doc.Load(@"C:\Users\eary.ortiz\Documents\GitHub\ProodFloorCSharpp\ProdFloor\wwwroot\AppData\Steps.xml");

            var ALLSteps = doc.DocumentNode.SelectSingleNode("//allsteps");

            var ALLXMLobs = ALLSteps.SelectSingleNode("//steps");
            var XMLobs = ALLXMLobs.SelectNodes("//step");

            var ALLtriggers = ALLSteps.SelectSingleNode("//triggerfeatures");
            var triggers = ALLtriggers.SelectNodes("//triggerfeature");

            foreach (var XMLob in XMLobs)
            {
                var stepid = XMLob.SelectSingleNode(".//stepid").InnerText;
                var jobtypeid = XMLob.SelectSingleNode(".//jobtypeid").InnerText;
                var stage = XMLob.SelectSingleNode(".//stage").InnerText;
                var expectedtime = XMLob.SelectSingleNode(".//expectedtime").InnerText;
                var description = XMLob.SelectSingleNode(".//description").InnerText;
                var order = XMLob.SelectSingleNode(".//order").InnerText;

                context.Steps.Add(new Step
                {
                    StepID = Int32.Parse(stepid),
                    JobTypeID = Int32.Parse(jobtypeid),
                    Stage = stage,
                    ExpectedTime = DateTime.Parse(expectedtime),
                    Description = description,
                    Order = Int32.Parse(order)
                });
                context.Database.OpenConnection();
                try
                {
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Steps ON");
                    context.SaveChanges();
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Steps OFF");
                }
                finally
                {
                    context.Database.CloseConnection();
                }
            }
            foreach (var po in triggers)
            {
                var id = po.SelectSingleNode(".//id").InnerText;
                var sttepid = po.SelectSingleNode(".//stepid").InnerText;
                var name = po.SelectSingleNode(".//name").InnerText;
                var isselected = po.SelectSingleNode(".//isselected").InnerText;
                context.TriggeringFeatures.Add(new TriggeringFeature
                {
                    TriggeringFeatureID = Int32.Parse(id),
                    StepID = Int32.Parse(sttepid),
                    Name = name == "Nulo" ? null : name,
                    IsSelected = Boolean.Parse(isselected)

                });
                context.Database.OpenConnection();
                try
                {
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.TriggeringFeatures ON");
                    context.SaveChanges();
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.TriggeringFeatures OFF");
                }
                finally
                {
                    context.Database.CloseConnection();
                }
            }

        }

        [HttpPost]
        public IActionResult SeedXML()
        {
            StepController.ImportXML(HttpContext.RequestServices);
            return RedirectToAction(nameof(List));
        }
    }
}