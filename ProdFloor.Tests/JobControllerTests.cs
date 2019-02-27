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

namespace ProdFloor.Tests
{
    public class JobControllerTests
    {
        [Fact]
        public void Can_Paginate()
        {
            // Arrange
            Mock<IJobRepository> mock = new Mock<IJobRepository>();
            Mock<IItemRepository> mockitems = new Mock<IItemRepository>();
            List<AppUser> _users = new List<AppUser>
            {
                new AppUser{ EngID = 1 },
                new AppUser{ EngID = 2 }
            };
            Mock<UserManager<AppUser>> mockusers = MockUserManager<AppUser>(_users);
            mock.Setup(m => m.Jobs).Returns((new Job[] {
                new Job {JobID = 1, Name = "P1"},
                new Job {JobID = 2, Name = "P2"},
                new Job {JobID = 3, Name = "P3"},
                new Job {JobID = 4, Name = "P4"},
                new Job {JobID = 5, Name = "P5"}
            }).AsQueryable<Job>());

            JobController controller = new JobController(mock.Object, mockitems.Object, mockusers.Object);
            controller.PageSize = 3;

            // Act
            JobsListViewModel result =
            controller.List(1, 2).ViewData.Model as JobsListViewModel;

            // Assert
            Job[] prodArray = result.Jobs.ToArray();
            Assert.True(prodArray.Length == 2);
            Assert.Equal("P4", prodArray[0].Name);
            Assert.Equal("P5", prodArray[1].Name);
        }

        [Fact]
        public void Can_Send_Pagination_View_Model()
        {
            // Arrange
            Mock<IJobRepository> mock = new Mock<IJobRepository>();
            Mock<IItemRepository> mockitems = new Mock<IItemRepository>();
            List<AppUser> _users = new List<AppUser>
            {
                new AppUser{ EngID = 1 },
                new AppUser{ EngID = 2 }
            };
            Mock<UserManager<AppUser>> mockusers = MockUserManager<AppUser>(_users);
            mock.Setup(m => m.Jobs).Returns((new Job[] {
                new Job {JobID = 1, Name = "P1"},
                new Job {JobID = 2, Name = "P2"},
                new Job {JobID = 3, Name = "P3"},
                new Job {JobID = 4, Name = "P4"},
                new Job {JobID = 5, Name = "P5"}
            }).AsQueryable<Job>());

            // Arrange
            JobController controller =
            new JobController(mock.Object, mockitems.Object, mockusers.Object) { PageSize = 3 };

            // Act
            JobsListViewModel result =
            controller.List(1,2).ViewData.Model as JobsListViewModel;

            // Assert
            PagingInfo pageInfo = result.PagingInfo;
            Assert.Equal(2, pageInfo.CurrentPage);
            Assert.Equal(3, pageInfo.ItemsPerPage);
            Assert.Equal(5, pageInfo.TotalItems);
            Assert.Equal(2, pageInfo.TotalPages);
        }

        [Fact]
        public void CanNot_Filter_Products()
        {
            // Arrange
            // - create the mock repository
            Mock<IJobRepository> mock = new Mock<IJobRepository>();
            Mock<IItemRepository> mockitems = new Mock<IItemRepository>();
            List<AppUser> _users = new List<AppUser>
            {
                new AppUser{ EngID = 1 },
                new AppUser{ EngID = 2 }
            };
            Mock<UserManager<AppUser>> mockusers = MockUserManager<AppUser>(_users);
            mock.Setup(m => m.Jobs).Returns((new Job[] {
                new Job {JobID = 1, Name = "P1", JobTypeID = 1},
                new Job {JobID = 2, Name = "P2", JobTypeID = 2},
                new Job {JobID = 3, Name = "P3", JobTypeID = 3},
                new Job {JobID = 4, Name = "P4", JobTypeID = 4},
                new Job {JobID = 5, Name = "P5", JobTypeID = 5}
            }).AsQueryable<Job>());

            // Arrange - create a controller and make the page size 3 items
            JobController controller = new JobController(mock.Object, mockitems.Object, mockusers.Object);
            controller.PageSize = 3;

            // Action
            Job[] result =
            (controller.List(2, 1).ViewData.Model as JobsListViewModel)
            .Jobs.ToArray();

            // Assert
            Assert.Equal(3, result.Length);
            Assert.False(result[0].Name == "P2" && result[0].JobTypeID == 2);
            Assert.False(result[1].Name == "P4" && result[1].JobTypeID == 2);
        }

