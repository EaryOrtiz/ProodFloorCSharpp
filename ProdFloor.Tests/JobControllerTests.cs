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
using System.IO;
using System.Security.Principal;

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
        /*Todavia no se puede crear el moq de hhttp.context
        [Fact]
        public void Can_Save_New_Valid_Job()
        {
            // Arrange - create mock repository
            Mock<IJobRepository> mock = new Mock<IJobRepository>();
            Mock<IItemRepository> mockitems = new Mock<IItemRepository>();
            Mock<JobController> mockJob = new Mock<JobController>();


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
        Todavia no se puede crear el moq de hhttp.context
        [Fact]
        public void Can_Not_Save_New_Valid_Job()
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
            Job Job = new Job { Name = "Test Job"};

            // Arrange - add an error to the model state
            target.ModelState.AddModelError("error", "error");

            // Act - try to save the Job
            IActionResult result = target.NewJob(Job);

            // Assert - check that the repository was called
            mock.Verify(m => m.SaveJob(Job), Times.Never());

            // Assert - check the method result type
            Assert.IsType<ViewResult>(result);

        }
        */

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
        public void Can_Continue_a_Job()
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
        public void Can_Not_Continue_a_Job()
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
            Job Job = new Job { JobID = 1};

            // Arrange - create the mock repository
            mock.Setup(m => m.Jobs).Returns(new Job[] { Job }.AsQueryable<Job>());

            // Act - try to save the Job
            IActionResult result = target.Continue(2);

            // Assert - check the method result type
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Can_Go_To_NextForm()
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
        public void Can_Not_Go_To_NextForm()
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

            // Arrange - add an error to the model state
            target.ModelState.AddModelError("error", "error");

            // Arrange - create a Job
            Job Job = new Job { JobID = 1};

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
            mock.Verify(m => m.SaveEngJobView(viewModel), Times.Never());


            // Assert - check the method result type
            Assert.IsType<ViewResult>(result);
        }
        /*Todavia no se puede crear el moq de hhttp.context
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
        */
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

        [Fact]
        public void Can_Not_Edit_Job()
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

            // Arrange - add an error to the model state
            target.ModelState.AddModelError("error", "error");

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
            mock.Verify(m => m.SaveEngJobView(viewModel), Times.Never());

            // Assert - check the method result type
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Can_Add_New_SpecialFeature_Field_Get()
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
            mock.Setup(m => m.Jobs).Returns(new Job[] { Job }.AsQueryable<Job>());
            mock.Setup(m => m.JobsExtensions).Returns(new JobExtension[] { JobExt }.AsQueryable<JobExtension>);
            mock.Setup(m => m.HydroSpecifics).Returns(new HydroSpecific[] { hydro }.AsQueryable<HydroSpecific>);
            mock.Setup(m => m.GenericFeaturesList).Returns(new GenericFeatures[] { generic }.AsQueryable<GenericFeatures>);
            mock.Setup(m => m.Indicators).Returns(new Indicator[] { indicator }.AsQueryable<Indicator>);
            mock.Setup(m => m.HoistWayDatas).Returns(new HoistWayData[] { hoist }.AsQueryable<HoistWayData>);
            mock.Setup(m => m.SpecialFeatures).Returns(new SpecialFeatures[] { specialFeatures }.AsQueryable<SpecialFeatures>);


            // Act - try to add new SpecialFeature Field
            IActionResult result = target.AddSF(Job.JobID);

            // Assert - check the method result type
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Can_Delete_SpecialFeature_Field()
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
            SpecialFeatures sF = new SpecialFeatures { SpecialFeaturesID = 1, JobID = 1 };
            SpecialFeatures sF2 = new SpecialFeatures { SpecialFeaturesID = 2, JobID = 1 };

            // Arrange - create the mock repository
            mock.Setup(m => m.Jobs).Returns(new Job[] { Job }.AsQueryable<Job>());
            mock.Setup(m => m.JobsExtensions).Returns(new JobExtension[] { JobExt }.AsQueryable<JobExtension>);
            mock.Setup(m => m.HydroSpecifics).Returns(new HydroSpecific[] { hydro }.AsQueryable<HydroSpecific>);
            mock.Setup(m => m.GenericFeaturesList).Returns(new GenericFeatures[] { generic }.AsQueryable<GenericFeatures>);
            mock.Setup(m => m.Indicators).Returns(new Indicator[] { indicator }.AsQueryable<Indicator>);
            mock.Setup(m => m.HoistWayDatas).Returns(new HoistWayData[] { hoist }.AsQueryable<HoistWayData>);
            mock.Setup(m => m.SpecialFeatures).Returns(new SpecialFeatures[] { sF, sF2 }.AsQueryable<SpecialFeatures>);
            //Create a view model
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
            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();

            // Arrange - create the controller
            JobController target = new JobController(mock.Object, mockitems.Object, mockusers.Object)
            {
                TempData = tempData.Object
            };

            // Act - delete the product
            IActionResult result =  target.DeleteSF(sF2.SpecialFeaturesID, "retunrURL", viewModel);

            // Assert - ensure that the repository delete method was
            // called with the correct SpecialFeature Field
            mock.Verify(m => m.DeleteSpecialFeatures(sF2.SpecialFeaturesID));

            // Assert - check the method result type
            Assert.IsType<RedirectResult>(result);
        }

        [Fact]
        public void Can_Filter_JobList_On_Search()
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
            Job Job = new Job { JobID = 1, Name = "BRENTWOOD CONDOS", JobNum = 2017088571, Status =  "Incomplete", CityID = 1};
            Job Job2 = new Job { JobID = 2, Name = "School", JobNum = 2018088687, Status = "Incomplete", CityID = 2 };
            JobExtension JobExt = new JobExtension { JobID = 1, JobExtensionID = 1, DoorOperatorID = 1 };
            JobExtension JobExt2 = new JobExtension { JobID = 2, JobExtensionID = 1, DoorOperatorID = 1 };
            HydroSpecific hydro = new HydroSpecific { HydroSpecificID = 1, JobID = 1 };
            GenericFeatures generic = new GenericFeatures { GenericFeaturesID = 1, JobID = 1 };
            Indicator indicator = new Indicator { IndicatorID = 1, JobID = 1 };
            HoistWayData hoist = new HoistWayData { HoistWayDataID = 1, JobID = 1 };
            SpecialFeatures specialFeatures = new SpecialFeatures { SpecialFeaturesID = 1, JobID = 1 };

            List<Job> Joblist = new List<Job>(); Joblist.Add(Job); Joblist.Add(Job2);
            List<JobExtension> JobExtlist = new List<JobExtension>(); JobExtlist.Add(JobExt); JobExtlist.Add(JobExt2);
            List<HydroSpecific> hydroList = new List<HydroSpecific>(); hydroList.Add(hydro);
            List<GenericFeatures> genericList = new List<GenericFeatures>(); genericList.Add(generic);
            List<Indicator> indicatorList = new List<Indicator>(); indicatorList.Add(indicator);
            List<HoistWayData> hoistList = new List<HoistWayData>(); hoistList.Add(hoist);
            List<SpecialFeatures> specialFeaturesList = new List<SpecialFeatures>(); specialFeaturesList.Add(specialFeatures);


            // Arrange - create the mock repository
            mock.Setup(m => m.Jobs).Returns(new Job[] { Job, Job2 }.AsQueryable<Job>());
            mock.Setup(m => m.JobsExtensions).Returns(new JobExtension[] { JobExt, JobExt2 }.AsQueryable<JobExtension>);
            mock.Setup(m => m.HydroSpecifics).Returns(new HydroSpecific[] { hydro }.AsQueryable<HydroSpecific>);
            mock.Setup(m => m.GenericFeaturesList).Returns(new GenericFeatures[] { generic }.AsQueryable<GenericFeatures>);
            mock.Setup(m => m.Indicators).Returns(new Indicator[] { indicator }.AsQueryable<Indicator>);
            mock.Setup(m => m.HoistWayDatas).Returns(new HoistWayData[] { hoist }.AsQueryable<HoistWayData>);
            mock.Setup(m => m.SpecialFeatures).Returns(new SpecialFeatures[] { specialFeatures }.AsQueryable<SpecialFeatures>);

            JobSearchViewModel viewModel = new JobSearchViewModel()
            {
                JobsSearchList = Joblist,
                JobExtensionList = JobExtlist,
                HydroSpecificList = hydroList,
                GenericFeaturesList = genericList,
                IndicatorList = indicatorList,
                HoistWayDataList = hoistList,
                SpecialFeatureslist = specialFeaturesList,
                NumJobSearch = 2017088571,
            };

            // Act - try to save the Jobs
            //ViewResult result = (target.JobSearchList(viewModel, 1).Result as ViewResult);
            //Job[] jobArray = (result.ViewData.Model);

            var result  = (GetViewModel<JobSearchViewModel>(target.JobSearchList(viewModel,1).Result as IActionResult)).JobsSearchList;

            // Assert
            Job[] jobArray = result.ToArray();
            Assert.True(jobArray.Length == 1);
            Assert.Equal(1, jobArray[0].JobID);


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
