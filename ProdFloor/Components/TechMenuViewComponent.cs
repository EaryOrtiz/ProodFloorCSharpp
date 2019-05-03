using Microsoft.AspNetCore.Mvc;
using ProdFloor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Components
{
    public class TechMenuViewComponent : ViewComponent
    {
        private IItemRepository repository;
        private IJobRepository jobrepository;

        public TechMenuViewComponent(IItemRepository repo, IJobRepository jobrepo)
        {
            repository = repo;
            jobrepository = jobrepo;
        }

        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
