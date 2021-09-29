using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using ProdFloor.Models.ViewModels;
using ProdFloor.Models;
using System.Collections.Generic;
using System;
using System.Xml;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace ProdFloor.Controllers
{
    [Authorize(Roles = "Admin,Engineer,TechAdmin,Technician,Manager,ProductionAdmin,WirerPXP,Kitting")]
    public class AccountController : Controller
    {

        private UserManager<AppUser> userManager;
        private SignInManager<AppUser> signInManager;
        private ITestingRepository testingRepo;
        private IWiringRepository wiringRepo;
        private IJobRepository jobRepo;
        private RoleManager<IdentityRole> roleManager;
        private WiringController wiringController;
        private IHostingEnvironment _env;
        public int PageSize = 7;

        private IUserValidator<AppUser> userValidator;
        private IPasswordValidator<AppUser> passwordValidator;
        private IPasswordHasher<AppUser> passwordHasher;
        string appDataFolder => _env.WebRootPath.ToString() + @"\AppData\";


        public AccountController(UserManager<AppUser> usrMgr,
        IUserValidator<AppUser> userValid,
        IPasswordValidator<AppUser> passValid,
        IPasswordHasher<AppUser> passwordHash,
        RoleManager<IdentityRole> roleMgr,
        SignInManager<AppUser> signInMgr,
        WiringController wiring,
        IHostingEnvironment env,
        ITestingRepository repo,
        IWiringRepository repo2,
        IJobRepository repo3)
        {
            userManager = usrMgr;
            userValidator = userValid;
            passwordValidator = passValid;
            passwordHasher = passwordHash;
            wiringController = wiring;
            testingRepo = repo;
            wiringRepo = repo2;
            jobRepo = repo3;
            roleManager = roleMgr;

            signInManager = signInMgr;
            _env = env;
        }

        [AllowAnonymous]
        public ViewResult Login(string returnUrl)
        {
            return View(new LoginModel
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {

            if (ModelState.IsValid)
            {
                AppUser user =
                await userManager.FindByNameAsync(loginModel.Name);
                if (user != null)
                {
                    await signInManager.SignOutAsync();
                    if ((await signInManager.PasswordSignInAsync(user,
                    loginModel.Password, false, false)).Succeeded)
                    {
                        bool tech = GetCurrentUserRole(user,"Technician").Result;
                        bool wirer = GetCurrentUserRole(user,"Wirer").Result;
                        if (tech) 
                            RestartShiftEnd(user.EngID);

                        if (wirer)
                            WRRestartShiftEnd(user.EngID);

                        TempData["message"] = $"Welcome {user.FullName}!!";
                        return Redirect("/Home/Index");
                    }
                }
            }
            ModelState.AddModelError("", "Invalid name or password");
            return View(loginModel);
        }

        [AllowAnonymous]
        public async Task<RedirectResult> Logout(string returnUrl = "/")
        {
            AppUser user = await userManager.GetUserAsync(HttpContext.User);
            bool tech = GetCurrentUserRole("Technician").Result;
            bool wirer = GetCurrentUserRole("Wirer").Result;
            if (tech) 
                ShiftEnd(user.EngID);

            if (wirer)
                wiringController.ShiftEnd(user.EngID);

            await signInManager.SignOutAsync();
            TempData["message"] = $"You properly logged out.";
            return Redirect("/Account/Login");
        }

        public ViewResult Index(int page = 1)
        {
            List<AppUser> users = new List<AppUser>();
            string roleName = "";
            if (GetCurrentUserRole("EngAdmin").Result
                || GetCurrentUserRole("TechAdmin").Result
                || GetCurrentUserRole("ProductionAdmin").Result)
            {
                if (GetCurrentUserRole("EngAdmin").Result)
                    roleName = "Engineer";
                else if (GetCurrentUserRole("TechAdmin").Result)
                    roleName = "Technician";
                else if (GetCurrentUserRole("ProductionAdmin").Result)
                    roleName = "Wirer";

                foreach (var user in userManager.Users)
                {
                    if (user != null
                    && GetCurrentUserRole(user, roleName).Result)
                    {
                        users.Add(user);
                    }
                }
            }
            else
            {
                users = userManager.Users.ToList();
            }

            UsersListViewModel usersList = new UsersListViewModel
            {
                Users = users.OrderBy(p => p.UserName)
                     .Skip((page - 1) * PageSize)
                     .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = users.ToList().Count()
                }
            };

            return View(usersList);
        }

        public ViewResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CreateModel model)
        {
            if (ModelState.IsValid)
            {
                IEnumerable<AppUser> users = userManager.Users;
                bool engineer = GetCurrentUserRole("EngAdmin").Result;
                bool techAdmin = GetCurrentUserRole("TechAdmin").Result;
                bool productionAdmin = GetCurrentUserRole("ProductionAdmin").Result;
                bool SameID = users.Any(m => m.EngID == model.EngineerID);
                if (SameID == true && !techAdmin & !productionAdmin)
                {
                    TempData["alert"] = $"alert-danger";
                    TempData["message"] = $"That EngID is already in use, please contact to your admin";
                    return View(model);
                }
                IdentityResult result;
                AppUser user = new AppUser
                {
                    UserName = model.Name,
                    Email = model.Email,
                    EngID = model.EngineerID
                };

                if (techAdmin)
                {
                    IEnumerable<AppUser> technicians = userManager.Users;
                    technicians = technicians.Where(m => m.EngID >= 100 && m.EngID <= 299);
                    int MaxEngId = technicians.Select(m => m.EngID).Max();
                    if (MaxEngId == 299)
                    {
                        TempData["alert"] = $"alert-danger";
                        TempData["message"] = $"No EngId availables, please contact to your admin";
                        return View(model);
                    }
                    else MaxEngId++;
                    user.EngID = MaxEngId;
                }else if (productionAdmin)
                {
                    IQueryable<AppUser> auxUsers = userManager.Users.AsQueryable();
                    List<AppUser> wirers = new List<AppUser>();
                    int MaxEngId = 0;

                    foreach (AppUser auxUser in auxUsers)
                    {
                        bool IsInRole = GetCurrentUserRole(auxUser, "Wirer").Result;
                        if (IsInRole && (auxUser.EngID > 299 && auxUser.EngID < 600)) wirers.Add(auxUser);
                    }

                    if(wirers.Count == 0)
                    {
                        MaxEngId = 300;
                    }
                    else
                    {
                        MaxEngId = wirers.Select(m => m.EngID).Max();
                    }
                   
                    if (MaxEngId == 599)
                    {
                        TempData["alert"] = $"alert-danger";
                        TempData["message"] = $"No EngId availables, please contact to your admin";
                        return View(model);
                    }
                    else MaxEngId++;
                    user.EngID = MaxEngId;
                }

                    result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (engineer)
                    {
                        result = await userManager.AddToRoleAsync(user, "Engineer");
                        return RedirectToAction("EngineerAdminDashBoard", "Home");

                    }
                    else if (techAdmin)
                    {
                        result = await userManager.AddToRoleAsync(user, "Technician");
                        return RedirectToAction("Index", "Home");
                    }
                    else if (productionAdmin)
                    {
                        result = await userManager.AddToRoleAsync(user, "Wirer");
                        return RedirectToAction("Index", "Home");
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            bool engAdmin = GetCurrentUserRole("EngAdmin").Result;
            bool techAdmin = GetCurrentUserRole("TechAdmin").Result;
            bool Admin = GetCurrentUserRole("Admin").Result;

            if (!Admin)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"You don't have permissions, contact to your admin";

                return RedirectToAction("Index");
            }

            AppUser user = await userManager.FindByIdAsync(id);
            if (user != null && ( engAdmin || techAdmin || Admin ))
            {
                IdentityResult result = await userManager.DeleteAsync(user);
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
                ModelState.AddModelError("", "User Not Found");
            }
            return View("Index", userManager.Users);
        }

        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        public async Task<IActionResult> Edit(string id)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.ConfirmPassword = "";
                return View(user);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> MakePXP(string id)
        {
            IdentityResult result;
            AppUser user = await userManager.FindByIdAsync(id);

            if (user != null)
            {
                result = await userManager.AddToRoleAsync(user, "WirerPXP");
                if (!result.Succeeded)
                {
                    TempData["alert"] = $"alert-danger";
                    TempData["message"] = $"The user {user.FullName} is already a PXP";

                    AddErrorsFromResult(result);
                }
                else
                {
                    TempData["message"] = $"The user {user.FullName} is now a PXP";
                } 
            }

            
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, string email,
            string password, int EngID)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.EngID = EngID;
                user.Email = email;
                IdentityResult validEmail
                = await userValidator.ValidateAsync(userManager, user);
                if (!validEmail.Succeeded)
                {
                    AddErrorsFromResult(validEmail);
                }
                IdentityResult validPass = null;
                if (!string.IsNullOrEmpty(password))
                {
                    validPass = await passwordValidator.ValidateAsync(userManager,
                    user, password);
                    if (validPass.Succeeded)
                    {
                        user.PasswordHash = passwordHasher.HashPassword(user,
                        password);
                    }
                    else
                    {
                        AddErrorsFromResult(validPass);
                    }
                }
                if ((validEmail.Succeeded && validPass == null)
                || (validEmail.Succeeded
                && password != string.Empty && validPass.Succeeded))
                {
                    IdentityResult result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        TempData["message"] = $"You have changed the password correctly.";

                        bool engAdmin = GetCurrentUserRole("EngAdmin").Result;
                        bool techAdmin = GetCurrentUserRole("TechAdmin").Result;
                        bool Admin = GetCurrentUserRole("Admin").Result;

                        if (engAdmin || techAdmin || Admin)
                            return RedirectToAction("Index");

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        AddErrorsFromResult(result);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
            }
            return View(user);
        }

        public void ShiftEnd(int TechnicianID)
        {
            List<TestJob> testJobList = testingRepo.TestJobs.Where(m => m.TechnicianID == TechnicianID && (m.Status == "Working on it" || m.Status == "Stopped" || m.Status == "Reassignment")).ToList();
            if (testJobList.Count > 0)
            {
                foreach (TestJob testjob in testJobList)
                {
                    List<Stop> stops = new List<Stop>();
                    stops = testingRepo.Stops.Where(p => testjob.TestJobID == p.TestJobID && p.Reason1 != 981 && p.Reason3 == 0 && p.Reason2 == 0).ToList();

                    if (stops.Count > 0)
                    {
                        foreach (Stop stop in stops)
                        {
                            TimeSpan auxTime = (DateTime.Now - stop.StartDate);
                            stop.Elapsed += auxTime;
                            stop.StartDate = DateTime.Now;
                            stop.StopDate = DateTime.Now;
                            testingRepo.SaveStop(stop);
                        }

                    }

                    try

                    {  /**Esto es para el actual step*/
                        var AllStepsForJob = testingRepo.StepsForJobs.Where(m => m.TestJobID == testjob.TestJobID && m.Obsolete == false).OrderBy(m => m.Consecutivo).ToList();
                        StepsForJob actualStepForAUX = AllStepsForJob.FirstOrDefault(m => m.Complete == false);
                        //For actual Step
                        actualStepForAUX.Stop = DateTime.Now;
                        TimeSpan elapsed = actualStepForAUX.Stop - actualStepForAUX.Start;
                        if (actualStepForAUX.Elapsed.Hour == 0 && actualStepForAUX.Elapsed.Minute == 0 && actualStepForAUX.Elapsed.Second == 0)
                        {

                            actualStepForAUX.Elapsed = new DateTime(1, 1, 1, elapsed.Hours, elapsed.Minutes, elapsed.Seconds);
                        }
                        else
                        {
                            int newsecond = 0, newhour = 0, newMinute = 0;

                            newsecond = actualStepForAUX.Elapsed.Second + elapsed.Seconds;
                            newMinute = actualStepForAUX.Elapsed.Minute + elapsed.Minutes;
                            newhour = actualStepForAUX.Elapsed.Hour + elapsed.Hours;
                            if (newsecond >= 60)
                            {
                                newsecond -= 60;
                                newMinute++;
                            }
                            newMinute += elapsed.Minutes;
                            if (newMinute >= 60)
                            {
                                newMinute -= 60;
                                newhour++;
                            }


                            actualStepForAUX.Elapsed = new DateTime(1, 1, 1, newhour, newMinute, newsecond);
                        }
                        testingRepo.SaveStepsForJob(actualStepForAUX);
                        /**Esto es para el actual step*/
                    }
                    catch { }

                   

                    Stop NewtStop = new Stop
                    {
                        TestJobID = testjob.TestJobID,
                        Reason1 = 981,
                        Reason2 = 0,
                        Reason3 = 0,
                        Reason4 = 0,
                        Reason5ID = 0,
                        Description = "Automatic Shift End",
                        Critical = true,
                        StartDate = DateTime.Now,
                        StopDate = DateTime.Now,
                        Elapsed = new DateTime(1, 1, 1, 0, 0, 0),
                        AuxStationID = testjob.StationID,
                        AuxTechnicianID = testjob.TechnicianID,
                    };
                    testingRepo.SaveStop(NewtStop);

                    testjob.Status = "Shift End";
                    testingRepo.SaveTestJob(testjob);
                }
            }

        }

        public void RestartShiftEnd(int TechnicianID)
        {
            List<TestJob> filteredList = new List<TestJob>();
            IQueryable<TestJob> testJobList = testingRepo.TestJobs.Where(m => m.TechnicianID == TechnicianID);
            IQueryable<Stop> stopsShiftEnd = testingRepo.Stops.Where(m => testJobList.Any(n => n.TestJobID == m.TestJobID) && m.Reason1 == 981 && m.Reason2 == 0 && m.Reason3 == 0);

            filteredList = testJobList.Where(m => stopsShiftEnd.Any(n => n.TestJobID == m.TestJobID)).ToList();

            if (filteredList.Count > 0)
            {
                foreach (TestJob testJob in filteredList)
                {
                    List<Stop> stops = new List<Stop>();

                    Stop ShiftEndStop = testingRepo.Stops.LastOrDefault(p => p.TestJobID == testJob.TestJobID && p.Reason1 == 981 && p.Reason2 == 0 && p.Reason3 == 0);

                    Stop ReassignmentStop = testingRepo.Stops.LastOrDefault(p => p.TestJobID == testJob.TestJobID && p.Reason1 == 980 && p.Reason2 == 0 && p.Reason3 == 0);

                    stops = testingRepo.Stops.Where(p => testJob.TestJobID == p.TestJobID && p.Reason1 != 980 && p.Reason1 != 981 && p.Reason2 == 0).ToList();

                    TimeSpan auxTime = (DateTime.Now - ShiftEndStop.StartDate);
                    ShiftEndStop.Elapsed += auxTime;
                    ShiftEndStop.StopDate = DateTime.Now;
                    ShiftEndStop.Reason2 = 981;
                    ShiftEndStop.Reason3 = 981;
                    ShiftEndStop.Reason4 = 981;
                    ShiftEndStop.Reason5ID = 981;
                    testingRepo.SaveStop(ShiftEndStop);


                    var AllStepsForJob = testingRepo.StepsForJobs.Where(m => m.TestJobID == testJob.TestJobID && m.Obsolete == false).OrderBy(m => m.Consecutivo).ToList();
                    StepsForJob CurrentStep = AllStepsForJob.FirstOrDefault(m => m.Complete == false);
                    CurrentStep.Start = DateTime.Now;
                    CurrentStep.Stop = DateTime.Now;
                    testingRepo.SaveStepsForJob(CurrentStep);

                    if (stops.Count > 0)
                    {
                        foreach (Stop stop in stops)
                        {
                            stop.StartDate = DateTime.Now;
                            stop.StopDate = DateTime.Now;
                            testingRepo.SaveStop(stop);
                        }

                    }

                    if (ReassignmentStop != null)
                    {
                        ReassignmentStop.StartDate = DateTime.Now;
                        ReassignmentStop.StopDate = DateTime.Now;
                        testingRepo.SaveStop(ReassignmentStop);
                        testJob.Status = "Reassignment";
                    }
                    else if (stops.Any(m => m.Critical == true))
                    {
                        testJob.Status = "Stopped";
                    }
                    else
                    {
                        testJob.Status = "Working on it";
                    }


                    testingRepo.SaveTestJob(testJob);
                }
            }

        }


        public void WRRestartShiftEnd(int WirerID)
        {
            List<Wiring> filteredList = new List<Wiring>();
            IQueryable<Wiring> wiringList = wiringRepo.Wirings.Where(m => m.WirerID == WirerID);
            IQueryable<WiringStop> stopsShiftEnd = wiringRepo.WiringStops.Where(m => wiringList.Any(n => n.WiringID == m.WiringID) && m.Reason1 == 981 && m.Reason2 == 0 && m.Reason3 == 0);

            filteredList = wiringList.Where(m => stopsShiftEnd.Any(n => n.WiringID == m.WiringID)).ToList();

            if (filteredList.Count > 0)
            {
                foreach (Wiring wiring in filteredList)
                {
                    StatusPO statusPO = jobRepo.StatusPOs.FirstOrDefault(m => m.POID == wiring.POID);
                    PO po = jobRepo.POs.FirstOrDefault(m => m.POID == wiring.POID);
                    List<WiringStop> stops = new List<WiringStop>();

                    try {

                       

                        WiringStop ShiftEndStop = wiringRepo.WiringStops.LastOrDefault(p => p.WiringID == wiring.WiringID && p.Reason1 == 981 && p.Reason5ID == 0);
                        WiringStop ReassignmentStop = wiringRepo.WiringStops.LastOrDefault(p => p.WiringID == wiring.WiringID && p.Reason1 == 980 && p.Reason5ID == 0);
                        stops = wiringRepo.WiringStops.Where(p => wiring.WiringID == p.WiringID && p.Reason1 != 980 && p.Reason1 != 981 && p.Reason5ID == 0).ToList();

                        TimeSpan auxTime = (DateTime.Now - ShiftEndStop.StartDate);
                        ShiftEndStop.Elapsed += auxTime;
                        ShiftEndStop.StopDate = DateTime.Now;
                        ShiftEndStop.Reason2 = 981;
                        ShiftEndStop.Reason3 = 981;
                        ShiftEndStop.Reason4 = 981;
                        ShiftEndStop.Reason5ID = 981;
                        wiringRepo.SaveWiringStop(ShiftEndStop);

                        wiringController.RestarTimeStep(wiring.WiringID);

                        if (stops.Count > 0)
                        {
                            foreach (WiringStop stop in stops)
                            {
                                stop.StartDate = DateTime.Now;
                                stop.StopDate = DateTime.Now;
                                wiringRepo.SaveWiringStop(stop);
                            }

                        }

                        if (ReassignmentStop != null)
                        {
                            ReassignmentStop.StartDate = DateTime.Now;
                            ReassignmentStop.StopDate = DateTime.Now;
                            wiringRepo.SaveWiringStop(ReassignmentStop);

                            statusPO.Status = "WR: Reassignment";
                        }
                        else if (stops.Any(m => m.Critical == true))
                        {
                            statusPO.Status = "WR: Stopped";
                        }
                        else
                        {
                            statusPO.Status = "Wiring on progress";
                        }
                    }
                    catch(Exception e)
                    {
                        TempData["message"] = $"Algo salio mal al tratar de cerrar el Shift end, contacte al admin, Error:{e.Message}, PO: {po.PONumb}";
                        TempData["alert"] = $"alert-danger";
                    }


                }
            }

        }

        private async Task<bool> GetCurrentUserRole(string role)
        {
            AppUser user = await userManager.GetUserAsync(HttpContext.User);

            bool isInRole = await userManager.IsInRoleAsync(user, role);

            return isInRole;
        }

        private async Task<bool> GetCurrentUserRole(AppUser user, string role)
        {

            bool isInRole = await userManager.IsInRoleAsync(user, role);

            return isInRole;
        }

        private async Task<AppUser> GetCurrentUser()
        {
            AppUser user = await userManager.GetUserAsync(HttpContext.User);

            return user;
        }

        [HttpPost]
        public async Task<FileStreamResult> ExportToXML()
        {
            MemoryStream ms = new MemoryStream();
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.OmitXmlDeclaration = true;
            xws.Indent = true;

            List<AppUser> users = userManager.Users.Where(m => m.UserName != "Admin").ToList();

            using (XmlWriter xw = XmlWriter.Create(ms, xws))
            {
                xw.WriteStartDocument();
                xw.WriteStartElement("Users");

                foreach (AppUser user in users)
                {
                    xw.WriteStartElement("User");

                    xw.WriteElementString("UserName", user.UserName);
                    xw.WriteElementString("Email", user.Email);
                    xw.WriteElementString("EngID", user.EngID.ToString());
                    xw.WriteStartElement("Roles");
                    foreach (IdentityRole role in roleManager.Roles)
                    {
                        if (await userManager.IsInRoleAsync(user, role.Name))
                            xw.WriteElementString("RoleName", role.Name);
                    }

                    xw.WriteEndElement();
                    xw.WriteEndElement();
                }

                xw.WriteEndElement();
                xw.WriteEndDocument();
            }
            ms.Position = 0;
            return File(ms, "text/xml", "Users.xml");
        }

        public async Task<IActionResult> ImportXML()
        {
            List<AppUser> users = userManager.Users.ToList();
            List<string> duplicatedUserList = new List<string>();
            List<string> duplicatedUserNameList = new List<string>();
            List<string> duplicatedEngIdList = new List<string>();
            string duplicatedAux = "";
            string password = "Welcome01$";

            XmlDocument doc = new XmlDocument();
            doc.Load(appDataFolder + "Users.xml");

            var XMLobs = doc.DocumentElement.SelectSingleNode("//Users");
            var XMLUsers = XMLobs.SelectNodes(".//User");

            try
            {
                if (XMLobs != null)
                {
                    foreach (XmlElement user in XMLUsers)
                    {
                        List<string> roleNamesList = new List<string>();
                        var name = user.SelectSingleNode(".//UserName").InnerText;
                        var email = user.SelectSingleNode(".//Email").InnerText;
                        var engId = user.SelectSingleNode(".//EngID").InnerText;
                        var XMRoles = user.SelectSingleNode(".//Roles");
                        var XMLRoleNames = XMRoles.SelectNodes(".//RoleName");
                        foreach (XmlElement roleName in XMLRoleNames)
                        {
                            roleNamesList.Add(roleName.InnerText);
                        }

                        bool isUserDuplicated = users.Any(m => m.UserName == name && m.EngID == Int32.Parse(engId));
                        bool isUserNameDuplicated = users.Any(m => m.UserName == name && m.EngID != Int32.Parse(engId));
                        bool isEngDuplicated = users.Any(m => m.EngID == Int32.Parse(engId) && m.UserName != name);

                        if (isUserDuplicated)
                        {
                            duplicatedAux = "UserName: " + name + ", EngID: " + engId;
                            duplicatedUserList.Add(duplicatedAux);

                        }
                        else if (isUserNameDuplicated)
                        {
                            AppUser userAux = users.FirstOrDefault(m => m.UserName == name);
                            duplicatedAux = "UserFromDB: " + userAux.UserName + ", EngID: " + userAux.EngID.ToString();
                            duplicatedAux = duplicatedAux + ", UserFromXML: " + name + ", EngID: " + engId + ".";

                            duplicatedUserNameList.Add(duplicatedAux);
                        }
                        else if (isEngDuplicated)
                        {
                            AppUser userAux = users.FirstOrDefault(m => m.EngID == Int32.Parse(engId));
                            duplicatedAux = "UserFromDB: " + userAux.UserName + ", EngID: " + userAux.EngID.ToString();
                            duplicatedAux = duplicatedAux + ", UserFromXML: " + name + ", EngID: " + engId + ".";

                            duplicatedEngIdList.Add(duplicatedAux);
                        }
                        else
                        {
                            IdentityResult result;
                            AppUser userToImport = new AppUser
                            {
                                UserName = name,
                                Email = email,
                                EngID = Int32.Parse(engId)
                            };

                            result = await userManager.CreateAsync(userToImport, password);
                            if (result.Succeeded)
                            {
                                foreach (string roleName in roleNamesList)
                                {
                                    result = await userManager.AddToRoleAsync(userToImport, roleName);
                                }

                            }
                        }


                    }

                }
            }
            catch
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error while reading xml";
                return RedirectToAction(nameof(Index));
            }

            

            if(duplicatedUserList.Count() > 0 || 
               duplicatedUserNameList.Count() > 0 ||
               duplicatedEngIdList.Count() > 0)
            {
                GenerateImportErrorLog(duplicatedUserList, duplicatedUserNameList, duplicatedEngIdList);
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Errors founded on users import, check log to read more";
                return RedirectToAction(nameof(Index));
            }

            TempData["message"] = $"Users imported succesfully: ";
            return RedirectToAction(nameof(Index));

        }

        public  void GenerateImportErrorLog(List<string> Users, List<string> UserNames, List<string> EngIds)
        {
            string fileName = appDataFolder + "ImportErrorsLog.txt";

            try
            {
                // Check if file already exists. If yes, delete it.     
                if (System.IO.File.Exists(fileName))
                {
                    System.IO.File.Delete(fileName);
                }

                // Create a new file     
                using (StreamWriter sw = System.IO.File.CreateText(fileName))
                {
                    sw.WriteLine("New Import Errors log created: {0}", DateTime.Now.ToString());
                    sw.WriteLine("Duplicated users: ");
                    foreach(string item in Users)
                    {
                        sw.WriteLine(item);
                    }
                    sw.WriteLine("Duplicated UserNames: ");
                    foreach (string item in UserNames)
                    {
                        sw.WriteLine(item);
                    }
                    sw.WriteLine("Duplicated EngIds: ");
                    foreach (string item in EngIds)
                    {
                        sw.WriteLine(item);
                    }
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }
        } 
    }
}