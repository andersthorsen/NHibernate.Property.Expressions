using NHibernate.Engine;
using NHibernate.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace NHibernate.Property.Expressions
{
    public class ExpressionUnpackQueryProvider : DefaultQueryProvider
    {
        public ExpressionUnpackQueryProvider(ISessionImplementor session) : base(session)
        {
        }

        public override object Execute(Expression expression)
        {
            var visitor = new ReplacePropertyWithExpressionByConvention();

            var exp = visitor.Visit(expression);

            return base.Execute(exp);
        }

        public override IFutureEnumerable<TResult> ExecuteFuture<TResult>(Expression expression)
        {
            var visitor = new ReplacePropertyWithExpressionByConvention();

            var exp = visitor.Visit(expression);

            return base.ExecuteFuture<TResult>(exp);
        }

        public override IFutureValue<TResult> ExecuteFutureValue<TResult>(Expression expression)
        {
            var visitor = new ReplacePropertyWithExpressionByConvention();

            var exp = visitor.Visit(expression);

            return base.ExecuteFutureValue<TResult>(exp);
        }

        public override Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
        {
            var visitor = new ReplacePropertyWithExpressionByConvention();

            var exp = visitor.Visit(expression);

            return base.ExecuteAsync(exp, cancellationToken);
        }
    }
}