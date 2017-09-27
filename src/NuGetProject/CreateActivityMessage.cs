
using NuGetProject.Enums;

namespace NuGetProject
{
    public class CreateActivityMessage
    {
        public string AccountId { get; set; }

        public ActivityType Type { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }


    }
}
