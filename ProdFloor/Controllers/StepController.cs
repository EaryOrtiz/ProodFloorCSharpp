using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProdFloor.Models;
using ProdFloor.Models.ViewModels;
using ProdFloor.Models.ViewModels.Testing;
using ProdFloor.Models.ViewModels.TestJob;

namespace ProdFloor.Controllers
{
    [Authorize(Roles = "Admin,TechAdmin")]
    public class StepController : Controller
    {
        private IItemRepository itemprepo;
        private ITestingRepository testingrepo;
        private IHostingEnvironment _env;
        private UserManager<AppUser> userManager;
        public int PageSize = 10;
        string appDataFolder => _env.WebRootPath.ToString() + @"\AppData\";


        public StepController(ITestingRepository repo, 
            IItemRepository repo2,
            IHostingEnvironment env,
            UserManager<AppUser> userMrg)
        {
            testingrepo = repo;
            itemprepo = repo2;
            _env = env;
            userManager = userMrg;
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
            return View(new StepViewModel() { Step = new Step(), Time = new TimeSpan() });
        }

        [HttpPost]
        public IActionResult NewStep(StepViewModel newStep)
        {
               
            if (newStep.Step.JobTypeID != 0 && newStep.Step.Order > 0 && !string.IsNullOrEmpty(newStep.Step.Stage))
            {
                DateTime ExpectedTime = new DateTime(1, 1, 1, newStep.Time.Hours, newStep.Time.Minutes, newStep.Time.Seconds);
                newStep.Step.ExpectedTime = ExpectedTime;
                testingrepo.SaveStep(newStep.Step);
                StepViewModel newStepViewModel = new StepViewModel
                {
                    Step = newStep.Step,
                    Time = newStep.Time,
                    TriggeringList = new List<TriggeringFeature> { new TriggeringFeature { StepID = newStep.Step.StepID } },
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
                nextViewModel.TriggeringList.Add(new TriggeringFeature { StepID = nextViewModel.TriggeringList[0].StepID });
                nextViewModel.CurrentTab = "Triggering";
            }
            else
            {
                if (ModelState.IsValid)
                {
                    if (nextViewModel.Step.StepID == 0) nextViewModel.Step.StepID = nextViewModel.TriggeringList[0].StepID;
                    if (nextViewModel.TriggeringList != null)
                    {
                        DateTime ExpectedTime = new DateTime(1, 1, 1, nextViewModel.Time.Hours, nextViewModel.Time.Minutes, nextViewModel.Time.Seconds);
                        nextViewModel.Step.ExpectedTime = ExpectedTime;
                        testingrepo.SaveTestStep(nextViewModel);
                        nextViewModel.CurrentTab = "Main";
                        TempData["message"] = $"everything was saved";
                        return RedirectToAction(nameof(List));
                    }
                    else
                    {
                        testingrepo.SaveTestStep(nextViewModel);
                        nextViewModel.TriggeringList = new List<TriggeringFeature> { new TriggeringFeature { StepID = nextViewModel.TriggeringList[0].StepID } };
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
                viewModel.Time = new TimeSpan(step.ExpectedTime.Hour, step.ExpectedTime.Minute,step.ExpectedTime.Second);
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
                DateTime ExpectedTime = new DateTime(1, 1, 1, multiEditViewModel.Time.Hours, multiEditViewModel.Time.Minutes, multiEditViewModel.Time.Seconds);
                multiEditViewModel.Step.ExpectedTime = ExpectedTime;
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
            bool admin = GetCurrentUserRole("Admin").Result;

            if (!admin)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"You don't have permissions, contact to your admin";

                return RedirectToAction("List");
            }

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
            try
            {
                TimeSpan elapsed = viewModel.StepsForJob.Stop - viewModel.StepsForJob.Start;
                viewModel.StepsForJob.Elapsed += elapsed;
                testingrepo.SaveStepsForJob(viewModel.StepsForJob);
                TempData["message"] = $"{viewModel.StepsForJob.StepsForJobID} has been saved...";
                return RedirectToAction("SearchTestJob", "TestJob");
            }
            catch(Exception e)
            {
                StepsForJob stepsForJob = testingrepo.StepsForJobs.FirstOrDefault(m => m.StepsForJobID == viewModel.StepsForJob.StepsForJobID);
                TestJobViewModel testJobView = new TestJobViewModel
                {
                    StepsForJob = stepsForJob,
                    TestJob = testingrepo.TestJobs.FirstOrDefault(m => m.TestJobID == stepsForJob.TestJobID),
                    Step = testingrepo.Steps.FirstOrDefault(m => m.StepID == stepsForJob.StepID)
                };
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"There was an error with your times";
                return View(testJobView);
            }
            
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

        public void ImportXML(IServiceProvider services)
        {
            ApplicationDbContext context = services.GetRequiredService<ApplicationDbContext>();


            XmlDocument doc = new XmlDocument();
            doc.Load(appDataFolder + "Steps.xml");

            var ALLSteps = doc.DocumentElement.SelectSingleNode("//AllSteps");

            var ALLXMLobs = ALLSteps.SelectSingleNode("//Steps");
            var XMLobs = ALLXMLobs.SelectNodes("//Step");

            var ALLtriggers = ALLSteps.SelectSingleNode("//TriggerFeatures");
            var triggers = ALLtriggers.SelectNodes("//TriggerFeature");

            foreach (XmlElement XMLob in XMLobs)
            {
                var stepid = XMLob.SelectSingleNode(".//StepID").InnerText;
                var jobtypeid = XMLob.SelectSingleNode(".//JobTypeID").InnerText;
                var stage = XMLob.SelectSingleNode(".//Stage").InnerText;
                var expectedtime = XMLob.SelectSingleNode(".//ExpectedTime").InnerText;
                var description = XMLob.SelectSingleNode(".//Description").InnerText;
                var order = XMLob.SelectSingleNode(".//Order").InnerText;

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

            foreach (XmlElement po in triggers)
            {
                var id = po.SelectSingleNode(".//ID").InnerText;
                var sttepid = po.SelectSingleNode(".//StepID").InnerText;
                var name = po.SelectSingleNode(".//Name").InnerText;
                var isselected = po.SelectSingleNode(".//IsSelected").InnerText;
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
            ImportXML(HttpContext.RequestServices);
            return RedirectToAction(nameof(List));
        }

        private async Task<bool> GetCurrentUserRole(string role)
        {
            AppUser user = await userManager.GetUserAsync(HttpContext.User);

            bool isInRole = await userManager.IsInRoleAsync(user, role);

            return isInRole;
        }
    }
}