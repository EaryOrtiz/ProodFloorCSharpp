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

        public IActionResult List(WiringPXPViewModel viewModel, int page = 1, int totalitemsfromlastsearch = 0)
        {
            if (viewModel.CleanFields) return RedirectToAction("List");
            IQueryable<PXPReason> reasons = wiringRepo.pXPReasons.AsQueryable();

            if (!string.IsNullOrEmpty(viewModel.pXPReason.Description)) reasons = reasons.Where(m => m.Description.Contains(viewModel.pXPReason.Description));


            viewModel.TotalItems = wiringRepo.pXPReasons.Count();


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
            viewModel.pXPReasonList = reasons.OrderBy(p => p.Description).Skip((page - 1) * 5).Take(5).ToList();
            viewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = page,
                ItemsPerPage = 5,
                TotalItemsFromLastSearch = totalitemsfromlastsearch,
                TotalItems = reasons.Count()
            };
            return View(viewModel);
        }

        public ViewResult Add() => View("Edit", new PXPReason());

        public ViewResult Edit(int ID) =>
            View(wiringRepo.pXPReasons
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
            PXPReason deletedPXPReason = wiringRepo.DeletePXPReason(ID);

            if (deletedPXPReason != null)
            {
                TempData["message"] = $"{deletedPXPReason.Description} was deleted";
            }
            return RedirectToAction("List");
        }
    }
}
