using Microsoft.AspNetCore.Mvc;
using ProdFloor.Models;
using ProdFloor.Models.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProdFloor.Models.ViewModels.Job;

namespace ProdFloor.Controllers
{
    [Authorize(Roles = "Admin,Engineer")]
    public class JobController : Controller
    {
        private IJobRepository repository;
        private IItemRepository itemsrepository;
        private UserManager<AppUser> userManager;
        public int PageSize = 4;

        public JobController(IJobRepository repo, IItemRepository itemsrepo, UserManager<AppUser> userMgr)
        {
            repository = repo;
            itemsrepository = itemsrepo;
            userManager = userMgr;
        }

        public void alert()
        {
            TempData["alert"] = $"alert-danger";
            TempData["message"] = $"There was an error with your request";
        }
        // Recibe jobType y jobPage y regresa un 
        //JobsListViewModel con los jobs filtrados por tipo y sorteados por JobID 
        public ViewResult List(int jobType, int jobPage = 1)
        {

            var JobCount = repository.Jobs.Count();

            return View(new JobsListViewModel
            {
                    Jobs = repository.Jobs
                    .OrderBy(p => p.JobID)
                    .Skip((jobPage - 1) * PageSize)
                    .Take(PageSize),
                    PagingInfo = new PagingInfo
                    {
                       CurrentPage = jobPage,
                       ItemsPerPage = PageSize,
                       TotalItems = JobCount
                    },
                        CurrentJobType = jobType.ToString()
             });
        }

        // Al recibir un post de Delete llama a DeleteEngJob con el ID recibido y redirige 
        //a List con un mensaje de success o failure
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

        // Si recibe un get de Delete redirige a List con un mensaje de failure
        public IActionResult Delete()
        {
            TempData["alert"] = $"alert-danger";
            TempData["message"] = $"The requested Job Id doesn't exist";
            return RedirectToAction("List");
        }

        /* Post de NewJob; recibe el Job completado por el usuario si todo esta correcto procede a agregar el Job al repositorio
         * llena el campo de EngID en base al usuario que capturo la forma y el status lo pone como "Incomplete"
         * regresa un newJobViewModel a la vista NextForm con el CurrentJob = al job recien creado y un JobExtension en blanco a excepcion
         * del JobID el cual se especifica = al del Job recien creado; Si hay algun error en la forma recibida regresa el Job que recibio junto
         * a un mensaje de failure
        */
        public ViewResult NewJob()
        {

            ViewData["CountryID"] = new SelectList(itemsrepository.Countries, "CountryID", "Name");
            ViewData["StateID"] = new SelectList(itemsrepository.States, "StateID", "Name");
            ViewData["CityID"] = new SelectList(itemsrepository.Cities, "CityID", "Name");
            ViewData["Style"] = new SelectList(itemsrepository.DoorOperators.FromSql("select * from dbo.DoorOperators where dbo.DoorOperators.DoorOperatorID " +
                "in (Select max(dbo.DoorOperators.DoorOperatorID) FROM dbo.DoorOperators group by dbo.DoorOperators.Style); "), "Style", "Style");
            ViewData["Brand2"] = new SelectList(itemsrepository.DoorOperators, "Brand", "Brand");
            ViewData["DoorOperatorID"] = new SelectList(itemsrepository.DoorOperators, "DoorOperatorID", "Name");

            return View(new Job());
        } 

