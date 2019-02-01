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
    public class DoorOperatorControllerTests
    {
        [Fact]
        public void Index_Contains_All_States()
        {
            // Arrange - create the mock repository
            Mock<IItemRepository> mock = new Mock<IItemRepository>();
            mock.Setup(m => m.DoorOperators).Returns(new DoorOperator[]
            {
                new DoorOperator {DoorOperatorID = 1,Brand = "B1",Style = "S1", Name = "P1"},
                new DoorOperator {DoorOperatorID = 2,Brand = "B2",Style = "S2", Name = "P2"},
                new DoorOperator {DoorOperatorID = 3, Brand = "B3",Style = "S3", Name = "P3"},
            }.AsQueryable<DoorOperator>());

            // Arrange - create a controller
            DoorOperatorController target = new DoorOperatorController(mock.Object);

            // Action
            DoorOperatorsListViewModel result = target.List(null, 1).ViewData.Model as DoorOperatorsListViewModel;

            // Assert
            Assert.Equal(3, result.DoorOperators.Count());
            Assert.Equal("P1", result.DoorOperators[0].Name);
            Assert.Equal("P2", result.DoorOperators[1].Name);
            Assert.Equal("P3", result.DoorOperators[2].Name);
        }

        [Fact]
        public void Can_Edit_State()
        {
            // Arrange - create the mock repository
            Mock<IItemRepository> mock = new Mock<IItemRepository>();
            mock.Setup(m => m.DoorOperators).Returns(new DoorOperator[]
            {
                new DoorOperator {DoorOperatorID = 1,Brand = "B1",Style = "S1", Name = "P1"},
                new DoorOperator {DoorOperatorID = 2,Brand = "B2",Style = "S2", Name = "P2"},
                new DoorOperator {DoorOperatorID = 3, Brand = "B3",Style = "S3", Name = "P3"},
            }.AsQueryable<DoorOperator>());

            // Arrange - create a controller
            DoorOperatorController target = new DoorOperatorController(mock.Object);

            // Act
            DoorOperator p1 = GetViewModel<DoorOperator>(target.Edit(1));
            DoorOperator p2 = GetViewModel<DoorOperator>(target.Edit(2));
            DoorOperator p3 = GetViewModel<DoorOperator>(target.Edit(3));

            // Assert
            Assert.Equal(1, p1.DoorOperatorID);
            Assert.Equal(2, p2.DoorOperatorID);
            Assert.Equal(3, p3.DoorOperatorID);
        }

        [Fact]
        public void Cannot_Edit_Nonexistent_State()
        {
            // Arrange - create the mock repository
            Mock<IItemRepository> mock = new Mock<IItemRepository>();
            mock.Setup(m => m.DoorOperators).Returns(new DoorOperator[]
            {
                new DoorOperator {DoorOperatorID = 1,Brand = "B1",Style = "S1", Name = "P1"},
                new DoorOperator {DoorOperatorID = 2,Brand = "B2",Style = "S2", Name = "P2"},
                new DoorOperator {DoorOperatorID = 3, Brand = "B3",Style = "S3", Name = "P3"},
            }.AsQueryable<DoorOperator>());

            // Arrange - create a controller
            DoorOperatorController target = new DoorOperatorController(mock.Object);

            // Act
            DoorOperator result = GetViewModel<DoorOperator>(target.Edit(4));

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Cannot_Save_Invalid_Changes()
        {
            // Arrange - create mock repository
            Mock<IItemRepository> mock = new Mock<IItemRepository>();

            // Arrange - create the controller
            DoorOperatorController target = new DoorOperatorController(mock.Object);

            // Arrange - create a product
            DoorOperator door = new DoorOperator { Name = "Test" };

            // Arrange - add an error to the model state
            target.ModelState.AddModelError("error", "error");

            // Act - try to save the product
            IActionResult result = target.Edit(door);

            // Assert - check that the repository was not called
            mock.Verify(m => m.SaveDoorOperator(door), Times.Never());

            // Assert - check the method result type
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Can_Delete_Valid_States()
        {
            // Arrange - create a State
            DoorOperator prod = new DoorOperator { DoorOperatorID = 2, Name = "Test" };

            // Arrange - create the mock repository
            Mock<IItemRepository> mock = new Mock<IItemRepository>();
            mock.Setup(m => m.DoorOperators).Returns(new DoorOperator[]
            {
                new DoorOperator {DoorOperatorID = 1, Name = "P1"},
                prod,
                new DoorOperator {DoorOperatorID = 3, Name = "P3"},
            }.AsQueryable<DoorOperator>());

            // Arrange - create the controller
            DoorOperatorController target = new DoorOperatorController(mock.Object);

            // Act - delete the product
            target.Delete(prod.DoorOperatorID);

            // Assert - ensure that the repository delete method was
            // called with the correct State
            mock.Verify(m => m.DeleteDoorOperator(prod.DoorOperatorID));
        }

        [Fact]
        public void Can_Paginate()
        {
            Mock<IItemRepository> mock = new Mock<IItemRepository>();
            mock.Setup(m => m.DoorOperators).Returns(new DoorOperator[]
            {
                new DoorOperator {DoorOperatorID = 1,Brand = "B1",Style = "S1", Name = "P1"},
                new DoorOperator {DoorOperatorID = 2,Brand = "B2",Style = "S2", Name = "P2"},
                new DoorOperator {DoorOperatorID = 3, Brand = "B3",Style = "S3", Name = "P3"},
                new DoorOperator {DoorOperatorID = 4, Brand = "B4",Style = "S4", Name = "P4"},
                new DoorOperator {DoorOperatorID = 5, Brand = "B5",Style = "S5", Name = "P5"},
            }.AsQueryable<DoorOperator>());

            // Arrange - create a controller
            DoorOperatorController controller = new DoorOperatorController(mock.Object);
            controller.PageSize = 3;

            // Act
            DoorOperatorsListViewModel result = controller.List(null, 2).ViewData.Model as DoorOperatorsListViewModel;

            // Assert
            DoorOperator[] prodArray = result.DoorOperators.ToArray();
            Assert.True(prodArray.Length == 2);
            Assert.Equal("P4", prodArray[0].Name);
            Assert.Equal("P5", prodArray[1].Name);
        }

        [Fact]
        public void Can_Send_Pagination_View_Model()
        {
            // Arrange
            Mock<IItemRepository> mock = new Mock<IItemRepository>();
            mock.Setup(m => m.DoorOperators).Returns(new DoorOperator[]
            {
                new DoorOperator {DoorOperatorID = 1,Brand = "B1",Style = "S1", Name = "P1"},
                new DoorOperator {DoorOperatorID = 2,Brand = "B2",Style = "S2", Name = "P2"},
                new DoorOperator {DoorOperatorID = 3, Brand = "B3",Style = "S3", Name = "P3"},
                new DoorOperator {DoorOperatorID = 4, Brand = "B4",Style = "S4", Name = "P4"},
                new DoorOperator {DoorOperatorID = 5, Brand = "B5",Style = "S5", Name = "P5"},
            }.AsQueryable<DoorOperator>());

            // Arrange
            DoorOperatorController controller =
            new DoorOperatorController(mock.Object) { PageSize = 3 };

            // Act
            DoorOperatorsListViewModel result =
            controller.List(null,2).ViewData.Model as DoorOperatorsListViewModel;

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
            DoorOperatorController target = new DoorOperatorController(mockitems.Object)
            {
                TempData = tempData.Object
            };
            // Arrange - create a city
            DoorOperator door = new DoorOperator { DoorOperatorID = 1, Brand = "B1", Style = "S1", Name = "P1" };

            // Act - try to save the city
            IActionResult result = target.Edit(door);

            // Assert - check that the repository was not called
            mockitems.Verify(m => m.SaveDoorOperator(door), Times.AtLeastOnce());

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
