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
    public class FireCodeControllerTests
    {
        [Fact]
        public void Index_Contains_All_FireCodes()
        {
            // Arrange - create the mock repository
            Mock<IItemRepository> mock = new Mock<IItemRepository>();
            mock.Setup(m => m.FireCodes).Returns(new FireCode[]
            {
                new FireCode {FireCodeID = 1, Name = "P1"},
                new FireCode {FireCodeID = 2, Name = "P2"},
                new FireCode {FireCodeID = 3, Name = "P3"},
            }.AsQueryable<FireCode>());

            // Arrange - create a controller
            FireCodeController target = new FireCodeController(mock.Object);

            // Action
            FireCodesListViewModel result = target.List(1).ViewData.Model as FireCodesListViewModel;

            // Assert
            Assert.Equal(3, result.FireCodes.Count());
            Assert.Equal("P1", result.FireCodes[0].Name);
            Assert.Equal("P2", result.FireCodes[1].Name);
            Assert.Equal("P3", result.FireCodes[2].Name);
        }

        [Fact]
        public void Can_Edit_FireCode()
        {
            // Arrange - create the mock repository
            Mock<IItemRepository> mock = new Mock<IItemRepository>();
            mock.Setup(m => m.FireCodes).Returns(new FireCode[]
            {
                new FireCode {FireCodeID = 1, Name = "P1"},
                new FireCode {FireCodeID = 2, Name = "P2"},
                new FireCode {FireCodeID = 3, Name = "P3"},
            }.AsQueryable<FireCode>());

            // Arrange - create a controller
            FireCodeController target = new FireCodeController(mock.Object);

            // Act
            FireCode p1 = GetViewModel<FireCode>(target.Edit(1));
            FireCode p2 = GetViewModel<FireCode>(target.Edit(2));
            FireCode p3 = GetViewModel<FireCode>(target.Edit(3));

            // Assert
            Assert.Equal(1, p1.FireCodeID);
            Assert.Equal(2, p2.FireCodeID);
            Assert.Equal(3, p3.FireCodeID);
        }

        [Fact]
        public void Cannot_Edit_Nonexistent_FireCode()
        {
            // Arrange - create the mock repository
            Mock<IItemRepository> mock = new Mock<IItemRepository>();
            mock.Setup(m => m.FireCodes).Returns(new FireCode[]
            {
                new FireCode {FireCodeID = 1, Name = "P1"},
                new FireCode {FireCodeID = 2, Name = "P2"},
                new FireCode {FireCodeID = 3, Name = "P3"},
            }.AsQueryable<FireCode>());

            // Arrange - create a controller
            FireCodeController target = new FireCodeController(mock.Object);

            // Act
            FireCode result = GetViewModel<FireCode>(target.Edit(4));

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Cannot_Save_Invalid_Changes()
        {
            // Arrange - create mock repository
            Mock<IItemRepository> mock = new Mock<IItemRepository>();

            // Arrange - create the controller
            FireCodeController target = new FireCodeController(mock.Object);

            // Arrange - create a product
            FireCode fireCode = new FireCode { Name = "Test" };

            // Arrange - add an error to the model state
            target.ModelState.AddModelError("error", "error");

            // Act - try to save the product
            IActionResult result = target.Edit(fireCode);

            // Assert - check that the repository was not called
            mock.Verify(m => m.SaveFireCode(fireCode), Times.Never());

            // Assert - check the method result type
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Can_Delete_Valid_FireCodes()
        {
            // Arrange - create a State
            FireCode prod = new FireCode { FireCodeID = 2, Name = "Test" };

            // Arrange - create the mock repository
            Mock<IItemRepository> mock = new Mock<IItemRepository>();
            mock.Setup(m => m.FireCodes).Returns(new FireCode[]
            {
                new FireCode {FireCodeID = 1, Name = "P1"},
                prod,
                new FireCode {FireCodeID = 3, Name = "P3"},
            }.AsQueryable<FireCode>());

            // Arrange - create the controller
            FireCodeController target = new FireCodeController(mock.Object);

            // Act - delete the product
            target.Delete(prod.FireCodeID);

            // Assert - ensure that the repository delete method was
            // called with the correct State
            mock.Verify(m => m.DeleteFireCode(prod.FireCodeID));
        }

        [Fact]
        public void Can_Paginate()
        {
            // Arrange
            Mock<IItemRepository> mock = new Mock<IItemRepository>();
            mock.Setup(m => m.FireCodes).Returns((new FireCode[] {
                new FireCode {FireCodeID = 1, Name = "S2"},
                new FireCode {FireCodeID = 2, Name = "S1"},
                new FireCode {FireCodeID = 3, Name = "S3"},
                new FireCode {FireCodeID = 4, Name = "S4"},
                new FireCode {FireCodeID = 5, Name = "S5"}
            }).AsQueryable<FireCode>());
            FireCodeController controller = new FireCodeController(mock.Object);
            controller.PageSize = 3;

            // Act
            FireCodesListViewModel result = controller.List(2).ViewData.Model as FireCodesListViewModel;

            // Assert
            FireCode[] prodArray = result.FireCodes.ToArray();
            Assert.True(prodArray.Length == 2);
            Assert.Equal("S4", prodArray[0].Name);
            Assert.Equal("S5", prodArray[1].Name);
        }

        [Fact]
        public void Can_Send_Pagination_View_Model()
        {
            // Arrange
            Mock<IItemRepository> mock = new Mock<IItemRepository>();
            mock.Setup(m => m.FireCodes).Returns((new FireCode[] {
                new FireCode {FireCodeID = 1, Name = "S2"},
                new FireCode {FireCodeID = 2, Name = "S1"},
                new FireCode {FireCodeID = 3, Name = "S3"},
                new FireCode {FireCodeID = 4, Name = "S4"},
                new FireCode {FireCodeID = 5, Name = "S5"}
            }).AsQueryable<FireCode>());
            FireCodeController controller = new FireCodeController(mock.Object);
            controller.PageSize = 3;

            // Act
            FireCodesListViewModel result = controller.List(2).ViewData.Model as FireCodesListViewModel;

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
            FireCodeController target = new FireCodeController(mockitems.Object)
            {
                TempData = tempData.Object
            };
            // Arrange - create a city
            FireCode fireCode = new FireCode { FireCodeID = 1, Name = "Test" };

            // Act - try to save the city
            IActionResult result = target.Edit(fireCode);

            // Assert - check that the repository was called
            mockitems.Verify(m => m.SaveFireCode(fireCode), Times.AtLeastOnce());

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
