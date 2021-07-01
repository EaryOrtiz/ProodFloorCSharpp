using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
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
    public class WiringReasonsController : Controller
    {
        private IWiringRepository repository;
        private IHostingEnvironment _env;
        private UserManager<AppUser> userManager;
        public int PageSize = 7;
        string appDataFolder => _env.WebRootPath.ToString() + @"\AppData\";

        public WiringReasonsController(IWiringRepository wiringrepo,
            IHostingEnvironment env,
            UserManager<AppUser> userMrg)
        {
            repository = wiringrepo;
            _env = env;
            userManager = userMrg;
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

            List<WiringReason1> reason1s = repository.WiringReasons1
                .Where(m => m.Description != "-")
                .OrderBy(p => p.WiringReason1ID).ToList();

            if (Sort != "default")
            {
                reason1s = repository.WiringReasons1
                 .Where(m => m.Description.Contains(Sort) && m.Description != "-")
                 .OrderBy(p => p.WiringReason1ID).ToList();
            }

            int TotalItemsSearch = repository.WiringReasons1.Count();
            if (page == 1)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
            }
            else if (TotalItemsSearch != totalitemsfromlastsearch)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
                page = 1;
            }
            WiringReasonViewModel reasonView = new WiringReasonViewModel
            {
                WiringReasons1 = reason1s.Skip((page - 1) * PageSize)
                .Take(PageSize).ToList(),
                TotalItems= repository.WiringReasons1.Count(),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    sort = Sort != "default" ? Sort : "default",
                    TotalItemsFromLastSearch = totalitemsfromlastsearch,
                    ItemsPerPage = PageSize,

                    TotalItems = TotalItemsSearch
                }
            };
            return View(reasonView);
        }

        public IActionResult Reason2List(WiringReasonViewModel viewModel, int page = 1, int totalitemsfromlastsearch = 0)
        {
            if (viewModel.CleanFields) return RedirectToAction("Reason2List");
            IQueryable<WiringReason2> reasons = repository.WiringReasons2.Where(m => m.Description != "-").AsQueryable();

            if (viewModel.WiringReason1ID > 0) reasons = reasons.Where(m => m.WiringReason1ID == viewModel.WiringReason1ID);
            if (!string.IsNullOrEmpty(viewModel.Description)) reasons = reasons.Where(m => m.Description.Contains(viewModel.Description));


            viewModel.TotalItems = reasons.Count();

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
            viewModel.WiringReasons2 = reasons.OrderBy(p => p.Description).Skip((page - 1) * 5).Take(5).ToList();
            viewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = page,
                TotalItemsFromLastSearch = totalitemsfromlastsearch,
                ItemsPerPage = 5,
                TotalItems = reasons.Count()
            };
            return View(viewModel);
        }

        public IActionResult Reason3List(WiringReasonViewModel viewModel, int page = 1, int totalitemsfromlastsearch = 0)
        {
            if (viewModel.CleanFields) return RedirectToAction("Reason3List");
            IQueryable<WiringReason2> reasons2 = repository.WiringReasons2.AsQueryable();
            IQueryable<WiringReason3> reasons3 = repository.WiringReasons3.Where(m => m.Description != "-").AsQueryable();

            if (viewModel.WiringReason1ID > 0)
            {
                reasons2 = reasons2.Where(m => m.WiringReason1ID == viewModel.WiringReason1ID);
                reasons3 = reasons3.Where(m => reasons2.Any(s => s.WiringReason2ID == m.WiringReason2ID));
            }
            if (viewModel.WiringReason2ID > 0) reasons3 = reasons3.Where(m => m.WiringReason2ID == viewModel.WiringReason2ID);
            if (!string.IsNullOrEmpty(viewModel.Description)) reasons3 = reasons3.Where(m => m.Description.Contains(viewModel.Description));


            int TotalItemsSearch = reasons3.Count();
            if (page == 1)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
            }
            else if (TotalItemsSearch != totalitemsfromlastsearch)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
                page = 1;
            }
            viewModel.WiringReasons3 = reasons3.OrderBy(p => p.Description).Skip((page - 1) * 5).Take(5).ToList();
            viewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = page,
                ItemsPerPage = 5,
                TotalItemsFromLastSearch = totalitemsfromlastsearch,
                TotalItems = reasons3.Count()
            };
            return View(viewModel);
        }

        public IActionResult Reason4List(WiringReasonViewModel viewModel, int page = 1, int totalitemsfromlastsearch = 0)
        {
            if (viewModel.CleanFields) return RedirectToAction("Reason4List");
            IQueryable<WiringReason2> reasons2 = repository.WiringReasons2.AsQueryable();
            IQueryable<WiringReason3> reasons3 = repository.WiringReasons3.AsQueryable();
            IQueryable<WiringReason4> reasons4 = repository.WiringReasons4.Where(m => m.Description != "-").AsQueryable();

            if (viewModel.WiringReason1ID > 0)
            {
                reasons2 = reasons2.Where(m => m.WiringReason1ID == viewModel.WiringReason1ID);
                reasons3 = reasons3.Where(m => reasons2.Any(s => s.WiringReason2ID == m.WiringReason2ID));
                reasons4 = reasons4.Where(m => reasons3.Any(s => s.WiringReason3ID == m.WiringReason3ID));
            }
            if (viewModel.WiringReason2ID > 0)
            {
                reasons3 = reasons3.Where(m => m.WiringReason2ID == viewModel.WiringReason2ID);
                reasons4 = reasons4.Where(m => reasons3.Any(s => s.WiringReason3ID == m.WiringReason3ID));
            }
            if (viewModel.WiringReason3ID > 0) reasons4 = reasons4.Where(m => m.WiringReason3ID == viewModel.WiringReason3ID);


            if (!string.IsNullOrEmpty(viewModel.Description)) reasons4 = reasons4.Where(m => m.Description.Contains(viewModel.Description));

            int TotalItemsSearch = reasons4.Count();
            if (page == 1)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
            }
            else if (TotalItemsSearch != totalitemsfromlastsearch)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
                page = 1;
            }
            viewModel.WiringReasons4 = reasons4.OrderBy(p => p.Description).Skip((page - 1) * 5).Take(5).ToList();
            viewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = page,
                ItemsPerPage = 5,
                TotalItemsFromLastSearch = totalitemsfromlastsearch,
                TotalItems = reasons4.Count()
            };
            return View(viewModel);
        }

        public IActionResult Reason5List(WiringReasonViewModel viewModel, int page = 1, int totalitemsfromlastsearch = 0)
        {
            if (viewModel.CleanFields) return RedirectToAction("Reason5List");
            IQueryable<WiringReason2> reasons2 = repository.WiringReasons2.AsQueryable();
            IQueryable<WiringReason3> reasons3 = repository.WiringReasons3.AsQueryable();
            IQueryable<WiringReason4> reasons4 = repository.WiringReasons4.AsQueryable();
            IQueryable<WiringReason5> reasons5 = repository.WiringReasons5.Where(m => m.Description != "-").AsQueryable();

            if (viewModel.WiringReason1ID > 0)
            {
                reasons2 = reasons2.Where(m => m.WiringReason1ID == viewModel.WiringReason1ID);
                reasons3 = reasons3.Where(m => reasons2.Any(s => s.WiringReason2ID == m.WiringReason2ID));
                reasons4 = reasons4.Where(m => reasons3.Any(s => s.WiringReason3ID == m.WiringReason3ID));
                reasons5 = reasons5.Where(m => reasons4.Any(s => s.WiringReason4ID == m.WiringReason4ID));
            }
            if (viewModel.WiringReason2ID > 0)
            {
                reasons3 = reasons3.Where(m => m.WiringReason2ID == viewModel.WiringReason2ID);
                reasons4 = reasons4.Where(m => reasons3.Any(s => s.WiringReason3ID == m.WiringReason3ID));
                reasons5 = reasons5.Where(m => reasons4.Any(s => s.WiringReason4ID == m.WiringReason4ID));
            }
            if (viewModel.WiringReason3ID > 0)
            {
                reasons4 = reasons4.Where(m => m.WiringReason3ID == viewModel.WiringReason3ID);
                reasons5 = reasons5.Where(m => reasons4.Any(s => s.WiringReason4ID == m.WiringReason4ID));
            }
            if (viewModel.WiringReason4ID > 0) reasons5 = reasons5.Where(m => reasons4.Any(s => s.WiringReason4ID == m.WiringReason4ID));

            if (!string.IsNullOrEmpty(viewModel.Description)) reasons4 = reasons4.Where(m => m.Description.Contains(viewModel.Description));


            int TotalItemsSearch = reasons5.Count();
            if (page == 1)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
            }
            else if (TotalItemsSearch != totalitemsfromlastsearch)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
                page = 1;
            }
            viewModel.WiringReasons5 = reasons5.OrderBy(p => p.Description).Skip((page - 1) * 5).Take(5).ToList();
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
                case "R1": return View("WiringReason1Edit", new Reason1());
                case "R2": return View("RWiringeason2Edit", new Reason2());
                case "R3": return View("WiringReason3Edit", new Reason3());
                case "R4": return View("WiringReason4Edit", new Reason4());
                case "R5": return View("WiringReason5Edit", new Reason5());
            }

            return View();
        }

        public ViewResult Edit(string btn, int ID)
        {
            switch (btn)
            {
                case "R1": return View("WiringReason1Edit", repository.WiringReasons1.FirstOrDefault(j => j.WiringReason1ID == ID));
                case "R2": return View("WiringReason2Edit", repository.WiringReasons2.FirstOrDefault(j => j.WiringReason2ID == ID));
                case "R3": return View("WiringReason3Edit", repository.WiringReasons3.FirstOrDefault(j => j.WiringReason3ID == ID));
                case "R4": return View("WiringReason4Edit", repository.WiringReasons4.FirstOrDefault(j => j.WiringReason4ID == ID));
                case "R5": return View("WiringReason5Edit", repository.WiringReasons5.FirstOrDefault(j => j.WiringReason5ID == ID));
            }

            return View(NotFound());
        }

        [HttpPost]
        public IActionResult EditR1(WiringReason1 reason)
        {
            if (ModelState.IsValid)
            {
                repository.SaveWiringReason1(reason);
                TempData["message"] = $"{reason.WiringReason1ID},, has been saved...";
                return RedirectToAction("WiringReason1List");
            }
            else
            {
                // there is something wrong with the data values
                return View(reason);
            }
        }

        [HttpPost]
        public IActionResult EditR2(WiringReason2 reason)
        {
            if (ModelState.IsValid)
            {
                repository.SaveWiringReason2(reason);
                TempData["message"] = $"{reason.WiringReason2ID},, has been saved...";
                return RedirectToAction("WiringReason2List");
            }
            else
            {
                // there is something wrong with the data values
                return View(reason);
            }
        }

        [HttpPost]
        public IActionResult EditR3(WiringReason3 reason)
        {
            if (ModelState.IsValid)
            {
                repository.SaveWiringReason3(reason);
                TempData["message"] = $"{reason.WiringReason3ID}, has been saved...";
                return RedirectToAction("WiringReason3List");
            }
            else
            {
                // there is something wrong with the data values
                return View(reason);
            }
        }

        [HttpPost]
        public IActionResult EditR4(WiringReason4 reason)
        {
            if (ModelState.IsValid)
            {
                repository.SaveWiringReason4(reason);
                TempData["message"] = $"{reason.WiringReason4ID},, has been saved...";
                return RedirectToAction("WiringReason4List");
            }
            else
            {
                // there is something wrong with the data values
                return View(reason);
            }
        }

        [HttpPost]
        public IActionResult EditR5(WiringReason5 reason)
        {
            if (ModelState.IsValid)
            {
                repository.SaveWiringReason5(reason);
                TempData["message"] = $"{reason.WiringReason5ID},, has been saved...";
                return RedirectToAction("WiringReason5List");
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
            bool admin = GetCurrentUserRole("Admin").Result;

            if (!admin)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"You don't have permissions, contact to your admin";

                return RedirectToAction("Reason1List");
            }

            switch (btn)
            {
                case "R1":
                    WiringReason1 deleteReason = repository.DeleteWiringReason1(ID);

                    if (deleteReason != null)
                    {
                        TempData["message"] = $"{deleteReason.WiringReason1ID} was deleted";
                    }
                    return RedirectToAction("WiringReason1List");

                case "R2":
                    WiringReason2 deleteReason2 = repository.DeleteWiringReason2(ID);

                    if (deleteReason2 != null)
                    {
                        TempData["message"] = $"{deleteReason2.WiringReason2ID} was deleted";
                    }
                    return RedirectToAction("WiringReason2List");
                case "R3":
                    WiringReason3 deleteReason3 = repository.DeleteWiringReason3(ID);

                    if (deleteReason3 != null)
                    {
                        TempData["message"] = $"{deleteReason3.WiringReason3ID} was deleted";
                    }
                    return RedirectToAction("WiringReason3List");
                case "R4":
                    WiringReason4 deleteReason4 = repository.DeleteWiringReason4(ID);

                    if (deleteReason4 != null)
                    {
                        TempData["message"] = $"{deleteReason4.WiringReason4ID} was deleted";
                    }
                    return RedirectToAction("Reason4List");
                case "R5":
                    WiringReason5 deleteReason5 = repository.DeleteWiringReason5(ID);

                    if (deleteReason5 != null)
                    {
                        TempData["message"] = $"{deleteReason5.WiringReason5ID} was deleted";
                    }
                    return RedirectToAction("Reason5List");
            }

            return NotFound();
        }

        public JsonResult GetReason2(int WiringReason1ID)
        {
            List<WiringReason2> WiringReasonlist = new List<WiringReason2>();
            WiringReasonlist = (from Wiringreason2 in repository.WiringReasons2 where Wiringreason2.WiringReason1ID == WiringReason1ID && Wiringreason2.Description != "-" select Wiringreason2).ToList();
            return Json(new SelectList(WiringReasonlist, "WiringReason2ID", "Description"));
        }

        public JsonResult GetReason3(int WiringReason2ID)
        {
            List<WiringReason3> WiringReasonlist = new List<WiringReason3>();
            WiringReasonlist = (from Wiringreason3 in repository.WiringReasons3 where Wiringreason3.WiringReason2ID == WiringReason2ID && Wiringreason3.Description != "-" select Wiringreason3).ToList();
            return Json(new SelectList(WiringReasonlist, "WiringReason3ID", "Description"));
        }

        public JsonResult GetReason4(int WiringReason3ID)
        {
            List<WiringReason4> WiringReasonlist = new List<WiringReason4>();
            WiringReasonlist = (from Wiringreason4 in repository.WiringReasons4 where Wiringreason4.WiringReason3ID == WiringReason3ID && Wiringreason4.Description != "-" select Wiringreason4).ToList();
            return Json(new SelectList(WiringReasonlist, "WiringReason4ID", "Description"));
        }

        public JsonResult GetReason5(int WiringReason4ID)
        {
            List<WiringReason5> WiringReasonlist = new List<WiringReason5>();
            WiringReasonlist = (from Wiringreason5 in repository.WiringReasons5 where Wiringreason5.WiringReason4ID == WiringReason4ID && Wiringreason5.Description != "-" select Wiringreason5).ToList();
            return Json(new SelectList(WiringReasonlist, "WiringReason5ID", "Description"));
        }

        [HttpPost]
        public FileStreamResult ExportToXML()
        {
            MemoryStream ms = new MemoryStream();
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.OmitXmlDeclaration = true;
            xws.Indent = true;

            List<WiringReason1> reason1s = repository.WiringReasons1.Where(m => m.WiringReason1ID != 0).ToList();
            List<WiringReason2> reason2s = repository.WiringReasons2.Where(m => m.WiringReason2ID != 0).ToList();
            List<WiringReason3> reason3s = repository.WiringReasons3.Where(m => m.WiringReason3ID != 0).ToList();
            List<WiringReason4> reason4s = repository.WiringReasons4.Where(m => m.WiringReason4ID != 0).ToList();
            List<WiringReason5> reason5s = repository.WiringReasons5.Where(m => m.WiringReason5ID != 0).ToList();

            if (reason5s.Count > 0)
            {
                using (XmlWriter xw = XmlWriter.Create(ms, xws))
                {
                    xw.WriteStartDocument();
                    xw.WriteStartElement("WiringReasons");

                    xw.WriteStartElement("WiringReasons1");
                    foreach (WiringReason1 reason1 in reason1s)
                    {
                        xw.WriteStartElement("WiringReason1");
                        xw.WriteElementString("WiringReason1ID", reason1.WiringReason1ID.ToString());
                        xw.WriteElementString("Description", reason1.Description.ToString());
                        xw.WriteEndElement();
                    }
                    xw.WriteEndElement();
                    xw.WriteStartElement("WiringReasons2");
                    foreach (WiringReason2 reason2 in reason2s)
                    {
                        xw.WriteStartElement("WiringReason2");
                        xw.WriteElementString("WiringReason2ID", reason2.WiringReason2ID.ToString());
                        xw.WriteElementString("Reason1ID", reason2.WiringReason1ID.ToString());
                        xw.WriteElementString("Description", reason2.Description.ToString());
                        xw.WriteEndElement();
                    }
                    xw.WriteEndElement();
                    xw.WriteStartElement("WiringReasons3");
                    foreach (WiringReason3 reason3 in reason3s)
                    {
                        xw.WriteStartElement("WiringReason3");
                        xw.WriteElementString("WiringReason3ID", reason3.WiringReason3ID.ToString());
                        xw.WriteElementString("Reason2ID", reason3.WiringReason2ID.ToString());
                        xw.WriteElementString("Description", reason3.Description.ToString());
                        xw.WriteEndElement();
                    }
                    xw.WriteEndElement();
                    xw.WriteStartElement("WiringReasons4");
                    foreach (WiringReason4 reason4 in reason4s)
                    {
                        xw.WriteStartElement("WiringReason4");
                        xw.WriteElementString("WiringReason4ID", reason4.WiringReason4ID.ToString());
                        xw.WriteElementString("WiringReason3ID", reason4.WiringReason3ID.ToString());
                        xw.WriteElementString("Description", reason4.Description.ToString());
                        xw.WriteEndElement();
                    }
                    xw.WriteEndElement();
                    xw.WriteStartElement("WiringReasons5");
                    foreach (WiringReason5 reason5 in reason5s)
                    {
                        xw.WriteStartElement("Reason5");
                        xw.WriteElementString("WiringReason5ID", reason5.WiringReason5ID.ToString());
                        xw.WriteElementString("WiringReason4ID", reason5.WiringReason4ID.ToString());
                        xw.WriteElementString("Description", reason5.Description.ToString());
                        xw.WriteEndElement();
                    }
                    xw.WriteEndElement();
                    xw.WriteEndElement();
                    xw.WriteEndDocument();
                }
                ms.Position = 0;
                return File(ms, "text/xml", "WiringReasons.xml");
            }

            ms.Position = 0;
            return File(ms, "text/xml", "Error.xml");
        }

        public  void ImportXML(IServiceProvider services)
        {
            ApplicationDbContext context = services.GetRequiredService<ApplicationDbContext>();
            HtmlDocument doc = new HtmlDocument();
            doc.Load(appDataFolder + "WiringReasons.xml");

            var XMLobs = doc.DocumentNode.SelectSingleNode("//reasons");

            var ALLReasons1 = XMLobs.SelectSingleNode(".//wiringreasons1");
            var ALLReasons2 = XMLobs.SelectSingleNode(".//wiringreasons2");
            var ALLReasons3 = XMLobs.SelectSingleNode(".//wiringreasons3");
            var ALLReasons4 = XMLobs.SelectSingleNode(".//wiringreasons4");
            var ALLReasons5 = XMLobs.SelectSingleNode(".//wiringreasons5");

            var XMLReasons1 = ALLReasons1.SelectNodes(".//wiringreason1");
            var XMLReasons2 = ALLReasons2.SelectNodes(".//wiringreason2");
            var XMLReasons3 = ALLReasons3.SelectNodes(".//wiringreason3");
            var XMLReasons4 = ALLReasons4.SelectNodes(".//wiringreason4");
            var XMLReasons5 = ALLReasons5.SelectNodes(".//wiringreason5");

            if (XMLobs != null && !context.Reasons5.Any())
            {
                foreach (var Reasons1 in XMLReasons1)
                {
                    var reason1id = Reasons1.SelectSingleNode(".//wiringreason1id").InnerText;
                    var description = Reasons1.SelectSingleNode(".//description").InnerText;
                    context.WiringReasons1.Add(new WiringReason1
                    {
                        WiringReason1ID = Int32.Parse(reason1id),
                        Description = description,

                    });
                    context.Database.OpenConnection();
                    try
                    {
                        context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.WiringReasons1 ON");
                        context.SaveChanges();
                        context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.WiringReasons1 OFF");
                    }
                    finally
                    {
                        context.Database.CloseConnection();
                    }
                }
                foreach (var Reasons2 in XMLReasons2)
                {
                    var reason2id = Reasons2.SelectSingleNode(".//wiringreason2id").InnerText;
                    var reason1id = Reasons2.SelectSingleNode(".//wiringreason1id").InnerText;
                    var description = Reasons2.SelectSingleNode(".//description").InnerText;
                    context.WiringReasons2.Add(new WiringReason2
                    {
                        WiringReason2ID = Int32.Parse(reason2id),
                        WiringReason1ID = Int32.Parse(reason1id),
                        Description = description

                    });
                    context.Database.OpenConnection();
                    try
                    {
                        context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.WiringReasons2 ON");
                        context.SaveChanges();
                        context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.WiringReasons2 OFF");
                    }
                    finally
                    {
                        context.Database.CloseConnection();
                    }
                }
                foreach (var Reasons3 in XMLReasons3)
                {
                    var reason3id = Reasons3.SelectSingleNode(".//wiringreason3id").InnerText;
                    var reason2id = Reasons3.SelectSingleNode(".//wiringreason2id").InnerText;
                    var description = Reasons3.SelectSingleNode(".//description").InnerText;
                    context.WiringReasons3.Add(new WiringReason3
                    {
                        WiringReason3ID = Int32.Parse(reason3id),
                        WiringReason2ID = Int32.Parse(reason2id),
                        Description = description

                    });
                    context.Database.OpenConnection();
                    try
                    {
                        context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.WiringReasons3 ON");
                        context.SaveChanges();
                        context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.WiringReasons3 OFF");
                    }
                    finally
                    {
                        context.Database.CloseConnection();
                    }
                }
                foreach (var Reasons4 in XMLReasons4)
                {
                    var reason4id = Reasons4.SelectSingleNode(".//wiringreason4id").InnerText;
                    var reason3id = Reasons4.SelectSingleNode(".//wiringreason3id").InnerText;
                    var description = Reasons4.SelectSingleNode(".//description").InnerText;
                    context.WiringReasons4.Add(new WiringReason4
                    {
                        WiringReason4ID = Int32.Parse(reason4id),
                        WiringReason3ID = Int32.Parse(reason3id),
                        Description = description

                    });
                    context.Database.OpenConnection();
                    try
                    {
                        context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.WiringReasons4 ON");
                        context.SaveChanges();
                        context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.WiringReasons4 OFF");
                    }
                    finally
                    {
                        context.Database.CloseConnection();
                    }
                }

                foreach (var Reasons5 in XMLReasons5)
                {
                    var reason5id = Reasons5.SelectSingleNode(".//wiringreason5id").InnerText;
                    var reason4id = Reasons5.SelectSingleNode(".//wiringreason4id").InnerText;
                    var description = Reasons5.SelectSingleNode(".//description").InnerText;
                    context.WiringReasons5.Add(new WiringReason5
                    {
                        WiringReason5ID = Int32.Parse(reason5id),
                        WiringReason4ID = Int32.Parse(reason4id),
                        Description = description

                    });
                    context.Database.OpenConnection();
                    try
                    {
                        context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.WiringReasons5 ON");
                        context.SaveChanges();
                        context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.WiringReasons5 OFF");
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
            ImportXML(HttpContext.RequestServices);
            return RedirectToAction(nameof(Reason1List));
        }

        public IActionResult SearchR1()
        {
            try
            {
                string term = HttpContext.Request.Query["term"].ToString();
                var names = repository.WiringReasons1.Where(p => p.Description.Contains(term)).Select(p => p.Description).Distinct().ToList();
                return Ok(names);
            }
            catch
            {
                return BadRequest();
            }
        }

        private async Task<bool> GetCurrentUserRole(string role)
        {
            AppUser user = await userManager.GetUserAsync(HttpContext.User);

            bool isInRole = await userManager.IsInRoleAsync(user, role);

            return isInRole;
        }

    }
}