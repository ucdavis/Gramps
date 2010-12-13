using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Gramps.Core.Domain
{
// ReSharper disable InconsistentNaming
    public class User : DomainObject
// ReSharper restore InconsistentNaming
    {
        public virtual string LoginId { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Email { get; set; }

        public virtual string FullName
        {
            get { return FirstName + " " + LastName; }
        }
    }

// ReSharper disable InconsistentNaming
    public class UserMap : ClassMap<User>
// ReSharper restore InconsistentNaming
    {
        public UserMap()
        {
            Table("vUsers");
            ReadOnly();

            Id(x => x.Id);

            Map(x => x.LoginId);
            Map(x => x.FirstName);
            Map(x => x.LastName);
            Map(x => x.Email);
        }
    }
}
