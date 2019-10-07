﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using ProdFloor.Models.ViewModels;
using ProdFloor.Models;
using System.Collections.Generic;
using System;

namespace ProdFloor.Controllers
{
    [Authorize(Roles = "Admin,Engineer,TechAdmin,Technician")]
    public class AccountController : Controller
    {

        private UserManager<AppUser> userManager;
        private SignInManager<AppUser> signInManager;
        private ITestingRepository testingRepo;
        public int PageSize = 7;

        private IUserValidator<AppUser> userValidator;

        private IPasswordValidator<AppUser> passwordValidator;

        private IPasswordHasher<AppUser> passwordHasher;


        public AccountController(UserManager<AppUser> usrMgr,
        IUserValidator<AppUser> userValid,
        IPasswordValidator<AppUser> passValid,
        IPasswordHasher<AppUser> passwordHash,
        SignInManager<AppUser> signInMgr,
        ITestingRepository repo)
        {
            userManager = usrMgr;
            userValidator = userValid;
            passwordValidator = passValid;
            passwordHasher = passwordHash;
            testingRepo = repo;

            signInManager = signInMgr;
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
                        if (user.EngID >= 100 && user.EngID < 300) RestartShiftEnd(user.EngID);

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
            if (tech) ShiftEnd(user.EngID);


            await signInManager.SignOutAsync();
            TempData["message"] = $"You properly logged out.";
            return Redirect("/Account/Login");
        }

        public ViewResult Index(int page = 1) => View(new UsersListViewModel
        {
            Users = userManager.Users.OrderBy(p => p.UserName)
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize),
            PagingInfo = new PagingInfo
            {
                CurrentPage = page,
                ItemsPerPage = PageSize,
                TotalItems = userManager.Users.ToList().Count()
            }
        });

        public ViewResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CreateModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser
                {
                    UserName = model.Name,
                    Email = model.Email,
                    EngID = model.EngineerID
                };
                IdentityResult result
                = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
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
            AppUser user = await userManager.FindByIdAsync(id);
            if (user != null)
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
                        bool engineer = GetCurrentUserRole("Engineer").Result;
                        if (engineer) return RedirectToAction("Index", "Home");
                        return RedirectToAction("Index");
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
            List<TestJob> testJobList = testingRepo.TestJobs.Where(m => m.TechnicianID == TechnicianID && (m.Status != "Completed" && m.Status != "Incomplete")).ToList();
            if (testJobList.Count > 0)
            {
                foreach (TestJob testjob in testJobList)
                {
                    if (testjob.Status != "Stopped")
                    {
                        testjob.Status = "Shift End";
                        testingRepo.SaveTestJob(testjob);

                        Stop NewtStop = new Stop
                        {
                            TestJobID = testjob.TestJobID,
                            Reason1 = 981,
                            Reason2 = 981,
                            Reason3 = 981,
                            Reason4 = 981,
                            Reason5ID = 981,
                            Description = "Automatic Shift End",
                            Critical = true,
                            StartDate = DateTime.Now,
                            StopDate = DateTime.Now,
                            Elapsed = new DateTime(1, 1, 1, 0, 0, 0),
                            AuxStationID = testjob.StationID,
                            AuxTechnicianID = testjob.TechnicianID,
                        };
                        testingRepo.SaveStop(NewtStop);

                    }
                    else
                    {

                        Stop CurrentStop = testingRepo.Stops.FirstOrDefault(p => p.StopID == testingRepo.Stops.Max(x => x.StopID) && p.Critical == true && p.Reason1 != 980);
                        TimeSpan auxTime = (DateTime.Now - CurrentStop.StartDate);
                        CurrentStop.Elapsed += auxTime;
                        CurrentStop.StopDate = DateTime.Now;
                        testingRepo.SaveStop(CurrentStop);

                        testjob.Status = "Shift End";
                        testingRepo.SaveTestJob(testjob);

                        Stop NewtStop = new Stop
                        {
                            TestJobID = testjob.TestJobID,
                            Reason1 = 981,
                            Reason2 = 981,
                            Reason3 = 981,
                            Reason4 = 981,
                            Reason5ID = 981,
                            Description = "Automatic Shift End",
                            Critical = true,
                            StartDate = DateTime.Now,
                            StopDate = DateTime.Now,
                            Elapsed = new DateTime(1, 1, 1, 0, 0, 0),
                            AuxStationID = testjob.StationID,
                            AuxTechnicianID = testjob.TechnicianID,
                        };
                        testingRepo.SaveStop(NewtStop);
                    }
                }
            }

        }

        public void RestartShiftEnd(int TechnicianID)
        {
            List<TestJob> testJobList = testingRepo.TestJobs.Where(m => m.TechnicianID == TechnicianID && m.Status == "Shift End").ToList();
            if (testJobList.Count > 0)
            {
                foreach (TestJob testJob in testJobList)
                {
                    Stop ShiftEndStop = testingRepo.Stops.FirstOrDefault(p => p.TestJobID == testJob.TestJobID && p.StopID == testingRepo.Stops.Max(x => x.StopID) && p.Reason1 == 981);
                    Stop ReassignmentStop = testingRepo.Stops.FirstOrDefault(p => p.TestJobID == testJob.TestJobID && p.Reason1 == 980 && p.StopID == testingRepo.Stops.Max(x => x.StopID));
                    Stop PreviusStop = testingRepo.Stops.FirstOrDefault(p => p.TestJobID == testJob.TestJobID && p.Reason2 == 0 && p.Critical == true);
                    if (ReassignmentStop != null)
                    {
                        TimeSpan auxTime = (DateTime.Now - ShiftEndStop.StartDate);
                        ShiftEndStop.Elapsed += auxTime;
                        ShiftEndStop.StopDate = DateTime.Now;
                        testingRepo.SaveStop(ShiftEndStop);

                        testJob.Status = "Reassignment";
                        testingRepo.SaveTestJob(testJob);
                    }
                    else if (PreviusStop != null)
                    {
                        TimeSpan auxTime = (DateTime.Now - ShiftEndStop.StartDate);
                        ShiftEndStop.Elapsed += auxTime;
                        ShiftEndStop.StopDate = DateTime.Now;
                        testingRepo.SaveStop(ShiftEndStop);

                        testJob.Status = "Stopped";
                        testingRepo.SaveTestJob(testJob);
                    }
                    else
                    {
                        TimeSpan auxTime = (DateTime.Now - ShiftEndStop.StartDate);
                        ShiftEndStop.Elapsed += auxTime;
                        ShiftEndStop.StopDate = DateTime.Now;
                        testingRepo.SaveStop(ShiftEndStop);

                        testJob.Status = "Working on it";
                        testingRepo.SaveTestJob(testJob);
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
    }
}