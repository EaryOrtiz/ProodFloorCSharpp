using Microsoft.AspNetCore.Mvc;
using ProdFloor.Models;
using ProdFloor.Models.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using ProdFloor.Models.ViewModels.Item;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;

namespace ProdFloor.Controllers
{
    public class ItemController : Controller
    {
        private IItemRepository repository;
        private IJobRepository jobrepo;
        private ITestingRepository testRepo;
        private IHostingEnvironment _env;

        public int PageSize = 4;
        string appDataFolder => _env.WebRootPath.ToString() + @"\AppData\";

        public ItemController(IItemRepository repo,
            IJobRepository jobRepository,
            ITestingRepository testingRepo,
            IHostingEnvironment env)
        {
            repository = repo;
            jobrepo = jobRepository;
            testRepo = testingRepo;
            _env = env;
        }


        public ViewResult Index() => View();

        [HttpPost]
        public FileStreamResult ExportToXML(string buttonXML)
        {
            MemoryStream ms = new MemoryStream();
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.OmitXmlDeclaration = true;
            xws.Indent = true;

            switch (buttonXML)
            {
                case "Countries":
                    List<Country> countries = new List<Country>();
                    countries = repository.Countries.ToList();


                    using (XmlWriter xw = XmlWriter.Create(ms, xws))
                    {
                        xw.WriteStartDocument();
                        xw.WriteStartElement("Countries");

                        foreach (Country country in countries)
                        {
                            xw.WriteStartElement("Country");

                            xw.WriteElementString("ID", country.CountryID.ToString());
                            xw.WriteElementString("Name", country.Name);
                            xw.WriteEndElement();
                        }

                        xw.WriteEndElement();
                        xw.WriteEndDocument();
                    }
                    ms.Position = 0;
                    return File(ms, "text/xml", "Countries.xml");

                case "Cities":
                    List<City> cities = new List<City>();
                    cities = repository.Cities.ToList();


                    using (XmlWriter xw = XmlWriter.Create(ms, xws))
                    {
                        xw.WriteStartDocument();
                        xw.WriteStartElement("Cities");

                        foreach (City city in cities)
                        {
                            xw.WriteStartElement("City");

                            xw.WriteElementString("ID", city.CityID.ToString());
                            xw.WriteElementString("Name", city.Name);
                            xw.WriteElementString("StateID", city.StateID.ToString());
                            xw.WriteElementString("FireCodeID", city.FirecodeID.ToString());
                            xw.WriteEndElement();
                        }

                        xw.WriteEndElement();
                        xw.WriteEndDocument();
                    }
                    ms.Position = 0;
                    return File(ms, "text/xml", "Cities.xml");

                case "DoorOperators":
                    List<DoorOperator> doors = new List<DoorOperator>();
                    doors = repository.DoorOperators.ToList();


                    using (XmlWriter xw = XmlWriter.Create(ms, xws))
                    {
                        xw.WriteStartDocument();
                        xw.WriteStartElement("DoorOperators");

                        foreach (DoorOperator door in doors)
                        {
                            xw.WriteStartElement("DoorOperator");

                            xw.WriteElementString("ID", door.DoorOperatorID.ToString());
                            xw.WriteElementString("Style", door.Style);
                            xw.WriteElementString("Branch", door.Brand);
                            xw.WriteElementString("Name", door.Name);
                            xw.WriteEndElement();
                        }

                        xw.WriteEndElement();
                        xw.WriteEndDocument();
                    }
                    ms.Position = 0;
                    return File(ms, "text/xml", "DoorOperators.xml");

                case "LandingSys":
                    List<LandingSystem> landings = new List<LandingSystem>();
                    landings = repository.LandingSystems.ToList();


                    using (XmlWriter xw = XmlWriter.Create(ms, xws))
                    {
                        xw.WriteStartDocument();
                        xw.WriteStartElement("LandingSystems");

                        foreach (LandingSystem landinsys in landings)
                        {
                            xw.WriteStartElement("LandingSystem");

                            xw.WriteElementString("ID", landinsys.LandingSystemID.ToString());
                            xw.WriteElementString("UsedIn", landinsys.UsedIn.ToString());
                            xw.WriteElementString("Name", landinsys.Name);
                            xw.WriteEndElement();
                        }

                        xw.WriteEndElement();
                        xw.WriteEndDocument();
                    }
                    ms.Position = 0;
                    return File(ms, "text/xml", "LandingSystems.xml");

                case "States":
                    List<State> states = new List<State>();
                    states = repository.States.ToList();


                    using (XmlWriter xw = XmlWriter.Create(ms, xws))
                    {
                        xw.WriteStartDocument();
                        xw.WriteStartElement("States");

                        foreach (State state in states)
                        {
                            xw.WriteStartElement("State");

                            xw.WriteElementString("ID", state.StateID.ToString());
                            xw.WriteElementString("Name", state.Name);
                            xw.WriteElementString("CountryID", state.CountryID.ToString());
                            xw.WriteEndElement();
                        }

                        xw.WriteEndElement();
                        xw.WriteEndDocument();
                    }
                    ms.Position = 0;
                    return File(ms, "text/xml", "States.xml");

                case "FireCodes":
                    List<FireCode> fireCodes = new List<FireCode>();
                    fireCodes = repository.FireCodes.ToList();


                    using (XmlWriter xw = XmlWriter.Create(ms, xws))
                    {
                        xw.WriteStartDocument();
                        xw.WriteStartElement("FireCodes");

                        foreach (FireCode fireCode in fireCodes)
                        {
                            xw.WriteStartElement("FireCode");

                            xw.WriteElementString("ID", fireCode.FireCodeID.ToString());
                            xw.WriteElementString("Name", fireCode.Name);
                            xw.WriteEndElement();
                        }

                        xw.WriteEndElement();
                        xw.WriteEndDocument();
                    }
                    ms.Position = 0;
                    return File(ms, "text/xml", "FireCodes.xml");

                case "JobTypes":
                    List<JobType> jobTypes = new List<JobType>();
                    jobTypes = repository.JobTypes.ToList();


                    using (XmlWriter xw = XmlWriter.Create(ms, xws))
                    {
                        xw.WriteStartDocument();
                        xw.WriteStartElement("JobTypes");

                        foreach (JobType jobType in jobTypes)
                        {
                            xw.WriteStartElement("JobType");

                            xw.WriteElementString("ID", jobType.JobTypeID.ToString());
                            xw.WriteElementString("Name", jobType.Name);
                            xw.WriteEndElement();
                        }

                        xw.WriteEndElement();
                        xw.WriteEndDocument();
                    }
                    ms.Position = 0;
                    return File(ms, "text/xml", "JobTypes.xml");

                case "Slowdowns":
                    List<Slowdown> slowdowns = new List<Slowdown>();
                    slowdowns = repository.Slowdowns.ToList();


                    using (XmlWriter xw = XmlWriter.Create(ms, xws))
                    {
                        xw.WriteStartDocument();
                        xw.WriteStartElement("Slowdowns");

                        foreach (Slowdown slowdown in slowdowns)
                        {
                            xw.WriteStartElement("Slowdown");

                            xw.WriteElementString("ID", slowdown.SlowdownID.ToString());
                            xw.WriteElementString("CarSpeedFPM", slowdown.CarSpeedFPM.ToString());
                            xw.WriteElementString("Distance", slowdown.Distance.ToString());
                            xw.WriteElementString("A", slowdown.A.ToString());
                            xw.WriteElementString("SlowLimit", slowdown.SlowLimit.ToString());
                            xw.WriteElementString("MiniumFloorHeight", slowdown.MiniumFloorHeight.ToString());
                            xw.WriteEndElement();
                        }

                        xw.WriteEndElement();
                        xw.WriteEndDocument();
                    }
                    ms.Position = 0;
                    return File(ms, "text/xml", "Slowdowns.xml");

                case "Starters":
                    List<Starter> starters = new List<Starter>();
                    starters = repository.Starters.ToList();


                    using (XmlWriter xw = XmlWriter.Create(ms, xws))
                    {
                        xw.WriteStartDocument();
                        xw.WriteStartElement("Starters");

                        foreach (Starter starter in starters)
                        {
                            xw.WriteStartElement("Starter");

                            xw.WriteElementString("ID", starter.StarterID.ToString());
                            xw.WriteElementString("FLA", starter.FLA.ToString());
                            xw.WriteElementString("Type", starter.StarterType);
                            xw.WriteElementString("StarterType", starter.StarterType);
                            xw.WriteElementString("FLA", starter.FLA.ToString());
                            xw.WriteElementString("Volts", starter.Volts);
                            xw.WriteElementString("HP", starter.HP.ToString());
                            xw.WriteElementString("MCEPart", starter.MCPart);
                            xw.WriteElementString("NewManufacturerPart", starter.NewManufacturerPart);
                            xw.WriteElementString("OverloadTable", starter.OverloadTable.ToString());
                            xw.WriteEndElement();
                        }

                        xw.WriteEndElement();
                        xw.WriteEndDocument();
                    }
                    ms.Position = 0;
                    return File(ms, "text/xml", "Starters.xml");

                case "WireTypesSizes":
                    List<WireTypesSize> wireTypesSizes = new List<WireTypesSize>();
                    wireTypesSizes = repository.WireTypesSizes.ToList();


                    using (XmlWriter xw = XmlWriter.Create(ms, xws))
                    {
                        xw.WriteStartDocument();
                        xw.WriteStartElement("WireTypesSizes");

                        foreach (WireTypesSize wireTypesSize in wireTypesSizes)
                        {
                            xw.WriteStartElement("WireTypesSize");

                            xw.WriteElementString("ID", wireTypesSize.WireTypesSizeID.ToString());
                            xw.WriteElementString("Type", wireTypesSize.Type);
                            xw.WriteElementString("Size", wireTypesSize.Size);
                            xw.WriteElementString("AMPRating", wireTypesSize.AMPRating.ToString());
                            xw.WriteEndElement();
                        }

                        xw.WriteEndElement();
                        xw.WriteEndDocument();
                    }
                    ms.Position = 0;
                    return File(ms, "text/xml", "WireTypesSizes.xml");

                case "Overloads":
                    List<Overload> overloads = new List<Overload>();
                    overloads = repository.Ovearloads.ToList();


                    using (XmlWriter xw = XmlWriter.Create(ms, xws))
                    {
                        xw.WriteStartDocument();
                        xw.WriteStartElement("Overloads");

                        foreach (Overload overload in overloads)
                        {
                            xw.WriteStartElement("Overload");

                            xw.WriteElementString("ID", overload.OverloadID.ToString());
                            xw.WriteElementString("AMPMin", overload.AMPMin.ToString());
                            xw.WriteElementString("AMPMax", overload.AMPMax.ToString());
                            xw.WriteElementString("OverTableNum", overload.OverTableNum.ToString());
                            xw.WriteElementString("MCEPart", overload.MCPart.ToString());
                            xw.WriteElementString("SiemensPart", overload.SiemensPart.ToString());
                            xw.WriteEndElement();
                        }

                        xw.WriteEndElement();
                        xw.WriteEndDocument();
                    }
                    ms.Position = 0;
                    return File(ms, "text/xml", "Overloads.xml");

                case "Stations":
                    List<Station> stations = new List<Station>();
                    stations = testRepo.Stations.ToList();


                    using (XmlWriter xw = XmlWriter.Create(ms, xws))
                    {
                        xw.WriteStartDocument();
                        xw.WriteStartElement("Stations");

                        foreach (Station station in stations)
                        {
                            xw.WriteStartElement("Station");

                            xw.WriteElementString("ID", station.StationID.ToString());
                            xw.WriteElementString("Label", station.Label);
                            xw.WriteElementString("JobTypeID", station.JobTypeID.ToString());
                            xw.WriteEndElement();
                        }

                        xw.WriteEndElement();
                        xw.WriteEndDocument();
                    }
                    ms.Position = 0;
                    return File(ms, "text/xml", "Stations.xml");
            }

            return File(ms, "text/xml", "Error.xml");
        }

