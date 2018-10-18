using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerformanceTester.Types.Types
{
    public class RunDetails
    {
        public RunDetails()
        {
            this.StoreDetails = new List<StoreTaskDetails>();
        }

        public List<StoreTaskDetails> StoreDetails { get; set; }

        public IEnumerable<Task> Tasks
        {
            get
            {
                return this.StoreDetails.Select<StoreTaskDetails, Task>((Func<StoreTaskDetails, Task>)(sd => sd.Task));
            }
        }
    }
}
