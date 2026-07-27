using System.Linq.Expressions;

namespace NHibernate.Property.Expressions
{
    public class VariableRenameVisitor : ExpressionVisitor
    {
        private readonly Expression _rewriteTo;
        private readonly ParameterExpression _rewriteFrom;

        public VariableRenameVisitor(Expression rewriteTo, ParameterExpression rewriteFrom)
        {
            _rewriteTo = rewriteTo;
            _rewriteFrom = rewriteFrom;
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m.Expression == _rewriteFrom)
            {
                return Expression.MakeMemberAccess(_rewriteTo, m.Member);
            }

            return base.VisitMemberAccess(m);
        }
    }
}