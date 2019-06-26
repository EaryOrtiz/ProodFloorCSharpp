using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProdFloor.Controllers;
using ProdFloor.Models;
using ProdFloor.Models.ViewModels;
using Xunit;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ProdFloor.Models.ViewModels.Job;
using ProdFloor.Models.ViewModels.TestJob;
using System.IO;
using System.Security.Principal;
namespace ProdFloor.Tests
{
    public class TestJobOnTestController
    {
        [Fact]
        public void Can_Filter()
        {
            // Arrange
            Mock<IJobRepository> mock = new Mock<IJobRepository>();
            Mock<ITestingRepository> mockTesting = new Mock<ITestingRepository>();
            Mock<IItemRepository> mockItem = new Mock<IItemRepository>();
            List<AppUser> _users = new List<AppUser>
            {
                new AppUser{ EngID = 1 },
                new AppUser{ EngID = 2 }
            };
            TestJob testJob = new TestJob()
            {
                TestJobID = 1,
                TechnicianID = 1,
                JobID = 1,
                SinglePO = 30002110,
            };
            mockItem.Setup(m => m.DoorOperators).Returns((new DoorOperator[]
            {
                new DoorOperator {DoorOperatorID = 7, Style = "Automatic", Name  = "MOD(230V)"},
                new DoorOperator {DoorOperatorID = 8, Style = "Automatic", Name  = "MOD(115V)"},
                new DoorOperator {DoorOperatorID = 3, Style = "Automatic", Name  = "Otra"},
            }).AsQueryable());
            mockItem.Setup(m => m.LandingSystems).Returns((new LandingSystem[]
            {
                new LandingSystem {LandingSystemID = 7, UsedIn = "M2000", Name  = "LS-EDGE"},
                new LandingSystem {LandingSystemID = 8, UsedIn = "M4000", Name  = "LS-EDGE"},
                new LandingSystem {LandingSystemID = 3, UsedIn = "M3000", Name  = "LS-QUTE"},
            }).AsQueryable());
            mockItem.Setup(m => m.Countries).Returns((new Country[]
            {
                new Country {CountryID = 1, Name  = "USA"},
                new Country {CountryID = 2, Name  = "Canada"},
            }).AsQueryable());
            mockItem.Setup(m => m.States).Returns((new State[]
            {
                new State { StateID = 1, CountryID = 1, Name  = "Chicago"},
                new State { StateID = 2, CountryID = 2, Name  = "Ontario"},
            }).AsQueryable());
            mockItem.Setup(m => m.Cities).Returns((new City[]
            {
                new City { StateID = 1, CityID = 1, Name  = "other"},
                new City { StateID = 2, CityID = 2, Name  = "other"},
            }).AsQueryable());
            Mock<UserManager<AppUser>> mockusers = MockUserManager(_users);
            mock.Setup(m => m.Jobs).Returns((new Job[] {
                new Job {JobID = 1, Name = "P1"},
                new Job {JobID = 2, Name = "P2"},
                new Job {JobID = 2, Name = "P3"},
            }).AsQueryable());
            mock.Setup(m => m.JobsExtensions).Returns((new JobExtension[]
            {
                new JobExtension {JobID = 1, JobExtensionID = 1, DoorOperatorID  = 8, JobTypeMain = "Duplex", SHC = false},
                new JobExtension {JobID = 2, JobExtensionID = 2, DoorOperatorID  = 7, JobTypeMain = "Duplex", SHC = false},
                new JobExtension {JobID = 3, JobExtensionID = 3, DoorOperatorID  = 22, JobTypeMain = "Duplex", SHC = false},
            }).AsQueryable());
            mock.Setup(m => m.HydroSpecifics).Returns((new HydroSpecific[]
            {
                new HydroSpecific {JobID = 1, HydroSpecificID = 1, BatteryBrand  = "HAPS", MotorsNum = 1},
                new HydroSpecific {JobID = 2, HydroSpecificID = 2, BatteryBrand  = "Duracell", MotorsNum = 1},
                new HydroSpecific {JobID = 3, HydroSpecificID = 3, BatteryBrand  = "HAPS", MotorsNum = 1},
               
            }).AsQueryable());
            mock.Setup(m => m.HoistWayDatas).Returns((new HoistWayData[]
            {
                new HoistWayData {JobID = 1, HoistWayDataID = 1, LandingSystemID  = 1},
                new HoistWayData {JobID = 2, HoistWayDataID = 2, LandingSystemID  = 2},
                new HoistWayData {JobID = 3, HoistWayDataID = 3, LandingSystemID  = 2},
            }).AsQueryable());
            mock.Setup(m => m.GenericFeaturesList).Returns((new GenericFeatures[]
            {
                new GenericFeatures {JobID = 1, GenericFeaturesID = 1, Monitoring  = "Other"},
                new GenericFeatures {JobID = 2, GenericFeaturesID = 2, Monitoring  = "Other"},
                new GenericFeatures {JobID = 3, GenericFeaturesID = 3, Monitoring  = "Other"},
            }).AsQueryable());
            mockTesting.Setup(m => m.Steps).Returns((new Step[]
            {
                new Step {StepID = 1, Description = "Start", Order = 0},
                new Step {StepID = 2, Description = "MOD" , Order = 1},
                new Step {StepID = 3, Description = "next" , Order = 2},
                new Step {StepID = 4, Description = "HAPS" , Order = 3},
                new Step {StepID = 5, Description = "next" , Order = 4},
                new Step {StepID = 6, Description = "LS-EDGE" , Order = 5},
                new Step {StepID = 7, Description = "End" , Order = 6},
            }).AsQueryable());
            mockTesting.Setup(m => m.TriggeringFeatures).Returns((new TriggeringFeature[]
            {
                new TriggeringFeature {TriggeringFeatureID = 1, StepID = 2, IsSelected = true , Name = "MOD Door Operator"},
                new TriggeringFeature {TriggeringFeatureID = 2, StepID = 4, IsSelected = true , Name = "HAPS Battery"},
                new TriggeringFeature {TriggeringFeatureID = 3, StepID = 6, IsSelected = true , Name = "Edge-LS"},
                new TriggeringFeature {TriggeringFeatureID = 4, StepID = 1, IsSelected = false},
                new TriggeringFeature {TriggeringFeatureID = 5, StepID = 3, IsSelected = false},
                new TriggeringFeature {TriggeringFeatureID = 6, StepID = 5, IsSelected = false},
            }).AsQueryable());
            mockTesting.Setup(m => m.TestJobs).Returns((new TestJob[]{
                testJob
            }).AsQueryable());
            mockTesting.Setup(m => m.TestFeatures).Returns((new TestFeature[]
            {
                new TestFeature {TestFeatureID = 1, TestJobID = 1, Cartop = false, ShortFloor = false, CTL2 = false, Custom = false
                , EMBrake = false , EMCO = false , Group = false , Local = false , MRL = false , Overlay = false , PC = false
                , R6 = false , TrajetaCPI = false , BrakeCoilVoltageMoreThan10 = false }
            }).AsQueryable());


            TestJobController controller = new TestJobController(mockTesting.Object, mock.Object,  mockItem.Object, mockusers.Object);
            controller.PageSize = 3;

            TestJobViewModel viewModel = new TestJobViewModel()
            {
                TestJob = testJob
            };
            // Act 
            Func<ViewResult, TestJobViewModel> GetModel = result => result?.ViewData?.Model as TestJobViewModel;
            StepsForJob stepsForJob = GetModel(controller.NewTestFeatures(viewModel))?.StepsForJob;

            // Assert
            
        }

        private T GetViewModel<T>(IActionResult result) where T : class
        {
            return (result as ViewResult)?.ViewData.Model as T;
        }

        public static Mock<UserManager<TUser>> MockUserManager<TUser>(List<TUser> ls) where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<TUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

            mgr.Setup(x => x.DeleteAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);
            mgr.Setup(x => x.CreateAsync(It.IsAny<TUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Callback<TUser, string>((x, y) => ls.Add(x));
            mgr.Setup(x => x.UpdateAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);

            return mgr;
        }

        static Mock<HttpContext> MockContext()
        {
            Mock request = new Mock<HttpRequest>("", "http://my.url.com", "");

            Mock response = new Mock<HttpResponse>(new StringWriter());

            var context = new Mock<HttpContext>(request, response);
            var principal = new GenericPrincipal(new GenericIdentity("username"), new string[0]);
            context.Setup(m => m.User).Returns(principal);

            return context;
        }
    }
}
