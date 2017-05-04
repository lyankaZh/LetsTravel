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
    public class UserControllerTests
    {
        Mock<ControllerContext> controllerContext = new Mock<ControllerContext>();
        Mock<IPrincipal> principal = new Mock<IPrincipal>();

        public void SetIdentityMocks()
        {
            principal.SetupGet(x => x.Identity.Name).Returns("user");
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
        }

        [TestMethod]
        public void ShowProfileForGuideTest()
        {
            principal.Setup(p => p.IsInRole("Guide")).Returns(true);

            SetIdentityMocks();
            Mock<ITravelRepository> repository = new Mock<ITravelRepository>();
            repository.Setup(x => x.GetUserById(It.IsAny<string>())).Returns(
                new User
                {
                    UserName = "guide",
                    Id = "guide",
                    FirstName = "John",
                    LastName = "Snow"
                });

            repository.Setup(x => x.GetExcursions()).Returns(
                new List<Excursion> {
                    new Excursion
                    {
                        ExcursionId = 1,
                        Guide = "guide"
                    },
                    new Excursion
                    {
                        ExcursionId = 2,
                        Guide = "anotherGuide"
                    },
                     new Excursion
                    {
                        ExcursionId = 3,
                        Guide = "guide"
                    }
                });

            UserController controller = new UserController(repository.Object)
            {
                ControllerContext = controllerContext.Object
            };

            var result = (ProfileModel)controller.ShowProfile().ViewData.Model;
            Assert.AreEqual("guide",result.Id);
            Assert.AreEqual("guide", result.UserName);
            Assert.AreEqual("John", result.FirstName);
            Assert.AreEqual("Snow", result.LastName);
            Assert.AreEqual(2, result.OwnedExcursionsAmount);
            Assert.AreEqual(0, result.SubscribedExcursionsAmount);
        }

        [TestMethod]
        public void ShowProfileForTravellerTest()
        {
            principal.Setup(p => p.IsInRole("Traveller")).Returns(true);

            SetIdentityMocks();
            Mock<ITravelRepository> repository = new Mock<ITravelRepository>();

            var excursions = new List<Excursion>
            {
                new Excursion
                    {
                        ExcursionId = 1,
                        Guide = "guide"
                    },
                    new Excursion
                    {
                        ExcursionId = 2,
                        Guide = "anotherGuide"
                    },
            };
            repository.Setup(x => x.GetUserById(It.IsAny<string>())).Returns(
                new User
                {
                    UserName = "traveller",
                    Id = "traveller",
                    FirstName = "John",
                    LastName = "Snow",
                    Excursions = excursions
                });


            UserController controller = new UserController(repository.Object)
            {
                ControllerContext = controllerContext.Object
            };

            var result = (ProfileModel)controller.ShowProfile().ViewData.Model;
            Assert.AreEqual("traveller", result.Id);
            Assert.AreEqual("traveller", result.UserName);
            Assert.AreEqual("John", result.FirstName);
            Assert.AreEqual("Snow", result.LastName);
            Assert.AreEqual(0, result.OwnedExcursionsAmount);
            Assert.AreEqual(2, result.SubscribedExcursionsAmount);
        }

        [TestMethod]
        public void ShowProfileForAdminTest()
        {
            principal.Setup(p => p.IsInRole("Admin")).Returns(true);
            SetIdentityMocks();
            Mock<ITravelRepository> repository = new Mock<ITravelRepository>();
            repository.Setup(x => x.GetUserById(It.IsAny<string>())).Returns(
                new User
                {
                    UserName = "admin",
                    Id = "admin",
                    FirstName = "John",
                    LastName = "Snow",
                    Excursions = null
               });


            UserController controller = new UserController(repository.Object)
            {
                ControllerContext = controllerContext.Object
            };

            var result = (ProfileModel)controller.ShowProfile().ViewData.Model;
            Assert.AreEqual("admin", result.Id);
            Assert.AreEqual("admin", result.UserName);
            Assert.AreEqual("John", result.FirstName);
            Assert.AreEqual("Snow", result.LastName);
            Assert.AreEqual(0, result.OwnedExcursionsAmount);
            Assert.AreEqual(0, result.SubscribedExcursionsAmount);
        }

        [TestMethod]
        public void EditGetTest()
        {
            SetIdentityMocks();
            Mock<ITravelRepository> repository = new Mock<ITravelRepository>();
            repository.Setup(x => x.GetUserById(It.IsAny<string>())).Returns(
                new User
                {
                    UserName = "user",
                    BlockedUser = null
                });

            UserController controller = new UserController(repository.Object)
            {
                ControllerContext = controllerContext.Object
            };

            var result = controller.Edit().ViewName;
            Assert.AreEqual("EditProfileView", result);
        }

        [TestMethod]
        public void EditPostTest()
        {

        }

        [TestMethod]
        public void EditWithIncorrectValuesTest()
        {
        }

        [TestMethod]
        public void GetImageTest()
        {
        }

        [TestMethod]
        public void LoadImageTest()
        {
        }

        [TestMethod]
        public void DeleteTest()
        {
        }

        [TestMethod]
        public void DeleteWithActiveExcursionsTest()
        {
        }
    }
}
