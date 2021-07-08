using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProdFloor.Models;
using ProdFloor.Models.ViewModels;
using ProdFloor.Models.ViewModels.Wiring;

namespace ProdFloor.Controllers
{
    public class WiringOptionsController : Controller
    {
        private IWiringRepository wiringRepo;
        private UserManager<AppUser> userManager;
        private IHostingEnvironment _env;
        public int PageSize = 7;
        string appDataFolder => _env.WebRootPath.ToString() + @"\AppData\";

        public WiringOptionsController(IWiringRepository repo,
            UserManager<AppUser> userMgr,
            IHostingEnvironment env)
        {
            wiringRepo = repo;
            userManager = userMgr;
            _env = env;
        }

        public IActionResult List(WiringOptionsViewModel viewModel, int page = 1, int totalitemsfromlastsearch = 0)
        {
            if (viewModel.CleanFields) return RedirectToAction("List");
            IQueryable<WiringOption> wiringOptions = wiringRepo.WiringOptions.AsQueryable();

            if (viewModel.isDeleted) wiringOptions = wiringOptions.Where(m => m.isDeleted == viewModel.isDeleted);
            if (!string.IsNullOrEmpty(viewModel.Description)) wiringOptions = wiringOptions.Where(m => m.Description.Contains(viewModel.Description));

            viewModel.TotalItems = wiringRepo.WiringOptions.Count();

            int TotalItemsSearch = wiringOptions.Count();
            if (page == 1)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
            }
            else if (TotalItemsSearch != totalitemsfromlastsearch)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
                page = 1;
            }
            viewModel.WiringOptions = new List<WiringOption>();
            viewModel.WiringOptions = wiringOptions.OrderBy(p => p.Description).Skip((page - 1) * 5).Take(5).ToList();
            viewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = page,
                ItemsPerPage = 5,
                TotalItemsFromLastSearch = totalitemsfromlastsearch,
                TotalItems = wiringOptions.Count()
            };
            return View(viewModel);
        }

        public ViewResult Add()
        {
            return View("Edit", new WiringOption());
        }


        public ViewResult Edit(int ID) =>
            View(wiringRepo.WiringOptions
                .FirstOrDefault(j => j.WiringOptionID == ID));

        [HttpPost]
        public IActionResult Edit(WiringOption option)
        {
            if (ModelState.IsValid)
            {
                wiringRepo.SaveWiringOption(option);
                TempData["message"] = $"{option.Description} has been saved";
                return RedirectToAction("List");
            }
            else
            {
                // there is something wrong with the data values
                return View(option);
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

            WiringOption deletedOption = wiringRepo.DeleteWiringOption(ID);

            if (deletedOption != null)
            {
                TempData["message"] = $"{deletedOption.Description} was deleted";
            }
            return RedirectToAction("List");
        }

        [HttpPost]
        public IActionResult FakeDelete(int ID)
        {
            bool admin = GetCurrentUserRole("Admin").Result;
            bool prodAdmin = GetCurrentUserRole("ProductionAdmin").Result;

            if (!prodAdmin && !admin)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"You don't have permissions, contact to your admin";

                return RedirectToAction("List");
            }

            WiringOption option = wiringRepo.WiringOptions.FirstOrDefault(m => m.WiringOptionID == ID);
            option.isDeleted = true;
            wiringRepo.SaveWiringOption(option);
            
            TempData["message"] = $"{option.Description} was deleted";
            return RedirectToAction("List");
        }

        [HttpPost]
        public FileStreamResult ExportToXML()
        {
            MemoryStream ms = new MemoryStream();
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.OmitXmlDeclaration = true;
            xws.Indent = true;

            List<WiringOption> options = wiringRepo.WiringOptions.ToList();

            using (XmlWriter xw = XmlWriter.Create(ms, xws))
            {
                xw.WriteStartDocument();

                xw.WriteStartElement("WiringOptions");
                foreach (WiringOption option in options)
                {
                    xw.WriteStartElement("WiringOption");
                    xw.WriteElementString("WiringOptionID", option.WiringOptionID.ToString());
                    xw.WriteElementString("Description", option.Description);
                    xw.WriteElementString("isDeleted", option.isDeleted.ToString());
                    xw.WriteEndElement();
                }
                xw.WriteEndElement();
                xw.WriteEndDocument();
            }

            ms.Position = 0;
            return File(ms, "text/xml", "WiringOptions.xml");
        }

        public void ImportXML(IServiceProvider services)
        {
            ApplicationDbContext context = services.GetRequiredService<ApplicationDbContext>();


            XmlDocument doc = new XmlDocument();
            doc.Load(appDataFolder + "WiringOptions.xml");

            var ALLXMLobs = doc.DocumentElement.SelectSingleNode("//WiringOptions");
            var XMLobs = ALLXMLobs.SelectNodes("//WiringOption");

            foreach (XmlElement XMLob in XMLobs)
            {
                var wiringOptionID = XMLob.SelectSingleNode(".//WiringOptionID").InnerText;
                var description = XMLob.SelectSingleNode(".//Description").InnerText;
                var isdeleted = XMLob.SelectSingleNode(".//isDeleted").InnerText;

                context.WiringOptions.Add(new WiringOption
                {
                    WiringOptionID = Int32.Parse(wiringOptionID),
                    Description = description,
                    isDeleted = Boolean.Parse(isdeleted),
                });
                context.Database.OpenConnection();
                try
                {
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.WiringOptions ON");
                    context.SaveChanges();
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.WiringOptions OFF");
                }
                finally
                {
                    context.Database.CloseConnection();
                }
            }

        }

        [HttpPost]
        public IActionResult SeedXML()
        {
            ImportXML(HttpContext.RequestServices);
            return RedirectToAction(nameof(List));
        }

        private async Task<bool> GetCurrentUserRole(string role)
        {
            AppUser user = await userManager.GetUserAsync(HttpContext.User);

            bool isInRole = await userManager.IsInRoleAsync(user, role);

            return isInRole;
        }

        private async Task<AppUser> GetCurrentUser()
        {
            AppUser user = await userManager.GetUserAsync(HttpContext.User);

            return user;
        }
    }
}
