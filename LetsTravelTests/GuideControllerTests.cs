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
    public class GuideControllerTests
    {
        Mock<ControllerContext> controllerContext = new Mock<ControllerContext>();
        Mock<IPrincipal> principal = new Mock<IPrincipal>();

        public void SetIdentityMocks()
        {
            principal.Setup(p => p.IsInRole("Guide")).Returns(true);
            principal.SetupGet(x => x.Identity.Name).Returns("guide");
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
        }

        [TestMethod]
        public void CreateExcursionGetTest()
        {
            SetIdentityMocks();
            Mock<ITravelRepository> repository = new Mock<ITravelRepository>();

            GuideController controller = new GuideController(repository.Object);

            var result = controller.CreateExcursion().ViewName;
            Assert.AreEqual("CreateExcursionView", result);
        }

        [TestMethod]
        public void CreateExcursionPostTest()
        {
            SetIdentityMocks();
            Mock<ITravelRepository> repository = new Mock<ITravelRepository>();

            GuideController controller = new GuideController(repository.Object)
            {
                ControllerContext = controllerContext.Object
            };

            ExcursionModel excursionModel = new ExcursionModel
            {
                City = "Lviv",
                Date = new DateTime(2017, 5, 4),
                Description = "Great excursion",
                Duration = 5,
                PeopleLimit = 10,
                Price = 60,
                Route = "Route"
            };

            var result = (RedirectToRouteResult)controller.CreateExcursion(excursionModel);
            repository.Verify(x => x.InsertExcursion(It.Is<Excursion>(y => y.City == "Lviv" &&
                              y.Description == "Great excursion" && y.Route == "Route")),
                                Times.Once);
            Assert.AreEqual("ShowOwnExcursions", result.RouteValues["action"]);
        }

        [TestMethod]
        public void CreateExcursionPostWithInvalidModelTest()
        {
            SetIdentityMocks();
            Mock<ITravelRepository> repository = new Mock<ITravelRepository>();


            GuideController controller = new GuideController(repository.Object)
            {
                ControllerContext = controllerContext.Object
            };
            controller.ModelState.AddModelError("error", "error");

            var result = (ViewResult)controller.CreateExcursion(null);
            repository.Verify(x => x.InsertExcursion(It.Is<Excursion>(y => y.City == "Lviv" &&
                              y.Description == "Great excursion" && y.Route == "Route")),
                                Times.Never);
            Assert.AreEqual("CreateExcursionView", result.ViewName);
        }

        [TestMethod]
        public void ShowOwnExcursionsTest()
        {
            SetIdentityMocks();
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
            SetIdentityMocks();
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
            var ownExcursionsList = (List<ExcursionModel>)result.ViewData.Model;
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
            SetIdentityMocks();
            Mock<ITravelRepository> repository = new Mock<ITravelRepository>();
            repository.Setup(x => x.GetExcursionById(1)).Returns(
                new Excursion
                {
                    ExcursionId = 1,
                    City = "Lviv",
                    Date = new DateTime(2017, 5, 4),
                    Description = "Great excursion",
                    Duration = 5,
                    PeopleLimit = 10,
                    Price = 60,
                    Route = "Route",
                    Users = new List<User>()
                });

            GuideController controller = new GuideController(repository.Object)
            {
                ControllerContext = controllerContext.Object
            };

            ExcursionModel excursionModel = new ExcursionModel
            {
                ExcursionId = 1,
                City = "Ternopil",
                ModalId = "#1"
            };

            var result = (RedirectToRouteResult)controller.Edit(excursionModel);
            repository.Verify(x => x.UpdateExcursion(It.Is<Excursion>(y => y.ExcursionId == 1)),
                                Times.Once);
            Assert.AreEqual("ShowOwnExcursions", result.RouteValues["action"]);
        }

        [TestMethod]
        public void EditWithInvalidModel()
        {
            SetIdentityMocks();
            Mock<ITravelRepository> repository = new Mock<ITravelRepository>();
            repository.Setup(x => x.GetExcursionById(1)).Returns(
                new Excursion
                {
                    ExcursionId = 1,
                    City = "Lviv",
                    Date = new DateTime(2017, 5, 4),
                    Description = "Great excursion",
                    Duration = 5,
                    PeopleLimit = 10,
                    Price = 60,
                    Route = "Route",
                    Users = new List<User>()
                });

            GuideController controller = new GuideController(repository.Object)
            {
                ControllerContext = controllerContext.Object
            };

            ExcursionModel excursionModel = new ExcursionModel
            {
                ExcursionId = 1,
                City = "Ternopil"
            };
            controller.ModelState.AddModelError("error", "error");

            var result = (ViewResult)controller.Edit(excursionModel);
            repository.Verify(x => x.UpdateExcursion(It.Is<Excursion>(y => y.ExcursionId == 1)),
                                Times.Never);
            Assert.AreEqual("EditExcursionView", result.ViewName);
            Assert.AreEqual(excursionModel, result.ViewData.Model);
        }

        [TestMethod]
        public void EditWithIncorrectPeopleLimitTest()
        {
            SetIdentityMocks();
            Mock<ITravelRepository> repository = new Mock<ITravelRepository>();
            repository.Setup(x => x.GetExcursionById(1)).Returns(
                new Excursion
                {
                    ExcursionId = 1,
                    City = "Lviv",
                    Date = new DateTime(2017, 5, 4),
                    Description = "Great excursion",
                    Duration = 5,
                    PeopleLimit = 10,
                    Price = 60,
                    Route = "Route",
                    Users = new List<User>
                    {
                        new User()
                    }
                });

            GuideController controller = new GuideController(repository.Object)
            {
                ControllerContext = controllerContext.Object
            };

            ExcursionModel excursionModel = new ExcursionModel
            {
                ExcursionId = 1,
                City = "Ternopil",
                PeopleLimit = 0
            };

            var result = (ViewResult)controller.Edit(excursionModel);
            repository.Verify(x => x.UpdateExcursion(It.Is<Excursion>(y => y.ExcursionId == 1)),
                                Times.Never);
            Assert.AreEqual("EditExcursionView", result.ViewName);
            Assert.AreEqual(excursionModel, result.ViewData.Model);
            Assert.AreEqual("There are more subscribers than in new limit of people value", controller.ModelState[""].Errors[0].ErrorMessage);
            
        }



        [TestMethod]
        public void DeleteExcursionWithoutSubscribersTest()
        {
            SetIdentityMocks();
            Mock<ITravelRepository> repository = new Mock<ITravelRepository>();
            repository.Setup(x => x.GetExcursionById(1)).Returns(
                new Excursion()
                {
                    ExcursionId = 1,
                    City = "Lviv",
                    Users = new List<User>() });
          
            GuideController controller = new GuideController(repository.Object)
            {
                ControllerContext = controllerContext.Object
            };

            var result = controller.DeleteExcursion(1);
            repository.Verify(x => x.DeleteExcursion(1), Times.Once);
            Assert.AreEqual("Excursion has been deleted", controller.TempData["excursionDeleted"]);
        }

        [TestMethod]
        public void DeleteNullTest()
        {
            SetIdentityMocks();
            Mock<ITravelRepository> repository = new Mock<ITravelRepository>();
            
            repository.Setup(x => x.GetExcursionById(1)).Returns((Excursion) null);

            GuideController controller = new GuideController(repository.Object)
            {
                ControllerContext = controllerContext.Object
            };

            var result = (RedirectToRouteResult)controller.DeleteExcursion(1);
            repository.Verify(x => x.DeleteExcursion(1), Times.Never);
            Assert.AreEqual("ShowOwnExcursions", result.RouteValues["action"]);
        }

        [TestMethod]
        public void DeleteExcursionWithSubscribersTest()
        {
            SetIdentityMocks();
            Mock<ITravelRepository> repository = new Mock<ITravelRepository>();
            repository.Setup(x => x.GetExcursionById(1)).Returns(
                new Excursion()
                {
                    ExcursionId = 1,
                    City = "Lviv",
                    Users = new List<User>
                    { 
                        new User()
                    }
                });

            GuideController controller = new GuideController(repository.Object)
            {
                ControllerContext = controllerContext.Object
            };

            var result = controller.DeleteExcursion(1);
            repository.Verify(x => x.DeleteExcursion(1), Times.Never);
            Assert.AreEqual("You can`t delete this excursion because it has subscribers", controller.TempData["excursionDeleted"]);
        }
    }
}
