using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Nest;
using NuGet;

namespace MessagingConsole
{
    class Program
    {
        private static ElasticClient elasticClient;
        static void Main(string[] args)
        {
            var indexName = "activitiestest";
            //var elasticSettings = new ConnectionSettings(new Uri(configuration.ElasticServerBaseUrl));
            var elasticSettings = new ConnectionSettings(new Uri("http://localhost:9200")).DefaultIndex(indexName);
            elasticClient = new ElasticClient(elasticSettings);

            elasticClient.DeleteIndex(indexName);

            CreateIndex(indexName);

            var activities = MakeSomeActivities();

            //elasticClient.IndexMany(activities, "activitiestest");

            foreach (var activity in activities)
            {
                elasticClient.Index(activity);
            }

        }

        private static void CreateIndex(string indexName)
        {
            elasticClient.CreateIndex(indexName, i => i.Mappings(ms => ms.Map<Activity>(m => m.AutoMap())));
        }

        private static List<Activity> MakeSomeActivities()
        {
            List<Activity> rtn = new List<Activity>
            {
                MakeActivity(Activity.ActivityType.ActivityOne),
                MakeActivity(Activity.ActivityType.ActivityOne),
                MakeActivity(Activity.ActivityType.ActivityOne),
                MakeActivity(Activity.ActivityType.ActivityOne),

                MakeActivity(Activity.ActivityType.ActivityTwo),
                MakeActivity(Activity.ActivityType.ActivityTwo),
                MakeActivity(Activity.ActivityType.ActivityTwo),

                MakeActivity(Activity.ActivityType.ActivityThree),
                MakeActivity(Activity.ActivityType.ActivityThree),
                MakeActivity(Activity.ActivityType.ActivityThree),
                MakeActivity(Activity.ActivityType.ActivityThree),

                MakeActivity(Activity.ActivityType.ActivityFour),
                MakeActivity(Activity.ActivityType.ActivityFour),

                MakeActivity(Activity.ActivityType.ActivityFive),
                MakeActivity(Activity.ActivityType.ActivityFive),


            };



            return rtn;
        }

        private static Activity MakeActivity(Activity.ActivityType type)
        {
            return new FluentActivity()
                .ActivityType(type)
                .PostedDateTime(DateTime.Now)
                .DescriptionSingular("desc singular")
                .Object();
        }
    }
}
