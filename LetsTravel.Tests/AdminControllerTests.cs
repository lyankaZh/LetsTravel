using System;
using System.Data.Entity.Infrastructure;
using Domain.Abstract;
using LetsTravel.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace LetsTravel.Tests
{
    [TestClass]
    public class AdminControllerTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            Mock<ITravelRepository> mock = new Mock<ITravelRepository>();

            AdminController adminController = new AdminController(mock.Object);
        }
    }
}
