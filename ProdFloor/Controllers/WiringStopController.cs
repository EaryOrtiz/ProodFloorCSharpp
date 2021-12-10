using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProdFloor.Models;
using ProdFloor.Models.ViewModels;
using ProdFloor.Models.ViewModels.Wiring;

namespace ProdFloor.Controllers
{
    public class WiringStopController : Controller
    {
        private IJobRepository jobRepo;
        private IWiringRepository wiringRepo;
        private IItemRepository itemRepo;
        private ITestingRepository testRepo;
        private WiringController wiringController;
        private UserManager<AppUser> userManager;
        private IHostingEnvironment _env;
        public int PageSize = 7;
        string appDataFolder => _env.WebRootPath.ToString() + @"\AppData\";

        public WiringStopController(IWiringRepository repo,
            IJobRepository repo2,
            IItemRepository repo3,
            ITestingRepository repo4,
            WiringController wiring,
            UserManager<AppUser> userMgr,
            IHostingEnvironment env)
        {
            jobRepo = repo2;
            wiringRepo = repo;
            itemRepo = repo3;
            wiringController = wiring;
            userManager = userMgr;
            testRepo = repo4;
            _env = env;
        }

        public IActionResult List(WiringStopViewModel searchViewModel, int page = 1)
        {
            if (searchViewModel.CleanFields) return RedirectToAction("List");
            if (searchViewModel.Stop == null) searchViewModel.Stop = new WiringStop();

            IQueryable<WiringStop> StopSearchList = wiringRepo.WiringStops.Where(m => !string.IsNullOrEmpty(m.Description));
            if (searchViewModel.WithReassignment == false) StopSearchList = StopSearchList.Where(m => m.Reason1 != 980);
            if (searchViewModel.WithShiftEnd == false) StopSearchList = StopSearchList.Where(m => m.Reason1 != 981);
            if (searchViewModel.WithReturnedFromComplete == false) StopSearchList = StopSearchList.Where(m => m.Reason1 != 982);

            #region StopsInfo
            if (searchViewModel.Stop.Reason1 > 0)
            {
                StopSearchList = StopSearchList.Where(m => m.Reason1 == searchViewModel.Stop.Reason1);
                if (searchViewModel.Stop.Reason2 > 0)
                {
                    StopSearchList = StopSearchList.Where(m => m.Reason2 == searchViewModel.Stop.Reason2);
                    if (searchViewModel.Stop.Reason3 > 0)
                    {
                        StopSearchList = StopSearchList.Where(m => m.Reason3 == searchViewModel.Stop.Reason3);
                        if (searchViewModel.Stop.Reason4 > 0)
                        {
                            StopSearchList = StopSearchList.Where(m => m.Reason4 == searchViewModel.Stop.Reason2);
                            if (searchViewModel.Stop.Reason5ID > 0)
                            {
                                StopSearchList = StopSearchList.Where(m => m.Reason5ID == searchViewModel.Stop.Reason5ID);
                            }
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(searchViewModel.Stop.Description)) StopSearchList = StopSearchList.Where(m => m.Description.Contains(searchViewModel.Stop.Description));
            if (!string.IsNullOrEmpty(searchViewModel.Critical)) StopSearchList = StopSearchList.Where(m => searchViewModel.Critical == "Si" ? m.Critical == true : m.Critical == false);
            #endregion


            searchViewModel.StopList = StopSearchList.OrderBy(p => p.WiringStopID).Skip((page - 1) * 10).Take(10).ToList();
            searchViewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = page,
                ItemsPerPage = 10,
                TotalItems = StopSearchList.Count()
            };

            searchViewModel.Reasons1List = wiringRepo.WiringReasons1.ToList();
            searchViewModel.Reasons2List = wiringRepo.WiringReasons2.ToList();
            searchViewModel.Reasons3List = wiringRepo.WiringReasons3.ToList();
            searchViewModel.Reasons4List = wiringRepo.WiringReasons4.ToList();
            searchViewModel.Reasons5List = wiringRepo.WiringReasons5.ToList();

            searchViewModel.WiringList = wiringRepo.Wirings.Where(m => searchViewModel.StopList.Any(n => n.WiringID == m.WiringID)).ToList();
            searchViewModel.POList = jobRepo.POs.Where(m => searchViewModel.WiringList.Any(n => n.POID == m.POID)).ToList();

            return View(searchViewModel);
        }

        public ViewResult NewStop(int ID)
        {
            return View(new WiringStop { WiringID = ID });
        }

        [HttpPost]
        public ViewResult NewStop(WiringStop Stop)
        {
            if (Stop.Reason1 != 0)
            {
                bool admin = GetCurrentUserRole("Admin").Result;
                Wiring wiring = wiringRepo.Wirings.FirstOrDefault(m => m.WiringID == Stop.WiringID);
                StatusPO statusPO = jobRepo.StatusPOs.FirstOrDefault(m => m.POID == wiring.POID);
                Job job = jobRepo.Jobs.FirstOrDefault(m => m._PO.Any(n => n.POID == wiring.POID));

                WiringStop NewtStop = new WiringStop
                {
                    WiringID = Stop.WiringID,
                    Reason1 = Stop.Reason1,
                    Reason2 = 0,
                    Reason3 = 0,
                    Reason4 = 0,
                    Reason5ID = 0,
                    Description = null,
                    Critical = true,
                    StartDate = DateTime.Now,
                    StopDate = DateTime.Now,
                    Elapsed = new DateTime(1, 1, 1, 0, 0, 0),
                    AuxStationID = wiring.StationID,
                    AuxWirerID = wiring.WiringID,
                };
                wiringRepo.SaveWiringStop(NewtStop);

                try
                {
                    WiringStepForJob currentStep = wiringRepo.WiringStepsForJobs.Where(m => m.WiringID == wiring.WiringID && m.Complete == false && m.Obsolete == false)
                                                                                 .OrderBy(m => m.Consecutivo).FirstOrDefault();
                    //For Current step
                    if (currentStep.Start != DateTime.Now)
                    {
                        currentStep.Elapsed = wiringController.GetElpasedAsDateTime(currentStep.Elapsed, currentStep.Start, DateTime.Now);
                    }

                    currentStep.Start = DateTime.Now;
                    currentStep.Stop = DateTime.Now;
                    wiringRepo.SaveWiringStepForJob(currentStep);
                }
                catch { }

                WiringStopViewModel viewModel = new WiringStopViewModel();
                viewModel.Stop = wiringRepo.WiringStops.Last(m => m.WiringID == wiring.WiringID && m.Reason1 == Stop.Reason1 && m.Reason5ID == 0);
                viewModel.Reason1Name = wiringRepo.WiringReasons1.FirstOrDefault(m => m.WiringReason1ID == viewModel.Stop.Reason1).Description;
                viewModel.JobNum = job.JobNum;


                statusPO.Status = "WR: Stopped";
                jobRepo.SaveStatusPO(statusPO);

                return View("WaitingForRestar", viewModel);

            }
            else
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, seleccione una razonn valida";
                return View("NewStop", Stop);
            }

        }

        public IActionResult WaitingForRestar(int WiringID)
        {
            Wiring wiring = wiringRepo.Wirings.FirstOrDefault(m => m.WiringID == WiringID);
            StatusPO statusPO = jobRepo.StatusPOs.FirstOrDefault(m => m.POID == wiring.POID);
            PO po = jobRepo.POs.FirstOrDefault(m => m.POID == wiring.POID);
            Job job = jobRepo.Jobs.FirstOrDefault(m => m._PO.Any(n => n.POID == wiring.POID));
            int wirerID = GetCurrentUser().Result.EngID;

            if (wiring != null)
            {
                AppUser currentUser = GetCurrentUser().Result;
                bool isSameEngineer = currentUser.EngID == wiring.WirerID;
                bool isNotCompleted = statusPO.Status != "Waiting for PXP";

                if (isNotCompleted && isSameEngineer)
                {
                    WiringStop ReturnedFromCompleteStop = wiringRepo.WiringStops.LastOrDefault(p => p.WiringID == wiring.WiringID && p.Reason1 == 982 && p.Reason5ID == 0);
                    if (wiringController.AnyWiringJobOnProgress(wirerID))
                    {
                        TempData["alert"] = $"alert-danger";
                        TempData["message"] = $"Error, Tiene un trabajo activo, intente de nuevo o contacte al admin";
                        return RedirectToAction("Index", "Home");
                    }
                    else if (ReturnedFromCompleteStop != null)
                    {
                        ReturnedFromCompleteStop.Reason5ID = 982;
                        ReturnedFromCompleteStop.StopDate = DateTime.Now;
                        ReturnedFromCompleteStop.Elapsed = wiringController.GetElpasedAsDateTime(ReturnedFromCompleteStop.Elapsed, ReturnedFromCompleteStop.StartDate, ReturnedFromCompleteStop.StopDate);
                        wiringRepo.SaveWiringStop(ReturnedFromCompleteStop);

                        statusPO.Status = "Wiring on progress";
                        jobRepo.SaveStatusPO(statusPO);

                        return RedirectToAction("ContinueStep", "Wiring", new { WiringID = wiring.WiringID });
                    }
                    WiringStopViewModel viewModel = new WiringStopViewModel();
                    viewModel.Stop = wiringRepo.WiringStops.Last(m => m.WiringID == wiring.WiringID && m.Reason5ID == 0 && m.Critical == true);
                    viewModel.Reason1Name = wiringRepo.WiringReasons1.FirstOrDefault(m => m.WiringReason1ID == viewModel.Stop.Reason1).Description;
                    viewModel.JobNum = job.JobNum;
                    viewModel.PONum = po.PONumb;

                    return View("WaitingForRestar", viewModel);

                }
                else
                {
                    TempData["alert"] = $"alert-danger";
                    if (isNotCompleted == false) TempData["message"] = $"Error, El wiringjob ya ha sido completado, intente de nuevo o contacte al Admin";
                    else TempData["message"] = $"Error, El wiringjob a sido reasignado, intente de nuevo o contacte al Admin";

                    return RedirectToAction("Index", "Home");
                }

            }
            else
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, El wiringjob no existe o a sido eliminado, intente de nuevo o contacte al Admin";
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult RestartWiringJob(int ID, bool Critical = true)
        {
            WiringStop CurrentStop = wiringRepo.WiringStops.FirstOrDefault(p => p.WiringStopID == ID);
            Wiring wiring = wiringRepo.Wirings.FirstOrDefault(m => m.WiringID == CurrentStop.WiringID);
            StatusPO statusPO = jobRepo.StatusPOs.FirstOrDefault(m => m.POID == wiring.POID);
            PO po = jobRepo.POs.FirstOrDefault(m => m.POID == wiring.POID);

            AppUser currentUser = GetCurrentUser().Result;
            bool isSameEngineer = currentUser.EngID == wiring.WirerID;
            bool isNotCompleted = statusPO.Status != "Completed";
            bool isNotOnReassigment = statusPO.Status != "WR: Reassignment";
            bool isNotOnShiftEnd = statusPO.Status != "WR: Shift End";
            bool isOnWorkingOnIt = statusPO.Status == "Wiring on progress";

            if (isNotOnShiftEnd && isNotOnReassigment && isNotCompleted && isSameEngineer)
            {
                if (Critical == true)
                {
                    CurrentStop.Critical = true;
                    wiringRepo.SaveWiringStop(CurrentStop);
                    Job job = jobRepo.Jobs.FirstOrDefault(m => m.JobID == po.JobID);
                    return View(new WiringStopViewModel { JobNum = job.JobNum, Stop = CurrentStop, PONum = po.PONumb });
                }
                else
                {
                    CurrentStop.Critical = false;
                    wiringRepo.SaveWiringStop(CurrentStop);
                    statusPO.Status = "Wiring on progress";
                    jobRepo.SaveStatusPO(statusPO);
                    return RedirectToAction("ContinueStep", "Wiring", new { WiringID = wiring.WiringID });
                }
            }
            else
            {
                TempData["alert"] = $"alert-danger";
                if (isNotCompleted == false) TempData["message"] = $"Error, El wiringjob ya ha sido completado, intente de nuevo o contacte al Admin";
                else if (!isNotOnShiftEnd) TempData["message"] = $"Error, El wiringjob esta en shift end, pulse el boton de continuar";
                else TempData["message"] = $"Error, El wiringjob a sido reasignado, intente de nuevo o contacte al Admin";

                return RedirectToAction("Index", "Home");
            }

        }

        [HttpPost]
        public IActionResult RestartWiringJob(WiringStopViewModel viewModel)
        {
            bool isProductionAdmin = GetCurrentUserRole("ProductionAdmin").Result;
            bool isAdmin = GetCurrentUserRole("Admin").Result;

            WiringStop UpdatedStop = wiringRepo.WiringStops.FirstOrDefault(m => m.WiringStopID == viewModel.Stop.WiringStopID);
            UpdatedStop.StopDate = DateTime.Now;
            UpdatedStop.Elapsed = wiringController.GetElpasedAsDateTime(UpdatedStop.Elapsed, UpdatedStop.StartDate, UpdatedStop.StopDate);
            UpdatedStop.Reason1 = viewModel.Stop.Reason1;
            UpdatedStop.Reason2 = viewModel.Stop.Reason2;
            UpdatedStop.Reason3 = viewModel.Stop.Reason3;
            UpdatedStop.Reason4 = viewModel.Stop.Reason4;
            UpdatedStop.Reason5ID = viewModel.Stop.Reason5ID;
            UpdatedStop.Description = viewModel.Stop.Description;
            wiringRepo.SaveWiringStop(UpdatedStop);

            if (isProductionAdmin || isAdmin)
                return RedirectToAction("SearchStop");

            Wiring wiring = wiringRepo.Wirings.FirstOrDefault(m => m.WiringID == UpdatedStop.WiringID);
            StatusPO statusPO = jobRepo.StatusPOs.FirstOrDefault(m => m.POID == wiring.POID);

            List<WiringStop> stops = wiringRepo.WiringStops.Where(p => wiring.WiringID == p.WiringID)
                                                          .Where(p => p.WiringStopID != 982 && p.WiringStopID != 981 && p.Reason2 == 0).ToList();
            if (stops.Count > 0)
            {
                foreach (WiringStop stop in stops)
                {
                    stop.StartDate = DateTime.Now;
                    stop.StopDate = DateTime.Now;
                    wiringRepo.SaveWiringStop(stop);
                }

                if (stops.Any(m => m.Critical == true))
                {
                    statusPO.Status = "WR: Stopped";
                    jobRepo.SaveStatusPO(statusPO);
                }

            }

            statusPO.Status = "Wiring on progress";
            jobRepo.SaveStatusPO(statusPO);

            wiringController.RestarTimeStep(wiring.WiringID);

            return RedirectToAction("ContinueStep", "Wiring", new { WiringID = wiring.WiringID });
        }

        public IActionResult RestartReassignment(int WiringID)
        {
            Wiring wiring = wiringRepo.Wirings.FirstOrDefault(m => m.WiringID == WiringID);
            if (wiring == null)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, el WiringJob no existe, contacte al Admin";
                return RedirectToAction("Index", "Home");
            }
            int wirerID = GetCurrentUser().Result.EngID;
            if (wiring.WirerID != wirerID)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, el wiringjob no esta reasignadoa usted, contacte al Admin";
                return RedirectToAction("Index", "Home");
            }
            StatusPO statusPO = jobRepo.StatusPOs.FirstOrDefault(m => m.POID == wiring.POID);
            WiringStop ReassignmentStop = wiringRepo.WiringStops.LastOrDefault(p => p.WiringID == wiring.WiringID && p.Reason1 == 980 && p.Reason5ID == 0);
            Job job = jobRepo.Jobs.FirstOrDefault(m => m._PO.Any(n => n.POID == wiring.POID));
            if (wiringController.AnyWiringJobOnProgress(wirerID))
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, Tiene un trabajo activo, intente de nuevo o contacte al admin";
                return RedirectToAction("Index", "Home");
            }

