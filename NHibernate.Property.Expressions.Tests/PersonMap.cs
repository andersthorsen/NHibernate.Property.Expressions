using NHibernate.Mapping.ByCode.Conformist;

namespace NHibernate.Property.Expressions.Tests
{
    public class PersonMap : ClassMapping<Person>
    {
        public PersonMap()
        {
            Id(p => p.Id);
            Property(p => p.Name);
            Property(p => p.StartDate);
            Property(p => p.EndDate);
            ManyToOne(p => p.Manager);
        }
    }
}
