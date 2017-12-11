using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGet
{
    public class Activity
    { 
        public ActivityTypeEnum Type { get; set; }

        public virtual DateTime At { get;  set; }

        public virtual long? AccountId { get;  set; }

        public string Creator { get; set; }
        public string CreatorUserRef { get; set; }

        public virtual long? ProviderUkprn { get; set; }

        public virtual IDictionary<string, string> Data { get; set; }
    }
}
