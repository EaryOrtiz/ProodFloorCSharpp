using Microsoft.AspNetCore.Mvc;
using ProdFloor.Models;
using ProdFloor.Models.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace ProdFloor.Controllers
{
    [Authorize(Roles = "Admin")]
    public class JobTypeController : Controller
    {
        private IItemRepository repository;
        private ItemController itemController;

        public int PageSize = 10;

        public JobTypeController(IItemRepository repo,
            ItemController item)
        {
            repository = repo;
            itemController = item;
        }
        public ViewResult List(int page = 1)
            => View(new JobTypesListViewModel
            {
                JobTypes = repository.JobTypes
                .OrderBy(p => p.JobTypeID)
                .Skip((page - 1) * PageSize)
                .Take(PageSize).ToList(),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = repository.JobTypes.Count()
                }
            });

        public ViewResult Edit(int ID) =>
            View(repository.JobTypes
                .FirstOrDefault(j => j.JobTypeID == ID));

        [HttpPost]
        public IActionResult Edit(JobType jobType)
        {
            if (ModelState.IsValid)
            {
                repository.SaveJobType(jobType);
                TempData["message"] = $"{jobType.Name} has been saved...{jobType.JobTypeID}";
                return RedirectToAction("List");
            }
            else
            {
                // there is something wrong with the data values
                return View(jobType);
            }
        }

        [HttpPost]
        public IActionResult Delete(int ID)
        {
            JobType deletedJobType = repository.DeleteJobType(ID);

            if (deletedJobType != null)
            {
                TempData["message"] = $"{deletedJobType.Name} was deleted";
            }
            return RedirectToAction("List");
        }

        [HttpPost]
        public IActionResult SeedXML(string buttonImportXML)
        {
            string resp = buttonImportXML;
            itemController.ImportXML(HttpContext.RequestServices, resp);
            return RedirectToAction(nameof(List));
        }

        public ViewResult Add() => View("Edit", new JobType());
    }
}
