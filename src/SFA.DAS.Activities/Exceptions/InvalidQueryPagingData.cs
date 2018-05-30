using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Activities.Exceptions
{
    public class InvalidQueryPagingData : Exception
    {
        public InvalidQueryPagingData(string message) : base(message)
        {
            // just call base    
        }
    }
}
