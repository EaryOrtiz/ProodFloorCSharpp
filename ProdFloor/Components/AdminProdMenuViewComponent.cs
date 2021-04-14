using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections.Generic;
using ProdFloor.Models;
using ProdFloor.Models.ViewModels;

namespace ProdFloor.Components
{
    public class AdminProdMenuViewComponent : ViewComponent
    {
        private IWiringRepository repository;
        private IJobRepository jobrepository;

        public AdminProdMenuViewComponent(IWiringRepository repo, IJobRepository jobrepo)
        {
            repository = repo;
            jobrepository = jobrepo;
        }

        public IViewComponentResult Invoke()
        {
            ViewBag.Selected = RouteData?.Values["selected"];

            List<string> jobTypes = new List<string>();
            List<string> countries = new List<string>();
            List<string> states = new List<string>();
            List<string> dooroperators = new List<string>();
            Dictionary<string, List<string>> cities = new Dictionary<string, List<string>>();



            //new AdminNavMenuViewModel { }
            return View(new AdminNavMenuViewModel
            {
                Jobtypes = jobTypes,
                Cities = cities,
                States = countries,
                DoorOperators = dooroperators
            });
        }
    }
}
