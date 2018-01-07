using System.Collections.Generic;
using Nest;
using SFA.DAS.Activities.Elastic;

namespace SFA.DAS.Activities.IndexMappers
{
    public class ActivitiesIndexMapper : IndexMapper<Activity>
    {
        protected override string IndexName => "activities";

        protected override void Map(TypeMappingDescriptor<Activity> mapper)
        {
            mapper.AutoMap()
                .Properties(p => p
                    .Nested<Dictionary<string, string>>(n => n
                        .Name(a => a.Data)
                        .IncludeInParent()
                    )
                );
        }
    }
}