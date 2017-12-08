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
        public string TypeOfActivity { get; set; }

        public DateTime PostedDateTime { get;  set; }

        public long AccountId { get;  set; }

        public long ProviderUkprn { get; set; }

        public ICollection<KeyValuePair<string, string>> Data { get; set; }
    }
}
