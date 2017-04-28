using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web.Mvc;
using Domain.Abstract;
using Domain.Entities;
using LetsTravel.Controllers;
using LetsTravel.Models;
using Microsoft.AspNet.Identity;
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
                new User { UserName = "user1", Email = "email1@com", FirstName = "Jack", LastName = "Coper", Excursions = new List<Excursion>()}
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
            travellerController.Subscribe(exc);
            mock.Verify(x => x.UpdateUser(It.IsAny<User>()), Times.Once);
            mock.Verify(x => x.UpdateExcursion(It.IsAny<Excursion>()), Times.Once);
        }
        [TestMethod]
        public void UnSubscribeTest()
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
            travellerController.Subscribe(exc);
            mock.Verify(x => x.UpdateUser(It.IsAny<User>()), Times.Once);
            mock.Verify(x => x.UpdateExcursion(It.IsAny<Excursion>()), Times.Once);
        }

        [TestMethod]
        public void ShowSubscribedTest()
        {
        }
    }
}
