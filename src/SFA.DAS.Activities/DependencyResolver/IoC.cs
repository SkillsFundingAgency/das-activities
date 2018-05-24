using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Activities.Worker;

namespace SFA.DAS.Activities.DependencyResolver
{
    public static class IoC
    {
        public static IContainer Initialize()
        {
            return new Container(c =>
            {
                c.AddRegistry<CommonWorkerRegistry>(); 
            });
        }
    }
}
