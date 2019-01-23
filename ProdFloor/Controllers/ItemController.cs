using Microsoft.AspNetCore.Mvc;
using ProdFloor.Models;
using ProdFloor.Models.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;
using System.Collections.Generic;

namespace ProdFloor.Controllers
{
    public class ItemController : Controller
    {
        private IItemRepository repository;

        public int PageSize = 4;

        public ItemController(IItemRepository repo)
        {
            repository = repo;
        }

        public ViewResult Index() => View();
        
    }
}
