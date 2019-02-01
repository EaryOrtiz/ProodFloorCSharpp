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
    public class CityControllerTests
    {
        [Fact]
        public void Index_Contains_All_States()
        {
            // Arrange - create the mock repository
            Mock<IItemRepository> mock = new Mock<IItemRepository>();
            mock.Setup(m => m.Cities).Returns(new City[]
            {
                new City {CityID = 1, Name = "P1"},
                new City {CityID = 2, Name = "P2"},
                new City {CityID = 3, Name = "P3"},
            }.AsQueryable<City>());

            // Arrange - create a controller
            CityController target = new CityController(mock.Object);

            // Action
            CityListViewModel result = target.List(1, 1).ViewData.Model as CityListViewModel;

            // Assert
            Assert.Equal(3, result.Cities.Count());
            Assert.Equal("P1", result.Cities[0].Name);
            Assert.Equal("P2", result.Cities[1].Name);
            Assert.Equal("P3", result.Cities[2].Name);
        }

        [Fact]
        public void Can_Edit_State()
        {
            // Arrange - create the mock repository
            Mock<IItemRepository> mock = new Mock<IItemRepository>();
            mock.Setup(m => m.Cities).Returns(new City[]
            {
                new City {CityID = 1, Name = "P1"},
                new City {CityID = 2, Name = "P2"},
                new City {CityID = 3, Name = "P3"},
            }.AsQueryable<City>());

            // Arrange - create the controller
            CityController target = new CityController(mock.Object);

            // Act
            City p1 = GetViewModel<City>(target.Edit(1));
            City p2 = GetViewModel<City>(target.Edit(2));
            City p3 = GetViewModel<City>(target.Edit(3));

            // Assert
            Assert.Equal(1, p1.CityID);
            Assert.Equal(2, p2.CityID);
            Assert.Equal(3, p3.CityID);
        }

        [Fact]
        public void Cannot_Edit_Nonexistent_State()
        {
            // Arrange - create the mock repository
            Mock<IItemRepository> mock = new Mock<IItemRepository>();
            mock.Setup(m => m.Cities).Returns(new City[]
            {
                new City {CityID = 1, Name = "P1"},
                new City {CityID = 2, Name = "P2"},
                new City {CityID = 3, Name = "P3"},
            }.AsQueryable<City>());

            // Arrange - create the controller
            CityController target = new CityController(mock.Object);

            // Act
            City result = GetViewModel<City>(target.Edit(4));

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Cannot_Save_Invalid_Changes()
        {
            // Arrange - create mock repository
            Mock<IItemRepository> mock = new Mock<IItemRepository>();

            // Arrange - create the controller
            CityController target = new CityController(mock.Object);

            // Arrange - create a product
            City city = new City { Name = "Test" };

            // Arrange - add an error to the model state
            target.ModelState.AddModelError("error", "error");

            // Act - try to save the product
            IActionResult result = target.Edit(city);

            // Assert - check that the repository was not called
            mock.Verify(m => m.SaveCity(city), Times.Never());

            // Assert - check the method result type
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Can_Delete_Valid_States()
        {
            // Arrange - create a State
            City prod = new City { CityID = 2, Name = "Test" };

            // Arrange - create the mock repository
            Mock<IItemRepository> mock = new Mock<IItemRepository>();
            mock.Setup(m => m.Cities).Returns(new City[]
            {
                new City {CityID = 1, Name = "P1"},
                prod,
                new City {CityID = 3, Name = "P3"},
            }.AsQueryable<City>());

            // Arrange - create the controller
            CityController target = new CityController(mock.Object);

            // Act - delete the product
            target.Delete(prod.CityID);

            // Assert - ensure that the repository delete method was
            // called with the correct State
            mock.Verify(m => m.DeleteCity(prod.CityID));
        }

        [Fact]
        public void Can_Paginate()
        {
            // Arrange
            Mock<IItemRepository> mock = new Mock<IItemRepository>();
            mock.Setup(m => m.Cities).Returns((new City[] {
                new City {StateID = 1, CityID = 1, Name = "S2"},
                new City {StateID = 2, CityID = 2, Name = "S1"},
                new City {StateID = 3, CityID = 3, Name = "S3"},
                new City {StateID = 4, CityID = 4, Name = "S4"},
                new City {StateID = 5, CityID = 5, Name = "S5"}
            }).AsQueryable<City>());
            CityController controller = new CityController(mock.Object);
            controller.PageSize = 3;

            // Act
            CityListViewModel result = controller.List(1, 2).ViewData.Model as CityListViewModel;

            // Assert
            City[] prodArray = result.Cities.ToArray();
            Assert.True(prodArray.Length == 2);
            Assert.Equal("S4", prodArray[0].Name);
            Assert.Equal("S5", prodArray[1].Name);
        }

        [Fact]
        public void Can_Send_Pagination_View_Model()
        {
            // Arrange
            Mock<IItemRepository> mock = new Mock<IItemRepository>();
            mock.Setup(m => m.Cities).Returns((new City[] {
                new City {StateID = 1, CityID = 1, Name = "S2"},
                new City {StateID = 2, CityID = 2, Name = "S1"},
                new City {StateID = 3, CityID = 3, Name = "S3"},
                new City {StateID = 4, CityID = 4, Name = "S4"},
                new City {StateID = 5, CityID = 5, Name = "S5"}
            }).AsQueryable<City>());

            // Arrange
            CityController controller =
            new CityController(mock.Object) { PageSize = 3 };

            // Act
            CityListViewModel result =
            controller.List(2).ViewData.Model as CityListViewModel;

            // Assert
            PagingInfo pageInfo = result.PagingInfo;
            Assert.Equal(1, pageInfo.CurrentPage);
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
            CityController target = new CityController(mockitems.Object)
            {
                TempData = tempData.Object
            };
            // Arrange - create a city
            City city = new City { StateID = 1, CityID = 1, Name = "Test" };

            // Act - try to save the city
            IActionResult result = target.Edit(city);

            // Assert - check that the repository was not called
            mockitems.Verify(m => m.SaveCity(city), Times.AtLeastOnce());

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