        [Fact]
        public void CanNot_Generate_Category_Specific_Product_Count()
        {
            // Arrange
            Mock<IJobRepository> mock = new Mock<IJobRepository>();
            Mock<IItemRepository> mockitems = new Mock<IItemRepository>();
            List<AppUser> _users = new List<AppUser>
            {
                new AppUser{ EngID = 1 },
                new AppUser{ EngID = 2 }
            };
            Mock<UserManager<AppUser>> mockusers = MockUserManager<AppUser>(_users);
            mock.Setup(m => m.Jobs).Returns((new Job[] {
                new Job {JobID = 1, Name = "P1", JobTypeID = 1},
                new Job {JobID = 2, Name = "P2", JobTypeID = 2},
                new Job {JobID = 3, Name = "P3", JobTypeID = 3},
                new Job {JobID = 4, Name = "P4", JobTypeID = 4},
                new Job {JobID = 5, Name = "P5", JobTypeID = 5}
            }).AsQueryable<Job>());

            JobController target = new JobController(mock.Object, mockitems.Object, mockusers.Object);
            target.PageSize = 3;

            Func<ViewResult, JobsListViewModel> GetModel = result =>
            result?.ViewData?.Model as JobsListViewModel;

            // Action
            int? res1 = GetModel(target.List(1,1))?.PagingInfo.TotalItems;
            int? res2 = GetModel(target.List(2,2))?.PagingInfo.TotalItems;
            int? res3 = GetModel(target.List(3,3))?.PagingInfo.TotalItems;
            int? resAll = GetModel(target.List(5,5))?.PagingInfo.TotalItems;

            // Assert
            Assert.NotEqual(2, res1);
            Assert.NotEqual(2, res2);
            Assert.NotEqual(1, res3);
            Assert.Equal(5, resAll);
        }

        [Fact]
        public void Can_Save_Valid_Changes()
        {
            // Arrange - create mock repository
            Mock<IJobRepository> mock = new Mock<IJobRepository>();
            Mock<IItemRepository> mockitems = new Mock<IItemRepository>();
            
            List<AppUser> _users = new List<AppUser>
            {
                new AppUser{ EngID = 1 },
                new AppUser{ EngID = 2 }
            };
            Mock<UserManager<AppUser>> mockusers = MockUserManager<AppUser>(_users);

            // Arrange - create mock temp data
            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();

            // Arrange - create the controller
            JobController target = new JobController(mock.Object, mockitems.Object, mockusers.Object)
            {
                TempData = tempData.Object
            };

            DateTime fecha = DateTime.Now;
            // Arrange - create a Job
            Job Job = new Job {
                JobID = 1,
                EngID = _users[1].EngID,
                Name = "Test",
                JobNum = 2015000000,
                JobTypeID = 1,
                PO = 3000000,
                CityID = 1,
                FireCodeID = 1,
                Contractor = "gaggg",
                Cust = "ffff",
                ShipDate = fecha,
                LatestFinishDate = fecha,
            };

            // Act - try to save the Job
            IActionResult result = target.NewJob(Job);

            // Assert - check that the repository was called
            mock.Verify(m => m.SaveJob(Job), Times.AtLeastOnce());

            // Assert - check the method result type
            Assert.IsType<ViewResult>(result);

        }

