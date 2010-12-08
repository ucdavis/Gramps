using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace Gramps.Core.Domain
{
    public class Template : DomainObject
    {
        [Required]
        [Length(100)]
        public virtual string Name { get; set; }
        public virtual bool IsActive { get; set; }
    }
}