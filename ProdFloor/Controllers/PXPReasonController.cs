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
    public class PXPReasonController : Controller
    {
        private IJobRepository jobRepo;
        private IWiringRepository wiringRepo;
        private UserManager<AppUser> userManager;
        private IHostingEnvironment _env;
        public int PageSize = 7;
        string appDataFolder => _env.WebRootPath.ToString() + @"\AppData\";

        public PXPReasonController(IWiringRepository repo,
            IJobRepository repo2,
            UserManager<AppUser> userMgr,
            IHostingEnvironment env)
        {
            jobRepo = repo2;
            wiringRepo = repo;
            userManager = userMgr;
            _env = env;
        }

        //PXPReasons part

        public IActionResult List(string filtrado, string Sort = "default", int page = 1, int totalitemsfromlastsearch = 0)
        {
            if (filtrado != null) Sort = filtrado;

            List<PXPReason> reasons = wiringRepo.PXPReasons
                .Where(m => m.Description != "-")
                .OrderBy(p => p.PXPReasonID).ToList();

            if (Sort != "default")
            {
                reasons = wiringRepo.PXPReasons
                 .Where(m => m.Description.Contains(Sort) && m.Description != "-")
                 .OrderBy(p => p.PXPReasonID).ToList();
            }


            int TotalItemsSearch = reasons.Count();
            if (page == 1)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
            }
            else if (TotalItemsSearch != totalitemsfromlastsearch)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
                page = 1;
            }

            WiringPXPViewModel viewModel = new WiringPXPViewModel
            {
                pXPReasonList = reasons.Skip((page - 1) * PageSize)
                .Take(PageSize).ToList(),
                TotalItems = wiringRepo.PXPReasons.Count(),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    sort = Sort != "default" ? Sort : "default",
                    TotalItemsFromLastSearch = totalitemsfromlastsearch,
                    ItemsPerPage = PageSize,

                    TotalItems = TotalItemsSearch
                }
            };
            return View(viewModel);
        }

        public ViewResult Add() => View("Edit", new PXPReason());

        public ViewResult Edit(int ID) =>
            View(wiringRepo.PXPReasons
                .FirstOrDefault(j => j.PXPReasonID == ID));

        [HttpPost]
        public IActionResult Edit(PXPReason reason)
        {
            if (ModelState.IsValid)
            {
                wiringRepo.SavePXPReason(reason);
                TempData["message"] = $"{reason.Description},, has been saved...";
                return RedirectToAction("List");
            }
            else
            {
                // there is something wrong with the data values
                return View(reason);
            }
        }

        [HttpPost]
        public IActionResult Delete(int ID)
        {
            bool admin = GetCurrentUserRole("Admin").Result;

            if (!admin)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"You don't have permissions, contact to your admin";

                return RedirectToAction("List");
            }

            PXPReason deletedPXPReason = wiringRepo.DeletePXPReason(ID);

            if (deletedPXPReason != null)
            {
                TempData["message"] = $"{deletedPXPReason.Description} was deleted";
            }
            return RedirectToAction("List");
        }

        private async Task<bool> GetCurrentUserRole(string role)
        {
            AppUser user = await userManager.GetUserAsync(HttpContext.User);

            bool isInRole = await userManager.IsInRoleAsync(user, role);

            return isInRole;
        }
    }
}
