﻿namespace EntrantTests
{
    using System;
    using System.Collections.Generic;
    using Entrant.Controllers;
    using Entrant.Dals;
    using Entrant.Models;
    using NUnit.Framework;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;

    [TestFixture]
    public class EntrantControllerTests
    {

        [Test]
        public void GetAll_Returns_expectedEntrants()
        {
            var expectedEntrants = new List<Entrant>
            {
                new Entrant { Id = 1, FirstName = "First1", LastName = "Last1" },
                new Entrant { Id = 2, FirstName = "First2", LastName = "Last2" },
            };

            var dal = Substitute.For<IEntrantDal>();
            dal.GetAll().Returns(expectedEntrants);

            var controller = new EntrantController(dal);

            var action = controller.GetAll();

            Assert.That(action, Is.TypeOf<OkObjectResult>());
            var okResult = action as OkObjectResult;
            Assert.That(okResult.StatusCode.GetValueOrDefault(), Is.EqualTo(200));

            var actualEntrants = okResult.Value as List<Entrant>;
            Assert.That(actualEntrants, Is.EquivalentTo(expectedEntrants));
        }

        [Test]
        public void GetAll_Returns_EmptyCollection()
        {
            var expectedEntrants = new List<Entrant>();

            var dal = Substitute.For<IEntrantDal>();
            dal.GetAll().Returns(expectedEntrants);

            var controller = new EntrantController(dal);

            var action = controller.GetAll();

            Assert.That(action, Is.TypeOf<OkObjectResult>());
            var okResult = action as OkObjectResult;
            Assert.That(okResult.StatusCode.GetValueOrDefault(), Is.EqualTo(200));

            var actualEntrants = okResult.Value as List<Entrant>;
            Assert.That(actualEntrants, Is.Empty);
        }

        [Test]
        public void GetAll_OnException_ReturnsInternalError()
        {
            var dal = Substitute.For<IEntrantDal>();
            dal.GetAll().Throws( new Exception());

            var controller = new EntrantController(dal);

            var action = controller.GetAll();

            Assert.That(action, Is.TypeOf<StatusCodeResult>());
            var result = action as StatusCodeResult;
            Assert.That(result.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public void GetById_Returns_expectedEntrants()
        {
            var expectedEntrant = new Entrant {Id = 2, FirstName = "First2", LastName = "Last2"};

            var dal = Substitute.For<IEntrantDal>();
            dal.GetById(0).ReturnsForAnyArgs(expectedEntrant);

            var controller = new EntrantController(dal);

            var action = controller.GetById(0);

            Assert.That(action, Is.TypeOf<OkObjectResult>());
            var okResult = action as OkObjectResult;
            Assert.That(okResult.StatusCode.GetValueOrDefault(), Is.EqualTo(200));

            Assert.That(okResult.Value, Is.TypeOf<Entrant>());
            Assert.That(okResult.Value, Is.EqualTo(expectedEntrant));
        }

        [Test]
        public void GetById_ForMissingEntrant_ReturnsNotFound()
        {
            var dal = Substitute.For<IEntrantDal>();
            dal.GetById(0).Throws(new EntrantNotFoundException("message"));

            var controller = new EntrantController(dal);

            var action = controller.GetById(0);

            Assert.That(action, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public void GetById_OnException_ReturnsInternalError()
        {
            var dal = Substitute.For<IEntrantDal>();
            dal.GetById(0).Throws(new Exception());

            var controller = new EntrantController(dal);

            var getAllAction = controller.GetById(0);

            Assert.That(getAllAction, Is.TypeOf<StatusCodeResult>());
            var result = getAllAction as StatusCodeResult;
            Assert.That(result.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public void Create_Returns_expectedEntrants()
        {
            var expectedEntrant = new Entrant { FirstName = "First2", LastName = "Last2" };

            var dal = Substitute.For<IEntrantDal>();
            dal.Create(null).ReturnsForAnyArgs(expectedEntrant);

            var controller = new EntrantController(dal);

            var action = controller.Create(expectedEntrant);

            Assert.That(action, Is.TypeOf<CreatedAtActionResult>());
            var okResult = action as CreatedAtActionResult;
            Assert.That(okResult.StatusCode.GetValueOrDefault(), Is.EqualTo(201));

            Assert.That(okResult.Value, Is.TypeOf<Entrant>());
        }


        [Test]
        public void Create_ArgumentException_ReturnsBadRequest()
        {
            var dal = Substitute.For<IEntrantDal>();
            dal.Create(null).ThrowsForAnyArgs(new ArgumentException());

            var controller = new EntrantController(dal);

            var entrant = new Entrant { FirstName = "First2", LastName = "Last2" };
            var action = controller.Create(entrant);

            Assert.That(action, Is.TypeOf<BadRequestObjectResult>());
        }

        [TestCase("", "name")]
        [TestCase("name", "")]
        [TestCase(" ", "name")]
        [TestCase("name", " ")]
        [TestCase(null, "name")]
        [TestCase("name", null)]
        [TestCase("<mark up>", "name")]
        [TestCase("name", "<mark up>")]
        public void Create_BadNameArguments_ReturnsBadRequest(string firstName, string secondName)
        {
            var dal = Substitute.For<IEntrantDal>();

            var controller = new EntrantController(dal);

            var entrant = new Entrant { FirstName = firstName, LastName = secondName };
            var action = controller.Create(entrant);

            Assert.That(action, Is.TypeOf<BadRequestObjectResult>());
        }
        [Test]
        public void Create_OnException_ReturnsInternalError()
        {
            var expectedEntrant = new Entrant { FirstName = "First2", LastName = "Last2" };

            var dal = Substitute.For<IEntrantDal>();
            dal.Create(null).Throws(new Exception());

            var controller = new EntrantController(dal);

            var action = controller.Create(expectedEntrant);

            Assert.That(action, Is.TypeOf<StatusCodeResult>());
            var result = action as StatusCodeResult;
            Assert.That(result.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public void Create_OnEmptyCollection_AddsEntrant()
        {
            var dal = new EntrantDal();

            var controller = new EntrantController(dal);

            var entrant = new Entrant { FirstName = "First", LastName = "Last" };
            var action = controller.Create(entrant);
            Assert.That(action, Is.TypeOf<CreatedAtActionResult>());
        }

        [Test]
        public void Create_OnSparseCollection_AddsEntrant()
        {
            var dal = new EntrantDal();

            var controller = new EntrantController(dal);

            var entrant1 = new Entrant { FirstName = "First1", LastName = "Last1" };
            var action1 = controller.Create(entrant1);
            Assert.That(action1, Is.TypeOf<CreatedAtActionResult>());

            var entrant2 = new Entrant { FirstName = "First2", LastName = "Last2" };
            var action2 = controller.Create(entrant2);
            Assert.That(action2, Is.TypeOf<CreatedAtActionResult>());

            // create a gap by removing first entrant
            var deleteAction = controller.Delete(1);
            Assert.That(deleteAction, Is.TypeOf<OkResult>());

            var newEntrant = new Entrant { FirstName = "First2", LastName = "Last2" };
            var action = controller.Create(newEntrant);
            Assert.That(action, Is.TypeOf<CreatedAtActionResult>());
        }

        [Test]
        public void Delete_DoesNotThrow()
        {
            var dal = Substitute.For<IEntrantDal>();

            var controller = new EntrantController(dal);

            var action = controller.Delete(0);

            Assert.That(action, Is.TypeOf<OkResult>());
        }
    }
}
