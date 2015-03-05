using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using FluentAssertions;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Property.Expressions.Tests
{
    public static class DateTimeTestable
    {
        public static Func<DateTime> Today = () => DateTime.Today;
    }

    [TestFixture]
    public class PersonFixture
    {
        [Test]
        public void When_Today_Is_Between_StartDate_And_EndDate_Then_IsActive_Should_Return_True()
        {
            DateTimeTestable.Today = () => new DateTime(2014, 5, 1);

            var person = new Person
            {
                Name = "Ola Normann",
                StartDate = new DateTime(2014, 1, 1),
                EndDate = new DateTime(2015, 1, 1)
            };

            person.IsActive.Should().BeTrue();
        }

        [Test]
        public void When_Linq_Querying_For_Person_When_Today_Is_Inside_Start_And_End_Then_One_Row_Should_Be_Found()
        {
            var factory = new DatabaseFactory();

            DateTimeTestable.Today = () => new DateTime(2014, 5, 1);
            using (var tx = new TransactionScope())
            using (
                var session = factory.OpenSession())
            {

                var person = new Person
                {
                    Name = "Ola Normann",
                    StartDate = new DateTime(2014, 1, 1),
                    EndDate = new DateTime(2015, 1, 1)
                };

                session.Save(person);

                tx.Complete();
            }

            using (var session = factory.OpenSession())
            {
                var list = session.QueryExtended<Person>().Where(p => p.IsActive).ToList();

                list.Should().HaveCount(1);
            }
        }

        [Test]
        public void When_Linq_Querying_For_A_Person_With_Today_Outside_Start_And_End_Then_No_Rows_Should_Be_Found()
        {
            var factory = new DatabaseFactory();

            DateTimeTestable.Today = () => new DateTime(2015, 5, 1);
            using (var tx = new TransactionScope())
            using (
                var session = factory.OpenSession())
            {

                var person = new Person
                {
                    Name = "Ola Normann",
                    StartDate = new DateTime(2014, 1, 1),
                    EndDate = new DateTime(2015, 1, 1)
                };

                session.Save(person);

                tx.Complete();
            }

            using (var session = factory.OpenSession())
            {
                var list = session.QueryExtended<Person>().Where(p => p.IsActive).ToList();

                list.Should().HaveCount(0);
            }
        }

        [Test]
        public void When_Linq_Querying_For_Any_Active_Personell_With_Inactive_Manager_One_Row_Should_Be_Found()
        {
            var factory = new DatabaseFactory();

            DateTimeTestable.Today = () => new DateTime(2015, 5, 1);
            using (var tx = new TransactionScope())
            using (
                var session = factory.OpenSession())
            {
                var manager = new Person()
                {
                    Id = Guid.NewGuid(),
                    Name = "Kari Normann",
                    StartDate = new DateTime(2014, 1, 1),
                    EndDate = new DateTime(2014, 4, 1)
                };

                session.Save(manager);

                var person = new Person
                {
                    Id = Guid.NewGuid(),
                    Name = "Ola Normann",
                    StartDate = new DateTime(2014, 1, 1),
                    EndDate = new DateTime(2016, 1, 1),
                    Manager = manager
                };

                session.Save(person);

                tx.Complete();
            }

            using (var session = factory.OpenSession())
            {
                var list = session.QueryExtended<Person>().Where(p => p.IsActive && !p.Manager.IsActive).ToList();

                list.Should().HaveCount(1);
            }
        }
    }

    [TestFixture]
    public class ExpressionResolverRegistryFixture 
    {
        
    }
}
