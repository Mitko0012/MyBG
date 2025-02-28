using Moq;
using MyBG.Data;
using MyBG.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.Sockets;

namespace MyBG_Tests
{
    [TestClass]
    public class UnitTest
    {
        private ApplicationDbContext _dbContext;
        private Mock<UserManager<IdentityUser>> _mockUser;
        private PageController _controller;

        [TestMethod]
        public void TestSortingBySearch()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase1")
            .Options;

            _dbContext = new ApplicationDbContext(options);
            _dbContext.Pages.AddRange(new List<MyBG.Models.PageModel>
            {
                new MyBG.Models.PageModel
                {
                    Title = "CorrectPage",
                    Summary = "TestSummary",
                    TextBody = "TestBody",
                    VerifyTransport = ".",
                    Approved = true,
                    DestinationType = DestinationType.Town
                },
                new MyBG.Models.PageModel
                {
                    Title = "WrongPage",
                    Summary = "TestSummary",
                    TextBody = "TestBody",
                    VerifyTransport = ".",
                    Approved = true,
                    DestinationType = DestinationType.Town
                },
                new MyBG.Models.PageModel
                {
                    Title = "CorrectPage",
                    Summary = "TestSummary",
                    TextBody = "TestBody",
                    VerifyTransport = ".",
                    Approved = true,
                    DestinationType = DestinationType.HistoricalSite
                },
                new MyBG.Models.PageModel
                {
                    Title = "WrongPage",
                    Summary = "TestSummary",
                    TextBody = "TestBody",
                    VerifyTransport = ".",
                    Approved = true,
                    DestinationType = DestinationType.NatureSite
                }
            });
            _dbContext.SaveChanges();

            _mockUser = new Mock<UserManager<IdentityUser>>(
            Mock.Of<IUserStore<IdentityUser>>(),
            null, null, null, null, null, null, null, null
            );
            _controller = new PageController(_dbContext, _mockUser.Object);
            MyBG.Models.PageModelContainer model = ((_controller.Index("Search", "Correct", 0, (int)DestinationType.NatureSite) as ViewResult).Model) as MyBG.Models.PageModelContainer;
            Assert.AreEqual(model.Pages.Count, 2);
        }

        [TestMethod]
        public void TestSortingByType()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase2")
            .Options;

            _dbContext = new ApplicationDbContext(options);
            _dbContext.Pages.AddRange(new List<MyBG.Models.PageModel>
            {
                new MyBG.Models.PageModel
                {
                    Title = "CorrectPage",
                    Summary = "TestSummary",
                    TextBody = "TestBody",
                    VerifyTransport = ".",
                    Approved = true,
                    DestinationType = DestinationType.Town
                },
                new MyBG.Models.PageModel
                {
                    Title = "WrongPage",
                    Summary = "TestSummary",
                    TextBody = "TestBody",
                    VerifyTransport = ".",
                    Approved = true,
                    DestinationType = DestinationType.Town
                },
                new MyBG.Models.PageModel
                {
                    Title = "CorrectPage",
                    Summary = "TestSummary",
                    TextBody = "TestBody",
                    VerifyTransport = ".",
                    Approved = true,
                    DestinationType = DestinationType.HistoricalSite
                },
                new MyBG.Models.PageModel
                {
                    Title = "WrongPage",
                    Summary = "TestSummary",
                    TextBody = "TestBody",
                    VerifyTransport = ".",
                    Approved = true,
                    DestinationType = DestinationType.NatureSite
                }
            });
            _dbContext.SaveChanges();

            _mockUser = new Mock<UserManager<IdentityUser>>(
            Mock.Of<IUserStore<IdentityUser>>(),
            null, null, null, null, null, null, null, null
            );
            _controller = new PageController(_dbContext, _mockUser.Object);
            MyBG.Models.PageModelContainer model = ((_controller.Index("Destination", "Correct", 0, (int)DestinationType.NatureSite) as ViewResult).Model) as MyBG.Models.PageModelContainer;
            Assert.AreEqual(model.Pages.Count, 1);
        }
    }
}