            ReassignmentStop.StopDate = DateTime.Now;
            ReassignmentStop.Elapsed = wiringController.GetElpasedAsDateTime(ReassignmentStop.Elapsed, ReassignmentStop.StartDate, ReassignmentStop.StopDate);
            ReassignmentStop.Reason2 = 980;
            ReassignmentStop.Reason3 = 980;
            ReassignmentStop.Reason4 = 980;
            ReassignmentStop.Reason5ID = 980;
            wiringRepo.SaveWiringStop(ReassignmentStop);

            List<WiringStop> stops = wiringRepo.WiringStops.Where(p => wiring.WiringID == p.WiringID)
                                                           .Where(p => p.WiringStopID != 982 && p.WiringStopID != 981 && p.Reason2 == 0).ToList();
            if (stops.Count > 0)
            {
                foreach (WiringStop stop in stops)
                {
                    stop.StartDate = DateTime.Now;
                    stop.StopDate = DateTime.Now;
                    wiringRepo.SaveWiringStop(stop);
                }

                if (stops.Any(m => m.Critical == true))
                {
                    statusPO.Status = "WR: Stopped";
                    jobRepo.SaveStatusPO(statusPO);
                }

            }

            statusPO.Status = "Wiring on progress";
            jobRepo.SaveStatusPO(statusPO);

