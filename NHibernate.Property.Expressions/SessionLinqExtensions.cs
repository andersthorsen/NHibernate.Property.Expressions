using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq;

namespace NHibernate.Property.Expressions
{
    public static class SessionLinqExtensions
    {
        public static IQueryable<T> QueryExtended<T>(this ISession session)
        {
            return new NhQueryable<T>(new ExpressionUnpackQueryProvider(session.GetSessionImplementation()), Expression.Constant(new NhQueryable<T>(session.GetSessionImplementation())));
        }
    }
}