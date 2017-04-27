using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Web;
using Domain.Abstract;
using Domain.Entities;
using LetsTravel.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using System.Web.Security;
using Domain.Concrete;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Moq;

namespace LetsTravelTests
{

    [TestClass]
    public class UnitTest1
    {
        //private AppUserManager UserManager => System.Web.HttpContext.GetOwinContext().GetUserManager<AppUserManager>();

        [TestMethod]
        public void ShowUsersForAdminTest()
        {
            Mock< ITravelRepository> mock = new Mock<ITravelRepository>();
            mock.Setup(x => x.GetUsers()).Returns(new List<User>
            {
                new User {UserName = "user1", Email = "email1@com", FirstName = "Jack", LastName = "Coper"},
                new User {UserName = "user2", Email = "email2@com", FirstName = "Martin", LastName = "Denis"}
            });

            AdminController adminController = new AdminController(mock.Object);
            List<User> result = (List<User>)adminController.ShowUsersForAdmin().ViewData.Model;
            Assert.AreEqual(result.Count, 2);
            Assert.AreEqual("user1", result[0].UserName);
            Assert.AreEqual("user2", result[1].UserName);
        }
    }
}
