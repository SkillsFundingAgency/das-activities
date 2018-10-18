using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nest;
using PerformanceTester.Types;

namespace PerformanceTester.ElasticDb.IndexMappers
{
    public class ActivitiesIndexMapper : IndexMapper<Activity>
    {
        protected override string IndexName => "activities";

        protected override void Map(TypeMappingDescriptor<Activity> mapper)
        {
            mapper.AutoMap()
                .Properties(p =>
                    p.Nested<Dictionary<string, string>>(n => n
                            .Name(a => a.Data)
                            .IncludeInParent()
                        )
                        //TODO: set the id field as a keyword rather than text.
                        /*
                        *  this is what the mapping currently looks like:
                        *          "id": {
                                            "type": "text",
                                            "fields": {
                                              "keyword": {
                                                "type": "keyword",
                                                "ignore_above": 256
                                              }
                                            }
                                          },
                            and I want type to be "keyword". The fields including keyword
                            already should be enough but it doesn't seem to be.
                        *
                        *
                        */
                        .Keyword(f => f.Name(a => a.Id).Index(true))
                );
        }
    }
}
