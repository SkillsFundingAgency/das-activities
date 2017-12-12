﻿using NUnit.Framework;

namespace SFA.DAS.Activities.UnitTests
{
    public abstract class Test
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            Given();
            When();
        }

        protected abstract void Given();
        protected abstract void When();
    }
}