        public async Task<IActionResult> ReferencesSearch(ReferencesSearchvViewModel ViewModel)
        {
            var jobSearch = jobrepo.Jobs.Include(j => j._HydroSpecific).Include(h => h._HoistWayData).Include(ex => ex._jobExtension).AsQueryable();
            var SlowReferSearch = repository.Slowdowns.AsQueryable();
            var WireReferSearch = repository.WireTypesSizes.AsQueryable();
            List<Starter> StarterRefer = repository.Starters.ToList();
            var OverloadReferSearch = repository.Ovearloads.AsQueryable();
            var LandingList = repository.LandingSystems.AsQueryable();
            var FireCodeList = repository.FireCodes.AsQueryable();
            var StatesList = repository.States.AsQueryable();
            var CitiesList = repository.Cities.AsQueryable();
            var POTotalList = jobrepo.POs.AsQueryable();
            ViewModel.status = null;

            ReferencesSearchvViewModel referSearchAux = new ReferencesSearchvViewModel
            {
                RefernceData = false,
            };

            if (ViewModel.JobID != 0)
            {
                var JobSearch = jobSearch.FirstOrDefault(m => m.JobID == ViewModel.JobID);
                if (JobTypeName(JobSearch.JobTypeID) != "M2000")
                {
                    TempData["alert"] = $"alert-danger";
                    TempData["message"] = $"Worksheet not available for the jobtype: {JobTypeName(JobSearch.JobTypeID)}";
                    return RedirectToAction("Index", "Home");
                }

                if (JobSearch != null)
                {
                    if (JobSearch.Status != "Incomplete")
                    {
                        #region JobData
                        var LandingOne = LandingList.FirstOrDefault(m => m.LandingSystemID == JobSearch._HoistWayData.LandingSystemID);
                        var FireCodeOne = FireCodeList.FirstOrDefault(m => m.FireCodeID == JobSearch.FireCodeID);
                        ViewModel.RMSAMPS = "18,000A";
                        ViewModel.SCCRVOLTS = "600V";
                        ViewModel.SPH = JobSearch._HydroSpecific.SPH;
                        ViewModel.FLA = JobSearch._HydroSpecific.FLA;
                        ViewModel.NumJobSearch = JobSearch.JobNum;
                        ViewModel.JobName = JobSearch.Name;
                        ViewModel.JobName2 = JobSearch.Name2;
                        ViewModel.Cust = JobSearch.Cust;
                        ViewModel.Contractor = JobSearch.Contractor;
                        ViewModel.JobTypeMain = JobSearch._jobExtension.JobTypeMain;
                        ViewModel.ValveBrand = JobSearch._HydroSpecific.ValveBrand;
                        ViewModel.HP = JobSearch._HydroSpecific.HP;
                        ViewModel.FireCodeName = FireCodeOne.Name;
                        ViewModel.LandingName = LandingOne.Name;
                        ViewModel.DownSpeed = JobSearch._HoistWayData.DownSpeed;
                        ViewModel.UpSpeed = JobSearch._HoistWayData.UpSpeed;
                        ViewModel.StarterType = JobSearch._HydroSpecific.Starter;
                        ViewModel.NumJobSearch = JobSearch.JobNum;
                        ViewModel.SHCisSelected = JobSearch._jobExtension.SHC;
                        ViewModel.TotalTravel = JobSearch._HoistWayData.TotalTravel;
                        ViewModel.anyRear = JobSearch._HoistWayData.AnyRear;
                        ViewModel.FrontFloor = JobSearch._HoistWayData.FrontFloorOpenings;
                        ViewModel.RearFloor = JobSearch._HoistWayData.RearFloorOpenings;
                        ViewModel.InputPhase = JobSearch._jobExtension.InputPhase;
                        ViewModel.POList = POTotalList.Where(m => m.JobID == JobSearch.JobID).ToList();
                        var volts = JobSearch._jobExtension.InputVoltage;
                        ViewModel.InputVoltage = volts;
                        if (volts >= 200 && volts <= 220 && (ViewModel.StarterType == "ATL" || ViewModel.StarterType == "YD" || ViewModel.StarterType == "Sprecher SS : 6/12" || ViewModel.StarterType == "Sprecher SS : 3/9" || ViewModel.StarterType == "Siemens SS : 6/12" || ViewModel.StarterType == "Siemens SS : 3/9")) ViewModel.Volts = "208";
                        if (volts > 220 && volts <= 240 && (ViewModel.StarterType == "ATL" || ViewModel.StarterType == "YD" || ViewModel.StarterType == "Sprecher SS : 6/12" || ViewModel.StarterType == "Sprecher SS : 3/9" || ViewModel.StarterType == "Siemens SS : 6/12" || ViewModel.StarterType == "Siemens SS : 3/9")) ViewModel.Volts = "240";
                        if (volts >= 380 && volts <= 480 && (ViewModel.StarterType == "ATL" || ViewModel.StarterType == "YD" || ViewModel.StarterType == "Sprecher SS : 6/12" || ViewModel.StarterType == "Sprecher SS : 3/9")) ViewModel.Volts = "480";
                        if (volts > 480 && volts <= 600 && (ViewModel.StarterType == "ATL" || ViewModel.StarterType == "YD" || ViewModel.StarterType == "Sprecher SS : 6/12" || ViewModel.StarterType == "Sprecher SS : 3/9" || ViewModel.StarterType == "Siemens SS : 6/12" || ViewModel.StarterType == "Siemens SS : 3/9")) ViewModel.Volts = "575";
                        if (volts > 430 && volts <= 480 && (ViewModel.StarterType == "Siemens SS : 6/12" || ViewModel.StarterType == "Siemens SS : 3/9")) ViewModel.Volts = "460";
                        if (volts >= 300 && volts <= 430 && (ViewModel.StarterType == "Siemens SS : 6/12" || ViewModel.StarterType == "Siemens SS : 3/9")) ViewModel.Volts = "380/415";

                        if (ViewModel.HP <= 50)
                                ViewModel.RMSAMPS = "5,000A";
                        else if (ViewModel.HP <= 200)
                                ViewModel.RMSAMPS = "10,000A";

                        if (volts <= 240)
                            ViewModel.SCCRVOLTS = "240V";
                        else if (volts <= 480)
                            ViewModel.SCCRVOLTS = "480V";


                        #endregion

                        #region SlowdownAndWire
                        //Slowdown Table

                        //For Down Speed
                        var SlowdoenRegDown = SlowReferSearch.Where(m => m.CarSpeedFPM >= ViewModel.DownSpeed).OrderBy(o => o.CarSpeedFPM).Skip(0).Take(1).ToList();
                        ViewModel.CarSpeedFPM = ViewModel.DownSpeed;
                        if (SlowdoenRegDown != null)
                        {
                            ViewModel.Distance = SlowdoenRegDown[0].Distance;
                            ViewModel.A = SlowdoenRegDown[0].A;
                            ViewModel.SlowLimit = SlowdoenRegDown[0].SlowLimit;
                            ViewModel.MiniumFloorHeight = SlowdoenRegDown[0].MiniumFloorHeight;
                        }
                        else
                        {
                            ViewModel.Distance = 0;
                            ViewModel.A = 0;
                            ViewModel.SlowLimit = 0;
                            ViewModel.MiniumFloorHeight = 0;
                        }

                        //For Up Speed
                        var SlowdoenRegUP = SlowReferSearch.Where(m => m.CarSpeedFPM >= ViewModel.UpSpeed).OrderBy(o => o.CarSpeedFPM).Skip(0).Take(1).ToList();
                        ViewModel.CarUpSpeedFPM = ViewModel.UpSpeed;
                        if (SlowdoenRegUP != null)
                        {
                            ViewModel.UPDistance = SlowdoenRegUP[0].Distance;
                            ViewModel.UPA = SlowdoenRegUP[0].A;
                            ViewModel.UPSlowLimit = SlowdoenRegUP[0].SlowLimit;
                            ViewModel.UPMiniumFloorHeight = SlowdoenRegUP[0].MiniumFloorHeight;
                        }
                        else
                        {
                            ViewModel.UPDistance = 0;
                            ViewModel.UPA = 0;
                            ViewModel.UPSlowLimit = 0;
                            ViewModel.UPMiniumFloorHeight = 0;
                        }

                        //WireTypeSizes
                        var WireTypeReg = WireReferSearch.Where(m => m.AMPRating >= ViewModel.FLA && m.Type == "125°C PTL").OrderBy(o => o.AMPRating).Skip(0).Take(1).ToList();
                        ViewModel.AMPRating = WireTypeReg[0].AMPRating;
                        if (WireTypeReg != null)
                        {
                            ViewModel.Size = WireTypeReg[0].Size;
                            ViewModel.Type = WireTypeReg[0].Type;
                        }
                        else
                        {
                            ViewModel.Size = "---Error---";
                            ViewModel.Type = "---Error---";
                        }
                        #endregion

                        #region CustomSoftWare
                        //Rellena las listas que se llenaran para la comparacion
                        List<CustomFeature> FilteredCustomsF = jobrepo.CustomFeatures.Where(m => m.JobID == JobSearch.JobID).ToList();
                        if (FilteredCustomsF.Count() > 0)
                        {
                            foreach (CustomFeature obj in FilteredCustomsF)
                            {
                                jobrepo.DeleteCustomFeature(obj.CustomFeatureID);
                            }
                        }
                        List<CustomSoftware> Customs = jobrepo.CustomSoftwares.Include(m => m._CustomFeatures).ToList();
                        List<TriggeringCustSoft> TriggersWithNameNull = jobrepo.TriggeringCustSofts.Where(m => m.Name == null).ToList();
                        List<TriggeringCustSoft> TriggersWithOutNameNull = jobrepo.TriggeringCustSofts.Where(m => m.Name != null).ToList();
                        Job FeaturesFromJob = jobrepo.Jobs.Include(m => m._jobExtension).Include(m => m._HydroSpecific).Include(m => m._HoistWayData).Include(m => m._GenericFeatures)
                            .First(m => m.JobNum == ViewModel.NumJobSearch);
                        //Checa si la lista de steps no esta vacia
                        if (Customs.Count > 0)
                        {
                            //inicia el contador del consecutivo
                            int consecutivo = 1;

                            //Checa cada step de la lista
                            foreach (CustomSoftware custom in Customs)
                            {
                                //Obtiene el primer trigger del step actual step
                                TriggeringCustSoft TriggerInStep = custom._TriggeringCustSofts.FirstOrDefault();

                                //si su name es nulo significa que es un step por default, debido a esto lo agrega a step for Job
                                if (TriggerInStep.Name == null)
                                {
                                    CustomFeature CustomForJob = new CustomFeature
                                    {
                                        CustomSoftwareID = custom.CustomSoftwareID,
                                        JobID = JobSearch.JobID
                                    };

                                    jobrepo.SaveCustomFeature(CustomForJob);
                                    consecutivo++;
                                }
                                /*si su name no es nulo significa que es un trigger optativo, debido a esto se comparara sus features con los del job
                                y si concuerdan se anadira a steps for job*/
                                else if (TriggerInStep.Name != null)
                                {
                                    //Crea una lista con todos los triggers del step actual
                                    var triggers = jobrepo.TriggeringCustSofts.Where(m => m.CustomSoftwareID == custom.CustomSoftwareID).ToList();
                                    //checa que la lista de triggers no este vacia
                                    if (triggers.Count > 0)
                                    {
                                        int count = triggers.Count;
                                        int countAux = 0;
                                        //Checa que cada feature de la lista concuerde con los features del testjob
                                        foreach (TriggeringCustSoft trigger in triggers)
                                        {
                                            switch (trigger.Name)
                                            {
                                                case "Contractor":
                                                    if (trigger.isSelected == true && trigger.itemToMatch == JobSearch.Contractor) countAux++;
                                                    else if ((trigger.isSelected == false) && trigger.itemToMatch != JobSearch.Contractor) countAux++;
                                                    break;
                                                case "Fire Code":
                                                    if (trigger.isSelected == true && trigger.itemToMatch == FireCodeList.First(m => m.FireCodeID == JobSearch.FireCodeID).Name) countAux++;
                                                    else if ((trigger.isSelected == false) && trigger.itemToMatch != FireCodeList.First(m => m.FireCodeID == JobSearch.FireCodeID).Name) countAux++;
                                                    break;
                                                case "State":
                                                    City Onecity = CitiesList.FirstOrDefault(m => m.CityID == JobSearch.CityID);
                                                    if (trigger.isSelected == true && trigger.itemToMatch == StatesList.First(m => m.StateID == Onecity.StateID).Name) countAux++;
                                                    else if ((trigger.isSelected == false) && trigger.itemToMatch != StatesList.First(m => m.StateID == Onecity.StateID).Name) countAux++;
                                                    break;
                                                case "City":
                                                    if (trigger.isSelected == true && trigger.itemToMatch == CitiesList.First(m => m.CityID == JobSearch.CityID).Name) countAux++;
                                                    else if ((trigger.isSelected == false) && trigger.itemToMatch != CitiesList.First(m => m.CityID == JobSearch.CityID).Name) countAux++;
                                                    break;
                                                case "VCI": if (trigger.isSelected == JobSearch._HydroSpecific.VCI) { countAux++; } break;
                                                case "Valve Brand":
                                                    if (trigger.isSelected == true && trigger.itemToMatch == JobSearch._HydroSpecific.ValveBrand) countAux++;
                                                    else if ((trigger.isSelected == false) && trigger.itemToMatch != JobSearch._HydroSpecific.ValveBrand) countAux++;
                                                    break;
                                                case "Switch Style":
                                                    if (trigger.isSelected == true && trigger.itemToMatch == JobSearch._GenericFeatures.SwitchStyle) countAux++;
                                                    else if ((trigger.isSelected == false) && trigger.itemToMatch != JobSearch._GenericFeatures.SwitchStyle) countAux++;
                                                    break;
                                                case "Landing System":
                                                    if (trigger.isSelected == true && trigger.itemToMatch == LandingList.First(m => m.LandingSystemID == JobSearch._HoistWayData.LandingSystemID).Name) countAux++;
                                                    else if ((trigger.isSelected == false) && trigger.itemToMatch != LandingList.First(m => m.LandingSystemID == JobSearch._HoistWayData.LandingSystemID).Name) countAux++;
                                                    break;
                                                default: break;
                                            }
                                        }
                                        //Si se vuelve valido agrega el step a la lista de steps for job
                                        if (count == countAux)
                                        {
                                            CustomFeature CustomForJob = new CustomFeature
                                            {
                                                CustomSoftwareID = custom.CustomSoftwareID,
                                                JobID = JobSearch.JobID
                                            };

                                            jobrepo.SaveCustomFeature(CustomForJob);
                                            consecutivo++;
                                        }
                                    }
                                }
                            }
                        }
                        List<CustomSoftware> FilteredCustomSoftware = new List<CustomSoftware>();
                        FilteredCustomsF = jobrepo.CustomFeatures.Where(m => m.JobID == JobSearch.JobID).ToList();
                        foreach (CustomFeature custom in FilteredCustomsF)
                        {
                            CustomSoftware one = Customs.FirstOrDefault(m => m.CustomSoftwareID == custom.CustomSoftwareID);
                            if (one != null) FilteredCustomSoftware.Add(one);
                        }



                        #endregion

                        #region StarterAndOverload

                        //Lista para strater and overload table

                        List<Starter> StarterList = StarterRefer.Where(m => m.Volts == ViewModel.Volts && m.StarterType == JobSearch._HydroSpecific.Starter
                        && m.FLA >= ViewModel.FLA && m.HP >= ViewModel.HP).OrderBy(o => o.FLA).Skip(0).Take(4).ToList();

                        if (StarterList.Count > 0)
                        {
                            if (ViewModel.SPH == 80 && ViewModel.InputPhase == 1 && ViewModel.StarterType == "ATL" && StarterList.Count > 2)
                            {
                                ViewModel.MCPart = StarterList[2].MCPart;
                                ViewModel.NewManufacturerPart = StarterList[2].NewManufacturerPart;
                                ViewModel.OverloadTable = StarterList[2].OverloadTable;
                            }
                            else if (ViewModel.SPH == 120 && ViewModel.InputPhase == 1 && ViewModel.StarterType == "ATL" && StarterList.Count > 3)
                            {
                                ViewModel.MCPart = StarterList[3].MCPart;
                                ViewModel.NewManufacturerPart = StarterList[3].NewManufacturerPart;
                                ViewModel.OverloadTable = StarterList[3].OverloadTable;
                            }
                            else if (ViewModel.SPH == 80)
                            {
                                ViewModel.MCPart = StarterList[0].MCPart;
                                ViewModel.NewManufacturerPart = StarterList[0].NewManufacturerPart;
                                ViewModel.OverloadTable = StarterList[0].OverloadTable;
                            }
                            else if (ViewModel.SPH == 120 && StarterList.Count > 1)
                            {
                                ViewModel.MCPart = StarterList[1].MCPart;
                                ViewModel.NewManufacturerPart = StarterList[1].NewManufacturerPart;
                                ViewModel.OverloadTable = StarterList[1].OverloadTable;
                            }
                            else
                            {
                                ViewModel.MCPart = "------Error------";
                                ViewModel.NewManufacturerPart = "------Error------";
                                ViewModel.OverloadTable = "N/A";

                                TempData["alert"] = $"alert-danger";
                                TempData["message"] = $"Starter Model out of range, please validate their SPH, HP, FLA and try again";
                            }
                        }
                        else
                        {
                            ViewModel.MCPart = "------Error------";
                            ViewModel.NewManufacturerPart = "------Error------";
                            ViewModel.OverloadTable = "N/A";

                            TempData["alert"] = $"alert-danger";
                            TempData["message"] = $"Starter Model out of range, please validate their SPH, HP, FLA and try again";
                        }
                        //Overload Table
                        if (ViewModel.OverloadTable != null && ViewModel.OverloadTable != "N/A")
                        {
                            float FLAModified = 0;
                            if (ViewModel.StarterType == "YD")
                            {
                                FLAModified = (float)(ViewModel.FLA * 0.58);
                            }
                            else
                            {
                                FLAModified = ViewModel.FLA;
                            }

                            var OverLoadReg = OverloadReferSearch.FirstOrDefault(m => m.OverTableNum == Int32.Parse(ViewModel.OverloadTable)
                            && m.AMPMin <= FLAModified && m.AMPMax >= FLAModified);

                            if (OverLoadReg != null)
                            {
                                ViewModel.MCPartOver = OverLoadReg.MCPart;
                                ViewModel.SiemensPart = OverLoadReg.SiemensPart;
                            }
                            else
                            {
                                ViewModel.MCPartOver = "N/A";
                                ViewModel.SiemensPart = "N/A";
                            }

                        }
                        else
                        {
                            ViewModel.MCPartOver = "N/A";
                            ViewModel.SiemensPart = "N/A";
                        }

                        #endregion

                        #region SHC Calculator
                        if (ViewModel.SHCisSelected)
                        {
                            ViewModel.calculatedFrontSHC = ((ViewModel.TotalTravel) + 70 + (10 * ViewModel.FrontFloor));
                            if (ViewModel.anyRear)
                            {
                                ViewModel.calculatedRearSHC = ((ViewModel.TotalTravel) + 70 + (10 * ViewModel.RearFloor));
                            }
                            else
                            {
                                ViewModel.calculatedRearSHC = 0;
                            }
                        }
                        else
                        {
                            ViewModel.calculatedFrontSHC = 0;
                            ViewModel.calculatedRearSHC = 0;
                        }

                        #endregion

                        #region ReferSearchVM
                        try
                        {
                            string auxName2 = "---------------------------------------";
                            if (ViewModel.JobName2 == null) ViewModel.JobName2 = auxName2;
                        }
                        catch (Exception e)
                        {
                            ViewModel.JobName2 = "-----------------------------------------";
                        }
                        ReferencesSearchvViewModel referSearch = new ReferencesSearchvViewModel
                        {
                            RefernceData = true,

                            //JobData
                            FLA = ViewModel.FLA,
                            JobName = ViewModel.JobName.ToUpper(),
                            JobName2 = ViewModel.JobName2.ToUpper(),
                            Cust = ViewModel.Cust.ToUpper(),
                            Contractor = ViewModel.Contractor.ToUpper(),
                            JobTypeMain = ViewModel.JobTypeMain.ToUpper(),
                            ValveBrand = ViewModel.ValveBrand.ToUpper(),
                            InputVoltage = ViewModel.InputVoltage,
                            HP = ViewModel.HP,
                            FireCodeName = ViewModel.FireCodeName.ToUpper(),
                            LandingName = ViewModel.LandingName.ToUpper(),
                            NumJobSearch = ViewModel.NumJobSearch,
                            RMSAMPS = ViewModel.RMSAMPS,
                            SCCRVOLTS =  ViewModel.SCCRVOLTS,
                            //****Slow Table

                            //For Down Speed
                            CarSpeedFPM = ViewModel.CarSpeedFPM,
                            Distance = ViewModel.Distance,
                            A = ViewModel.A,
                            SlowLimit = ViewModel.SlowLimit,
                            MiniumFloorHeight = ViewModel.MiniumFloorHeight,

                            //For up Speed
                            CarUpSpeedFPM = ViewModel.CarUpSpeedFPM,
                            UPDistance = ViewModel.UPDistance,
                            UPA = ViewModel.UPA,
                            UPSlowLimit = ViewModel.UPSlowLimit,
                            UPMiniumFloorHeight = ViewModel.UPMiniumFloorHeight,

                            //WireTypesSize
                            AMPRating = ViewModel.AMPRating,
                            Size = ViewModel.Size,
                            Type = ViewModel.Type,

                            //Starter
                            MCPart = ViewModel.MCPart,
                            NewManufacturerPart = ViewModel.NewManufacturerPart,

                            //Overload
                            MCPartOver = ViewModel.MCPartOver,
                            SiemensPart = ViewModel.SiemensPart,

                            //SHC Calculator
                            calculatedFrontSHC = ViewModel.calculatedFrontSHC,
                            calculatedRearSHC = ViewModel.calculatedRearSHC,
                            SHCisSelected = ViewModel.SHCisSelected,

                            //Custom
                            CustomSoftList = FilteredCustomSoftware,

                            //POList
                            POList = ViewModel.POList

                        };

                        return View(referSearch);

                        #endregion

                    }

                    TempData["alert"] = $"alert-danger";
                    TempData["message"] = $"That job isn't completed, please finish it and try again";

                    return RedirectToAction("ReferencesSearch", referSearchAux);
                }

                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"That job doesn't exist, please enter a new one";

                return RedirectToAction("ReferencesSearch", referSearchAux);

            }
            else
            {

                return View(referSearchAux);
            }

        }

