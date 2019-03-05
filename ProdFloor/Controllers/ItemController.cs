using Microsoft.AspNetCore.Mvc;
using ProdFloor.Models;
using ProdFloor.Models.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace ProdFloor.Controllers
{
    public class ItemController : Controller
    {
        private IItemRepository repository;

        public int PageSize = 4;

        public ItemController(IItemRepository repo)
        {
            repository = repo;
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
            }

            return File(ms, "text/xml", "Error.xml");
        }

    }
}