        [Fact]
        public void Can_Delete_Job()
        {
            // Arrange - create mock repository
            Mock<IJobRepository> mock = new Mock<IJobRepository>();
            Mock<IItemRepository> mockitems = new Mock<IItemRepository>();

            List<AppUser> _users = new List<AppUser>
            {
                new AppUser{ EngID = 1 },
                new AppUser{ EngID = 2 }
            };
            Mock<UserManager<AppUser>> mockusers = MockUserManager<AppUser>(_users);

            // Arrange - create a Job
            Job Job = new Job { JobID = 1, Name = "BRENTWOOD CONDOS", JobNum = 2017088571, Status = null, };

            // Arrange - create the mock repository
            mock.Setup(m => m.Jobs).Returns(new Job[] { Job }.AsQueryable<Job>());

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();

            // Arrange - create the controller
            JobController target = new JobController(mock.Object, mockitems.Object, mockusers.Object)
            {
                TempData = tempData.Object
            };

            // Act - delete the product
            target.Delete(Job.JobID);

            // Assert - ensure that the repository delete method was
            // called with the correct Country
            mock.Verify(m => m.DeleteEngJob(Job.JobID));
        }

        [Fact]
        public void Can_Delete_Complete_Job()
        {
            // Arrange - create mock repository
            Mock<IJobRepository> mock = new Mock<IJobRepository>();
            Mock<IItemRepository> mockitems = new Mock<IItemRepository>();

            List<AppUser> _users = new List<AppUser>
            {
                new AppUser{ EngID = 1 },
                new AppUser{ EngID = 2 }
            };
            Mock<UserManager<AppUser>> mockusers = MockUserManager<AppUser>(_users);

            // Arrange - create a Job
            Job Job = new Job { JobID = 1, Name = "BRENTWOOD CONDOS", JobNum = 2017088571, Status = null, };
            JobExtension JobExt = new JobExtension { JobID = 1, JobExtensionID = 1, };
            HydroSpecific hydro = new HydroSpecific { HydroSpecificID = 1, JobID = 1 };
            GenericFeatures generic = new GenericFeatures { GenericFeaturesID = 1, JobID = 1 };
            Indicator indicator = new Indicator { IndicatorID = 1, JobID = 1 };
            HoistWayData hoist = new HoistWayData { HoistWayDataID = 1, JobID = 1 };
            SpecialFeatures specialFeatures = new SpecialFeatures { SpecialFeaturesID = 1, JobID = 1 };

            // Arrange - create the mock repository
            mock.Setup(m => m.Jobs).Returns(new Job[] { Job }.AsQueryable<Job>());
            mock.Setup(m => m.JobsExtensions).Returns(new JobExtension[] { JobExt }.AsQueryable<JobExtension>);
            mock.Setup(m => m.HydroSpecifics).Returns(new HydroSpecific[] { hydro }.AsQueryable<HydroSpecific>);
            mock.Setup(m => m.GenericFeaturesList).Returns(new GenericFeatures[] { generic }.AsQueryable<GenericFeatures>);
            mock.Setup(m => m.Indicators).Returns(new Indicator[] { indicator }.AsQueryable<Indicator>);
            mock.Setup(m => m.HoistWayDatas).Returns(new HoistWayData[] { hoist }.AsQueryable<HoistWayData>);
            mock.Setup(m => m.SpecialFeatures).Returns(new SpecialFeatures[] { specialFeatures }.AsQueryable<SpecialFeatures>);

            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();

            // Arrange - create the controller
            JobController target = new JobController(mock.Object, mockitems.Object, mockusers.Object)
            {
                TempData = tempData.Object
            };

            // Act - delete the product
            target.Delete(Job.JobID);

            // Assert - ensure that the repository delete method was
            // called with the correct Country
            mock.Verify(m => m.DeleteEngJob(Job.JobID));
        }

