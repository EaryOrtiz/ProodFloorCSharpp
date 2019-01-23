using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Routing;
using Moq;
using ProdFloor.Components;
using ProdFloor.Models;
using Xunit;

namespace ProdFloor.Tests
{
    public class NavigationMenuViewComponentTests
    {
        [Fact]
        public void Can_Select_Categories()
        {
            // Arrange
            Mock<IJobRepository> mock = new Mock<IJobRepository>();
            mock.Setup(m => m.Jobs).Returns((new Job[] {
                new Job {JobID = 1, Name = "P1", JobType = "Apples"},
                new Job {JobID = 2, Name = "P2", JobType = "Apples"},
                new Job {JobID = 3, Name = "P3", JobType = "Plums"},
                new Job {JobID = 4, Name = "P4", JobType = "Oranges"},
            }).AsQueryable<Job>());
            NavigationMenuViewComponent target =
            new NavigationMenuViewComponent(mock.Object);
            // Act = get the set of categories
            string[] results = ((IEnumerable<string>)(target.Invoke()
            as ViewViewComponentResult).ViewData.Model).ToArray();
            // Assert
            Assert.True(Enumerable.SequenceEqual(new string[] { "Apples",
                "Oranges", "Plums" }, results));
        }

        [Fact]
        public void Indicates_Selected_Category()
        {
            // Arrange
            string JobTypeToSelect = "Apples";
            Mock<IJobRepository> mock = new Mock<IJobRepository>();
            mock.Setup(m => m.Jobs).Returns((new Job[] {
                new Job {JobID = 1, Name = "P1", JobType = "Apples"},
                new Job {JobID = 4, Name = "P2", JobType = "Oranges"},
            }).AsQueryable<Job>());

            NavigationMenuViewComponent target =
            new NavigationMenuViewComponent(mock.Object);

            target.ViewComponentContext = new ViewComponentContext
            {
                ViewContext = new ViewContext
                {
                    RouteData = new RouteData()
                }
            };
            target.RouteData.Values["JobType"] = JobTypeToSelect;

            // Action
            string result = (string)(target.Invoke() as
            ViewViewComponentResult).ViewData["SelectedJobType"];

            // Assert
            Assert.Equal(JobTypeToSelect, result);
        }
    }
}
