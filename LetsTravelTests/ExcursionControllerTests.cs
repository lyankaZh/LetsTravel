using System;
using System.Collections.Generic;
using Domain.Abstract;
using Domain.Entities;
using LetsTravel.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace LetsTravelTests
{
    [TestClass]
    public class ExcursionControllerTests
    {
       
        [TestMethod]
        public void GetAllExcursionsForGuest()
        {
          
        }

        [TestMethod]
        public void GetAllExcursions()
        {
            Mock<ITravelRepository> mock = new Mock<ITravelRepository>();
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
            ExcursionController excursionController = new ExcursionController(mock.Object);
            var result = (List<Excursion>)excursionController.GetAllExcursionsForGuest().ViewData.Model;
            Assert.AreEqual(result.Count, 1);
            Assert.AreEqual(1, result[0].ExcursionId);
            Assert.AreEqual("Lviv", result[0].City);
            Assert.AreEqual(new DateTime(2017, 2, 2), result[0].Date);
            Assert.AreEqual(25, result[0].Price);
            Assert.AreEqual(4, result[0].PeopleLimit);
            Assert.AreEqual("new Route", result[0].Route);
            Assert.AreEqual("new excursion", result[0].Description);
            Assert.AreEqual("guide1", result[0].Guide);
        }



    }
}
