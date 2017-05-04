using System;
using System.Collections.Generic;
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
        //private AppUserManager UserManager => System.Web.HttpContext.GetOwinContext().GetUserManager<AppUserManager>();

        [TestMethod]
        public void ShowUsersForAdminTest()
        {
            //Mock<ITravelRepository> mock = new Mock<ITravelRepository>();
            //mock.Setup(x => x.GetUsers()).Returns(new List<User>
            //{
            //    new User {UserName = "user1", Email = "email1@com", FirstName = "Jack", LastName = "Coper"},
            //    new User {UserName = "user2", Email = "email2@com", FirstName = "Martin", LastName = "Denis"}
            //});
            //AdminController adminController = new AdminController(mock.Object);
            //List<User> result = (List<User>)adminController.ShowUsersForAdmin().ViewData.Model;
            //Assert.AreEqual(result.Count, 2);
            //Assert.AreEqual("user1", result[0].UserName);
            //Assert.AreEqual("user2", result[1].UserName);
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
        public void DeleteGuideTest()
        {

            //    Mock<AppUserManager> userManager = new Mock<AppUserManager>();
            //userManager.Setup(x => x.IsInRole("guide1","Guide"))
            //    .Returns(true);

            //List<IdentityRole> roles = new List<IdentityRole>()
            //{
            //    new IdentityRole()
            //    {
            //        Name = "Guide"
            //    }
            //};
            
            //    Mock<ITravelRepository> mock = new Mock<ITravelRepository>();
            //mock.Setup(x => x.GetUserById(It.Is<string>(i => i == "guide1")))
            //        .Returns(new User() { Id = "guide1", UserName = "guide1", FirstName = "John", LastName = "Smith", Roles = roles });

            //mock.Setup(x => x.GetExcursions()).Returns(new List<Excursion>
            //{
            //    new Excursion
            //    {
            //        ExcursionId = 1,
            //        City = "Lviv",
            //        Date =new DateTime(2017,02,02),
            //        Description = "new excursion",
            //        Duration = 6,
            //        Guide = "guide1",
            //        PeopleLimit = 4,
            //        Price = 25,
            //        Route = "new Route"
            //    }
            //});

            //AdminController adminController = new AdminController(mock.Object);
            //adminController.DeleteUser("guide1");
            //mock.Verify(x => x.DeleteUser("guide1"), Times.Once());
            //mock.Verify(x => x.DeleteExcursion(1), Times.Once());
        }

    }
}
