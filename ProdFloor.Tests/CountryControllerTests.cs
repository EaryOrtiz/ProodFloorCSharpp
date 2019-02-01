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
    public class CountryControllerTests
    {
        [Fact]
        public void Index_Contains_All_Countries()
        {
            // Arrange - create the mock repository
            Mock<IItemRepository> mock = new Mock<IItemRepository>();
            mock.Setup(m => m.Countries).Returns(new Country[]
            {
                new Country {CountryID = 1, Name = "P1"},
                new Country {CountryID = 2, Name = "P2"},
                new Country {CountryID = 3, Name = "P3"},
            }.AsQueryable<Country>());

            // Arrange - create a controller
            CountryController target = new CountryController(mock.Object);

            // Action
            CountryListViewModel result = target.List(1).ViewData.Model as CountryListViewModel;
            //Country[] result = GetViewModel<CountryListViewModel>(target.List())?.ToArray();

            // Assert
            Assert.Equal(3, result.Countries.Count());
            Assert.Equal("P1", result.Countries[0].Name);
            Assert.Equal("P2", result.Countries[1].Name);
            Assert.Equal("P3", result.Countries[2].Name);
        }

        [Fact]
        public void Can_Edit_Country()
        {
            // Arrange - create the mock repository
            Mock<IItemRepository> mock = new Mock<IItemRepository>();
            mock.Setup(m => m.Countries).Returns(new Country[]
            {
                new Country {CountryID = 1, Name = "P1"},
                new Country {CountryID = 2, Name = "P2"},
                new Country {CountryID = 3, Name = "P3"},
            }.AsQueryable<Country>());

            // Arrange - create the controller
            CountryController target = new CountryController(mock.Object);

            // Act
            Country p1 = GetViewModel<Country>(target.Edit(1));
            Country p2 = GetViewModel<Country>(target.Edit(2));
            Country p3 = GetViewModel<Country>(target.Edit(3));

            // Assert
            Assert.Equal(1, p1.CountryID);
            Assert.Equal(2, p2.CountryID);
            Assert.Equal(3, p3.CountryID);
        }

        [Fact]
        public void Cannot_Edit_Nonexistent_Country()
        {
            // Arrange - create the mock repository
            Mock<IItemRepository> mock = new Mock<IItemRepository>();
            mock.Setup(m => m.Countries).Returns(new Country[]
            {
                new Country {CountryID = 1, Name = "P1"},
                new Country {CountryID = 2, Name = "P2"},
                new Country {CountryID = 3, Name = "P3"},
            }.AsQueryable<Country>());

            // Arrange - create the controller
            CountryController target = new CountryController(mock.Object);

            // Act
            Country result = GetViewModel<Country>(target.Edit(4));

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Cannot_Save_Invalid_Changes()
        {
            // Arrange - create mock repository
            Mock<IItemRepository> mock = new Mock<IItemRepository>();

            // Arrange - create the controller
            CountryController target = new CountryController(mock.Object);

            // Arrange - create a product
            Country country = new Country { Name = "Test" };

            // Arrange - add an error to the model state
            target.ModelState.AddModelError("error", "error");

            // Act - try to save the product
            IActionResult result = target.Edit(country);

            // Assert - check that the repository was not called
            mock.Verify(m => m.SaveCountry(country), Times.Never());

            // Assert - check the method result type
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Can_Delete_Valid_Countrys()
        {
            // Arrange - create a Country
            Country prod = new Country { CountryID = 2, Name = "Test" };

            // Arrange - create the mock repository
            Mock<IItemRepository> mock = new Mock<IItemRepository>();
            mock.Setup(m => m.Countries).Returns(new Country[]
            {
                new Country {CountryID = 1, Name = "P1"},
                prod,
                new Country {CountryID = 3, Name = "P3"},
            }.AsQueryable<Country>());

            // Arrange - create the controller
            CountryController target = new CountryController(mock.Object);

            // Act - delete the product
            target.Delete(prod.CountryID);

            // Assert - ensure that the repository delete method was
            // called with the correct Country
            mock.Verify(m => m.DeleteCountry(prod.CountryID));
        }

        [Fact]
        public void Can_Paginate()
        {
            // Arrange
            Mock<IItemRepository> mock = new Mock<IItemRepository>();
            mock.Setup(m => m.Countries).Returns((new Country[] {
                new Country {CountryID = 1, Name = "Mexico"},
                new Country {CountryID = 2, Name = "Paris"},
                new Country {CountryID = 3, Name = "USA"},
                new Country {CountryID = 4, Name = "Italia"},
                new Country {CountryID = 5, Name = "Polonia"}
            }).AsQueryable<Country>());
            Mock<IItemRepository> mockitems = new Mock<IItemRepository>();
            CountryController controller = new CountryController(mock.Object);
            controller.PageSize = 3;

            // Act
            CountryListViewModel result = controller.List(2).ViewData.Model as CountryListViewModel;

            // Assert
            Country[] prodArray = result.Countries.ToArray();
            Assert.True(prodArray.Length == 2);
            Assert.Equal("Italia", prodArray[0].Name);
            Assert.Equal("Polonia", prodArray[1].Name);
        }

        [Fact]
        public void Can_Send_Pagination_View_Model()
        {
            // Arrange
            Mock<IItemRepository> mockitems = new Mock<IItemRepository>();
            mockitems.Setup(m => m.Countries).Returns((new Country[] {
                new Country {CountryID = 1, Name = "P1"},
                new Country {CountryID = 2, Name = "P2"},
                new Country {CountryID = 3, Name = "P3"},
                new Country {CountryID = 4, Name = "P4"},
                new Country {CountryID = 5, Name = "P5"}
            }).AsQueryable<Country>());

            // Arrange
            CountryController controller =
            new CountryController(mockitems.Object) { PageSize = 3 };

            // Act
            CountryListViewModel result =
            controller.List(2).ViewData.Model as CountryListViewModel;

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
            CountryController target = new CountryController(mockitems.Object)
            {
                TempData = tempData.Object
            };
            // Arrange - create a country
            Country country = new Country { CountryID = 1 ,Name = "Test" };

            // Act - try to save the Country
            IActionResult result = target.Edit(country);

            // Assert - check that the repository was not called
            mockitems.Verify(m => m.SaveCountry(country), Times.AtLeastOnce());

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