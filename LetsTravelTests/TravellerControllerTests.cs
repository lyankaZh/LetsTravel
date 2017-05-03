using System.Collections.Generic;
using System.Security.Principal;
using System.Web.Mvc;
using Domain.Abstract;
using Domain.Entities;
using LetsTravel.Controllers;
using LetsTravel.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace LetsTravelTests
{
    [TestClass]
    public class TravellerControllerTests
    {
        [TestMethod]
        public void SubscribeTest()
        {
            var controllerContext = new Mock<ControllerContext>();
            var principal = new Mock<IPrincipal>();
            principal.Setup(p => p.IsInRole("Traveller")).Returns(true);
            principal.SetupGet(x => x.Identity.Name).Returns("user1");
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);

            Mock<ITravelRepository> mock = new Mock<ITravelRepository>();
            mock.Setup(x => x.GetUserById(It.IsAny<string>())).Returns(
                new User { UserName = "user1", Email = "email1@com", FirstName = "Jack", LastName = "Coper", Excursions = new List<Excursion>() }
            );
            mock.Setup(x => x.GetExcursionById(1)).Returns(
                new Excursion()
                {
                    ExcursionId = 1,
                    City = "Lviv",
                    Users = new List<User>()
                });

            TravellerController travellerController = new TravellerController(mock.Object);
            travellerController.ControllerContext = controllerContext.Object;
            ExcursionForTraveller exc = new ExcursionForTraveller()
            {
                ExcursionId = 1,
                City = "Lviv"
            };
            var result = travellerController.Subscribe(exc);
            mock.Verify(x => x.UpdateUser(It.IsAny<User>()), Times.Once);
            mock.Verify(x => x.UpdateExcursion(It.IsAny<Excursion>()), Times.Once);
            Assert.AreEqual("ShowSubscribedExcursions", result.RouteValues["action"]);
        }

        [TestMethod]
        public void UnSubscribeTest()
        {
            var controllerContext = new Mock<ControllerContext>();
            var principal = new Mock<IPrincipal>();
            principal.Setup(p => p.IsInRole("Traveller")).Returns(true);
            principal.SetupGet(x => x.Identity.Name).Returns("user1");
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);

            Excursion excursion = new Excursion()
            {
                ExcursionId = 1,
                City = "Lviv"
            };

            User user = new User
            {
                UserName = "user1",
                Email = "email1@com",
                FirstName = "Jack",
                LastName = "Coper",
                Excursions = new List<Excursion> { excursion }
            };

            Mock<ITravelRepository> mock = new Mock<ITravelRepository>();
            mock.Setup(x => x.GetUserById(It.IsAny<string>())).Returns(user);

            TravellerController travellerController = new TravellerController(mock.Object);
            travellerController.ControllerContext = controllerContext.Object;

            ExcursionWithGuideInfoViewModel exc = new ExcursionWithGuideInfoViewModel
            {
                ExcursionId = 1,
                City = "Lviv"
            };
            var result = travellerController.UnSubscribe(exc);
            Assert.AreEqual("ShowSubscribedExcursions", result.RouteValues["action"]);
            mock.Verify(x => x.UpdateUser(It.IsAny<User>()), Times.Once);

        }

        [TestMethod]
        public void ShowSubscribedTest()
        {
            var controllerContext = new Mock<ControllerContext>();
            var principal = new Mock<IPrincipal>();
            principal.Setup(p => p.IsInRole("Traveller")).Returns(true);
            principal.SetupGet(x => x.Identity.Name).Returns("user1");
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);

            List<Excursion> excursions = new List<Excursion>
            {
                new Excursion()
                {
                    ExcursionId = 1,
                    City = "Lviv",
                    Guide = "guideId1"
                },
                 new Excursion()
                {
                    ExcursionId = 2,
                    City = "Ternopil",
                    Guide = "guideId2"
                }
            };

            User user = new User
            {
                UserName = "user1",
                Email = "email1@com",
                FirstName = "Jack",
                LastName = "Coper",
                Excursions = excursions
            };

            Mock<ITravelRepository> mock = new Mock<ITravelRepository>();
            TravellerController travellerController = new TravellerController(mock.Object)
            {
                ControllerContext = controllerContext.Object
            };
            mock.Setup(x => x.GetUserById(It.IsAny<string>())).Returns(user);
            mock.Setup(x => x.GetUsers()).Returns(new List<IdentityUser>()
            {
                user,
                new User
                {
                    Id = "guideId1",
                    UserName = "guide1"
                },
                new User
                {
                    Id = "guideId2",
                    UserName = "guide2"
                }
            });

            var result = (List<ExcursionWithGuideInfoViewModel>)travellerController.ShowSubscribedExcursions().ViewData.Model;
            Assert.AreEqual(1, result[0].ExcursionId);
            Assert.AreEqual("Lviv", result[0].City);
            Assert.AreEqual("guideId1", result[0].Guide.Id);
            Assert.AreEqual("guide1", result[0].Guide.UserName);

            Assert.AreEqual(2, result[1].ExcursionId);
            Assert.AreEqual("Ternopil", result[1].City);
            Assert.AreEqual("guideId2", result[1].Guide.Id);
            Assert.AreEqual("guide2", result[1].Guide.UserName);
        }

        [TestMethod]
        public void ShowSubscribedIfNoSuchTest()
        {
            var controllerContext = new Mock<ControllerContext>();
            var principal = new Mock<IPrincipal>();
            principal.Setup(p => p.IsInRole("Traveller")).Returns(true);
            principal.SetupGet(x => x.Identity.Name).Returns("user1");
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);

            User user = new User
            {
                UserName = "user1",
                Email = "email1@com",
                FirstName = "Jack",
                LastName = "Coper",
                Excursions = new List<Excursion>()
            };

            Mock<ITravelRepository> mock = new Mock<ITravelRepository>();
            TravellerController travellerController = new TravellerController(mock.Object)
            {
                ControllerContext = controllerContext.Object
            };
            mock.Setup(x => x.GetUserById(It.IsAny<string>())).Returns(user);
          

            var result = travellerController.ShowSubscribedExcursions();
            var listOfExcursions = (List<ExcursionWithGuideInfoViewModel>) result.ViewData.Model;
            Assert.AreEqual(0, listOfExcursions.Count);   
            Assert.AreEqual("You haven't subscribed to any excursion yet", result.ViewBag.NoExcursions);
        }
    }
}