            wiringController.RestarTimeStep(WiringID);

            WirersInvolved involved = wiringRepo.WirersInvolveds
                                     .Where(m => m.WiringID == wiring.WiringID)
                                     .FirstOrDefault(m => m.WirerID == wiring.WirerID);
            if (involved == null)
            {
                WirersInvolved wirersInvolved = new WirersInvolved();
                wirersInvolved.WiringID = wiring.WiringID;
                wirersInvolved.WirerID = wiring.WirerID;
                wiringRepo.SaveWirersInvolved(wirersInvolved);
            }

            return RedirectToAction("ContinueStep", "Wiring", new { WiringID = wiring.WiringID });
        }

        public IActionResult WRRestartShiftEnd(int WiringID)
        {
            Wiring wiring = wiringRepo.Wirings.FirstOrDefault(m => m.WiringID == WiringID);
            StatusPO statusPO = jobRepo.StatusPOs.FirstOrDefault(m => m.POID == wiring.POID);
            PO po = jobRepo.POs.FirstOrDefault(m => m.POID == wiring.POID);
            List<WiringStop> stops = new List<WiringStop>();

            try
            {
                WiringStop ShiftEndStop = wiringRepo.WiringStops.LastOrDefault(p => p.WiringID == wiring.WiringID && p.Reason1 == 981 && p.Reason5ID == 0);
                WiringStop ReassignmentStop = wiringRepo.WiringStops.LastOrDefault(p => p.WiringID == wiring.WiringID && p.Reason1 == 980 && p.Reason5ID == 0);
                stops = wiringRepo.WiringStops.Where(p => wiring.WiringID == p.WiringID && p.Reason1 != 980 && p.Reason1 != 981 && p.Reason5ID == 0).ToList();

                TimeSpan auxTime = (DateTime.Now - ShiftEndStop.StartDate);
                ShiftEndStop.Elapsed += auxTime;
                ShiftEndStop.StopDate = DateTime.Now;
                ShiftEndStop.Reason2 = 981;
                ShiftEndStop.Reason3 = 981;
                ShiftEndStop.Reason4 = 981;
                ShiftEndStop.Reason5ID = 981;
                wiringRepo.SaveWiringStop(ShiftEndStop);

                wiringController.RestarTimeStep(wiring.WiringID);

                if (stops.Count > 0)
                {
                    foreach (WiringStop stop in stops)
                    {
                        stop.StartDate = DateTime.Now;
                        stop.StopDate = DateTime.Now;
                        wiringRepo.SaveWiringStop(stop);
                    }

                }

                if (ReassignmentStop != null)
                {
                    ReassignmentStop.StartDate = DateTime.Now;
                    ReassignmentStop.StopDate = DateTime.Now;
                    wiringRepo.SaveWiringStop(ReassignmentStop);

                    statusPO.Status = "WR: Reassignment";
                }
                else if (stops.Any(m => m.Critical == true))
                {
                    statusPO.Status = "WR: Stopped";
                }
                else
                {
                    statusPO.Status = "Wiring on progress";

                    wiringController.RestarTimeStep(wiring.WiringID);
                    TempData["message"] = $"Shift end terminado con exito";
                    return RedirectToAction("ContinueStep", "Wiring", new { WiringID = wiring.WiringID });
                }

                TempData["message"] = $"Shift end terminado con exito";
                return RedirectToAction("List", "Wiring");
            }
            catch (Exception e)
            {
                TempData["message"] = $"Algo salio mal al tratar de cerrar el Shift end, contacte al admin, Error:{e.Message}, PO: {po.PONumb}]";
                TempData["alert"] = $"alert-danger";
                return RedirectToAction("List", "Wiring");
            }

        }

