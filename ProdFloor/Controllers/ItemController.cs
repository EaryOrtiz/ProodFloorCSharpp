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

namespace ProdFloor.Controllers
{
    public class ItemController : Controller
    {
        private IItemRepository repository;
        private IJobRepository jobrepo;

        public int PageSize = 4;

        public ItemController(IItemRepository repo, IJobRepository jobRepository)
        {
            repository = repo;
            jobrepo = jobRepository;
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
                            xw.WriteElementString("StateID",city.StateID.ToString());
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
                            xw.WriteElementString("Brand", starter.Brand);
                            xw.WriteElementString("FLA", starter.FLA.ToString());
                            xw.WriteElementString("Type", starter.Type);
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
            }

            return File(ms, "text/xml", "Error.xml");
        }

        public async Task<IActionResult> ReferencesSearch(ReferencesSearchvViewModel ViewModel)
        {
            var jobSearch = jobrepo.Jobs.Include(j => j._HydroSpecific).Include( h => h._HoistWayData).Include(ex => ex._jobExtension).AsQueryable();
            var SlowReferSearch = repository.Slowdowns.AsQueryable();
            var WireReferSearch = repository.WireTypesSizes.AsQueryable();
            var StarterReferSearch = repository.Starters.AsQueryable();
            var OverloadReferSearch = repository.Ovearloads.AsQueryable();
            var LandingList= repository.LandingSystems.AsQueryable();
            var FireCodeList = repository.FireCodes.AsQueryable();

            ReferencesSearchvViewModel referSearchAux = new ReferencesSearchvViewModel
            {
                RefernceData = false,
            };

            if (ViewModel.NumJobSearch != 0)
            {
                
                var JobSearch = jobSearch.FirstOrDefault(m => m.JobNum == ViewModel.NumJobSearch);

                if (JobSearch != null)
                {
                    #region JobData
                    var LandingOne = LandingList.FirstOrDefault(m => m.LandingSystemID == JobSearch._HoistWayData.LandingSystemID);
                    var FireCodeOne = FireCodeList.FirstOrDefault(m => m.FireCodeID == JobSearch.FireCodeID);
                    ViewModel.SPH = JobSearch._HydroSpecific.SPH;
                    ViewModel.FLA = JobSearch._HydroSpecific.FLA;
                    ViewModel.JobName = JobSearch.Name;
                    ViewModel.Contractor = JobSearch.Contractor;
                    ViewModel.JobTypeMain = JobSearch._jobExtension.JobTypeMain;
                    ViewModel.ValveBrand = JobSearch._HydroSpecific.ValveBrand;
                    ViewModel.PO = JobSearch.PO;
                    ViewModel.InputVoltage = JobSearch._jobExtension.InputVoltage;
                    ViewModel.HP = JobSearch._HydroSpecific.HP;
                    ViewModel.FireCodeName = FireCodeOne.Name;
                    ViewModel.LandingName = LandingOne.Name;
                    ViewModel.DownSpeed = JobSearch._HoistWayData.DownSpeed;
                    #endregion

                    #region SlowdownAndWire
                    //Slowdown Table
                    var SlowdoenReg = SlowReferSearch.Where(m => m.CarSpeedFPM >= ViewModel.DownSpeed).OrderBy(o => o.CarSpeedFPM).Skip(0).Take(1).ToList();
                    ViewModel.CarSpeedFPM = SlowdoenReg[0].CarSpeedFPM;
                    ViewModel.Distance = SlowdoenReg[0].Distance;
                    ViewModel.A = SlowdoenReg[0].A;
                    ViewModel.SlowLimit = SlowdoenReg[0].SlowLimit;
                    ViewModel.MiniumFloorHeight = SlowdoenReg[0].MiniumFloorHeight;

                    //WireTypeSizes
                    var WireTypeReg = WireReferSearch.Where(m => m.AMPRating >= ViewModel.FLA).OrderBy(o => o.AMPRating).Skip(0).Take(1).ToList();
                    ViewModel.AMPRating = WireTypeReg[0].AMPRating;
                    ViewModel.Size = WireTypeReg[0].Size;
                    ViewModel.Type = WireTypeReg[0].Type;
                    #endregion

                    #region StarterAndOverload

                    //Lista para strater and overload table
                    List<Starter> StarterList = StarterReferSearch.Where(m => m.Volts.Contains(ViewModel.InputVoltage.ToString())
                    && m.Type == JobSearch._HydroSpecific.Starter && m.FLA >= ViewModel.FLA && m.HP >= ViewModel.HP).OrderBy(o => o.FLA).Skip(0).Take(2).ToList();


                    if (ViewModel.SPH == 80)
                    {
                        ViewModel.MCPart = StarterList[0].MCPart;
                        ViewModel.NewManufacturerPart = StarterList[0].NewManufacturerPart;
                        ViewModel.OverloadTable = StarterList[0].OverloadTable;
                    }
                    else if(ViewModel.SPH == 120  && StarterList.Count != 0)
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

                    //Overload Table
                    if (ViewModel.OverloadTable != null && ViewModel.OverloadTable != "N/A")
                    {
                        var OverLoadReg = OverloadReferSearch.FirstOrDefault(m => m.OverTableNum == Int32.Parse(ViewModel.OverloadTable)
                        && m.AMPMin >= ViewModel.FLA && m.AMPMax <= ViewModel.FLA);
                        ViewModel.MCPartOver = OverLoadReg.MCPart;
                        ViewModel.SiemensPart = OverLoadReg.SiemensPart;
                    }
                    else
                    {
                        ViewModel.MCPartOver = "N/A";
                        ViewModel.SiemensPart = "N/A";
                    }

                    #endregion

                    #region ReferSearchVM

                    ReferencesSearchvViewModel referSearch = new ReferencesSearchvViewModel
                    {
                        RefernceData = true,

                        //JobData
                        FLA = ViewModel.FLA,
                        JobName = ViewModel.JobName,
                        Contractor = ViewModel.Contractor,
                        JobTypeMain = ViewModel.JobTypeMain,
                        ValveBrand = ViewModel.ValveBrand,
                        PO = JobSearch.PO,
                        InputVoltage = ViewModel.InputVoltage,
                        HP = ViewModel.HP,
                        FireCodeName = ViewModel.FireCodeName,
                        LandingName = ViewModel.LandingName,

                        //Slow Table
                        CarSpeedFPM = ViewModel.CarSpeedFPM,
                        Distance = ViewModel.Distance,
                        A = ViewModel.A,
                        SlowLimit = ViewModel.SlowLimit,
                        MiniumFloorHeight = ViewModel.MiniumFloorHeight,

                        //WireTypesSize
                        AMPRating = ViewModel.AMPRating,
                        Size = ViewModel.Size,
                        Type = ViewModel.Type,

                        //Starter
                        MCPart = ViewModel.MCPart,
                        NewManufacturerPart = ViewModel.NewManufacturerPart,

                        //Overload
                        MCPartOver = ViewModel.MCPartOver,
                        SiemensPart = ViewModel.SiemensPart
                    };

                    return View(referSearch);

                    #endregion
                }

                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"That job doesn't exist, please try again";

                return View(referSearchAux);

            }
            else {
                
                return View(referSearchAux);
            }

        }
    }
}
