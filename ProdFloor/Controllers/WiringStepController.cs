using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProdFloor.Models;
using ProdFloor.Models.ViewModels;
using ProdFloor.Models.ViewModels.Wiring;

namespace ProdFloor.Controllers
{
    [Authorize(Roles = "Admin,TechAdmin,ProductionAdmin")]
    public class WiringStepController : Controller
    {
        private IItemRepository itemprepo;
        private ITestingRepository testingrepo;
        private IWiringRepository wiringRepo;
        private IHostingEnvironment _env;
        private UserManager<AppUser> userManager;
        public int PageSize = 10;
        string appDataFolder => _env.WebRootPath.ToString() + @"\AppData\";


        public WiringStepController(ITestingRepository repo, 
            IItemRepository repo2,
            IHostingEnvironment env,
            IWiringRepository repo3,
            UserManager<AppUser> userMrg)
        {
            testingrepo = repo;
            wiringRepo = repo3;
            itemprepo = repo2;
            _env = env;
            userManager = userMrg;
        }

        public ViewResult List(string filtrado,string JobTypeName = "Traction", int ElmHydroPage = 1, int ElmTractionPage = 1, int M2000Page = 1, int M4000Page = 1)
        {
            if (filtrado != null ) JobTypeName = filtrado;
            List<WiringStep> StepList = wiringRepo.WiringSteps.ToList();

            switch (JobTypeName)
            {
                case "Hydro":
                    StepList = wiringRepo.WiringSteps.Where(s => s.JobTypeID == 1).OrderBy(p => p.Order)
                        .Skip((ElmHydroPage - 1) * PageSize).Take(PageSize).ToList(); break;
                case "Traction":
                        StepList = wiringRepo.WiringSteps.Where(s => s.JobTypeID == 5).OrderBy(p => p.Order)
                        .Skip((ElmTractionPage - 1) * PageSize).Take(PageSize).ToList(); break;
                case "M2000":
                    StepList = wiringRepo.WiringSteps.Where(s => s.JobTypeID == 2).OrderBy(p => p.Order)
                    .Skip((M2000Page - 1) * PageSize).Take(PageSize).ToList();break;
                case "M4000":
                    StepList = wiringRepo.WiringSteps.Where(s => s.JobTypeID == 4).OrderBy(p => p.Order)
                        .Skip((M4000Page - 1) * PageSize).Take(PageSize).ToList();break;
            }

            WiringStepViewModel stepView = new WiringStepViewModel
            {
                WiringStepList = StepList,
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
                WiringTriggeringList = wiringRepo.WiringTriggeringFeatures.Where(m => m.WiringOptionID != 0).ToList(),
                JobTypeSelected = JobTypeName,
                JobTypesList = itemprepo.JobTypes.ToList(),
            };
            return View(stepView);
        }

        public ViewResult NewStep()
        {
            return View(new WiringStepViewModel() { WiringStep = new WiringStep(), Time = new TimeSpan() });
        }

        [HttpPost]
        public IActionResult NewStep(WiringStepViewModel newStep)
        {
               
            if (newStep.WiringStep.JobTypeID != 0 && newStep.WiringStep.Order > 0 && !string.IsNullOrEmpty(newStep.WiringStep.Stage))
            {
                newStep.WiringStep.ExpectedTime = new DateTime(1, 1, 1, newStep.Time.Hours, newStep.Time.Minutes, newStep.Time.Seconds);
                wiringRepo.SaveWiringStep(newStep.WiringStep);
                WiringStepViewModel newWiringStepViewModel = new WiringStepViewModel
                {
                    WiringStep = newStep.WiringStep,
                    Time = newStep.Time,
                    WiringTriggeringList = new List<WiringTriggeringFeature> { new WiringTriggeringFeature { WiringStepID = newStep.WiringStep.WiringStepID } },
                    CurrentTab = "Triggering"
                };
                TempData["message"] = $"Step# {newWiringStepViewModel.WiringStep.WiringStepID} has been saved....";
                return View("NextForm", newWiringStepViewModel);
            }
            else
            {
                TempData["message"] = $"There seems to be errors in the form. Please validate....";
                TempData["alert"] = $"alert-danger";
                return View(newStep);
            }
        }

