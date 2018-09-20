using System;
using System.Collections;
using System.Collections.Generic;
using PerformanceTester.Types.Interfaces;

namespace PerformanceTester.Types
{
    public class UnknownStoreNameException : Exception
    {
        public UnknownStoreNameException(string name, IEnumerable<string> availableStores) : base($"The store name '{name}' is not the name of a store. Recognised stores are {string.Join(",", availableStores)}")
        {
            // just call base    
        }
    }
}