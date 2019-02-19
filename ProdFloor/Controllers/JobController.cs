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
            var jobSearchRepo = repository.Jobs.Include( j => j._jobExtension).Include( hy => hy._HydroSpecific).Include(g => g._GenericFeatures)
                .Include( i => i._Indicator).Include(ho => ho._HoistWayData).Include(sp => sp._SpecialFeatureslist).AsQueryable();
            IQueryable<string> statusQuery = from s in repository.Jobs orderby s.Status select s.Status;
            /*
             * 
            **Campos de tipo Numerico: Primero checa que el valor introoducido este en el rango adecuado y/o mayor a cero,
              despues regresa los trabajos que son iguales  a el valor introducido
            
            **Campos de tipo Caracter: Primero checa que la variable no este nula y despues regresa los trabajos que 
              que contengan esa palabra o letras introducidas

            **Campos de tipo Caracter-Booleanos: Primero checa que la variable no este nula y despues dependiendo si 
              selecciono si o no sera los trabajos que tienen o no ese campo

            */
            //Opciones de busqueda para el modelo principal de job
            if (searchViewModel.NumJobSearch >= 2015000000 && searchViewModel.NumJobSearch <= 2021000000) jobSearchRepo = jobSearchRepo.Where(s => s.JobNum == searchViewModel.NumJobSearch);
            if (searchViewModel.POJobSearch >= 3000000 && searchViewModel.POJobSearch <= 4900000) jobSearchRepo = repository.Jobs.Where(s => s.PO == searchViewModel.POJobSearch);
            if (searchViewModel.EngID > 0) jobSearchRepo = jobSearchRepo.Where(s => s.EngID == searchViewModel.EngID);
            if (searchViewModel.CrossAppEngID > 0) jobSearchRepo = jobSearchRepo.Where(s => s.CrossAppEngID == searchViewModel.CrossAppEngID);
            if (searchViewModel.CityID > 0) jobSearchRepo = jobSearchRepo.Where(s => s.CityID == searchViewModel.CityID);
            if (searchViewModel.FireCodeID > 0) jobSearchRepo = jobSearchRepo.Where(s => s.FireCodeID == searchViewModel.FireCodeID);
            if (searchViewModel.JobTypeID > 0) jobSearchRepo = jobSearchRepo.Where(s => s.JobTypeID == searchViewModel.JobTypeID);

            if (!string.IsNullOrEmpty(searchViewModel.NameJobSearch)) jobSearchRepo = jobSearchRepo.Where(s => s.Name.Contains(searchViewModel.NameJobSearch));
            if (!string.IsNullOrEmpty(searchViewModel.CustJobSearch)) jobSearchRepo = jobSearchRepo.Where(s => s.Cust.Contains(searchViewModel.CustJobSearch));
            if (!string.IsNullOrEmpty(searchViewModel.ContractorJobSearch)) jobSearchRepo = jobSearchRepo.Where(s => s.Contractor.Contains(searchViewModel.ContractorJobSearch));
            if (!string.IsNullOrEmpty(searchViewModel.StatusJobSearch)) jobSearchRepo = jobSearchRepo.Where(s => s.Status.Equals(searchViewModel.StatusJobSearch));

            //Opciones de busqueda para el modelo de jobExtensions.
            if (searchViewModel.InputFrecuency >= 50 && searchViewModel.InputFrecuency <= 61) jobSearchRepo = jobSearchRepo.Where(s => s._jobExtension.InputFrecuency == searchViewModel.InputFrecuency);
            if (searchViewModel.InputPhase >= 1 && searchViewModel.InputPhase <= 3) jobSearchRepo = jobSearchRepo.Where(s => s._jobExtension.InputPhase == searchViewModel.InputPhase);
            if (searchViewModel.InputVoltage >= 114 && searchViewModel.InputVoltage <= 600) jobSearchRepo = jobSearchRepo.Where(s => s._jobExtension.InputVoltage == searchViewModel.InputVoltage);
            if (searchViewModel.NumOfStops >= 1 && searchViewModel.NumOfStops <= 32) jobSearchRepo = jobSearchRepo.Where(s => s._jobExtension.NumOfStops == searchViewModel.NumOfStops);
            if (searchViewModel.DoorOperatorID > 0) jobSearchRepo = jobSearchRepo.Where(s => s._jobExtension.DoorOperatorID == searchViewModel.DoorOperatorID);

            if (!string.IsNullOrEmpty(searchViewModel.JobTypeAdd)) jobSearchRepo = jobSearchRepo.Where(s => s._jobExtension.JobTypeAdd.Equals(searchViewModel.JobTypeAdd));
            if (!string.IsNullOrEmpty(searchViewModel.JobTypeMain)) jobSearchRepo = jobSearchRepo.Where(s => s._jobExtension.JobTypeMain.Equals(searchViewModel.JobTypeMain));

            if (!string.IsNullOrEmpty(searchViewModel.AuxCop)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.AuxCop == "Si" ? s._jobExtension.AUXCOP == true : s._jobExtension.AUXCOP == false);
            if (!string.IsNullOrEmpty(searchViewModel.CartopDoorButtons)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.CartopDoorButtons == "Si" ? s._jobExtension.CartopDoorButtons == true : s._jobExtension.CartopDoorButtons == false);
            if (!string.IsNullOrEmpty(searchViewModel.DoorHold)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.DoorHold == "Si" ? s._jobExtension.DoorHold == true : s._jobExtension.DoorHold == false);
            if (!string.IsNullOrEmpty(searchViewModel.HeavyDoors)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.HeavyDoors == "Si" ? s._jobExtension.HeavyDoors == true : s._jobExtension.HeavyDoors == false);
            if (!string.IsNullOrEmpty(searchViewModel.InfDetector)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.InfDetector == "Si" ? s._jobExtension.InfDetector == true : s._jobExtension.InfDetector == false);
            if (!string.IsNullOrEmpty(searchViewModel.MechSafEdge)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.MechSafEdge == "Si" ? s._jobExtension.MechSafEdge == true : s._jobExtension.MechSafEdge == false);
            if (!string.IsNullOrEmpty(searchViewModel.Scop)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.Scop == "Si" ? s._jobExtension.SCOP == true : s._jobExtension.SCOP == false);
            if (!string.IsNullOrEmpty(searchViewModel.Shc)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.Shc == "Si" ? s._jobExtension.SHC == true : s._jobExtension.SHC == false);

            //Opciones de bsuqueda para el modelo de HydroSpecifics
            if (searchViewModel.FLA > 0) jobSearchRepo = jobSearchRepo.Where(s => s._HydroSpecific.FLA == searchViewModel.FLA);
            if (searchViewModel.HP > 0) jobSearchRepo = jobSearchRepo.Where(s => s._HydroSpecific.HP == searchViewModel.HP);
            if (searchViewModel.MotorsNum > 0) jobSearchRepo = jobSearchRepo.Where(s => s._HydroSpecific.MotorsNum == searchViewModel.MotorsNum);
            if (searchViewModel.SPH > 0) jobSearchRepo = jobSearchRepo.Where(s => s._HydroSpecific.SPH == searchViewModel.SPH);

            if (!string.IsNullOrEmpty(searchViewModel.BatteryBrand)) jobSearchRepo = jobSearchRepo.Where( s => s._HydroSpecific.BatteryBrand.Equals(searchViewModel.BatteryBrand));
            if (!string.IsNullOrEmpty(searchViewModel.Starter)) jobSearchRepo = jobSearchRepo.Where(s => s._HydroSpecific.Starter.Contains(searchViewModel.Starter));
            if (!string.IsNullOrEmpty(searchViewModel.ValveBrand)) jobSearchRepo = jobSearchRepo.Where(s => s._HydroSpecific.ValveBrand.Equals(searchViewModel.ValveBrand));

            if (!string.IsNullOrEmpty(searchViewModel.Battery)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.Battery == "Si" ? s._HydroSpecific.Battery == true : s._HydroSpecific.Battery == false);
            if (!string.IsNullOrEmpty(searchViewModel.LOS)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.LOS == "Si" ? s._HydroSpecific.LOS == true : s._HydroSpecific.LOS == false);
            if (!string.IsNullOrEmpty(searchViewModel.LifeJacket)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.LifeJacket == "Si" ? s._HydroSpecific.LifeJacket == true : s._HydroSpecific.LifeJacket == false);
            if (!string.IsNullOrEmpty(searchViewModel.OilCool)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.OilCool == "Si" ? s._HydroSpecific.OilCool == true : s._HydroSpecific.OilCool == false);
            if (!string.IsNullOrEmpty(searchViewModel.OilTank)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.OilTank == "Si" ? s._HydroSpecific.OilTank == true : s._HydroSpecific.OilTank == false);
            if (!string.IsNullOrEmpty(searchViewModel.PSS)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.PSS == "Si" ? s._HydroSpecific.PSS == true : s._HydroSpecific.PSS == false);
            if (!string.IsNullOrEmpty(searchViewModel.Resync)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.Resync == "Si" ? s._HydroSpecific.Resync == true : s._HydroSpecific.Resync == false);
            if (!string.IsNullOrEmpty(searchViewModel.Roped)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.Roped == "Si" ? s._HydroSpecific.Roped == true : s._HydroSpecific.Roped == false);
            if (!string.IsNullOrEmpty(searchViewModel.VCI)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.VCI == "Si" ? s._HydroSpecific.VCI == true : s._HydroSpecific.VCI == false);

            //Opciones de bsuqueda para el modelo de GenericFeatures
            if (!string.IsNullOrEmpty(searchViewModel.EPCarsNumber)) jobSearchRepo = jobSearchRepo.Where(s => s._GenericFeatures.EPCarsNumber.Equals(searchViewModel.EPCarsNumber));
            if (!string.IsNullOrEmpty(searchViewModel.SwitchStyle)) jobSearchRepo = jobSearchRepo.Where(s => s._GenericFeatures.SwitchStyle.Equals(searchViewModel.SwitchStyle));
            if (!string.IsNullOrEmpty(searchViewModel.BottomAccessLocation)) jobSearchRepo = jobSearchRepo.Where(s => s._GenericFeatures.BottomAccessLocation.Equals(searchViewModel.BottomAccessLocation));
            if (!string.IsNullOrEmpty(searchViewModel.EPContact)) jobSearchRepo = jobSearchRepo.Where(s => s._GenericFeatures.EPContact.Equals(searchViewModel.EPContact));
            if (!string.IsNullOrEmpty(searchViewModel.GovModel)) jobSearchRepo = jobSearchRepo.Where(s => s._GenericFeatures.GovModel.Contains(searchViewModel.GovModel));
            if (!string.IsNullOrEmpty(searchViewModel.INCPButtons)) jobSearchRepo = jobSearchRepo.Where(s => s._GenericFeatures.INCPButtons.Equals(searchViewModel.INCPButtons));
            if (!string.IsNullOrEmpty(searchViewModel.Monitoring)) jobSearchRepo = jobSearchRepo.Where(s => s._GenericFeatures.Monitoring.Equals(searchViewModel.Monitoring));
            if (!string.IsNullOrEmpty(searchViewModel.TopAccessLocation)) jobSearchRepo = jobSearchRepo.Where(s => s._GenericFeatures.TopAccessLocation.Equals(searchViewModel.TopAccessLocation));

            if (!string.IsNullOrEmpty(searchViewModel.Attendant)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.Attendant == "Si" ? s._GenericFeatures.Attendant == true : s._GenericFeatures.Attendant == false);
            if (!string.IsNullOrEmpty(searchViewModel.CallEnable)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.CallEnable == "Si" ? s._GenericFeatures.CallEnable == true : s._GenericFeatures.CallEnable == false);
            if (!string.IsNullOrEmpty(searchViewModel.CarToLobby)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.CarToLobby == "Si" ? s._GenericFeatures.CarToLobby == true : s._GenericFeatures.CarToLobby == false);
            if (!string.IsNullOrEmpty(searchViewModel.EMT)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.EMT == "Si" ? s._GenericFeatures.EMT == true : s._GenericFeatures.EMT == false);
            if (!string.IsNullOrEmpty(searchViewModel.EP)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.EP == "Si" ? s._GenericFeatures.EP == true : s._GenericFeatures.EP == false);
            if (!string.IsNullOrEmpty(searchViewModel.EQ)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.EQ == "Si" ? s._GenericFeatures.EQ == true : s._GenericFeatures.EQ == false);
            if (!string.IsNullOrEmpty(searchViewModel.FLO)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.FLO == "Si" ? s._GenericFeatures.FLO == true : s._GenericFeatures.FLO == false);
            if (!string.IsNullOrEmpty(searchViewModel.FRON2)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.FRON2 == "Si" ? s._GenericFeatures.FRON2 == true : s._GenericFeatures.FRON2 == false);
            if (!string.IsNullOrEmpty(searchViewModel.Hosp)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.Hosp == "Si" ? s._GenericFeatures.Hosp == true : s._GenericFeatures.Hosp == false);
            if (!string.IsNullOrEmpty(searchViewModel.EPVoltage)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.EPVoltage == "Si" ? s._GenericFeatures.EPVoltage == true : s._GenericFeatures.EPVoltage == false);
            if (!string.IsNullOrEmpty(searchViewModel.INA)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.INA == "Si" ? s._GenericFeatures.INA == true : s._GenericFeatures.INA == false);
            if (!string.IsNullOrEmpty(searchViewModel.INCP)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.INCP == "Si" ? s._GenericFeatures.INCP == true : s._GenericFeatures.INCP == false);
            if (!string.IsNullOrEmpty(searchViewModel.Ind)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.Ind == "Si" ? s._GenericFeatures.Ind == true : s._GenericFeatures.Ind == false);
            if (!string.IsNullOrEmpty(searchViewModel.LoadWeigher)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.LoadWeigher == "Si" ? s._GenericFeatures.LoadWeigher == true : s._GenericFeatures.LoadWeigher == false);
            if (!string.IsNullOrEmpty(searchViewModel.TopAccess)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.TopAccess == "Si" ? s._GenericFeatures.TopAccess == true : s._GenericFeatures.TopAccess == false);
            if (!string.IsNullOrEmpty(searchViewModel.CRO)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.CRO == "Si" ? s._GenericFeatures.CRO == true : s._GenericFeatures.CRO == false);
            if (!string.IsNullOrEmpty(searchViewModel.CarCallRead)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.CarCallRead == "Si" ? s._GenericFeatures.CarCallRead == true : s._GenericFeatures.CarCallRead == false);
            if (!string.IsNullOrEmpty(searchViewModel.CarKey)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.CarKey == "Si" ? s._GenericFeatures.CarKey == true : s._GenericFeatures.CarKey == false);
            if (!string.IsNullOrEmpty(searchViewModel.HCRO)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.HCRO == "Si" ? s._GenericFeatures.HCRO == true : s._GenericFeatures.HCRO == false);
            if (!string.IsNullOrEmpty(searchViewModel.HallCallRead)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.HallCallRead == "Si" ? s._GenericFeatures.HallCallRead == true : s._GenericFeatures.HallCallRead == false);
            if (!string.IsNullOrEmpty(searchViewModel.HallKey)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.HallKey == "Si" ? s._GenericFeatures.HallKey == true : s._GenericFeatures.HallKey == false);
            if (!string.IsNullOrEmpty(searchViewModel.BottomAccess)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.BottomAccess == "Si" ? s._GenericFeatures.BottomAccess == true : s._GenericFeatures.BottomAccess == false);
            if (!string.IsNullOrEmpty(searchViewModel.CTINSPST)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.CTINSPST == "Si" ? s._GenericFeatures.CTINSPST == true : s._GenericFeatures.CTINSPST == false);
            if (!string.IsNullOrEmpty(searchViewModel.EPSelect)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.EPSelect == "Si" ? s._GenericFeatures.EPSelect == true : s._GenericFeatures.EPSelect == false);
            if (!string.IsNullOrEmpty(searchViewModel.PTI)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.PTI == "Si" ? s._GenericFeatures.PTI == true : s._GenericFeatures.PTI == false);

            //Opciones de bsuqueda para el modelo de HydroSpecifics
            if (searchViewModel.IndicatorsVoltage > 0) jobSearchRepo = jobSearchRepo.Where(s => s._Indicator.IndicatorsVoltage == searchViewModel.IndicatorsVoltage);

            if (!string.IsNullOrEmpty(searchViewModel.CarCallsVoltage)) jobSearchRepo = jobSearchRepo.Where(s => s._Indicator.CarCallsVoltage.Equals(searchViewModel.CarCallsVoltage));
            if (!string.IsNullOrEmpty(searchViewModel.HallCallsType)) jobSearchRepo = jobSearchRepo.Where(s => s._Indicator.HallCallsType.Equals(searchViewModel.HallCallsType));
            if (!string.IsNullOrEmpty(searchViewModel.CarCallsVoltageType)) jobSearchRepo = jobSearchRepo.Where(s => s._Indicator.CarCallsVoltageType.Equals(searchViewModel.CarCallsVoltageType));
            if (!string.IsNullOrEmpty(searchViewModel.HallCallsVoltage)) jobSearchRepo = jobSearchRepo.Where(s => s._Indicator.HallCallsVoltage.Equals(searchViewModel.HallCallsVoltage));
            if (!string.IsNullOrEmpty(searchViewModel.HallCallsVoltageType)) jobSearchRepo = jobSearchRepo.Where(s => s._Indicator.HallCallsVoltageType.Equals(searchViewModel.HallCallsVoltageType));
            if (!string.IsNullOrEmpty(searchViewModel.IndicatorsVoltageType)) jobSearchRepo = jobSearchRepo.Where(s => s._Indicator.IndicatorsVoltageType.Equals(searchViewModel.IndicatorsVoltageType));

            if (!string.IsNullOrEmpty(searchViewModel.CarLanterns)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.CarLanterns == "Si" ? s._Indicator.CarLanterns == true : s._Indicator.CarLanterns == false);
            if (!string.IsNullOrEmpty(searchViewModel.HallLanterns)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.HallLanterns == "Si" ? s._Indicator.HallLanterns == true : s._Indicator.HallLanterns == false);
            if (!string.IsNullOrEmpty(searchViewModel.HallPI)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.HallPI == "Si" ? s._Indicator.HallPI == true : s._Indicator.HallPI == false);
            if (!string.IsNullOrEmpty(searchViewModel.PassingFloor)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.PassingFloor == "Si" ? s._Indicator.PassingFloor == true : s._Indicator.PassingFloor == false);
            if (!string.IsNullOrEmpty(searchViewModel.VoiceAnnunciationPI)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.VoiceAnnunciationPI == "Si" ? s._Indicator.VoiceAnnunciationPI == true : s._Indicator.VoiceAnnunciationPI == false);

            //Opciones de bsuqueda para el modelo de HoistWayData
            if (searchViewModel.Capacity > 0) jobSearchRepo = jobSearchRepo.Where(s => s._HoistWayData.Capacity == searchViewModel.Capacity);
            if (searchViewModel.DownSpeed > 0) jobSearchRepo = jobSearchRepo.Where(s => s._HoistWayData.DownSpeed == searchViewModel.DownSpeed);
            if (searchViewModel.UpSpeed > 0) jobSearchRepo = jobSearchRepo.Where(s => s._HoistWayData.UpSpeed == searchViewModel.UpSpeed);
            if (searchViewModel.HoistWaysNumber > 0) jobSearchRepo = jobSearchRepo.Where(s => s._HoistWayData.HoistWaysNumber == searchViewModel.HoistWaysNumber);
            if (searchViewModel.MachineRooms > 0) jobSearchRepo = jobSearchRepo.Where(s => s._HoistWayData.MachineRooms == searchViewModel.MachineRooms);
            if (searchViewModel.LandingSystemID > 0) jobSearchRepo = jobSearchRepo.Where(s => s._HoistWayData.LandingSystemID == searchViewModel.LandingSystemID);

            //Opciones de bsuqueda para el modelo de Special Features
            if (!string.IsNullOrEmpty(searchViewModel.Description))
            {
                    jobSearchRepo = jobSearchRepo.Where(a => a._SpecialFeatureslist.Any(b => b.Description.Equals(searchViewModel.Description)));
            }

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
