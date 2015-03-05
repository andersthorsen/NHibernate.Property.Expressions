using System;
using System.Linq.Expressions;

namespace NHibernate.Property.Expressions.Tests
{
    public class Person
    {
        public virtual Guid Id { get; set; }
        public virtual string Name { get; set; }
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual Person Manager { get; set; }

        public static Expression<Func<Person, bool>> IsActiveExpression =
            person => person.StartDate <= DateTimeTestable.Today() && (person.EndDate == null || person.EndDate >= DateTimeTestable.Today());

        public static Func<Person, bool> CompiledIsActive = IsActiveExpression.Compile(); 

        public virtual bool IsActive { get { return CompiledIsActive(this); } }
    }
}
