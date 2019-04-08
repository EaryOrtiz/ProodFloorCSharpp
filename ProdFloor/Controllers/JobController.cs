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
using System.IO;
using System.Xml;
using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;

namespace ProdFloor.Controllers
{
    [Authorize(Roles = "Admin,Engineer")]
    public class JobController : Controller
    {
        private IJobRepository repository;
        private IItemRepository itemsrepository;
        private UserManager<AppUser> userManager;
        public int PageSize = 5;

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
            return View(new Job());
        }

        [HttpPost]
        public IActionResult NewJob(Job newJob)
        {
            //Desactivar esta funcion para que funcione el test de Job
            AppUser currentUser = GetCurrentUser().Result;
            if (ModelState.IsValid)
            {
                //y esta esta tambien y poner denuevo el {currenuser.engId} en Los TempDatas cuando terminen los test 
                newJob.EngID = currentUser.EngID;
                newJob.CrossAppEngID = 117;
                newJob.Status = "Incomplete";
                repository.SaveJob(newJob);
                JobViewModel newJobViewModel = new JobViewModel
                {
                    CurrentUserID = currentUser.EngID,
                    CurrentJob = newJob,
                    CurrentJobExtension = new JobExtension { JobID = newJob.JobID },
                    CurrentHydroSpecific = new HydroSpecific(),
                    CurrentGenericFeatures = new GenericFeatures(),
                    CurrentIndicator = new Indicator(),
                    CurrentHoistWayData = new HoistWayData(),
                    SpecialFeatureslist = new List<SpecialFeatures> { new SpecialFeatures() },
                    CurrentTab = "Extension"
                };
                TempData["message"] = $"Job# {newJobViewModel.CurrentJob.JobNum} has been saved...{newJobViewModel.CurrentJob.JobID}...";
                return View("NextForm", newJobViewModel);


            }
            else
            {
                TempData["message"] = $"There seems to be errors in the form. Please validate....";
                TempData["alert"] = $"alert-danger";
                return View(newJob);
            }
        }

