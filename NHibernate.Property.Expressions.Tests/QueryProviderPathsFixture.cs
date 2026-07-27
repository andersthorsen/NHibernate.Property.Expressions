using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using FluentAssertions;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Property.Expressions.Tests
{
    [TestFixture]
    public class QueryProviderPathsFixture
    {
        private DatabaseFactory _factory;

        [SetUp]
        public void SetUp()
        {
            // "Today" is 2014-05-01: the seeded person below is active, the seeded manager is not.
            DateTimeTestable.Today = () => new DateTime(2014, 5, 1);

            _factory = new DatabaseFactory();

            using (var tx = new TransactionScope())
            using (var session = _factory.OpenSession())
            {
                var manager = new Person
                {
                    Id = Guid.NewGuid(),
                    Name = "Inactive Manager",
                    StartDate = new DateTime(2010, 1, 1),
                    EndDate = new DateTime(2011, 1, 1)
                };

                var active = new Person
                {
                    Id = Guid.NewGuid(),
                    Name = "Active Person",
                    StartDate = new DateTime(2014, 1, 1),
                    EndDate = new DateTime(2015, 1, 1),
                    Manager = manager
                };

                session.Save(manager);
                session.Save(active);

                tx.Complete();
            }
        }

        [TearDown]
        public void TearDown()
        {
            _factory.Dispose();
        }

        // ExecuteList<T> path
        [Test]
        public void ToList_resolves_expression_property()
        {
            using var session = _factory.OpenSession();
            session.QueryExtended<Person>().Where(p => p.IsActive).ToList().Should().HaveCount(1);
        }

        // Execute<TResult> path (scalar aggregate)
        [Test]
        public void Count_resolves_expression_property()
        {
            using var session = _factory.OpenSession();
            session.QueryExtended<Person>().Count(p => p.IsActive).Should().Be(1);
        }

        // Execute<TResult> path (Any)
        [Test]
        public void Any_resolves_expression_property()
        {
            using var session = _factory.OpenSession();
            session.QueryExtended<Person>().Any(p => p.IsActive).Should().BeTrue();
        }

        // Execute<TResult> path (single element)
        [Test]
        public void FirstOrDefault_resolves_expression_property()
        {
            using var session = _factory.OpenSession();
            session.QueryExtended<Person>().FirstOrDefault(p => p.IsActive).Should().NotBeNull();
        }

        // Nested expression property through an association
        [Test]
        public void ToList_resolves_nested_expression_property()
        {
            using var session = _factory.OpenSession();
            session.QueryExtended<Person>()
                .Where(p => p.IsActive && p.Manager != null && !p.Manager.IsActive)
                .ToList().Should().HaveCount(1);
        }

        // ExecuteListAsync<T> path
        [Test]
        public async Task ToListAsync_resolves_expression_property()
        {
            using var session = _factory.OpenSession();
            (await session.QueryExtended<Person>().Where(p => p.IsActive).ToListAsync())
                .Should().HaveCount(1);
        }

        // Known limitation: NHibernate's future batching resolves the query provider from the
        // query expression's inner constant (seeded with the default provider by QueryExtended),
        // so ToFuture/ToFutureValue bypass ExpressionUnpackQueryProvider and never run the
        // rewrite visitor. Supporting futures requires registering the provider globally via
        // Environment.QueryLinqProvider instead of the QueryExtended() wrapper.
        [Test]
        [Ignore("Futures bypass the custom query provider; tracked as a known limitation. See QueryExtended/Environment.QueryLinqProvider.")]
        public void ToFuture_resolves_expression_property()
        {
            using var session = _factory.OpenSession();
            var future = session.QueryExtended<Person>().Where(p => p.IsActive).ToFuture();
            future.ToList().Should().HaveCount(1);
        }

        [Test]
        [Ignore("Futures bypass the custom query provider; tracked as a known limitation. See QueryExtended/Environment.QueryLinqProvider.")]
        public void ToFutureValue_resolves_expression_property()
        {
            using var session = _factory.OpenSession();
            var count = session.QueryExtended<Person>().Where(p => p.IsActive).ToFutureValue(q => q.Count());
            count.Value.Should().Be(1);
        }
    }
}
