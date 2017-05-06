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
    public class ExcursionControllerTests
    {
        Mock<ControllerContext> controllerContext = new Mock<ControllerContext>();
        Mock<IPrincipal> principal = new Mock<IPrincipal>();
        public void SetIdentityMocks()
        {
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            principal.Setup(p => p.IsInRole("Admin")).Returns(true);
        }

        [TestMethod]
        public void GetAllExcursionsForGuestTest()
        {
            Mock<ITravelRepository> repository = new Mock<ITravelRepository>();
            var guide1 = new User {Id = "guide1", BlockedUser = new BlockedUser()};
            var guide2 = new User { Id = "guide2", BlockedUser = null };
            var excursions = new List<Excursion>
            {
                new Excursion() {ExcursionId = 1, Guide = "guide1"},
                new Excursion() {ExcursionId = 2, Guide = "guide2"},
                new Excursion() {ExcursionId = 3, Guide = "guide1"},
            };
            repository.Setup(x => x.GetExcursions()).Returns(excursions);
            repository.Setup(x => x.GetUserById("guide1")).Returns(guide1);
            repository.Setup(x => x.GetUserById("guide2")).Returns(guide2);

            ExcursionController excursionController = new ExcursionController(repository.Object);
            var result = (List<Excursion>)excursionController.GetAllExcursionsForGuest().ViewData.Model;
            Assert.AreEqual(2, result[0].ExcursionId);           
        }

        [TestMethod]
        public void GetAllExcursionsForBlockedUserTest()
        {
            SetIdentityMocks();
            principal.SetupGet(x => x.Identity.Name).Returns("traveller");
            principal.Setup(p => p.IsInRole("Admin")).Returns(false);

            Mock<ITravelRepository> repository = new Mock<ITravelRepository>();
            
            var traveller = new User {UserName = "traveller", Id = "traveller", BlockedUser = new BlockedUser()};
            var excursions = new List<Excursion>
            {
                new Excursion() {ExcursionId = 1, Guide = "guide1"},
                new Excursion() {ExcursionId = 2, Guide = "guide2"},
                new Excursion() {ExcursionId = 3, Guide = "guide1"},
            };
            repository.Setup(x => x.GetExcursions()).Returns(excursions);
            repository.Setup(x => x.GetUserById(It.IsAny<string>())).Returns(traveller);
            repository.Setup(x => x.GetBlockedUsers()).Returns(
            new List<BlockedUser> {
                new BlockedUser
            {
               User =  traveller
            }});
            repository.Setup(x => x.GetUsers()).Returns(new List<User> { traveller });
            ExcursionController excursionController = new ExcursionController(repository.Object)
            {
                ControllerContext = controllerContext.Object
            };

            var result = (BlockedUser)excursionController.GetAllExcursions().ViewData.Model;
            Assert.AreEqual("traveller", result.User.UserName);
        }

        [TestMethod]
        public void GetAllExcursionsTravellerGuideTest()
        {
            SetIdentityMocks();
            principal.Setup(p => p.IsInRole("Traveller")).Returns(true);
            principal.Setup(p => p.IsInRole("Guide")).Returns(true);
            principal.Setup(p => p.IsInRole("Admin")).Returns(false);
            principal.SetupGet(x => x.Identity.Name).Returns("user");

            Mock<ITravelRepository> repository = new Mock<ITravelRepository>();

            var user = new User { UserName = "user", Id = "user" };
            var excursions = new List<Excursion>
            {
                new Excursion() {ExcursionId = 1, Guide = "user"}
            };
            repository.Setup(x => x.GetExcursions()).Returns(excursions);
            repository.Setup(x => x.GetUserById(It.IsAny<string>())).Returns(user);
            repository.Setup(x => x.GetUsers()).Returns(new List<User>
            {
                user,
                new User {UserName = "newUser"}
            });

            ExcursionController excursionController = new ExcursionController(repository.Object)
            {
                ControllerContext = controllerContext.Object
            };

            var result = excursionController.GetAllExcursions();
            var model = (List<ExcursionForTraveller>)result.ViewData.Model;
            Assert.AreEqual(1, model[0].ExcursionId);
            Assert.AreEqual("It is your excursion", model[0].ReasonForSubscribingDisability);
            Assert.AreEqual(false, model[0].CouldBeSubscribed);
            Assert.AreEqual("AllExcursionsForTraveller", result.ViewName);
        }

        [TestMethod]
        public void GetAllExcursionsTravellerAndSubscribedExcursionTest()
        {
            SetIdentityMocks();
            principal.Setup(p => p.IsInRole("Traveller")).Returns(true);
            principal.Setup(p => p.IsInRole("Guide")).Returns(false);
            principal.Setup(p => p.IsInRole("Admin")).Returns(false);
            principal.SetupGet(x => x.Identity.Name).Returns("user");

            Mock<ITravelRepository> repository = new Mock<ITravelRepository>();

            var user = new User { UserName = "user", Id = "user", Excursions = new List<Excursion>() };
            var excursion = new Excursion() {ExcursionId = 1, Guide = "guide", Users = new List<User> {user} };
            var excursions = new List<Excursion>
            {
               excursion
            };
            user.Excursions.Add(excursion);

            repository.Setup(x => x.GetExcursions()).Returns(excursions);
            repository.Setup(x => x.GetUserById(It.IsAny<string>())).Returns(user);
            repository.Setup(x => x.GetUsers()).Returns(new List<User>
            {
                user,
                new User {UserName = "newUser"}
            });

            ExcursionController excursionController = new ExcursionController(repository.Object)
            {
                ControllerContext = controllerContext.Object
            };

            var result = excursionController.GetAllExcursions();
            var model = (List<ExcursionForTraveller>)result.ViewData.Model;
            Assert.AreEqual(1, model[0].ExcursionId);
            Assert.AreEqual("You have already subcribed", model[0].ReasonForSubscribingDisability);
            Assert.AreEqual(false, model[0].CouldBeSubscribed);
            Assert.AreEqual("AllExcursionsForTraveller", result.ViewName);
        }


        [TestMethod]
        public void GetAllExcursionsTravellerWhenNoPlaceTest()
        {
            SetIdentityMocks();
            principal.Setup(p => p.IsInRole("Traveller")).Returns(true);
            principal.Setup(p => p.IsInRole("Guide")).Returns(false);
            principal.Setup(p => p.IsInRole("Admin")).Returns(false);

            principal.SetupGet(x => x.Identity.Name).Returns("user");

            Mock<ITravelRepository> repository = new Mock<ITravelRepository>();

            var user = new User { UserName = "user", Id = "user"};
            var excursion = new Excursion()
            {
                ExcursionId = 1,
                Guide = "guide",
                PeopleLimit = 0,
                Users = new List<User>()
            };
            var excursions = new List<Excursion>
            {
               excursion
            };

            repository.Setup(x => x.GetExcursions()).Returns(excursions);
            repository.Setup(x => x.GetUserById(It.IsAny<string>())).Returns(user);
            repository.Setup(x => x.GetUsers()).Returns(new List<User>
            {
                user,
                new User {UserName = "newUser"}
            });

            ExcursionController excursionController = new ExcursionController(repository.Object)
            {
                ControllerContext = controllerContext.Object
            };

            var result = excursionController.GetAllExcursions();
            var model = (List<ExcursionForTraveller>)result.ViewData.Model;
            Assert.AreEqual(1, model[0].ExcursionId);
            Assert.AreEqual("There are no free places", model[0].ReasonForSubscribingDisability);
            Assert.AreEqual(false, model[0].CouldBeSubscribed);
            Assert.AreEqual("AllExcursionsForTraveller", result.ViewName);
        }

        [TestMethod]
        public void GetAllExcursionsForTravellerTest()
        {
            SetIdentityMocks();
            principal.Setup(p => p.IsInRole("Traveller")).Returns(true);
            principal.Setup(p => p.IsInRole("Guide")).Returns(false);
            principal.Setup(p => p.IsInRole("Admin")).Returns(false);

            principal.SetupGet(x => x.Identity.Name).Returns("user");

            Mock<ITravelRepository> repository = new Mock<ITravelRepository>();

            var user = new User { UserName = "user", Id = "user" };
            var excursion = new Excursion()
            {
                ExcursionId = 1,
                Guide = "guide",
                PeopleLimit = 6,
                Users = new List<User>()
            };
            var excursions = new List<Excursion>
            {
               excursion
            };

            repository.Setup(x => x.GetExcursions()).Returns(excursions);
            repository.Setup(x => x.GetUserById(It.IsAny<string>())).Returns(user);
            repository.Setup(x => x.GetUsers()).Returns(new List<User>
            {
                user,
                new User {UserName = "newUser"}
            });

            ExcursionController excursionController = new ExcursionController(repository.Object)
            {
                ControllerContext = controllerContext.Object
            };

            var result = excursionController.GetAllExcursions();
            var model = (List<ExcursionForTraveller>)result.ViewData.Model;
            Assert.AreEqual(1, model[0].ExcursionId);
            Assert.AreEqual(true, model[0].CouldBeSubscribed);
            Assert.AreEqual("AllExcursionsForTraveller", result.ViewName);
        }

        [TestMethod]
        public void GetAllExcursionsForGuideTest()
        {
            SetIdentityMocks();
            principal.Setup(p => p.IsInRole("Traveller")).Returns(false);
            principal.Setup(p => p.IsInRole("Guide")).Returns(true);
            principal.Setup(p => p.IsInRole("Admin")).Returns(false);
            principal.SetupGet(x => x.Identity.Name).Returns("guide1");

            Mock<ITravelRepository> repository = new Mock<ITravelRepository>();

            var guide1 = new User { Id = "guide1", UserName = "guide1"};
            var guide2 = new User { Id = "guide2", UserName = "guide2", BlockedUser = new BlockedUser() };
            var guide3 = new User { Id = "guide3", UserName = "guide2" };
            repository.Setup(x => x.GetUsers()).Returns(new List<User> {guide1, guide2, guide3});
            var excursions = new List<Excursion>
            {
                new Excursion() {ExcursionId = 1, Guide = "guide1"},
                new Excursion() {ExcursionId = 2, Guide = "guide2"},
                new Excursion() {ExcursionId = 3, Guide = "guide3"},
            };
            repository.Setup(x => x.GetExcursions()).Returns(excursions);
            repository.Setup(x => x.GetUserById("guide1")).Returns(guide1);
            repository.Setup(x => x.GetUserById("guide2")).Returns(guide2);
            repository.Setup(x => x.GetUserById("guide3")).Returns(guide3);

            ExcursionController excursionController = new ExcursionController(repository.Object)
            {
                ControllerContext = controllerContext.Object
            };

            var result = excursionController.GetAllExcursions();
            var model = (List<Excursion>)result.ViewData.Model;
            Assert.AreEqual(1, model[0].ExcursionId);
            Assert.AreEqual(3, model[1].ExcursionId);
            Assert.AreEqual("AllExcursionsForGuide", result.ViewName);
        }

        [TestMethod]
        public void GetAllExcursionsForAdminTest()
        {
            SetIdentityMocks();
            principal.Setup(p => p.IsInRole("Admin")).Returns(false);
            principal.SetupGet(x => x.Identity.Name).Returns("admin");

            Mock<ITravelRepository> repository = new Mock<ITravelRepository>();

            var guide1 = new User { Id = "guide1", UserName = "guide1" };
            var guide2 = new User { Id = "guide2", UserName = "guide2", BlockedUser = new BlockedUser() };
            var guide3 = new User { Id = "guide3", UserName = "guide2" };
            repository.Setup(x => x.GetUsers()).Returns(new List<User>
            {
                guide1, guide2, guide3 ,
                new User {UserName = "admin", Id = "admin"}
            });
            var excursions = new List<Excursion>
            {
                new Excursion() {ExcursionId = 1, Guide = "guide1"},
                new Excursion() {ExcursionId = 2, Guide = "guide2"},
                new Excursion() {ExcursionId = 3, Guide = "guide3"},
            };
            repository.Setup(x => x.GetExcursions()).Returns(excursions);
            repository.Setup(x => x.GetUserById("guide1")).Returns(guide1);
            repository.Setup(x => x.GetUserById("guide2")).Returns(guide2);
            repository.Setup(x => x.GetUserById("guide3")).Returns(guide3);

            ExcursionController excursionController = new ExcursionController(repository.Object)
            {
                ControllerContext = controllerContext.Object
            };

            var result = excursionController.GetAllExcursions();
            var model = (List<Excursion>)result.ViewData.Model;
            Assert.AreEqual(1, model[0].ExcursionId);
            Assert.AreEqual(2, model[1].ExcursionId);
            Assert.AreEqual(3, model[2].ExcursionId);
            Assert.AreEqual("AllExcursionsForAdmin", result.ViewName);
        }

    }
}
