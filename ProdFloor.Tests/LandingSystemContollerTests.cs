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
    public class LandingSystemContollerTests
    {
        [Fact]
        public void Index_Contains_All_LandingSystems()
        {
            // Arrange - create the mock repository
            Mock<IItemRepository> mock = new Mock<IItemRepository>();
            mock.Setup(m => m.LandingSystems).Returns(new LandingSystem[]
            {
                new LandingSystem {LandingSystemID = 1, Name = "P1", UsedIn = "U1"},
                new LandingSystem {LandingSystemID = 2, Name = "P2", UsedIn = "U2"},
                new LandingSystem {LandingSystemID = 3, Name = "P3", UsedIn = "U2"},
            }.AsQueryable<LandingSystem>());

            // Arrange - create a controller
            LandingSystemController target = new LandingSystemController(mock.Object);

            // Action
            LandingSystemsListViewModel result = target.List(null, 1).ViewData.Model as LandingSystemsListViewModel;

            // Assert
            Assert.Equal(3, result.LandingSystems.Count());
            Assert.Equal("P1", result.LandingSystems[0].Name);
            Assert.Equal("P2", result.LandingSystems[1].Name);
            Assert.Equal("P3", result.LandingSystems[2].Name);
        }

        [Fact]
        public void Can_Edit_LandingSystem()
        {
            // Arrange - create the mock repository
            Mock<IItemRepository> mock = new Mock<IItemRepository>();
            mock.Setup(m => m.LandingSystems).Returns(new LandingSystem[]
            {
                new LandingSystem {LandingSystemID = 1, Name = "P1", UsedIn = "U1"},
                new LandingSystem {LandingSystemID = 2, Name = "P2", UsedIn = "U2"},
                new LandingSystem {LandingSystemID = 3, Name = "P3", UsedIn = "U2"},
            }.AsQueryable<LandingSystem>());

            // Arrange - create a controller
            LandingSystemController target = new LandingSystemController(mock.Object);

            // Act
            LandingSystem p1 = GetViewModel<LandingSystem>(target.Edit(1));
            LandingSystem p2 = GetViewModel<LandingSystem>(target.Edit(2));
            LandingSystem p3 = GetViewModel<LandingSystem>(target.Edit(3));

            // Assert
            Assert.Equal(1, p1.LandingSystemID);
            Assert.Equal(2, p2.LandingSystemID);
            Assert.Equal(3, p3.LandingSystemID);
        }

        [Fact]
        public void Cannot_Edit_Nonexistent_LandingSystem()
        {
            // Arrange - create the mock repository
            Mock<IItemRepository> mock = new Mock<IItemRepository>();
            mock.Setup(m => m.LandingSystems).Returns(new LandingSystem[]
            {
                new LandingSystem {LandingSystemID = 1, Name = "P1", UsedIn = "U1"},
                new LandingSystem {LandingSystemID = 2, Name = "P2", UsedIn = "U2"},
                new LandingSystem {LandingSystemID = 3, Name = "P3", UsedIn = "U2"},
            }.AsQueryable<LandingSystem>());

            // Arrange - create a controller
            LandingSystemController target = new LandingSystemController(mock.Object);

            // Act
            LandingSystem result = GetViewModel<LandingSystem>(target.Edit(4));

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Cannot_Save_Invalid_Changes()
        {
            // Arrange - create mock repository
            Mock<IItemRepository> mock = new Mock<IItemRepository>();

            // Arrange - create the controller
            LandingSystemController target = new LandingSystemController(mock.Object);

            // Arrange - create a product
            LandingSystem landingSystem = new LandingSystem { Name = "Test" };

            // Arrange - add an error to the model state
            target.ModelState.AddModelError("error", "error");

            // Act - try to save the product
            IActionResult result = target.Edit(landingSystem);

            // Assert - check that the repository was not called
            mock.Verify(m => m.SaveLandingSystem(landingSystem), Times.Never());

            // Assert - check the method result type
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Can_Delete_Valid_LandingSystems()
        {
            // Arrange - create a State
            LandingSystem prod = new LandingSystem { LandingSystemID = 2, Name = "Test",UsedIn = "U1"};

            // Arrange - create the mock repository
            Mock<IItemRepository> mock = new Mock<IItemRepository>();
            mock.Setup(m => m.LandingSystems).Returns(new LandingSystem[]
            {
                new LandingSystem {LandingSystemID = 1, Name = "P1",UsedIn = "U1"},
                prod,
                new LandingSystem {LandingSystemID = 3, Name = "P3",UsedIn = "U1"},
            }.AsQueryable<LandingSystem>());

            // Arrange - create the controller
            LandingSystemController target = new LandingSystemController(mock.Object);

            // Act - delete the product
            target.Delete(prod.LandingSystemID);

            // Assert - ensure that the repository delete method was
            // called with the correct State
            mock.Verify(m => m.DeleteLandingSystem(prod.LandingSystemID));
        }

        [Fact]
        public void Can_Paginate()
        {
            // Arrange
            Mock<IItemRepository> mock = new Mock<IItemRepository>();
            mock.Setup(m => m.LandingSystems).Returns((new LandingSystem[] {
                new LandingSystem {LandingSystemID = 1, Name = "S2",UsedIn = "U1"},
                new LandingSystem {LandingSystemID = 2, Name = "S1",UsedIn = "U2"},
                new LandingSystem {LandingSystemID = 3, Name = "S3",UsedIn = "U3"},
                new LandingSystem {LandingSystemID = 4, Name = "S4",UsedIn = "U4"},
                new LandingSystem {LandingSystemID = 5, Name = "S5",UsedIn = "U4"}
            }).AsQueryable<LandingSystem>());
            LandingSystemController controller = new LandingSystemController(mock.Object);
            controller.PageSize = 3;

            // Act
            LandingSystemsListViewModel result = controller.List(null,2).ViewData.Model as LandingSystemsListViewModel;
            
            // Assert
            LandingSystem[] prodArray = result.LandingSystems.ToArray();
            Assert.True(prodArray.Length == 2);
            Assert.Equal("S4", prodArray[0].Name);
            Assert.Equal("S5", prodArray[1].Name);
        }

        [Fact]
        public void Can_Send_Pagination_View_Model()
        {
            // Arrange
            Mock<IItemRepository> mock = new Mock<IItemRepository>();
            mock.Setup(m => m.LandingSystems).Returns((new LandingSystem[] {
                new LandingSystem {LandingSystemID = 1, Name = "S2",UsedIn = "U1"},
                new LandingSystem {LandingSystemID = 2, Name = "S1",UsedIn = "U2"},
                new LandingSystem {LandingSystemID = 3, Name = "S3",UsedIn = "U3"},
                new LandingSystem {LandingSystemID = 4, Name = "S4",UsedIn = "U4"},
                new LandingSystem {LandingSystemID = 5, Name = "S5",UsedIn = "U4"}
            }).AsQueryable<LandingSystem>());
            LandingSystemController controller = new LandingSystemController(mock.Object);
            controller.PageSize = 3;

            // Act
            LandingSystemsListViewModel result = controller.List(null, 2).ViewData.Model as LandingSystemsListViewModel;


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
            Mock<IItemRepository> mockitems = new Mock<IItemRepository>();

            // Arrange - create mock temp data
            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();

            // Arrange - create the controller
            LandingSystemController target = new LandingSystemController(mockitems.Object)
            {
                TempData = tempData.Object
            };
            // Arrange - create a city
            LandingSystem landingSystem = new LandingSystem { LandingSystemID = 1, Name = "Test", UsedIn = "U1" };

            // Act - try to save the city
            IActionResult result = target.Edit(landingSystem);

            // Assert - check that the repository was called
            mockitems.Verify(m => m.SaveLandingSystem(landingSystem), Times.AtLeastOnce());

            // Assert - check the method result type
            Assert.IsType<RedirectToActionResult>(result);
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
