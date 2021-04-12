using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections.Generic;
using ProdFloor.Models;
using ProdFloor.Models.ViewModels;

namespace ProdFloor.Components
{
    public class AdminProdMenuViewComponent : ViewComponent
    {
        private IItemRepository repository;
        private IJobRepository jobrepository;

        public AdminProdMenuViewComponent(IItemRepository repo, IJobRepository jobrepo)
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

            foreach (string brand in repository.DoorOperators
                .Select(x => x.Brand)
                .Distinct()
                .OrderBy(x => x))
            {
                dooroperators.Add(brand);
            }

            foreach (string state in repository.States
                .Select(x => x.Name)
                .Distinct()
                .OrderBy(x => x))
            {
                states.Add(state);
            }

            foreach (string jobType in repository.JobTypes
                .Select(x => x.Name)
                .Distinct())
            {
                jobTypes.Add(jobType);
            }

            foreach (string country in repository.Countries
                .Select(x => x.Name)
                .Distinct()
                .OrderBy(x => x))
            {
                countries.Add(country);
            }

            foreach (string country in countries)
            {//Se debe arreglar con jions!!!!!!!!
                foreach (string state in repository.States
                    .Where(x => x.Name == country)
                    .Select(x => x.Name)
                    .OrderBy(x => x))
                {
                    if (cities.ContainsKey(country))
                    {
                        cities[country].Add(state);
                    }
                    else
                    {
                        cities.Add(country, new List<string> { state });
                    }
                }
            }

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
