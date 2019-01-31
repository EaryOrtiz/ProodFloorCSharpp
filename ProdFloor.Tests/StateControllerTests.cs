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

namespace ProdFloor.Tests
{
    public class StateControllerTests
    {
        [Fact]
        public void Index_Contains_All_States()
        {
            // Arrange - create the mock repository
            Mock<IItemRepository> mock = new Mock<IItemRepository>();
            mock.Setup(m => m.States).Returns(new State[]
            {
                new State {StateID = 1, Name = "P1"},
                new State {StateID = 2, Name = "P2"},
                new State {StateID = 3, Name = "P3"},
            }.AsQueryable<State>());

            // Arrange - create a controller
            StateController target = new StateController(mock.Object);

            // Action
            StateListViewModel result = target.List(1,1).ViewData.Model as StateListViewModel;

            // Assert
            Assert.Equal(3, result.States.Count());
            Assert.Equal("P1", result.States[0].Name);
            Assert.Equal("P2", result.States[1].Name);
            Assert.Equal("P3", result.States[2].Name);
        }

        [Fact]
        public void Can_Edit_State()
        {
            // Arrange - create the mock repository
            Mock<IItemRepository> mock = new Mock<IItemRepository>();
            mock.Setup(m => m.States).Returns(new State[]
            {
                new State {StateID = 1, Name = "P1"},
                new State {StateID = 2, Name = "P2"},
                new State {StateID = 3, Name = "P3"},
            }.AsQueryable<State>());

            // Arrange - create the controller
            StateController target = new StateController(mock.Object);

            // Act
            State p1 = GetViewModel<State>(target.Edit(1));
            State p2 = GetViewModel<State>(target.Edit(2));
            State p3 = GetViewModel<State>(target.Edit(3));

            // Assert
            Assert.Equal(1, p1.StateID);
            Assert.Equal(2, p2.StateID);
            Assert.Equal(3, p3.StateID);
        }

        [Fact]
        public void Cannot_Edit_Nonexistent_State()
        {
            // Arrange - create the mock repository
            Mock<IItemRepository> mock = new Mock<IItemRepository>();
            mock.Setup(m => m.States).Returns(new State[]
            {
                new State {StateID = 1, Name = "P1"},
                new State {StateID = 2, Name = "P2"},
                new State {StateID = 3, Name = "P3"},
            }.AsQueryable<State>());

            // Arrange - create the controller
            StateController target = new StateController(mock.Object);

            // Act
            State result = GetViewModel<State>(target.Edit(4));

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Cannot_Save_Invalid_Changes()
        {
            // Arrange - create mock repository
            Mock<IItemRepository> mock = new Mock<IItemRepository>();

            // Arrange - create the controller
            StateController target = new StateController(mock.Object);

            // Arrange - create a product
            State product = new State { Name = "Test" };

            // Arrange - add an error to the model state
            target.ModelState.AddModelError("error", "error");

            // Act - try to save the product
            IActionResult result = target.Edit(product);

            // Assert - check that the repository was not called
            mock.Verify(m => m.SaveState(It.IsAny<State>()), Times.Never());

            // Assert - check the method result type
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Can_Delete_Valid_States()
        {
            // Arrange - create a State
            State prod = new State { StateID = 2, Name = "Test" };

            // Arrange - create the mock repository
            Mock<IItemRepository> mock = new Mock<IItemRepository>();
            mock.Setup(m => m.States).Returns(new State[]
            {
                new State {StateID = 1, Name = "P1"},
                prod,
                new State {StateID = 3, Name = "P3"},
            }.AsQueryable<State>());

            // Arrange - create the controller
            StateController target = new StateController(mock.Object);

            // Act - delete the product
            target.Delete(prod.StateID);

            // Assert - ensure that the repository delete method was
            // called with the correct State
            mock.Verify(m => m.DeleteState(prod.StateID));
        }

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
            controller.List(1, 2).ViewData.Model as JobsListViewModel;

            // Assert
            PagingInfo pageInfo = result.PagingInfo;
            Assert.Equal(2, pageInfo.CurrentPage);
            Assert.Equal(3, pageInfo.ItemsPerPage);
            Assert.Equal(5, pageInfo.TotalItems);
            Assert.Equal(2, pageInfo.TotalPages);
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
            JobController target = new JobController(mock.Object,mockitems.Object, mockusers.Object)
            {
                TempData = tempData.Object
            };

            // Arrange - create a Job
            Job Job = new Job { Name = "Test" };

            // Act - try to save the Job
            IActionResult result = target.Edit(Job.JobID);
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
    }
}