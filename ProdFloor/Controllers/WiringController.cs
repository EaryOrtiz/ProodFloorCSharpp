using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProdFloor.Models;

namespace ProdFloor.Controllers
{
    public class WiringController : Controller
    {
        private IJobRepository jobRepo;
        private IWiringRepository wiringRepo;
        private UserManager<AppUser> userManager;
        private IHostingEnvironment _env;
        public int PageSize = 7;
        string appDataFolder => _env.WebRootPath.ToString() + @"\AppData\";

        public WiringController(IWiringRepository repo,
            IJobRepository repo2,
            UserManager<AppUser> userMgr,
            IHostingEnvironment env)
        {
            jobRepo = repo2;
            wiringRepo = repo;
            userManager = userMgr;
            _env = env;
        }



        public IActionResult ProductionAdminDash()
        {
            return View();
        }
    }
}
