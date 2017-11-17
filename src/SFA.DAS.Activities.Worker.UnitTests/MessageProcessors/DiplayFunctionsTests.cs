using System;
using NUnit.Framework;
using SFA.DAS.Activities.WorkerRole.MessageProcessors;

namespace SFA.DAS.Tasks.Worker.UnitTests.MessageProcessors
{
    public class DiplayFunctionsTests
    {
        [Test]
        public void DateFormatTest()
        {
            var formatted = DisplayFunctions.FormatDateTime(new DateTime(2016,11,15,14,15,16,300));
            Assert.AreEqual("2:15 pm", formatted);
        }
    }
}