        //Admin only functions
        public ViewResult Edit(int StopID)
        {
            WiringStopViewModel viewModel = new WiringStopViewModel();
            viewModel.Stop = wiringRepo.WiringStops.FirstOrDefault(p => p.WiringStopID == StopID);
            Wiring wiring = wiringRepo.Wirings.FirstOrDefault(m => m.WiringID == viewModel.Stop.WiringID);
            viewModel.PONum = jobRepo.POs.FirstOrDefault(m => m.POID == wiring.POID).PONumb;
            viewModel.JobNum = jobRepo.Jobs.FirstOrDefault(m => m.JobID == wiring.POID).JobNum;

            return View("RestartWiringJob", viewModel);
        }

        [HttpPost]
        public IActionResult Delete(int ID)
        {
            bool isProductionAdmin = GetCurrentUserRole("ProductionAdmin").Result;
            bool isAdmin = GetCurrentUserRole("Admin").Result;

            if (isProductionAdmin == false && isAdmin == false)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error,No tiene permisos para usar esta accion, contacte con el admin";
                return RedirectToAction("List");
            }

            WiringStop deletedStop = wiringRepo.DeleteWiringStop(ID);
            if (deletedStop != null)
            {
                WiringReason1 reason1 = wiringRepo.WiringReasons1.FirstOrDefault(m => m.WiringReason1ID == deletedStop.Reason1);
                TempData["message"] = $"{reason1.Description} was deleted";
            }
            return RedirectToAction("List");
        }

        public IActionResult FinishPendingStops(int wiringID)
        {
            Wiring wiring = wiringRepo.Wirings.FirstOrDefault(m => m.WiringID == wiringID);
            if (wiring == null)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, El Testjob no existe o a sido eliminado, intente de nuevo o contacte al Admin";
                return RedirectToAction("Index", "Home");
            }

            List<WiringStop> stops = wiringRepo.WiringStops.Where(p => wiringID == p.WiringID && p.Reason1 != 980 && p.Reason1 != 981 && p.Reason2 == 0).ToList();
            if (stops.Count == 0)
            {
                TempData["message"] = $"El WiringJob no tiene paradas pendientes";
                return RedirectToAction("JobCompletion", "Wiring", new { WiringID = wiring.WiringID });
            }

            StatusPO statusPO = jobRepo.StatusPOs.FirstOrDefault(m => m.POID == wiring.POID);
            PO po = jobRepo.POs.FirstOrDefault(m => m.POID == wiring.POID);
            Job job = jobRepo.Jobs.FirstOrDefault(m => m._PO.Any(n => n.POID == wiring.POID));

            bool isProductionAdmin = GetCurrentUserRole("ProductionAdmin").Result;
            bool isAdmin = GetCurrentUserRole("Admin").Result;
            bool isNotCompleted = statusPO.Status != "Waiting for PXP";

            if (isNotCompleted && (isAdmin || isProductionAdmin))
            {
                WiringStop CurrentStop = stops.FirstOrDefault();

                return View(new WiringStopViewModel { JobNum = job.JobNum, Stop = CurrentStop, PONum = po.PONumb });
            }
            else
            {
                TempData["alert"] = $"alert-danger";
                if (isNotCompleted == false) TempData["message"] = $"Error, El Testjob ya ha sido completado, intente de nuevo o contacte al Admin";
                else TempData["message"] = $"Error, no cuenta con los permisos suficientes, intente de nuevo o contacte al Admin";

                return RedirectToAction("Index", "Home");
            }

        }

        [HttpPost]
        public IActionResult FinishPendingStops(WiringStopViewModel viewModel)
        {
            Wiring wiring = wiringRepo.Wirings.FirstOrDefault(m => m.WiringID == viewModel.Stop.WiringID);
            if (wiring == null)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, El Testjob no existe o a sido eliminado, intente de nuevo o contacte al Admin";
                return RedirectToAction("Index", "Home");
            }

            WiringStop UpdatedStop = wiringRepo.WiringStops.FirstOrDefault(m => m.WiringStopID == viewModel.Stop.WiringStopID);
            UpdatedStop.StopDate = DateTime.Now;
            UpdatedStop.Elapsed = wiringController.GetElpasedAsDateTime(UpdatedStop.Elapsed, UpdatedStop.StartDate, UpdatedStop.StopDate);
            UpdatedStop.Reason1 = viewModel.Stop.Reason1;
            UpdatedStop.Reason2 = viewModel.Stop.Reason2;
            UpdatedStop.Reason3 = viewModel.Stop.Reason3;
            UpdatedStop.Reason4 = viewModel.Stop.Reason4;
            UpdatedStop.Reason5ID = viewModel.Stop.Reason5ID;
            UpdatedStop.Description = viewModel.Stop.Description;
            wiringRepo.SaveWiringStop(UpdatedStop);

            WiringStop stop = wiringRepo.WiringStops.FirstOrDefault(m => m.Reason1 != 980 & m.Reason1 != 981 && m.Reason2 == 0 && m.WiringID == wiring.WiringID);
            if (stop != null)
            {
                TempData["message"] = $"{UpdatedStop.Description} has been ended..";
                return RedirectToAction("FinishPendingStops", "Stop", new { WiringID = wiring.WiringID });
            }

            TempData["message"] = $"El WiringJob no tiene paradas pendientes";
            return RedirectToAction("JobCompletion", "Wiring", new { WiringID = wiring.WiringID });

        }

        //Extra functions
        private async Task<AppUser> GetCurrentUser()
        {
            AppUser user = await userManager.GetUserAsync(HttpContext.User);

            return user;
        }

        private async Task<bool> GetCurrentUserRole(string role)
        {
            AppUser user = await userManager.GetUserAsync(HttpContext.User);

            bool isInRole = await userManager.IsInRoleAsync(user, role);

            return isInRole;
        }


    }
}
