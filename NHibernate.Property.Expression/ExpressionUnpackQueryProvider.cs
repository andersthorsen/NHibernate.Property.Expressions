using NHibernate.Engine;
using NHibernate.Linq;
using System.Collections.Generic;
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

        private static Expression Unpack(Expression expression)
        {
            var visitor = new ReplacePropertyWithExpressionByConvention();

            return visitor.Visit(expression);
        }

        public override object Execute(Expression expression)
        {
            return base.Execute(Unpack(expression));
        }

        public override IList<TResult> ExecuteList<TResult>(Expression expression)
        {
            return base.ExecuteList<TResult>(Unpack(expression));
        }

        public override Task<IList<TResult>> ExecuteListAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            return base.ExecuteListAsync<TResult>(Unpack(expression), cancellationToken);
        }

        public override IFutureEnumerable<TResult> ExecuteFuture<TResult>(Expression expression)
        {
            return base.ExecuteFuture<TResult>(Unpack(expression));
        }

        public override IFutureValue<TResult> ExecuteFutureValue<TResult>(Expression expression)
        {
            return base.ExecuteFutureValue<TResult>(Unpack(expression));
        }

        public override Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
        {
            return base.ExecuteAsync(Unpack(expression), cancellationToken);
        }
    }
}