        [HttpPost]
        public IActionResult NextForm(WiringStepViewModel nextViewModel)
        {

            if (nextViewModel.buttonAction == "AddSF")
            {
                nextViewModel.WiringTriggeringList.Add(new WiringTriggeringFeature { WiringStepID = nextViewModel.WiringTriggeringList[0].WiringStepID });
                nextViewModel.CurrentTab = "Triggering";
            }
            else
            {
                if (ModelState.IsValid)
                {
                    if (nextViewModel.WiringStep.WiringStepID == 0) nextViewModel.WiringStep.WiringStepID = nextViewModel.WiringTriggeringList[0].WiringStepID;
                    if (nextViewModel.WiringTriggeringList != null)
                    {

                        nextViewModel.WiringStep.ExpectedTime = new DateTime(1, 1, 1, nextViewModel.Time.Hours, nextViewModel.Time.Minutes, nextViewModel.Time.Seconds);
                        wiringRepo.SaveWiringStepWithTriggers(nextViewModel);
                        nextViewModel.CurrentTab = "Main";

                        TempData["message"] = $"everything was saved";
                        return RedirectToAction(nameof(List));
                    }
                    else
                    {
                        wiringRepo.SaveWiringStepWithTriggers(nextViewModel);
                        nextViewModel.WiringTriggeringList = new List<WiringTriggeringFeature> { new WiringTriggeringFeature { WiringStepID = nextViewModel.WiringTriggeringList[0].WiringStepID } };
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
            WiringStep step = wiringRepo.WiringSteps.FirstOrDefault(j => j.WiringStepID == ID);
            if (step == null)
            {
                TempData["message"] = $"The requested Step doesn't exist.";
                return RedirectToAction("List");
            }
            else
            {
                List<WiringTriggeringFeature> SfList = wiringRepo.WiringTriggeringFeatures.Where(j => j.WiringStepID == ID).ToList();
                WiringStepViewModel viewModel = new WiringStepViewModel();
                viewModel.WiringStep = step;
                viewModel.Time = new TimeSpan(step.ExpectedTime.Hour, step.ExpectedTime.Minute,step.ExpectedTime.Second);
                viewModel.CurrentTab = "Main";
                if (SfList.Count != 0) viewModel.WiringTriggeringList = SfList;
                else {
                    viewModel.WiringTriggeringList = new List<WiringTriggeringFeature> { new WiringTriggeringFeature { WiringStepID = step.WiringStepID } };
                    viewModel.CurrentTab = "Triggering";
                }
                
                
                return View(viewModel);
            }

        }

        [HttpPost]
        public IActionResult Edit(WiringStepViewModel multiEditViewModel)
        {
            if (ModelState.IsValid)
            {
                multiEditViewModel.WiringStep.ExpectedTime = new DateTime(1, 1, 1, multiEditViewModel.Time.Hours, multiEditViewModel.Time.Minutes, multiEditViewModel.Time.Seconds);
                wiringRepo.SaveWiringStepWithTriggers(multiEditViewModel);
                multiEditViewModel.CurrentTab = "Main";

                TempData["message"] = $"{multiEditViewModel.WiringStep.WiringStepID} ID has been saved...";
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

            WiringStep deletedWiringStep = wiringRepo.DeleteWiringStep(ID);
            if (deletedWiringStep != null)
            {
                TempData["message"] = $"{deletedWiringStep.WiringStepID} was deleted";
            }
            else
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"There was an error with your request";
            }
            return RedirectToAction("List");
        }

        [HttpPost]
        public IActionResult DeleteSF(int fieldID, string returnUrl, WiringStepViewModel viewModel)
        {
            WiringTriggeringFeature deletedField = wiringRepo.DeleteWiringTriggeringFeature(fieldID);
            if (deletedField != null)
            {
                TempData["message"] = $"{deletedField.WiringTriggeringFeatureID} was deleted";
            }
            else
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"There was an error with your request{fieldID}";
            }
            return Redirect(returnUrl);
        }


        [HttpPost]
        public FileStreamResult ExportToXML()
        {
            MemoryStream ms = new MemoryStream();
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.OmitXmlDeclaration = true;
            xws.Indent = true;

            List<WiringStep> steps = wiringRepo.WiringSteps.ToList();
            List<WiringTriggeringFeature> triggerings = wiringRepo.WiringTriggeringFeatures.ToList();

            using (XmlWriter xw = XmlWriter.Create(ms, xws))
            {
                xw.WriteStartDocument();
                xw.WriteStartElement("AllWiringSteps");

                xw.WriteStartElement("WiringSteps");
                foreach (WiringStep step in steps)
                {
                    xw.WriteStartElement("WiringStep");
                    xw.WriteElementString("WiringStepID", step.WiringStepID.ToString());
                    xw.WriteElementString("JobTypeID", step.JobTypeID.ToString());
                    xw.WriteElementString("Stage", step.Stage.ToString());
                    xw.WriteElementString("ExpectedTime", step.ExpectedTime.ToString());
                    xw.WriteElementString("Description", step.Description);
                    xw.WriteElementString("Order", step.Order.ToString());
                    xw.WriteEndElement();
                }
                xw.WriteEndElement();
                xw.WriteStartElement("WiringTriggerFeatures");
                string aux;
                foreach (WiringTriggeringFeature triggering in triggerings)
                {
                    xw.WriteStartElement("WiringTriggeringFeature");
                    xw.WriteElementString("WiringTriggeringFeatureID", triggering.WiringTriggeringFeatureID.ToString());
                    xw.WriteElementString("WiringStepID", triggering.WiringStepID.ToString());
                    xw.WriteElementString("WiringOptionID", triggering.WiringOptionID.ToString());
                    xw.WriteElementString("Quantity", triggering.Quantity.ToString());
                    xw.WriteElementString("Equality", triggering.Equality);
                    xw.WriteElementString("IsSelected", triggering.IsSelected.ToString());
                    xw.WriteEndElement();

                }
                xw.WriteEndElement();
                xw.WriteEndElement();
                xw.WriteEndDocument();
            }

            ms.Position = 0;
            return File(ms, "text/xml", "WiringSteps.xml");
        }

        public void ImportXML(IServiceProvider services)
        {
            ApplicationDbContext context = services.GetRequiredService<ApplicationDbContext>();


            XmlDocument doc = new XmlDocument();
            doc.Load(appDataFolder + "WiringSteps.xml");

            var ALLSteps = doc.DocumentElement.SelectSingleNode("//AllWiringSteps");

            var ALLXMLobs = ALLSteps.SelectSingleNode("//WiringSteps");
            var XMLobs = ALLXMLobs.SelectNodes("//WiringStep");

            var ALLtriggers = ALLSteps.SelectSingleNode("//WiringTriggerFeatures");
            var triggers = ALLtriggers.SelectNodes("//WiringTriggeringFeature");

            foreach (XmlElement XMLob in XMLobs)
            {
                var stepid = XMLob.SelectSingleNode(".//WiringStepID").InnerText;
                var jobtypeid = XMLob.SelectSingleNode(".//JobTypeID").InnerText;
                var stage = XMLob.SelectSingleNode(".//Stage").InnerText;
                var expectedtime = XMLob.SelectSingleNode(".//ExpectedTime").InnerText;
                var description = XMLob.SelectSingleNode(".//Description").InnerText;
                var order = XMLob.SelectSingleNode(".//Order").InnerText;

                context.WiringSteps.Add(new WiringStep
                {
                    WiringStepID = Int32.Parse(stepid),
                    JobTypeID = Int32.Parse(jobtypeid),
                    Stage = stage,
                    ExpectedTime = DateTime.Parse(expectedtime),
                    Description = description,
                    Order = Int32.Parse(order)
                });
                context.Database.OpenConnection();
                try
                {
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.WiringSteps ON");
                    context.SaveChanges();
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.WiringSteps OFF");
                }
                finally
                {
                    context.Database.CloseConnection();
                }
            }

            foreach (XmlElement po in triggers)
            {
                var id = po.SelectSingleNode(".//WiringTriggeringFeatureID").InnerText;
                var sttepid = po.SelectSingleNode(".//WiringStepID").InnerText;
                var wiringOptionID = po.SelectSingleNode(".//WiringOptionID").InnerText;
                var quantity = po.SelectSingleNode(".//Quantity").InnerText;
                var equality = po.SelectSingleNode(".//Equality").InnerText;
                var isselected = po.SelectSingleNode(".//IsSelected").InnerText;
                context.WiringTriggeringFeatures.Add(new WiringTriggeringFeature
                {
                    WiringTriggeringFeatureID = Int32.Parse(id),
                    WiringStepID = Int32.Parse(sttepid),
                    WiringOptionID = Int32.Parse(wiringOptionID),
                    Quantity = Int32.Parse(quantity),
                    Equality = equality,
                    IsSelected = Boolean.Parse(isselected)

                });
                context.Database.OpenConnection();
                try
                {
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.WiringTriggeringFeatures ON");
                    context.SaveChanges();
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.WiringTriggeringFeatures OFF");
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