        public async Task<IActionResult> ReferencesSearchElement(ReferencesSrchElementViewModel ViewModel)
        {
            var jobSearch = jobrepo.Jobs.Include(j => j._Elements).Include(h => h._Elements).Include(ex => ex._EmentTractions).AsQueryable();
            IQueryable<DoorOperator> DoorOperatorList = repository.DoorOperators.AsQueryable();
            IQueryable<PO> POTotalList = jobrepo.POs.AsQueryable();
            List<Starter> StarterRefer = repository.Starters.ToList();

            ReferencesSearchvViewModel referSearchAux = new ReferencesSearchvViewModel
            {
                RefernceData = false,
            };

            if (ViewModel.JobID != 0)
            {
                Job job = jobSearch.FirstOrDefault(m => m.JobID == ViewModel.JobID);

                if (job != null)
                {
                    if (job.Status != "Incomplete")
                    {
                        Element element = jobrepo.Elements.FirstOrDefault(m => m.JobID == job.JobID);
                        ElementHydro elementHydro = jobrepo.ElementHydros.FirstOrDefault(m => m.JobID == job.JobID);
                        ElementTraction elementTraction = jobrepo.ElementTractions.FirstOrDefault(m => m.JobID == job.JobID);
                        string jobTypeName = JobTypeName(job.JobTypeID);
                        ViewModel.NumJobSearch = job.JobNum;

                        if (jobTypeName == "ElmHydro")
                        {
                            #region Hydro
                            ViewModel.SPH = elementHydro.SPH;
                            ViewModel.StarterType = elementHydro.Starter;
                            ViewModel.FLA = elementHydro.FLA;
                            ViewModel.HP = elementHydro.HP;
                            ViewModel.POList = POTotalList.Where(m => m.JobID == job.JobID).ToList();
                            ViewModel.DoorOperator = DoorOperatorList.FirstOrDefault(m => m.DoorOperatorID == element.DoorOperatorID).Name;
                            ViewModel.HAPS = element.HAPS;
                            var volts = element.Voltage;
                            ViewModel.Voltage = volts;
                            if (volts >= 200 && volts <= 220 && (ViewModel.StarterType == "ATL" || ViewModel.StarterType == "YD" || ViewModel.StarterType == "Sprecher SS : 6/12" || ViewModel.StarterType == "Sprecher SS : 3/9" || ViewModel.StarterType == "Siemens SS : 6/12" || ViewModel.StarterType == "Siemens SS : 3/9")) ViewModel.Volts = "208";
                            if (volts > 220 && volts <= 240 && (ViewModel.StarterType == "ATL" || ViewModel.StarterType == "YD" || ViewModel.StarterType == "Sprecher SS : 6/12" || ViewModel.StarterType == "Sprecher SS : 3/9" || ViewModel.StarterType == "Siemens SS : 6/12" || ViewModel.StarterType == "Siemens SS : 3/9")) ViewModel.Volts = "240";
                            if (volts >= 380 && volts <= 480 && (ViewModel.StarterType == "ATL" || ViewModel.StarterType == "YD" || ViewModel.StarterType == "Sprecher SS : 6/12" || ViewModel.StarterType == "Sprecher SS : 3/9")) ViewModel.Volts = "480";
                            if (volts > 480 && volts <= 600 && (ViewModel.StarterType == "ATL" || ViewModel.StarterType == "YD" || ViewModel.StarterType == "Sprecher SS : 6/12" || ViewModel.StarterType == "Sprecher SS : 3/9" || ViewModel.StarterType == "Siemens SS : 6/12" || ViewModel.StarterType == "Siemens SS : 3/9")) ViewModel.Volts = "575";
                            if (volts > 380 && volts <= 480 && (ViewModel.StarterType == "Siemens SS : 6/12" || ViewModel.StarterType == "Siemens SS : 3/9")) ViewModel.Volts = "460";
                            if (volts >= 300 && volts <= 380 && (ViewModel.StarterType == "Siemens SS : 6/12" || ViewModel.StarterType == "Siemens SS : 3/9")) ViewModel.Volts = "380/415";

                            //Lista para strater and overload table

                            List<Starter> StarterList = StarterRefer.Where(m => m.Volts == ViewModel.Volts && m.StarterType == elementHydro.Starter
                            && m.FLA >= ViewModel.FLA && m.HP >= ViewModel.HP).OrderBy(o => o.FLA).Skip(0).Take(4).ToList();

                            if (StarterList.Count > 0)
                            {
                                if (ViewModel.SPH == 80 && ViewModel.Voltage == 1 && ViewModel.StarterType == "ATL" && StarterList.Count > 2)
                                {
                                    ViewModel.MCPart = StarterList[2].MCPart;
                                    ViewModel.NewManufacturerPart = StarterList[2].NewManufacturerPart;
                                }
                                else if (ViewModel.SPH == 120 && ViewModel.Voltage == 1 && ViewModel.StarterType == "ATL" && StarterList.Count > 3)
                                {
                                    ViewModel.MCPart = StarterList[3].MCPart;
                                    ViewModel.NewManufacturerPart = StarterList[3].NewManufacturerPart;
                                }
                                else if (ViewModel.SPH == 80)
                                {
                                    ViewModel.MCPart = StarterList[0].MCPart;
                                    ViewModel.NewManufacturerPart = StarterList[0].NewManufacturerPart;
                                }
                                else if (ViewModel.SPH == 120 && StarterList.Count > 1)
                                {
                                    ViewModel.MCPart = StarterList[1].MCPart;
                                    ViewModel.NewManufacturerPart = StarterList[1].NewManufacturerPart;
                                }
                                else
                                {
                                    ViewModel.MCPart = "------Error------";
                                    ViewModel.NewManufacturerPart = "------Error------";

                                    TempData["alert"] = $"alert-danger";
                                    TempData["message"] = $"Starter Model out of range, please validate their SPH, HP, FLA and try again";
                                }
                            }
                            else
                            {
                                ViewModel.MCPart = "------Error------";
                                ViewModel.NewManufacturerPart = "------Error------";

                                TempData["alert"] = $"alert-danger";
                                TempData["message"] = $"Starter Model out of range, please validate their SPH, HP, FLA and try again";
                            }

                            #endregion

                            #region ReferSearchVM
                            ReferencesSrchElementViewModel referSearch = new ReferencesSrchElementViewModel
                            {
                                RefernceData = true,

                                //General
                                NumJobSearch = ViewModel.NumJobSearch,
                                Voltage = ViewModel.Voltage,
                                DoorOperator = ViewModel.DoorOperator.ToUpper(),
                                HAPS = ViewModel.HAPS,

                                //Starter
                                MCPart = ViewModel.MCPart,
                                NewManufacturerPart = ViewModel.NewManufacturerPart,

                                //POList
                                POList = ViewModel.POList

                            };

                            return View("ReferencesSrchHydro", referSearch);

                            #endregion
                        }
                        else
                        {
                            ViewModel.HP = elementTraction.HP;
                            ViewModel.Speed = element.Speed;
                            ViewModel.Capacity = element.Capacity;

                            ViewModel.IdealVIn = (int)(((elementTraction.PickVoltage) * (1.1)) / (0.85));
                            ViewModel.MinVIn = (int)((elementTraction.PickVoltage) / (0.85));
                            ViewModel.MaxVIn = (int)(((elementTraction.PickVoltage) * (1.25)) / (0.85));

                            if (elementTraction.Resistance > 101) ViewModel.BrakeResistor = "1500R 100W";
                            else ViewModel.BrakeResistor = "500R 225W";

                            #region ReferSearchVM
                            ReferencesSrchElementViewModel referSearch = new ReferencesSrchElementViewModel
                            {
                                RefernceData = true,

                                NumJobSearch = ViewModel.NumJobSearch,

                                //General
                                Speed = ViewModel.Speed,
                                HP = ViewModel.HP,
                                Capacity = ViewModel.Capacity,

                                //Brake
                                IdealVIn = ViewModel.IdealVIn,
                                MinVIn = ViewModel.MinVIn,
                                MaxVIn = ViewModel.MaxVIn,
                                BrakeResistor = ViewModel.BrakeResistor,

                            };

                            return View("ReferencesSrchTraction", referSearch);

                            #endregion
                        }

                    }

                    TempData["alert"] = $"alert-danger";
                    TempData["message"] = $"That job isn't completed, please finish it and try again";

                    return RedirectToAction("ReferencesSearch", referSearchAux);
                }

                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"That job doesn't exist, please enter a new one";

                return RedirectToAction("ReferencesSearch", referSearchAux);

            }
            else
            {

                return View(referSearchAux);
            }

        }

