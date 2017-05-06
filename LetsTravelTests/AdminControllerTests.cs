using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web.Mvc;
using Domain.Abstract;
using Domain.Entities;
using LetsTravel.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LetsTravel.Models;
using Moq;

namespace LetsTravelTests
{

    [TestClass]
    public class AdminControllerTests
    {
        Mock<ControllerContext> controllerContext = new Mock<ControllerContext>();
        Mock<IPrincipal> principal = new Mock<IPrincipal>();
        public void SetIdentityMocks()
        {
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            principal.Setup(p => p.IsInRole("Admin")).Returns(true);
        }

        [TestMethod]
        public void ShowExcursionsForAdminTest()
        {
            Mock<ITravelRepository> mock = new Mock<ITravelRepository>();
            mock.Setup(x => x.GetUserById("guide1"))
                .Returns(new User() { Id = "guide1", UserName = "guide1", FirstName = "John", LastName = "Smith" });
            mock.Setup(x => x.GetExcursions()).Returns(new List<Excursion>
            {
                new Excursion
                {
                    ExcursionId = 1,
                    City = "Lviv",
                    Date =new DateTime(2017,02,02),
                    Description = "new excursion",
                    Duration = 6,
                    Guide = "guide1",
                    PeopleLimit = 4,
                    Price = 25,
                    Route = "new Route"
                }
            });
            AdminController adminController = new AdminController(mock.Object);
            var result = (List<ExcursionModelForAdmin>)adminController.ShowExcursionsForAdmin().ViewData.Model;
            Assert.AreEqual(result.Count, 1);
            Assert.AreEqual(1, result[0].ExcursionId);
            Assert.AreEqual("Lviv", result[0].City);
            Assert.AreEqual(new DateTime(2017, 2, 2), result[0].Date);
            Assert.AreEqual(25, result[0].Price);
            Assert.AreEqual("guide1", result[0].GuideNickname);
            Assert.AreEqual("John", result[0].GuideFirstName);
            Assert.AreEqual("Smith", result[0].GuideLastName);
        }

        [TestMethod]
        public void ShowUsersForAdminTest()
        {
            SetIdentityMocks();
            Mock<ITravelRepository> mock = new Mock<ITravelRepository>();
            mock.Setup(x => x.GetUsers()).Returns(new List<User>
            {
                new User {Id = "user1", UserName = "user1", Email = "email1@com", FirstName = "Jack", LastName = "Coper"},
                new User {Id = "user2", UserName = "user2", Email = "email2@com", FirstName = "Martin", LastName = "Denis"}
            });
            mock.Setup(x => x.GetBlockedUsers()).Returns(new List<BlockedUser>
            {
                new BlockedUser { User = new User { Id = "user1"} }
            });

            AdminController adminController = new AdminController(mock.Object);
            var result = (List<UserForAdminViewModel>)adminController.ShowUsersForAdmin().ViewData.Model;
            Assert.AreEqual(result.Count, 2);
            Assert.AreEqual("user1", result[0].Nickname);
            Assert.AreEqual("user1", result[0].Id);
            Assert.AreEqual(true, result[0].IsBlocked);
            Assert.AreEqual("user2", result[1].Nickname);
            Assert.AreEqual("user2", result[1].Nickname);
            Assert.AreEqual(false, result[1].IsBlocked);
        }

        [TestMethod]
        public void DeleteGuideTest()
        {
            SetIdentityMocks();
            Mock<ITravelRepository> repository = new Mock<ITravelRepository>();
            repository.Setup(x => x.GetUserById("user")).Returns(new User { Id = "user" });
            repository.Setup(x => x.IsInRole("Guide", It.Is<User>(y => y.Id == "user"))).Returns(true);
            var travellerUser = new User { Id = "travellerUser", Excursions = new List<Excursion>() };
            var excursion =
                new Excursion
                {
                    ExcursionId = 1,
                    Guide = "user",
                    Users = new List<User> { travellerUser }
                };
            repository.Setup(x => x.GetExcursions()).Returns(new List<Excursion>
            {
                excursion,
                new Excursion
                {
                    ExcursionId = 1,
                    Guide = "user2"
                }
            });
            travellerUser.Excursions.Add(excursion);
            AdminController controller = new AdminController(repository.Object)
            {
                ControllerContext = controllerContext.Object
            };
            var result = (RedirectToRouteResult)controller.DeleteUser("user");
            repository.Verify(x => x.DeleteExcursion(1));
            repository.Verify(x => x.DeleteUser("user"));
            Assert.AreEqual(0, travellerUser.Excursions.Count);
            Assert.AreEqual("User has been deleted", controller.TempData["userDeleted"]);
            Assert.AreEqual("ShowUsersForAdmin", result.RouteValues["action"]);
        }