        [Fact]
        public void Can_Continue()
        {
            // Arrange - create mock repository
            Mock<IJobRepository> mock = new Mock<IJobRepository>();
            Mock<IItemRepository> mockitems = new Mock<IItemRepository>();

            List<AppUser> _users = new List<AppUser>
            {
                new AppUser{ EngID = 1 },
                new AppUser{ EngID = 2 }
            };
            Mock<UserManager<AppUser>> mockusers = MockUserManager<AppUser>(_users);

            // Arrange - create mock temp data
            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();

            // Arrange - create the controller
            JobController target = new JobController(mock.Object, mockitems.Object, mockusers.Object)
            {
                TempData = tempData.Object
            };
            DateTime fecha = DateTime.Now;

            // Arrange - create a Job
            Job Job = new Job { JobID = 1, Name = "BRENTWOOD CONDOS", JobNum = 2017088571, Status = null, };

            // Arrange - create the mock repository
            mock.Setup(m => m.Jobs).Returns(new Job[] { Job }.AsQueryable<Job>());

            // Act - try to save the Job
            IActionResult result = target.Continue(Job.JobID);

            // Assert - check the method result type
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Can_Got_To_NextForm()
        {
            // Arrange - create mock repository
            Mock<IJobRepository> mock = new Mock<IJobRepository>();
            Mock<IItemRepository> mockitems = new Mock<IItemRepository>();

            List<AppUser> _users = new List<AppUser>
            {
                new AppUser{ EngID = 1 },
                new AppUser{ EngID = 2 }
            };
            Mock<UserManager<AppUser>> mockusers = MockUserManager<AppUser>(_users);

            // Arrange - create mock temp data
            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();

            // Arrange - create the controller
            JobController target = new JobController(mock.Object, mockitems.Object, mockusers.Object)
            {
                TempData = tempData.Object
            };

            // Arrange - create a Job
            Job Job = new Job { JobID = 1, Name = "BRENTWOOD CONDOS", JobNum = 2017088571, Status = null, };

            // Arrange - create the mock repository
            mock.Setup(m => m.Jobs).Returns(new Job[] { Job }.AsQueryable<Job>());

            JobViewModel viewModel = new JobViewModel()
            {
                CurrentJob = Job,
                CurrentJobExtension = null,
                CurrentGenericFeatures = null,
                CurrentHydroSpecific = null,
                CurrentIndicator = null,
                CurrentHoistWayData = null,
                SpecialFeatureslist = null,
                CurrentUserID = 1,
                CurrentTab = "Main",
                buttonAction = "Nel"
            };

            // Act - try to save the Job
            IActionResult result = target.NextForm(viewModel);

            // Assert - check that the repository was called
            mock.Verify(m => m.SaveEngJobView(viewModel), Times.AtLeastOnce());


            // Assert - check the method result type
            Assert.IsType<ViewResult>(result);
        
        }

        [Fact]
        public void Can_Edit_Job_Get()
        {
            // Arrange - create mock repository
            Mock<IJobRepository> mock = new Mock<IJobRepository>();
            Mock<IItemRepository> mockitems = new Mock<IItemRepository>();

            List<AppUser> _users = new List<AppUser>
            {
                new AppUser{ EngID = 1 },
                new AppUser{ EngID = 2 }
            };
            Mock<UserManager<AppUser>> mockusers = MockUserManager<AppUser>(_users);

            // Arrange - create mock temp data
            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();

            // Arrange - create the controller
            JobController target = new JobController(mock.Object, mockitems.Object, mockusers.Object)
            {
                TempData = tempData.Object
            };

            // Arrange - create a Job
            Job Job = new Job { JobID = 1, Name = "BRENTWOOD CONDOS", JobNum = 2017088571, Status = null, };
            JobExtension JobExt = new JobExtension { JobID = 1, JobExtensionID = 1, };
            HydroSpecific hydro = new HydroSpecific { HydroSpecificID = 1, JobID = 1 };
            GenericFeatures generic = new GenericFeatures { GenericFeaturesID = 1, JobID = 1 };
            Indicator indicator = new Indicator { IndicatorID = 1, JobID = 1 };
            HoistWayData hoist = new HoistWayData { HoistWayDataID = 1, JobID = 1 };
            SpecialFeatures specialFeatures = new SpecialFeatures { SpecialFeaturesID = 1, JobID = 1 };

            // Arrange - create the mock repository
            mock.Setup(m => m.Jobs).Returns(new Job[]{Job}.AsQueryable<Job>());
            mock.Setup(m => m.JobsExtensions).Returns(new JobExtension[] { JobExt }.AsQueryable<JobExtension>);
            mock.Setup(m => m.HydroSpecifics).Returns(new HydroSpecific[] { hydro }.AsQueryable<HydroSpecific>);
            mock.Setup(m => m.GenericFeaturesList).Returns(new GenericFeatures[] { generic }.AsQueryable<GenericFeatures>);
            mock.Setup(m => m.Indicators).Returns(new Indicator[] { indicator }.AsQueryable<Indicator>);
            mock.Setup(m => m.HoistWayDatas).Returns(new HoistWayData[] { hoist }.AsQueryable<HoistWayData>);
            mock.Setup(m => m.SpecialFeatures).Returns(new SpecialFeatures[] { specialFeatures }.AsQueryable<SpecialFeatures>);


            // Act - try to save the Job
            IActionResult result = target.Edit(Job.JobID);

            // Assert - check the method result type
            Assert.IsType<ViewResult>(result);

        }

        [Fact]
        public void Can_Edit_Job_Post()
        {
            // Arrange - create mock repository
            Mock<IJobRepository> mock = new Mock<IJobRepository>();
            Mock<IItemRepository> mockitems = new Mock<IItemRepository>();

            List<AppUser> _users = new List<AppUser>
            {
                new AppUser{ EngID = 1 },
                new AppUser{ EngID = 2 }
            };
            Mock<UserManager<AppUser>> mockusers = MockUserManager<AppUser>(_users);

            // Arrange - create mock temp data
            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();

            // Arrange - create the controller
            JobController target = new JobController(mock.Object, mockitems.Object, mockusers.Object)
            {
                TempData = tempData.Object
            };

            // Arrange - create a Job
            Job Job = new Job{JobID = 1,Name = "BRENTWOOD CONDOS",JobNum = 2017088571,Status = null,};
            JobExtension JobExt = new JobExtension{ JobID = 1, JobExtensionID = 1,};
            HydroSpecific hydro = new HydroSpecific{HydroSpecificID = 1, JobID = 1};
            GenericFeatures generic = new GenericFeatures{GenericFeaturesID = 1,JobID = 1};
            Indicator indicator = new Indicator{IndicatorID = 1,JobID = 1};
            HoistWayData hoist = new HoistWayData{HoistWayDataID = 1,JobID = 1};
            SpecialFeatures specialFeatures = new SpecialFeatures{SpecialFeaturesID = 1,JobID = 1};

            // Arrange - create the mock repository
            mock.Setup(m => m.Jobs).Returns(new Job[]{Job}.AsQueryable<Job>());
            mock.Setup(m => m.JobsExtensions).Returns(new JobExtension[]{JobExt}.AsQueryable<JobExtension>);
            mock.Setup(m => m.HydroSpecifics).Returns(new HydroSpecific[]{hydro}.AsQueryable<HydroSpecific>);
            mock.Setup(m => m.GenericFeaturesList).Returns(new GenericFeatures[]{generic}.AsQueryable<GenericFeatures>);
            mock.Setup(m => m.Indicators).Returns(new Indicator[]{indicator}.AsQueryable<Indicator>);
            mock.Setup(m => m.HoistWayDatas).Returns(new HoistWayData[]{hoist}.AsQueryable<HoistWayData>);
            mock.Setup(m => m.SpecialFeatures).Returns(new SpecialFeatures[]{specialFeatures}.AsQueryable<SpecialFeatures>);

            JobViewModel viewModel = new JobViewModel()
            {
                CurrentJob = Job,
                CurrentJobExtension = JobExt,
                CurrentGenericFeatures = generic,
                CurrentHydroSpecific = hydro,
                CurrentIndicator = indicator,
                CurrentHoistWayData = hoist,
                SpecialFeatureslist = null,
                CurrentUserID = 1,
                CurrentTab = "Main",
                buttonAction = "Nel"
            };

            // Act - try to save the Job
            IActionResult result = target.Edit(viewModel);

            // Assert - check that the repository was called
            mock.Verify(m => m.SaveEngJobView(viewModel), Times.AtLeastOnce());

            // Assert - check the method result type
            Assert.IsType<ViewResult>(result);

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

    }
}
