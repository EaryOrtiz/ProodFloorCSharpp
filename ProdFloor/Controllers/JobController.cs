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
using TextCopy;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Data;

namespace ProdFloor.Controllers
{
    [Authorize(Roles = "Admin,Engineer")]
    public class JobController : Controller
    {
        private IJobRepository repository;
        private IItemRepository itemsrepository;
        private UserManager<AppUser> userManager;
        private readonly IHostingEnvironment environment;
        public int PageSize = 5;

        public JobController(IJobRepository repo, IItemRepository itemsrepo, UserManager<AppUser> userMgr, IHostingEnvironment environment)
        {
            repository = repo;
            itemsrepository = itemsrepo;
            userManager = userMgr;
            this.environment = environment;
        }

        public void alert()
        {
            TempData["alert"] = $"alert-danger";
            TempData["message"] = $"There was an error with your request";
        }

        // Recibe jobType y jobPage y regresa un 
        //JobsListViewModel con los jobs filtrados por tipo y sorteados por JobID 
        public IActionResult List(JobSearchViewModel searchViewModel, int page = 1, int totalitemsfromlastsearch = 0 ,int EngID = 0, string JobTypeName = "M2000")
        {
            searchViewModel.CurrentUserEngID = GetCurrentUser().Result.EngID;
            if (EngID != 0) searchViewModel.EngID = EngID; 
            if (!string.IsNullOrEmpty(JobTypeName)) searchViewModel.JobTypeName = JobTypeName;
            searchViewModel.jobTypeAux = itemsrepository.JobTypes.FirstOrDefault(m => m.Name == JobTypeName);
            var JobCount = repository.Jobs
                     .Where(s => s.Status != "Pending" && s.Status != "Incomplete")
                     .Where(d => d.JobTypeID == searchViewModel.jobTypeAux.JobTypeID)
                     .Count();

            if (searchViewModel.CleanFields) return RedirectToAction("List");
            var jobSearchRepo = repository.Jobs.Include(j => j._jobExtension).Include(hy => hy._HydroSpecific).Include(g => g._GenericFeatures)
                .Include(i => i._Indicator).Include(ho => ho._HoistWayData).Include(sp => sp._SpecialFeatureslist).Include(po => po._PO).Where(y => y.Status != "Pending" && y.Status != "Incomplete")
                .Where(d => d.JobTypeID == searchViewModel.jobTypeAux.JobTypeID).AsQueryable();
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
            else if (searchViewModel.NumJobSearch != 0)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Job number is out of range, ";

            }
            if (searchViewModel.EngID > 0) jobSearchRepo = jobSearchRepo.Where(s => s.EngID == searchViewModel.EngID);
            if (searchViewModel.CrossAppEngID > 0) jobSearchRepo = jobSearchRepo.Where(s => s.CrossAppEngID == searchViewModel.CrossAppEngID);
            if (searchViewModel.CityID > 0) jobSearchRepo = jobSearchRepo.Where(s => s.CityID == searchViewModel.CityID);
            else if (searchViewModel.StateID > 0)
            {
                IQueryable<City> cities = itemsrepository.Cities.Where(m => m.StateID == searchViewModel.StateID);
                jobSearchRepo = jobSearchRepo.Where(m => cities.Any(n => n.CityID == m.CityID));
            } else if (searchViewModel.CountryID > 0)
            {
                IQueryable<State> states = itemsrepository.States.Where(m => m.CountryID == searchViewModel.CountryID);
                IQueryable<City> cities = itemsrepository.Cities.Where(m => states.Any(n => n.StateID == m.StateID));

                jobSearchRepo = jobSearchRepo.Where(m => cities.Any(n => n.CityID == m.CityID));
            }



