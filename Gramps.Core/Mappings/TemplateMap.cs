using FluentNHibernate.Mapping;
using Gramps.Core.Domain;

namespace Gramps.Core.Mappings
{
    public class TemplateMap : ClassMap<Template>
    {
        public TemplateMap()
        {
            Id(x => x.Id);

            Map(x => x.Name);
            Map(x => x.IsActive);

        }
    }
}
