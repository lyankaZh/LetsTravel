using System.Collections.Generic;
using System.Security.Principal;
using System.Web;
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
            Assert.AreEqual("guide", result.Id);
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
        public void EditPostInvalidModelTest()
        {
            SetIdentityMocks();
            Mock<ITravelRepository> repository = new Mock<ITravelRepository>();
            UserController controller = new UserController(repository.Object)
            {
                ControllerContext = controllerContext.Object
            };
            controller.ModelState.AddModelError("error", "error");
            var result = (RedirectToRouteResult)controller.Edit(null);
            Assert.AreEqual("Edit", result.RouteValues["action"]);
        }

        [TestMethod]
        public void EditPostTest()
        {
            SetIdentityMocks();
            Mock<ITravelRepository> repository = new Mock<ITravelRepository>();
            var user = new User
            {
                Id = "user",
                UserName = "user",
                ImageData = new byte[] { },
                ImageMimeType = "image/png"
            };
            repository.Setup(x => x.GetUserById("user")).Returns(user);
            repository.Setup(x => x.GetUsers()).Returns(
                new List<User>
                {
                     new User
                    {
                        Id = "user",
                        UserName = "user",
                        Email = "user@mail"
                    },
                    new User
                    {
                       Id = "user2",
                       UserName = "user2",
                        Email = "user2@mail"
                    },
                    new User
                    {
                        Id = "user3",
                        UserName = "user3",
                        Email = "user3@mail"
                    }
                }
            );
            UserController controller = new UserController(repository.Object)
            {
                ControllerContext = controllerContext.Object
            };

            var resultWithAbout = (RedirectToRouteResult)controller.Edit(
                new User
                {
                    Id = "user",
                    UserName = "user",
                    Email = "newEmail",
                    AboutMyself = "About"
                });

            var resultWithoutAbout = (RedirectToRouteResult)controller.Edit(
                new User
                {
                    Id = "user",
                    UserName = "user",
                    Email = "newEmail"
                });
            repository.Verify(x => x.UpdateUser(user), Times.Exactly(2));
            Assert.AreEqual("ShowProfile", resultWithAbout.RouteValues["action"]);
            Assert.AreEqual("ShowProfile", resultWithoutAbout.RouteValues["action"]);
        }

        [TestMethod]
        public void EditPostWithImageTest()
        {
            SetIdentityMocks();
            Mock<ITravelRepository> repository = new Mock<ITravelRepository>();
            var user = new User
            {
                Id = "user",
                UserName = "user"
            };
            repository.Setup(x => x.GetUserById("user")).Returns(user);
            repository.Setup(x => x.GetUsers()).Returns(
                new List<User>
                {
                     new User
                    {
                        Id = "user",
                        UserName = "user",
                        Email = "user@mail"
                    },
                    new User
                    {
                       Id = "user2",
                       UserName = "user2",
                        Email = "user2@mail"
                    },
                    new User
                    {
                        Id = "user3",
                        UserName = "user3",
                        Email = "user3@mail"
                    }
                }
            );
            UserController controller = new UserController(repository.Object)
            {
                ControllerContext = controllerContext.Object
            };

            Mock<HttpPostedFileBase> image = new Mock<HttpPostedFileBase>();
            image.Setup(x => x.FileName).Returns("image.jpg");
            image.Setup(x => x.ContentType).Returns("image/jpeg");
            image.Setup(x => x.ContentLength).Returns(0);
            image.Setup(x => x.InputStream.Read(null, 0, 0)).Returns(null);
            var result = (RedirectToRouteResult)controller.Edit(
                new User
                {
                    Id = "user",
                    UserName = "user",
                    Email = "newEmail",
                    AboutMyself = "About"
                }, image.Object);
            repository.Verify(x => x.UpdateUser(user), Times.Once);
            Assert.AreEqual("ShowProfile", result.RouteValues["action"]);
        }
        [TestMethod]
        public void EditPostWithWrongUsernameTest()
        {
            SetIdentityMocks();
            Mock<ITravelRepository> repository = new Mock<ITravelRepository>();
            var user = new User
            {
                Id = "user",
                UserName = "user"
            };
            repository.Setup(x => x.GetUserById("user")).Returns(user);
            repository.Setup(x => x.GetUsers()).Returns(
                new List<User>
                {
                     new User
                    {
                        Id = "user",
                        UserName = "user",
                        Email = "user@mail"
                    },
                    new User
                    {
                       Id = "user2",
                       UserName = "user2",
                        Email = "user2@mail"
                    },
                    new User
                    {
                        Id = "user3",
                        UserName = "user3",
                        Email = "user3@mail"
                    }
                }
            );
            UserController controller = new UserController(repository.Object)
            {
                ControllerContext = controllerContext.Object
            };

            var result = (RedirectToRouteResult)controller.Edit(
                new User
                {
                    Id = "user",
                    UserName = "user2",
                    Email = "newEmail",
                    AboutMyself = "About"
                });

            repository.Verify(x => x.UpdateUser(user), Times.Never);
            Assert.AreEqual("Edit", result.RouteValues["action"]);
            Assert.AreEqual("Such nickname already exists", controller.ModelState[""].Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void EditPostWithWrongEmailTest()
        {
            SetIdentityMocks();
            Mock<ITravelRepository> repository = new Mock<ITravelRepository>();
            var user = new User
            {
                Id = "user",
                UserName = "user"
            };
            repository.Setup(x => x.GetUserById("user")).Returns(user);
            repository.Setup(x => x.GetUsers()).Returns(
                new List<User>
                {
                     new User
                    {
                        Id = "user",
                        UserName = "user",
                        Email = "user@mail"
                    },
                    new User
                    {
                       Id = "user2",
                       UserName = "user2",
                        Email = "user2@mail"
                    },
                    new User
                    {
                        Id = "user3",
                        UserName = "user3",
                        Email = "user3@mail"
                    }
                }
            );
            UserController controller = new UserController(repository.Object)
            {
                ControllerContext = controllerContext.Object
            };

            var result = (RedirectToRouteResult)controller.Edit(
                new User
                {
                    Id = "user",
                    UserName = "user",
                    Email = "user2@mail",
                    AboutMyself = "About"
                });

            repository.Verify(x => x.UpdateUser(user), Times.Never);
            Assert.AreEqual("Edit", result.RouteValues["action"]);
            Assert.AreEqual("Such email already exists", controller.ModelState[""].Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void GetImageTest()
        {
            SetIdentityMocks();
            Mock<ITravelRepository> repository = new Mock<ITravelRepository>();
            var user = new User
            {
                Id = "user",
                UserName = "user",
                ImageData = new byte[] { },
                ImageMimeType = "image/png"
            };
            repository.Setup(x => x.GetUserById("user")).Returns(user);
            UserController controller = new UserController(repository.Object)
            {
                ControllerContext = controllerContext.Object
            };

            ActionResult result = controller.GetImage("user");

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(FileResult));
            Assert.AreEqual(user.ImageMimeType, ((FileResult)result).ContentType);
        }

        [TestMethod]
        public void GetImageForIncorrectIdTest()
        {
            SetIdentityMocks();
            Mock<ITravelRepository> repository = new Mock<ITravelRepository>();
            var user = new User
            {
                Id = "user",
                UserName = "user",
                ImageData = new byte[] { },
                ImageMimeType = "image/png"
            };
            repository.Setup(x => x.GetUserById("user")).Returns(user);
            UserController controller = new UserController(repository.Object)
            {
                ControllerContext = controllerContext.Object
            };

            ActionResult result = controller.GetImage("user2");

            Assert.IsNull(result);
        }

        [TestMethod]
        public void DeleteGuideTest()
        {
            SetIdentityMocks();
            principal.Setup(p => p.IsInRole("Traveller")).Returns(false);
            principal.Setup(p => p.IsInRole("Guide")).Returns(true);
            Mock<ITravelRepository> repository = new Mock<ITravelRepository>();
            repository.Setup(x => x.GetUserById("user")).Returns(
                new User
                {
                    UserName = "user",
                    Excursions = new List<Excursion>()
                });
            var excursions = new List<Excursion>
            {
                new Excursion
                { ExcursionId = 1, Guide = "user", Users = new List<User>()},
                new Excursion {Guide = "user2", Users = new List<User>()},
                new Excursion {Guide = "user3", Users = new List<User>()}
            };
            repository.Setup(x => x.GetExcursions()).Returns(excursions);
            UserController controller = new UserController(repository.Object)
            {
                ControllerContext = controllerContext.Object
            };
            var result = (RedirectToRouteResult)controller.Delete("user");
            repository.Verify(x => x.DeleteExcursion(1), Times.Once);
            repository.Verify(x => x.DeleteUser("user"), Times.Once);
            repository.Verify(x => x.Save(), Times.Once);
            Assert.AreEqual("Logout", result.RouteValues["action"]);
            Assert.AreEqual("Account", result.RouteValues["controller"]);
        }

        [TestMethod]
        public void DeleteGuideWithSubscribersToExcursionsTest()
        {
            SetIdentityMocks();
            principal.Setup(p => p.IsInRole("Traveller")).Returns(false);
            principal.Setup(p => p.IsInRole("Guide")).Returns(true);
            Mock<ITravelRepository> repository = new Mock<ITravelRepository>();
            repository.Setup(x => x.GetUserById("user")).Returns(
                new User
                {
                    UserName = "user",
                    Excursions = new List<Excursion>()
                });
            var excursions = new List<Excursion>
            {
                new Excursion
                { ExcursionId = 1, Guide = "user", Users = new List<User> { new User()}},
                new Excursion {Guide = "user2", Users = new List<User>()},
                new Excursion {Guide = "user3", Users = new List<User>()}
            };
            repository.Setup(x => x.GetExcursions()).Returns(excursions);
            UserController controller = new UserController(repository.Object)
            {
                ControllerContext = controllerContext.Object
            };
            var result = (RedirectToRouteResult)controller.Delete("user");
            repository.Verify(x => x.DeleteExcursion(1), Times.Never);
            repository.Verify(x => x.DeleteUser("user"), Times.Never);
            repository.Verify(x => x.Save(), Times.Never);
            Assert.AreEqual("ShowProfile", result.RouteValues["action"]);
            Assert.AreEqual("You can`t delete your profile because you have active excursions with subscribers",
                            controller.TempData["deleteGuideErrorMessage"]);
        }

        [TestMethod]
        public void DeleteTravellerTest()
        {
            SetIdentityMocks();
            principal.Setup(p => p.IsInRole("Traveller")).Returns(true);
            principal.Setup(p => p.IsInRole("Guide")).Returns(false);
            Mock<ITravelRepository> repository = new Mock<ITravelRepository>();
            repository.Setup(x => x.GetUserById("user")).Returns(
                new User
                {
                    UserName = "user",
                    Excursions = new List<Excursion>()
                });

            UserController controller = new UserController(repository.Object)
            {
                ControllerContext = controllerContext.Object
            };
            var result = (RedirectToRouteResult)controller.Delete("user");
            repository.Verify(x => x.DeleteUser("user"), Times.Once);
            repository.Verify(x => x.Save(), Times.Once);
            Assert.AreEqual("Logout", result.RouteValues["action"]);
            Assert.AreEqual("Account", result.RouteValues["controller"]);
        }

        [TestMethod]
        public void DeleteTravellerWithActiveExcursionsTest()
        {
            SetIdentityMocks();
            principal.Setup(p => p.IsInRole("Traveller")).Returns(true);
            principal.Setup(p => p.IsInRole("Guide")).Returns(false);
            Mock<ITravelRepository> repository = new Mock<ITravelRepository>();
            var excursions = new List<Excursion>
            {
                new Excursion()
            };
            repository.Setup(x => x.GetUserById("user")).Returns(
                new User
                {
                    UserName = "user",
                    Excursions = excursions
                });

            UserController controller = new UserController(repository.Object)
            {
                ControllerContext = controllerContext.Object
            };
            var result = (RedirectToRouteResult)controller.Delete("user");
           
            repository.Verify(x => x.DeleteUser("user"), Times.Never);
            repository.Verify(x => x.Save(), Times.Never);
            Assert.AreEqual("ShowProfile", result.RouteValues["action"]);
            Assert.AreEqual("Before deleting profile unsubscribe from all excursions", 
                            controller.TempData["deleteTravellerErrorMessage"]);
        }
    }
}
