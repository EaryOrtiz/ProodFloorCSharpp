using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProdFloor.Models;
using ProdFloor.Models.ViewModels;
using ProdFloor.Models.ViewModels.Reasons;

namespace ProdFloor.Controllers
{
    [Authorize(Roles = "Admin,TechAdmin,Technician")]
    public class ReasonsController : Controller
    {
        private ITestingRepository repository;
        public int PageSize = 7;

        public ReasonsController(ITestingRepository testingrepo)
        {
            repository = testingrepo;
        }


        /*public ViewResult List(int reasonNumber, int separator, int page = 1)
        {

            switch (reasonNumber)
            {
                case 1:
                    var ReasonCount = repository.Reasons1
                        .Where(s => s.Description != "-").Count();

                    return View("Reason1List", new ReasonViewModel
                    {
                        Reasons1 = repository.Reasons1
                        .Where(s => s.Description != "-")
                        .OrderBy(p => p.Reason1ID)
                        .Skip((page - 1) * PageSize)
                        .Take(PageSize).ToList(),
                        PagingInfo = new PagingInfo
                        {
                            CurrentPage = page,
                            ItemsPerPage = PageSize,
                            TotalItems = ReasonCount
                        },
                        CurrentSeparator = separator.ToString()
                    });

                case 2:
                    var Reason2Count = repository.Reasons2.Count();

                    return View("Reason2List", new ReasonViewModel
                    {
                        Reasons2 = repository.Reasons2
                        .Where(s => s.Description != "-")
                        .OrderBy(p => p.Reason2ID)
                        .Skip((page - 1) * PageSize)
                        .Take(PageSize).ToList(),
                        PagingInfo = new PagingInfo
                        {
                            CurrentPage = page,
                            ItemsPerPage = PageSize,
                            TotalItems = Reason2Count
                        },
                        CurrentSeparator = separator.ToString()
                    });

                case 3:
                    var Reason3Count = repository.Reasons3.Count();

                    return View("Reason3List", new ReasonViewModel
                    {
                        Reasons3 = repository.Reasons3
                        .Where(s => s.Description != "-")
                        .OrderBy(p => p.Reason3ID)
                        .Skip((page - 1) * PageSize)
                        .Take(PageSize).ToList(),
                        PagingInfo = new PagingInfo
                        {
                            CurrentPage = page,
                            ItemsPerPage = PageSize,
                            TotalItems = Reason3Count
                        },
                        CurrentSeparator = separator.ToString()
                    });

                case 4:
                    var Reason4Count = repository.Reasons4.Count();

                    return View("Reason4List", new ReasonViewModel
                    {
                        Reasons4 = repository.Reasons4
                        .Where(s => s.Description != "-")
                        .OrderBy(p => p.Reason4ID)
                        .Skip((page - 1) * PageSize)
                        .Take(PageSize).ToList(),
                        PagingInfo = new PagingInfo
                        {
                            CurrentPage = page,
                            ItemsPerPage = PageSize,
                            TotalItems = Reason4Count
                        },
                        CurrentSeparator = separator.ToString()
                    });

                case 5:
                    var Reason5Count = repository.Reasons5.Count();

                    return View("Reason5List", new ReasonViewModel
                    {
                        Reasons5 = repository.Reasons5
                        .Where(s => s.Description != "-")
                        .OrderBy(p => p.Reason5ID)
                        .Skip((page - 1) * PageSize)
                        .Take(PageSize).ToList(),
                        PagingInfo = new PagingInfo
                        {
                            CurrentPage = page,
                            ItemsPerPage = PageSize,
                            TotalItems = Reason5Count
                        },
                        CurrentSeparator = separator.ToString()
                    });
            }

            return View(NotFound());
        }
        */
        public ViewResult Reason1List(string filtrado, string Sort = "default", int page = 1, int totalitemsfromlastsearch = 0)
        {
            if (filtrado != null) Sort = filtrado;

            List<Reason1> reason1s = repository.Reasons1
                .Where(m => m.Description != "-")
                .OrderBy(p => p.Reason1ID).ToList();

            if (Sort != "default")
            {
                reason1s = repository.Reasons1
                 .Where(m => m.Description.Contains(Sort) && m.Description != "-")
                 .OrderBy(p => p.Reason1ID).ToList();
            }

            int TotalItemsSearch = repository.Reasons1.Count();
            if (page == 1)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
            }
            else if (TotalItemsSearch != totalitemsfromlastsearch)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
                page = 1;
            }
            ReasonViewModel reasonView = new ReasonViewModel
            {
                Reasons1 = reason1s.Skip((page - 1) * PageSize)
                .Take(PageSize).ToList(),
                TotalItems= repository.Reasons1.Count(),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    sort = Sort != "default" ? Sort : "default",
                    ItemsPerPage = PageSize,
                    TotalItemsFromLastSearch = totalitemsfromlastsearch,

                    TotalItems = TotalItemsSearch
                }
            };
            return View(reasonView);
        }

        public IActionResult Reason2List(ReasonViewModel viewModel, int page = 1, int totalitemsfromlastsearch = 0)
        {
            if (viewModel.CleanFields) return RedirectToAction("Reason2List");
            IQueryable<Reason2> reasons = repository.Reasons2.Where(m => m.Description != "-").AsQueryable();

            if (viewModel.Reason1ID > 0) reasons = reasons.Where(m => m.Reason1ID == viewModel.Reason1ID);
            if (!string.IsNullOrEmpty(viewModel.Description)) reasons = reasons.Where(m => m.Description.Contains(viewModel.Description));
            viewModel.TotalItems = repository.Reasons2.Count();

            int TotalItemsSearch = viewModel.TotalItems;
            if (page == 1)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
            }
            else if (TotalItemsSearch != totalitemsfromlastsearch)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
                page = 1;
            }
            viewModel.Reasons2 = reasons.OrderBy(p => p.Description).Skip((page - 1) * 5).Take(5).ToList();
            viewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = page,
                TotalItemsFromLastSearch = totalitemsfromlastsearch,
                ItemsPerPage = 5,
                TotalItems = reasons.Count()
            };
            return View(viewModel);
        }

        public IActionResult Reason3List(ReasonViewModel viewModel, int page = 1, int totalitemsfromlastsearch = 0)
        {
            if (viewModel.CleanFields) return RedirectToAction("Reason3List");
            IQueryable<Reason2> reasons2 = repository.Reasons2.AsQueryable();
            IQueryable<Reason3> reasons3 = repository.Reasons3.Where(m => m.Description != "-").AsQueryable();

            if (viewModel.Reason1ID > 0)
            {
                reasons2 = reasons2.Where(m => m.Reason1ID == viewModel.Reason1ID);
                reasons3 = reasons3.Where(m => reasons2.Any(s => s.Reason2ID == m.Reason2ID));
            }
            if (viewModel.Reason2ID > 0) reasons3 = reasons3.Where(m => m.Reason2ID == viewModel.Reason2ID);
            if (!string.IsNullOrEmpty(viewModel.Description)) reasons3 = reasons3.Where(m => m.Description.Contains(viewModel.Description));

            viewModel.TotalItems = repository.Reasons3.Count();

            int TotalItemsSearch = viewModel.TotalItems;
            if (page == 1)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
            }
            else if (TotalItemsSearch != totalitemsfromlastsearch)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
                page = 1;
            }
            viewModel.Reasons3 = reasons3.OrderBy(p => p.Description).Skip((page - 1) * 5).Take(5).ToList();
            viewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = page,
                ItemsPerPage = 5,
                TotalItemsFromLastSearch = totalitemsfromlastsearch,
                TotalItems = reasons3.Count()
            };
            return View(viewModel);
        }

        public IActionResult Reason4List(ReasonViewModel viewModel, int page = 1, int totalitemsfromlastsearch = 0)
        {
            if (viewModel.CleanFields) return RedirectToAction("Reason4List");
            IQueryable<Reason2> reasons2 = repository.Reasons2.AsQueryable();
            IQueryable<Reason3> reasons3 = repository.Reasons3.AsQueryable();
            IQueryable<Reason4> reasons4 = repository.Reasons4.Where(m => m.Description != "-").AsQueryable();

            if (viewModel.Reason1ID > 0)
            {
                reasons2 = reasons2.Where(m => m.Reason1ID == viewModel.Reason1ID);
                reasons3 = reasons3.Where(m => reasons2.Any(s => s.Reason2ID == m.Reason2ID));
                reasons4 = reasons4.Where(m => reasons3.Any(s => s.Reason3ID == m.Reason3ID));
            }
            if (viewModel.Reason2ID > 0)
            {
                reasons3 = reasons3.Where(m => m.Reason2ID == viewModel.Reason2ID);
                reasons4 = reasons4.Where(m => reasons3.Any(s => s.Reason3ID == m.Reason3ID));
            }
            if (viewModel.Reason3ID > 0) reasons4 = reasons4.Where(m => m.Reason3ID == viewModel.Reason3ID);


            if (!string.IsNullOrEmpty(viewModel.Description)) reasons4 = reasons4.Where(m => m.Description.Contains(viewModel.Description));

            viewModel.TotalItems = repository.Reasons4.Count();

            int TotalItemsSearch = viewModel.TotalItems;
            if (page == 1)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
            }
            else if (TotalItemsSearch != totalitemsfromlastsearch)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
                page = 1;
            }
            viewModel.Reasons4 = reasons4.OrderBy(p => p.Description).Skip((page - 1) * 5).Take(5).ToList();
            viewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = page,
                ItemsPerPage = 5,
                TotalItemsFromLastSearch = totalitemsfromlastsearch,
                TotalItems = reasons4.Count()
            };
            return View(viewModel);
        }

        public IActionResult Reason5List(ReasonViewModel viewModel, int page = 1, int totalitemsfromlastsearch = 0)
        {
            if (viewModel.CleanFields) return RedirectToAction("Reason5List");
            IQueryable<Reason2> reasons2 = repository.Reasons2.AsQueryable();
            IQueryable<Reason3> reasons3 = repository.Reasons3.AsQueryable();
            IQueryable<Reason4> reasons4 = repository.Reasons4.AsQueryable();
            IQueryable<Reason5> reasons5 = repository.Reasons5.Where(m => m.Description != "-").AsQueryable();

            if (viewModel.Reason1ID > 0)
            {
                reasons2 = reasons2.Where(m => m.Reason1ID == viewModel.Reason1ID);
                reasons3 = reasons3.Where(m => reasons2.Any(s => s.Reason2ID == m.Reason2ID));
                reasons4 = reasons4.Where(m => reasons3.Any(s => s.Reason3ID == m.Reason3ID));
                reasons5 = reasons5.Where(m => reasons4.Any(s => s.Reason4ID == m.Reason4ID));
            }
            if (viewModel.Reason2ID > 0)
            {
                reasons3 = reasons3.Where(m => m.Reason2ID == viewModel.Reason2ID);
                reasons4 = reasons4.Where(m => reasons3.Any(s => s.Reason3ID == m.Reason3ID));
                reasons5 = reasons5.Where(m => reasons4.Any(s => s.Reason4ID == m.Reason4ID));
            }
            if (viewModel.Reason3ID > 0)
            {
                reasons4 = reasons4.Where(m => m.Reason3ID == viewModel.Reason3ID);
                reasons5 = reasons5.Where(m => reasons4.Any(s => s.Reason4ID == m.Reason4ID));
            }
            if (viewModel.Reason4ID > 0) reasons5 = reasons5.Where(m => reasons4.Any(s => s.Reason4ID == m.Reason4ID));

            if (!string.IsNullOrEmpty(viewModel.Description)) reasons4 = reasons4.Where(m => m.Description.Contains(viewModel.Description));

            viewModel.TotalItems = repository.Reasons5.Count();

            int TotalItemsSearch = viewModel.TotalItems;
            if (page == 1)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
            }
            else if (TotalItemsSearch != totalitemsfromlastsearch)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
                page = 1;
            }
            viewModel.Reasons5 = reasons5.OrderBy(p => p.Description).Skip((page - 1) * 5).Take(5).ToList();
            viewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = page,
                ItemsPerPage = 5,
                TotalItemsFromLastSearch = totalitemsfromlastsearch,
                TotalItems = reasons5.Count()
            };
            return View(viewModel);
        }

        public ViewResult Add(string AddR)
        {
            switch (AddR)
            {
                case "R1": return View("Reason1Edit", new Reason1());
                case "R2": return View("Reason2Edit", new Reason2());
                case "R3": return View("Reason3Edit", new Reason3());
                case "R4": return View("Reason4Edit", new Reason4());
                case "R5": return View("Reason5Edit", new Reason5());
            }

            return View();
        }

        public ViewResult Edit(string btn, int ID)
        {
            switch (btn)
            {
                case "R1": return View("Reason1Edit", repository.Reasons1.FirstOrDefault(j => j.Reason1ID == ID));
                case "R2": return View("Reason2Edit", repository.Reasons2.FirstOrDefault(j => j.Reason2ID == ID));
                case "R3": return View("Reason3Edit", repository.Reasons3.FirstOrDefault(j => j.Reason3ID == ID));
                case "R4": return View("Reason4Edit", repository.Reasons4.FirstOrDefault(j => j.Reason4ID == ID));
                case "R5": return View("Reason5Edit", repository.Reasons5.FirstOrDefault(j => j.Reason5ID == ID));
            }

            return View(NotFound());
        }

        [HttpPost]
        public IActionResult EditR1(Reason1 reason)
        {
            if (ModelState.IsValid)
            {
                repository.SaveReason1(reason);
                TempData["message"] = $"{reason.Reason1ID},, has been saved...";
                return RedirectToAction("Reason1List");
            }
            else
            {
                // there is something wrong with the data values
                return View(reason);
            }
        }

        [HttpPost]
        public IActionResult EditR2(Reason2 reason)
        {
            if (ModelState.IsValid)
            {
                repository.SaveReason2(reason);
                TempData["message"] = $"{reason.Reason2ID},, has been saved...";
                return RedirectToAction("Reason2List");
            }
            else
            {
                // there is something wrong with the data values
                return View(reason);
            }
        }

        [HttpPost]
        public IActionResult EditR3(Reason3 reason)
        {
            if (ModelState.IsValid)
            {
                repository.SaveReason3(reason);
                TempData["message"] = $"{reason.Reason3ID}, has been saved...";
                return RedirectToAction("Reason3List");
            }
            else
            {
                // there is something wrong with the data values
                return View(reason);
            }
        }

        [HttpPost]
        public IActionResult EditR4(Reason4 reason)
        {
            if (ModelState.IsValid)
            {
                repository.SaveReason4(reason);
                TempData["message"] = $"{reason.Reason4ID},, has been saved...";
                return RedirectToAction("Reason4List");
            }
            else
            {
                // there is something wrong with the data values
                return View(reason);
            }
        }

        [HttpPost]
        public IActionResult EditR5(Reason5 reason)
        {
            if (ModelState.IsValid)
            {
                repository.SaveReason5(reason);
                TempData["message"] = $"{reason.Reason5ID},, has been saved...";
                return RedirectToAction("Reason5List");
            }
            else
            {
                // there is something wrong with the data values
                return View(reason);
            }
        }

        [HttpPost]
        public IActionResult Delete(string btn, int ID)
        {
            switch (btn)
            {
                case "R1":
                    Reason1 deleteReason = repository.DeleteReason1(ID);

                    if (deleteReason != null)
                    {
                        TempData["message"] = $"{deleteReason.Reason1ID} was deleted";
                    }
                    return RedirectToAction("Reason1List");

                case "R2":
                    Reason2 deleteReason2 = repository.DeleteReason2(ID);

                    if (deleteReason2 != null)
                    {
                        TempData["message"] = $"{deleteReason2.Reason2ID} was deleted";
                    }
                    return RedirectToAction("Reason2List");
                case "R3":
                    Reason3 deleteReason3 = repository.DeleteReason3(ID);

                    if (deleteReason3 != null)
                    {
                        TempData["message"] = $"{deleteReason3.Reason3ID} was deleted";
                    }
                    return RedirectToAction("Reason3List");
                case "R4":
                    Reason4 deleteReason4 = repository.DeleteReason4(ID);

                    if (deleteReason4 != null)
                    {
                        TempData["message"] = $"{deleteReason4.Reason4ID} was deleted";
                    }
                    return RedirectToAction("Reason4List");
                case "R5":
                    Reason5 deleteReason5 = repository.DeleteReason5(ID);

                    if (deleteReason5 != null)
                    {
                        TempData["message"] = $"{deleteReason5.Reason5ID} was deleted";
                    }
                    return RedirectToAction("Reason5List");
            }

            return NotFound();
        }

        public JsonResult GetReason2(int Reason1ID)
        {
            List<Reason2> Reasonlist = new List<Reason2>();
            Reasonlist = (from reason2 in repository.Reasons2 where reason2.Reason1ID == Reason1ID && reason2.Description != "-" select reason2).ToList();
            return Json(new SelectList(Reasonlist, "Reason2ID", "Description"));
        }

        public JsonResult GetReason3(int Reason2ID)
        {
            List<Reason3> Reasonlist = new List<Reason3>();
            Reasonlist = (from reason3 in repository.Reasons3 where reason3.Reason2ID == Reason2ID && reason3.Description != "-" select reason3).ToList();
            return Json(new SelectList(Reasonlist, "Reason3ID", "Description"));
        }

        public JsonResult GetReason4(int Reason3ID)
        {
            List<Reason4> Reasonlist = new List<Reason4>();
            Reasonlist = (from reason4 in repository.Reasons4 where reason4.Reason3ID == Reason3ID && reason4.Description != "-" select reason4).ToList();
            return Json(new SelectList(Reasonlist, "Reason4ID", "Description"));
        }

        public JsonResult GetReason5(int Reason4ID)
        {
            List<Reason5> Reasonlist = new List<Reason5>();
            Reasonlist = (from reason5 in repository.Reasons5 where reason5.Reason4ID == Reason4ID && reason5.Description != "-" select reason5).ToList();
            return Json(new SelectList(Reasonlist, "Reason5ID", "Description"));
        }

        [HttpPost]
        public FileStreamResult ExportToXML()
        {
            MemoryStream ms = new MemoryStream();
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.OmitXmlDeclaration = true;
            xws.Indent = true;

            List<Reason1> reason1s = repository.Reasons1.Where(m => m.Reason1ID != 0).ToList();
            List<Reason2> reason2s = repository.Reasons2.Where(m => m.Reason2ID != 0).ToList();
            List<Reason3> reason3s = repository.Reasons3.Where(m => m.Reason3ID != 0).ToList();
            List<Reason4> reason4s = repository.Reasons4.Where(m => m.Reason4ID != 0).ToList();
            List<Reason5> reason5s = repository.Reasons5.Where(m => m.Reason5ID != 0).ToList();

            if (reason5s.Count > 0)
            {
                using (XmlWriter xw = XmlWriter.Create(ms, xws))
                {
                    xw.WriteStartDocument();
                    xw.WriteStartElement("Reasons");

                    xw.WriteStartElement("Reasons1");
                    foreach (Reason1 reason1 in reason1s)
                    {
                        xw.WriteStartElement("Reason1");
                        xw.WriteElementString("Reason1ID", reason1.Reason1ID.ToString());
                        xw.WriteElementString("Description", reason1.Description.ToString());
                        xw.WriteEndElement();
                    }
                    xw.WriteEndElement();
                    xw.WriteStartElement("Reasons2");
                    foreach (Reason2 reason2 in reason2s)
                    {
                        xw.WriteStartElement("Reason2");
                        xw.WriteElementString("Reason2ID", reason2.Reason2ID.ToString());
                        xw.WriteElementString("Reason1ID", reason2.Reason1ID.ToString());
                        xw.WriteElementString("Description", reason2.Description.ToString());
                        xw.WriteEndElement();
                    }
                    xw.WriteEndElement();
                    xw.WriteStartElement("Reasons3");
                    foreach (Reason3 reason3 in reason3s)
                    {
                        xw.WriteStartElement("Reason3");
                        xw.WriteElementString("Reason3ID", reason3.Reason3ID.ToString());
                        xw.WriteElementString("Reason2ID", reason3.Reason2ID.ToString());
                        xw.WriteElementString("Description", reason3.Description.ToString());
                        xw.WriteEndElement();
                    }
                    xw.WriteEndElement();
                    xw.WriteStartElement("Reasons4");
                    foreach (Reason4 reason4 in reason4s)
                    {
                        xw.WriteStartElement("Reason4");
                        xw.WriteElementString("Reason4ID", reason4.Reason4ID.ToString());
                        xw.WriteElementString("Reason3ID", reason4.Reason3ID.ToString());
                        xw.WriteElementString("Description", reason4.Description.ToString());
                        xw.WriteEndElement();
                    }
                    xw.WriteEndElement();
                    xw.WriteStartElement("Reasons5");
                    foreach (Reason5 reason5 in reason5s)
                    {
                        xw.WriteStartElement("Reason5");
                        xw.WriteElementString("Reason5ID", reason5.Reason5ID.ToString());
                        xw.WriteElementString("Reason4ID", reason5.Reason4ID.ToString());
                        xw.WriteElementString("Description", reason5.Description.ToString());
                        xw.WriteEndElement();
                    }
                    xw.WriteEndElement();
                    xw.WriteEndElement();
                    xw.WriteEndDocument();
                }
                ms.Position = 0;
                return File(ms, "text/xml", "Reasons.xml");
            }

            ms.Position = 0;
            return File(ms, "text/xml", "Error.xml");
        }

        public static void ImportXML(IServiceProvider services)
        {
            ApplicationDbContext context = services.GetRequiredService<ApplicationDbContext>();
            HtmlDocument doc = new HtmlDocument();
            doc.Load(@"C:\ProdFloorBeta\ProdFloor\wwwroot\AppData\Reasons.xml");

            var XMLobs = doc.DocumentNode.SelectSingleNode("//reasons");

            var ALLReasons1 = XMLobs.SelectSingleNode(".//reasons1");
            var ALLReasons2 = XMLobs.SelectSingleNode(".//reasons2");
            var ALLReasons3 = XMLobs.SelectSingleNode(".//reasons3");
            var ALLReasons4 = XMLobs.SelectSingleNode(".//reasons4");
            var ALLReasons5 = XMLobs.SelectSingleNode(".//reasons5");

            var XMLReasons1 = ALLReasons1.SelectNodes(".//reason1");
            var XMLReasons2 = ALLReasons2.SelectNodes(".//reason2");
            var XMLReasons3 = ALLReasons3.SelectNodes(".//reason3");
            var XMLReasons4 = ALLReasons4.SelectNodes(".//reason4");
            var XMLReasons5 = ALLReasons5.SelectNodes(".//reason5");

            if (XMLobs != null && !context.Reasons5.Any())
            {
                foreach (var Reasons1 in XMLReasons1)
                {
                    var reason1id = Reasons1.SelectSingleNode(".//reason1id").InnerText;
                    var description = Reasons1.SelectSingleNode(".//description").InnerText;
                    context.Reasons1.Add(new Reason1
                    {
                        Reason1ID = Int32.Parse(reason1id),
                        Description = description,

                    });
                    context.Database.OpenConnection();
                    try
                    {
                        context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Reasons1 ON");
                        context.SaveChanges();
                        context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Reasons1 OFF");
                    }
                    finally
                    {
                        context.Database.CloseConnection();
                    }
                }
                foreach (var Reasons2 in XMLReasons2)
                {
                    var reason2id = Reasons2.SelectSingleNode(".//reason2id").InnerText;
                    var reason1id = Reasons2.SelectSingleNode(".//reason1id").InnerText;
                    var description = Reasons2.SelectSingleNode(".//description").InnerText;
                    context.Reasons2.Add(new Reason2
                    {
                        Reason2ID = Int32.Parse(reason2id),
                        Reason1ID = Int32.Parse(reason1id),
                        Description = description

                    });
                    context.Database.OpenConnection();
                    try
                    {
                        context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Reasons2 ON");
                        context.SaveChanges();
                        context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Reasons2 OFF");
                    }
                    finally
                    {
                        context.Database.CloseConnection();
                    }
                }
                foreach (var Reasons3 in XMLReasons3)
                {
                    var reason3id = Reasons3.SelectSingleNode(".//reason3id").InnerText;
                    var reason2id = Reasons3.SelectSingleNode(".//reason2id").InnerText;
                    var description = Reasons3.SelectSingleNode(".//description").InnerText;
                    context.Reasons3.Add(new Reason3
                    {
                        Reason3ID = Int32.Parse(reason3id),
                        Reason2ID = Int32.Parse(reason2id),
                        Description = description

                    });
                    context.Database.OpenConnection();
                    try
                    {
                        context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Reasons3 ON");
                        context.SaveChanges();
                        context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Reasons3 OFF");
                    }
                    finally
                    {
                        context.Database.CloseConnection();
                    }
                }
                foreach (var Reasons4 in XMLReasons4)
                {
                    var reason4id = Reasons4.SelectSingleNode(".//reason4id").InnerText;
                    var reason3id = Reasons4.SelectSingleNode(".//reason3id").InnerText;
                    var description = Reasons4.SelectSingleNode(".//description").InnerText;
                    context.Reasons4.Add(new Reason4
                    {
                        Reason4ID = Int32.Parse(reason4id),
                        Reason3ID = Int32.Parse(reason3id),
                        Description = description

                    });
                    context.Database.OpenConnection();
                    try
                    {
                        context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Reasons4 ON");
                        context.SaveChanges();
                        context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Reasons4 OFF");
                    }
                    finally
                    {
                        context.Database.CloseConnection();
                    }
                }

                foreach (var Reasons5 in XMLReasons5)
                {
                    var reason5id = Reasons5.SelectSingleNode(".//reason5id").InnerText;
                    var reason4id = Reasons5.SelectSingleNode(".//reason4id").InnerText;
                    var description = Reasons5.SelectSingleNode(".//description").InnerText;
                    context.Reasons5.Add(new Reason5
                    {
                        Reason5ID = Int32.Parse(reason5id),
                        Reason4ID = Int32.Parse(reason4id),
                        Description = description

                    });
                    context.Database.OpenConnection();
                    try
                    {
                        context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Reasons5 ON");
                        context.SaveChanges();
                        context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Reasons5 OFF");
                    }
                    finally
                    {
                        context.Database.CloseConnection();
                    }
                }
            }

        }

        [HttpPost]
        public IActionResult SeedXML()
        {
            ReasonsController.ImportXML(HttpContext.RequestServices);
            return RedirectToAction(nameof(Reason1List));
        }

        public IActionResult SearchR1()
        {
            try
            {
                string term = HttpContext.Request.Query["term"].ToString();
                var names = repository.Reasons1.Where(p => p.Description.Contains(term)).Select(p => p.Description).Distinct().ToList();
                return Ok(names);
            }
            catch
            {
                return BadRequest();
            }
        }

    }
}