        /* Get de Edit; Si el ID recibido exite en el repositorio regresa un JobViewModel con los objetos relacionados a este ID,
         * de lo contrario regresa un mensaje de failure a la view List
         */
        public IActionResult Edit(int ID)
        {

            AppUser currentUser = GetCurrentUser().Result;
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
                if (SfList != null) viewModel.SpecialFeatureslist = SfList;
                else viewModel.SpecialFeatureslist = new List<SpecialFeatures> { new SpecialFeatures() };
                viewModel.CurrentUserID = currentUser.EngID;
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
            AppUser currentUser = GetCurrentUser().Result;
            multiEditViewModel.CurrentUserID = currentUser.EngID;
            if (ModelState.IsValid)
            {

                if (multiEditViewModel.CurrentJob.Status == "" || multiEditViewModel.CurrentJob.Status == null)
                {
                    multiEditViewModel.CurrentJob.Status = "Working on it";
                }
                if (multiEditViewModel.CurrentHydroSpecific.BatteryBrand == "Other" && !string.IsNullOrEmpty(multiEditViewModel.CurrentHydroSpecific.OtherBatteryBrand))
                {
                    multiEditViewModel.CurrentHydroSpecific.BatteryBrand = multiEditViewModel.CurrentHydroSpecific.OtherBatteryBrand;
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
            if (repository.Jobs.FirstOrDefault(j => j.JobID == ID) != null)
            {
                List<SpecialFeatures> SfList = repository.SpecialFeatures.Where(j => j.JobID == ID).ToList();
                JobViewModel continueJobViewModel = new JobViewModel();
                AppUser currentUser = GetCurrentUser().Result;
                continueJobViewModel.CurrentUserID = currentUser.EngID;
                continueJobViewModel.CurrentTab = "Main";
                continueJobViewModel.CurrentJob = repository.Jobs.FirstOrDefault(j => j.JobID == ID);
                continueJobViewModel.CurrentJobExtension = (repository.JobsExtensions.FirstOrDefault(j => j.JobID == ID) ?? new JobExtension());
                continueJobViewModel.CurrentHydroSpecific = (repository.HydroSpecifics.FirstOrDefault(j => j.JobID == ID) ?? new HydroSpecific());
                continueJobViewModel.CurrentGenericFeatures = (repository.GenericFeaturesList.FirstOrDefault(j => j.JobID == ID) ?? new GenericFeatures());
                continueJobViewModel.CurrentIndicator = (repository.Indicators.FirstOrDefault(j => j.JobID == ID) ?? new Indicator());
                continueJobViewModel.CurrentHoistWayData = (repository.HoistWayDatas.FirstOrDefault(j => j.JobID == ID) ?? new HoistWayData());
                if (SfList.Count > 1) continueJobViewModel.SpecialFeatureslist = SfList;
                else continueJobViewModel.SpecialFeatureslist = new List<SpecialFeatures> { new SpecialFeatures() };

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
            AppUser currentUser = GetCurrentUser().Result;
            nextViewModel.CurrentUserID = currentUser.EngID;

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
                                            nextViewModel.SpecialFeatureslist = new List<SpecialFeatures> { new SpecialFeatures { JobID = nextViewModel.CurrentJob.JobID } };
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
                                    if (nextViewModel.CurrentJobExtension.SCOP == true)
                                    {
                                        nextViewModel.CurrentIndicator = new Indicator
                                        {
                                            JobID = nextViewModel.CurrentJob.JobID,
                                            CarCallsVoltage = 24.ToString(),
                                            CarCallsVoltageType = "DC",
                                            CarCallsType = "LED",
                                            HallCallsVoltage = 24.ToString(),
                                            HallCallsVoltageType = "DC",
                                            HallCallsType = "LED",
                                            IndicatorsVoltage = 24,
                                            IndicatorsVoltageType = "DC"
                                        };
                                    }
                                    else
                                    {
                                        nextViewModel.CurrentIndicator = new Indicator { JobID = nextViewModel.CurrentJob.JobID };
                                    }
                                    nextViewModel.CurrentHoistWayData = new HoistWayData();
                                    nextViewModel.SpecialFeatureslist = new List<SpecialFeatures> { new SpecialFeatures() };
                                    nextViewModel.CurrentTab = "Indicator";
                                    TempData["message"] = $"generic was saved";
                                    return View(nextViewModel);
                                }
                            }
                            else
                            {
                                if (nextViewModel.CurrentHydroSpecific.BatteryBrand == "Other" && !string.IsNullOrEmpty(nextViewModel.CurrentHydroSpecific.OtherBatteryBrand))
                                {
                                    nextViewModel.CurrentHydroSpecific.BatteryBrand = nextViewModel.CurrentHydroSpecific.OtherBatteryBrand;
                                }
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

                        try
                        {
                            repository.SaveEngJobView(nextViewModel);
                        }
                        catch (DbUpdateException e)
                        {
                            TempData["message"] = $"The Job PO #{nextViewModel.CurrentJob.PO} already exists or some fields are empty!";
                            TempData["alert"] = $"alert-danger";
                            return View(nextViewModel);
                        }
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

        public async Task<IActionResult> JobSearchList(JobSearchViewModel searchViewModel, int jobPage = 1)
        {
            if (searchViewModel.CleanFields)
            {
                var jobSearchNew = repository.Jobs.ToList();
                JobSearchViewModel search = new JobSearchViewModel
                {
                    JobsSearchList = jobSearchNew,
                    PagingInfo = new PagingInfo
                    {
                        CurrentPage = jobPage,
                        ItemsPerPage = 10,
                        TotalItems = jobSearchNew.Count()
                    }
                };

                return RedirectToAction("JobSearchList", search);
            }

            var JobCount = repository.Jobs.Count();
            var jobSearchRepo = repository.Jobs.Include(j => j._jobExtension).Include(hy => hy._HydroSpecific).Include(g => g._GenericFeatures)
                .Include(i => i._Indicator).Include(ho => ho._HoistWayData).Include(sp => sp._SpecialFeatureslist).AsQueryable();
            IQueryable<string> statusQuery = from s in repository.Jobs orderby s.Status select s.Status;
            #region comments
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
            #endregion

            #region JobModelSearch
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
            #endregion

            #region JobExtension
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
            #endregion

            #region HydroSpecifics
            //Opciones de bsuqueda para el modelo de HydroSpecifics
            if (searchViewModel.FLA > 0) jobSearchRepo = jobSearchRepo.Where(s => s._HydroSpecific.FLA == searchViewModel.FLA);
            if (searchViewModel.HP > 0) jobSearchRepo = jobSearchRepo.Where(s => s._HydroSpecific.HP == searchViewModel.HP);
            if (searchViewModel.MotorsNum > 0) jobSearchRepo = jobSearchRepo.Where(s => s._HydroSpecific.MotorsNum == searchViewModel.MotorsNum);
            if (searchViewModel.SPH > 0) jobSearchRepo = jobSearchRepo.Where(s => s._HydroSpecific.SPH == searchViewModel.SPH);

            if (!string.IsNullOrEmpty(searchViewModel.BatteryBrand)) jobSearchRepo = jobSearchRepo.Where(s => s._HydroSpecific.BatteryBrand.Equals(searchViewModel.BatteryBrand));
            if (!string.IsNullOrEmpty(searchViewModel.Starter)) jobSearchRepo = jobSearchRepo.Where(s => s._HydroSpecific.Starter.Contains(searchViewModel.Starter));
            if (!string.IsNullOrEmpty(searchViewModel.ValveBrand)) jobSearchRepo = jobSearchRepo.Where(s => s._HydroSpecific.ValveBrand.Equals(searchViewModel.ValveBrand));

            if (!string.IsNullOrEmpty(searchViewModel.Battery)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.Battery == "Si" ? s._HydroSpecific.Battery == true : s._HydroSpecific.Battery == false);
            if (!string.IsNullOrEmpty(searchViewModel.LOS)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.LOS == "Si" ? s._HydroSpecific.LOS == true : s._HydroSpecific.LOS == false);
            if (!string.IsNullOrEmpty(searchViewModel.LifeJacket)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.LifeJacket == "Si" ? s._HydroSpecific.LifeJacket == true : s._HydroSpecific.LifeJacket == false);
            if (!string.IsNullOrEmpty(searchViewModel.OilCool)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.OilCool == "Si" ? s._HydroSpecific.OilCool == true : s._HydroSpecific.OilCool == false);
            if (!string.IsNullOrEmpty(searchViewModel.OilTank)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.OilTank == "Si" ? s._HydroSpecific.OilTank == true : s._HydroSpecific.OilTank == false);
            if (!string.IsNullOrEmpty(searchViewModel.PSS)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.PSS == "Si" ? s._HydroSpecific.PSS == true : s._HydroSpecific.PSS == false);
            if (!string.IsNullOrEmpty(searchViewModel.Resync)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.Resync == "Si" ? s._HydroSpecific.Resync == true : s._HydroSpecific.Resync == false);
            if (!string.IsNullOrEmpty(searchViewModel.VCI)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.VCI == "Si" ? s._HydroSpecific.VCI == true : s._HydroSpecific.VCI == false);
            #endregion

            #region GenericFeatures
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
            #endregion

            #region HydroSpecifics
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
            #endregion

            #region HoistWayData
            //Opciones de bsuqueda para el modelo de HoistWayData
            if (!string.IsNullOrEmpty(searchViewModel.AnyRear)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.AnyRear == "Si" ? s._HoistWayData.AnyRear == true : s._HoistWayData.AnyRear == false);

            if (!string.IsNullOrEmpty(searchViewModel.IndependentRearOpenings)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.IndependentRearOpenings == "Si" ?
            s._HoistWayData.IndependentRearOpenings == true && s._HoistWayData.AnyRear == true : s._HoistWayData.IndependentRearOpenings == false && s._HoistWayData.AnyRear == true);

            if (searchViewModel.RearFloorOpenings > 0) jobSearchRepo = jobSearchRepo.Where(s => s._HoistWayData.RearFloorOpenings == searchViewModel.RearFloorOpenings);
            if (searchViewModel.TopFloor > 0) jobSearchRepo = jobSearchRepo.Where(s => s._HoistWayData.TopFloor == searchViewModel.TopFloor);
            if (searchViewModel.FrontFloorOpenings > 0) jobSearchRepo = jobSearchRepo.Where(s => s._HoistWayData.FrontFloorOpenings == searchViewModel.FrontFloorOpenings);

            if (searchViewModel.Capacity > 0) jobSearchRepo = jobSearchRepo.Where(s => s._HoistWayData.Capacity == searchViewModel.Capacity);
            if (searchViewModel.DownSpeed > 0) jobSearchRepo = jobSearchRepo.Where(s => s._HoistWayData.DownSpeed == searchViewModel.DownSpeed);
            if (searchViewModel.UpSpeed > 0) jobSearchRepo = jobSearchRepo.Where(s => s._HoistWayData.UpSpeed == searchViewModel.UpSpeed);
            if (searchViewModel.HoistWaysNumber > 0) jobSearchRepo = jobSearchRepo.Where(s => s._HoistWayData.HoistWaysNumber == searchViewModel.HoistWaysNumber);
            if (searchViewModel.MachineRooms > 0) jobSearchRepo = jobSearchRepo.Where(s => s._HoistWayData.MachineRooms == searchViewModel.MachineRooms);
            if (searchViewModel.LandingSystemID > 0) jobSearchRepo = jobSearchRepo.Where(s => s._HoistWayData.LandingSystemID == searchViewModel.LandingSystemID);
            #endregion

            #region SpecialFeatures
            //Opciones de bsuqueda para el modelo de Special Features
            if (!string.IsNullOrEmpty(searchViewModel.Description))
            {
                jobSearchRepo = jobSearchRepo.Where(a => a._SpecialFeatureslist.Any(b => b.Description.Equals(searchViewModel.Description)));
            }
            #endregion

            int TotalItemsSearch = jobSearchRepo.Count() + 1;
            JobSearchViewModel jobSearch = new JobSearchViewModel
            {
                Status = new SelectList(statusQuery.Distinct().ToList()),
                JobsSearchList = jobSearchRepo.OrderBy(p => p.JobID).Skip((jobPage - 1) * 10).Take(TotalItemsSearch).ToList(),
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
            BrandList = itemsrepository.DoorOperators.FromSql("select * from dbo.DoorOperators where Style = {0} AND dbo.DoorOperators.DoorOperatorID in " +
                "(Select max(dbo.DoorOperators.DoorOperatorID) FROM dbo.DoorOperators group by dbo.DoorOperators.Brand)", Style).ToList();
            return Json(new SelectList(BrandList, "Brand", "Brand"));
        }

        public JsonResult GetDoorOperatorID(string Brand)
        {
            List<DoorOperator> DoorOperatorList = new List<DoorOperator>();
            DoorOperatorList = (from door in itemsrepository.DoorOperators where door.Brand == Brand select door).ToList();
            return Json(new SelectList(DoorOperatorList, "DoorOperatorID", "Name"));
        }

        [HttpPost]
        public FileStreamResult ExportJobToXML(string buttonXML)
        {
            MemoryStream ms = new MemoryStream();
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.OmitXmlDeclaration = true;
            xws.Indent = true;

            List<Job> jobs = repository.Jobs.ToList();

            using (XmlWriter xw = XmlWriter.Create(ms, xws))
            {
                xw.WriteStartDocument();
                xw.WriteStartElement("Jobs");
                if (repository.Jobs.Any())
                {
                    foreach (Job job in jobs)
                    {
                        if (job.Status != "Incomplete")
                        {
                            xw.WriteStartElement("Job");

                            xw.WriteStartElement("JobBase");
                            xw.WriteElementString("ID", job.JobID.ToString());
                            if (!string.IsNullOrEmpty(job.Status)) xw.WriteElementString("Status", job.Status);
                            xw.WriteElementString("EngID", job.EngID.ToString());
                            xw.WriteElementString("CrossAppEngID", job.CrossAppEngID.ToString());
                            xw.WriteElementString("Name", job.Name);
                            xw.WriteElementString("JobNum", job.JobNum.ToString());
                            xw.WriteElementString("PO", job.PO.ToString());
                            xw.WriteElementString("ShipDate", job.ShipDate.ToString());
                            xw.WriteElementString("LatestFinishDate", job.LatestFinishDate.ToString());
                            xw.WriteElementString("Cust", job.Cust);
                            xw.WriteElementString("Contractor", job.Contractor);
                            xw.WriteElementString("JobTypeID", job.JobTypeID.ToString());
                            xw.WriteElementString("CityID", job.CityID.ToString());
                            xw.WriteElementString("FireCodeID", job.FireCodeID.ToString());
                            xw.WriteEndElement();

                            JobExtension jobExtension = repository.JobsExtensions.First(m => m.JobID == job.JobID);
                            if (jobExtension != null)
                            {
                                string aux;
                                xw.WriteStartElement("JobExtension");
                                xw.WriteElementString("ID", jobExtension.JobExtensionID.ToString());
                                xw.WriteElementString("JobID", jobExtension.JobID.ToString());
                                xw.WriteElementString("NumOfStops", jobExtension.NumOfStops.ToString());
                                xw.WriteElementString("JobTypeMain", jobExtension.JobTypeMain);
                                xw.WriteElementString("JobTypeAdd", jobExtension.JobTypeAdd);
                                xw.WriteElementString("InputVoltage", jobExtension.InputVoltage.ToString());
                                xw.WriteElementString("InputPhase", jobExtension.InputPhase.ToString());
                                xw.WriteElementString("InputFrecuency", jobExtension.InputFrecuency.ToString());
                                xw.WriteElementString("DoorGate", jobExtension.DoorGate);
                                xw.WriteElementString("DoorHoist", jobExtension.DoorHoist);
                                xw.WriteElementString("SHCRisers", jobExtension.SHCRisers.ToString());
                                xw.WriteElementString("DoorOperatorID", jobExtension.DoorOperatorID.ToString());
                                aux = jobExtension.InfDetector ? "1" : "0";
                                xw.WriteElementString("InfDetector", aux);
                                aux = jobExtension.MechSafEdge ? "1" : "0";
                                xw.WriteElementString("MechSafEdge", aux);
                                aux = jobExtension.HeavyDoors ? "1" : "0";
                                xw.WriteElementString("HeavyDoors", aux);
                                aux = jobExtension.CartopDoorButtons ? "1" : "0";
                                xw.WriteElementString("CartopDoorButtons", aux);
                                aux = jobExtension.DoorHold ? "1" : "0";
                                xw.WriteElementString("DoorHold", aux);
                                aux = jobExtension.Nudging ? "1" : "0";
                                xw.WriteElementString("Nudging", aux);
                                aux = jobExtension.SCOP ? "1" : "0";
                                xw.WriteElementString("SCOP", aux);
                                aux = jobExtension.SHC ? "1" : "0";
                                xw.WriteElementString("SHC", aux);
                                aux = jobExtension.AUXCOP ? "1" : "0";
                                xw.WriteElementString("AUXCOP", aux);
                                xw.WriteEndElement();
                            }
                            HydroSpecific hydro = repository.HydroSpecifics.First(m => m.JobID == job.JobID);
                            if (hydro != null)
                            {
                                string aux;
                                xw.WriteStartElement("HydroSpecific");
                                xw.WriteElementString("ID", hydro.HydroSpecificID.ToString());
                                xw.WriteElementString("JobID", hydro.JobID.ToString());
                                xw.WriteElementString("Starter", hydro.Starter);
                                xw.WriteElementString("HP", hydro.HP.ToString());
                                xw.WriteElementString("FLA", hydro.FLA.ToString());
                                xw.WriteElementString("SPH", hydro.SPH.ToString());
                                xw.WriteElementString("MotorsNum", hydro.MotorsNum.ToString());
                                xw.WriteElementString("MotorsDisconnect", hydro.MotorsDisconnect.ToString());
                                xw.WriteElementString("ValveBrand", hydro.ValveBrand);
                                xw.WriteElementString("ValveCoils", hydro.ValveCoils.ToString());
                                xw.WriteElementString("ValveNum", hydro.ValveNum.ToString());
                                xw.WriteElementString("ValveVoltage", hydro.ValveVoltage.ToString());
                                if (!string.IsNullOrEmpty(hydro.BatteryBrand)) xw.WriteElementString("BatteryBrand", hydro.BatteryBrand);
                                aux = hydro.Battery ? "1" : "0";
                                xw.WriteElementString("Battery", aux);
                                aux = hydro.LifeJacket ? "1" : "0";
                                xw.WriteElementString("LifeJacket", aux);
                                aux = hydro.LOS ? "1" : "0";
                                xw.WriteElementString("LOS", aux);
                                aux = hydro.OilCool ? "1" : "0";
                                xw.WriteElementString("OilCool", aux);
                                aux = hydro.OilTank ? "1" : "0";
                                xw.WriteElementString("OilTank", aux);
                                aux = hydro.PSS ? "1" : "0";
                                xw.WriteElementString("PSS", aux);
                                aux = hydro.Resync ? "1" : "0";
                                xw.WriteElementString("Resync", aux);
                                aux = hydro.VCI ? "1" : "0";
                                xw.WriteElementString("VCI", aux);
                                xw.WriteEndElement();
                            }
                            GenericFeatures generic = repository.GenericFeaturesList.First(m => m.JobID == job.JobID);
                            if (generic != null)
                            {
                                string aux;
                                xw.WriteStartElement("GenericFeatures");
                                xw.WriteElementString("ID", generic.GenericFeaturesID.ToString());
                                xw.WriteElementString("JobID", generic.JobID.ToString());
                                if (!string.IsNullOrEmpty(generic.EPCarsNumber)) xw.WriteElementString("EPCarsNumber", generic.EPCarsNumber);
                                if (!string.IsNullOrEmpty(generic.EPContact)) xw.WriteElementString("EPContact", generic.EPContact);
                                if (!string.IsNullOrEmpty(generic.TopAccessLocation)) xw.WriteElementString("TopAccessLocation", generic.TopAccessLocation);
                                if (!string.IsNullOrEmpty(generic.BottomAccessLocation)) xw.WriteElementString("BottomAccessLocation", generic.BottomAccessLocation);
                                if (!string.IsNullOrEmpty(generic.INCPButtons)) xw.WriteElementString("INCPButtons", generic.INCPButtons);
                                if (!string.IsNullOrEmpty(generic.SwitchStyle)) xw.WriteElementString("SwitchStyle", generic.SwitchStyle);
                                if (!string.IsNullOrEmpty(generic.GovModel)) xw.WriteElementString("GovModel", generic.GovModel);
                                if (!string.IsNullOrEmpty(generic.Monitoring)) xw.WriteElementString("Monitoring", generic.Monitoring);
                                if (!string.IsNullOrEmpty(generic.CarCallCodeSecurity)) xw.WriteElementString("CarCallCodeSecurity", generic.CarCallCodeSecurity);
                                if (!string.IsNullOrEmpty(generic.SpecialInstructions)) xw.WriteElementString("SpecialInstructions", generic.SpecialInstructions);
                                aux = generic.FRON2 ? "1" : "0";
                                xw.WriteElementString("FRON2", aux);
                                aux = generic.Attendant ? "1" : "0";
                                xw.WriteElementString("Attendant", aux);
                                aux = generic.CarToLobby ? "1" : "0";
                                xw.WriteElementString("CarToLobby", aux);
                                aux = generic.EQ ? "1" : "0";
                                xw.WriteElementString("EQ", aux);
                                aux = generic.EMT ? "1" : "0";
                                xw.WriteElementString("EMT", aux);
                                aux = generic.EP ? "1" : "0";
                                xw.WriteElementString("EP", aux);
                                aux = generic.EPVoltage ? "1" : "0";
                                xw.WriteElementString("EPVoltage", aux);
                                aux = generic.EPOtherCars ? "1" : "0";
                                xw.WriteElementString("EPOtherCars", aux);
                                aux = generic.PTI ? "1" : "0";
                                xw.WriteElementString("PTI", aux);
                                aux = generic.EPSelect ? "1" : "0";
                                xw.WriteElementString("EPSelect", aux);
                                aux = generic.FLO ? "1" : "0";
                                xw.WriteElementString("FLO", aux);
                                aux = generic.Hosp ? "1" : "0";
                                xw.WriteElementString("Hosp", aux);
                                aux = generic.Ind ? "1" : "0";
                                xw.WriteElementString("Ind", aux);
                                aux = generic.INA ? "1" : "0";
                                xw.WriteElementString("INA", aux);
                                aux = generic.TopAccess ? "1" : "0";
                                xw.WriteElementString("TopAccess", aux);
                                aux = generic.BottomAccess ? "1" : "0";
                                xw.WriteElementString("BottomAccess", aux);
                                aux = generic.INCP ? "1" : "0";
                                xw.WriteElementString("INCP", aux);
                                aux = generic.LoadWeigher ? "1" : "0";
                                xw.WriteElementString("LoadWeigher", aux);
                                aux = generic.CTINSPST ? "1" : "0";
                                xw.WriteElementString("CTINSPST", aux);
                                aux = generic.Roped ? "1" : "0";
                                xw.WriteElementString("Roped", aux);
                                aux = generic.CallEnable ? "1" : "0";
                                xw.WriteElementString("CallEnable", aux);
                                aux = generic.CarCallRead ? "1" : "0";
                                xw.WriteElementString("CarCallRead", aux);
                                aux = generic.HallCallRead ? "1" : "0";
                                xw.WriteElementString("HallCallRead", aux);
                                aux = generic.CarKey ? "1" : "0";
                                xw.WriteElementString("CarKey", aux);
                                aux = generic.HallKey ? "1" : "0";
                                xw.WriteElementString("HallKey", aux);
                                aux = generic.CRO ? "1" : "0";
                                xw.WriteElementString("CRO", aux);
                                aux = generic.HCRO ? "1" : "0";
                                xw.WriteElementString("HCRO", aux);
                                xw.WriteEndElement();
                            }
                            Indicator indicator = repository.Indicators.FirstOrDefault(m => m.JobID == job.JobID);
                            if (indicator != null)
                            {
                                string aux;
                                xw.WriteStartElement("Indicator");
                                xw.WriteElementString("ID", indicator.IndicatorID.ToString());
                                xw.WriteElementString("JobID", indicator.JobID.ToString());
                                xw.WriteElementString("CarCallsVoltage", indicator.CarCallsVoltage);
                                xw.WriteElementString("CarCallsVoltageType", indicator.CarCallsVoltageType);
                                xw.WriteElementString("CarCallsType", indicator.CarCallsType);
                                xw.WriteElementString("HallCallsVoltage", indicator.HallCallsVoltage);
                                xw.WriteElementString("HallCallsVoltageType", indicator.HallCallsVoltageType);
                                xw.WriteElementString("HallCallsType", indicator.HallCallsType);
                                if (!string.IsNullOrEmpty(indicator.CarPIType)) xw.WriteElementString("CarPIType", indicator.CarPIType);
                                if (!string.IsNullOrEmpty(indicator.CarPIDiscreteType)) xw.WriteElementString("CarPIDiscreteType", indicator.CarPIDiscreteType);
                                if (!string.IsNullOrEmpty(indicator.HallPIType)) xw.WriteElementString("HallPIType", indicator.HallPIType);
                                if (!string.IsNullOrEmpty(indicator.HallPIDiscreteType)) xw.WriteElementString("HallPIDiscreteType", indicator.HallPIDiscreteType);
                                if (!string.IsNullOrEmpty(indicator.VoiceAnnunciationPIType)) xw.WriteElementString("VoiceAnnunciationPIType", indicator.VoiceAnnunciationPIType);
                                if (!string.IsNullOrEmpty(indicator.CarLanternsStyle)) xw.WriteElementString("CarLanternsStyle", indicator.CarLanternsStyle);
                                if (!string.IsNullOrEmpty(indicator.CarLanternsType)) xw.WriteElementString("CarLanternsType", indicator.CarLanternsType);
                                if (!string.IsNullOrEmpty(indicator.HallLanternsStyle)) xw.WriteElementString("HallLanternsStyle", indicator.HallLanternsStyle);
                                if (!string.IsNullOrEmpty(indicator.HallLanternsType)) xw.WriteElementString("HallLanternsType", indicator.HallLanternsType);
                                if (!string.IsNullOrEmpty(indicator.PassingFloorType)) xw.WriteElementString("PassingFloorType", indicator.PassingFloorType);
                                if (!string.IsNullOrEmpty(indicator.PassingFloorDiscreteType)) xw.WriteElementString("PassingFloorDiscreteType", indicator.PassingFloorDiscreteType);
                                xw.WriteElementString("IndicatorsVoltage", indicator.IndicatorsVoltage.ToString());
                                xw.WriteElementString("IndicatorsVoltageType", indicator.IndicatorsVoltageType);
                                aux = indicator.CarPI ? "1" : "0";
                                xw.WriteElementString("CarPI", aux);
                                aux = indicator.HallPI ? "1" : "0";
                                xw.WriteElementString("HallPI", aux);
                                aux = indicator.VoiceAnnunciationPI ? "1" : "0";
                                xw.WriteElementString("VoiceAnnunciationPI", aux);
                                aux = indicator.CarLanterns ? "1" : "0";
                                xw.WriteElementString("CarLanterns", aux);
                                aux = indicator.HallLanterns ? "1" : "0";
                                xw.WriteElementString("HallLanterns", aux);
                                aux = indicator.PassingFloor ? "1" : "0";
                                xw.WriteElementString("PassingFloor", aux);
                                aux = indicator.PassingFloorEnable ? "1" : "0";
                                xw.WriteElementString("PassingFloorEnable", aux);
                                xw.WriteEndElement();
                            }
                            HoistWayData hoist = repository.HoistWayDatas.FirstOrDefault(m => m.JobID == job.JobID);
                            if (hoist != null)
                            {
                                string aux;
                                xw.WriteStartElement("HoistWayData");
                                xw.WriteElementString("ID", hoist.HoistWayDataID.ToString());
                                xw.WriteElementString("JobID", hoist.JobID.ToString());
                                xw.WriteElementString("HoistWaysNumber", hoist.HoistWaysNumber.ToString());
                                xw.WriteElementString("MachineRooms", hoist.MachineRooms.ToString());
                                xw.WriteElementString("Capacity", hoist.Capacity.ToString());
                                xw.WriteElementString("UpSpeed", hoist.UpSpeed.ToString());
                                xw.WriteElementString("DownSpeed", hoist.DownSpeed.ToString());
                                xw.WriteElementString("TotalTravel", hoist.TotalTravel.ToString());
                                xw.WriteElementString("LandingSystemID", hoist.LandingSystemID.ToString());
                                aux = hoist.FrontFirstServed ? "1" : "0";
                                xw.WriteElementString("FrontFirstServed", aux);
                                aux = hoist.RearFirstServed ? "1" : "0";
                                xw.WriteElementString("RearFirstServed", aux);
                                aux = hoist.FrontSecondServed ? "1" : "0";
                                xw.WriteElementString("FrontSecondServed", aux);
                                aux = hoist.RearSecondServed ? "1" : "0";
                                xw.WriteElementString("RearSecondServed", aux);
                                aux = hoist.FrontThirdServed ? "1" : "0";
                                xw.WriteElementString("FrontThirdServed", aux);
                                aux = hoist.RearThirdServed ? "1" : "0";
                                xw.WriteElementString("RearThirdServed", aux);
                                aux = hoist.FrontFourthServed ? "1" : "0";
                                xw.WriteElementString("FrontFourthServed", aux);
                                aux = hoist.RearFourthServed ? "1" : "0";
                                xw.WriteElementString("RearFourthServed", aux);
                                aux = hoist.FrontFifthServed ? "1" : "0";
                                xw.WriteElementString("FrontFifthServed", aux);
                                aux = hoist.RearFifthServed ? "1" : "0";
                                xw.WriteElementString("RearFifthServed", aux);
                                aux = hoist.FrontSexthServed ? "1" : "0";
                                xw.WriteElementString("FrontSexthServed", aux);
                                aux = hoist.RearSexthServed ? "1" : "0";
                                xw.WriteElementString("RearSexthServed", aux);
                                aux = hoist.FrontSeventhServed ? "1" : "0";
                                xw.WriteElementString("FrontSeventhServed", aux);
                                aux = hoist.RearSeventhServed ? "1" : "0";
                                xw.WriteElementString("RearSeventhServed", aux);
                                aux = hoist.FrontEightServed ? "1" : "0";
                                xw.WriteElementString("FrontEightServed", aux);
                                aux = hoist.RearEightServed ? "1" : "0";
                                xw.WriteElementString("RearEightServed", aux);
                                aux = hoist.FrontNinthServed ? "1" : "0";
                                xw.WriteElementString("FrontNinthServed", aux);
                                aux = hoist.RearNinthServed ? "1" : "0";
                                xw.WriteElementString("RearNinthServed", aux);
                                aux = hoist.FrontTenthServed ? "1" : "0";
                                xw.WriteElementString("FrontTenthServed", aux);
                                aux = hoist.RearTenthServed ? "1" : "0";
                                xw.WriteElementString("RearTenthServed", aux);
                                aux = hoist.FrontEleventhServed ? "1" : "0";
                                xw.WriteElementString("FrontEleventhServed", aux);
                                aux = hoist.RearEleventhServed ? "1" : "0";
                                xw.WriteElementString("RearEleventhServed", aux);
                                aux = hoist.FrontTwelvethServed ? "1" : "0";
                                xw.WriteElementString("FrontTwelvethServed", aux);
                                aux = hoist.RearTwelvethServed ? "1" : "0";
                                xw.WriteElementString("RearTwelvethServed", aux);
                                aux = hoist.FrontThirteenthServed ? "1" : "0";
                                xw.WriteElementString("FrontThirteenthServed", aux);
                                aux = hoist.RearThirteenthServed ? "1" : "0";
                                xw.WriteElementString("RearThirteenthServed", aux);
                                aux = hoist.FrontFourteenthServed ? "1" : "0";
                                xw.WriteElementString("FrontFourteenthServed", aux);
                                aux = hoist.RearFourteenthServed ? "1" : "0";
                                xw.WriteElementString("RearFourteenthServed", aux);
                                aux = hoist.FrontFifteenthServed ? "1" : "0";
                                xw.WriteElementString("FrontFifteenthServed", aux);
                                aux = hoist.RearFifteenthServed ? "1" : "0";
                                xw.WriteElementString("RearFifteenthServed", aux);
                                aux = hoist.FrontSixteenthServed ? "1" : "0";
                                xw.WriteElementString("FrontSixteenthServed", aux);
                                aux = hoist.RearSixteenthServed ? "1" : "0";
                                xw.WriteElementString("RearSixteenthServed", aux);
                                xw.WriteEndElement();
                            }
                            List<SpecialFeatures> specialList = repository.SpecialFeatures.Where(m => m.JobID == job.JobID).ToList();
                            if (specialList.Count > 0)
                            {
                                xw.WriteStartElement("SpecialFeatures");
                                foreach (SpecialFeatures special in specialList)
                                {
                                    if (special.Description != null)
                                    {
                                        xw.WriteStartElement("SpecialFeature");
                                        xw.WriteElementString("ID", special.SpecialFeaturesID.ToString());
                                        xw.WriteElementString("JobID", special.JobID.ToString());
                                        xw.WriteElementString("Description", special.Description);
                                        xw.WriteEndElement();
                                    }
                                }
                                xw.WriteEndElement();
                            }
                            xw.WriteEndElement();//Jobs
                        }
                    }
                }

                xw.WriteEndElement();
                xw.WriteEndDocument();
            }
            ms.Position = 0;
            return File(ms, "text/xml", "Jobs.xml");
        }

        public static void ImportXML(IServiceProvider services, string resp)
        {
            ApplicationDbContext context = services.GetRequiredService<ApplicationDbContext>();

            HtmlDocument doc = new HtmlDocument();
            doc.Load(@"C:\Users\eary.ortiz\Documents\GitHub\ProodFloorCSharpp\ProdFloor\wwwroot\AppData\Jobs.xml");

            var XMLJob = doc.DocumentNode.SelectNodes("//job");
            var XMLIndicator = doc.DocumentNode.SelectNodes("//indicator");
            var XMLHoist = doc.DocumentNode.SelectNodes("//hoistwayData");
            var XMLSpecialF = doc.DocumentNode.SelectNodes("//specialseatures");

            foreach (var xmlJob in XMLJob)
            {
                    var XMLJobBase = xmlJob.SelectSingleNode("//jobbase");
                    var id = XMLJobBase.SelectSingleNode(".//id").InnerText;
                    var status = XMLJobBase.SelectSingleNode(".//status").InnerText;
                    var engid = XMLJobBase.SelectSingleNode(".//distance").InnerText;
                    var crossappengid = XMLJobBase.SelectSingleNode(".//crossappengid").InnerText;
                    var name = XMLJobBase.SelectSingleNode(".//name").InnerText;
                    var jobnum = XMLJobBase.SelectSingleNode(".//jobnum").InnerText;
                    var po = XMLJobBase.SelectSingleNode(".//po").InnerText;
                    var shipdate = XMLJobBase.SelectSingleNode(".//shipdate").InnerText;
                    var latestfinishdate = XMLJobBase.SelectSingleNode(".//latestfinishdate").InnerText;
                    var cust = XMLJobBase.SelectSingleNode(".//cust").InnerText;
                    var contractor = XMLJobBase.SelectSingleNode(".//contractor").InnerText;
                    var jobtypeid = XMLJobBase.SelectSingleNode(".//jobtypeid").InnerText;
                    var cityid = XMLJobBase.SelectSingleNode(".//cityid").InnerText;
                    var fireCodeid = XMLJobBase.SelectSingleNode(".//fireCodeid").InnerText;
                    context.Jobs.Add(new Job
                    {
                        JobID = Int32.Parse(id),
                        Status = status,
                        EngID = Int32.Parse(engid),
                        CrossAppEngID = Int32.Parse(crossappengid),
                        Name = name,
                        JobNum = Int32.Parse(jobnum),
                        PO = Int32.Parse(po),
                        ShipDate = DateTime.Parse(shipdate),
                        LatestFinishDate = DateTime.Parse(latestfinishdate),
                        Cust = cust,
                        Contractor = contractor,
                        JobTypeID = Int32.Parse(jobtypeid),
                        CityID = Int32.Parse(cityid),
                        FireCodeID = Int32.Parse(fireCodeid),

                    });
                    context.Database.OpenConnection();
                    try
                    {
                        context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Jobs ON");
                        context.SaveChanges();
                        context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Jobs OFF");
                    }
                    finally
                    {
                        context.Database.CloseConnection();
                    }

                var XMLJobExtension = xmlJob.SelectSingleNode("//jobextension");
                var idEx = XMLJobExtension.SelectSingleNode(".//id").InnerText;
                var jobid = XMLJobExtension.SelectSingleNode(".//jobid").InnerText;
                var numofstops = XMLJobExtension.SelectSingleNode(".//numofstops").InnerText;
                var jobtypemain = XMLJobExtension.SelectSingleNode(".//jobtypemain").InnerText;
                var jobtypeadd = XMLJobExtension.SelectSingleNode(".//jobtypeadd").InnerText;
                var inputvoltage = XMLJobExtension.SelectSingleNode(".//inputvoltage").InnerText;
                var inputphase = XMLJobExtension.SelectSingleNode(".//inputphase").InnerText;
                var inputfrecuency = XMLJobExtension.SelectSingleNode(".//inputfrecuency").InnerText;
                var doorgate = XMLJobExtension.SelectSingleNode(".//doorgate").InnerText;
                var doorhoist = XMLJobExtension.SelectSingleNode(".//doorhoist").InnerText;
                var infdetector = XMLJobExtension.SelectSingleNode(".//infdetector").InnerText;
                var mechsafedge = XMLJobExtension.SelectSingleNode(".//mechsafedge").InnerText;
                var heavydoors = XMLJobExtension.SelectSingleNode(".//heavydoors").InnerText;
                var cartopdoorbuttons = XMLJobExtension.SelectSingleNode(".//cartopdoorbuttons").InnerText;
                var doorhold = XMLJobExtension.SelectSingleNode(".//doorhold").InnerText;
                var nudging = XMLJobExtension.SelectSingleNode(".//nudging").InnerText;
                var scop = XMLJobExtension.SelectSingleNode(".//scop").InnerText;
                var shc = XMLJobExtension.SelectSingleNode(".//shc").InnerText;
                var shcrisers = XMLJobExtension.SelectSingleNode(".//shcrisers").InnerText;
                var auxcop = XMLJobExtension.SelectSingleNode(".//auxcop").InnerText;
                var dooroperatorid = XMLJobExtension.SelectSingleNode(".//dooroperatorid").InnerText;
                context.JobsExtensions.Add(new JobExtension
                {
                    JobExtensionID = Int32.Parse(id),
                    JobID = Int32.Parse(jobid),
                    NumOfStops = Int32.Parse(numofstops),
                    JobTypeMain = jobtypemain,
                    JobTypeAdd = jobtypeadd,
                    InputVoltage = Int32.Parse(inputvoltage),
                    InputPhase = Int32.Parse(inputphase),
                    InputFrecuency = Int32.Parse(inputfrecuency),
                    DoorGate = doorgate,
                    DoorHoist = doorhoist,
                    InfDetector = Boolean.Parse(infdetector),
                    MechSafEdge = Boolean.Parse(mechsafedge),
                    HeavyDoors = Boolean.Parse(heavydoors),
                    CartopDoorButtons = Boolean.Parse(cartopdoorbuttons),
                    DoorHold = Boolean.Parse(doorhold),
                    Nudging = Boolean.Parse(nudging),
                    SCOP = Boolean.Parse(scop),
                    SHC = Boolean.Parse(shc),
                    AUXCOP = Boolean.Parse(auxcop),
                    SHCRisers = Int32.Parse(shcrisers),
                    DoorOperatorID = Int32.Parse(dooroperatorid)


                });
                context.Database.OpenConnection();
                try
                {
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.JobsExtensions ON");
                    context.SaveChanges();
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.JobsExtensions OFF");
                }
                finally
                {
                    context.Database.CloseConnection();
                }

                var XMLHydro = xmlJob.SelectSingleNode("//hydrospecific");
                var idHidro = XMLHydro.SelectSingleNode(".//id").InnerText;
                var starter = XMLHydro.SelectSingleNode(".//starter").InnerText;
                var hp = XMLHydro.SelectSingleNode(".//hp").InnerText;
                var fla = XMLHydro.SelectSingleNode(".//fla").InnerText;
                var sph = XMLHydro.SelectSingleNode(".//sph").InnerText;
                var motorsnum = XMLHydro.SelectSingleNode(".//motorsnum").InnerText;
                var motorsdisconnect = XMLHydro.SelectSingleNode(".//motorsdisconnect").InnerText;
                var valvebrand = XMLHydro.SelectSingleNode(".//valvebrand").InnerText;
                var valvecoils = XMLHydro.SelectSingleNode(".//valvecoils").InnerText;
                var valvenum = XMLHydro.SelectSingleNode(".//valvenum").InnerText;
                var valvevoltage = XMLHydro.SelectSingleNode(".//valvevoltage").InnerText;
                var batterybrand = XMLHydro.SelectSingleNode(".//batterybrand").InnerText;
                var battery = XMLHydro.SelectSingleNode(".//battery").InnerText;
                var lifejacket = XMLHydro.SelectSingleNode(".//lifejacket").InnerText;
                var los = XMLHydro.SelectSingleNode(".//los").InnerText;
                var oilcool = XMLHydro.SelectSingleNode(".//oilcool").InnerText;
                var oiltank = XMLHydro.SelectSingleNode(".//oiltank").InnerText;
                var pss = XMLHydro.SelectSingleNode(".//pss").InnerText;
                var resync = XMLHydro.SelectSingleNode(".//resync").InnerText;
                var vci = XMLHydro.SelectSingleNode(".//vci").InnerText;
                context.HydroSpecifics.Add(new HydroSpecific
                {
                    HydroSpecificID = Int32.Parse(idHidro),
                    JobID = Int32.Parse(jobid),
                    Starter = starter,
                    HP = Int32.Parse(hp),
                    FLA = Int32.Parse(fla),
                    SPH = Int32.Parse(sph),
                    MotorsNum = Int32.Parse(motorsnum),
                    MotorsDisconnect = Int32.Parse(motorsdisconnect),
                    ValveBrand = valvebrand,
                    ValveCoils = Int32.Parse(valvecoils),
                    ValveNum = Int32.Parse(valvenum),
                    ValveVoltage = Int32.Parse(valvevoltage),
                    BatteryBrand = batterybrand == null ? "" : batterybrand,
                    Battery = Boolean.Parse(battery),
                    LifeJacket = Boolean.Parse(lifejacket),
                    LOS = Boolean.Parse(los),
                    OilCool = Boolean.Parse(oilcool),
                    OilTank = Boolean.Parse(oiltank),
                    PSS = Boolean.Parse(pss),
                    Resync = Boolean.Parse(resync),
                    VCI = Boolean.Parse(vci),
                });
                context.Database.OpenConnection();
                try
                {
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.HydroSpecifics ON");
                    context.SaveChanges();
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.HydroSpecifics OFF");
                }
                finally
                {
                    context.Database.CloseConnection();
                }

                var XMLGeneric = xmlJob.SelectSingleNode("//genericfeatures");
                var idGeneric = XMLGeneric.SelectSingleNode(".//id").InnerText;
                var fron2 = XMLGeneric.SelectSingleNode(".//fron2").InnerText;
                var attendant = XMLGeneric.SelectSingleNode(".//attendant").InnerText;
                var cartolobby = XMLGeneric.SelectSingleNode(".//cartolobby").InnerText;
                var eq = XMLGeneric.SelectSingleNode(".//eq").InnerText;
                var emt = XMLGeneric.SelectSingleNode(".//emt").InnerText;
                var ep = XMLGeneric.SelectSingleNode(".//ep").InnerText;
                var epvoltage = XMLGeneric.SelectSingleNode(".//epvoltage").InnerText;
                var epothercars = XMLGeneric.SelectSingleNode(".//epothercars").InnerText;
                var epcarsnumber = XMLGeneric.SelectSingleNode(".//epcarsnumber").InnerText;
                var epcontact = XMLGeneric.SelectSingleNode(".//epcontact").InnerText;
                var pti = XMLGeneric.SelectSingleNode(".//pti").InnerText;
                var epselect = XMLGeneric.SelectSingleNode(".//epselect").InnerText;
                var flo = XMLGeneric.SelectSingleNode(".//flo").InnerText;
                var hosp = XMLGeneric.SelectSingleNode(".//hosp").InnerText;
                var ind = XMLGeneric.SelectSingleNode(".//ind").InnerText;
                var ina = XMLGeneric.SelectSingleNode(".//ina").InnerText;
                var topaccess = XMLGeneric.SelectSingleNode(".//topaccess").InnerText;
                var topaccesslocation = XMLGeneric.SelectSingleNode(".//topaccesslocation").InnerText;
                var bottomaccess = XMLGeneric.SelectSingleNode(".//bottomaccess").InnerText;
                var bottomaccesslocation = XMLGeneric.SelectSingleNode(".//bottomaccesslocation").InnerText;
                var incp = XMLGeneric.SelectSingleNode(".//incp").InnerText;
                var incpbuttons = XMLGeneric.SelectSingleNode(".//incpbuttons").InnerText;
                var switchstyle = XMLGeneric.SelectSingleNode(".//switchstyle").InnerText;
                var loadweigher = XMLGeneric.SelectSingleNode(".//loadweigher").InnerText;
                var ctinspst = XMLGeneric.SelectSingleNode(".//ctinspst").InnerText;
                var roped = XMLGeneric.SelectSingleNode(".//roped").InnerText;
                var govmodel = XMLGeneric.SelectSingleNode(".//govmodel").InnerText;
                var monitoring = XMLGeneric.SelectSingleNode(".//monitoring").InnerText;
                var callenable = XMLGeneric.SelectSingleNode(".//callenable").InnerText;
                var carcallread = XMLGeneric.SelectSingleNode(".//carcallread").InnerText;
                var hallcallread = XMLGeneric.SelectSingleNode(".//hallcallread").InnerText;
                var carkey = XMLGeneric.SelectSingleNode(".//carkey").InnerText;
                var hallkey = XMLGeneric.SelectSingleNode(".//hallkey").InnerText;
                var cro = XMLGeneric.SelectSingleNode(".//cro").InnerText;
                var hcro = XMLGeneric.SelectSingleNode(".//hcro").InnerText;
                var carcallcodesecurity = XMLGeneric.SelectSingleNode(".//carcallcodesecurity").InnerText;
                var specialinstructions = XMLGeneric.SelectSingleNode(".//specialinstructions").InnerText;
                context.GenericFeaturesList.Add(new GenericFeatures
                {
                    JobID = Int32.Parse(jobid),
                });
                context.Database.OpenConnection();
                try
                {
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.GenericFeaturesList ON");
                    context.SaveChanges();
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.GenericFeaturesList OFF");
                }
                finally
                {
                    context.Database.CloseConnection();
                }
            }
        }

    }
}