            if (searchViewModel.FireCodeID > 0) jobSearchRepo = jobSearchRepo.Where(s => s.FireCodeID == searchViewModel.FireCodeID);
            if (searchViewModel.POJobSearch > 3000000 && searchViewModel.POJobSearch < 4900000)
            {
                jobSearchRepo = jobSearchRepo.Where(a => a._PO.Any(b => b.PONumb.Equals(searchViewModel.POJobSearch)));
            }
            else if (searchViewModel.POJobSearch != 0)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] += $"PO number is out of range, ";

            }

            if (!string.IsNullOrEmpty(searchViewModel.NameJobSearch)) jobSearchRepo = jobSearchRepo.Where(s => s.Name.Contains(searchViewModel.NameJobSearch));
            if (!string.IsNullOrEmpty(searchViewModel.Name2)) jobSearchRepo = jobSearchRepo.Where(s => s.Name2.Contains(searchViewModel.Name2));
            if (!string.IsNullOrEmpty(searchViewModel.CustJobSearch)) jobSearchRepo = jobSearchRepo.Where(s => s.Cust.Contains(searchViewModel.CustJobSearch));
            if (!string.IsNullOrEmpty(searchViewModel.ContractorJobSearch)) jobSearchRepo = jobSearchRepo.Where(s => s.Contractor.Contains(searchViewModel.ContractorJobSearch));
            if (!string.IsNullOrEmpty(searchViewModel.StatusJobSearch)) jobSearchRepo = jobSearchRepo.Where(s => s.Status.Equals(searchViewModel.StatusJobSearch));
            #endregion

            #region JobExtension
            //Opciones de busqueda para el modelo de jobExtensions.
            if (searchViewModel.InputFrecuency >= 50 && searchViewModel.InputFrecuency <= 61) jobSearchRepo = jobSearchRepo.Where(s => s._jobExtension.InputFrecuency == searchViewModel.InputFrecuency);
            else if(searchViewModel.InputFrecuency != 0)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] += $"Frequency out of range, ";
            } 
            if (searchViewModel.InputPhase >= 1 && searchViewModel.InputPhase <= 3) jobSearchRepo = jobSearchRepo.Where(s => s._jobExtension.InputPhase == searchViewModel.InputPhase);
            else if (searchViewModel.InputPhase != 0)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] += $"Please enter the input phase, ";
            }
            if (searchViewModel.InputVoltage >= 114 && searchViewModel.InputVoltage <= 600) jobSearchRepo = jobSearchRepo.Where(s => s._jobExtension.InputVoltage == searchViewModel.InputVoltage);
            else if (searchViewModel.InputVoltage != 0)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] += $"Voltage out of range, ";
            }
            if (searchViewModel.NumOfStops >= 1 && searchViewModel.NumOfStops <= 32) jobSearchRepo = jobSearchRepo.Where(s => s._jobExtension.NumOfStops == searchViewModel.NumOfStops);
            else if (searchViewModel.NumOfStops != 0)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] += $"Stops are out of range, ";
            }



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

            if (searchViewModel.MotorsDisconnect > 0) jobSearchRepo = jobSearchRepo.Where(s => s._HydroSpecific.MotorsDisconnect == searchViewModel.MotorsDisconnect);
            if (searchViewModel.ValveNum > 0) jobSearchRepo = jobSearchRepo.Where(s => s._HydroSpecific.ValveNum == searchViewModel.ValveNum);
            if (searchViewModel.ValveCoils > 0) jobSearchRepo = jobSearchRepo.Where(s => s._HydroSpecific.ValveCoils == searchViewModel.ValveCoils);
            if (searchViewModel.ValveVoltage > 0) jobSearchRepo = jobSearchRepo.Where(s => s._HydroSpecific.ValveVoltage == searchViewModel.ValveVoltage);

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

            if (!string.IsNullOrEmpty(searchViewModel.CarCallCodeSecurity)) jobSearchRepo = jobSearchRepo.Where(s => s._GenericFeatures.CarCallCodeSecurity.Contains(searchViewModel.CarCallCodeSecurity));
            if (!string.IsNullOrEmpty(searchViewModel.BSI)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.BSI == "Si" ? s._GenericFeatures.BSI == true : s._GenericFeatures.BSI == false);

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
            if (!string.IsNullOrEmpty(searchViewModel.Pit)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.Pit == "Si" ? s._GenericFeatures.Pit == true : s._GenericFeatures.Pit == false);
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

            #region Indicators
            //Opciones de bsuqueda para el modelo de Indicators

            if (!string.IsNullOrEmpty(searchViewModel.CarPIType)) jobSearchRepo = jobSearchRepo.Where(s => s._Indicator.CarPIType.Equals(searchViewModel.CarPIType));
            if (!string.IsNullOrEmpty(searchViewModel.CarPIDiscreteType)) jobSearchRepo = jobSearchRepo.Where(s => s._Indicator.CarPIDiscreteType.Equals(searchViewModel.CarPIDiscreteType));
            if (!string.IsNullOrEmpty(searchViewModel.HallPIType)) jobSearchRepo = jobSearchRepo.Where(s => s._Indicator.HallPIType.Equals(searchViewModel.HallPIType));
            if (!string.IsNullOrEmpty(searchViewModel.HallPIDiscreteType)) jobSearchRepo = jobSearchRepo.Where(s => s._Indicator.HallPIDiscreteType.Equals(searchViewModel.HallPIDiscreteType));
            if (!string.IsNullOrEmpty(searchViewModel.VoiceAnnunciationPIType)) jobSearchRepo = jobSearchRepo.Where(s => s._Indicator.VoiceAnnunciationPIType.Equals(searchViewModel.VoiceAnnunciationPIType));
            if (!string.IsNullOrEmpty(searchViewModel.CarLanternsStyle)) jobSearchRepo = jobSearchRepo.Where(s => s._Indicator.CarLanternsStyle.Equals(searchViewModel.CarLanternsStyle));
            if (!string.IsNullOrEmpty(searchViewModel.CarLanternsType)) jobSearchRepo = jobSearchRepo.Where(s => s._Indicator.CarLanternsType.Equals(searchViewModel.CarLanternsType));
            if (!string.IsNullOrEmpty(searchViewModel.HallLanternsStyle)) jobSearchRepo = jobSearchRepo.Where(s => s._Indicator.HallLanternsStyle.Equals(searchViewModel.HallLanternsStyle));
            if (!string.IsNullOrEmpty(searchViewModel.HallLanternsType)) jobSearchRepo = jobSearchRepo.Where(s => s._Indicator.HallLanternsType.Equals(searchViewModel.HallLanternsType));
            if (!string.IsNullOrEmpty(searchViewModel.PassingFloorType)) jobSearchRepo = jobSearchRepo.Where(s => s._Indicator.PassingFloorType.Equals(searchViewModel.PassingFloorType));
            if (!string.IsNullOrEmpty(searchViewModel.PassingFloorDiscreteType)) jobSearchRepo = jobSearchRepo.Where(s => s._Indicator.PassingFloorDiscreteType.Equals(searchViewModel.PassingFloorDiscreteType));

            if (searchViewModel.IndicatorsVoltage > 0) jobSearchRepo = jobSearchRepo.Where(s => s._Indicator.IndicatorsVoltage == searchViewModel.IndicatorsVoltage);
            if (!string.IsNullOrEmpty(searchViewModel.IndicatorsVoltageType)) jobSearchRepo = jobSearchRepo.Where(s => s._Indicator.IndicatorsVoltageType.Equals(searchViewModel.IndicatorsVoltageType));

            if (!string.IsNullOrEmpty(searchViewModel.CarLanterns)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.CarLanterns == "Si" ? s._Indicator.CarLanterns == true : s._Indicator.CarLanterns == false);
            if (!string.IsNullOrEmpty(searchViewModel.HallLanterns)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.HallLanterns == "Si" ? s._Indicator.HallLanterns == true : s._Indicator.HallLanterns == false);
            if (!string.IsNullOrEmpty(searchViewModel.HallPI)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.HallPI == "Si" ? s._Indicator.HallPI == true : s._Indicator.HallPI == false);
            if (!string.IsNullOrEmpty(searchViewModel.PassingFloor)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.PassingFloor == "Si" ? s._Indicator.PassingFloor == true : s._Indicator.PassingFloor == false);
            if (!string.IsNullOrEmpty(searchViewModel.CarPI)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.CarPI == "Si" ? s._Indicator.CarPI == true : s._Indicator.CarPI == false);
            if (!string.IsNullOrEmpty(searchViewModel.HallPIAll)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.HallPIAll == "Si" ? s._Indicator.HallPIAll == true : s._Indicator.HallPIAll == false);
            if (!string.IsNullOrEmpty(searchViewModel.VoiceAnnunciationPI)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.VoiceAnnunciationPI == "Si" ? s._Indicator.VoiceAnnunciationPI == true : s._Indicator.VoiceAnnunciationPI == false);
            if (!string.IsNullOrEmpty(searchViewModel.PassingFloorEnable)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.PassingFloorEnable == "Si" ? s._Indicator.PassingFloorEnable == true : s._Indicator.PassingFloorEnable == false);
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

            int TotalItemsSearch = jobSearchRepo.Count();
            if (page == 1 )
            {
                totalitemsfromlastsearch = TotalItemsSearch;
            }
            else if(TotalItemsSearch != totalitemsfromlastsearch)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
                page = 1;
            }
            searchViewModel.Status = new SelectList(statusQuery.Distinct().ToList());
            searchViewModel.JobTypelist = itemsrepository.JobTypes.ToList();
            searchViewModel.Citylist = itemsrepository.Cities.ToList();
            searchViewModel.Statelist = itemsrepository.States.ToList();
            searchViewModel.Landinglist = itemsrepository.LandingSystems.ToList();
            searchViewModel.JobExtensionList = repository.JobsExtensions.ToList();
            searchViewModel.HoistWayDataList = repository.HoistWayDatas.ToList();
            searchViewModel.SpecialFeaturesTable = getSpecialFeaturesEX();
            searchViewModel.JobTotalCount = repository.Jobs.Count();
            searchViewModel.JobsSearchList = jobSearchRepo.OrderByDescending(p => p.JobNum).Skip((page - 1) * 5).Take(5).ToList();

            if (TempData["message"] != null)
            {
                TempData["alert"] = $"alert-danger";
                string Messagge = TempData["message"].ToString();
                TempData["message"] = $"Search error: ";
                TempData["message"] += Messagge;
                TempData["message"] += "please try again.";
            }
            searchViewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = page,
                ItemsPerPage = 5,
                JobTypeName = searchViewModel.JobTypeName,
                TotalItemsFromLastSearch = totalitemsfromlastsearch,
                TotalItems = jobSearchRepo.Count()
            };

            return View(searchViewModel);
        }

        public IActionResult ElementList(ElementSearchViewModel searchViewModel, int page = 1, int totalitemsfromlastsearch = 0, string JobTypeName = "")
        {
            searchViewModel.CurrentUserEngID = GetCurrentUser().Result.EngID;
            if (!string.IsNullOrEmpty(JobTypeName)) searchViewModel.JobTypeName = JobTypeName;
            searchViewModel.jobTypeAux = itemsrepository.JobTypes.FirstOrDefault(m => m.Name == JobTypeName);
            var JobCount = repository.Jobs
                     .Where(s => s.Status != "Pending" && s.Status != "Incomplete")
                     .Where(d => d.JobTypeID == searchViewModel.jobTypeAux.JobTypeID)
                     .Count();


            if (searchViewModel.CleanFields) {
                ElementSearchViewModel NewViewModel = new ElementSearchViewModel();
                NewViewModel.JobTypeName = searchViewModel.JobTypeName;
                return RedirectToAction("ElementList", NewViewModel); 
            }
            var jobSearchRepo = repository.Jobs.Include(j => j._Elements).Include(hy => hy._ElementHydros).Include(g => g._EmentTractions).Include(sp => sp._SpecialFeatureslist)
                .Include(po => po._PO).Where(y => y.Status != "Pending" && y.Status != "Incomplete").Where(d => d.JobTypeID == searchViewModel.jobTypeAux.JobTypeID).AsQueryable();
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
            if (searchViewModel.EngID > 0) jobSearchRepo = jobSearchRepo.Where(s => s.EngID == searchViewModel.EngID);
            if (searchViewModel.CrossAppEngID > 0) jobSearchRepo = jobSearchRepo.Where(s => s.CrossAppEngID == searchViewModel.CrossAppEngID);
            if (searchViewModel.CityID > 0) jobSearchRepo = jobSearchRepo.Where(s => s.CityID == searchViewModel.CityID);
            else if (searchViewModel.StateID > 0)
            {
                IQueryable<City> cities = itemsrepository.Cities.Where(m => m.StateID == searchViewModel.StateID);
                jobSearchRepo = jobSearchRepo.Where(m => cities.Any(n => n.CityID == m.CityID));
            }
            else if (searchViewModel.CountryID > 0)
            {
                IQueryable<State> states = itemsrepository.States.Where(m => m.CountryID == searchViewModel.CountryID);
                IQueryable<City> cities = itemsrepository.Cities.Where(m => states.Any(n => n.StateID == m.StateID));

                jobSearchRepo = jobSearchRepo.Where(m => cities.Any(n => n.CityID == m.CityID));
            }
            if (searchViewModel.FireCodeID > 0) jobSearchRepo = jobSearchRepo.Where(s => s.FireCodeID == searchViewModel.FireCodeID);
            if (searchViewModel.POJobSearch > 3000000 && searchViewModel.POJobSearch < 4900000)
            {
                jobSearchRepo = jobSearchRepo.Where(a => a._PO.Any(b => b.PONumb.Equals(searchViewModel.POJobSearch)));
            }

            if (!string.IsNullOrEmpty(searchViewModel.NameJobSearch)) jobSearchRepo = jobSearchRepo.Where(s => s.Name.Contains(searchViewModel.NameJobSearch));
            if (!string.IsNullOrEmpty(searchViewModel.Name2)) jobSearchRepo = jobSearchRepo.Where(s => s.Name2.Contains(searchViewModel.Name2));
            if (!string.IsNullOrEmpty(searchViewModel.CustJobSearch)) jobSearchRepo = jobSearchRepo.Where(s => s.Cust.Contains(searchViewModel.CustJobSearch));
            if (!string.IsNullOrEmpty(searchViewModel.ContractorJobSearch)) jobSearchRepo = jobSearchRepo.Where(s => s.Contractor.Contains(searchViewModel.ContractorJobSearch));
            if (!string.IsNullOrEmpty(searchViewModel.StatusJobSearch)) jobSearchRepo = jobSearchRepo.Where(s => s.Status.Equals(searchViewModel.StatusJobSearch));
            #endregion


            #region ElementInfo
            //Opciones de bsuqueda para el modelo de GenericFeatures

            if (searchViewModel.DoorOperatorID > 0) jobSearchRepo = jobSearchRepo.Where(s => s._Elements.Any(m => m.DoorOperatorID == searchViewModel.DoorOperatorID));
            if (searchViewModel.LandingSystemID > 0) jobSearchRepo = jobSearchRepo.Where(s => s._Elements.Any(m => m.LandingSystemID == searchViewModel.LandingSystemID));
            if (searchViewModel.Capacity > 0) jobSearchRepo = jobSearchRepo.Where(s => s._Elements.Any(m => m.Capacity == searchViewModel.Capacity));
            if (searchViewModel.Speed > 0) jobSearchRepo = jobSearchRepo.Where(s => s._Elements.Any(m => m.Speed == searchViewModel.Speed));
            if (searchViewModel.Voltage > 0) jobSearchRepo = jobSearchRepo.Where(s => s._Elements.Any(m => m.Voltage == searchViewModel.Voltage));
            if (searchViewModel.Phase > 0) jobSearchRepo = jobSearchRepo.Where(s => s._Elements.Any(m => m.Phase == searchViewModel.Phase));
            if (searchViewModel.Frequency > 0) jobSearchRepo = jobSearchRepo.Where(s => s._Elements.Any(m => m.Frequency == searchViewModel.Frequency));

            if (!string.IsNullOrEmpty(searchViewModel.LoadWeigher)) jobSearchRepo = jobSearchRepo.Where(s => s._Elements.Any(m => m.LoadWeigher.Contains(searchViewModel.LoadWeigher)));
            if (!string.IsNullOrEmpty(searchViewModel.INA)) jobSearchRepo = jobSearchRepo.Where(s => s._Elements.Any(m => m.INA.Contains(searchViewModel.INA)));

            if (!string.IsNullOrEmpty(searchViewModel.CallEnable)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.CallEnable == "Si" ? s._Elements.Any(m => m.CallEnable == true) : s._Elements.Any(m => m.CallEnable == false));
            if (!string.IsNullOrEmpty(searchViewModel.EMT)) jobSearchRepo = jobSearchRepo.Where(s =>searchViewModel.EMT == "Si" ? s._Elements.Any(m => m.EMT == true) : s._Elements.Any(m => m.EMT == false));
            if (!string.IsNullOrEmpty(searchViewModel.EP)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.EP == "Si" ? s._Elements.Any(m => m.EP == true) : s._Elements.Any(m => m.EP == false));
            if (!string.IsNullOrEmpty(searchViewModel.EQ)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.EQ == "Si" ? s._Elements.Any(m => m.EQ == true) : s._Elements.Any(m => m.EQ == false));
            if (!string.IsNullOrEmpty(searchViewModel.FRON2)) jobSearchRepo = jobSearchRepo.Where(s =>searchViewModel.FRON2 == "Si" ? s._Elements.Any(m => m.FRON2 == true) : s._Elements.Any(m => m.FRON2 == false));
            if (!string.IsNullOrEmpty(searchViewModel.CRO)) jobSearchRepo = jobSearchRepo.Where(s =>searchViewModel.CRO == "Si" ? s._Elements.Any(m => m.CRO == true) : s._Elements.Any(m => m.CRO == false));
            if (!string.IsNullOrEmpty(searchViewModel.CarKey)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.CarKey == "Si" ? s._Elements.Any(m => m.CarKey == true) : s._Elements.Any(m => m.CarKey == false));
            if (!string.IsNullOrEmpty(searchViewModel.HCRO)) jobSearchRepo = jobSearchRepo.Where(s =>searchViewModel.HCRO == "Si" ? s._Elements.Any(m => m.HCRO == true) : s._Elements.Any(m => m.HCRO == false));
            if (!string.IsNullOrEmpty(searchViewModel.HallKey)) jobSearchRepo = jobSearchRepo.Where(s =>searchViewModel.HallKey == "Si" ? s._Elements.Any(m => m.HallKey == true) : s._Elements.Any(m => m.HallKey == false));
            if (!string.IsNullOrEmpty(searchViewModel.LOS)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.LOS == "Si" ? s._Elements.Any(m => m.LOS == true) : s._Elements.Any(m => m.LOS == false));
            if (!string.IsNullOrEmpty(searchViewModel.PSS)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.PSS == "Si" ? s._Elements.Any(m => m.PSS == true) : s._Elements.Any(m => m.PSS == false));
            if (!string.IsNullOrEmpty(searchViewModel.VCI)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.VCI == "Si" ? s._Elements.Any(m => m.VCI == true) : s._Elements.Any(m => m.VCI == false));

            if (!string.IsNullOrEmpty(searchViewModel.INCP)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.INCP == "Si" ? s._Elements.Any(m => m.INCP == true) : s._Elements.Any(m => m.INCP == false));
            if (!string.IsNullOrEmpty(searchViewModel.HallCardReader)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.HallCardReader == "Si" ? s._Elements.Any(m => m.HallCardReader == true) : s._Elements.Any(m => m.HallCardReader == false));
            if (!string.IsNullOrEmpty(searchViewModel.HAPS)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.HAPS == "Si" ? s._Elements.Any(m => m.HAPS == true) : s._Elements.Any(m => m.HAPS == false));
            if (!string.IsNullOrEmpty(searchViewModel.PTFLD)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.PTFLD == "Si" ? s._Elements.Any(m => m.PTFLD == true) : s._Elements.Any(m => m.PTFLD == false));
            if (!string.IsNullOrEmpty(searchViewModel.CReg)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.CReg == "Si" ? s._Elements.Any(m => m.CReg == true) : s._Elements.Any(m => m.CReg == false));
            if (!string.IsNullOrEmpty(searchViewModel.Egress)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.Egress == "Si" ? s._Elements.Any(m => m.Egress == true) : s._Elements.Any(m => m.Egress == false));
            if (!string.IsNullOrEmpty(searchViewModel.PHECutOut)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.PHECutOut == "Si" ? s._Elements.Any(m => m.PHECutOut == true) : s._Elements.Any(m => m.PHECutOut == false));
            if (!string.IsNullOrEmpty(searchViewModel.CTINSPST)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.CTINSPST == "Si" ? s._Elements.Any(m => m.CTINSPST == true) : s._Elements.Any(m => m.CTINSPST == false));
            if (!string.IsNullOrEmpty(searchViewModel.Traveler)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.Traveler == "Si" ? s._Elements.Any(m => m.Traveler == true) : s._Elements.Any(m => m.Traveler == false));
            if (!string.IsNullOrEmpty(searchViewModel.PFGE)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.PFGE == "Si" ? s._Elements.Any(m => m.PFGE == true) : s._Elements.Any(m => m.PFGE == false));

            if (!string.IsNullOrEmpty(searchViewModel.CTL)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.CTL == "Si" ? s._Elements.Any(m => m.CTL == true) : s._Elements.Any(m => m.CTL == false));
            if (!string.IsNullOrEmpty(searchViewModel.CSD)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.CSD == "Si" ? s._Elements.Any(m => m.CSD == true) : s._Elements.Any(m => m.CSD == false));
            if (!string.IsNullOrEmpty(searchViewModel.CTF)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.CTF == "Si" ? s._Elements.Any(m => m.CTF == true) : s._Elements.Any(m => m.CTF == false));
            if (!string.IsNullOrEmpty(searchViewModel.LJ)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.LJ == "Si" ? s._Elements.Any(m => m.LJ == true) : s._Elements.Any(m => m.LJ == false));
            if (!string.IsNullOrEmpty(searchViewModel.DHLD)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.DHLD == "Si" ? s._Elements.Any(m => m.DHLD == true) : s._Elements.Any(m => m.DHLD == false));

            #endregion

            #region Elements
            //Opciones de bsuqueda para el modelo de HydroSpecifics

            if (searchViewModel.JobTypeName == "ElmTract")
            {
                if (searchViewModel.FLA > 0) jobSearchRepo = jobSearchRepo.Where(s => s._EmentTractions.Any(m => m.FLA == searchViewModel.FLA));
                if (searchViewModel.HP > 0) jobSearchRepo = jobSearchRepo.Where(s => s._EmentTractions.Any(m => m.HP == searchViewModel.HP));
                if (searchViewModel.PickVoltage > 0) jobSearchRepo = jobSearchRepo.Where(s => s._EmentTractions.Any(m => m.PickVoltage == searchViewModel.PickVoltage));
                if (searchViewModel.HoldVoltage > 0) jobSearchRepo = jobSearchRepo.Where(s => s._EmentTractions.Any(m => m.HoldVoltage == searchViewModel.HoldVoltage));
                if (searchViewModel.Resistance > 0) jobSearchRepo = jobSearchRepo.Where(s => s._EmentTractions.Any(m => m.Resistance == searchViewModel.Resistance));
                if (searchViewModel.Current > 0) jobSearchRepo = jobSearchRepo.Where(s => s._EmentTractions.Any(m => m.Current == searchViewModel.Current));

                if (!string.IsNullOrEmpty(searchViewModel.MachineLocation)) jobSearchRepo = jobSearchRepo.Where(s => s._EmentTractions.Any(m => m.MachineLocation.Contains(searchViewModel.MachineLocation)));
                if (!string.IsNullOrEmpty(searchViewModel.VVVF)) jobSearchRepo = jobSearchRepo.Where(s => s._EmentTractions.Any(m => m.VVVF.Contains(searchViewModel.VVVF)));
                if (!string.IsNullOrEmpty(searchViewModel.MotorBrand)) jobSearchRepo = jobSearchRepo.Where(s => s._EmentTractions.Any(m => m.MotorBrand.Contains(searchViewModel.MotorBrand)));
                if (!string.IsNullOrEmpty(searchViewModel.Contact)) jobSearchRepo = jobSearchRepo.Where(s => s._EmentTractions.Any(m => m.Contact.Contains(searchViewModel.Contact)));

                if (!string.IsNullOrEmpty(searchViewModel.Encoder)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.Encoder == "Si" ? s._EmentTractions.Any(m => m.Encoder == true) : s._EmentTractions.Any(m => m.Encoder == false));
                if (!string.IsNullOrEmpty(searchViewModel.ISO)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.ISO == "Si" ? s._EmentTractions.Any(m => m.ISO == true) : s._EmentTractions.Any(m => m.ISO == false));
            }
            else
            {
                if (searchViewModel.FLA > 0) jobSearchRepo = jobSearchRepo.Where(s => s._ElementHydros.Any(m => m.FLA == searchViewModel.FLA));
                if (searchViewModel.HP > 0) jobSearchRepo = jobSearchRepo.Where(s => s._ElementHydros.Any(m => m.HP == searchViewModel.HP));
                if (searchViewModel.SPH > 0) jobSearchRepo = jobSearchRepo.Where(s => s._ElementHydros.Any(m => m.SPH == searchViewModel.SPH));
                

                if (!string.IsNullOrEmpty(searchViewModel.Starter)) jobSearchRepo = jobSearchRepo.Where(s => s._ElementHydros.Any(m => m.Starter.Contains(searchViewModel.Starter)));
                if (!string.IsNullOrEmpty(searchViewModel.ValveBrand)) jobSearchRepo = jobSearchRepo.Where(s => s._ElementHydros.Any(m => m.ValveBrand.Contains(searchViewModel.ValveBrand)));
            }

            #endregion

            #region SpecialFeatures
            //Opciones de bsuqueda para el modelo de Special Features
            if (!string.IsNullOrEmpty(searchViewModel.Description))
            {
                jobSearchRepo = jobSearchRepo.Where(a => a._SpecialFeatureslist.Any(b => b.Description.Equals(searchViewModel.Description)));
            }
            #endregion

            int TotalItemsSearch = jobSearchRepo.Count();
            if (page == 1)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
            }
            else if (TotalItemsSearch != totalitemsfromlastsearch)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
                page = 1;
            }
            searchViewModel.Status = new SelectList(statusQuery.Distinct().ToList());
            searchViewModel.Citylist = itemsrepository.Cities.ToList();
            searchViewModel.Statelist = itemsrepository.States.ToList();
            searchViewModel.ElementList = repository.Elements.ToList();
            searchViewModel.Landinglist = itemsrepository.LandingSystems.ToList();
            searchViewModel.SpecialFeaturesTable = getSpecialFeaturesEX();
            searchViewModel.JobsSearchList = jobSearchRepo.OrderBy(p => p.JobID).Skip((page - 1) * 5).Take(5).ToList();
            searchViewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = page,
                ItemsPerPage = 5,
                JobTypeName = searchViewModel.JobTypeName,
                TotalItemsFromLastSearch = totalitemsfromlastsearch,
                TotalItems = jobSearchRepo.Count()
            };

            return View(searchViewModel);
        }

        public ViewResult MyjobsList(int page = 1)
        {
            AppUser currentUser = GetCurrentUser().Result;
            var JobCount = repository.Jobs
                     .Where(s => s.Status != "Pending")
                    .Where(n => n.EngID == currentUser.EngID).Count();


            return View(new JobsListViewModel
            {
                Jobs = repository.Jobs
                    .Where(s => s.Status != "Pending")
                    .Where( n => n.EngID == currentUser.EngID)
                    .OrderBy(p => p.JobID)
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = JobCount
                },
                MyJobs = true
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
            return RedirectToAction("Index","Home");
        }

        [HttpPost]
        public IActionResult DeleteAdmin(int ID)
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
            return RedirectToAction("EngineerAdminDashBoard", "Home");
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
        public ViewResult NewJob(string JobType)
        {
            int JobtypeID = itemsrepository.JobTypes.FirstOrDefault(m => m.Name == JobType).JobTypeID;
            switch (JobType)
            {
                case "M2000":
                    JobViewModel viewModel1 = new JobViewModel
                    {
                        CurrentJob = new Job { ShipDate = DateTime.Now, LatestFinishDate = DateTime.Now, JobTypeID = JobtypeID },
                        POList = new List<PO> { new PO { JobID = 0 } },
                        JobTypeName = JobType
                    };
                    return View(viewModel1);

                case "ElmHydro":
                    JobViewModel viewModel2 = new JobViewModel
                    {
                        CurrentJob = new Job { ShipDate = DateTime.Now, LatestFinishDate = DateTime.Now, JobTypeID = JobtypeID },
                        POList = new List<PO> { new PO { JobID = 0 } },
                        JobTypeName = JobType
                    };
                    return View(viewModel2);

                case "ElmTract":
                    JobViewModel viewModel3 = new JobViewModel
                    {
                        CurrentJob = new Job { ShipDate = DateTime.Now, LatestFinishDate = DateTime.Now, JobTypeID = JobtypeID },
                        POList = new List<PO> { new PO { JobID = 0 } },
                        JobTypeName = JobType
                    };
                    return View(viewModel3);

                default: return View(NotFound());
            }

        }

        [HttpPost]
        public IActionResult NewJob(JobViewModel newJob)
        {
            //Desactivar esta funcion para que funcione el test de Job
            AppUser currentUser = GetCurrentUser().Result;
            newJob.SpecialFeaturesTable = getSpecialFeaturesEX();
            if (ModelState.IsValid)
            {
                List<PO> PoAux = new List<PO>();
                foreach (PO itemes in newJob.POList)
                {
                    try
                    {
                        PO poUniqueAUx = repository.POs.FirstOrDefault(m => m.PONumb == itemes.PONumb);
                        PoAux.Add(poUniqueAUx);
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                }
                if (PoAux.Count <= 0 || PoAux[0] == null)
                {
                    //y esta esta tambien y poner denuevo el {currenuser.engId} en Los TempDatas cuando terminen los test 
                    newJob.CurrentJob.EngID = currentUser.EngID;
                    newJob.CurrentJob.CrossAppEngID = 0;
                    newJob.CurrentJob.Status = "Incomplete";
                    repository.SaveJob(newJob.CurrentJob);
                    Job currentJob = repository.Jobs.FirstOrDefault(p => p.JobID == repository.Jobs.Max(x => x.JobID));
                    JobAdditional jobAdditional = new JobAdditional
                    {
                        JobID = currentJob.JobID,
                        Status = "",
                        Action = "",
                        Priority = 0,
                        ERDate = currentJob.LatestFinishDate
                    };
                    repository.SaveJobAdditional(jobAdditional);
                    foreach (PO items in newJob.POList)
                    {
                        items.JobID = currentJob.JobID;
                        repository.SavePO(items);
                    }
                    List<PO> POsList = repository.POs.Where(j => j.JobID == currentJob.JobID).ToList();
                    List<PO> POlistAUX = new List<PO>();
                    if (POsList != null) POlistAUX = POsList;
                    else POlistAUX = new List<PO> { new PO() };

                    if (JobTypeName(currentJob.JobTypeID) == "ElmHydro")
                    {
                        JobElementHydroViewModel elementHydroViewModel = new JobElementHydroViewModel
                        {
                            CurrentUserID = currentUser.EngID,
                            CurrentJob = currentJob,
                            Element = new Element { JobID = newJob.CurrentJob.JobID },
                            ElementHydro = new ElementHydro(),
                            SpecialFeatureslist = new List<SpecialFeatures> { new SpecialFeatures() },
                            POList = POlistAUX,
                            SpecialFeaturesTable = getSpecialFeaturesEX(),
                            CurrentTab = "Element"
                        };
                        TempData["message"] = $"Job# {elementHydroViewModel.CurrentJob.JobNum} has been saved...{elementHydroViewModel.CurrentJob.JobID}...";
                        return View("NextFormHydro", elementHydroViewModel);
                    }
                    else if (JobTypeName(currentJob.JobTypeID) == "ElmTract")
                    {
                        JobElementTractionViewModel jobElementTraction = new JobElementTractionViewModel
                        {
                            CurrentUserID = currentUser.EngID,
                            CurrentJob = currentJob,
                            Element = new Element { JobID = newJob.CurrentJob.JobID },
                            ElementTraction = new ElementTraction(),
                            SpecialFeatureslist = new List<SpecialFeatures> { new SpecialFeatures() },
                            POList = POlistAUX,
                            SpecialFeaturesTable = getSpecialFeaturesEX(),
                            CurrentTab = "Element"
                        };
                        TempData["message"] = $"Job# {jobElementTraction.CurrentJob.JobNum} has been saved...{jobElementTraction.CurrentJob.JobID}...";
                        return View("NextFormTraction", jobElementTraction);
                    }
                    else
                    {
                        JobViewModel newJobViewModel = new JobViewModel
                        {
                            CurrentUserID = currentUser.EngID,
                            CurrentJob = currentJob,
                            CurrentJobExtension = new JobExtension { JobID = newJob.CurrentJob.JobID },
                            CurrentHydroSpecific = new HydroSpecific(),
                            CurrentGenericFeatures = new GenericFeatures(),
                            CurrentIndicator = new Indicator(),
                            CurrentHoistWayData = new HoistWayData(),
                            SpecialFeatureslist = new List<SpecialFeatures> { new SpecialFeatures() },
                            POList = POlistAUX,
                            SpecialFeaturesTable = getSpecialFeaturesEX(),
                            CurrentTab = "Extension"
                        };
                        TempData["message"] = $"Job# {newJobViewModel.CurrentJob.JobNum} has been saved...{newJobViewModel.CurrentJob.JobID}...";
                        return View("NextForm", newJobViewModel);
                    }
                }
                else
                {
                    TempData["message"] = $"One of the POs already exists. Please validate.";
                    TempData["alert"] = $"alert-danger";
                    return View(newJob);
                }

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
        public IActionResult Edit(int ID, string buttonAction)
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
                if (JobTypeName(job.JobTypeID) == "ElmHydro")
                {
                    List<PO> POsList = repository.POs.Where(j => j.JobID == ID).ToList();
                    List<SpecialFeatures> SfList = repository.SpecialFeatures.Where(j => j.JobID == ID).ToList();
                    JobElementHydroViewModel hydroViewModel = new JobElementHydroViewModel();
                    hydroViewModel.CurrentJob = job;
                    string LastFive = job.JobNum.ToString().Substring(5);
                    string FirstTwo = LastFive.Substring(0, 2);
                    hydroViewModel.JobFolder = @"L:\" + FirstTwo + "000\\" + LastFive;
                    hydroViewModel.Element = repository.Elements.FirstOrDefault(j => j.JobID == ID);
                    hydroViewModel.ElementHydro = repository.ElementHydros.FirstOrDefault(j => j.JobID == ID);
                    if (SfList != null) hydroViewModel.SpecialFeatureslist = SfList;
                    else hydroViewModel.SpecialFeatureslist = new List<SpecialFeatures> { new SpecialFeatures() };
                    if (POsList != null) hydroViewModel.POList = POsList;
                    else hydroViewModel.POList = new List<PO> { new PO() };
                    hydroViewModel.CurrentUserID = currentUser.EngID;
                    hydroViewModel.CurrentTab = "Main";
                    hydroViewModel.JobTypeName = JobTypeName(job.JobTypeID);
                    hydroViewModel.SpecialFeaturesTable = getSpecialFeaturesEX();

                    return View("EditHydro", hydroViewModel);
                }
                else if (JobTypeName(job.JobTypeID) == "ElmTract")
                {
                    List<PO> POsList = repository.POs.Where(j => j.JobID == ID).ToList();
                    List<SpecialFeatures> SfList = repository.SpecialFeatures.Where(j => j.JobID == ID).ToList();
                    JobElementTractionViewModel jobElementTractionView = new JobElementTractionViewModel();
                    string LastFive = job.JobNum.ToString().Substring(5);
                    string FirstTwo = LastFive.Substring(0, 2);
                    jobElementTractionView.JobFolder = @"L:\" + FirstTwo + "000\\" + LastFive;
                    jobElementTractionView.CurrentJob = job;
                    jobElementTractionView.Element = repository.Elements.FirstOrDefault(j => j.JobID == ID);
                    jobElementTractionView.ElementTraction = repository.ElementTractions.FirstOrDefault(j => j.JobID == ID);
                    if (SfList != null) jobElementTractionView.SpecialFeatureslist = SfList;
                    else jobElementTractionView.SpecialFeatureslist = new List<SpecialFeatures> { new SpecialFeatures() };
                    if (POsList != null) jobElementTractionView.POList = POsList;
                    else jobElementTractionView.POList = new List<PO> { new PO() };
                    jobElementTractionView.CurrentUserID = currentUser.EngID;
                    jobElementTractionView.CurrentTab = "Main";
                    jobElementTractionView.JobTypeName = JobTypeName(job.JobTypeID);
                    jobElementTractionView.SpecialFeaturesTable = getSpecialFeaturesEX();

                    return View("EditTraction", jobElementTractionView);
                }
                else
                {
                    List<PO> POsList = repository.POs.Where(j => j.JobID == ID).ToList();
                    List<SpecialFeatures> SfList = repository.SpecialFeatures.Where(j => j.JobID == ID).ToList();
                    JobViewModel viewModel = new JobViewModel();
                    string LastFive = job.JobNum.ToString().Substring(5);
                    string FirstTwo = LastFive.Substring(0, 2);
                    viewModel.JobFolder = @"L:\" + FirstTwo + "000\\" + LastFive;
                    viewModel.CurrentJob = job;
                    viewModel.CurrentJobExtension = repository.JobsExtensions.FirstOrDefault(j => j.JobID == ID);
                    viewModel.CurrentHydroSpecific = repository.HydroSpecifics.FirstOrDefault(j => j.JobID == ID);
                    viewModel.CurrentGenericFeatures = repository.GenericFeaturesList.FirstOrDefault(j => j.JobID == ID);
                    viewModel.CurrentIndicator = repository.Indicators.FirstOrDefault(j => j.JobID == ID);
                    viewModel.CurrentHoistWayData = repository.HoistWayDatas.FirstOrDefault(j => j.JobID == ID);
                    if (SfList != null) viewModel.SpecialFeatureslist = SfList;
                    else viewModel.SpecialFeatureslist = new List<SpecialFeatures> { new SpecialFeatures() };
                    if (POsList != null) viewModel.POList = POsList;
                    else viewModel.POList = new List<PO> { new PO() };
                    viewModel.CurrentUserID = currentUser.EngID;
                    viewModel.CurrentTab = "Main";
                    viewModel.JobTypeName = JobTypeName(viewModel.CurrentJob.JobTypeID);
                    viewModel.SpecialFeaturesTable = getSpecialFeaturesEX();
                    return View(viewModel);
                }


            }
        }


        public IActionResult CopyJob(int ID)
        {
            AppUser currentUser = GetCurrentUser().Result;
            Job jobToCopy = repository.Jobs.FirstOrDefault(j => j.JobID == ID);
            if (jobToCopy == null)
            {
                TempData["message"] = $"The requested Job doesn't exist.";
                return RedirectToAction("List");
            }
            else
            {
                //Get the job
                List<SpecialFeatures> SfList = repository.SpecialFeatures.Where(j => j.JobID == ID).ToList();
                JobViewModel viewModel = new JobViewModel();
                viewModel.CurrentJob = jobToCopy;
                viewModel.CurrentJobExtension = repository.JobsExtensions.FirstOrDefault(j => j.JobID == ID);
                viewModel.CurrentHydroSpecific = repository.HydroSpecifics.FirstOrDefault(j => j.JobID == ID);
                viewModel.CurrentGenericFeatures = repository.GenericFeaturesList.FirstOrDefault(j => j.JobID == ID);
                viewModel.CurrentIndicator = repository.Indicators.FirstOrDefault(j => j.JobID == ID);
                viewModel.CurrentHoistWayData = repository.HoistWayDatas.FirstOrDefault(j => j.JobID == ID);
                viewModel.SpecialFeatureslist = SfList;
                int jobNumAux = viewModel.CurrentJob.JobNum;
                viewModel.CurrentJob.JobNum = 0;
                viewModel.CurrentJob.Status = "Copied";
                viewModel.POList = new List<PO> { new PO { JobID = viewModel.CurrentJob.JobID } };
                viewModel.CurrentUserID = currentUser.EngID;
                viewModel.CurrentJob.CrossAppEngID = 0;
                viewModel.CurrentJob.EngID = currentUser.EngID;
                viewModel.JobTypeName = JobTypeName(jobToCopy.JobTypeID);

                TempData["message"] = $"You have copied the job #{jobNumAux} succesfully, please change the name, Job number & PO";
                return View("Edit", viewModel);
            }
        }

        public IActionResult CopyHydroJob(int ID)
        {
            AppUser currentUser = GetCurrentUser().Result;
            Job jobToCopy = repository.Jobs.FirstOrDefault(j => j.JobID == ID);
            if (jobToCopy == null)
            {
                TempData["message"] = $"The requested Job doesn't exist.";
                return RedirectToAction("List");
            }
            else
            {
                //Get the job
                List<SpecialFeatures> SfList = repository.SpecialFeatures.Where(j => j.JobID == ID).ToList();
                JobElementHydroViewModel viewModel = new JobElementHydroViewModel();
                viewModel.CurrentJob = jobToCopy;
                viewModel.Element = repository.Elements.FirstOrDefault(j => j.JobID == ID);
                viewModel.ElementHydro = repository.ElementHydros.FirstOrDefault(j => j.JobID == ID);
                viewModel.SpecialFeatureslist = SfList;
                int jobNumAux = viewModel.CurrentJob.JobNum;
                viewModel.CurrentJob.JobNum = 0;
                viewModel.CurrentUserID = currentUser.EngID;
                viewModel.CurrentJob.CrossAppEngID = 0;
                viewModel.CurrentJob.CrossAppEngID = 0;
                viewModel.JobTypeName = JobTypeName(jobToCopy.JobTypeID);
                viewModel.CurrentJob.Status = "Copied";
                viewModel.POList = new List<PO> { new PO { JobID = viewModel.CurrentJob.JobID } };
                viewModel.CurrentJob.EngID = currentUser.EngID;

                TempData["message"] = $"You have copied the job #{jobNumAux} succesfully, please change the name, Job number & PO";
                return View("EditHydro", viewModel);
            }
        }

        public IActionResult CopyTractionJob(int ID)
        {
            AppUser currentUser = GetCurrentUser().Result;
            Job jobToCopy = repository.Jobs.FirstOrDefault(j => j.JobID == ID);
            if (jobToCopy == null)
            {
                TempData["message"] = $"The requested Job doesn't exist.";
                return RedirectToAction("List");
            }
            else
            {
                //Get the job
                List<SpecialFeatures> SfList = repository.SpecialFeatures.Where(j => j.JobID == ID).ToList();
                JobElementTractionViewModel viewModel = new JobElementTractionViewModel();
                viewModel.CurrentJob = jobToCopy;
                viewModel.Element = repository.Elements.FirstOrDefault(j => j.JobID == ID);
                viewModel.ElementTraction = repository.ElementTractions.FirstOrDefault(j => j.JobID == ID);
                viewModel.SpecialFeatureslist = SfList;
                int jobNumAux = viewModel.CurrentJob.JobNum;
                viewModel.CurrentJob.JobNum = 0;
                viewModel.JobTypeName = JobTypeName(jobToCopy.JobTypeID);
                viewModel.CurrentUserID = currentUser.EngID;
                viewModel.CurrentJob.CrossAppEngID = 0;
                viewModel.CurrentJob.Status = "Copied";
                viewModel.POList = new List<PO> { new PO { JobID = viewModel.CurrentJob.JobID } };
                viewModel.CurrentJob.EngID = currentUser.EngID;

                TempData["message"] = $"You have copied the job #{jobNumAux} succesfully, please change the name, Job number & PO";
                return View("EditTraction", viewModel);
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
            multiEditViewModel.SpecialFeaturesTable = getSpecialFeaturesEX();
            multiEditViewModel.CurrentUserID = currentUser.EngID;
            string StatusAux = "Working on it";
            if (multiEditViewModel.CurrentJob.Status == "Copied") StatusAux = "Copied";
            List<PO> PoAux = new List<PO>();
            if (ModelState.IsValid)
            {
                if (multiEditViewModel.CurrentJob.Status == "Copied")
                {
                    multiEditViewModel.CurrentJob.EngID = currentUser.EngID;
                    foreach (PO itemes in multiEditViewModel.POList)
                    {
                        try
                        {
                            if (multiEditViewModel.POList[0].PONumb != itemes.PONumb || multiEditViewModel.POList[0].POID == 0)
                            {
                                    PO poUniqueAUx = repository.POs.FirstOrDefault(m => m.PONumb == itemes.PONumb);
                                    PoAux.Add(poUniqueAUx);
                            }
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                    foreach(PO po in PoAux)
                    {
                        if(po != null)
                        {
                            TempData["message"] = $"That PO already exists. Please validate.";
                            TempData["alert"] = $"alert-danger";
                            multiEditViewModel.CurrentTab = "Main";
                            return View(multiEditViewModel);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    multiEditViewModel.CurrentJob.JobID = 0;
                    multiEditViewModel.CurrentJob.Status = "Working on it";
                    repository.SaveJob(multiEditViewModel.CurrentJob);
                    multiEditViewModel.CurrentJob = repository.Jobs.LastOrDefault();
                    JobAdditional jobAdditional = new JobAdditional
                    {
                        JobID = multiEditViewModel.CurrentJob.JobID,
                        Status = "",
                        Action = "",
                        Priority = 0,
                        ERDate = multiEditViewModel.CurrentJob.LatestFinishDate
                    };
                    repository.SaveJobAdditional(jobAdditional);
                    multiEditViewModel.CurrentJobExtension.JobID = multiEditViewModel.CurrentJob.JobID;
                    multiEditViewModel.CurrentHydroSpecific.JobID = multiEditViewModel.CurrentJob.JobID;
                    multiEditViewModel.CurrentGenericFeatures.JobID = multiEditViewModel.CurrentJob.JobID;
                    multiEditViewModel.CurrentIndicator.JobID = multiEditViewModel.CurrentJob.JobID;
                    multiEditViewModel.CurrentHoistWayData.JobID = multiEditViewModel.CurrentJob.JobID;
                    multiEditViewModel.CurrentJobExtension.JobExtensionID = 0;
                    multiEditViewModel.CurrentHydroSpecific.HydroSpecificID = 0;
                    multiEditViewModel.CurrentGenericFeatures.GenericFeaturesID = 0;
                    multiEditViewModel.CurrentIndicator.IndicatorID = 0;
                    multiEditViewModel.CurrentHoistWayData.HoistWayDataID = 0;
                    foreach (PO singlPO in multiEditViewModel.POList)
                    {
                        singlPO.POID = 0;
                        singlPO.JobID = multiEditViewModel.CurrentJob.JobID;
                    }
                    foreach (SpecialFeatures special in multiEditViewModel.SpecialFeatureslist)
                    {
                        special.SpecialFeaturesID = 0;
                        special.JobID = multiEditViewModel.CurrentJob.JobID;
                    }
                    multiEditViewModel.SpecialFeatureslist = multiEditViewModel.SpecialFeatureslist;
                }

                if (multiEditViewModel.CurrentJob.Status == "" || multiEditViewModel.CurrentJob.Status == null || multiEditViewModel.CurrentJob.Status == "Copied")
                {
                    multiEditViewModel.CurrentJob.Status = "Working on it";
                }
                if (multiEditViewModel.CurrentHydroSpecific.BatteryBrand == "Other" && !string.IsNullOrEmpty(multiEditViewModel.CurrentHydroSpecific.OtherBatteryBrand))
                {
                    multiEditViewModel.CurrentHydroSpecific.BatteryBrand = multiEditViewModel.CurrentHydroSpecific.OtherBatteryBrand;
                }
                try
                {
                    foreach (PO itemes in multiEditViewModel.POList.Where(m => m.POID == 0))
                    {
                        try
                        {
                            if (multiEditViewModel.POList[0].PONumb != itemes.PONumb || multiEditViewModel.POList[0].POID == 0)
                            {
                                PO poUniqueAUx = repository.POs.FirstOrDefault(m => m.PONumb == itemes.PONumb);
                                PoAux.Add(poUniqueAUx);
                            }
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                    foreach (PO po in PoAux)
                    {
                        if (po != null)
                        {
                            TempData["message"] = $"That PO already exists. Please validate.";
                            TempData["alert"] = $"alert-danger";
                            multiEditViewModel.CurrentTab = "Main";
                            return View(multiEditViewModel);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    repository.SaveEngJobView(multiEditViewModel);
                    JobViewModel CopyJobViewModel = new JobViewModel();
                    if (StatusAux == "Copied")
                    {
                        List<SpecialFeatures> SfList = repository.SpecialFeatures.Where(j => j.JobID == multiEditViewModel.CurrentJob.JobID).ToList();
                        List<PO> PoList = repository.POs.Where(j => j.JobID == multiEditViewModel.CurrentJob.JobID).ToList();
                        CopyJobViewModel.CurrentJob = multiEditViewModel.CurrentJob;
                        CopyJobViewModel.CurrentJobExtension = repository.JobsExtensions.FirstOrDefault(j => j.JobID == multiEditViewModel.CurrentJob.JobID);
                        CopyJobViewModel.CurrentHydroSpecific = repository.HydroSpecifics.FirstOrDefault(j => j.JobID == multiEditViewModel.CurrentJob.JobID);
                        CopyJobViewModel.CurrentGenericFeatures = repository.GenericFeaturesList.FirstOrDefault(j => j.JobID == multiEditViewModel.CurrentJob.JobID);
                        CopyJobViewModel.CurrentIndicator = repository.Indicators.FirstOrDefault(j => j.JobID == multiEditViewModel.CurrentJob.JobID);
                        CopyJobViewModel.CurrentHoistWayData = repository.HoistWayDatas.FirstOrDefault(j => j.JobID == multiEditViewModel.CurrentJob.JobID);
                        CopyJobViewModel.SpecialFeatureslist = SfList;
                        CopyJobViewModel.POList = PoList;
                        CopyJobViewModel.JobTypeName = JobTypeName(CopyJobViewModel.CurrentJob.JobTypeID);
                        CopyJobViewModel.CurrentTab = "Main";
                        TempData["message"] = $"{CopyJobViewModel.CurrentJob.JobNum} ID has been saved...{CopyJobViewModel.CurrentJob.JobID}";
                        return RedirectToAction("Edit", new { id = multiEditViewModel.CurrentJob.JobID });
                    }
                }
                catch (DbUpdateException e)
                {
                    TempData["message"] = $"That PO already exists. Please validate.";
                    TempData["alert"] = $"alert-danger";
                }

                multiEditViewModel.CurrentTab = "Main";
                multiEditViewModel.JobTypeName = JobTypeName(multiEditViewModel.CurrentJob.JobTypeID);
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

        [HttpPost]
        public IActionResult EditHydro(JobElementHydroViewModel multiEditViewModel)
        {
            AppUser currentUser = GetCurrentUser().Result;
            multiEditViewModel.SpecialFeaturesTable = getSpecialFeaturesEX();
            multiEditViewModel.CurrentUserID = currentUser.EngID;
            string StatusAux = "Cross Approval Complete";
            if (multiEditViewModel.CurrentJob.Status == "Copied") StatusAux = "Copied";
            if (ModelState.IsValid)
            {
                if (multiEditViewModel.CurrentJob.Status == "Copied")
                {
                    multiEditViewModel.CurrentJob.EngID = currentUser.EngID;
                    List<PO> PoAux = new List<PO>();
                    foreach (PO itemes in multiEditViewModel.POList)
                    {
                        try
                        {
                            if (multiEditViewModel.POList[0].PONumb != itemes.PONumb || multiEditViewModel.POList[0].POID == 0)
                            {
                                if (itemes.JobID == 0)
                                {
                                    PO poUniqueAUx = repository.POs.FirstOrDefault(m => m.PONumb == itemes.PONumb);
                                    PoAux.Add(poUniqueAUx);
                                }
                            }
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                    if (PoAux[0] != null || PoAux.Count > 1 )
                    {
                        TempData["message"] = $"That PO already exists. Please validate.";
                        TempData["alert"] = $"alert-danger";
                        multiEditViewModel.CurrentTab = "Main";
                        return View(multiEditViewModel);
                    }
                    multiEditViewModel.CurrentJob.JobID = 0;
                    multiEditViewModel.CurrentJob.Status = "Cross Approval Complete";
                    repository.SaveJob(multiEditViewModel.CurrentJob);
                    multiEditViewModel.CurrentJob = repository.Jobs.LastOrDefault();
                    JobAdditional jobAdditional = new JobAdditional
                    {
                        JobID = multiEditViewModel.CurrentJob.JobID,
                        Status = "",
                        Action = "",
                        Priority = 0,
                        ERDate = multiEditViewModel.CurrentJob.LatestFinishDate
                    };
                    repository.SaveJobAdditional(jobAdditional);
                    multiEditViewModel.Element.JobID = multiEditViewModel.CurrentJob.JobID;
                    multiEditViewModel.ElementHydro.JobID = multiEditViewModel.CurrentJob.JobID;
                    multiEditViewModel.Element.ElementID = 0;
                    multiEditViewModel.ElementHydro.ElementHydroID = 0;
                    foreach (PO singlPO in multiEditViewModel.POList)
                    {
                        singlPO.POID = 0;
                        singlPO.JobID = multiEditViewModel.CurrentJob.JobID;
                    }
                    foreach (SpecialFeatures special in multiEditViewModel.SpecialFeatureslist)
                    {
                        special.SpecialFeaturesID = 0;
                        special.JobID = multiEditViewModel.CurrentJob.JobID;
                    }
                    multiEditViewModel.SpecialFeatureslist = multiEditViewModel.SpecialFeatureslist;
                }
                if (multiEditViewModel.CurrentJob.Status == "" || multiEditViewModel.CurrentJob.Status == null || multiEditViewModel.CurrentJob.Status == "Copied") multiEditViewModel.CurrentJob.Status = "Cross Approval Complete";

                repository.SaveEngElementHydroJobView(multiEditViewModel);
                JobElementHydroViewModel CopyJobViewModel = new JobElementHydroViewModel();
                if (StatusAux == "Copied")
                {
                    List<SpecialFeatures> SfList = repository.SpecialFeatures.Where(j => j.JobID == multiEditViewModel.CurrentJob.JobID).ToList();
                    List<PO> PoList = repository.POs.Where(j => j.JobID == multiEditViewModel.CurrentJob.JobID).ToList();
                    CopyJobViewModel.CurrentJob = multiEditViewModel.CurrentJob;
                    CopyJobViewModel.Element = repository.Elements.FirstOrDefault(j => j.JobID == multiEditViewModel.CurrentJob.JobID);
                    CopyJobViewModel.ElementHydro = repository.ElementHydros.FirstOrDefault(j => j.JobID == multiEditViewModel.CurrentJob.JobID);
                    CopyJobViewModel.SpecialFeatureslist = SfList;
                    CopyJobViewModel.POList = PoList;
                    CopyJobViewModel.CurrentTab = "Main";
                    TempData["message"] = $"{CopyJobViewModel.CurrentJob.JobNum} ID has been saved...{CopyJobViewModel.CurrentJob.JobID}";
                    return RedirectToAction("Edit", new { id = multiEditViewModel.CurrentJob.JobID, buttonAction = "ElmHydro" });
                }

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

        [HttpPost]
        public IActionResult EditTraction(JobElementTractionViewModel multiEditViewModel)
        {
            AppUser currentUser = GetCurrentUser().Result;
            multiEditViewModel.SpecialFeaturesTable = getSpecialFeaturesEX();
            multiEditViewModel.CurrentUserID = currentUser.EngID;
            string StatusAux = "Working on it";
            if (multiEditViewModel.CurrentJob.Status == "Copied") StatusAux = "Copied";
            if (ModelState.IsValid)
            {
                if (multiEditViewModel.CurrentJob.Status == "Copied")
                {
                    multiEditViewModel.CurrentJob.EngID = currentUser.EngID;
                    List<PO> PoAux = new List<PO>();
                    foreach (PO itemes in multiEditViewModel.POList)
                    {
                        try
                        {
                            if (multiEditViewModel.POList[0].PONumb != itemes.PONumb || multiEditViewModel.POList[0].POID == 0)
                            {
                                if (itemes.JobID == 0)
                                {
                                    PO poUniqueAUx = repository.POs.FirstOrDefault(m => m.PONumb == itemes.PONumb);
                                    PoAux.Add(poUniqueAUx);
                                }
                            }
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                    if (PoAux[0] != null || PoAux.Count > 1)
                    {
                        TempData["message"] = $"That PO already exists. Please validate.";
                        TempData["alert"] = $"alert-danger";
                        multiEditViewModel.CurrentTab = "Main";
                        return View(multiEditViewModel);
                    }
                    multiEditViewModel.CurrentJob.JobID = 0;
                    multiEditViewModel.CurrentJob.Status = "Cross Approval Complete";
                    repository.SaveJob(multiEditViewModel.CurrentJob);
                    multiEditViewModel.CurrentJob = repository.Jobs.LastOrDefault();
                    JobAdditional jobAdditional = new JobAdditional
                    {
                        JobID = multiEditViewModel.CurrentJob.JobID,
                        Status = "",
                        Action = "",
                        Priority = 0,
                        ERDate = multiEditViewModel.CurrentJob.LatestFinishDate
                    };
                    repository.SaveJobAdditional(jobAdditional);
                    multiEditViewModel.Element.JobID = multiEditViewModel.CurrentJob.JobID;
                    multiEditViewModel.ElementTraction.JobID = multiEditViewModel.CurrentJob.JobID;
                    multiEditViewModel.Element.ElementID = 0;
                    multiEditViewModel.ElementTraction.ElementTractionID = 0;
                    foreach (PO singlPO in multiEditViewModel.POList)
                    {
                        singlPO.POID = 0;
                        singlPO.JobID = multiEditViewModel.CurrentJob.JobID;
                    }
                    foreach (SpecialFeatures special in multiEditViewModel.SpecialFeatureslist)
                    {
                        special.SpecialFeaturesID = 0;
                        special.JobID = multiEditViewModel.CurrentJob.JobID;
                    }
                    multiEditViewModel.SpecialFeatureslist = multiEditViewModel.SpecialFeatureslist;
                }

                if (multiEditViewModel.CurrentJob.Status == "" || multiEditViewModel.CurrentJob.Status == null || multiEditViewModel.CurrentJob.Status == "Copied")
                {
                    multiEditViewModel.CurrentJob.Status = "Working on it";
                }
                try
                {
                    repository.SaveEngElementTractionJobView(multiEditViewModel);
                    JobElementTractionViewModel CopyJobViewModel = new JobElementTractionViewModel();
                    if (StatusAux == "Copied")
                    {

                        List<SpecialFeatures> SfList = repository.SpecialFeatures.Where(j => j.JobID == multiEditViewModel.CurrentJob.JobID).ToList();
                        List<PO> PoList = repository.POs.Where(j => j.JobID == multiEditViewModel.CurrentJob.JobID).ToList();
                        CopyJobViewModel.CurrentJob = multiEditViewModel.CurrentJob;
                        CopyJobViewModel.Element = repository.Elements.FirstOrDefault(j => j.JobID == multiEditViewModel.CurrentJob.JobID);
                        CopyJobViewModel.ElementTraction = repository.ElementTractions.FirstOrDefault(j => j.JobID == multiEditViewModel.CurrentJob.JobID);
                        CopyJobViewModel.SpecialFeatureslist = SfList;
                        CopyJobViewModel.POList = PoList;
                        CopyJobViewModel.CurrentTab = "Main";
                        TempData["message"] = $"{CopyJobViewModel.CurrentJob.JobNum} ID has been saved...{CopyJobViewModel.CurrentJob.JobID}";
                        return RedirectToAction("Edit", new { id = multiEditViewModel.CurrentJob.JobID, buttonAction = "ElmTract" });
                    }
                }
                catch (DbUpdateException e)
                {
                    TempData["message"] = $"That PO already exists. Please validate.";
                    TempData["alert"] = $"alert-danger";
                }

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

        [HttpPost]
        public IActionResult AddPo(JobViewModel jobView)
        {
            AppUser currentUser = GetCurrentUser().Result;
            jobView.SpecialFeaturesTable = getSpecialFeaturesEX();

            if (jobView.CurrentJob.Status == null)
            {
                if (jobView.buttonAction == "AddPO")
                {
                    jobView.POList.Add(new PO { JobID = 0 });
                    jobView.CurrentTab = "Main";
                }

                return View("NewJob", jobView);

            }
            else if (jobView.CurrentJob.Status == "Incomplete")
            {
                jobView.CurrentUserID = currentUser.EngID;
                if (jobView.buttonAction == "AddPO")
                {
                    jobView.POList.Add(new PO { JobID = jobView.CurrentJob.JobID, POID = 0 });
                    if (jobView.CurrentJobExtension == null) jobView.CurrentJobExtension = new JobExtension();
                    if (jobView.CurrentHydroSpecific == null) jobView.CurrentHydroSpecific = new HydroSpecific();
                    if (jobView.CurrentGenericFeatures == null) jobView.CurrentGenericFeatures = new GenericFeatures();
                    if (jobView.CurrentIndicator == null) jobView.CurrentIndicator = new Indicator();
                    if (jobView.CurrentHoistWayData == null) jobView.CurrentHoistWayData = new HoistWayData();
                    if (jobView.SpecialFeatureslist == null) jobView.SpecialFeatureslist = new List<SpecialFeatures> { new SpecialFeatures() };
                    else
                    {
                        return View("NextForm", jobView);
                    }
                    jobView.fieldID = 0;
                }
                jobView.CurrentTab = "Main";
                return View("NextForm", jobView);
            }
            else
            {
                jobView.CurrentUserID = currentUser.EngID;
                if (jobView.buttonAction == "AddPO")
                {
                    jobView.POList.Add(new PO { JobID = jobView.CurrentJob.JobID, POID = 0 });
                    jobView.CurrentTab = "Main";
                    jobView.fieldID = 0;
                }
                return View("Edit", jobView);
            }

        }

        [HttpPost]
        public IActionResult AddPoHydro(JobElementHydroViewModel jobView)
        {
            AppUser currentUser = GetCurrentUser().Result;
            jobView.SpecialFeaturesTable = getSpecialFeaturesEX();
            if (jobView.CurrentJob.Status == "Incomplete")
            {
                jobView.CurrentUserID = currentUser.EngID;
                if (jobView.buttonAction == "AddPO")
                {
                    jobView.POList.Add(new PO { JobID = jobView.CurrentJob.JobID, POID = 0 });
                    if (jobView.Element == null) jobView.Element = new Element();
                    if (jobView.ElementHydro == null) jobView.ElementHydro = new ElementHydro();
                    if (jobView.SpecialFeatureslist == null) jobView.SpecialFeatureslist = new List<SpecialFeatures> { new SpecialFeatures() };
                    else
                    {
                        return View("NextFormHydro", jobView);
                    }
                    jobView.fieldID = 0;
                }
                jobView.CurrentTab = "Main";
                return View("NextFormHydro", jobView);
            }
            else
            {
                jobView.CurrentUserID = currentUser.EngID;
                if (jobView.buttonAction == "AddPO")
                {
                    jobView.POList.Add(new PO { JobID = jobView.CurrentJob.JobID, POID = 0 });
                    jobView.CurrentTab = "Main";
                    jobView.fieldID = 0;
                }
                return View("EditHydro", jobView);
            }

        }

        [HttpPost]
        public IActionResult AddPoTraction(JobElementTractionViewModel jobView)
        {
            AppUser currentUser = GetCurrentUser().Result;
            jobView.SpecialFeaturesTable = getSpecialFeaturesEX();
            if (jobView.CurrentJob.Status == "Incomplete")
            {
                jobView.CurrentUserID = currentUser.EngID;
                if (jobView.buttonAction == "AddPO")
                {
                    jobView.POList.Add(new PO { JobID = jobView.CurrentJob.JobID, POID = 0 });
                    if (jobView.Element == null) jobView.Element = new Element();
                    if (jobView.ElementTraction == null) jobView.ElementTraction = new ElementTraction();
                    if (jobView.SpecialFeatureslist == null) jobView.SpecialFeatureslist = new List<SpecialFeatures> { new SpecialFeatures() };
                    else
                    {
                        return View("NextFormTraction", jobView);
                    }
                    jobView.fieldID = 0;
                }
                jobView.CurrentTab = "Main";
                return View("NextFormTraction", jobView);
            }
            else
            {
                jobView.CurrentUserID = currentUser.EngID;
                if (jobView.buttonAction == "AddPO")
                {
                    jobView.POList.Add(new PO { JobID = jobView.CurrentJob.JobID, POID = 0 });
                    jobView.CurrentTab = "Main";
                    jobView.fieldID = 0;
                }
                return View("EditTraction", jobView);
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
            viewModel.SpecialFeaturesTable = getSpecialFeaturesEX();
            return View("Edit", viewModel);
        }

        [HttpPost]
        public IActionResult DeleteSF(int fieldID, JobViewModel viewModel)
        {
            AppUser currentUser = GetCurrentUser().Result;
            Job job = repository.Jobs.FirstOrDefault(j => j.JobID == viewModel.CurrentJob.JobID);
            JobExtension extension = repository.JobsExtensions.FirstOrDefault(p => p.JobID == job.JobID);
            HydroSpecific hydro = repository.HydroSpecifics.FirstOrDefault(p => p.JobID == job.JobID);
            GenericFeatures generic = repository.GenericFeaturesList.FirstOrDefault(p => p.JobID == job.JobID);
            Indicator indicator = repository.Indicators.FirstOrDefault(p => p.JobID == job.JobID);
            HoistWayData hoist = repository.HoistWayDatas.FirstOrDefault(p => p.JobID == job.JobID);
            List<PO> pOList = repository.POs.Where(m => m.JobID == job.JobID).ToList();
            if (extension == null) extension = new JobExtension();
            if (hydro == null) hydro = new HydroSpecific();
            if (generic == null) generic = new GenericFeatures();
            if (indicator == null) indicator = new Indicator();
            if (hoist == null) hoist = new HoistWayData();
            JobViewModel EditViewModel = new JobViewModel
            {
                CurrentJob = job,
                CurrentJobExtension = extension,
                CurrentHydroSpecific = hydro,
                CurrentGenericFeatures = generic,
                CurrentIndicator = indicator,
                CurrentHoistWayData = hoist,
                POList = pOList,
                CurrentTab = "SpecialFeatures",
                CurrentUserID = currentUser.EngID
            };
            List<SpecialFeatures> specialFeaturesList = repository.SpecialFeatures.Where(j => j.JobID == job.JobID).ToList();
            if (specialFeaturesList.Count <= 1)
            {
                SpecialFeatures NewSpecial = new SpecialFeatures
                {
                    JobID = job.JobID,
                    Description = ""
                };
                repository.SaveSpecialFeatures(NewSpecial);
            }
            EditViewModel.SpecialFeaturesTable = getSpecialFeaturesEX();
            SpecialFeatures deletedField = repository.DeleteSpecialFeatures(fieldID);
            if (deletedField != null)
            {
                TempData["message"] = $"{deletedField.SpecialFeaturesID} was deleted";
                if (job.Status == "Incomplete")
                {
                    EditViewModel.SpecialFeatureslist = specialFeaturesList.Where(d => d.Description != null).ToList();
                    return View("Continue", EditViewModel);
                }
                else
                {
                    List<SpecialFeatures> NewspecialFeaturesList = repository.SpecialFeatures.Where(j => j.JobID == job.JobID).ToList();
                    EditViewModel.SpecialFeatureslist = NewspecialFeaturesList;
                    return View("Edit", EditViewModel);
                }
            }
            else
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"There was an error with your request";

                if (job.Status == "Incomplete")
                {
                    EditViewModel.SpecialFeatureslist = specialFeaturesList.Where(d => d.Description != null).ToList();
                    return View("Continue", EditViewModel);
                }
                else
                {
                    List<SpecialFeatures> NewspecialFeaturesList = repository.SpecialFeatures.Where(j => j.JobID == job.JobID).ToList();
                    EditViewModel.SpecialFeatureslist = NewspecialFeaturesList;
                    return View("Edit", EditViewModel);
                }
            }
        }

        [HttpPost]
        public IActionResult DeleteSFHydro(int fieldID, JobElementHydroViewModel viewModel)
        {
            AppUser currentUser = GetCurrentUser().Result;
            Job job = repository.Jobs.FirstOrDefault(j => j.JobID == viewModel.CurrentJob.JobID);
            Element element = repository.Elements.FirstOrDefault(p => p.JobID == job.JobID);
            ElementHydro hydro = repository.ElementHydros.FirstOrDefault(p => p.JobID == job.JobID);
            List<PO> pOList = repository.POs.Where(m => m.JobID == job.JobID).ToList();
            if (element == null) element = new Element();
            if (hydro == null) hydro = new ElementHydro();
            JobElementHydroViewModel EditViewModel = new JobElementHydroViewModel
            {
                CurrentJob = job,
                Element = element,
                ElementHydro = hydro,
                POList = pOList,
                CurrentTab = "SpecialFeatures",
                CurrentUserID = currentUser.EngID
            };
            EditViewModel.SpecialFeaturesTable = getSpecialFeaturesEX();
            List<SpecialFeatures> specialFeaturesList = repository.SpecialFeatures.Where(j => j.JobID == job.JobID).ToList();
            if (specialFeaturesList.Count <= 1)
            {
                SpecialFeatures NewSpecial = new SpecialFeatures
                {
                    JobID = job.JobID,
                    Description = ""
                };
                repository.SaveSpecialFeatures(NewSpecial);
            }

            SpecialFeatures deletedField = repository.DeleteSpecialFeatures(fieldID);
            if (deletedField != null)
            {
                TempData["message"] = $"{deletedField.SpecialFeaturesID} was deleted";
                if (job.Status == "Incomplete")
                {
                    EditViewModel.SpecialFeatureslist = specialFeaturesList.Where(d => d.Description != null).ToList();
                    return View("Continue", EditViewModel);
                }
                else
                {
                    List<SpecialFeatures> NewspecialFeaturesList = repository.SpecialFeatures.Where(j => j.JobID == job.JobID).ToList();
                    EditViewModel.SpecialFeatureslist = NewspecialFeaturesList;
                    return View("EditHydro", EditViewModel);
                }
            }
            else
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"There was an error with your request";

                if (job.Status == "Incomplete")
                {
                    EditViewModel.SpecialFeatureslist = specialFeaturesList.Where(d => d.Description != null).ToList();
                    return View("Continue", EditViewModel);
                }
                else
                {
                    List<SpecialFeatures> NewspecialFeaturesList = repository.SpecialFeatures.Where(j => j.JobID == job.JobID).ToList();
                    EditViewModel.SpecialFeatureslist = NewspecialFeaturesList;
                    return View("EditHydro", EditViewModel);
                }
            }
        }

        [HttpPost]
        public IActionResult DeleteSFTraction(int fieldID, JobElementTractionViewModel viewModel)
        {
            AppUser currentUser = GetCurrentUser().Result;
            Job job = repository.Jobs.FirstOrDefault(j => j.JobID == viewModel.CurrentJob.JobID);
            Element element = repository.Elements.FirstOrDefault(p => p.JobID == job.JobID);
            ElementTraction traction = repository.ElementTractions.FirstOrDefault(p => p.JobID == job.JobID);
            List<PO> pOList = repository.POs.Where(m => m.JobID == job.JobID).ToList();
            if (element == null) element = new Element();
            if (traction == null) traction = new ElementTraction();
            JobElementTractionViewModel EditViewModel = new JobElementTractionViewModel
            {
                CurrentJob = job,
                Element = element,
                ElementTraction = traction,
                POList = pOList,
                CurrentTab = "SpecialFeatures",
                CurrentUserID = currentUser.EngID
            };
            EditViewModel.SpecialFeaturesTable = getSpecialFeaturesEX();
            List<SpecialFeatures> specialFeaturesList = repository.SpecialFeatures.Where(j => j.JobID == job.JobID).ToList();
            if (specialFeaturesList.Count <= 1)
            {
                SpecialFeatures NewSpecial = new SpecialFeatures
                {
                    JobID = job.JobID,
                    Description = ""
                };
                repository.SaveSpecialFeatures(NewSpecial);
            }

            SpecialFeatures deletedField = repository.DeleteSpecialFeatures(fieldID);
            if (deletedField != null)
            {
                TempData["message"] = $"{deletedField.SpecialFeaturesID} was deleted";
                if (job.Status == "Incomplete")
                {
                    EditViewModel.SpecialFeatureslist = specialFeaturesList.Where(d => d.Description != null).ToList();
                    return View("Continue", EditViewModel);
                }
                else
                {
                    List<SpecialFeatures> NewspecialFeaturesList = repository.SpecialFeatures.Where(j => j.JobID == job.JobID).ToList();
                    EditViewModel.SpecialFeatureslist = NewspecialFeaturesList;
                    return View("EditTraction", EditViewModel);
                }
            }
            else
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"There was an error with your request";

                if (job.Status == "Incomplete")
                {
                    EditViewModel.SpecialFeatureslist = specialFeaturesList.Where(d => d.Description != null).ToList();
                    return View("Continue", EditViewModel);
                }
                else
                {
                    List<SpecialFeatures> NewspecialFeaturesList = repository.SpecialFeatures.Where(j => j.JobID == job.JobID).ToList();
                    EditViewModel.SpecialFeatureslist = NewspecialFeaturesList;
                    return View("EditTraction", EditViewModel);
                }
            }
        }

        [HttpPost]
        public IActionResult DeletePOs(JobViewModel viewModel)
        {
            viewModel.SpecialFeaturesTable = getSpecialFeaturesEX();
            AppUser currentUser = GetCurrentUser().Result;
            PO deletedField = repository.DeletePO(viewModel.fieldID);
            Job job = repository.Jobs.FirstOrDefault(j => j.JobID == viewModel.CurrentJob.JobID);
            if (deletedField != null)
            {
                TempData["message"] = $"{deletedField.POID} was deleted";
                if (job.Status == null)
                {
                    TempData["message"] = $"The requested Job doesn't exist.";
                    return View("NewJob", viewModel);
                }
                else if (job.Status == "Incomplete")
                {
                    return RedirectToAction("Continue", new { id = job.JobID });
                }
                else
                {
                    return RedirectToAction("Edit", new { id = job.JobID });
                }
            }
            else if (job == null)
            {
                viewModel.CurrentJob.EngID = currentUser.EngID;
                viewModel.CurrentUserID = currentUser.EngID;
                List<PO> POs = viewModel.POList.Where(m => m.PONumb > 3000000 && m.PONumb < 4900000).ToList();
                viewModel.POList = POs;
                return View("NewJob", viewModel);
            }
            else
            {
                if (job.Status == null)
                {
                    viewModel.CurrentJob.EngID = currentUser.EngID;
                    viewModel.CurrentUserID = currentUser.EngID;
                    TempData["message"] = $"The requested Job doesn't exist.";
                    return View("NewJob", viewModel);
                }
                else if (job.Status == "Incomplete")
                {
                    return RedirectToAction("Continue", new { id = job.JobID });
                }
                else
                {
                    return RedirectToAction("Edit", new { id = job.JobID });
                }

            }
        }

        /* Get de Continue; esta clase es para cuando el usuario estaba capturando un job y por algun motivo no lo termino;
         * recibe un ID y busca los objetos relacionados a este, en caso de encontrar un Job con este ID regresa un JobViewModel a NextForm con los 
         * objetos que concuerdan con el ID, de lo contrario manda a List con un mensaje de Failure
         */
        public IActionResult Continue(int ID)
        {
            Job Job = repository.Jobs.FirstOrDefault(j => j.JobID == ID);
            if (Job != null)
            {
                if (JobTypeName(Job.JobTypeID) == "ElmHydro")
                {
                    List<SpecialFeatures> SfList = repository.SpecialFeatures.Where(j => j.JobID == ID).ToList();
                    List<PO> POsList = repository.POs.Where(j => j.JobID == ID).ToList();
                    JobElementHydroViewModel continueJobViewModel = new JobElementHydroViewModel();
                    AppUser currentUser = GetCurrentUser().Result;
                    continueJobViewModel.CurrentUserID = currentUser.EngID;
                    continueJobViewModel.CurrentTab = "Main";
                    continueJobViewModel.CurrentJob = Job;
                    string LastFive = continueJobViewModel.CurrentJob.JobNum.ToString().Substring(5);
                    string FirstTwo = LastFive.Substring(0, 2);
                    continueJobViewModel.JobFolder = @"L:\" + FirstTwo + "000\\" + LastFive;
                    if (POsList != null) continueJobViewModel.POList = POsList;
                    else continueJobViewModel.POList = new List<PO> { new PO() };
                    continueJobViewModel.Element = (repository.Elements.FirstOrDefault(j => j.JobID == ID) ?? new Element());
                    continueJobViewModel.ElementHydro = (repository.ElementHydros.FirstOrDefault(j => j.JobID == ID) ?? new ElementHydro());
                    if (SfList.Count > 1) continueJobViewModel.SpecialFeatureslist = SfList;
                    else continueJobViewModel.SpecialFeatureslist = new List<SpecialFeatures> { new SpecialFeatures() };
                    continueJobViewModel.JobTypeName = JobTypeName(Job.JobTypeID);
                    continueJobViewModel.SpecialFeaturesTable = getSpecialFeaturesEX();

                    return View("NextFormHydro", continueJobViewModel);
                }
                else if (JobTypeName(Job.JobTypeID) == "ElmTract")
                {
                    List<SpecialFeatures> SfList = repository.SpecialFeatures.Where(j => j.JobID == ID).ToList();
                    List<PO> POsList = repository.POs.Where(j => j.JobID == ID).ToList();
                    JobElementTractionViewModel continueJobViewModel = new JobElementTractionViewModel();
                    AppUser currentUser = GetCurrentUser().Result;
                    continueJobViewModel.CurrentUserID = currentUser.EngID;
                    continueJobViewModel.CurrentTab = "Main";
                    continueJobViewModel.CurrentJob = repository.Jobs.FirstOrDefault(j => j.JobID == ID);
                    string LastFive = continueJobViewModel.CurrentJob.JobNum.ToString().Substring(5);
                    string FirstTwo = LastFive.Substring(0, 2);
                    continueJobViewModel.JobFolder = @"L:\" + FirstTwo + "000\\" + LastFive;
                    if (POsList != null) continueJobViewModel.POList = POsList;
                    else continueJobViewModel.POList = new List<PO> { new PO() };
                    continueJobViewModel.Element = (repository.Elements.FirstOrDefault(j => j.JobID == ID) ?? new Element());
                    continueJobViewModel.ElementTraction = (repository.ElementTractions.FirstOrDefault(j => j.JobID == ID) ?? new ElementTraction());
                    if (SfList.Count > 1) continueJobViewModel.SpecialFeatureslist = SfList;
                    else continueJobViewModel.SpecialFeatureslist = new List<SpecialFeatures> { new SpecialFeatures() };
                    continueJobViewModel.JobTypeName = JobTypeName(Job.JobTypeID);
                    continueJobViewModel.SpecialFeaturesTable = getSpecialFeaturesEX();

                    return View("NextFormTraction", continueJobViewModel);
                }
                else
                {
                    List<SpecialFeatures> SfList = repository.SpecialFeatures.Where(j => j.JobID == ID).ToList();
                    List<PO> POsList = repository.POs.Where(j => j.JobID == ID).ToList();
                    JobViewModel continueJobViewModel = new JobViewModel();
                    AppUser currentUser = GetCurrentUser().Result;
                    continueJobViewModel.CurrentUserID = currentUser.EngID;
                    continueJobViewModel.CurrentTab = "Main";
                    continueJobViewModel.CurrentJob = repository.Jobs.FirstOrDefault(j => j.JobID == ID);
                    string LastFive = continueJobViewModel.CurrentJob.JobNum.ToString().Substring(5);
                    string FirstTwo = LastFive.Substring(0, 2);
                    continueJobViewModel.JobFolder = @"L:\" + FirstTwo + "000\\" + LastFive;
                    if (POsList != null) continueJobViewModel.POList = POsList;
                    else continueJobViewModel.POList = new List<PO> { new PO() };
                    continueJobViewModel.CurrentJobExtension = (repository.JobsExtensions.FirstOrDefault(j => j.JobID == ID) ?? new JobExtension());
                    continueJobViewModel.CurrentHydroSpecific = (repository.HydroSpecifics.FirstOrDefault(j => j.JobID == ID) ?? new HydroSpecific());
                    continueJobViewModel.CurrentGenericFeatures = (repository.GenericFeaturesList.FirstOrDefault(j => j.JobID == ID) ?? new GenericFeatures());
                    continueJobViewModel.CurrentIndicator = (repository.Indicators.FirstOrDefault(j => j.JobID == ID) ?? new Indicator());
                    continueJobViewModel.CurrentHoistWayData = (repository.HoistWayDatas.FirstOrDefault(j => j.JobID == ID) ?? new HoistWayData());
                    if (SfList.Count > 1) continueJobViewModel.SpecialFeatureslist = SfList;
                    else continueJobViewModel.SpecialFeatureslist = new List<SpecialFeatures> { new SpecialFeatures() };
                    continueJobViewModel.JobTypeName = JobTypeName(Job.JobTypeID);
                    continueJobViewModel.SpecialFeaturesTable = getSpecialFeaturesEX();

                    return View("NextForm", continueJobViewModel);
                }


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
            nextViewModel.SpecialFeaturesTable = getSpecialFeaturesEX();
            List<PO> PoAux = new List<PO>();
            foreach (PO itemes in nextViewModel.POList)
            {
                try
                {
                    if (nextViewModel.POList[0].PONumb != itemes.PONumb)
                    {
                        if (itemes.JobID == 0)
                        {
                            PO poUniqueAUx = repository.POs.FirstOrDefault(m => m.PONumb == itemes.PONumb);
                            PoAux.Add(poUniqueAUx);
                        }
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }
            if (PoAux.Count <= 0 || PoAux[0] == null || nextViewModel.CurrentJobExtension.JobExtensionID == 0)
            {
                if (nextViewModel.CurrentJob.JobID == 0 && nextViewModel.CurrentJob.Status == "Incomplete") nextViewModel.CurrentJob.JobID = nextViewModel.CurrentJobExtension.JobID;
                if (nextViewModel.buttonAction == "AddSF")
                {
                    if(nextViewModel.SpecialFeatureslist != null)
                    {
                        if (!string.IsNullOrWhiteSpace(nextViewModel.SpecialFeatureslist.Last().Description))
                        {
                            nextViewModel.SpecialFeatureslist.Add(new SpecialFeatures { JobID = nextViewModel.CurrentJob.JobID, SpecialFeaturesID = 0 });
                            nextViewModel.CurrentTab = "SpecialFeatures";
                        }
                        else
                        {
                            nextViewModel.CurrentTab = "SpecialFeatures";
                            TempData["alert"] = $"alert-danger";
                            TempData["message"] = $"Fill the previus field on Special Features";
                            return View(nextViewModel);
                        }
                    }
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        if (nextViewModel.CurrentJobExtension != null && nextViewModel.CurrentJobExtension.JobID != 0)
                        {
                            string LastFive = nextViewModel.CurrentJob.JobNum.ToString().Substring(5);
                            string FirstTwo = LastFive.Substring(0, 2);
                            nextViewModel.JobFolder = @"L:\" + FirstTwo + "000\\" + LastFive;

                            if (nextViewModel.CurrentGenericFeatures != null && nextViewModel.CurrentGenericFeatures.JobID != 0)
                            {
                                if (nextViewModel.CurrentHydroSpecific != null && nextViewModel.CurrentHydroSpecific.JobID != 0)
                                {
                                    if (nextViewModel.CurrentIndicator != null && nextViewModel.CurrentIndicator.JobID != 0)
                                    {
                                        if (nextViewModel.CurrentHoistWayData != null && nextViewModel.CurrentHoistWayData.JobID != 0)
                                        {
                                            if (nextViewModel.SpecialFeatureslist != null)
                                            {   
                                                if(nextViewModel.CurrentJob.Status == "Incomplete") nextViewModel.CurrentJob.Status = "Working on it";
                                                repository.SaveEngJobView(nextViewModel);
                                                nextViewModel.CurrentTab = "Main";
                                                TempData["message"] = $"everything was saved";
                                                // Here the Job Filling Status should be changed the Working on it
                                                // Redirect to Hub??
                                                List<SpecialFeatures> NewspecialFeaturesList = repository.SpecialFeatures.Where(j => j.JobID == nextViewModel.CurrentJob.JobID).ToList();
                                                nextViewModel.SpecialFeatureslist = NewspecialFeaturesList;
                                                TempData["message"] = $"everything was saved";
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
                                        if (nextViewModel.CurrentHydroSpecific.BatteryBrand == "Other" && !string.IsNullOrEmpty(nextViewModel.CurrentHydroSpecific.OtherBatteryBrand))
                                        {
                                            nextViewModel.CurrentHydroSpecific.BatteryBrand = nextViewModel.CurrentHydroSpecific.OtherBatteryBrand;
                                        }

                                        if (nextViewModel.CurrentHydroSpecific.ValveBrand == "Other" && !string.IsNullOrEmpty(nextViewModel.CurrentHydroSpecific.OtherValveBrand))
                                        {
                                            nextViewModel.CurrentHydroSpecific.ValveBrand = nextViewModel.CurrentHydroSpecific.OtherValveBrand;
                                        }
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
                                        TempData["message"] = $"hydro specific was saved";
                                        return View(nextViewModel);
                                    }
                                }
                                else
                                {
                                    repository.SaveEngJobView(nextViewModel);
                                    nextViewModel.CurrentHydroSpecific = new HydroSpecific { JobID = nextViewModel.CurrentJob.JobID };
                                    nextViewModel.CurrentIndicator = new Indicator();
                                    nextViewModel.CurrentHoistWayData = new HoistWayData();
                                    nextViewModel.SpecialFeatureslist = new List<SpecialFeatures> { new SpecialFeatures() };
                                    nextViewModel.CurrentTab = "HydroSpecifics";
                                    TempData["message"] = $"generic was saved";
                                    return View(nextViewModel);
                                }
                            }
                            else
                            {
                                repository.SaveEngJobView(nextViewModel);
                                nextViewModel.CurrentGenericFeatures = new GenericFeatures { JobID = nextViewModel.CurrentJob.JobID };
                                nextViewModel.CurrentHydroSpecific = new HydroSpecific();
                                nextViewModel.CurrentIndicator = new Indicator();
                                nextViewModel.CurrentHoistWayData = new HoistWayData();
                                nextViewModel.SpecialFeatureslist = new List<SpecialFeatures> { new SpecialFeatures() };
                                nextViewModel.CurrentTab = "GenericFeatures";
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
                        nextViewModel.POList = (nextViewModel.POList ?? new List<PO> { new PO() });
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
            }
            else
            {
                nextViewModel.CurrentJob = (nextViewModel.CurrentJob ?? new Job());
                nextViewModel.POList = (nextViewModel.POList ?? new List<PO> { new PO() });
                nextViewModel.CurrentJobExtension = (nextViewModel.CurrentJobExtension ?? new JobExtension());
                nextViewModel.CurrentHydroSpecific = (nextViewModel.CurrentHydroSpecific ?? new HydroSpecific());
                nextViewModel.CurrentGenericFeatures = (nextViewModel.CurrentGenericFeatures ?? new GenericFeatures());
                nextViewModel.CurrentIndicator = (nextViewModel.CurrentIndicator ?? new Indicator());
                nextViewModel.CurrentHoistWayData = (nextViewModel.CurrentHoistWayData ?? new HoistWayData());
                nextViewModel.SpecialFeatureslist = (nextViewModel.SpecialFeatureslist ?? new List<SpecialFeatures> { new SpecialFeatures() });
                TempData["message"] = $"nothing was saved";
                return View(nextViewModel);
            }


            return View(nextViewModel);
        }

        [HttpPost]
        public IActionResult NextFormHydro(JobElementHydroViewModel nextViewModel)
        {
            AppUser currentUser = GetCurrentUser().Result;
            nextViewModel.CurrentUserID = currentUser.EngID;
            nextViewModel.SpecialFeaturesTable = getSpecialFeaturesEX();
            List<PO> PoAux = new List<PO>();
            foreach (PO itemes in nextViewModel.POList)
            {
                try
                {
                    if (nextViewModel.POList[0].PONumb != itemes.PONumb)
                    {
                        if (itemes.JobID == 0)
                        {
                            PO poUniqueAUx = repository.POs.FirstOrDefault(m => m.PONumb == itemes.PONumb);
                            PoAux.Add(poUniqueAUx);
                        }
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }
            if (PoAux.Count <= 0 || PoAux[0] == null)
            {
                if (nextViewModel.CurrentJob.JobID == 0 && nextViewModel.CurrentJob.Status == "Incomplete") nextViewModel.CurrentJob.JobID = nextViewModel.Element.JobID;
                if (nextViewModel.buttonAction == "AddSF")
                {
                    if (!string.IsNullOrWhiteSpace(nextViewModel.SpecialFeatureslist.Last().Description))
                    {
                        nextViewModel.SpecialFeatureslist.Add(new SpecialFeatures { JobID = nextViewModel.CurrentJob.JobID, SpecialFeaturesID = 0 });
                        nextViewModel.CurrentTab = "SpecialFeatures";
                    }
                    else
                    {
                        nextViewModel.CurrentTab = "SpecialFeatures";
                        TempData["alert"] = $"alert-danger";
                        TempData["message"] = $"Fill the previus field on Special Features";
                        return View(nextViewModel);
                    }
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        if (nextViewModel.Element != null && nextViewModel.Element.JobID != 0)
                        {
                            if (nextViewModel.ElementHydro != null && nextViewModel.ElementHydro.JobID != 0)
                            {
                                if (nextViewModel.Element.ElementID == 0 && nextViewModel.CurrentJob.Status == "Incomplete") nextViewModel.Element.ElementID = repository.Elements.FirstOrDefault(m => m.JobID == nextViewModel.CurrentJob.JobID).ElementID;

                                if (nextViewModel.SpecialFeatureslist != null)
                                {
                                    if (nextViewModel.CurrentJob.Status == "Incomplete") nextViewModel.CurrentJob.Status = "Cross Approval Complete";
                                    repository.SaveEngElementHydroJobView(nextViewModel);
                                    nextViewModel.CurrentTab = "Main";
                                    TempData["message"] = $"everything was saved";
                                    // Here the Job Filling Status should be changed the Working on it
                                    // Redirect to Hub??
                                    List<SpecialFeatures> NewspecialFeaturesList = repository.SpecialFeatures.Where(j => j.JobID == nextViewModel.CurrentJob.JobID).ToList();
                                    nextViewModel.SpecialFeatureslist = NewspecialFeaturesList;
                                    TempData["message"] = $"everything was saved";
                                    return View(nextViewModel);
                                }
                                else
                                {
                                    repository.SaveEngElementHydroJobView(nextViewModel);
                                    nextViewModel.CurrentTab = "SpecialFeatures";
                                    TempData["message"] = $"ElementHydro info was saved";
                                    nextViewModel.SpecialFeatureslist = new List<SpecialFeatures> { new SpecialFeatures { JobID = nextViewModel.CurrentJob.JobID } };
                                    return View(nextViewModel);
                                }

                            }
                            else
                            {
                                repository.SaveEngElementHydroJobView(nextViewModel);
                                nextViewModel.ElementHydro = new ElementHydro { JobID = nextViewModel.CurrentJob.JobID };
                                nextViewModel.SpecialFeatureslist = new List<SpecialFeatures> { new SpecialFeatures() };
                                nextViewModel.CurrentTab = "ElementHydro";
                                TempData["message"] = $"Element info was saved";
                                return View(nextViewModel);
                            }
                        }
                        else
                        {
                            repository.SaveEngElementHydroJobView(nextViewModel);
                            Element elem = repository.Elements.FirstOrDefault(j => j.JobID == nextViewModel.CurrentJob.JobID);
                            nextViewModel.Element = (elem ?? new Element { JobID = nextViewModel.CurrentJob.JobID });
                            nextViewModel.ElementHydro = new ElementHydro();
                            nextViewModel.SpecialFeatureslist = new List<SpecialFeatures> { new SpecialFeatures() };
                            nextViewModel.CurrentTab = "Element";
                            TempData["message"] = $"job was saved";
                            return View(nextViewModel);
                        }

                    }
                    nextViewModel.CurrentJob = (nextViewModel.CurrentJob ?? new Job());
                    nextViewModel.POList = (nextViewModel.POList ?? new List<PO> { new PO() });
                    nextViewModel.Element = (nextViewModel.Element ?? new Element());
                    nextViewModel.ElementHydro = (nextViewModel.ElementHydro ?? new ElementHydro());
                    nextViewModel.SpecialFeatureslist = (nextViewModel.SpecialFeatureslist ?? new List<SpecialFeatures> { new SpecialFeatures() });
                    TempData["message"] = $"nothing was saved";
                    return View(nextViewModel);
                }
            }
            else
            {
                nextViewModel.CurrentJob = (nextViewModel.CurrentJob ?? new Job());
                nextViewModel.POList = (nextViewModel.POList ?? new List<PO> { new PO() });
                nextViewModel.Element = (nextViewModel.Element ?? new Element());
                nextViewModel.ElementHydro = (nextViewModel.ElementHydro ?? new ElementHydro());
                nextViewModel.SpecialFeatureslist = (nextViewModel.SpecialFeatureslist ?? new List<SpecialFeatures> { new SpecialFeatures() });
                TempData["message"] = $"One of the POs already exists. Please validate.";
                TempData["alert"] = $"alert-danger";
                return View(nextViewModel);
            }
            return View(nextViewModel);
        }

        [HttpPost]
        public IActionResult NextFormTraction(JobElementTractionViewModel nextViewModel)
        {
            AppUser currentUser = GetCurrentUser().Result;
            nextViewModel.CurrentUserID = currentUser.EngID;
            nextViewModel.SpecialFeaturesTable = getSpecialFeaturesEX();
            List<PO> PoAux = new List<PO>();
            foreach (PO itemes in nextViewModel.POList)
            {
                try
                {
                    if (nextViewModel.POList[0].PONumb != itemes.PONumb)
                    {
                        if (itemes.JobID == 0)
                        {
                            PO poUniqueAUx = repository.POs.FirstOrDefault(m => m.PONumb == itemes.PONumb);
                            PoAux.Add(poUniqueAUx);
                        }
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }
            if (PoAux.Count <= 0 || PoAux[0] == null)
            {
                if (nextViewModel.CurrentJob.JobID == 0 && nextViewModel.CurrentJob.Status == "Incomplete") nextViewModel.CurrentJob.JobID = nextViewModel.Element.JobID;
                if (nextViewModel.buttonAction == "AddSF")
                {
                    if (!string.IsNullOrWhiteSpace(nextViewModel.SpecialFeatureslist.Last().Description))
                    {
                        nextViewModel.SpecialFeatureslist.Add(new SpecialFeatures { JobID = nextViewModel.CurrentJob.JobID, SpecialFeaturesID = 0 });
                        nextViewModel.CurrentTab = "SpecialFeatures";
                    }
                    else
                    {
                        nextViewModel.CurrentTab = "SpecialFeatures";
                        TempData["alert"] = $"alert-danger";
                        TempData["message"] = $"Fill the previus field on Special Features";
                        return View(nextViewModel);
                    }
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        if (nextViewModel.Element != null && nextViewModel.Element.JobID != 0)
                        {
                            if (nextViewModel.ElementTraction != null && nextViewModel.ElementTraction.JobID != 0)
                            {
                                if (nextViewModel.Element.ElementID == 0 && nextViewModel.CurrentJob.Status == "Incomplete") nextViewModel.Element.ElementID = repository.Elements.FirstOrDefault(m => m.JobID == nextViewModel.CurrentJob.JobID).ElementID;
                                if (nextViewModel.SpecialFeatureslist != null)
                                {
                                    if (nextViewModel.CurrentJob.Status == "Incomplete") nextViewModel.CurrentJob.Status = "Cross Approval Complete";
                                    repository.SaveEngElementTractionJobView(nextViewModel);
                                    nextViewModel.CurrentTab = "Main";
                                    TempData["message"] = $"everything was saved";
                                    // Here the Job Filling Status should be changed the Working on it
                                    // Redirect to Hub??
                                    List<SpecialFeatures> NewspecialFeaturesList = repository.SpecialFeatures.Where(j => j.JobID == nextViewModel.CurrentJob.JobID).ToList();
                                    nextViewModel.SpecialFeatureslist = NewspecialFeaturesList;
                                    TempData["message"] = $"everything was saved";
                                    return View(nextViewModel);
                                }
                                else
                                {
                                    repository.SaveEngElementTractionJobView(nextViewModel);
                                    nextViewModel.CurrentTab = "SpecialFeatures";
                                    TempData["message"] = $"Elemnent Traction info was saved";
                                    nextViewModel.SpecialFeatureslist = new List<SpecialFeatures> { new SpecialFeatures { JobID = nextViewModel.CurrentJob.JobID } };
                                    return View(nextViewModel);
                                }

                            }
                            else
                            {
                                repository.SaveEngElementTractionJobView(nextViewModel);
                                nextViewModel.ElementTraction = new ElementTraction { JobID = nextViewModel.CurrentJob.JobID };
                                nextViewModel.SpecialFeatureslist = new List<SpecialFeatures> { new SpecialFeatures() };
                                nextViewModel.CurrentTab = "ElementTraction";
                                TempData["message"] = $"Element Info was saved";
                                return View(nextViewModel);
                            }
                        }
                        else
                        {
                            repository.SaveEngElementTractionJobView(nextViewModel);
                            Element elem = repository.Elements.FirstOrDefault(j => j.JobID == nextViewModel.CurrentJob.JobID);
                            nextViewModel.Element = (elem ?? new Element { JobID = nextViewModel.CurrentJob.JobID });
                            nextViewModel.ElementTraction = new ElementTraction();
                            nextViewModel.SpecialFeatureslist = new List<SpecialFeatures> { new SpecialFeatures() };
                            nextViewModel.CurrentTab = "Element";
                            TempData["message"] = $"job was saved";
                            return View(nextViewModel);
                        }

                    }

                    nextViewModel.CurrentJob = (nextViewModel.CurrentJob ?? new Job());
                    nextViewModel.POList = (nextViewModel.POList ?? new List<PO> { new PO() });
                    nextViewModel.Element = (nextViewModel.Element ?? new Element());
                    nextViewModel.ElementTraction = (nextViewModel.ElementTraction ?? new ElementTraction());
                    nextViewModel.SpecialFeatureslist = (nextViewModel.SpecialFeatureslist ?? new List<SpecialFeatures> { new SpecialFeatures() });
                    TempData["message"] = $"nothing was saved";
                    return View(nextViewModel);
                }
            }
            else
            {
                nextViewModel.CurrentJob = (nextViewModel.CurrentJob ?? new Job());
                nextViewModel.POList = (nextViewModel.POList ?? new List<PO> { new PO() });
                nextViewModel.Element = (nextViewModel.Element ?? new Element());
                nextViewModel.ElementTraction = (nextViewModel.ElementTraction ?? new ElementTraction());
                nextViewModel.SpecialFeatureslist = (nextViewModel.SpecialFeatureslist ?? new List<SpecialFeatures> { new SpecialFeatures() });
                TempData["message"] = $"One of the POs already exists. Please validate.";
                TempData["alert"] = $"alert-danger";
                return View(nextViewModel);
            }


            return View(nextViewModel);
        }

        //Funcion para obtener el usuario que realizo la llamada
        private async Task<AppUser> GetCurrentUser()
        {
            AppUser user = await userManager.GetUserAsync(HttpContext.User);

            return user;
        }

        
       public async Task<IActionResult> JobSearchList(JobSearchViewModel searchViewModel, int page = 1)
        {
            if (searchViewModel.CleanFields) return RedirectToAction("JobSearchList");

            var JobCount = repository.Jobs.Count();
            var jobSearchRepo = repository.Jobs.Include(j => j._jobExtension).Include(hy => hy._HydroSpecific).Include(g => g._GenericFeatures)
                .Include(i => i._Indicator).Include(ho => ho._HoistWayData).Include(sp => sp._SpecialFeatureslist).Include(po => po._PO).Where(y => y.Status != "Pending").AsQueryable();
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
            if (searchViewModel.EngID > 0) jobSearchRepo = jobSearchRepo.Where(s => s.EngID == searchViewModel.EngID);
            if (searchViewModel.CrossAppEngID > 0) jobSearchRepo = jobSearchRepo.Where(s => s.CrossAppEngID == searchViewModel.CrossAppEngID);
            if (searchViewModel.CountryID > 0){
                IQueryable<State> states = itemsrepository.States.Where(m => m.CountryID == searchViewModel.CountryID);
                IQueryable<City> cities = itemsrepository.Cities.Where(m => states.Any( n => n.StateID ==  m.StateID));

                jobSearchRepo = jobSearchRepo.Where(m => cities.Any(n => n.CityID == m.CityID));
            }
            if (searchViewModel.StateID > 0)
            {
                IQueryable<City> cities = itemsrepository.Cities.Where(m => m.StateID == searchViewModel.StateID);
                jobSearchRepo = jobSearchRepo.Where(m => cities.Any(n => n.CityID == m.CityID));
            }
            if (searchViewModel.CityID > 0) jobSearchRepo = jobSearchRepo.Where(s => s.CityID == searchViewModel.CityID);
            if (searchViewModel.FireCodeID > 0) jobSearchRepo = jobSearchRepo.Where(s => s.FireCodeID == searchViewModel.FireCodeID);
            if (searchViewModel.JobTypeID > 0) jobSearchRepo = jobSearchRepo.Where(s => s.JobTypeID == searchViewModel.JobTypeID);
            if (searchViewModel.POJobSearch > 3000000 && searchViewModel.POJobSearch < 4900000)
            {
                jobSearchRepo = jobSearchRepo.Where(a => a._PO.Any(b => b.PONumb.Equals(searchViewModel.POJobSearch)));
            }

            if (!string.IsNullOrEmpty(searchViewModel.NameJobSearch)) jobSearchRepo = jobSearchRepo.Where(s => s.Name.Contains(searchViewModel.NameJobSearch));
            if (!string.IsNullOrEmpty(searchViewModel.Name2)) jobSearchRepo = jobSearchRepo.Where(s => s.Name2.Contains(searchViewModel.Name2));
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

            //if (!string.IsNullOrEmpty(searchViewModel.Battery)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.Battery == "Si" ? s._HydroSpecific.Battery == true : s._HydroSpecific.Battery == false);
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

            if (!string.IsNullOrEmpty(searchViewModel.CarCallCodeSecurity)) jobSearchRepo = jobSearchRepo.Where(s => s._GenericFeatures.CarCallCodeSecurity.Contains(searchViewModel.CarCallCodeSecurity));
            if (!string.IsNullOrEmpty(searchViewModel.BSI)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.BSI == "Si" ? s._GenericFeatures.BSI == true : s._GenericFeatures.BSI == false);

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
            if (!string.IsNullOrEmpty(searchViewModel.Pit)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.Pit == "Si" ? s._GenericFeatures.Pit == true : s._GenericFeatures.Pit == false);
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

            #region Indicators
            //Opciones de bsuqueda para el modelo de Indicators
           
            if (!string.IsNullOrEmpty(searchViewModel.CarPIType)) jobSearchRepo = jobSearchRepo.Where(s => s._Indicator.CarPIType.Equals(searchViewModel.CarPIType));
            if (!string.IsNullOrEmpty(searchViewModel.CarPIDiscreteType)) jobSearchRepo = jobSearchRepo.Where(s => s._Indicator.CarPIDiscreteType.Equals(searchViewModel.CarPIDiscreteType));
            if (!string.IsNullOrEmpty(searchViewModel.HallPIType)) jobSearchRepo = jobSearchRepo.Where(s => s._Indicator.HallPIType.Equals(searchViewModel.HallPIType));
            if (!string.IsNullOrEmpty(searchViewModel.HallPIDiscreteType)) jobSearchRepo = jobSearchRepo.Where(s => s._Indicator.HallPIDiscreteType.Equals(searchViewModel.HallPIDiscreteType));
            if (!string.IsNullOrEmpty(searchViewModel.VoiceAnnunciationPIType)) jobSearchRepo = jobSearchRepo.Where(s => s._Indicator.VoiceAnnunciationPIType.Equals(searchViewModel.VoiceAnnunciationPIType));
            if (!string.IsNullOrEmpty(searchViewModel.CarLanternsStyle)) jobSearchRepo = jobSearchRepo.Where(s => s._Indicator.CarLanternsStyle.Equals(searchViewModel.CarLanternsStyle));
            if (!string.IsNullOrEmpty(searchViewModel.CarLanternsType)) jobSearchRepo = jobSearchRepo.Where(s => s._Indicator.CarLanternsType.Equals(searchViewModel.CarLanternsType));
            if (!string.IsNullOrEmpty(searchViewModel.HallLanternsStyle)) jobSearchRepo = jobSearchRepo.Where(s => s._Indicator.HallLanternsStyle.Equals(searchViewModel.HallLanternsStyle));
            if (!string.IsNullOrEmpty(searchViewModel.HallLanternsType)) jobSearchRepo = jobSearchRepo.Where(s => s._Indicator.HallLanternsType.Equals(searchViewModel.HallLanternsType));
            if (!string.IsNullOrEmpty(searchViewModel.PassingFloorType)) jobSearchRepo = jobSearchRepo.Where(s => s._Indicator.PassingFloorType.Equals(searchViewModel.PassingFloorType));
            if (!string.IsNullOrEmpty(searchViewModel.PassingFloorDiscreteType)) jobSearchRepo = jobSearchRepo.Where(s => s._Indicator.PassingFloorDiscreteType.Equals(searchViewModel.PassingFloorDiscreteType));

            if (searchViewModel.IndicatorsVoltage > 0) jobSearchRepo = jobSearchRepo.Where(s => s._Indicator.IndicatorsVoltage == searchViewModel.IndicatorsVoltage);
            if (!string.IsNullOrEmpty(searchViewModel.IndicatorsVoltageType)) jobSearchRepo = jobSearchRepo.Where(s => s._Indicator.IndicatorsVoltageType.Equals(searchViewModel.IndicatorsVoltageType));

            if (!string.IsNullOrEmpty(searchViewModel.CarLanterns)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.CarLanterns == "Si" ? s._Indicator.CarLanterns == true : s._Indicator.CarLanterns == false);
            if (!string.IsNullOrEmpty(searchViewModel.HallLanterns)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.HallLanterns == "Si" ? s._Indicator.HallLanterns == true : s._Indicator.HallLanterns == false);
            if (!string.IsNullOrEmpty(searchViewModel.HallPI)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.HallPI == "Si" ? s._Indicator.HallPI == true : s._Indicator.HallPI == false);
            if (!string.IsNullOrEmpty(searchViewModel.PassingFloor)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.PassingFloor == "Si" ? s._Indicator.PassingFloor == true : s._Indicator.PassingFloor == false);
            if (!string.IsNullOrEmpty(searchViewModel.CarPI)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.CarPI == "Si" ? s._Indicator.CarPI == true : s._Indicator.CarPI == false);
            if (!string.IsNullOrEmpty(searchViewModel.HallPIAll)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.HallPIAll == "Si" ? s._Indicator.HallPIAll == true : s._Indicator.HallPIAll == false);
            if (!string.IsNullOrEmpty(searchViewModel.VoiceAnnunciationPI)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.VoiceAnnunciationPI == "Si" ? s._Indicator.VoiceAnnunciationPI == true : s._Indicator.VoiceAnnunciationPI == false);
            if (!string.IsNullOrEmpty(searchViewModel.PassingFloorEnable)) jobSearchRepo = jobSearchRepo.Where(s => searchViewModel.PassingFloorEnable == "Si" ? s._Indicator.PassingFloorEnable == true : s._Indicator.PassingFloorEnable == false);
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
            searchViewModel.Status = new SelectList(statusQuery.Distinct().ToList());
            searchViewModel.JobsSearchList = jobSearchRepo.OrderBy(p => p.JobID).Skip((page - 1) * 5).Take(5).ToList();
            searchViewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = page,
                ItemsPerPage = 5,
                TotalItems = jobSearchRepo.Count()
            };

            return View(searchViewModel);
        }

        //Funciones para el llenado de los dropdowns en casacada
        public JsonResult GetJobState(int CountryID)
        {
            List<State> JobStatelist = new List<State>();
            JobStatelist = (from state in itemsrepository.States where state.CountryID == CountryID select state).OrderBy(s => s.Name).ToList();
            return Json(new SelectList(JobStatelist, "StateID", "Name"));
        }

        public JsonResult GetJobCity(int StateID)
        {
            List<City> CityCascadeList = new List<City>();
            CityCascadeList = (from city in itemsrepository.Cities where city.StateID == StateID select city).OrderBy(s => s.Name).ToList();
            return Json(new SelectList(CityCascadeList, "CityID", "Name"));
        }

        public JsonResult GetCurrentFireCode(int CityID)
        {
            int FirecodeOncityID = itemsrepository.Cities.FirstOrDefault(m => m.CityID == CityID).FirecodeID;
            List<FireCode> FireCodeList = new List<FireCode>();
            FireCodeList = (from firecode in itemsrepository.FireCodes where firecode.FireCodeID == FirecodeOncityID select firecode).OrderBy(s => s.Name).ToList();
            return Json(new SelectList(FireCodeList, "FireCodeID", "Name"));
        }

        public JsonResult GetBrand(string Style)
        {
            List<DoorOperator> BrandList = new List<DoorOperator>();
            BrandList = itemsrepository.DoorOperators.FromSql("select * from dbo.DoorOperators where Style = {0} AND dbo.DoorOperators.DoorOperatorID in " +
                "(Select max(dbo.DoorOperators.DoorOperatorID) FROM dbo.DoorOperators group by dbo.DoorOperators.Brand)", Style).OrderBy(s => s.Brand).ToList();
            return Json(new SelectList(BrandList, "Brand", "Brand"));
        }

        public JsonResult GetDoorOperatorID(string Brand)
        {
            List<DoorOperator> DoorOperatorList = new List<DoorOperator>();
            DoorOperatorList = (from door in itemsrepository.DoorOperators where door.Brand == Brand select door).OrderBy(s => s.Name).ToList();
            return Json(new SelectList(DoorOperatorList, "DoorOperatorID", "Name"));
        }

        public JsonResult GetJobTypeAdd(string JobTypeMain)
        {
            IList<SelectList> JobTypeAddList = new List<SelectList>();

            if (JobTypeMain == "Simplex")
            {
                IList<SelectListItem> Simplex = new List<SelectListItem>
                {
                new SelectListItem{Text = " --- Please Select one --- ", Value = ""},
                new SelectListItem{Text = "Selective Collective", Value = "Selective Collective"},
                new SelectListItem{Text = "SAPB Single Automatic Pushbutton", Value = "SAPB Single Automatic Pushbutton"},
                new SelectListItem{Text = "SBC Single Button Collective", Value = "SBC Single Button Collective"}

                };

                return Json(new SelectList(Simplex, "Text", "Value"));
            }
            else if (JobTypeMain == "Duplex")
            {
                IList<SelectListItem> Duplex = new List<SelectListItem>
                {
                new SelectListItem{Text = "Duplex Operation", Value = "Duplex Operation"},

                };
                return Json(new SelectList(Duplex, "Text", "Value"));
            }
            else
            {
                IList<SelectListItem> Duplex = new List<SelectListItem>
                {
                new SelectListItem{Text = "Group Operation", Value = "Group Operation"},

                };
                return Json(new SelectList(Duplex, "Text", "Value"));
            }

        }

        [HttpPost]
        public FileStreamResult ExportUniqueJobToXML(int ID)
        {

            MemoryStream ms = new MemoryStream();
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.OmitXmlDeclaration = true;
            xws.Indent = true;

            using (XmlWriter xw = XmlWriter.Create(ms, xws))
            {
                xw.WriteStartDocument();
                xw.WriteStartElement("UniqueJob");
                if (repository.Jobs.Any())
                {
                    Job job = repository.Jobs.FirstOrDefault(m => m.JobID == ID);

                    if (job.Status != "Incomplete")
                    {
                        xw.WriteStartElement("Job");

                        xw.WriteStartElement("JobBase");
                        xw.WriteElementString("ID", job.JobID.ToString());
                        xw.WriteElementString("Status", job.Status);
                        xw.WriteElementString("EngID", job.EngID.ToString());
                        xw.WriteElementString("CrossAppEngID", job.CrossAppEngID.ToString());
                        xw.WriteElementString("Name", job.Name);
                        xw.WriteElementString("Name2", job.Name2);
                        xw.WriteElementString("JobNum", job.JobNum.ToString());
                        xw.WriteElementString("ShipDate", job.ShipDate.ToString());
                        xw.WriteElementString("LatestFinishDate", job.LatestFinishDate.ToString());
                        xw.WriteElementString("Cust", job.Cust);
                        xw.WriteElementString("Contractor", job.Contractor);
                        xw.WriteElementString("JobTypeID", job.JobTypeID.ToString());
                        xw.WriteElementString("CityID", job.CityID.ToString());
                        xw.WriteElementString("FireCodeID", job.FireCodeID.ToString());
                        xw.WriteEndElement();

                        List<PO> pOsList = repository.POs.Where(m => m.JobID == job.JobID).ToList();
                        if (pOsList.Count > 0)
                        {
                            xw.WriteStartElement("POs");
                            foreach (PO po in pOsList)
                            {
                                xw.WriteStartElement("PO");
                                xw.WriteElementString("ID", po.POID.ToString());
                                xw.WriteElementString("JobID", po.JobID.ToString());
                                xw.WriteElementString("PONumb", po.PONumb.ToString());
                                xw.WriteEndElement();
                            }
                            xw.WriteEndElement();
                        }

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
                            aux = jobExtension.SwingOp ? "True" : "False";
                            xw.WriteElementString("SwingOp", aux);
                            aux = jobExtension.BackUpDisp ? "True" : "False";
                            xw.WriteElementString("BackUpDisp", aux);
                            aux = jobExtension.AltRis ? "True" : "False";
                            xw.WriteElementString("AltRis", aux);
                            aux = jobExtension.InfDetector ? "True" : "False";
                            xw.WriteElementString("InfDetector", aux);
                            aux = jobExtension.MechSafEdge ? "True" : "False";
                            xw.WriteElementString("MechSafEdge", aux);
                            aux = jobExtension.HeavyDoors ? "True" : "False";
                            xw.WriteElementString("HeavyDoors", aux);
                            aux = jobExtension.CartopDoorButtons ? "True" : "False";
                            xw.WriteElementString("CartopDoorButtons", aux);
                            aux = jobExtension.DoorHold ? "True" : "False";
                            xw.WriteElementString("DoorHold", aux);
                            aux = jobExtension.Nudging ? "True" : "False";
                            xw.WriteElementString("Nudging", aux);
                            aux = jobExtension.SCOP ? "True" : "False";
                            xw.WriteElementString("SCOP", aux);
                            aux = jobExtension.SHC ? "True" : "False";
                            xw.WriteElementString("SHC", aux);
                            aux = jobExtension.AUXCOP ? "True" : "False";
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
                            aux = !string.IsNullOrEmpty(hydro.BatteryBrand) ? hydro.BatteryBrand : "Nulo";
                            xw.WriteElementString("BatteryBrand", aux);
                            aux = hydro.Battery ? "True" : "False";
                            xw.WriteElementString("Battery", aux);
                            aux = hydro.LifeJacket ? "True" : "False";
                            xw.WriteElementString("LifeJacket", aux);
                            aux = hydro.LOS ? "True" : "False";
                            xw.WriteElementString("LOS", aux);
                            aux = hydro.OilCool ? "True" : "False";
                            xw.WriteElementString("OilCool", aux);
                            aux = hydro.OilTank ? "True" : "False";
                            xw.WriteElementString("OilTank", aux);
                            aux = hydro.PSS ? "True" : "False";
                            xw.WriteElementString("PSS", aux);
                            aux = hydro.Resync ? "True" : "False";
                            xw.WriteElementString("Resync", aux);
                            aux = hydro.VCI ? "True" : "False";
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
                            aux = !string.IsNullOrEmpty(generic.EPCarsNumber) ? generic.EPCarsNumber : "Nulo";
                            xw.WriteElementString("EPCarsNumber", aux);
                            aux = !string.IsNullOrEmpty(generic.EPContact) ? generic.EPContact : "Nulo";
                            xw.WriteElementString("EPContact", aux);
                            aux = !string.IsNullOrEmpty(generic.TopAccessLocation) ? generic.TopAccessLocation : "Nulo";
                            xw.WriteElementString("TopAccessLocation", aux);
                            aux = !string.IsNullOrEmpty(generic.BottomAccessLocation) ? generic.BottomAccessLocation : "Nulo";
                            xw.WriteElementString("BottomAccessLocation", aux);
                            aux = !string.IsNullOrEmpty(generic.INCPButtons) ? generic.INCPButtons : "Nulo";
                            xw.WriteElementString("INCPButtons", aux);
                            aux = !string.IsNullOrEmpty(generic.SwitchStyle) ? generic.SwitchStyle : "Nulo";
                            xw.WriteElementString("SwitchStyle", aux);
                            aux = !string.IsNullOrEmpty(generic.GovModel) ? generic.GovModel : "Nulo";
                            xw.WriteElementString("GovModel", aux);
                            aux = !string.IsNullOrEmpty(generic.Monitoring) ? generic.Monitoring : "Nulo";
                            xw.WriteElementString("Monitoring", aux);
                            aux = !string.IsNullOrEmpty(generic.CarCallCodeSecurity) ? generic.CarCallCodeSecurity : "Nulo";
                            xw.WriteElementString("CarCallCodeSecurity", aux);
                            aux = !string.IsNullOrEmpty(generic.SpecialInstructions) ? generic.SpecialInstructions : "Nulo";
                            xw.WriteElementString("SpecialInstructions", aux);
                            aux = generic.FRON2 ? "True" : "False";
                            xw.WriteElementString("FRON2", aux);
                            aux = generic.Attendant ? "True" : "False";
                            xw.WriteElementString("Attendant", aux);
                            aux = generic.CarToLobby ? "True" : "False";
                            xw.WriteElementString("CarToLobby", aux);
                            aux = generic.EQ ? "True" : "False";
                            xw.WriteElementString("EQ", aux);
                            aux = generic.EMT ? "True" : "False";
                            xw.WriteElementString("EMT", aux);
                            aux = generic.EP ? "True" : "False";
                            xw.WriteElementString("EP", aux);
                            aux = generic.EPVoltage ? "True" : "False";
                            xw.WriteElementString("EPVoltage", aux);
                            aux = generic.EPOtherCars ? "True" : "False";
                            xw.WriteElementString("EPOtherCars", aux);
                            aux = generic.PTI ? "True" : "False";
                            xw.WriteElementString("Pit", aux);
                            aux = generic.EPSelect ? "True" : "False";
                            xw.WriteElementString("EPSelect", aux);
                            aux = generic.FLO ? "True" : "False";
                            xw.WriteElementString("FLO", aux);
                            aux = generic.Hosp ? "True" : "False";
                            xw.WriteElementString("Hosp", aux);
                            aux = generic.Pit ? "True" : "False";
                            xw.WriteElementString("Pit", aux);
                            aux = generic.INA ? "True" : "False";
                            xw.WriteElementString("INA", aux);
                            aux = generic.TopAccess ? "True" : "False";
                            xw.WriteElementString("TopAccess", aux);
                            aux = generic.BottomAccess ? "True" : "False";
                            xw.WriteElementString("BottomAccess", aux);
                            aux = generic.INCP ? "True" : "False";
                            xw.WriteElementString("INCP", aux);
                            aux = generic.LoadWeigher ? "True" : "False";
                            xw.WriteElementString("LoadWeigher", aux);
                            aux = generic.CTINSPST ? "True" : "False";
                            xw.WriteElementString("CTINSPST", aux);
                            aux = generic.Roped ? "True" : "False";
                            xw.WriteElementString("Roped", aux);
                            aux = generic.CallEnable ? "True" : "False";
                            xw.WriteElementString("CallEnable", aux);
                            aux = generic.CarCallRead ? "True" : "False";
                            xw.WriteElementString("CarCallRead", aux);
                            aux = generic.HallCallRead ? "True" : "False";
                            xw.WriteElementString("HallCallRead", aux);
                            aux = generic.CarKey ? "True" : "False";
                            xw.WriteElementString("CarKey", aux);
                            aux = generic.HallKey ? "True" : "False";
                            xw.WriteElementString("HallKey", aux);
                            aux = generic.CRO ? "True" : "False";
                            xw.WriteElementString("CRO", aux);
                            aux = generic.HCRO ? "True" : "False";
                            xw.WriteElementString("HCRO", aux);
                            aux = generic.BSI ? "True" : "False";
                            xw.WriteElementString("BSI", aux);
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
                            aux = !string.IsNullOrEmpty(indicator.CarPIType) ? indicator.CarPIType : "Nulo";
                            xw.WriteElementString("CarPIType", indicator.CarPIType);
                            aux = !string.IsNullOrEmpty(indicator.CarPIDiscreteType) ? indicator.CarPIDiscreteType : "Nulo";
                            xw.WriteElementString("CarPIDiscreteType", indicator.CarPIDiscreteType);
                            aux = !string.IsNullOrEmpty(indicator.HallPIType) ? indicator.HallPIType : "Nulo";
                            xw.WriteElementString("HallPIType", indicator.HallPIType);
                            aux = !string.IsNullOrEmpty(indicator.HallPIDiscreteType) ? indicator.HallPIDiscreteType : "Nulo";
                            xw.WriteElementString("HallPIDiscreteType", indicator.HallPIDiscreteType);
                            aux = !string.IsNullOrEmpty(indicator.VoiceAnnunciationPIType) ? indicator.VoiceAnnunciationPIType : "Nulo";
                            xw.WriteElementString("VoiceAnnunciationPIType", indicator.VoiceAnnunciationPIType);
                            aux = !string.IsNullOrEmpty(indicator.CarLanternsStyle) ? indicator.CarLanternsStyle : "Nulo";
                            xw.WriteElementString("CarLanternsStyle", indicator.CarLanternsStyle);
                            aux = !string.IsNullOrEmpty(indicator.CarLanternsType) ? indicator.CarLanternsType : "Nulo";
                            xw.WriteElementString("CarLanternsType", indicator.CarLanternsType);
                            aux = !string.IsNullOrEmpty(indicator.HallLanternsStyle) ? indicator.HallLanternsStyle : "Nulo";
                            xw.WriteElementString("HallLanternsStyle", indicator.HallLanternsStyle);
                            aux = !string.IsNullOrEmpty(indicator.HallLanternsType) ? indicator.HallLanternsType : "Nulo";
                            xw.WriteElementString("HallLanternsType", indicator.HallLanternsType);
                            aux = !string.IsNullOrEmpty(indicator.PassingFloorType) ? indicator.PassingFloorType : "Nulo";
                            xw.WriteElementString("PassingFloorType", indicator.PassingFloorType);
                            aux = !string.IsNullOrEmpty(indicator.PassingFloorDiscreteType) ? indicator.PassingFloorDiscreteType : "Nulo";
                            xw.WriteElementString("PassingFloorDiscreteType", indicator.PassingFloorDiscreteType);
                            xw.WriteElementString("IndicatorsVoltage", indicator.IndicatorsVoltage.ToString());
                            xw.WriteElementString("IndicatorsVoltageType", indicator.IndicatorsVoltageType);
                            aux = indicator.CarPI ? "True" : "False";
                            xw.WriteElementString("CarPI", aux);
                            aux = indicator.HallPI ? "True" : "False";
                            xw.WriteElementString("HallPI", aux);
                            aux = indicator.VoiceAnnunciationPI ? "True" : "False";
                            xw.WriteElementString("VoiceAnnunciationPI", aux);
                            aux = indicator.CarLanterns ? "True" : "False";
                            xw.WriteElementString("CarLanterns", aux);
                            aux = indicator.HallLanterns ? "True" : "False";
                            xw.WriteElementString("HallLanterns", aux);
                            aux = indicator.PassingFloor ? "True" : "False";
                            xw.WriteElementString("PassingFloor", aux);
                            aux = indicator.PassingFloorEnable ? "True" : "False";
                            xw.WriteElementString("PassingFloorEnable", aux);
                            aux = indicator.HallPIAll ? "True" : "False";
                            xw.WriteElementString("hallpiall", aux);
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
                            aux = hoist.FrontFirstServed ? "True" : "False";
                            xw.WriteElementString("FrontFirstServed", aux);
                            aux = hoist.RearFirstServed ? "True" : "False";
                            xw.WriteElementString("RearFirstServed", aux);
                            aux = hoist.FrontSecondServed ? "True" : "False";
                            xw.WriteElementString("FrontSecondServed", aux);
                            aux = hoist.RearSecondServed ? "True" : "False";
                            xw.WriteElementString("RearSecondServed", aux);
                            aux = hoist.FrontThirdServed ? "True" : "False";
                            xw.WriteElementString("FrontThirdServed", aux);
                            aux = hoist.RearThirdServed ? "True" : "False";
                            xw.WriteElementString("RearThirdServed", aux);
                            aux = hoist.FrontFourthServed ? "True" : "False";
                            xw.WriteElementString("FrontFourthServed", aux);
                            aux = hoist.RearFourthServed ? "True" : "False";
                            xw.WriteElementString("RearFourthServed", aux);
                            aux = hoist.FrontFifthServed ? "True" : "False";
                            xw.WriteElementString("FrontFifthServed", aux);
                            aux = hoist.RearFifthServed ? "True" : "False";
                            xw.WriteElementString("RearFifthServed", aux);
                            aux = hoist.FrontSexthServed ? "True" : "False";
                            xw.WriteElementString("FrontSexthServed", aux);
                            aux = hoist.RearSexthServed ? "True" : "False";
                            xw.WriteElementString("RearSexthServed", aux);
                            aux = hoist.FrontSeventhServed ? "True" : "False";
                            xw.WriteElementString("FrontSeventhServed", aux);
                            aux = hoist.RearSeventhServed ? "True" : "False";
                            xw.WriteElementString("RearSeventhServed", aux);
                            aux = hoist.FrontEightServed ? "True" : "False";
                            xw.WriteElementString("FrontEightServed", aux);
                            aux = hoist.RearEightServed ? "True" : "False";
                            xw.WriteElementString("RearEightServed", aux);
                            aux = hoist.FrontNinthServed ? "True" : "False";
                            xw.WriteElementString("FrontNinthServed", aux);
                            aux = hoist.RearNinthServed ? "True" : "False";
                            xw.WriteElementString("RearNinthServed", aux);
                            aux = hoist.FrontTenthServed ? "True" : "False";
                            xw.WriteElementString("FrontTenthServed", aux);
                            aux = hoist.RearTenthServed ? "True" : "False";
                            xw.WriteElementString("RearTenthServed", aux);
                            aux = hoist.FrontEleventhServed ? "True" : "False";
                            xw.WriteElementString("FrontEleventhServed", aux);
                            aux = hoist.RearEleventhServed ? "True" : "False";
                            xw.WriteElementString("RearEleventhServed", aux);
                            aux = hoist.FrontTwelvethServed ? "True" : "False";
                            xw.WriteElementString("FrontTwelvethServed", aux);
                            aux = hoist.RearTwelvethServed ? "True" : "False";
                            xw.WriteElementString("RearTwelvethServed", aux);
                            aux = hoist.FrontThirteenthServed ? "True" : "False";
                            xw.WriteElementString("FrontThirteenthServed", aux);
                            aux = hoist.RearThirteenthServed ? "True" : "False";
                            xw.WriteElementString("RearThirteenthServed", aux);
                            aux = hoist.FrontFourteenthServed ? "True" : "False";
                            xw.WriteElementString("FrontFourteenthServed", aux);
                            aux = hoist.RearFourteenthServed ? "True" : "False";
                            xw.WriteElementString("RearFourteenthServed", aux);
                            aux = hoist.FrontFifteenthServed ? "True" : "False";
                            xw.WriteElementString("FrontFifteenthServed", aux);
                            aux = hoist.RearFifteenthServed ? "True" : "False";
                            xw.WriteElementString("RearFifteenthServed", aux);
                            aux = hoist.FrontSixteenthServed ? "True" : "False";
                            xw.WriteElementString("FrontSixteenthServed", aux);
                            aux = hoist.RearSixteenthServed ? "True" : "False";
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

                xw.WriteEndElement();
                xw.WriteEndDocument();
            }
            ms.Position = 0;
            return File(ms, "text/xml", "UniqueJob.xml");
        }

        [HttpPost]
        public FileStreamResult ExportJobToXML()
        {
            MemoryStream ms = new MemoryStream();
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.OmitXmlDeclaration = true;
            xws.Indent = true;

            List<Job> jobs = repository.Jobs.Where(m => m.Contractor != "Fake").ToList();

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
                            xw.WriteElementString("Status", job.Status);
                            xw.WriteElementString("EngID", job.EngID.ToString());
                            xw.WriteElementString("CrossAppEngID", job.CrossAppEngID.ToString());
                            xw.WriteElementString("Name", job.Name);
                            xw.WriteElementString("Name2", job.Name2);
                            xw.WriteElementString("JobNum", job.JobNum.ToString());
                            xw.WriteElementString("ShipDate", job.ShipDate.ToString());
                            xw.WriteElementString("LatestFinishDate", job.LatestFinishDate.ToString());
                            xw.WriteElementString("Cust", job.Cust);
                            xw.WriteElementString("Contractor", job.Contractor);
                            xw.WriteElementString("JobTypeID", job.JobTypeID.ToString());
                            xw.WriteElementString("CityID", job.CityID.ToString());
                            xw.WriteElementString("FireCodeID", job.FireCodeID.ToString());
                            xw.WriteEndElement();

                            List<PO> pOsList = repository.POs.Where(m => m.JobID == job.JobID).ToList();
                            if (pOsList.Count > 0)
                            {
                                xw.WriteStartElement("POs");
                                foreach (PO po in pOsList)
                                {
                                    xw.WriteStartElement("PO");
                                    xw.WriteElementString("ID", po.POID.ToString());
                                    xw.WriteElementString("JobID", po.JobID.ToString());
                                    xw.WriteElementString("PONumb", po.PONumb.ToString());
                                    xw.WriteEndElement();
                                }
                                xw.WriteEndElement();
                            }

                            if (JobTypeName(job.JobTypeID) == "M2000")
                            {
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
                                    aux = jobExtension.SwingOp ? "True" : "False";
                                    xw.WriteElementString("SwingOp", aux);
                                    aux = jobExtension.BackUpDisp ? "True" : "False";
                                    xw.WriteElementString("BackUpDisp", aux);
                                    aux = jobExtension.AltRis ? "True" : "False";
                                    xw.WriteElementString("AltRis", aux);
                                    aux = jobExtension.InfDetector ? "True" : "False";
                                    xw.WriteElementString("InfDetector", aux);
                                    aux = jobExtension.MechSafEdge ? "True" : "False";
                                    xw.WriteElementString("MechSafEdge", aux);
                                    aux = jobExtension.HeavyDoors ? "True" : "False";
                                    xw.WriteElementString("HeavyDoors", aux);
                                    aux = jobExtension.CartopDoorButtons ? "True" : "False";
                                    xw.WriteElementString("CartopDoorButtons", aux);
                                    aux = jobExtension.DoorHold ? "True" : "False";
                                    xw.WriteElementString("DoorHold", aux);
                                    aux = jobExtension.Nudging ? "True" : "False";
                                    xw.WriteElementString("Nudging", aux);
                                    aux = jobExtension.SCOP ? "True" : "False";
                                    xw.WriteElementString("SCOP", aux);
                                    aux = jobExtension.SHC ? "True" : "False";
                                    xw.WriteElementString("SHC", aux);
                                    aux = jobExtension.AUXCOP ? "True" : "False";
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
                                    aux = !string.IsNullOrEmpty(hydro.BatteryBrand) ? hydro.BatteryBrand : "Nulo";
                                    xw.WriteElementString("BatteryBrand", aux);
                                    aux = hydro.Battery ? "True" : "False";
                                    xw.WriteElementString("Battery", aux);
                                    aux = hydro.LifeJacket ? "True" : "False";
                                    xw.WriteElementString("LifeJacket", aux);
                                    aux = hydro.LOS ? "True" : "False";
                                    xw.WriteElementString("LOS", aux);
                                    aux = hydro.OilCool ? "True" : "False";
                                    xw.WriteElementString("OilCool", aux);
                                    aux = hydro.OilTank ? "True" : "False";
                                    xw.WriteElementString("OilTank", aux);
                                    aux = hydro.PSS ? "True" : "False";
                                    xw.WriteElementString("PSS", aux);
                                    aux = hydro.Resync ? "True" : "False";
                                    xw.WriteElementString("Resync", aux);
                                    aux = hydro.VCI ? "True" : "False";
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
                                    aux = !string.IsNullOrEmpty(generic.EPCarsNumber) ? generic.EPCarsNumber : "Nulo";
                                    xw.WriteElementString("EPCarsNumber", aux);
                                    aux = !string.IsNullOrEmpty(generic.EPContact) ? generic.EPContact : "Nulo";
                                    xw.WriteElementString("EPContact", aux);
                                    aux = !string.IsNullOrEmpty(generic.TopAccessLocation) ? generic.TopAccessLocation : "Nulo";
                                    xw.WriteElementString("TopAccessLocation", aux);
                                    aux = !string.IsNullOrEmpty(generic.BottomAccessLocation) ? generic.BottomAccessLocation : "Nulo";
                                    xw.WriteElementString("BottomAccessLocation", aux);
                                    aux = !string.IsNullOrEmpty(generic.INCPButtons) ? generic.INCPButtons : "Nulo";
                                    xw.WriteElementString("INCPButtons", aux);
                                    aux = !string.IsNullOrEmpty(generic.SwitchStyle) ? generic.SwitchStyle : "Nulo";
                                    xw.WriteElementString("SwitchStyle", aux);
                                    aux = !string.IsNullOrEmpty(generic.GovModel) ? generic.GovModel : "Nulo";
                                    xw.WriteElementString("GovModel", aux);
                                    aux = !string.IsNullOrEmpty(generic.Monitoring) ? generic.Monitoring : "Nulo";
                                    xw.WriteElementString("Monitoring", aux);
                                    aux = !string.IsNullOrEmpty(generic.CarCallCodeSecurity) ? generic.CarCallCodeSecurity : "Nulo";
                                    xw.WriteElementString("CarCallCodeSecurity", aux);
                                    aux = !string.IsNullOrEmpty(generic.SpecialInstructions) ? generic.SpecialInstructions : "Nulo";
                                    xw.WriteElementString("SpecialInstructions", aux);
                                    aux = generic.FRON2 ? "True" : "False";
                                    xw.WriteElementString("FRON2", aux);
                                    aux = generic.Attendant ? "True" : "False";
                                    xw.WriteElementString("Attendant", aux);
                                    aux = generic.CarToLobby ? "True" : "False";
                                    xw.WriteElementString("CarToLobby", aux);
                                    aux = generic.EQ ? "True" : "False";
                                    xw.WriteElementString("EQ", aux);
                                    aux = generic.EMT ? "True" : "False";
                                    xw.WriteElementString("EMT", aux);
                                    aux = generic.EP ? "True" : "False";
                                    xw.WriteElementString("EP", aux);
                                    aux = generic.EPVoltage ? "True" : "False";
                                    xw.WriteElementString("EPVoltage", aux);
                                    aux = generic.EPOtherCars ? "True" : "False";
                                    xw.WriteElementString("EPOtherCars", aux);
                                    aux = generic.PTI ? "True" : "False";
                                    xw.WriteElementString("PTI", aux);
                                    aux = generic.EPSelect ? "True" : "False";
                                    xw.WriteElementString("EPSelect", aux);
                                    aux = generic.FLO ? "True" : "False";
                                    xw.WriteElementString("FLO", aux);
                                    aux = generic.Hosp ? "True" : "False";
                                    xw.WriteElementString("Hosp", aux);
                                    aux = generic.Pit ? "True" : "False";
                                    xw.WriteElementString("Pit", aux);
                                    aux = generic.INA ? "True" : "False";
                                    xw.WriteElementString("INA", aux);
                                    aux = generic.TopAccess ? "True" : "False";
                                    xw.WriteElementString("TopAccess", aux);
                                    aux = generic.BottomAccess ? "True" : "False";
                                    xw.WriteElementString("BottomAccess", aux);
                                    aux = generic.INCP ? "True" : "False";
                                    xw.WriteElementString("INCP", aux);
                                    aux = generic.LoadWeigher ? "True" : "False";
                                    xw.WriteElementString("LoadWeigher", aux);
                                    aux = generic.CTINSPST ? "True" : "False";
                                    xw.WriteElementString("CTINSPST", aux);
                                    aux = generic.Roped ? "True" : "False";
                                    xw.WriteElementString("Roped", aux);
                                    aux = generic.CallEnable ? "True" : "False";
                                    xw.WriteElementString("CallEnable", aux);
                                    aux = generic.CarCallRead ? "True" : "False";
                                    xw.WriteElementString("CarCallRead", aux);
                                    aux = generic.HallCallRead ? "True" : "False";
                                    xw.WriteElementString("HallCallRead", aux);
                                    aux = generic.CarKey ? "True" : "False";
                                    xw.WriteElementString("CarKey", aux);
                                    aux = generic.HallKey ? "True" : "False";
                                    xw.WriteElementString("HallKey", aux);
                                    aux = generic.CRO ? "True" : "False";
                                    xw.WriteElementString("CRO", aux);
                                    aux = generic.HCRO ? "True" : "False";
                                    xw.WriteElementString("HCRO", aux);
                                    aux = generic.BSI ? "True" : "False";
                                    xw.WriteElementString("BSI", aux);
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
                                    aux = !string.IsNullOrEmpty(indicator.CarPIType) ? indicator.CarPIType : "Nulo";
                                    xw.WriteElementString("CarPIType", indicator.CarPIType);
                                    aux = !string.IsNullOrEmpty(indicator.CarPIDiscreteType) ? indicator.CarPIDiscreteType : "Nulo";
                                    xw.WriteElementString("CarPIDiscreteType", indicator.CarPIDiscreteType);
                                    aux = !string.IsNullOrEmpty(indicator.HallPIType) ? indicator.HallPIType : "Nulo";
                                    xw.WriteElementString("HallPIType", indicator.HallPIType);
                                    aux = !string.IsNullOrEmpty(indicator.HallPIDiscreteType) ? indicator.HallPIDiscreteType : "Nulo";
                                    xw.WriteElementString("HallPIDiscreteType", indicator.HallPIDiscreteType);
                                    aux = !string.IsNullOrEmpty(indicator.VoiceAnnunciationPIType) ? indicator.VoiceAnnunciationPIType : "Nulo";
                                    xw.WriteElementString("VoiceAnnunciationPIType", indicator.VoiceAnnunciationPIType);
                                    aux = !string.IsNullOrEmpty(indicator.CarLanternsStyle) ? indicator.CarLanternsStyle : "Nulo";
                                    xw.WriteElementString("CarLanternsStyle", indicator.CarLanternsStyle);
                                    aux = !string.IsNullOrEmpty(indicator.CarLanternsType) ? indicator.CarLanternsType : "Nulo";
                                    xw.WriteElementString("CarLanternsType", indicator.CarLanternsType);
                                    aux = !string.IsNullOrEmpty(indicator.HallLanternsStyle) ? indicator.HallLanternsStyle : "Nulo";
                                    xw.WriteElementString("HallLanternsStyle", indicator.HallLanternsStyle);
                                    aux = !string.IsNullOrEmpty(indicator.HallLanternsType) ? indicator.HallLanternsType : "Nulo";
                                    xw.WriteElementString("HallLanternsType", indicator.HallLanternsType);
                                    aux = !string.IsNullOrEmpty(indicator.PassingFloorType) ? indicator.PassingFloorType : "Nulo";
                                    xw.WriteElementString("PassingFloorType", indicator.PassingFloorType);
                                    aux = !string.IsNullOrEmpty(indicator.PassingFloorDiscreteType) ? indicator.PassingFloorDiscreteType : "Nulo";
                                    xw.WriteElementString("PassingFloorDiscreteType", indicator.PassingFloorDiscreteType);
                                    xw.WriteElementString("IndicatorsVoltage", indicator.IndicatorsVoltage.ToString());
                                    xw.WriteElementString("IndicatorsVoltageType", indicator.IndicatorsVoltageType);
                                    aux = indicator.CarPI ? "True" : "False";
                                    xw.WriteElementString("CarPI", aux);
                                    aux = indicator.HallPI ? "True" : "False";
                                    xw.WriteElementString("HallPI", aux);
                                    aux = indicator.VoiceAnnunciationPI ? "True" : "False";
                                    xw.WriteElementString("VoiceAnnunciationPI", aux);
                                    aux = indicator.CarLanterns ? "True" : "False";
                                    xw.WriteElementString("CarLanterns", aux);
                                    aux = indicator.HallLanterns ? "True" : "False";
                                    xw.WriteElementString("HallLanterns", aux);
                                    aux = indicator.PassingFloor ? "True" : "False";
                                    xw.WriteElementString("PassingFloor", aux);
                                    aux = indicator.PassingFloorEnable ? "True" : "False";
                                    xw.WriteElementString("PassingFloorEnable", aux);
                                    aux = indicator.HallPIAll ? "True" : "False";
                                    xw.WriteElementString("hallpiall", aux);
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
                                    aux = hoist.FrontFirstServed ? "True" : "False";
                                    xw.WriteElementString("FrontFirstServed", aux);
                                    aux = hoist.RearFirstServed ? "True" : "False";
                                    xw.WriteElementString("RearFirstServed", aux);
                                    aux = hoist.FrontSecondServed ? "True" : "False";
                                    xw.WriteElementString("FrontSecondServed", aux);
                                    aux = hoist.RearSecondServed ? "True" : "False";
                                    xw.WriteElementString("RearSecondServed", aux);
                                    aux = hoist.FrontThirdServed ? "True" : "False";
                                    xw.WriteElementString("FrontThirdServed", aux);
                                    aux = hoist.RearThirdServed ? "True" : "False";
                                    xw.WriteElementString("RearThirdServed", aux);
                                    aux = hoist.FrontFourthServed ? "True" : "False";
                                    xw.WriteElementString("FrontFourthServed", aux);
                                    aux = hoist.RearFourthServed ? "True" : "False";
                                    xw.WriteElementString("RearFourthServed", aux);
                                    aux = hoist.FrontFifthServed ? "True" : "False";
                                    xw.WriteElementString("FrontFifthServed", aux);
                                    aux = hoist.RearFifthServed ? "True" : "False";
                                    xw.WriteElementString("RearFifthServed", aux);
                                    aux = hoist.FrontSexthServed ? "True" : "False";
                                    xw.WriteElementString("FrontSexthServed", aux);
                                    aux = hoist.RearSexthServed ? "True" : "False";
                                    xw.WriteElementString("RearSexthServed", aux);
                                    aux = hoist.FrontSeventhServed ? "True" : "False";
                                    xw.WriteElementString("FrontSeventhServed", aux);
                                    aux = hoist.RearSeventhServed ? "True" : "False";
                                    xw.WriteElementString("RearSeventhServed", aux);
                                    aux = hoist.FrontEightServed ? "True" : "False";
                                    xw.WriteElementString("FrontEightServed", aux);
                                    aux = hoist.RearEightServed ? "True" : "False";
                                    xw.WriteElementString("RearEightServed", aux);
                                    aux = hoist.FrontNinthServed ? "True" : "False";
                                    xw.WriteElementString("FrontNinthServed", aux);
                                    aux = hoist.RearNinthServed ? "True" : "False";
                                    xw.WriteElementString("RearNinthServed", aux);
                                    aux = hoist.FrontTenthServed ? "True" : "False";
                                    xw.WriteElementString("FrontTenthServed", aux);
                                    aux = hoist.RearTenthServed ? "True" : "False";
                                    xw.WriteElementString("RearTenthServed", aux);
                                    aux = hoist.FrontEleventhServed ? "True" : "False";
                                    xw.WriteElementString("FrontEleventhServed", aux);
                                    aux = hoist.RearEleventhServed ? "True" : "False";
                                    xw.WriteElementString("RearEleventhServed", aux);
                                    aux = hoist.FrontTwelvethServed ? "True" : "False";
                                    xw.WriteElementString("FrontTwelvethServed", aux);
                                    aux = hoist.RearTwelvethServed ? "True" : "False";
                                    xw.WriteElementString("RearTwelvethServed", aux);
                                    aux = hoist.FrontThirteenthServed ? "True" : "False";
                                    xw.WriteElementString("FrontThirteenthServed", aux);
                                    aux = hoist.RearThirteenthServed ? "True" : "False";
                                    xw.WriteElementString("RearThirteenthServed", aux);
                                    aux = hoist.FrontFourteenthServed ? "True" : "False";
                                    xw.WriteElementString("FrontFourteenthServed", aux);
                                    aux = hoist.RearFourteenthServed ? "True" : "False";
                                    xw.WriteElementString("RearFourteenthServed", aux);
                                    aux = hoist.FrontFifteenthServed ? "True" : "False";
                                    xw.WriteElementString("FrontFifteenthServed", aux);
                                    aux = hoist.RearFifteenthServed ? "True" : "False";
                                    xw.WriteElementString("RearFifteenthServed", aux);
                                    aux = hoist.FrontSixteenthServed ? "True" : "False";
                                    xw.WriteElementString("FrontSixteenthServed", aux);
                                    aux = hoist.RearSixteenthServed ? "True" : "False";
                                    xw.WriteElementString("RearSixteenthServed", aux);
                                    xw.WriteEndElement();
                                }


                            }
                            else if (JobTypeName(job.JobTypeID) == "ElmHydro" || JobTypeName(job.JobTypeID) == "ElmTract")
                            {
                                Element element = repository.Elements.First(m => m.JobID == job.JobID);
                                if (element != null)
                                {
                                    string aux;
                                    xw.WriteStartElement("Element");
                                    xw.WriteElementString("ID", element.ElementID.ToString());
                                    xw.WriteElementString("JobID", element.JobID.ToString());

                                    xw.WriteElementString("DoorOperatorID", element.DoorOperatorID.ToString());
                                    xw.WriteElementString("Capacity", element.Capacity.ToString());
                                    xw.WriteElementString("Speed", element.Speed.ToString());
                                    xw.WriteElementString("Voltage", element.Voltage.ToString());
                                    xw.WriteElementString("Phase", element.Phase.ToString());
                                    xw.WriteElementString("Frequency", element.Frequency.ToString());
                                    xw.WriteElementString("LoadWeigher", element.LoadWeigher.ToString());


                                    aux = !string.IsNullOrEmpty(element.DoorGate) ? element.DoorGate : "Nulo";
                                    xw.WriteElementString("DoorGate", aux);
                                    aux = !string.IsNullOrEmpty(element.INA) ? element.INA : "Nulo";
                                    xw.WriteElementString("INA", aux);
                                    aux = !string.IsNullOrEmpty(element.LoadWeigher) ? element.LoadWeigher : "Nulo";
                                    xw.WriteElementString("LoadWeigher", aux);
                                    aux = element.FRON2 ? "True" : "False";
                                    xw.WriteElementString("FRON2", aux);
                                    aux = element.EQ ? "True" : "False";
                                    xw.WriteElementString("EQ", aux);
                                    aux = element.EMT ? "True" : "False";
                                    xw.WriteElementString("EMT", aux);
                                    aux = element.EP ? "True" : "False";
                                    xw.WriteElementString("EP", aux);
                                    aux = element.INCP ? "True" : "False";
                                    xw.WriteElementString("INCP", aux);
                                    aux = element.CTINSPST ? "True" : "False";
                                    xw.WriteElementString("CTINSPST", aux);
                                    aux = element.CallEnable ? "True" : "False";
                                    xw.WriteElementString("CallEnable", aux);
                                    aux = element.CarKey ? "True" : "False";
                                    xw.WriteElementString("CarKey", aux);
                                    aux = element.HallKey ? "True" : "False";
                                    xw.WriteElementString("HallKey", aux);
                                    aux = element.CRO ? "True" : "False";
                                    xw.WriteElementString("CRO", aux);
                                    aux = element.HCRO ? "True" : "False";
                                    xw.WriteElementString("HCRO", aux);
                                    aux = element.CarCardReader ? "True" : "False";
                                    xw.WriteElementString("CarCardReader", aux);
                                    aux = element.HallCardReader ? "True" : "False";
                                    xw.WriteElementString("HallCardReader", aux);
                                    aux = element.HAPS ? "True" : "False";
                                    xw.WriteElementString("HAPS", aux);
                                    aux = element.PSS ? "True" : "False";
                                    xw.WriteElementString("PSS", aux);
                                    aux = element.PTFLD ? "True" : "False";
                                    xw.WriteElementString("PTFLD", aux);
                                    aux = element.VCI ? "True" : "False";
                                    xw.WriteElementString("VCI", aux);
                                    aux = element.CReg ? "True" : "False";
                                    xw.WriteElementString("CReg", aux);
                                    aux = element.Egress ? "True" : "False";
                                    xw.WriteElementString("Egress", aux);
                                    aux = element.PHECutOut ? "True" : "False";
                                    xw.WriteElementString("PHECutOut", aux);
                                    aux = element.Traveler ? "True" : "False";
                                    xw.WriteElementString("Traveler", aux);
                                    aux = element.LOS ? "True" : "False";
                                    xw.WriteElementString("LOS", aux);
                                    aux = element.PFGE ? "True" : "False";
                                    xw.WriteElementString("PFGE", aux);
                                    aux = element.CTL ? "True" : "False";
                                    xw.WriteElementString("CTL", aux);
                                    aux = element.CSD ? "True" : "False";
                                    xw.WriteElementString("CSD", aux);
                                    aux = element.CTF ? "True" : "False";
                                    xw.WriteElementString("CTF", aux);
                                    aux = element.LJ ? "True" : "False";
                                    xw.WriteElementString("LJ", aux);
                                    aux = element.DHLD ? "True" : "False";
                                    xw.WriteElementString("DHLD", aux);
                                    xw.WriteEndElement();
                                }

                                if (JobTypeName(job.JobTypeID) == "ElmHydro")
                                {
                                    ElementHydro hydro = repository.ElementHydros.First(m => m.JobID == job.JobID);
                                    if (hydro != null)
                                    {
                                        xw.WriteStartElement("ElementHydro");
                                        xw.WriteElementString("ID", hydro.ElementHydroID.ToString());
                                        xw.WriteElementString("JobID", hydro.JobID.ToString());
                                        xw.WriteElementString("Starter", hydro.Starter);
                                        xw.WriteElementString("HP", hydro.HP.ToString());
                                        xw.WriteElementString("FLA", hydro.FLA.ToString());
                                        xw.WriteElementString("SPH", hydro.SPH.ToString());
                                        xw.WriteElementString("ValveBrand", hydro.ValveBrand);
                                        xw.WriteEndElement();
                                    }
                                }
                                else
                                {
                                    ElementTraction traction = repository.ElementTractions.First(m => m.JobID == job.JobID);
                                    if (traction != null)
                                    {
                                        string aux;
                                        xw.WriteStartElement("ElementTraction");
                                        xw.WriteElementString("ID", traction.ElementTractionID.ToString());
                                        xw.WriteElementString("JobID", traction.JobID.ToString());
                                        xw.WriteElementString("HP", traction.HP.ToString());
                                        xw.WriteElementString("FLA", traction.FLA.ToString());
                                        xw.WriteElementString("PickVoltage", traction.PickVoltage.ToString());
                                        xw.WriteElementString("HoldVoltage", traction.HoldVoltage.ToString());
                                        xw.WriteElementString("Resistance", traction.Resistance.ToString());
                                        xw.WriteElementString("Current", traction.Current.ToString());

                                        xw.WriteElementString("MachineLocation", traction.MachineLocation);
                                        xw.WriteElementString("VVVF", traction.VVVF);
                                        xw.WriteElementString("MotorBrand", traction.MotorBrand);
                                        xw.WriteElementString("Contact", traction.Contact);

                                        aux = traction.Encoder ? "True" : "False";
                                        xw.WriteElementString("Encoder", aux);
                                        aux = traction.ISO ? "True" : "False";
                                        xw.WriteElementString("ISO", aux);

                                        xw.WriteEndElement();
                                    }
                                }
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


        public void ExportJobsToXML()
        {
            MemoryStream ms = new MemoryStream();
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.OmitXmlDeclaration = true;
            xws.Indent = true;
           string fileName = "Jobs-"+ DateTime.Now.ToString("dd-MM-yyyy")+".xml";

            List<Job> jobs = repository.Jobs.Where(m => m.Contractor != "Fake").ToList();
            var path = $@"{environment.ContentRootPath}\wwwroot\DailyJobs\{fileName}";
            using (XmlWriter xw = XmlWriter.Create(path, xws))
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
                            xw.WriteElementString("Status", job.Status);
                            xw.WriteElementString("EngID", job.EngID.ToString());
                            xw.WriteElementString("CrossAppEngID", job.CrossAppEngID.ToString());
                            xw.WriteElementString("Name", job.Name);
                            xw.WriteElementString("Name2", job.Name2);
                            xw.WriteElementString("JobNum", job.JobNum.ToString());
                            xw.WriteElementString("ShipDate", job.ShipDate.ToString());
                            xw.WriteElementString("LatestFinishDate", job.LatestFinishDate.ToString());
                            xw.WriteElementString("Cust", job.Cust);
                            xw.WriteElementString("Contractor", job.Contractor);
                            xw.WriteElementString("JobTypeID", job.JobTypeID.ToString());
                            xw.WriteElementString("CityID", job.CityID.ToString());
                            xw.WriteElementString("FireCodeID", job.FireCodeID.ToString());
                            xw.WriteEndElement();

                            List<PO> pOsList = repository.POs.Where(m => m.JobID == job.JobID).ToList();
                            if (pOsList.Count > 0)
                            {
                                xw.WriteStartElement("POs");
                                foreach (PO po in pOsList)
                                {
                                    xw.WriteStartElement("PO");
                                    xw.WriteElementString("ID", po.POID.ToString());
                                    xw.WriteElementString("JobID", po.JobID.ToString());
                                    xw.WriteElementString("PONumb", po.PONumb.ToString());
                                    xw.WriteEndElement();
                                }
                                xw.WriteEndElement();
                            }

                            if (JobTypeName(job.JobTypeID) == "M2000")
                            {
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
                                    aux = jobExtension.SwingOp ? "True" : "False";
                                    xw.WriteElementString("SwingOp", aux);
                                    aux = jobExtension.BackUpDisp ? "True" : "False";
                                    xw.WriteElementString("BackUpDisp", aux);
                                    aux = jobExtension.AltRis ? "True" : "False";
                                    xw.WriteElementString("AltRis", aux);
                                    aux = jobExtension.InfDetector ? "True" : "False";
                                    xw.WriteElementString("InfDetector", aux);
                                    aux = jobExtension.MechSafEdge ? "True" : "False";
                                    xw.WriteElementString("MechSafEdge", aux);
                                    aux = jobExtension.HeavyDoors ? "True" : "False";
                                    xw.WriteElementString("HeavyDoors", aux);
                                    aux = jobExtension.CartopDoorButtons ? "True" : "False";
                                    xw.WriteElementString("CartopDoorButtons", aux);
                                    aux = jobExtension.DoorHold ? "True" : "False";
                                    xw.WriteElementString("DoorHold", aux);
                                    aux = jobExtension.Nudging ? "True" : "False";
                                    xw.WriteElementString("Nudging", aux);
                                    aux = jobExtension.SCOP ? "True" : "False";
                                    xw.WriteElementString("SCOP", aux);
                                    aux = jobExtension.SHC ? "True" : "False";
                                    xw.WriteElementString("SHC", aux);
                                    aux = jobExtension.AUXCOP ? "True" : "False";
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
                                    aux = !string.IsNullOrEmpty(hydro.BatteryBrand) ? hydro.BatteryBrand : "Nulo";
                                    xw.WriteElementString("BatteryBrand", aux);
                                    aux = hydro.Battery ? "True" : "False";
                                    xw.WriteElementString("Battery", aux);
                                    aux = hydro.LifeJacket ? "True" : "False";
                                    xw.WriteElementString("LifeJacket", aux);
                                    aux = hydro.LOS ? "True" : "False";
                                    xw.WriteElementString("LOS", aux);
                                    aux = hydro.OilCool ? "True" : "False";
                                    xw.WriteElementString("OilCool", aux);
                                    aux = hydro.OilTank ? "True" : "False";
                                    xw.WriteElementString("OilTank", aux);
                                    aux = hydro.PSS ? "True" : "False";
                                    xw.WriteElementString("PSS", aux);
                                    aux = hydro.Resync ? "True" : "False";
                                    xw.WriteElementString("Resync", aux);
                                    aux = hydro.VCI ? "True" : "False";
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
                                    aux = !string.IsNullOrEmpty(generic.EPCarsNumber) ? generic.EPCarsNumber : "Nulo";
                                    xw.WriteElementString("EPCarsNumber", aux);
                                    aux = !string.IsNullOrEmpty(generic.EPContact) ? generic.EPContact : "Nulo";
                                    xw.WriteElementString("EPContact", aux);
                                    aux = !string.IsNullOrEmpty(generic.TopAccessLocation) ? generic.TopAccessLocation : "Nulo";
                                    xw.WriteElementString("TopAccessLocation", aux);
                                    aux = !string.IsNullOrEmpty(generic.BottomAccessLocation) ? generic.BottomAccessLocation : "Nulo";
                                    xw.WriteElementString("BottomAccessLocation", aux);
                                    aux = !string.IsNullOrEmpty(generic.INCPButtons) ? generic.INCPButtons : "Nulo";
                                    xw.WriteElementString("INCPButtons", aux);
                                    aux = !string.IsNullOrEmpty(generic.SwitchStyle) ? generic.SwitchStyle : "Nulo";
                                    xw.WriteElementString("SwitchStyle", aux);
                                    aux = !string.IsNullOrEmpty(generic.GovModel) ? generic.GovModel : "Nulo";
                                    xw.WriteElementString("GovModel", aux);
                                    aux = !string.IsNullOrEmpty(generic.Monitoring) ? generic.Monitoring : "Nulo";
                                    xw.WriteElementString("Monitoring", aux);
                                    aux = !string.IsNullOrEmpty(generic.CarCallCodeSecurity) ? generic.CarCallCodeSecurity : "Nulo";
                                    xw.WriteElementString("CarCallCodeSecurity", aux);
                                    aux = !string.IsNullOrEmpty(generic.SpecialInstructions) ? generic.SpecialInstructions : "Nulo";
                                    xw.WriteElementString("SpecialInstructions", aux);
                                    aux = generic.FRON2 ? "True" : "False";
                                    xw.WriteElementString("FRON2", aux);
                                    aux = generic.Attendant ? "True" : "False";
                                    xw.WriteElementString("Attendant", aux);
                                    aux = generic.CarToLobby ? "True" : "False";
                                    xw.WriteElementString("CarToLobby", aux);
                                    aux = generic.EQ ? "True" : "False";
                                    xw.WriteElementString("EQ", aux);
                                    aux = generic.EMT ? "True" : "False";
                                    xw.WriteElementString("EMT", aux);
                                    aux = generic.EP ? "True" : "False";
                                    xw.WriteElementString("EP", aux);
                                    aux = generic.EPVoltage ? "True" : "False";
                                    xw.WriteElementString("EPVoltage", aux);
                                    aux = generic.EPOtherCars ? "True" : "False";
                                    xw.WriteElementString("EPOtherCars", aux);
                                    aux = generic.PTI ? "True" : "False";
                                    xw.WriteElementString("PTI", aux);
                                    aux = generic.EPSelect ? "True" : "False";
                                    xw.WriteElementString("EPSelect", aux);
                                    aux = generic.FLO ? "True" : "False";
                                    xw.WriteElementString("FLO", aux);
                                    aux = generic.Hosp ? "True" : "False";
                                    xw.WriteElementString("Hosp", aux);
                                    aux = generic.Pit ? "True" : "False";
                                    xw.WriteElementString("Pit", aux);
                                    aux = generic.INA ? "True" : "False";
                                    xw.WriteElementString("INA", aux);
                                    aux = generic.TopAccess ? "True" : "False";
                                    xw.WriteElementString("TopAccess", aux);
                                    aux = generic.BottomAccess ? "True" : "False";
                                    xw.WriteElementString("BottomAccess", aux);
                                    aux = generic.INCP ? "True" : "False";
                                    xw.WriteElementString("INCP", aux);
                                    aux = generic.LoadWeigher ? "True" : "False";
                                    xw.WriteElementString("LoadWeigher", aux);
                                    aux = generic.CTINSPST ? "True" : "False";
                                    xw.WriteElementString("CTINSPST", aux);
                                    aux = generic.Roped ? "True" : "False";
                                    xw.WriteElementString("Roped", aux);
                                    aux = generic.CallEnable ? "True" : "False";
                                    xw.WriteElementString("CallEnable", aux);
                                    aux = generic.CarCallRead ? "True" : "False";
                                    xw.WriteElementString("CarCallRead", aux);
                                    aux = generic.HallCallRead ? "True" : "False";
                                    xw.WriteElementString("HallCallRead", aux);
                                    aux = generic.CarKey ? "True" : "False";
                                    xw.WriteElementString("CarKey", aux);
                                    aux = generic.HallKey ? "True" : "False";
                                    xw.WriteElementString("HallKey", aux);
                                    aux = generic.CRO ? "True" : "False";
                                    xw.WriteElementString("CRO", aux);
                                    aux = generic.HCRO ? "True" : "False";
                                    xw.WriteElementString("HCRO", aux);
                                    aux = generic.BSI ? "True" : "False";
                                    xw.WriteElementString("BSI", aux);
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
                                    aux = !string.IsNullOrEmpty(indicator.CarPIType) ? indicator.CarPIType : "Nulo";
                                    xw.WriteElementString("CarPIType", indicator.CarPIType);
                                    aux = !string.IsNullOrEmpty(indicator.CarPIDiscreteType) ? indicator.CarPIDiscreteType : "Nulo";
                                    xw.WriteElementString("CarPIDiscreteType", indicator.CarPIDiscreteType);
                                    aux = !string.IsNullOrEmpty(indicator.HallPIType) ? indicator.HallPIType : "Nulo";
                                    xw.WriteElementString("HallPIType", indicator.HallPIType);
                                    aux = !string.IsNullOrEmpty(indicator.HallPIDiscreteType) ? indicator.HallPIDiscreteType : "Nulo";
                                    xw.WriteElementString("HallPIDiscreteType", indicator.HallPIDiscreteType);
                                    aux = !string.IsNullOrEmpty(indicator.VoiceAnnunciationPIType) ? indicator.VoiceAnnunciationPIType : "Nulo";
                                    xw.WriteElementString("VoiceAnnunciationPIType", indicator.VoiceAnnunciationPIType);
                                    aux = !string.IsNullOrEmpty(indicator.CarLanternsStyle) ? indicator.CarLanternsStyle : "Nulo";
                                    xw.WriteElementString("CarLanternsStyle", indicator.CarLanternsStyle);
                                    aux = !string.IsNullOrEmpty(indicator.CarLanternsType) ? indicator.CarLanternsType : "Nulo";
                                    xw.WriteElementString("CarLanternsType", indicator.CarLanternsType);
                                    aux = !string.IsNullOrEmpty(indicator.HallLanternsStyle) ? indicator.HallLanternsStyle : "Nulo";
                                    xw.WriteElementString("HallLanternsStyle", indicator.HallLanternsStyle);
                                    aux = !string.IsNullOrEmpty(indicator.HallLanternsType) ? indicator.HallLanternsType : "Nulo";
                                    xw.WriteElementString("HallLanternsType", indicator.HallLanternsType);
                                    aux = !string.IsNullOrEmpty(indicator.PassingFloorType) ? indicator.PassingFloorType : "Nulo";
                                    xw.WriteElementString("PassingFloorType", indicator.PassingFloorType);
                                    aux = !string.IsNullOrEmpty(indicator.PassingFloorDiscreteType) ? indicator.PassingFloorDiscreteType : "Nulo";
                                    xw.WriteElementString("PassingFloorDiscreteType", indicator.PassingFloorDiscreteType);
                                    xw.WriteElementString("IndicatorsVoltage", indicator.IndicatorsVoltage.ToString());
                                    xw.WriteElementString("IndicatorsVoltageType", indicator.IndicatorsVoltageType);
                                    aux = indicator.CarPI ? "True" : "False";
                                    xw.WriteElementString("CarPI", aux);
                                    aux = indicator.HallPI ? "True" : "False";
                                    xw.WriteElementString("HallPI", aux);
                                    aux = indicator.VoiceAnnunciationPI ? "True" : "False";
                                    xw.WriteElementString("VoiceAnnunciationPI", aux);
                                    aux = indicator.CarLanterns ? "True" : "False";
                                    xw.WriteElementString("CarLanterns", aux);
                                    aux = indicator.HallLanterns ? "True" : "False";
                                    xw.WriteElementString("HallLanterns", aux);
                                    aux = indicator.PassingFloor ? "True" : "False";
                                    xw.WriteElementString("PassingFloor", aux);
                                    aux = indicator.PassingFloorEnable ? "True" : "False";
                                    xw.WriteElementString("PassingFloorEnable", aux);
                                    aux = indicator.HallPIAll ? "True" : "False";
                                    xw.WriteElementString("hallpiall", aux);
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
                                    aux = hoist.FrontFirstServed ? "True" : "False";
                                    xw.WriteElementString("FrontFirstServed", aux);
                                    aux = hoist.RearFirstServed ? "True" : "False";
                                    xw.WriteElementString("RearFirstServed", aux);
                                    aux = hoist.FrontSecondServed ? "True" : "False";
                                    xw.WriteElementString("FrontSecondServed", aux);
                                    aux = hoist.RearSecondServed ? "True" : "False";
                                    xw.WriteElementString("RearSecondServed", aux);
                                    aux = hoist.FrontThirdServed ? "True" : "False";
                                    xw.WriteElementString("FrontThirdServed", aux);
                                    aux = hoist.RearThirdServed ? "True" : "False";
                                    xw.WriteElementString("RearThirdServed", aux);
                                    aux = hoist.FrontFourthServed ? "True" : "False";
                                    xw.WriteElementString("FrontFourthServed", aux);
                                    aux = hoist.RearFourthServed ? "True" : "False";
                                    xw.WriteElementString("RearFourthServed", aux);
                                    aux = hoist.FrontFifthServed ? "True" : "False";
                                    xw.WriteElementString("FrontFifthServed", aux);
                                    aux = hoist.RearFifthServed ? "True" : "False";
                                    xw.WriteElementString("RearFifthServed", aux);
                                    aux = hoist.FrontSexthServed ? "True" : "False";
                                    xw.WriteElementString("FrontSexthServed", aux);
                                    aux = hoist.RearSexthServed ? "True" : "False";
                                    xw.WriteElementString("RearSexthServed", aux);
                                    aux = hoist.FrontSeventhServed ? "True" : "False";
                                    xw.WriteElementString("FrontSeventhServed", aux);
                                    aux = hoist.RearSeventhServed ? "True" : "False";
                                    xw.WriteElementString("RearSeventhServed", aux);
                                    aux = hoist.FrontEightServed ? "True" : "False";
                                    xw.WriteElementString("FrontEightServed", aux);
                                    aux = hoist.RearEightServed ? "True" : "False";
                                    xw.WriteElementString("RearEightServed", aux);
                                    aux = hoist.FrontNinthServed ? "True" : "False";
                                    xw.WriteElementString("FrontNinthServed", aux);
                                    aux = hoist.RearNinthServed ? "True" : "False";
                                    xw.WriteElementString("RearNinthServed", aux);
                                    aux = hoist.FrontTenthServed ? "True" : "False";
                                    xw.WriteElementString("FrontTenthServed", aux);
                                    aux = hoist.RearTenthServed ? "True" : "False";
                                    xw.WriteElementString("RearTenthServed", aux);
                                    aux = hoist.FrontEleventhServed ? "True" : "False";
                                    xw.WriteElementString("FrontEleventhServed", aux);
                                    aux = hoist.RearEleventhServed ? "True" : "False";
                                    xw.WriteElementString("RearEleventhServed", aux);
                                    aux = hoist.FrontTwelvethServed ? "True" : "False";
                                    xw.WriteElementString("FrontTwelvethServed", aux);
                                    aux = hoist.RearTwelvethServed ? "True" : "False";
                                    xw.WriteElementString("RearTwelvethServed", aux);
                                    aux = hoist.FrontThirteenthServed ? "True" : "False";
                                    xw.WriteElementString("FrontThirteenthServed", aux);
                                    aux = hoist.RearThirteenthServed ? "True" : "False";
                                    xw.WriteElementString("RearThirteenthServed", aux);
                                    aux = hoist.FrontFourteenthServed ? "True" : "False";
                                    xw.WriteElementString("FrontFourteenthServed", aux);
                                    aux = hoist.RearFourteenthServed ? "True" : "False";
                                    xw.WriteElementString("RearFourteenthServed", aux);
                                    aux = hoist.FrontFifteenthServed ? "True" : "False";
                                    xw.WriteElementString("FrontFifteenthServed", aux);
                                    aux = hoist.RearFifteenthServed ? "True" : "False";
                                    xw.WriteElementString("RearFifteenthServed", aux);
                                    aux = hoist.FrontSixteenthServed ? "True" : "False";
                                    xw.WriteElementString("FrontSixteenthServed", aux);
                                    aux = hoist.RearSixteenthServed ? "True" : "False";
                                    xw.WriteElementString("RearSixteenthServed", aux);
                                    xw.WriteEndElement();
                                }


                            }
                            else if (JobTypeName(job.JobTypeID) == "ElmHydro" || JobTypeName(job.JobTypeID) == "ElmTract")
                            {
                                Element element = repository.Elements.First(m => m.JobID == job.JobID);
                                if (element != null)
                                {
                                    string aux;
                                    xw.WriteStartElement("Element");
                                    xw.WriteElementString("ID", element.ElementID.ToString());
                                    xw.WriteElementString("JobID", element.JobID.ToString());

                                    xw.WriteElementString("DoorOperatorID", element.DoorOperatorID.ToString());
                                    xw.WriteElementString("Capacity", element.Capacity.ToString());
                                    xw.WriteElementString("Speed", element.Speed.ToString());
                                    xw.WriteElementString("Voltage", element.Voltage.ToString());
                                    xw.WriteElementString("Phase", element.Phase.ToString());
                                    xw.WriteElementString("Frequency", element.Frequency.ToString());
                                    xw.WriteElementString("LoadWeigher", element.LoadWeigher.ToString());


                                    aux = !string.IsNullOrEmpty(element.DoorGate) ? element.DoorGate : "Nulo";
                                    xw.WriteElementString("DoorGate", aux);
                                    aux = !string.IsNullOrEmpty(element.INA) ? element.INA : "Nulo";
                                    xw.WriteElementString("INA", aux);
                                    aux = !string.IsNullOrEmpty(element.LoadWeigher) ? element.LoadWeigher : "Nulo";
                                    xw.WriteElementString("LoadWeigher", aux);
                                    aux = element.FRON2 ? "True" : "False";
                                    xw.WriteElementString("FRON2", aux);
                                    aux = element.EQ ? "True" : "False";
                                    xw.WriteElementString("EQ", aux);
                                    aux = element.EMT ? "True" : "False";
                                    xw.WriteElementString("EMT", aux);
                                    aux = element.EP ? "True" : "False";
                                    xw.WriteElementString("EP", aux);
                                    aux = element.INCP ? "True" : "False";
                                    xw.WriteElementString("INCP", aux);
                                    aux = element.CTINSPST ? "True" : "False";
                                    xw.WriteElementString("CTINSPST", aux);
                                    aux = element.CallEnable ? "True" : "False";
                                    xw.WriteElementString("CallEnable", aux);
                                    aux = element.CarKey ? "True" : "False";
                                    xw.WriteElementString("CarKey", aux);
                                    aux = element.HallKey ? "True" : "False";
                                    xw.WriteElementString("HallKey", aux);
                                    aux = element.CRO ? "True" : "False";
                                    xw.WriteElementString("CRO", aux);
                                    aux = element.HCRO ? "True" : "False";
                                    xw.WriteElementString("HCRO", aux);
                                    aux = element.CarCardReader ? "True" : "False";
                                    xw.WriteElementString("CarCardReader", aux);
                                    aux = element.HallCardReader ? "True" : "False";
                                    xw.WriteElementString("HallCardReader", aux);
                                    aux = element.HAPS ? "True" : "False";
                                    xw.WriteElementString("HAPS", aux);
                                    aux = element.PSS ? "True" : "False";
                                    xw.WriteElementString("PSS", aux);
                                    aux = element.PTFLD ? "True" : "False";
                                    xw.WriteElementString("PTFLD", aux);
                                    aux = element.VCI ? "True" : "False";
                                    xw.WriteElementString("VCI", aux);
                                    aux = element.CReg ? "True" : "False";
                                    xw.WriteElementString("CReg", aux);
                                    aux = element.Egress ? "True" : "False";
                                    xw.WriteElementString("Egress", aux);
                                    aux = element.PHECutOut ? "True" : "False";
                                    xw.WriteElementString("PHECutOut", aux);
                                    aux = element.Traveler ? "True" : "False";
                                    xw.WriteElementString("Traveler", aux);
                                    aux = element.LOS ? "True" : "False";
                                    xw.WriteElementString("LOS", aux);
                                    aux = element.PFGE ? "True" : "False";
                                    xw.WriteElementString("PFGE", aux);
                                    aux = element.CTL ? "True" : "False";
                                    xw.WriteElementString("CTL", aux);
                                    aux = element.CSD ? "True" : "False";
                                    xw.WriteElementString("CSD", aux);
                                    aux = element.CTF ? "True" : "False";
                                    xw.WriteElementString("CTF", aux);
                                    aux = element.LJ ? "True" : "False";
                                    xw.WriteElementString("LJ", aux);
                                    aux = element.DHLD ? "True" : "False";
                                    xw.WriteElementString("DHLD", aux);
                                    xw.WriteEndElement();
                                }

                                if (JobTypeName(job.JobTypeID) == "ElmHydro")
                                {
                                    ElementHydro hydro = repository.ElementHydros.First(m => m.JobID == job.JobID);
                                    if (hydro != null)
                                    {
                                        xw.WriteStartElement("ElementHydro");
                                        xw.WriteElementString("ID", hydro.ElementHydroID.ToString());
                                        xw.WriteElementString("JobID", hydro.JobID.ToString());
                                        xw.WriteElementString("Starter", hydro.Starter);
                                        xw.WriteElementString("HP", hydro.HP.ToString());
                                        xw.WriteElementString("FLA", hydro.FLA.ToString());
                                        xw.WriteElementString("SPH", hydro.SPH.ToString());
                                        xw.WriteElementString("ValveBrand", hydro.ValveBrand);
                                        xw.WriteEndElement();
                                    }
                                }
                                else
                                {
                                    ElementTraction traction = repository.ElementTractions.First(m => m.JobID == job.JobID);
                                    if (traction != null)
                                    {
                                        string aux;
                                        xw.WriteStartElement("ElementTraction");
                                        xw.WriteElementString("ID", traction.ElementTractionID.ToString());
                                        xw.WriteElementString("JobID", traction.JobID.ToString());
                                        xw.WriteElementString("HP", traction.HP.ToString());
                                        xw.WriteElementString("FLA", traction.FLA.ToString());
                                        xw.WriteElementString("PickVoltage", traction.PickVoltage.ToString());
                                        xw.WriteElementString("HoldVoltage", traction.HoldVoltage.ToString());
                                        xw.WriteElementString("Resistance", traction.Resistance.ToString());
                                        xw.WriteElementString("Current", traction.Current.ToString());

                                        xw.WriteElementString("MachineLocation", traction.MachineLocation);
                                        xw.WriteElementString("VVVF", traction.VVVF);
                                        xw.WriteElementString("MotorBrand", traction.MotorBrand);
                                        xw.WriteElementString("Contact", traction.Contact);

                                        aux = traction.Encoder ? "True" : "False";
                                        xw.WriteElementString("Encoder", aux);
                                        aux = traction.ISO ? "True" : "False";
                                        xw.WriteElementString("ISO", aux);

                                        xw.WriteEndElement();
                                    }
                                }
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
        }
        public static void ImportXML(IServiceProvider services, string buttonImportXML)
        {
            ApplicationDbContext context = services.GetRequiredService<ApplicationDbContext>();

            if (buttonImportXML == "All")
            {
                HtmlDocument doc = new HtmlDocument();
                doc.Load(@"C:\ProdFloorNew90\wwwroot\AppData\Jobs.xml");

                var XMLJobs = doc.DocumentNode.SelectSingleNode("//jobs");
                var XMLJob = XMLJobs.SelectNodes(".//job");

                foreach (var node in XMLJob)
                {
                    var XMLJobBase = node.SelectSingleNode(".//jobbase");
                    var id = XMLJobBase.SelectSingleNode(".//id").InnerText;
                    var status = XMLJobBase.SelectSingleNode(".//status").InnerText;
                    var engid = XMLJobBase.SelectSingleNode(".//engid").InnerText;
                    var crossappengid = XMLJobBase.SelectSingleNode(".//crossappengid").InnerText;
                    var name = XMLJobBase.SelectSingleNode(".//name").InnerText;
                    var name2 = XMLJobBase.SelectSingleNode(".//name2").InnerText;
                    var jobnum = XMLJobBase.SelectSingleNode(".//jobnum").InnerText;
                    var shipdate = XMLJobBase.SelectSingleNode(".//shipdate").InnerText;
                    var latestfinishdate = XMLJobBase.SelectSingleNode(".//latestfinishdate").InnerText;
                    var cust = XMLJobBase.SelectSingleNode(".//cust").InnerText;
                    var contractor = XMLJobBase.SelectSingleNode(".//contractor").InnerText;
                    var jobtypeid = XMLJobBase.SelectSingleNode(".//jobtypeid").InnerText;
                    var cityid = XMLJobBase.SelectSingleNode(".//cityid").InnerText;
                    var fireCodeid = XMLJobBase.SelectSingleNode(".//firecodeid").InnerText;
                    context.Jobs.Add(new Job
                    {
                        JobID = Int32.Parse(id),
                        Status = status,
                        EngID = Int32.Parse(engid),
                        CrossAppEngID = Int32.Parse(crossappengid),
                        Name = name,
                        Name2 = name2,
                        JobNum = Int32.Parse(jobnum),
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

                    var XMPOOOO = node.SelectSingleNode(".//pos");
                    var XMLPOs = XMPOOOO.SelectNodes(".//po");
                    if (XMLPOs != null)
                    {
                        foreach (var po in XMLPOs)
                        {
                            var idpo = po.SelectSingleNode(".//id").InnerText;
                            var idPOnumb = po.SelectSingleNode(".//ponumb").InnerText;
                            context.POs.Add(new PO
                            {
                                POID = Int32.Parse(idpo),
                                JobID = Int32.Parse(id),
                                PONumb = Int32.Parse(idPOnumb)

                            });
                            context.Database.OpenConnection();
                            try
                            {
                                context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.POs ON");
                                context.SaveChanges();
                                context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.POs OFF");
                            }
                            finally
                            {
                                context.Database.CloseConnection();
                            }
                        }
                    }

                    if (int.Parse(jobtypeid) == 2)
                    {


                        var XMLJobExtension = node.SelectSingleNode(".//jobextension");
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
                        var swingop = XMLJobExtension.SelectSingleNode(".//swingop").InnerText;
                        var backupdisp = XMLJobExtension.SelectSingleNode(".//backupdisp").InnerText;
                        var altris = XMLJobExtension.SelectSingleNode(".//altris").InnerText;
                        context.JobsExtensions.Add(new JobExtension
                        {
                            JobExtensionID = Int32.Parse(idEx),
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
                            SwingOp = Boolean.Parse(swingop),
                            AltRis = Boolean.Parse(altris),
                            BackUpDisp = Boolean.Parse(backupdisp),
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




                        var XMLHydro = node.SelectSingleNode(".//hydrospecific");
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
                            FLA = float.Parse(fla),
                            SPH = Int32.Parse(sph),
                            MotorsNum = Int32.Parse(motorsnum),
                            MotorsDisconnect = Int32.Parse(motorsdisconnect),
                            ValveBrand = valvebrand,
                            ValveCoils = Int32.Parse(valvecoils),
                            ValveNum = Int32.Parse(valvenum),
                            ValveVoltage = Int32.Parse(valvevoltage),
                            BatteryBrand = batterybrand == "Nulo" ? null : batterybrand,
                            Battery = Boolean.Parse(battery),
                            LifeJacket = Boolean.Parse(lifejacket),
                            LOS = Boolean.Parse(los),
                            OilCool = Boolean.Parse(oilcool),
                            OilTank = Boolean.Parse(oiltank),
                            PSS = Boolean.Parse(pss),
                            Resync = Boolean.Parse(resync),
                            VCI = Boolean.Parse(vci)
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

                        var XMLGeneric = node.SelectSingleNode(".//genericfeatures");
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
                        var pit = XMLGeneric.SelectSingleNode(".//pit").InnerText;
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
                        var bsi = XMLGeneric.SelectSingleNode(".//bsi").InnerText;
                        var carcallcodesecurity = XMLGeneric.SelectSingleNode(".//carcallcodesecurity").InnerText;
                        var specialinstructions = XMLGeneric.SelectSingleNode(".//specialinstructions").InnerText;
                        context.GenericFeaturesList.Add(new GenericFeatures
                        {
                            GenericFeaturesID = Int32.Parse(idGeneric),
                            JobID = Int32.Parse(jobid),
                            FRON2 = Boolean.Parse(fron2),
                            Attendant = Boolean.Parse(attendant),
                            CarToLobby = Boolean.Parse(cartolobby),
                            EQ = Boolean.Parse(eq),
                            EMT = Boolean.Parse(emt),
                            EP = Boolean.Parse(ep),
                            EPVoltage = Boolean.Parse(epvoltage),
                            EPOtherCars = Boolean.Parse(epothercars),
                            EPCarsNumber = epcarsnumber == "Nulo" ? null : epcarsnumber,
                            EPContact = epcontact == "Nulo" ? null : epcontact,
                            PTI = Boolean.Parse(pti),
                            EPSelect = Boolean.Parse(epselect),
                            FLO = Boolean.Parse(flo),
                            Hosp = Boolean.Parse(hosp),
                            Pit = Boolean.Parse(pit),
                            INA = Boolean.Parse(ina),
                            TopAccess = Boolean.Parse(topaccess),
                            TopAccessLocation = topaccesslocation,
                            BottomAccess = Boolean.Parse(bottomaccess),
                            BottomAccessLocation = bottomaccesslocation == "Nulo" ? null : bottomaccesslocation,
                            INCP = Boolean.Parse(incp),
                            INCPButtons = incpbuttons == "Nulo" ? null : incpbuttons,
                            SwitchStyle = switchstyle == "Nulo" ? null : switchstyle,
                            LoadWeigher = Boolean.Parse(loadweigher),
                            CTINSPST = Boolean.Parse(ctinspst),
                            Roped = Boolean.Parse(roped),
                            GovModel = govmodel == "Nulo" ? null : govmodel,
                            Monitoring = monitoring == "Nulo" ? null : monitoring,
                            CallEnable = Boolean.Parse(callenable),
                            CarCallRead = Boolean.Parse(carcallread),
                            HallCallRead = Boolean.Parse(hallcallread),
                            CarKey = Boolean.Parse(carkey),
                            HallKey = Boolean.Parse(hallkey),
                            CRO = Boolean.Parse(cro),
                            HCRO = Boolean.Parse(hcro),
                            BSI = Boolean.Parse(bsi),
                            CarCallCodeSecurity = carcallcodesecurity == "Nulo" ? null : carcallcodesecurity,
                            SpecialInstructions = specialinstructions == "Nulo" ? null : specialinstructions
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

                        var XMLIndicator = node.SelectSingleNode(".//indicator");
                        var idIndicator = XMLIndicator.SelectSingleNode(".//id").InnerText;
                        var carcallsvoltage = XMLIndicator.SelectSingleNode(".//carcallsvoltage").InnerText;
                        var carcallsvoltagetype = XMLIndicator.SelectSingleNode(".//carcallsvoltagetype").InnerText;
                        var carcallstype = XMLIndicator.SelectSingleNode(".//carcallstype").InnerText;
                        var hallcallsvoltage = XMLIndicator.SelectSingleNode(".//hallcallsvoltage").InnerText;
                        var hallcallsvoltagetype = XMLIndicator.SelectSingleNode(".//hallcallsvoltagetype").InnerText;
                        var hallcallstype = XMLIndicator.SelectSingleNode(".//hallcallstype").InnerText;
                        var carpi = XMLIndicator.SelectSingleNode(".//carpi").InnerText;
                        var carpitype = XMLIndicator.SelectSingleNode(".//carpitype").InnerText;
                        var carpidiscretetype = XMLIndicator.SelectSingleNode(".//carpidiscretetype").InnerText;
                        var hallpi = XMLIndicator.SelectSingleNode(".//hallpi").InnerText;
                        var hallpitype = XMLIndicator.SelectSingleNode(".//hallpitype").InnerText;
                        var hallpidiscretetype = XMLIndicator.SelectSingleNode(".//hallpidiscretetype").InnerText;
                        var voiceannunciationpi = XMLIndicator.SelectSingleNode(".//voiceannunciationpi").InnerText;
                        var voiceannunciationpitype = XMLIndicator.SelectSingleNode(".//voiceannunciationpitype").InnerText;
                        var carlanterns = XMLIndicator.SelectSingleNode(".//carlanterns").InnerText;
                        var carlanternsstyle = XMLIndicator.SelectSingleNode(".//carlanternsstyle").InnerText;
                        var carlanternstype = XMLIndicator.SelectSingleNode(".//carlanternstype").InnerText;
                        var halllanterns = XMLIndicator.SelectSingleNode(".//halllanterns").InnerText;
                        var hallpiAll = XMLIndicator.SelectSingleNode(".//hallpiall").InnerText;
                        var halllanternsstyle = XMLIndicator.SelectSingleNode(".//halllanternsstyle").InnerText;
                        var halllanternstype = XMLIndicator.SelectSingleNode(".//halllanternstype").InnerText;
                        var passingfloor = XMLIndicator.SelectSingleNode(".//passingfloor").InnerText;
                        var passingfloortype = XMLIndicator.SelectSingleNode(".//passingfloortype").InnerText;
                        var passingfloordiscretetype = XMLIndicator.SelectSingleNode(".//passingfloordiscretetype").InnerText;
                        var passingfloorenable = XMLIndicator.SelectSingleNode(".//passingfloorenable").InnerText;
                        var indicatorsvoltage = XMLIndicator.SelectSingleNode(".//indicatorsvoltage").InnerText;
                        var indicatorsvoltagetype = XMLIndicator.SelectSingleNode(".//indicatorsvoltagetype").InnerText;

                        context.Indicators.Add(new Indicator
                        {
                            IndicatorID = Int32.Parse(idIndicator),
                            JobID = Int32.Parse(jobid),
                            CarCallsVoltage = carcallsvoltage,
                            CarCallsVoltageType = carcallsvoltagetype,
                            CarCallsType = carcallstype,
                            HallCallsVoltage = hallcallsvoltage,
                            HallCallsVoltageType = hallcallsvoltagetype,
                            HallCallsType = hallcallstype,
                            CarPI = Boolean.Parse(carpi),
                            CarPIType = carpitype == "Nulo" ? null : carpitype,
                            CarPIDiscreteType = carpidiscretetype == "Nulo" ? null : carpidiscretetype,
                            HallPI = Boolean.Parse(hallpi),
                            HallPIAll = Boolean.Parse(hallpiAll),
                            HallPIType = hallpitype == "Nulo" ? "" : hallpitype,
                            HallPIDiscreteType = hallpidiscretetype == "Nulo" ? null : hallpidiscretetype,
                            VoiceAnnunciationPI = Boolean.Parse(voiceannunciationpi),
                            VoiceAnnunciationPIType = voiceannunciationpi == "Nulo" ? null : voiceannunciationpitype,
                            CarLanterns = Boolean.Parse(carlanterns),
                            CarLanternsStyle = carlanternsstyle == "Nulo" ? null : carlanternsstyle,
                            CarLanternsType = carlanternstype == "Nulo" ? null : carlanternstype,
                            HallLanterns = Boolean.Parse(halllanterns),
                            HallLanternsStyle = halllanternsstyle == "Nulo" ? null : halllanternsstyle,
                            HallLanternsType = halllanternstype == "Nulo" ? null : halllanternstype,
                            PassingFloor = Boolean.Parse(passingfloor),
                            PassingFloorType = passingfloortype == "Nulo" ? null : passingfloortype,
                            PassingFloorDiscreteType = passingfloordiscretetype == "Nulo" ? null : passingfloordiscretetype,
                            PassingFloorEnable = Boolean.Parse(passingfloorenable),
                            IndicatorsVoltage = Int32.Parse(indicatorsvoltage),
                            IndicatorsVoltageType = indicatorsvoltagetype == "Nulo" ? null : indicatorsvoltagetype

                        });
                        context.Database.OpenConnection();
                        try
                        {
                            context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Indicators ON");
                            context.SaveChanges();
                            context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Indicators OFF");
                        }
                        finally
                        {
                            context.Database.CloseConnection();
                        }


                        var XMLHoist = node.SelectSingleNode(".//hoistwaydata");
                        var idHoist = XMLHoist.SelectSingleNode(".//id").InnerText;
                        var hoistwaysnumber = XMLHoist.SelectSingleNode(".//hoistwaysnumber").InnerText;
                        var machinerooms = XMLHoist.SelectSingleNode(".//machinerooms").InnerText;
                        var capacity = XMLHoist.SelectSingleNode(".//capacity").InnerText;
                        var upspeed = XMLHoist.SelectSingleNode(".//upspeed").InnerText;
                        var downspeed = XMLHoist.SelectSingleNode(".//downspeed").InnerText;
                        var totaltravel = XMLHoist.SelectSingleNode(".//totaltravel").InnerText;
                        var landingsystemid = XMLHoist.SelectSingleNode(".//landingsystemid").InnerText;
                        var frontfirstserved = XMLHoist.SelectSingleNode(".//frontfirstserved").InnerText;
                        var rearfirstserved = XMLHoist.SelectSingleNode(".//rearfirstserved").InnerText;
                        var frontsecondserved = XMLHoist.SelectSingleNode(".//frontsecondserved").InnerText;
                        var rearsecondserved = XMLHoist.SelectSingleNode(".//rearsecondserved").InnerText;
                        var frontthirdserved = XMLHoist.SelectSingleNode(".//frontthirdserved").InnerText;
                        var rearthirdserved = XMLHoist.SelectSingleNode(".//rearthirdserved").InnerText;
                        var frontfourthserved = XMLHoist.SelectSingleNode(".//frontfourthserved").InnerText;
                        var rearfourthserved = XMLHoist.SelectSingleNode(".//rearfourthserved").InnerText;
                        var frontfifthserved = XMLHoist.SelectSingleNode(".//frontfifthserved").InnerText;
                        var rearfifthserved = XMLHoist.SelectSingleNode(".//rearfifthserved").InnerText;
                        var frontsexthserved = XMLHoist.SelectSingleNode(".//frontsexthserved").InnerText;
                        var rearsexthserved = XMLHoist.SelectSingleNode(".//rearsexthserved").InnerText;
                        var frontseventhserved = XMLHoist.SelectSingleNode(".//frontseventhserved").InnerText;
                        var rearseventhserved = XMLHoist.SelectSingleNode(".//rearseventhserved").InnerText;
                        var fronteightserved = XMLHoist.SelectSingleNode(".//fronteightserved").InnerText;
                        var reareightserved = XMLHoist.SelectSingleNode(".//reareightserved").InnerText;
                        var frontninthserved = XMLHoist.SelectSingleNode(".//frontninthserved").InnerText;
                        var rearninthserved = XMLHoist.SelectSingleNode(".//rearninthserved").InnerText;
                        var fronttenthserved = XMLHoist.SelectSingleNode(".//fronttenthserved").InnerText;
                        var reartenthserved = XMLHoist.SelectSingleNode(".//reartenthserved").InnerText;
                        var fronteleventhserved = XMLHoist.SelectSingleNode(".//fronteleventhserved").InnerText;
                        var reareleventhserved = XMLHoist.SelectSingleNode(".//reareleventhserved").InnerText;
                        var fronttwelvethserved = XMLHoist.SelectSingleNode(".//fronttwelvethserved").InnerText;
                        var reartwelvethserved = XMLHoist.SelectSingleNode(".//reartwelvethserved").InnerText;
                        var frontthirteenthserved = XMLHoist.SelectSingleNode(".//frontthirteenthserved").InnerText;
                        var rearthirteenthserved = XMLHoist.SelectSingleNode(".//rearthirteenthserved").InnerText;
                        var frontfourteenthserved = XMLHoist.SelectSingleNode(".//frontfourteenthserved").InnerText;
                        var rearfourteenthserved = XMLHoist.SelectSingleNode(".//rearfourteenthserved").InnerText;
                        var frontfifteenthserved = XMLHoist.SelectSingleNode(".//frontfifteenthserved").InnerText;
                        var rearfifteenthserved = XMLHoist.SelectSingleNode(".//rearfifteenthserved").InnerText;
                        var frontsixteenthserved = XMLHoist.SelectSingleNode(".//frontsixteenthserved").InnerText;
                        var rearsixteenthserved = XMLHoist.SelectSingleNode(".//rearsixteenthserved").InnerText;

                        context.HoistWayDatas.Add(new HoistWayData
                        {
                            HoistWayDataID = Int32.Parse(idHoist),
                            JobID = Int32.Parse(jobid),
                            HoistWaysNumber = Int32.Parse(hoistwaysnumber),
                            MachineRooms = Int32.Parse(machinerooms),
                            Capacity = Int32.Parse(capacity),
                            UpSpeed = Int32.Parse(upspeed),
                            DownSpeed = Int32.Parse(downspeed),
                            TotalTravel = Int32.Parse(totaltravel),
                            LandingSystemID = Int32.Parse(landingsystemid),
                            FrontFirstServed = Boolean.Parse(frontfirstserved),
                            RearFirstServed = Boolean.Parse(rearfirstserved),
                            FrontSecondServed = Boolean.Parse(frontsecondserved),
                            RearSecondServed = Boolean.Parse(rearsecondserved),
                            FrontThirdServed = Boolean.Parse(frontthirdserved),
                            RearThirdServed = Boolean.Parse(rearthirdserved),
                            FrontFourthServed = Boolean.Parse(frontfourthserved),
                            RearFourthServed = Boolean.Parse(rearfourthserved),
                            FrontFifthServed = Boolean.Parse(frontfifthserved),
                            RearFifthServed = Boolean.Parse(rearfifthserved),
                            FrontSexthServed = Boolean.Parse(frontsexthserved),
                            RearSexthServed = Boolean.Parse(rearsexthserved),
                            FrontSeventhServed = Boolean.Parse(frontseventhserved),
                            RearSeventhServed = Boolean.Parse(rearseventhserved),
                            FrontEightServed = Boolean.Parse(fronteightserved),
                            RearEightServed = Boolean.Parse(reareightserved),
                            FrontNinthServed = Boolean.Parse(frontninthserved),
                            RearNinthServed = Boolean.Parse(rearninthserved),
                            FrontTenthServed = Boolean.Parse(fronttenthserved),
                            RearTenthServed = Boolean.Parse(reartenthserved),
                            FrontEleventhServed = Boolean.Parse(fronteleventhserved),
                            RearEleventhServed = Boolean.Parse(reareleventhserved),
                            FrontTwelvethServed = Boolean.Parse(fronttwelvethserved),
                            RearTwelvethServed = Boolean.Parse(reartwelvethserved),
                            FrontThirteenthServed = Boolean.Parse(frontthirteenthserved),
                            RearThirteenthServed = Boolean.Parse(rearthirteenthserved),
                            FrontFourteenthServed = Boolean.Parse(frontfourteenthserved),
                            RearFourteenthServed = Boolean.Parse(rearfourteenthserved),
                            FrontFifteenthServed = Boolean.Parse(frontfifteenthserved),
                            RearFifteenthServed = Boolean.Parse(rearfifteenthserved),
                            FrontSixteenthServed = Boolean.Parse(frontsixteenthserved),
                            RearSixteenthServed = Boolean.Parse(rearsixteenthserved),
                        });
                        context.Database.OpenConnection();
                        try
                        {
                            context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.HoistWayDatas ON");
                            context.SaveChanges();
                            context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.HoistWayDatas OFF");
                        }
                        finally
                        {
                            context.Database.CloseConnection();
                        }
                    }
                    else if (int.Parse(jobtypeid) == 1 || int.Parse(jobtypeid) == 5)
                    {
                        var xmlElement = node.SelectSingleNode(".//element");
                        var idElm = xmlElement.SelectSingleNode(".//id").InnerText;
                        var jobid = xmlElement.SelectSingleNode(".//jobid").InnerText;

                        var dooroperatorid = xmlElement.SelectSingleNode(".//dooroperatorid").InnerText;
                        var capacity = xmlElement.SelectSingleNode(".//capacity").InnerText;
                        var speed = xmlElement.SelectSingleNode(".//speed").InnerText;
                        var voltage = xmlElement.SelectSingleNode(".//voltage").InnerText;
                        var phase = xmlElement.SelectSingleNode(".//phase").InnerText;
                        var frequency = xmlElement.SelectSingleNode(".//frequency").InnerText;


                        var doorgate = xmlElement.SelectSingleNode(".//doorgate").InnerText;
                        var ina = xmlElement.SelectSingleNode(".//ina").InnerText;
                        var loadweigher = xmlElement.SelectSingleNode(".//loadweigher").InnerText;


                        var incp = xmlElement.SelectSingleNode(".//incp").InnerText;
                        var carkey = xmlElement.SelectSingleNode(".//carkey").InnerText;
                        var carcardreader = xmlElement.SelectSingleNode(".//carcardreader").InnerText;
                        var cro = xmlElement.SelectSingleNode(".//cro").InnerText;
                        var hallkey = xmlElement.SelectSingleNode(".//hallkey").InnerText;
                        var hallcardreader = xmlElement.SelectSingleNode(".//hallcardreader").InnerText;
                        var hcro = xmlElement.SelectSingleNode(".//hcro").InnerText;
                        var callenable = xmlElement.SelectSingleNode(".//callenable").InnerText;
                        var haps = xmlElement.SelectSingleNode(".//haps").InnerText;
                        var ep = xmlElement.SelectSingleNode(".//ep").InnerText;
                        var emt = xmlElement.SelectSingleNode(".//emt").InnerText;
                        var pss = xmlElement.SelectSingleNode(".//pss").InnerText;
                        var ptfld = xmlElement.SelectSingleNode(".//ptfld").InnerText;
                        var vci = xmlElement.SelectSingleNode(".//vci").InnerText;
                        var creg = xmlElement.SelectSingleNode(".//creg").InnerText;
                        var egress = xmlElement.SelectSingleNode(".//egress").InnerText;
                        var phecutout = xmlElement.SelectSingleNode(".//phecutout").InnerText;
                        var ctinspst = xmlElement.SelectSingleNode(".//ctinspst").InnerText;
                        var traveler = xmlElement.SelectSingleNode(".//traveler").InnerText;
                        var los = xmlElement.SelectSingleNode(".//los").InnerText;
                        var pfge = xmlElement.SelectSingleNode(".//pfge").InnerText;
                        var fron2 = xmlElement.SelectSingleNode(".//fron2").InnerText;
                        var ctl = xmlElement.SelectSingleNode(".//ctl").InnerText;
                        var csd = xmlElement.SelectSingleNode(".//csd").InnerText;
                        var ctf = xmlElement.SelectSingleNode(".//ctf").InnerText;
                        var eq = xmlElement.SelectSingleNode(".//eq").InnerText;
                        var lj = xmlElement.SelectSingleNode(".//lj").InnerText;
                        var dhld = xmlElement.SelectSingleNode(".//dhld").InnerText;
                        context.Elements.Add(new Element
                        {
                            ElementID = Int32.Parse(idElm),
                            JobID = Int32.Parse(jobid),

                            DoorOperatorID = Int32.Parse(dooroperatorid),
                            Capacity = Int32.Parse(capacity),
                            Speed = Int32.Parse(speed),
                            Voltage = Int32.Parse(voltage),
                            Phase = Int32.Parse(phase),
                            Frequency = Int32.Parse(frequency),

                            DoorGate = doorgate,
                            INA = ina,
                            LoadWeigher = loadweigher,

                            INCP = Boolean.Parse(incp),
                            CarKey = Boolean.Parse(carkey),
                            CarCardReader = Boolean.Parse(carcardreader),
                            CRO = Boolean.Parse(cro),
                            HallKey = Boolean.Parse(hallkey),
                            HallCardReader = Boolean.Parse(hallcardreader),
                            HCRO = Boolean.Parse(hcro),
                            CallEnable = Boolean.Parse(callenable),
                            HAPS = Boolean.Parse(haps),
                            EP = Boolean.Parse(ep),
                            EMT = Boolean.Parse(emt),
                            PSS = Boolean.Parse(pss),
                            PTFLD = Boolean.Parse(ptfld),
                            VCI = Boolean.Parse(vci),
                            CReg = Boolean.Parse(creg),
                            Egress = Boolean.Parse(egress),
                            PHECutOut = Boolean.Parse(phecutout),
                            CTINSPST = Boolean.Parse(ctinspst),
                            Traveler = Boolean.Parse(traveler),
                            LOS = Boolean.Parse(los),
                            PFGE = Boolean.Parse(pfge),
                            FRON2 = Boolean.Parse(fron2),
                            CTL = Boolean.Parse(ctl),
                            CSD = Boolean.Parse(csd),
                            CTF = Boolean.Parse(ctf),
                            EQ = Boolean.Parse(eq),
                            LJ = Boolean.Parse(lj),
                            DHLD = Boolean.Parse(dhld),
                            LandingSystemID = 5,

                        });
                        context.Database.OpenConnection();
                        try
                        {
                            context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Elements ON");
                            context.SaveChanges();
                            context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Elements OFF");
                        }
                        finally
                        {
                            context.Database.CloseConnection();
                        }

                        if (int.Parse(jobtypeid) == 1)
                        {
                            var xmlElementHydro = node.SelectSingleNode(".//elementhydro");
                            var idElmh = xmlElementHydro.SelectSingleNode(".//id").InnerText;

                            var starter = xmlElementHydro.SelectSingleNode(".//starter").InnerText;
                            var hp = xmlElementHydro.SelectSingleNode(".//hp").InnerText;
                            var fla = xmlElementHydro.SelectSingleNode(".//fla").InnerText;
                            var sph = xmlElementHydro.SelectSingleNode(".//sph").InnerText;
                            var valvebrand = xmlElementHydro.SelectSingleNode(".//valvebrand").InnerText;
                            context.ElementHydros.Add(new ElementHydro
                            {
                                ElementHydroID = Int32.Parse(idElmh),
                                JobID = Int32.Parse(jobid),
                                
                                Starter = starter,
                                HP = float.Parse(hp),
                                FLA = float.Parse(fla),
                                SPH = Int32.Parse(sph),
                                ValveBrand = valvebrand,

                            });
                            context.Database.OpenConnection();
                            try
                            {
                                context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.ElementHydros ON");
                                context.SaveChanges();
                                context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.ElementHydros OFF");
                            }
                            finally
                            {
                                context.Database.CloseConnection();
                            }
                        }
                        else
                        {
                            var xmlElementTraction = node.SelectSingleNode(".//elementtraction");
                            var idElmt = xmlElementTraction.SelectSingleNode(".//id").InnerText;

                            var machinelocation = xmlElementTraction.SelectSingleNode(".//machinelocation").InnerText;
                            var vvvf = xmlElementTraction.SelectSingleNode(".//vvvf").InnerText;
                            var motorbrand = xmlElementTraction.SelectSingleNode(".//motorbrand").InnerText;
                            var contact = xmlElementTraction.SelectSingleNode(".//contact").InnerText;
                            var encoder = xmlElementTraction.SelectSingleNode(".//encoder").InnerText;
                            var iso = xmlElementTraction.SelectSingleNode(".//iso").InnerText;
                            var hp = xmlElementTraction.SelectSingleNode(".//hp").InnerText;
                            var fla = xmlElementTraction.SelectSingleNode(".//fla").InnerText;
                            var pickvoltage = xmlElementTraction.SelectSingleNode(".//pickvoltage").InnerText;
                            var holdvoltage = xmlElementTraction.SelectSingleNode(".//holdvoltage").InnerText;
                            var resistance = xmlElementTraction.SelectSingleNode("./resistance").InnerText;
                            var current = xmlElementTraction.SelectSingleNode(".//current").InnerText;

                            context.ElementTractions.Add(new ElementTraction
                            {
                                ElementTractionID = Int32.Parse(idElmt),
                                JobID = Int32.Parse(jobid),

                                MachineLocation = machinelocation,
                                VVVF = vvvf,
                                MotorBrand = motorbrand,
                                Contact = contact,
                                HP = float.Parse(hp),
                                FLA = float.Parse(fla),
                                PickVoltage = Int32.Parse(pickvoltage),
                                HoldVoltage = Int32.Parse(holdvoltage),
                                Resistance = Int32.Parse(resistance),
                                Current = float.Parse(current),


                                Encoder = Boolean.Parse(encoder),
                                ISO = Boolean.Parse(iso),

                            });
                            context.Database.OpenConnection();
                            try
                            {
                                context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.ElementTractions ON");
                                context.SaveChanges();
                                context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.ElementTractions OFF");
                            }
                            finally
                            {
                                context.Database.CloseConnection();
                            }
                        }
                    }


                    var XMLSpecialF = node.SelectSingleNode(".//specialfeatures");
                    var XMLFeatures = XMLSpecialF.SelectNodes(".//specialfeature");
                    if (XMLFeatures != null)
                    {
                        foreach (var Feature in XMLFeatures)
                        {
                            var idFeature = Feature.SelectSingleNode(".//id").InnerText;
                            var description = Feature.SelectSingleNode(".//description").InnerText;
                            context.SpecialFeatures.Add(new SpecialFeatures
                            {
                                SpecialFeaturesID = Int32.Parse(idFeature),
                                JobID = Int32.Parse(id),
                                Description = description

                            });
                            context.Database.OpenConnection();
                            try
                            {
                                context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.SpecialFeatures ON");
                                context.SaveChanges();
                                context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.SpecialFeatures OFF");
                            }
                            finally
                            {
                                context.Database.CloseConnection();
                            }
                        }
                    }
                    else
                    {
                        context.SpecialFeatures.Add(new SpecialFeatures
                        {
                            JobID = Int32.Parse(id),
                            Description = null

                        });
                        context.SaveChanges();
                    }
                }

            }

        }



        [HttpPost]
        public IActionResult SeedXML(string buttonImportXML)
        {
            JobController.ImportXML(HttpContext.RequestServices, buttonImportXML);
            return RedirectToAction(nameof(List));
        }

        public string JobTypeName(int ID)
        {
            return itemsrepository.JobTypes.FirstOrDefault(m => m.JobTypeID == ID).Name;
        }


        public IActionResult Search()
        {
            try
            {
                string term = HttpContext.Request.Query["term"].ToString();
                var names = repository.SpecialFeatures.Where(p => p.Description.Contains(term)).Select(p => p.Description).Distinct().ToList();
                return Ok(names);
            }
            catch
            {
                return BadRequest();
            }
        }

        public List<SpecialFeaturesEX> getSpecialFeaturesEX()
        {
            List<SpecialFeaturesEX> specialFeaturesTable = new List<SpecialFeaturesEX>();
            List<string> rowList = new List<string>();
            ISheet sheet;
            using (var stream = new FileStream(@"wwwroot\resources\SpecialFeaturesEX.xlsx", FileMode.Open))
            {
                stream.Position = 0;
                XSSFWorkbook xssWorkbook = new XSSFWorkbook(stream);
                sheet = xssWorkbook.GetSheetAt(0);
                IRow headerRow = sheet.GetRow(0);
                int cellCount = headerRow.LastCellNum;
                for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                {
                    SpecialFeaturesEX specialFeaturesEXItem = new SpecialFeaturesEX();
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;
                    if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                    for (int j = row.FirstCellNum; j < cellCount; j++)
                    {
                        if (row.GetCell(j) != null)
                        {
                            if (!string.IsNullOrEmpty(row.GetCell(j).ToString()) & !string.IsNullOrWhiteSpace(row.GetCell(j).ToString()))
                            {
                                rowList.Add(row.GetCell(j).ToString());
                             }
                        }
                    }
                    if (rowList.Count > 0)
                        specialFeaturesEXItem.Name = rowList.ElementAt(0);
                        specialFeaturesEXItem.Description = rowList.ElementAt(1);
                        specialFeaturesTable.Add(specialFeaturesEXItem);
                    rowList.Clear(); 
                }
            }

            return specialFeaturesTable;
        }


    }
}
