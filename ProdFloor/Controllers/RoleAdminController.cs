using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using ProdFloor.Models;
using ProdFloor.Models.ViewModels;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using System.Xml;
using System.Linq;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;

namespace ProdFloor.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RoleAdminController : Controller
    {
        private RoleManager<IdentityRole> roleManager;
        private UserManager<AppUser> userManager;
        private IHostingEnvironment _env;
        string appDataFolder => _env.WebRootPath.ToString() + @"\AppData\";

        public RoleAdminController(RoleManager<IdentityRole> roleMgr, 
            UserManager<AppUser> userMrg,
            IHostingEnvironment env)
        {
            roleManager = roleMgr;
            userManager = userMrg;
            _env = env;
        }

        public ViewResult Index() => View(roleManager.Roles);

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create([Required]string name)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result
                = await roleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    AddErrorsFromResult(result);
                }
            }
            return View(name);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            IdentityRole role = await roleManager.FindByIdAsync(id);
            if (role != null)
            {
                IdentityResult result = await roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    AddErrorsFromResult(result);
                }
            }
            else
            {
                ModelState.AddModelError("", "No role found");
            }
            return View("Index", roleManager.Roles);
        }

        public async Task<IActionResult> Edit(string id)
        {
            IdentityRole role = await roleManager.FindByIdAsync(id);
            List<AppUser> members = new List<AppUser>();
            List<AppUser> nonMembers = new List<AppUser>();
            foreach (AppUser user in userManager.Users)
            {
                var list = await userManager.IsInRoleAsync(user, role.Name)
                ? members : nonMembers;
                list.Add(user);
            }
            return View(new RoleEditModel
            {
                Role = role,
                Members = members,
                NonMembers = nonMembers
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RoleModificationModel model)
        {
            IdentityResult result;
            if (ModelState.IsValid)
            {
                foreach (string userId in model.IdsToAdd ?? new string[] { })
                {
                    AppUser user = await userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        result = await userManager.AddToRoleAsync(user,
                        model.RoleName);
                        if (!result.Succeeded)
                        {
                            AddErrorsFromResult(result);
                        }
                    }
                }
                foreach (string userId in model.IdsToDelete ?? new string[] { })
                {
                    AppUser user = await userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        result = await userManager.RemoveFromRoleAsync(user,
                        model.RoleName);
                        if (!result.Succeeded)
                        {
                            AddErrorsFromResult(result);
                        }
                    }
                }
            }
            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return await Edit(model.RoleId);
            }
        }

        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        [HttpPost]
        public FileStreamResult ExportToXML()
        {
            MemoryStream ms = new MemoryStream();
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.OmitXmlDeclaration = true;
            xws.Indent = true;

            IQueryable<IdentityRole> roles = roleManager.Roles;

            using (XmlWriter xw = XmlWriter.Create(ms, xws))
            {
                xw.WriteStartDocument();
                xw.WriteStartElement("Roles");

                foreach (IdentityRole role in roles)
                {
                    xw.WriteStartElement("Role");
                    xw.WriteElementString("Name", role.Name);
                    xw.WriteEndElement();
                }

                xw.WriteEndElement();
                xw.WriteEndDocument();
            }
            ms.Position = 0;
            return File(ms, "text/xml", "Roles.xml");
        }

        public async Task<IActionResult> ImportXML()
        {
            List<string> duplicatedRolesList = new List<string>();
            string duplicatedRoles = "";

            XmlDocument doc = new XmlDocument();
            doc.Load(appDataFolder + "Roles.xml");

            var XMLobs = doc.DocumentElement.SelectSingleNode("//Roles");
            var XMLRoles = XMLobs.SelectNodes(".//Role");

            if (XMLobs != null)
            {
                foreach (XmlElement role in XMLRoles)
                {
                    var name = role.SelectSingleNode(".//Name").InnerText;

                    IdentityResult result = await roleManager.CreateAsync(new IdentityRole(name));
                    if (!result.Succeeded)
                    {
                        duplicatedRolesList.Add(name);
                    }

                }

            }

            if(duplicatedRolesList.Count() > 0)
            {
                foreach(string role in duplicatedRolesList)
                {
                    duplicatedRoles = duplicatedRoles + ", " + role;
                }

                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"The following roles are duplicated: " + duplicatedRoles;
            }
            else
            {
                TempData["message"] = $"Roles imported succesfully";
            }

            return RedirectToAction(nameof(Index));

        }
    }
}
