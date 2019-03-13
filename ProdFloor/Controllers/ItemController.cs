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
            var jobSearch = jobrepo.Jobs.Include(j => j._HydroSpecific).Include(h => h._HoistWayData).Include(ex => ex._jobExtension).AsQueryable();
            var SlowReferSearch = repository.Slowdowns.AsQueryable();
            var WireReferSearch = repository.WireTypesSizes.AsQueryable();
            var StarterReferSearch = repository.Starters.AsQueryable();
            var OverloadReferSearch = repository.Ovearloads.AsQueryable();
            var LandingList = repository.LandingSystems.AsQueryable();
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
                    else if (ViewModel.SPH == 120 && StarterList.Count != 0)
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
            else
            {

                return View(referSearchAux);
            }

        }

        public static void ImportXML(IServiceProvider services, string resp)
        {
            ApplicationDbContext context = services.GetRequiredService<ApplicationDbContext>();

            switch (resp)
            {
                case "Wire":
                    HtmlDocument doc = new HtmlDocument();
                    doc.Load(@"C:\Users\eary.ortiz\Documents\GitHub\ProodFloorCSharpp\ProdFloor\wwwroot\AppData\WireTypesSizes.xml");

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
                    doc3.Load(@"C:\Users\eary.ortiz\Documents\GitHub\ProodFloorCSharpp\ProdFloor\wwwroot\AppData\Slowdowns.xml");

                    var XMLobs3 = doc3.DocumentNode.SelectNodes("//slowdown");

                    foreach (var XMLob in XMLobs3)
                    {
                        var ID = XMLob.SelectSingleNode(".//id").InnerText;
                        var carspeed = XMLob.SelectSingleNode(".//carspeedfpm").InnerText;
                        var distance = XMLob.SelectSingleNode(".//distance").InnerText;
                        var a = XMLob.SelectSingleNode(".//a").InnerText;
                        var slowlimit = XMLob.SelectSingleNode(".//slowlimit").InnerText;
                        var miniumFloor = XMLob.SelectSingleNode(".//miniumfloorheight").InnerText;

                            context.Slowdowns.Add(new Slowdown { SlowdownID = Int32.Parse(ID), CarSpeedFPM = Int32.Parse(carspeed), Distance = Int32.Parse(distance),
                                A = Int32.Parse(a), SlowLimit = Int32.Parse(slowlimit), MiniumFloorHeight = Int32.Parse(miniumFloor)});

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
                    doc4.Load(@"C:\Users\eary.ortiz\Documents\GitHub\ProodFloorCSharpp\ProdFloor\wwwroot\AppData\Starters.xml");

                    var XMLobs4 = doc4.DocumentNode.SelectNodes("//starter");

                    foreach (var XMLob in XMLobs4)
                    {
                        var ID = XMLob.SelectSingleNode(".//id").InnerText;
                        var brand = XMLob.SelectSingleNode(".//brand").InnerText;
                        var fla = XMLob.SelectSingleNode(".//fla").InnerText;
                        var type = XMLob.SelectSingleNode(".//type").InnerText;
                        var volts = XMLob.SelectSingleNode(".//volts").InnerText;
                        var hp = XMLob.SelectSingleNode(".//hp").InnerText;
                        var mcepart = XMLob.SelectSingleNode(".//mcepart").InnerText;
                        var manufacturer = XMLob.SelectSingleNode(".//newmanufacturerpart").InnerText;
                        var overloadT = XMLob.SelectSingleNode(".//overloadtable").InnerText;

                        context.Starters.Add(new Starter
                        {
                            StarterID = Int32.Parse(ID),
                            Brand = brand,
                            FLA = Int32.Parse(fla),
                            Type = type,
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
                    doc5.Load(@"C:\Users\eary.ortiz\Documents\GitHub\ProodFloorCSharpp\ProdFloor\wwwroot\AppData\Overloads.xml");

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
                    doc6.Load(@"C:\Users\eary.ortiz\Documents\GitHub\ProodFloorCSharpp\ProdFloor\wwwroot\AppData\Countries.xml");

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

                case "States":
                    HtmlDocument doc2 = new HtmlDocument();
                    doc2.Load(@"C:\Users\eary.ortiz\Documents\GitHub\ProodFloorCSharpp\ProdFloor\wwwroot\AppData\States.xml");

                    var XMLobs2 = doc2.DocumentNode.SelectNodes("//wiretypessize");

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
                    break;

                case "City":
                    HtmlDocument doc7 = new HtmlDocument();
                    doc7.Load(@"C:\Users\eary.ortiz\Documents\GitHub\ProodFloorCSharpp\ProdFloor\wwwroot\AppData\Cities.xml");

                    var XMLobs7 = doc7.DocumentNode.SelectNodes("//city");

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
                    break;

                case "LandingSys":
                    HtmlDocument doc8 = new HtmlDocument();
                    doc8.Load(@"C:\Users\eary.ortiz\Documents\GitHub\ProodFloorCSharpp\ProdFloor\wwwroot\AppData\LandingSystems.xml");

                    var XMLobs8 = doc8.DocumentNode.SelectNodes("//landingsystem");

                    foreach (var XMLob in XMLobs8)
                    {
                        var ID = XMLob.SelectSingleNode(".//id").InnerText;
                        var usein = XMLob.SelectSingleNode(".//usein").InnerText;
                        var name = XMLob.SelectSingleNode(".//name").InnerText;

                        context.LandingSystems.Add(new LandingSystem
                        {
                            LandingSystemID = Int32.Parse(ID),
                            UsedIn = ID,
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
                    doc9.Load(@"C:\Users\eary.ortiz\Documents\GitHub\ProodFloorCSharpp\ProdFloor\wwwroot\AppData\FireCodes.xml");

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
                    doc10.Load(@"C:\Users\eary.ortiz\Documents\GitHub\ProodFloorCSharpp\ProdFloor\wwwroot\AppData\DoorOperators.xml");

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
                    doc11.Load(@"C:\Users\eary.ortiz\Documents\GitHub\ProodFloorCSharpp\ProdFloor\wwwroot\AppData\JobTypes.xml");

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
            }


        }
    }
}