        [TestMethod]
        public void DeleteTravellerTest()
        {
            SetIdentityMocks();
            Mock<ITravelRepository> repository = new Mock<ITravelRepository>();
            repository.Setup(x => x.IsInRole("Traveller", It.Is<User>(y => y.Id == "user"))).Returns(true);

            var user = new User
            {
                Id = "user",
                Excursions = new List<Excursion>()
            };
            repository.Setup(x => x.GetUserById("user")).Returns(user);

            var excursion = new Excursion
            {
                ExcursionId = 1,
                Users = new List<User> { user }
            };

            user.Excursions.Add(excursion);

            AdminController controller = new AdminController(repository.Object)
            {
                ControllerContext = controllerContext.Object
            };
            var result = (RedirectToRouteResult)controller.DeleteUser("user");
            repository.Verify(x => x.DeleteUser("user"));
            Assert.AreEqual(0, user.Excursions.Count);
            Assert.AreEqual(0, excursion.Users.Count);
            Assert.AreEqual("User has been deleted", controller.TempData["userDeleted"]);
            Assert.AreEqual("ShowUsersForAdmin", result.RouteValues["action"]);
        }

        [TestMethod]
        public void DeleteExcursionTest()
        {
            SetIdentityMocks();
            Mock<ITravelRepository> repository = new Mock<ITravelRepository>();
            var user = new User { Id = "user", Excursions = new List<Excursion>() };
            var excursion = new Excursion
            {
                ExcursionId = 1,
                Users = new List<User> { user }
            };

            user.Excursions.Add(excursion);
            repository.Setup(x => x.GetExcursionById(1)).Returns(excursion);


            AdminController controller = new AdminController(repository.Object)
            {
                ControllerContext = controllerContext.Object
            };
            var result = (RedirectToRouteResult)controller.DeleteExcursion(1);
            Assert.AreEqual(0, user.Excursions.Count);
            repository.Verify(x => x.DeleteExcursion(1));
            Assert.AreEqual("Excursion has been deleted", controller.TempData["excursionDeleted"]);
            Assert.AreEqual("ShowExcursionsForAdmin", result.RouteValues["action"]);
        }

        [TestMethod]
        public void BlockUserWithValidModelTest()
        {
            SetIdentityMocks();
            Mock<ITravelRepository> repository = new Mock<ITravelRepository>();
            var user = new User { Id = "user" };
            repository.Setup(x => x.GetUserById("user")).Returns(user);
            var model = new UserForAdminViewModel
            {
                Id = "user",
                Reason = "reason"
            };

            AdminController controller = new AdminController(repository.Object)
            {
                ControllerContext = controllerContext.Object
            };
            var result = (RedirectToRouteResult)controller.Block(model);
            repository.Verify(x => x.InsertBlockedUser(It.Is<BlockedUser>(y => y.User.Id == "user")),
                            Times.Once);
            repository.Verify(x => x.Save(), Times.Once);
            Assert.AreEqual("User has been blocked", controller.TempData["userDeleted"]);
            Assert.AreEqual("ShowUsersForAdmin", result.RouteValues["action"]);
        }

        [TestMethod]
        public void BlockUserWithInvalidModelTest()
        {
            SetIdentityMocks();
            Mock<ITravelRepository> repository = new Mock<ITravelRepository>();
            var user = new User { Id = "user" };
            repository.Setup(x => x.GetUserById("user")).Returns(user);
            var model = new UserForAdminViewModel
            {
                Id = "user",
                Reason = "reason"
            };

            AdminController controller = new AdminController(repository.Object)
            {
                ControllerContext = controllerContext.Object
            };
            controller.ModelState.AddModelError("error", "error");
            var result = (RedirectToRouteResult)controller.Block(model);
            repository.Verify(x => x.InsertBlockedUser(It.Is<BlockedUser>(y => y.User.Id == "user")),
                            Times.Never);
            repository.Verify(x => x.Save(), Times.Never);
            Assert.AreEqual("ShowUsersForAdmin", result.RouteValues["action"]);
            Assert.AreEqual("Specify reason for blocking", controller.TempData["userDeleted"]);
        }

        [TestMethod]
        public void UnblockUserTest()
        {
            SetIdentityMocks();
            Mock<ITravelRepository> repository = new Mock<ITravelRepository>();

            var user = new User { Id = "user"};
            var blockedUser = new BlockedUser {User = user, BlockedUserId = 1};
            user.BlockedUser = blockedUser;

            repository.Setup(x => x.GetBlockedUsers()).Returns(
               new List<BlockedUser>
               {
                   blockedUser,
                   new BlockedUser {BlockedUserId = 2, User = new User {Id = "user2" }}
               });

            var model = new UserForAdminViewModel
            {
                Id = "user",
                Reason = "reason"
            };

            AdminController controller = new AdminController(repository.Object)
            {
                ControllerContext = controllerContext.Object
            };

            var result = (RedirectToRouteResult)controller.Unblock(model);
            repository.Verify(x => x.DeleteBlockedUser(1), Times.Once);
            repository.Verify(x => x.Save(), Times.Once);
            Assert.AreEqual("ShowUsersForAdmin", result.RouteValues["action"]);
            Assert.AreEqual("User has been unblocked", controller.TempData["userDeleted"]);
        }
    }
}
