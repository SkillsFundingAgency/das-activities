using StructureMap;

namespace SFA.DAS.Activities.Client.TestHost.IoC
{
    public static class IoC
    {
        public static IContainer InitialiseIoC()
        {
            return new Container(c =>
            {
                c.AddRegistry<CommonRegistry>();
                c.AddRegistry<ActivitiesClientRegistry>();
            });
        }
    }
}
