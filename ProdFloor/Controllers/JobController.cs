using Microsoft.AspNetCore.Mvc;
using ProdFloor.Models;
using ProdFloor.Models.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace ProdFloor.Controllers
{
    [Authorize(Roles = "Admin,Engineer")]
    public class JobController : Controller
    {
        private IJobRepository repository;
        private IItemRepository itemsrepository;
        private UserManager<AppUser> userManager;
        public int PageSize = 4;

        public JobController(IJobRepository repo,IItemRepository itemsrepo, UserManager<AppUser> userMgr)
        {
            repository = repo;
            itemsrepository = itemsrepo;
            userManager = userMgr;
        }
        
        public ViewResult List(int jobType, int jobPage = 1)
            => View(new JobsListViewModel
            {//Checar si vuelve a funcionar CON JOINS!!!!!!!!!!!!
                Jobs = repository.Jobs
                .Where(j => jobType == null || j.JobTypeID == jobType)
                .OrderBy(p => p.JobID)
                .Skip((jobPage - 1) * PageSize)
                .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = jobPage,
                    ItemsPerPage = PageSize,
                    TotalItems =jobType == null ?
                    repository.Jobs.Count() :
                    repository.Jobs.Where(e =>
                    e.JobTypeID == jobType).Count()
                },
                CurrentJobType = jobType.ToString()
            });

        [HttpPost]
        public IActionResult Delete(int ID)
        {
            Job deletedJob = repository.DeleteEngJob(ID);
            if (deletedJob != null)
            {
                TempData["message"] = $"{deletedJob.Name} was deleted";
            }
            else
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"There was an error with your request";
            }
            return RedirectToAction("List");
        }

        public IActionResult Delete()
        {
            TempData["message"] = $"The requested Job Id doesn't exist";
            return RedirectToAction("List");
        }

        public ViewResult NewJob()
        {

            List<City> CityList = new List<City>();
            //Getting Data
            CityList = (from city in itemsrepository.Cities select city).ToList();
            //Insert Select item in list
            CityList.Insert(0, new City { CityID = 0, Name = "Select" });
            //Assigning categorlist to viewbag
            ViewBag.CityList = CityList;

            List<FireCode> FireCodeList = new List<FireCode>();
            //Getting Data
            FireCodeList = (from firecode in itemsrepository.FireCodes select firecode).ToList();
            //Insert Select item in list
            FireCodeList.Insert(0, new FireCode { FireCodeID = 0, Name = "Select" });
            //Assigning categorlist to viewbag
            ViewBag.FireCodeList = FireCodeList;

            List<JobType> JobTypeList = new List<JobType>();
            //Getting Data
            JobTypeList = (from jobtype in itemsrepository.JobTypes select jobtype).ToList();
            //Insert Select item in list
            JobTypeList.Insert(0, new JobType { JobTypeID = 0, Name = "Select" });
            //Assigning categorlist to viewbag
            ViewBag.JobTypeList = JobTypeList;

            return View(new Job());
        } 

        [HttpPost]
        public IActionResult NewJob(Job newJob)
        {

            List<DoorOperator> DoorOperatorList = new List<DoorOperator>();
            //Getting Data
            DoorOperatorList = (from doorOperator in itemsrepository.DoorOperators select doorOperator).ToList();
            //Insert Select item in list
            DoorOperatorList.Insert(0, new DoorOperator { DoorOperatorID = 0, Name = "Select" });
            //Assigning categorlist to viewbag
            ViewBag.DoorOperatorList = DoorOperatorList;

            AppUser currentUser = GetCurrentUser().Result;
            if (ModelState.IsValid)
            {
                newJob.EngID = currentUser.EngID;
                newJob.Status = "Incomplete";
                repository.SaveJob(newJob);
                JobViewModel newJobViewModel = new JobViewModel
                {
                    CurrentJob = newJob,
                    CurrentJobExtension = new JobExtension { JobID = newJob.JobID },
                    CurrentHydroSpecific = new HydroSpecific(),
                    CurrentGenericFeatures = new GenericFeatures (),
                    CurrentIndicator = new Indicator (),
                    CurrentHoistWayData = new HoistWayData (),
                    CurrentSpecialFeatures = new SpecialFeatures(),
                    CurrentTab = "Extension"
                };
                TempData["message"] = $"Job# {newJobViewModel.CurrentJob.JobNum} has been saved...{newJobViewModel.CurrentJob.JobID}...{currentUser.EngID}";
                return View("NextForm", newJobViewModel);


            }
            else
            {
                TempData["message"] = $"There seems to be errors in the form. Please validate....{currentUser.EngID}";
                TempData["alert"] = $"alert-danger";
                return View(newJob);
            }
        }
        
        public IActionResult Edit(int ID)
        {
            List<DoorOperator> DoorOperatorList = new List<DoorOperator>();
            //Getting Data
            DoorOperatorList = (from doorOperator in itemsrepository.DoorOperators select doorOperator).ToList();
            //Insert Select item in list
            DoorOperatorList.Insert(0, new DoorOperator { DoorOperatorID = 0, Name = "Select" });
            //Assigning categorlist to viewbag
            ViewBag.DoorOperatorList = DoorOperatorList;

            Job job = repository.Jobs.FirstOrDefault(j => j.JobID == ID);
            if (job == null)
            {
                TempData["message"] = $"The requested Job doesn't exist.";
                return RedirectToAction("List");
            }
            else
            {
                JobViewModel viewModel = new JobViewModel();
                viewModel.CurrentJob = job;
                viewModel.CurrentJobExtension = repository.JobsExtensions.FirstOrDefault(j => j.JobID == ID);
                viewModel.CurrentHydroSpecific = repository.HydroSpecifics.FirstOrDefault(j => j.JobID == ID);
                viewModel.CurrentGenericFeatures = repository.GenericFeaturesList.FirstOrDefault(j => j.JobID == ID);
                viewModel.CurrentIndicator = repository.Indicators.FirstOrDefault(j => j.JobID == ID);
                viewModel.CurrentHoistWayData = repository.HoistWayDatas.FirstOrDefault(j => j.JobID == ID);
                viewModel.CurrentSpecialFeatures = repository.SpecialFeatures.FirstOrDefault(j => j.JobID == ID);
                viewModel.CurrentTab = "Main";
                return View(viewModel);
            }

        }

        [HttpPost]
        public IActionResult Edit(JobViewModel multiEditViewModel)
        {
            List<DoorOperator> DoorOperatorList = new List<DoorOperator>();
            //Getting Data
            DoorOperatorList = (from doorOperator in itemsrepository.DoorOperators select doorOperator).ToList();
            //Insert Select item in list
            DoorOperatorList.Insert(0, new DoorOperator { DoorOperatorID = 0, Name = "Select" });
            //Assigning categorlist to viewbag
            ViewBag.DoorOperatorList = DoorOperatorList;

            if (ModelState.IsValid)
            {
                if(multiEditViewModel.CurrentJob.Status == "" || multiEditViewModel.CurrentJob.Status == null)
                {
                    multiEditViewModel.CurrentJob.Status = "Working on it";
                }
                repository.SaveEngJobView(multiEditViewModel);
                multiEditViewModel.CurrentTab = "Main";
                TempData["message"] = $"{multiEditViewModel.CurrentJob.JobNum} ID has been saved...{multiEditViewModel.CurrentJob.JobID}";
                return View(multiEditViewModel);
            }
            else
            {
                // there is something wrong with the data values
                TempData["message"] = $"There seems to be errors in the form. Please validate.";
                TempData["alert"] = $"alert-danger";
                return View(multiEditViewModel);
            }
        }

        public IActionResult Continue(int ID)
        {
            List<DoorOperator> DoorOperatorList = new List<DoorOperator>();
            //Getting Data
            DoorOperatorList = (from doorOperator in itemsrepository.DoorOperators select doorOperator).ToList();
            //Insert Select item in list
            DoorOperatorList.Insert(0, new DoorOperator { DoorOperatorID = 0, Name = "Select" });
            //Assigning categorlist to viewbag
            ViewBag.DoorOperatorList = DoorOperatorList;

            if (repository.Jobs.FirstOrDefault(j => j.JobID == ID) != null)
            {

                JobViewModel continueJobViewModel = new JobViewModel();
                continueJobViewModel.CurrentJob = repository.Jobs.FirstOrDefault(j => j.JobID == ID);
                continueJobViewModel.CurrentJobExtension = (repository.JobsExtensions.FirstOrDefault(j => j.JobID == ID) ?? new JobExtension());
                continueJobViewModel.CurrentHydroSpecific = (repository.HydroSpecifics.FirstOrDefault(j => j.JobID == ID) ?? new HydroSpecific());
                continueJobViewModel.CurrentGenericFeatures = (repository.GenericFeaturesList.FirstOrDefault(j => j.JobID == ID) ?? new GenericFeatures());
                continueJobViewModel.CurrentIndicator = (repository.Indicators.FirstOrDefault(j => j.JobID == ID) ?? new Indicator());
                continueJobViewModel.CurrentHoistWayData = (repository.HoistWayDatas.FirstOrDefault(j => j.JobID == ID) ?? new HoistWayData());
                continueJobViewModel.CurrentSpecialFeatures = (repository.SpecialFeatures.FirstOrDefault(j => j.JobID == ID) ?? new SpecialFeatures());

                

                return View("NextForm", continueJobViewModel);

            }
            else
            {
                TempData["message"] = $"The requested Job Id# {ID} doesn't exist";
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public IActionResult NextForm(JobViewModel nextViewModel)
        {
            List<DoorOperator> DoorOperatorList = new List<DoorOperator>();
            //Getting Data
            DoorOperatorList = (from doorOperator in itemsrepository.DoorOperators select doorOperator).ToList();
            //Insert Select item in list
            DoorOperatorList.Insert(0, new DoorOperator { DoorOperatorID = 0, Name = "Select" });
            //Assigning categorlist to viewbag
            ViewBag.DoorOperatorList = DoorOperatorList;

            if (ModelState.IsValid)
            {
                if(nextViewModel.CurrentJobExtension != null)
                {
                    if (nextViewModel.CurrentHydroSpecific != null)
                    {
                        if (nextViewModel.CurrentGenericFeatures != null)
                        {
                            if (nextViewModel.CurrentIndicator != null)
                            {
                                if (nextViewModel.CurrentHoistWayData != null)
                                {
                                    if(nextViewModel.CurrentSpecialFeatures != null)
                                    {
                                        nextViewModel.CurrentJob.Status = "Working on it";
                                        repository.SaveEngJobView(nextViewModel);
                                        nextViewModel.CurrentTab = "Main";
                                        TempData["message"] = $"everything was saved";
                                        // Here the Job Filling Status should be changed the Working on it
                                        // Redirect to Hub??
                                        return View(nextViewModel);
                                    }
                                    else
                                    {
                                        repository.SaveEngJobView(nextViewModel);
                                        nextViewModel.CurrentSpecialFeatures = new SpecialFeatures { JobID = nextViewModel.CurrentJob.JobID };
                                        nextViewModel.CurrentTab = "SpecialFeatures";
                                        TempData["message"] = $"HoistWayData was saved";
                                        return View(nextViewModel);
                                    }
                                    
                                }
                                else
                                {
                                    repository.SaveEngJobView(nextViewModel);
                                    nextViewModel.CurrentHoistWayData = new HoistWayData { JobID = nextViewModel.CurrentJob.JobID };
                                    nextViewModel.CurrentSpecialFeatures = new SpecialFeatures();
                                    nextViewModel.CurrentTab = "HoistWayData";
                                    TempData["message"] = $"indicator was saved";
                                    return View(nextViewModel);
                                }
                            }
                            else
                            {
                                repository.SaveEngJobView(nextViewModel);
                                nextViewModel.CurrentIndicator = new Indicator { JobID = nextViewModel.CurrentJob.JobID };
                                nextViewModel.CurrentHoistWayData = new HoistWayData();
                                nextViewModel.CurrentSpecialFeatures = new SpecialFeatures();
                                nextViewModel.CurrentTab = "Indicator";
                                TempData["message"] = $"generic was saved";
                                return View(nextViewModel);
                            }
                        }
                        else
                        {
                            repository.SaveEngJobView(nextViewModel);
                            nextViewModel.CurrentGenericFeatures = new GenericFeatures { JobID = nextViewModel.CurrentJob.JobID };
                            nextViewModel.CurrentIndicator = new Indicator();
                            nextViewModel.CurrentHoistWayData = new HoistWayData();
                            nextViewModel.CurrentSpecialFeatures = new SpecialFeatures();
                            nextViewModel.CurrentTab = "GenericFeatures";
                            TempData["message"] = $"hydro specific was saved";
                            return View(nextViewModel);
                        }
                    }
                    else
                    {
                        repository.SaveEngJobView(nextViewModel);
                        nextViewModel.CurrentHydroSpecific = new HydroSpecific { JobID = nextViewModel.CurrentJob.JobID };
                        nextViewModel.CurrentGenericFeatures = new GenericFeatures();
                        nextViewModel.CurrentIndicator = new Indicator();
                        nextViewModel.CurrentHoistWayData = new HoistWayData();
                        nextViewModel.CurrentSpecialFeatures = new SpecialFeatures();
                        nextViewModel.CurrentTab = "HydroSpecifics";
                        TempData["message"] = $"jobextension was saved";
                        return View(nextViewModel);
                    }
                }
                else
                {
                    
                    repository.SaveEngJobView(nextViewModel);
                    JobExtension jobExt = repository.JobsExtensions.FirstOrDefault(j => j.JobID == nextViewModel.CurrentJob.JobID);
                    nextViewModel.CurrentJobExtension = (jobExt ?? new JobExtension { JobID = nextViewModel.CurrentJob.JobID });
                    nextViewModel.CurrentHydroSpecific = new HydroSpecific ();
                    nextViewModel.CurrentGenericFeatures = new GenericFeatures();
                    nextViewModel.CurrentIndicator = new Indicator();
                    nextViewModel.CurrentHoistWayData = new HoistWayData();
                    nextViewModel.CurrentSpecialFeatures = new SpecialFeatures();
                    nextViewModel.CurrentTab = "Extension";
                    TempData["message"] = $"job was saved";
                    return View(nextViewModel);
                }
            }
            else
            {
                nextViewModel.CurrentJob = (nextViewModel.CurrentJob ?? new Job());
                nextViewModel.CurrentJobExtension = (nextViewModel.CurrentJobExtension ?? new JobExtension());
                nextViewModel.CurrentHydroSpecific = (nextViewModel.CurrentHydroSpecific ?? new HydroSpecific());
                nextViewModel.CurrentGenericFeatures = (nextViewModel.CurrentGenericFeatures ?? new GenericFeatures());
                nextViewModel.CurrentIndicator = (nextViewModel.CurrentIndicator ?? new Indicator());
                nextViewModel.CurrentHoistWayData = (nextViewModel.CurrentHoistWayData ?? new HoistWayData());
                nextViewModel.CurrentSpecialFeatures = (nextViewModel.CurrentSpecialFeatures ?? new SpecialFeatures());
                TempData["message"] = $"nothing was saved";
                return View(nextViewModel);
            }
        }

        private async Task<AppUser> GetCurrentUser()
        {
            AppUser user = await userManager.GetUserAsync(HttpContext.User);

            return user;
        }
    }
}
