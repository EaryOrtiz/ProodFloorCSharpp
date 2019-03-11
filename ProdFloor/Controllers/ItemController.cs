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
            var jobSearch = jobrepo.Jobs.Include(j => j._HydroSpecific).Include( h => h._HoistWayData).AsQueryable();
            var SlowReferSearch = repository.Slowdowns.AsQueryable();
            var WireReferSearch = repository.WireTypesSizes.AsQueryable();

            if (ViewModel.NumJobSearch != 0)
            {
                //Slowdown Table
                List<int> CarSpeedList = jobSearch.Where(m => m.JobNum == ViewModel.NumJobSearch).Select(s => s._HoistWayData.DownSpeed).ToList(); ViewModel.CarSpeedFPM = CarSpeedList[0];
                List<int> Distancelist = SlowReferSearch.Where(m => m.CarSpeedFPM == ViewModel.CarSpeedFPM).Select(s => s.Distance).ToList(); ViewModel.Distance = Distancelist[0];
                List<int> landingPageList = SlowReferSearch.Where(m => m.CarSpeedFPM == ViewModel.CarSpeedFPM).Select(s => s.A).ToList(); ViewModel.A = landingPageList[0];
                List<int> SlowLimit = SlowReferSearch.Where(m => m.CarSpeedFPM == ViewModel.CarSpeedFPM).Select(s => s.SlowLimit).ToList(); ViewModel.SlowLimit = SlowLimit[0];
                List<int> MiniuimFloorList = SlowReferSearch.Where(m => m.CarSpeedFPM == ViewModel.CarSpeedFPM).Select(s => s.MiniumFloorHeight).ToList(); ViewModel.MiniumFloorHeight = MiniuimFloorList[0];

                //WireTypeSizes
                List<int> FLAlist = jobSearch.Where(m => m.JobNum == ViewModel.NumJobSearch).Select(s => s._HydroSpecific.FLA).ToList(); ViewModel.AMPRating = FLAlist[0];
                List<string>  TypeList = WireReferSearch.Where(m => m.AMPRating == ViewModel.AMPRating).Select(s => s.Type).ToList(); ViewModel.Type = TypeList[0];
                List<string> SizeList = WireReferSearch.Where(m => m.AMPRating == ViewModel.AMPRating).Select(s => s.Size).ToList(); ViewModel.Size = SizeList[0];
                ReferencesSearchvViewModel referSearch = new ReferencesSearchvViewModel
                {
                    RefernceData = true,
                    //Slow Table
                    CarSpeedFPM = ViewModel.CarSpeedFPM,
                    Distance = ViewModel.Distance,
                    A = ViewModel.A,
                    SlowLimit = ViewModel.SlowLimit,
                    MiniumFloorHeight = ViewModel.MiniumFloorHeight,

                    //WireTypesSize
                    AMPRating = ViewModel.AMPRating,
                    Size = ViewModel.Size,
                    Type = ViewModel.Type
                };

                return View(referSearch);

            }
            else {
                ReferencesSearchvViewModel referSearch2 = new ReferencesSearchvViewModel
                {
                    RefernceData = false,
                };
                return View(referSearch2);
            }
        }
    }
}
