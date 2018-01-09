using System;

namespace SFA.DAS.Activities.AcceptanceTests.Steps
{
    public class TestData
    {
        public long AccountId { get; }

        public TestData()
        {
            AccountId = new Random().Next(10000, 99999);
        }
    }
}