        public RedirectToActionResult SearchAux(int JobID)
        {

            Job job = jobrepo.Jobs.FirstOrDefault(m => m.JobID == JobID);

            if (JobTypeName(job.JobTypeID) == "M2000")
            {
                ReferencesSearchvViewModel viewModel = new ReferencesSearchvViewModel
                {
                    JobID = JobID,
                    RefernceData = false
                };
                return RedirectToAction("ReferencesSearch", viewModel);
            }
            else
            {
                ReferencesSrchElementViewModel viewModel = new ReferencesSrchElementViewModel
                {
                    JobID = JobID,
                    RefernceData = false
                };
                return RedirectToAction("ReferencesSearchElement", viewModel);
            }

        }

        public void ImportXML(IServiceProvider services, string resp)
        {
            ApplicationDbContext context = services.GetRequiredService<ApplicationDbContext>();
            switch (resp)
            {
                case "Wire":

                    HtmlDocument doc = new HtmlDocument();

                    doc.Load(appDataFolder + @"WireTypesSizes.xml");

                    var XMLobs = doc.DocumentNode.SelectNodes("//wiretypessize");

                    foreach (var XMLob in XMLobs)
                    {
                        var ID = XMLob.SelectSingleNode(".//id").InnerText;
                        var type = XMLob.SelectSingleNode(".//type").InnerText;
                        var size = XMLob.SelectSingleNode(".//size").InnerText;
                        var aMPRataing = XMLob.SelectSingleNode(".//amprating").InnerText;

                        context.WireTypesSizes.Add(new WireTypesSize { WireTypesSizeID = Int32.Parse(ID), Type = type, Size = size, AMPRating = Int32.Parse(aMPRataing) });
                        context.Database.OpenConnection();
                        try
                        {
                            context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.WireTypesSizes ON");
                            context.SaveChanges();
                            context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.WireTypesSizes OFF");
                        }
                        finally
                        {
                            context.Database.CloseConnection();
                        }
                    }
                    break;

                case "Slowdown":
                    HtmlDocument doc3 = new HtmlDocument();
                    doc3.Load(appDataFolder + @"Slowdowns.xml");
                    var XMLobs3 = doc3.DocumentNode.SelectNodes("//slowdown");

                    foreach (var XMLob in XMLobs3)
                    {
                        var ID = XMLob.SelectSingleNode(".//id").InnerText;
                        var carspeed = XMLob.SelectSingleNode(".//carspeedfpm").InnerText;
                        var distance = XMLob.SelectSingleNode(".//distance").InnerText;
                        var a = XMLob.SelectSingleNode(".//a").InnerText;
                        var slowlimit = XMLob.SelectSingleNode(".//slowlimit").InnerText;
                        var miniumFloor = XMLob.SelectSingleNode(".//miniumfloorheight").InnerText;

                        context.Slowdowns.Add(new Slowdown
                        {
                            SlowdownID = Int32.Parse(ID),
                            CarSpeedFPM = Int32.Parse(carspeed),
                            Distance = Int32.Parse(distance),
                            A = Int32.Parse(a),
                            SlowLimit = Int32.Parse(slowlimit),
                            MiniumFloorHeight = Int32.Parse(miniumFloor)
                        });

                        context.Database.OpenConnection();
                        try
                        {
                            context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Slowdowns ON");
                            context.SaveChanges();
                            context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Slowdowns OFF");
                        }
                        finally
                        {
                            context.Database.CloseConnection();
                        }
                    }
                    break;

                case "Starter":
                    HtmlDocument doc4 = new HtmlDocument();
                    doc4.Load(appDataFolder + @"Starters.xml");

                    var XMLobs4 = doc4.DocumentNode.SelectNodes("//starter");

                    foreach (var XMLob in XMLobs4)
                    {
                        var ID = XMLob.SelectSingleNode(".//id").InnerText;
                        var starter = XMLob.SelectSingleNode(".//startertype").InnerText;
                        var fla = XMLob.SelectSingleNode(".//fla").InnerText;
                        var volts = XMLob.SelectSingleNode(".//volts").InnerText;
                        var hp = XMLob.SelectSingleNode(".//hp").InnerText;
                        var mcepart = XMLob.SelectSingleNode(".//mcepart").InnerText;
                        var manufacturer = XMLob.SelectSingleNode(".//newmanufacturerpart").InnerText;
                        var overloadT = XMLob.SelectSingleNode(".//overloadtable").InnerText;

                        context.Starters.Add(new Starter
                        {
                            StarterID = Int32.Parse(ID),
                            StarterType = starter,
                            FLA = float.Parse(fla),
                            Volts = volts,
                            HP = float.Parse(hp),
                            MCPart = mcepart,
                            NewManufacturerPart = manufacturer,
                            OverloadTable = overloadT
                        });

                        context.Database.OpenConnection();
                        try
                        {
                            context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Starters ON");
                            context.SaveChanges();
                            context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Starters OFF");
                        }
                        finally
                        {
                            context.Database.CloseConnection();
                        }
                    }
                    break;

                case "Overload":
                    HtmlDocument doc5 = new HtmlDocument();
                    doc5.Load(appDataFolder + @"Overloads.xml");

                    var XMLobs5 = doc5.DocumentNode.SelectNodes("//overload");

                    foreach (var XMLob in XMLobs5)
                    {
                        var ID = XMLob.SelectSingleNode(".//id").InnerText;
                        var ampmin = XMLob.SelectSingleNode(".//ampmin").InnerText;
                        var ampmax = XMLob.SelectSingleNode(".//ampmax").InnerText;
                        var overT = XMLob.SelectSingleNode(".//overtablenum").InnerText;
                        var mcepart = XMLob.SelectSingleNode(".//mcepart").InnerText;
                        var siemenspart = XMLob.SelectSingleNode(".//siemenspart").InnerText;

                        context.Overloads.Add(new Overload
                        {
                            OverloadID = Int32.Parse(ID),
                            AMPMin = float.Parse(ampmin),
                            AMPMax = float.Parse(ampmax),
                            OverTableNum = Int32.Parse(overT),
                            MCPart = mcepart,
                            SiemensPart = siemenspart
                        });

                        context.Database.OpenConnection();
                        try
                        {
                            context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Overloads ON");
                            context.SaveChanges();
                            context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Overloads OFF");
                        }
                        finally
                        {
                            context.Database.CloseConnection();
                        }
                    }
                    break;

                case "Country":
                    HtmlDocument doc6 = new HtmlDocument();
                    doc6.Load(appDataFolder + @"Countries.xml");


                    var XMLobs6 = doc6.DocumentNode.SelectNodes("//country");

                    foreach (var XMLob in XMLobs6)
                    {
                        var ID = XMLob.SelectSingleNode(".//id").InnerText;
                        var name = XMLob.SelectSingleNode(".//name").InnerText;

                        context.Countries.Add(new Country
                        {
                            CountryID = Int32.Parse(ID),
                            Name = name
                        });

                        context.Database.OpenConnection();
                        try
                        {
                            context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Countries ON");
                            context.SaveChanges();
                            context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Countries OFF");
                        }
                        finally
                        {
                            context.Database.CloseConnection();
                        }
                    }
                    break;

                case "State":
                    HtmlDocument doc2 = new HtmlDocument();
                    doc2.Load(appDataFolder + @"States.xml");
                    var XMLobs2 = doc2.DocumentNode.SelectNodes("//state");

                    if (context.Countries.Any())
                    {
                        foreach (var XMLob in XMLobs2)
                        {
                            var ID = XMLob.SelectSingleNode(".//id").InnerText;
                            var name = XMLob.SelectSingleNode(".//name").InnerText;
                            var countryId = XMLob.SelectSingleNode(".//countryid").InnerText;

                            context.States.Add(new State { StateID = Int32.Parse(ID), Name = name, CountryID = Int32.Parse(countryId) });
                            context.Database.OpenConnection();
                            try
                            {
                                context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.States ON");
                                context.SaveChanges();
                                context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.States OFF");
                            }
                            finally
                            {
                                context.Database.CloseConnection();
                            }
                        }
                    }
                    break;

                case "City":
                    HtmlDocument doc7 = new HtmlDocument();
                    doc7.Load(appDataFolder + @"Cities.xml");

                    var XMLobs7 = doc7.DocumentNode.SelectNodes("//city");

                    if (context.States.Any() && context.FireCodes.Any())
                    {
                        foreach (var XMLob in XMLobs7)
                        {
                            var ID = XMLob.SelectSingleNode(".//id").InnerText;
                            var name = XMLob.SelectSingleNode(".//name").InnerText;
                            var stateID = XMLob.SelectSingleNode(".//stateid").InnerText;
                            var firecodeID = XMLob.SelectSingleNode(".//firecodeid").InnerText;

                            context.Cities.Add(new City
                            {
                                CityID = Int32.Parse(ID),
                                Name = name,
                                StateID = Int32.Parse(stateID),
                                FirecodeID = Int32.Parse(firecodeID),
                            });

                            context.Database.OpenConnection();
                            try
                            {
                                context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Cities ON");
                                context.SaveChanges();
                                context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Cities OFF");
                            }
                            finally
                            {
                                context.Database.CloseConnection();
                            }
                        }
                    }

                    break;

                case "LandingSys":
                    HtmlDocument doc8 = new HtmlDocument();
                    doc8.Load(appDataFolder + @"LandingSystems.xml");

                    var XMLobs8 = doc8.DocumentNode.SelectNodes("//landingsystem");

                    foreach (var XMLob in XMLobs8)
                    {
                        var ID = XMLob.SelectSingleNode(".//id").InnerText;
                        var usein = XMLob.SelectSingleNode(".//usedin").InnerText;
                        var name = XMLob.SelectSingleNode(".//name").InnerText;

                        context.LandingSystems.Add(new LandingSystem
                        {
                            LandingSystemID = Int32.Parse(ID),
                            UsedIn = usein,
                            Name = name
                        });

                        context.Database.OpenConnection();
                        try
                        {
                            context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.LandingSystems ON");
                            context.SaveChanges();
                            context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.LandingSystems OFF");
                        }
                        finally
                        {
                            context.Database.CloseConnection();
                        }
                    }
                    break;

                case "FireCode":
                    HtmlDocument doc9 = new HtmlDocument();
                    doc9.Load(appDataFolder + @"FireCodes.xml");

                    var XMLobs9 = doc9.DocumentNode.SelectNodes("//firecode");

                    foreach (var XMLob in XMLobs9)
                    {
                        var ID = XMLob.SelectSingleNode(".//id").InnerText;
                        var name = XMLob.SelectSingleNode(".//name").InnerText;

                        context.FireCodes.Add(new FireCode
                        {
                            FireCodeID = Int32.Parse(ID),
                            Name = name
                        });

                        context.Database.OpenConnection();
                        try
                        {
                            context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.FireCodes ON");
                            context.SaveChanges();
                            context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.FireCodes OFF");
                        }
                        finally
                        {
                            context.Database.CloseConnection();
                        }
                    }
                    break;

                case "DoorOperator":
                    HtmlDocument doc10 = new HtmlDocument();
                    doc10.Load(appDataFolder + @"DoorOperators.xml");

                    var XMLobs10 = doc10.DocumentNode.SelectNodes("//dooroperator");
                    foreach (var XMLob in XMLobs10)
                    {
                        var ID = XMLob.SelectSingleNode(".//id").InnerText;
                        var style = XMLob.SelectSingleNode(".//style").InnerText;
                        var branch = XMLob.SelectSingleNode(".//branch").InnerText;
                        var name = XMLob.SelectSingleNode(".//name").InnerText;

                        context.DoorOperators.Add(new DoorOperator
                        {
                            DoorOperatorID = Int32.Parse(ID),
                            Style = style,
                            Brand = branch,
                            Name = name
                        });

                        context.Database.OpenConnection();
                        try
                        {
                            context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.DoorOperators ON");
                            context.SaveChanges();
                            context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.DoorOperators OFF");
                        }
                        finally
                        {
                            context.Database.CloseConnection();
                        }
                    }
                    break;

                case "JobType":
                    HtmlDocument doc11 = new HtmlDocument();
                    doc11.Load(appDataFolder + @"JobTypes.xml");

                    var XMLobs11 = doc11.DocumentNode.SelectNodes("//jobtype");

                    foreach (var XMLob in XMLobs11)
                    {
                        var ID = XMLob.SelectSingleNode(".//id").InnerText;
                        var name = XMLob.SelectSingleNode(".//name").InnerText;

                        context.JobTypes.Add(new JobType
                        {
                            JobTypeID = Int32.Parse(ID),
                            Name = name
                        });

                        context.Database.OpenConnection();
                        try
                        {
                            context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.JobTypes ON");
                            context.SaveChanges();
                            context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.JobTypes OFF");
                        }
                        finally
                        {
                            context.Database.CloseConnection();
                        }
                    }
                    break;

                case "Stations":
                    HtmlDocument doc12 = new HtmlDocument();
                    doc12.Load(appDataFolder + @"Stations.xml");

                    var XMLobs12 = doc12.DocumentNode.SelectNodes("//station");

                    if (context.JobTypes.Any())
                    {
                        foreach (var XMLob in XMLobs12)
                        {

                            var ID = XMLob.SelectSingleNode(".//id").InnerText;
                            var label = XMLob.SelectSingleNode(".//label").InnerText;
                            var jobtypeid = XMLob.SelectSingleNode(".//jobtypeid").InnerText;

                            if (Int32.Parse(ID) != 0)
                            {
                                context.Stations.Add(new Station
                                {
                                    StationID = Int32.Parse(ID),
                                    JobTypeID = Int32.Parse(jobtypeid),
                                    Label = label
                                });

                                context.Database.OpenConnection();
                                try
                                {
                                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Stations ON");
                                    context.SaveChanges();
                                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Stations OFF");
                                }
                                finally
                                {
                                    context.Database.CloseConnection();
                                }
                            }

                        }
                    }

                    break;
            }


        }


