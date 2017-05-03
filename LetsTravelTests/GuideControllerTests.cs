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
    public class GuideControllerTests
    {
        [TestMethod]
        public void CreateExcursionGetTest()
        {
            Mock<ITravelRepository> repository = new Mock<ITravelRepository>();

            GuideController controller = new GuideController(repository.Object);

            var result = controller.CreateExcursion().ViewName;
            Assert.AreEqual("CreateExcursionView", result);
        }

        [TestMethod]
        public void CreateExcursionPostTest()
        {

        }

        [TestMethod]
        public void ShowOwnExcursionsTest()
        {
            Mock<ControllerContext> controllerContext = new Mock<ControllerContext>();
            Mock<IPrincipal> principal = new Mock<IPrincipal>();
            principal.Setup(p => p.IsInRole("Guide")).Returns(true);
            principal.SetupGet(x => x.Identity.Name).Returns("guide");

            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);

            Mock<ITravelRepository> repository = new Mock<ITravelRepository>();
            var guide = new User()
            {
                Id = "guide",
                UserName = "guide"
            };

            var users = new List<User>
            {
                new User
                { UserName = "user", Email = "email1@com", FirstName = "Jack", LastName = "Coper"},
            };


            repository.Setup(x => x.GetUserById(It.IsAny<string>())).Returns(guide);
            repository.Setup(x => x.GetExcursions()).Returns(
                new List<Excursion> {
                    new Excursion
                    {
                        ExcursionId = 1,
                        Guide = "guide",
                        Users = users
                        
                    },
                    new Excursion
                    {
                        ExcursionId = 2,
                        Guide = "anotherGuide",
                        Users = users
                    },
                     new Excursion
                    {
                        ExcursionId = 3,
                        Guide = "guide",
                        Users = users
                    }
                });


            GuideController controller = new GuideController(repository.Object)
            {
                ControllerContext = controllerContext.Object
            };
            var result = (List<ExcursionModel>)controller.ShowOwnExcursions().ViewData.Model;
            Assert.AreEqual(1, result[0].ExcursionId);
            Assert.AreEqual("user", result[0].Subscribers[0].UserName);

            Assert.AreEqual(3, result[1].ExcursionId);
            Assert.AreEqual("user", result[1].Subscribers[0].UserName);

        }

        [TestMethod]
        public void ShowOwnExcursionIfNoSuchTest()
        {
            Mock<ControllerContext> controllerContext = new Mock<ControllerContext>();
            Mock<IPrincipal> principal = new Mock<IPrincipal>();
            principal.Setup(p => p.IsInRole("Guide")).Returns(true);
            principal.SetupGet(x => x.Identity.Name).Returns("guide");

            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);

            Mock<ITravelRepository> repository = new Mock<ITravelRepository>();
            var guide = new User()
            {
                Id = "guide",
                UserName = "guide"
            };

            var users = new List<User>
            {
                new User
                { UserName = "user", Email = "email1@com", FirstName = "Jack", LastName = "Coper"},
            };

            repository.Setup(x => x.GetUserById(It.IsAny<string>())).Returns(guide);
            repository.Setup(x => x.GetExcursions()).Returns(
                new List<Excursion> {
                    new Excursion
                    {
                        ExcursionId = 2,
                        Guide = "anotherGuide",
                        Users = users
                    }                  
                });

            GuideController controller = new GuideController(repository.Object)
            {
                ControllerContext = controllerContext.Object
            };

            var result = controller.ShowOwnExcursions();
            var ownExcursionsList = (List<ExcursionModel>) result.ViewData.Model;
            Assert.AreEqual(0, ownExcursionsList.Count);
            Assert.AreEqual("You don't have any excursions yet", result.ViewBag.NoExcursions);
        }

        [TestMethod]
        public void EditExcursionTest()
        {
            Mock<ITravelRepository> repository = new Mock<ITravelRepository>();

            GuideController controller = new GuideController(repository.Object);
            ExcursionModel model = new ExcursionModel
            {
                ExcursionId = 1
            };
            var result = controller.EditExcursion(model);
            Assert.AreEqual("EditExcursionView", result.ViewName);
            Assert.AreEqual(1, ((ExcursionModel)result.ViewData.Model).ExcursionId);
        }


        [TestMethod]
        public void Edit()
        {
            
        }

        [TestMethod]
        public void EditWithSubscribersTest()
        {
        }


        [TestMethod]
        public void DeleteTest()
        {
        }

        [TestMethod]
        public void DeleteWithSubscribersTest()
        {
        }
    }
}