        [HttpPost]
        public IActionResult NewJob(Job newJob)
        {
            ViewData["CountryID"] = new SelectList(itemsrepository.Countries, "CountryID", "Name");
            ViewData["StateID"] = new SelectList(itemsrepository.States, "StateID", "Name");
            ViewData["CityID"] = new SelectList(itemsrepository.Cities, "CityID", "Name");
            ViewData["Style"] = new SelectList(itemsrepository.DoorOperators.FromSql("select * from dbo.DoorOperators where dbo.DoorOperators.DoorOperatorID " +
                "in (Select max(dbo.DoorOperators.DoorOperatorID) FROM dbo.DoorOperators group by dbo.DoorOperators.Style); "), "Style", "Style");
            ViewData["Brand2"] = new SelectList(itemsrepository.DoorOperators, "Brand", "Brand");
            ViewData["DoorOperatorID"] = new SelectList(itemsrepository.DoorOperators, "DoorOperatorID", "Name");
            AppUser currentUser = GetCurrentUser().Result;
            if (ModelState.IsValid)
            {
                newJob.EngID = currentUser.EngID;
                newJob.CrossAppEngID = 117;
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
                    SpecialFeatureslist = new List<SpecialFeatures> { new SpecialFeatures() },
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

        /* Get de Edit; Si el ID recibido exite en el repositorio regresa un JobViewModel con los objetos relacionados a este ID,
         * de lo contrario regresa un mensaje de failure a la view List
         */
        public IActionResult Edit(int ID)
        {
            ViewData["CountryID"] = new SelectList(itemsrepository.Countries, "CountryID", "Name");
            ViewData["StateID"] = new SelectList(itemsrepository.States, "StateID", "Name");
            ViewData["CityID"] = new SelectList(itemsrepository.Cities, "CityID", "Name");
            ViewData["Style"] = new SelectList(itemsrepository.DoorOperators.FromSql("select * from dbo.DoorOperators where dbo.DoorOperators.DoorOperatorID " +
                "in (Select max(dbo.DoorOperators.DoorOperatorID) FROM dbo.DoorOperators group by dbo.DoorOperators.Style); "), "Style", "Style");
            ViewData["Brand2"] = new SelectList(itemsrepository.DoorOperators, "Brand", "Brand");
            ViewData["DoorOperatorID"] = new SelectList(itemsrepository.DoorOperators, "DoorOperatorID", "Name");

            Job job = repository.Jobs.FirstOrDefault(j => j.JobID == ID);
            if (job == null)
            {
                TempData["message"] = $"The requested Job doesn't exist.";
                return RedirectToAction("List");
            }
            else
            {
                List<SpecialFeatures> SfList = repository.SpecialFeatures.Where(j => j.JobID == ID).ToList();
                JobViewModel viewModel = new JobViewModel();
                viewModel.CurrentJob = job;
                viewModel.CurrentJobExtension = repository.JobsExtensions.FirstOrDefault(j => j.JobID == ID);
                viewModel.CurrentHydroSpecific = repository.HydroSpecifics.FirstOrDefault(j => j.JobID == ID);
                viewModel.CurrentGenericFeatures = repository.GenericFeaturesList.FirstOrDefault(j => j.JobID == ID);
                viewModel.CurrentIndicator = repository.Indicators.FirstOrDefault(j => j.JobID == ID);
                viewModel.CurrentHoistWayData = repository.HoistWayDatas.FirstOrDefault(j => j.JobID == ID);
                if(SfList != null)
                {
                    viewModel.SpecialFeatureslist = SfList;
                }
                else
                {
                    viewModel.SpecialFeatureslist = new List<SpecialFeatures> { new SpecialFeatures() };
                }
                viewModel.CurrentTab = "Main";
                return View(viewModel);
            }

        }

        /* Post de Edit; recibe un JobViewModel, si todo esta bien procede a salvar cada objeto en el repositorio y en caso de que el status
         * este en blanco o no este configurado procede a cambiarlo a "Working on it"; Si el modelo recibido contiene algun error regresa el
         * modelo a la vista con un mensaje de failure
         */
        [HttpPost]
        public IActionResult Edit(JobViewModel multiEditViewModel)
        {
            ViewData["CountryID"] = new SelectList(itemsrepository.Countries, "CountryID", "Name");
            ViewData["StateID"] = new SelectList(itemsrepository.States, "StateID", "Name");
            ViewData["CityID"] = new SelectList(itemsrepository.Cities, "CityID", "Name");
            ViewData["Style"] = new SelectList(itemsrepository.DoorOperators.FromSql("select * from dbo.DoorOperators where dbo.DoorOperators.DoorOperatorID " +
                "in (Select max(dbo.DoorOperators.DoorOperatorID) FROM dbo.DoorOperators group by dbo.DoorOperators.Style); "), "Style", "Style");
            ViewData["Brand2"] = new SelectList(itemsrepository.DoorOperators, "Brand", "Brand");
            ViewData["DoorOperatorID"] = new SelectList(itemsrepository.DoorOperators, "DoorOperatorID", "Name");

            if (ModelState.IsValid)
            {
                
                if (multiEditViewModel.CurrentJob.Status == "" || multiEditViewModel.CurrentJob.Status == null)
                {
                        multiEditViewModel.CurrentJob.Status = "Working on it";
                }
                repository.SaveEngJobView(multiEditViewModel);

                multiEditViewModel.CurrentTab = "Main";
                TempData["message"] = $"{multiEditViewModel.CurrentJob.JobNum} ID has been saved...{multiEditViewModel.CurrentJob.JobID}";
                 return View(multiEditViewModel);
             }else
             {
                    // there is something wrong with the data values
                    TempData["message"] = $"There seems to be errors in the form. Please validate.";
                    TempData["alert"] = $"alert-danger";
                    return View(multiEditViewModel);
              }
        }

        public IActionResult AddSF(int Id)
        {
            Job currentJob = repository.Jobs.FirstOrDefault(p => p.JobID == Id);
            JobExtension extension = repository.JobsExtensions.FirstOrDefault(p => p.JobID == Id);
            HydroSpecific hydro = repository.HydroSpecifics.FirstOrDefault(p => p.JobID == Id);
            GenericFeatures generic = repository.GenericFeaturesList.FirstOrDefault(p => p.JobID == Id);
            Indicator indicator = repository.Indicators.FirstOrDefault(p => p.JobID == Id);
            HoistWayData hoist = repository.HoistWayDatas.FirstOrDefault(p => p.JobID == Id);
            IQueryable<SpecialFeatures> currentSpecialF = (repository.SpecialFeatures.Where(p => p.JobID == Id));

            JobViewModel viewModel = new JobViewModel
            {
                CurrentJob = currentJob,
                SpecialFeatureslist = currentSpecialF.ToList(),
                CurrentJobExtension = extension,
                CurrentHydroSpecific = hydro,
                CurrentGenericFeatures = generic,
                CurrentIndicator = indicator,
                CurrentHoistWayData = hoist,
                CurrentTab = "SpecialFeatures"
            };
            return View("Edit", viewModel);
        }

        [HttpPost]
        public IActionResult DeleteSF(int fieldID, string returnUrl, JobViewModel viewModel)
        {
            SpecialFeatures deletedField = repository.DeleteSpecialFeatures(fieldID);
            if (deletedField != null)
            {
                TempData["message"] = $"{deletedField.SpecialFeaturesID} was deleted";
            }
            else
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"There was an error with your request{fieldID}";
            }
            return Redirect(returnUrl);
        }

        /* Get de Continue; esta clase es para cuando el usuario estaba capturando un job y por algun motivo no lo termino;
         * recibe un ID y busca los objetos relacionados a este, en caso de encontrar un Job con este ID regresa un JobViewModel a NextForm con los 
         * objetos que concuerdan con el ID, de lo contrario manda a List con un mensaje de Failure
         */
        public IActionResult Continue(int ID)
        {
            ViewData["CountryID"] = new SelectList(itemsrepository.Countries, "CountryID", "Name");
            ViewData["StateID"] = new SelectList(itemsrepository.States, "StateID", "Name");
            ViewData["CityID"] = new SelectList(itemsrepository.Cities, "CityID", "Name");
            ViewData["Style"] = new SelectList(itemsrepository.DoorOperators.FromSql("select * from dbo.DoorOperators where dbo.DoorOperators.DoorOperatorID " +
                "in (Select max(dbo.DoorOperators.DoorOperatorID) FROM dbo.DoorOperators group by dbo.DoorOperators.Style); "), "Style", "Style");
            ViewData["Brand2"] = new SelectList(itemsrepository.DoorOperators, "Brand", "Brand");
            ViewData["DoorOperatorID"] = new SelectList(itemsrepository.DoorOperators, "DoorOperatorID", "Name");

            if (repository.Jobs.FirstOrDefault(j => j.JobID == ID) != null)
            {
                List<SpecialFeatures> SfList = repository.SpecialFeatures.Where(j => j.JobID == ID).ToList();
                JobViewModel continueJobViewModel = new JobViewModel();
                continueJobViewModel.CurrentTab = "Main";
                continueJobViewModel.CurrentJob = repository.Jobs.FirstOrDefault(j => j.JobID == ID);
                continueJobViewModel.CurrentJobExtension = (repository.JobsExtensions.FirstOrDefault(j => j.JobID == ID) ?? new JobExtension());
                continueJobViewModel.CurrentHydroSpecific = (repository.HydroSpecifics.FirstOrDefault(j => j.JobID == ID) ?? new HydroSpecific());
                continueJobViewModel.CurrentGenericFeatures = (repository.GenericFeaturesList.FirstOrDefault(j => j.JobID == ID) ?? new GenericFeatures());
                continueJobViewModel.CurrentIndicator = (repository.Indicators.FirstOrDefault(j => j.JobID == ID) ?? new Indicator());
                continueJobViewModel.CurrentHoistWayData = (repository.HoistWayDatas.FirstOrDefault(j => j.JobID == ID) ?? new HoistWayData());
                if (SfList.Count > 1)
                {

                    continueJobViewModel.SpecialFeatureslist = SfList;

                }
                else
                {
                    continueJobViewModel.SpecialFeatureslist = new List<SpecialFeatures> { new SpecialFeatures() };
                }

                return View("NextForm", continueJobViewModel);

            }
            else
            {
                TempData["message"] = $"The requested Job Id# {ID} doesn't exist";
                return RedirectToAction("List");
            }
        }

        /* ***TODO*** Post de NextForm; Recibe un JobViewModel y en verifica en que paso se encuentra el Job
         * Este objeto siempre contendra un Job completo pero los siguientes pueden no estarlo, los if's validan si el objeto no es nulo
         * en caso de que no lo sean revisa el siguiente objeto, si es nulo regresa un objeto nuevo con el jobID para ese objeto,
         * los siguientes objetos los regresa vacios y los anteriores los salva en la DB, tambien regresa la informacion de la tab actual, 
         * en caso de que el ultimo objeto a revisar este completo cambia el status a "Working on it" y salva todo:
         * 1- JobExtension
         * 2- HydroSpecific
         * 3- CurrentIndicator
         * 4- HoistWayData
         * 5- SpecialFeatures
         */
        [HttpPost]
        public IActionResult NextForm(JobViewModel nextViewModel)
        {
            ViewData["CountryID"] = new SelectList(itemsrepository.Countries, "CountryID", "Name");
            ViewData["StateID"] = new SelectList(itemsrepository.States, "StateID", "Name");
            ViewData["CityID"] = new SelectList(itemsrepository.Cities, "CityID", "Name");
            ViewData["Style"] = new SelectList(itemsrepository.DoorOperators.FromSql("select * from dbo.DoorOperators where dbo.DoorOperators.DoorOperatorID " +
                "in (Select max(dbo.DoorOperators.DoorOperatorID) FROM dbo.DoorOperators group by dbo.DoorOperators.Style); "), "Style", "Style");
            ViewData["Brand2"] = new SelectList(itemsrepository.DoorOperators, "Brand", "Brand");
            ViewData["DoorOperatorID"] = new SelectList(itemsrepository.DoorOperators, "DoorOperatorID", "Name");

            if (nextViewModel.buttonAction == "AddSF")
            {
                nextViewModel.SpecialFeatureslist.Add(new SpecialFeatures { JobID = nextViewModel.CurrentJob.JobID });
                nextViewModel.CurrentTab = "SpecialFeatures";
            }
            else
            {
                if (ModelState.IsValid)
                {
                    if (nextViewModel.CurrentJobExtension != null)
                    {
                        if (nextViewModel.CurrentHydroSpecific != null)
                        {
                            if (nextViewModel.CurrentGenericFeatures != null)
                            {
                                if (nextViewModel.CurrentIndicator != null)
                                {
                                    if (nextViewModel.CurrentHoistWayData != null)
                                    {
                                        if (nextViewModel.SpecialFeatureslist != null)
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
                                            nextViewModel.CurrentTab = "SpecialFeatures";
                                            TempData["message"] = $"HoistWayData was saved";
                                            nextViewModel.SpecialFeatureslist = new List<SpecialFeatures> { new SpecialFeatures {JobID = nextViewModel.CurrentJob.JobID } };
                                            return View(nextViewModel);
                                        }

                                    }
                                    else
                                    {
                                        repository.SaveEngJobView(nextViewModel);
                                        nextViewModel.CurrentHoistWayData = new HoistWayData { JobID = nextViewModel.CurrentJob.JobID };
                                        nextViewModel.SpecialFeatureslist = new List<SpecialFeatures> { new SpecialFeatures() };
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
                                    nextViewModel.SpecialFeatureslist = new List<SpecialFeatures> { new SpecialFeatures() };
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
                                nextViewModel.SpecialFeatureslist = new List<SpecialFeatures> { new SpecialFeatures() };
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
                            nextViewModel.SpecialFeatureslist = new List<SpecialFeatures> { new SpecialFeatures() };
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
                        nextViewModel.CurrentHydroSpecific = new HydroSpecific();
                        nextViewModel.CurrentGenericFeatures = new GenericFeatures();
                        nextViewModel.CurrentIndicator = new Indicator();
                        nextViewModel.CurrentHoistWayData = new HoistWayData();
                        nextViewModel.SpecialFeatureslist = new List<SpecialFeatures> { new SpecialFeatures() };
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
                    nextViewModel.SpecialFeatureslist = (nextViewModel.SpecialFeatureslist ?? new List<SpecialFeatures> { new SpecialFeatures() });
                    TempData["message"] = $"nothing was saved";
                    return View(nextViewModel);
                }
            }

            return View(nextViewModel);
        }

        //Funcion para obtener el usuario que realizo la llamada
        private async Task<AppUser> GetCurrentUser()
        {
            AppUser user = await userManager.GetUserAsync(HttpContext.User);

            return user;
        }


        public async Task<ViewResult> JobSearchList(JobSearchViewModel searchViewModel, int jobPage = 1)
        {
            var JobCount = repository.Jobs.Count();
            var jobSearchRepo = repository.Jobs.Include( j => j._jobExtension).AsQueryable();
            IQueryable<string> statusQuery = from s in repository.Jobs orderby s.Status select s.Status;
            //Opciones de busqueda para el modelo principal de job
            if (!string.IsNullOrEmpty(searchViewModel.NameJobSearch)) jobSearchRepo = jobSearchRepo.Where(s => s.Name.Contains(searchViewModel.NameJobSearch));//Busqueda por Job Name si este no es nulo
            if (searchViewModel.NumJobSearch >= 2015000000 && searchViewModel.NumJobSearch <= 2021000000) jobSearchRepo = jobSearchRepo.Where(s => s.JobNum == searchViewModel.NumJobSearch);//Busqueda por Job Number si esta en el rango + la anterior
            if (!string.IsNullOrEmpty(searchViewModel.CustJobSearch)) jobSearchRepo = jobSearchRepo.Where(s => s.Cust.Contains(searchViewModel.CustJobSearch));//Busqueda por Job CustID si no es nulo + las anteriores
            if (!string.IsNullOrEmpty(searchViewModel.ContractorJobSearch)) jobSearchRepo = jobSearchRepo.Where(s => s.Contractor.Contains(searchViewModel.ContractorJobSearch));//Busqueda por Job Contractor si no es nulo + las anteriores
            if (searchViewModel.POJobSearch >= 3000000 && searchViewModel.POJobSearch <= 4900000) jobSearchRepo = repository.Jobs.Where(s => s.PO == searchViewModel.POJobSearch);//Busqueda por Job PO si esta en el rango + las anteriores
            if (searchViewModel.EngID != 0) jobSearchRepo = jobSearchRepo.Where(s => s.EngID == searchViewModel.EngID);//Busqueda por el Id de Ingeniero si este no es 0 + las anteriores
            if (searchViewModel.CrossAppEngID != 0) jobSearchRepo = jobSearchRepo.Where(s => s.CrossAppEngID == searchViewModel.CrossAppEngID);//Busqueda por el Id de Ingeniero si este no es 0 + las anteriores
            if (!string.IsNullOrEmpty(searchViewModel.StatusJobSearch)) jobSearchRepo = jobSearchRepo.Where(s => s.Status == searchViewModel.StatusJobSearch);//Busqueda por Job Status si no es nulo + las anteriores
            if (searchViewModel.CityID != 0) jobSearchRepo = jobSearchRepo.Where(s => s.CityID == searchViewModel.CityID);//Busqueda por el CityID si este no es 0 + las anteriores
            if (searchViewModel.FireCodeID != 0) jobSearchRepo = jobSearchRepo.Where(s => s.FireCodeID == searchViewModel.FireCodeID);//Busqueda por el FireCodeID si este no es 0 + las anteriores
            if (searchViewModel.JobTypeID != 0) jobSearchRepo = jobSearchRepo.Where(s => s.JobTypeID == searchViewModel.JobTypeID);//Busqueda por el JobTypeID si este no es 0 + las anteriores
            //Opciones de busqueda para el modelo de jobExtensions.
            if (!string.IsNullOrEmpty(searchViewModel.AuxCop)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.AuxCop == "Si" ? s._jobExtension.AUXCOP == true : s._jobExtension.AUXCOP == false);
            if (!string.IsNullOrEmpty(searchViewModel.CartopDoorButtons)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.CartopDoorButtons == "Si" ? s._jobExtension.CartopDoorButtons == true : s._jobExtension.CartopDoorButtons == false);
            if (!string.IsNullOrEmpty(searchViewModel.DoorHold)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.DoorHold == "Si" ? s._jobExtension.DoorHold == true : s._jobExtension.DoorHold == false);
            if (!string.IsNullOrEmpty(searchViewModel.HeavyDoors)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.HeavyDoors == "Si" ? s._jobExtension.HeavyDoors == true : s._jobExtension.HeavyDoors == false);
            if (!string.IsNullOrEmpty(searchViewModel.InfDetector)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.InfDetector == "Si" ? s._jobExtension.InfDetector == true : s._jobExtension.InfDetector == false);
            if (!string.IsNullOrEmpty(searchViewModel.MechSafEdge)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.MechSafEdge == "Si" ? s._jobExtension.MechSafEdge == true : s._jobExtension.MechSafEdge == false);
            if (!string.IsNullOrEmpty(searchViewModel.Scop)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.Scop == "Si" ? s._jobExtension.SCOP == true : s._jobExtension.SCOP == false);
            if (!string.IsNullOrEmpty(searchViewModel.Shc)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.Shc == "Si" ? s._jobExtension.SHC == true : s._jobExtension.SHC == false);

            if (searchViewModel.InputFrecuency >= 50 && searchViewModel.InputFrecuency <= 61) jobSearchRepo = jobSearchRepo.Where(s => s._jobExtension.InputFrecuency == searchViewModel.InputFrecuency);
            if (searchViewModel.InputPhase >= 1 && searchViewModel.InputPhase <= 3) jobSearchRepo = jobSearchRepo.Where(s => s._jobExtension.InputPhase == searchViewModel.InputPhase);
            if (searchViewModel.InputVoltage >= 114 && searchViewModel.InputVoltage <= 600) jobSearchRepo = jobSearchRepo.Where(s => s._jobExtension.InputVoltage == searchViewModel.InputVoltage);
            if (searchViewModel.NumOfStops >= 1 && searchViewModel.NumOfStops <= 32) jobSearchRepo = jobSearchRepo.Where(s => s._jobExtension.NumOfStops == searchViewModel.NumOfStops);

            if (!string.IsNullOrEmpty(searchViewModel.JobTypeAdd)) jobSearchRepo = jobSearchRepo.Where(s => s._jobExtension.JobTypeAdd.Equals(searchViewModel.JobTypeAdd));
            if (!string.IsNullOrEmpty(searchViewModel.JobTypeMain)) jobSearchRepo = jobSearchRepo.Where(s => s._jobExtension.JobTypeMain.Equals(searchViewModel.JobTypeMain));
            int TotalItemsSearch = jobSearchRepo.Count() +1;
            JobSearchViewModel jobSearch = new JobSearchViewModel
            {
                Status = new SelectList(await statusQuery.Distinct().ToListAsync()),
                JobsSearchList = await jobSearchRepo.OrderBy(p => p.JobID).Skip((jobPage - 1) * 10).Take(TotalItemsSearch).ToListAsync(),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = jobPage,
                    ItemsPerPage = 10,
                    TotalItems = TotalItemsSearch
                },
            };

            return View(jobSearch);
        }

        //Funciones para el llenado de los dropdowns en casacada
        public JsonResult GetJobState(int CountryID)
        {
            List<State> JobStatelist = new List<State>();
            JobStatelist = (from state in itemsrepository.States where state.CountryID == CountryID select state).ToList();
            return Json(new SelectList(JobStatelist, "StateID", "Name"));
        }

        public JsonResult GetJobCity(int StateID)
        {
            List<City> CityCascadeList = new List<City>();
            CityCascadeList = (from city in itemsrepository.Cities where city.StateID == StateID select city).ToList();
            return Json(new SelectList(CityCascadeList, "CityID", "Name"));
        }

        public JsonResult GetBrand(string Style)
        {
            List<DoorOperator> BrandList = new List<DoorOperator>();
            BrandList = itemsrepository.DoorOperators.FromSql("select * from dbo.DoorOperators where Style = {0} AND dbo.DoorOperators.DoorOperatorID in "+
                "(Select max(dbo.DoorOperators.DoorOperatorID) FROM dbo.DoorOperators group by dbo.DoorOperators.Brand)",Style).ToList();
            return Json(new SelectList(BrandList, "Brand", "Brand"));
        }

        public JsonResult GetDoorOperatorID(string Brand)
        {
            List<DoorOperator> DoorOperatorList = new List<DoorOperator>();
            DoorOperatorList = (from door in itemsrepository.DoorOperators where door.Brand == Brand select door).ToList();
            return Json(new SelectList(DoorOperatorList, "DoorOperatorID", "Name"));
        }

    }
}
