using System.Collections.Generic;

namespace SFA.DAS.Activities.Models
{
    public class ActivityUrlLink
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public Dictionary<string, object> Parameters { get; set; }

        public ActivityUrlLink()
        {
            Parameters = new Dictionary<string, object>();
        }
    }
}
