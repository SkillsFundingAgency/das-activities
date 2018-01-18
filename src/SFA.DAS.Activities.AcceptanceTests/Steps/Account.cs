using System;
using System.Linq;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Activities.AcceptanceTests.Steps
{
    public class Account
    {
        public long Id { get; }
        public string Name { get; }
        public int MessageCount { get; private set; }

        private object _result;

        public Account(string name, ILog logger)
        {
            Id = long.Parse(DateTime.UtcNow.ToString("yyyyMMddhhmmssffff") + (char.ToUpper(name.ToCharArray().Single()) - 64));
            logger.Info($"Account Id for {name} : {Id}");
            Name = name;
        }

        public T GetResult<T>()
        {
            return (T)_result;
        }

        public void IncrementMessageCount()
        {
            MessageCount++;
        }

        public void SetResult<T>(T result)
        {
            _result = result;
        }
    }
}