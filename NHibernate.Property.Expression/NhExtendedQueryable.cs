using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq;

namespace NHibernate.Property.Expressions
{
    public class NhExtendedQueryable<T> : QueryableBase<T>
    {
        // This constructor is called by our users, create a new IQueryExecutor.
        public NhExtendedQueryable(IQueryProvider provider)
            : base(provider)
        {
        }

        // This constructor is called indirectly by LINQ's query methods, just pass to base.
        public NhExtendedQueryable(IQueryProvider provider, Expression expression)
            : base(provider, expression)
        {
        }
    }
}