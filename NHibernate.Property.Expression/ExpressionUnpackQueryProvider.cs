using System.Linq.Expressions;
using NHibernate.Engine;
using NHibernate.Linq;

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

        public override object ExecuteFuture(Expression expression)
        {
            var visitor = new ReplacePropertyWithExpressionByConvention();

            var exp = visitor.Visit(expression);

            return base.ExecuteFuture(exp);
        }
    }
}