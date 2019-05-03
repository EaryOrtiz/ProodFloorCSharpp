using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProdFloor.Models;
using ProdFloor.Models.ViewModels;
using ProdFloor.Models.ViewModels.Reasons;

namespace ProdFloor.Controllers
{
    [Authorize(Roles = "Admin,Technician")]
    public class ReasonsController : Controller
    {
        private ITestingRepository repository;
        public int PageSize = 7;

        public ReasonsController(ITestingRepository testingrepo)
        {
            repository = testingrepo;
        }
        public ViewResult List(int reason, int separator, int page = 1)
        {

            switch (reason)
            {
                case 1:
                    var ReasonCount = repository.Reasons1.Count();

                    return View("Reason1List", new ReasonViewModel
                    {
                        Reasons1 = repository.Reasons1
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

        public ViewResult Edit(string btn,int ID)
        {
            switch (btn)
            {
                case "R1": return View("Reason1Edit",repository.Reasons1.FirstOrDefault(j => j.Reason1ID == ID));
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
                return RedirectToAction("List", new { Page = 1, Reason = 1 });
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
                return RedirectToAction("List", new { Page = 1, Reason = 2 });
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
                return RedirectToAction("List", new { Page = 1, Reason = 3 });
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
                return RedirectToAction("List", new { Page = 1, Reason = 4 });
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
                return RedirectToAction("List", new { Page = 1, Reason = 5 });
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
                    return RedirectToAction("List", new { Page = 1, Reason = 1 });

                case "R2":
                    Reason2 deleteReason2 = repository.DeleteReason2(ID);

                    if (deleteReason2 != null)
                    {
                        TempData["message"] = $"{deleteReason2.Reason2ID} was deleted";
                    }
                    return RedirectToAction("List", new { Page = 1, Reason = 2 });
                case "R3":
                    Reason3 deleteReason3 = repository.DeleteReason3(ID);

                    if (deleteReason3 != null)
                    {
                        TempData["message"] = $"{deleteReason3.Reason3ID} was deleted";
                    }
                    return RedirectToAction("List", new { Page = 1, Reason = 3 });
                case "R4":
                    Reason4 deleteReason4 = repository.DeleteReason4(ID);

                    if (deleteReason4 != null)
                    {
                        TempData["message"] = $"{deleteReason4.Reason4ID} was deleted";
                    }
                    return RedirectToAction("List", new { Page = 1, Reason = 4 });
                case "R5":
                    Reason5 deleteReason5 = repository.DeleteReason5(ID);

                    if (deleteReason5 != null)
                    {
                        TempData["message"] = $"{deleteReason5.Reason5ID} was deleted";
                    }
                    return RedirectToAction("List", new { Page = 1, Reason = 5 });
            }

            return NotFound();
        }

        public JsonResult GetReason2(int Reason1ID)
        {
            List<Reason2> Reasonlist = new List<Reason2>();
            Reasonlist = (from reason2 in repository.Reasons2 where reason2.Reason1ID == Reason1ID select reason2).ToList();
            return Json(new SelectList(Reasonlist, "Reason2ID", "Description"));
        }

        public JsonResult GetReason3(int Reason2ID)
        {
            List<Reason3> Reasonlist = new List<Reason3>();
            Reasonlist = (from reason3 in repository.Reasons3 where reason3.Reason2ID == Reason2ID select reason3).ToList();
            return Json(new SelectList(Reasonlist, "Reason3ID", "Description"));
        }

        public JsonResult GetReason4(int Reason3ID)
        {
            List<Reason4> Reasonlist = new List<Reason4>();
            Reasonlist = (from reason4 in repository.Reasons4 where reason4.Reason3ID == Reason3ID select reason4).ToList();
            return Json(new SelectList(Reasonlist, "Reason4ID", "Description"));
        }

        public JsonResult GetReason5(int Reason4ID)
        {
            List<Reason5> Reasonlist = new List<Reason5>();
            Reasonlist = (from reason5 in repository.Reasons5 where reason5.Reason4ID == Reason4ID select reason5).ToList();
            return Json(new SelectList(Reasonlist, "Reason5ID", "Description"));
        }

    }
}