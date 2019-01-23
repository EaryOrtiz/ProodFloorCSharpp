using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using System.Collections.Generic;
using ProdFloor.Models;
using ProdFloor.Models.ViewModels;

namespace ProdFloor.Components
{
    public class EngineerMenuViewComponent : ViewComponent
    {
        private IItemRepository repository;
        private IJobRepository jobrepository;

        public EngineerMenuViewComponent(IItemRepository repo, IJobRepository jobrepo)
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