        public JsonResult GetLandingSystem(int JobTypeID)
        {
            string JobTypeName = repository.JobTypes.FirstOrDefault(m => m.JobTypeID == JobTypeID).Name;

            List<LandingSystem> LandingList = (from landing in repository.LandingSystems where landing.UsedIn == JobTypeName select landing).ToList();
            return Json(new SelectList(LandingList, "Name", "LandingSystemID"));
        }

        public JsonResult GetStation(int JobTypeID)
        {
            string JobTypeName = repository.JobTypes.FirstOrDefault(m => m.JobTypeID == JobTypeID).Name;

            if (JobTypeName == "M2000")
            {
                IList<SelectListItem> M2000 = new List<SelectListItem>
                {
                new SelectListItem{Text = "S1", Value = "S1"},
                new SelectListItem{Text = "S2", Value = "S2"},
                new SelectListItem{Text = "S3", Value = "S3"},
                new SelectListItem{Text = "S4", Value = "S4"},
                new SelectListItem{Text = "S5", Value = "S5"},
                new SelectListItem{Text = "S6", Value = "S6"}

                };

                return Json(new SelectList(M2000, "Text", "Value"));
            }
            else if (JobTypeName == "M4000")
            {
                IList<SelectListItem> M4000 = new List<SelectListItem>
                {
                new SelectListItem{Text = "S7", Value = "S7"},
                new SelectListItem{Text = "S8", Value = "S8"},
                new SelectListItem{Text = "S9", Value = "S9"},
                new SelectListItem{Text = "S10", Value = "S10"},
                new SelectListItem{Text = "S11", Value = "S11"},
                new SelectListItem{Text = "S12", Value = "S12"}

                };
                return Json(new SelectList(M4000, "Text", "Value"));
            }
            else
            {
                IList<SelectListItem> Element = new List<SelectListItem>
                {
                new SelectListItem{Text = "ELEM1", Value = "ELEM1"},
                new SelectListItem{Text = "ELEM2", Value = "ELEM2"},

                };
                return Json(new SelectList(Element, "Text", "Value"));
            }
        }

        public string JobTypeName(int ID)
        {
            return repository.JobTypes.FirstOrDefault(m => m.JobTypeID == ID).Name;
        }

        public String getJobNumb(string firstDigits, int lastDigits)
        {
            string JobNumb = firstDigits + lastDigits.ToString();

            return JobNumb;
        }
    }
}
