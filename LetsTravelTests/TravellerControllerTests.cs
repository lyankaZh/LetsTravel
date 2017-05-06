using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web.Mvc;
using Domain.Abstract;
using Domain.Entities;
using LetsTravel.Controllers;
using LetsTravel.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace LetsTravelTests
{
    [TestClass]
    public class TravellerControllerTests
    {
        Mock<ControllerContext> controllerContext = new Mock<ControllerContext>();
        Mock<IPrincipal> principal = new Mock<IPrincipal>();

        public void SetIdentityMocks()
        {
            principal.Setup(p => p.IsInRole("Traveller")).Returns(true);
            principal.SetupGet(x => x.Identity.Name).Returns("user1");
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
        }
        [TestMethod]
        public void SubscribeTest()
        {
            SetIdentityMocks();
            Mock<ITravelRepository> mock = new Mock<ITravelRepository>();
            var user = new User
            {
                UserName = "user1",
                Email = "email1@com",
                FirstName = "Jack",
                LastName = "Coper",
                Excursions = new List<Excursion>()
            };
            mock.Setup(x => x.GetUserById(It.IsAny<string>())).Returns(user);
            mock.Setup(x => x.GetExcursionById(1)).Returns(
                new Excursion()
                {
                    ExcursionId = 1,
                    Users = new List<User>()
                });

            TravellerController travellerController = new TravellerController(mock.Object);
            travellerController.ControllerContext = controllerContext.Object;
            ExcursionForTraveller exc = new ExcursionForTraveller()
            {
                ExcursionId = 1,
                City = "Lviv",
                Date = new DateTime(2017, 01, 01),
                Description = "Cool excursion",
                Duration = 4,
                PeopleLimit = 6,
                Route = "Route",
                Price = 25

            };
            var result = travellerController.Subscribe(exc);
            Assert.AreEqual(1, user.Excursions.Count);
            mock.Verify(x => x.UpdateUser(It.Is<User>(y => y.UserName == "user1")), Times.Once);
            mock.Verify(x => x.UpdateExcursion(It.Is<Excursion>(y => y.ExcursionId == 1)), Times.Once);
            Assert.AreEqual("ShowSubscribedExcursions", result.RouteValues["action"]);
        }

        [TestMethod]
        public void UnSubscribeTest()
        {
            SetIdentityMocks();
            Excursion excursion = new Excursion()
            {
                ExcursionId = 1,
                City = "Lviv",
                Date = new DateTime(2017, 01, 01),
                Description = "Cool excursion",
                Duration = 4,
                PeopleLimit = 6,
                Route = "Route",
                Price = 25,
                Guide = "guide"
            };

            User user = new User
            {
                UserName = "user1",
                Email = "email1@com",
                FirstName = "Jack",
                LastName = "Coper",
                Excursions = new List<Excursion> { excursion }
            };

            User guide = new User()
            {
                UserName = "guide",
                Id = "guide"
            };
            Mock<ITravelRepository> mock = new Mock<ITravelRepository>();
            mock.Setup(x => x.GetUserById(It.IsAny<string>())).Returns(user);

            TravellerController travellerController = new TravellerController(mock.Object);
            travellerController.ControllerContext = controllerContext.Object;

            ExcursionWithGuideInfoViewModel exc = new ExcursionWithGuideInfoViewModel
            {
                ExcursionId = 1,
                City = "Lviv",
                Date = new DateTime(2017, 01, 01),
                Description = "Cool excursion",
                Duration = 4,
                PeopleLimit = 6,
                Route = "Route",
                ModalId = "#1",
                Price = 25,
                Guide = guide
            };
            var result = travellerController.UnSubscribe(exc);
            Assert.AreEqual("ShowSubscribedExcursions", result.RouteValues["action"]);
            Assert.AreEqual(0, user.Excursions.Count);
            mock.Verify(x => x.UpdateUser(It.Is<User>(y => y.UserName == "user1")), Times.Once);

        }

        [TestMethod]
        public void ShowSubscribedTest()
        {
            SetIdentityMocks();
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
                AboutMyself = "Cool person",
                ImageData = null,
                ImageMimeType = null,
                Excursions = excursions
            };

            Mock<ITravelRepository> mock = new Mock<ITravelRepository>();
            TravellerController travellerController = new TravellerController(mock.Object)
            {
                ControllerContext = controllerContext.Object
            };
            mock.Setup(x => x.GetUserById(It.IsAny<string>())).Returns(user);
            mock.Setup(x => x.GetUsers()).Returns(new List<User>()
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
        public void ShowSubscribedForBlockedUserTest()
        {
            SetIdentityMocks();
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
                AboutMyself = "Cool person",
                ImageData = null,
                ImageMimeType = null,
                Excursions = excursions,
                BlockedUser = new BlockedUser()
            };

            Mock<ITravelRepository> mock = new Mock<ITravelRepository>();
            mock.Setup(x => x.GetUserById(It.IsAny<string>())).Returns(user);
            mock.Setup(x => x.GetBlockedUsers()).Returns(
            new List<BlockedUser> {
                new BlockedUser
            {
               User = user
            }});

            TravellerController travellerController = new TravellerController(mock.Object)
            {
                ControllerContext = controllerContext.Object
            };

            var result = (BlockedUser)travellerController.ShowSubscribedExcursions().ViewData.Model;
            Assert.AreEqual("user1", result.User.UserName);
        }

        [TestMethod]
        public void ShowSubscribedIfNoSuchTest()
        {
            SetIdentityMocks();
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
            var listOfExcursions = (List<ExcursionWithGuideInfoViewModel>)result.ViewData.Model;
            Assert.AreEqual(0, listOfExcursions.Count);
            Assert.AreEqual("You haven't subscribed to any excursion yet", result.ViewBag.NoExcursions);
        }
    }
}
