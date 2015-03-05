using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NHibernate.Property.Expressions
{
    public class ReplacePropertyWithExpressionByConvention : ExpressionVisitor
    {
        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            var expression = GetExpressionField(m.Member) as LambdaExpression;
            
            if (expression != null)
            {
                var childVisitor = new VariableRenameVisitor(m.Expression, expression.Parameters.FirstOrDefault());

                var exp = childVisitor.Visit(expression.Body);

                return exp;
            }

            return base.VisitMemberAccess(m);
        }

        protected virtual LambdaExpression GetExpressionField(MemberInfo m)
        {
            if (m.MemberType != MemberTypes.Property)
                return null;

            if (m.DeclaringType == null)
                return null;

            var name = m.Name + "Expression";
            var staticMember = m.DeclaringType.GetField(name, BindingFlags.Static | BindingFlags.Public);

            if (staticMember != null)
            {
                var expression = staticMember.GetValue(null) as LambdaExpression;

                if (expression == null)
                {
                    throw new InvalidOperationException(
                        string.Format(
                            "The expression named {0} must be a lambda to be usable as a Linq expression for property {1} on type {2} ",
                            name, m.Name, m.DeclaringType.Name));
                }
                
                var p = (PropertyInfo) m;

                var expectedType = typeof (Func<,>).MakeGenericType(m.DeclaringType, p.PropertyType);

                if (!expectedType.IsAssignableFrom(expression.Type))
                {
                    throw new InvalidOperationException(
                        string.Format(
                            "The expression named {0} must be of type Func<{2},{3}>  to be usable as a Linq expression for property {1} on type {2} ",
                            name, m.Name, m.DeclaringType.Name, p.PropertyType.Name));
                }

                return expression;
            }

            return null;
        }
    }
}