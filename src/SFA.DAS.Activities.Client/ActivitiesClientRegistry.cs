using StructureMap;

namespace SFA.DAS.Activities.Client
{
    public class ActivitiesClientRegistry : Registry
    {
        public ActivitiesClientRegistry()
        {
            IncludeRegistry<ActivitiesRegistry>();
            For<IActivitiesClient>().Use<ActivitiesClient>();
        }
    }
}