using StructureMap;

namespace SFA.DAS.Activities.Client
{
    public class ActivitiesClientRegistry : Registry
    {
        public ActivitiesClientRegistry()
        {
            For<IActivitiesClient>().Use<ActivitiesClient>();
        }
    }
}