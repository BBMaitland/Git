namespace EntrantTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Entrant.Dals;
    using Entrant.Models;
    using NUnit.Framework;

    [TestFixture]
    public class EntrantDalTests
    {
        [Test]
        public void Create_WithNullEntrant_ThrowsArgumentNullException()
        {
            var dal = new EntrantDal();
            Assert.Throws<ArgumentNullException>(() => dal.Create(null));
        }

        [TestCase("f", null)]
        [TestCase("f", "")]
        [TestCase("f", " ")]
        [TestCase(null, "l")]
        [TestCase("", "l")]
        [TestCase(" ", "l")]
        public void Create_WithBlankNames_ThrowsArgumentExceptionException(string firstName, string lastName)
        {
            var entrantMap = new Dictionary<int, Entrant>();
            var dal = new EntrantDal(entrantMap);

            var entrant = new Entrant {FirstName = firstName, LastName = lastName};

            Assert.Throws<ArgumentException>(() => dal.Create(entrant));
            Assert.That(entrantMap, Is.Empty);
        }

        [Test]
        public void Create_WithValidEntrant_AddToMap()
        {
            var entrantMap = new  Dictionary<int, Entrant>();
            var dal = new EntrantDal(entrantMap);
            var entrant = new Entrant { FirstName = "firstName", LastName = "lastName" };

            dal.Create(entrant);
            Assert.That(entrantMap.Count, Is.EqualTo(1));

            var firstItem = entrantMap.First().Value;
            Assert.That(firstItem.Id, Is.EqualTo(1));
        }

        [Test]
        public void Delete_WithValidId_RemoveItem()
        {
            var entrant = new Entrant { FirstName = "firstName", LastName = "lastName", Id=1 };
            var entrantMap = new Dictionary<int, Entrant>
            {
                { entrant.Id, entrant }
            };

            var dal = new EntrantDal(entrantMap);
            dal.Delete(entrant.Id);

            Assert.That(entrantMap, Is.Empty);
        }

        [Test]
        public void Delete_WithBadId_ThrowsNotFoundException()
        {
            var entrant = new Entrant { FirstName = "firstName", LastName = "lastName", Id = 1 };
            var entrantMap = new Dictionary<int, Entrant>
            {
                { entrant.Id, entrant }
            };

            var dal = new EntrantDal(entrantMap);
            Assert.Throws<EntrantNotFoundException>(() => dal.Delete(9));

            Assert.That(entrantMap.Count, Is.EqualTo(1));

            var firstItem = entrantMap.First().Value;
            Assert.That(firstItem.Id, Is.EqualTo(1));
        }

        [Test]
        public void GetAll_Returns_ExpectEntrants()
        {
            var entrantMap = new Dictionary<int, Entrant>
            {
                [1] = new Entrant { FirstName = "firstName", LastName = "lastName", Id = 1 },
                [2] = new Entrant { FirstName = "firstName2", LastName = "lastName2", Id = 2 },
            };

            var dal = new EntrantDal(entrantMap);

            var items = dal.GetAll();
            Assert.That(items, Is.EquivalentTo(entrantMap.Values.ToList()));
        }

        [Test]
        public void GetAll_Returns_EmptyList()
        {
            var dal = new EntrantDal();

            var items = dal.GetAll();
            Assert.That(items, Is.Empty);
        }

        [Test]
        public void GetById_WithValidId_ReturnsExpectedItem()
        {
            var entrantMap = new Dictionary<int, Entrant>
            {
                [1] = new Entrant { FirstName = "firstName", LastName = "lastName", Id = 1 },
                [2] = new Entrant { FirstName = "firstName2", LastName = "lastName2", Id = 2 },
            };

            var dal = new EntrantDal(entrantMap);

            var item = dal.GetById(1);

            Assert.That(item, Is.EqualTo(entrantMap[1]));
        }

        [Test]
        public void GetById_WithBadId_ThrowsNotFoundException()
        {
            var entrantMap = new Dictionary<int, Entrant>
            {
                [1] = new Entrant { FirstName = "firstName", LastName = "lastName", Id = 1 },
                [2] = new Entrant { FirstName = "firstName2", LastName = "lastName2", Id = 2 },
            };

            var dal = new EntrantDal(entrantMap);

            Assert.Throws<EntrantNotFoundException>(() => dal.GetById(4));
        }
    }
}
