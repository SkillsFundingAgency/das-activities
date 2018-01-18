using System;
using System.Linq;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Activities.AcceptanceTests.Models
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
            Name = name;

            logger.Info($"Account created with Id '{Id}' and Name '{Name}'.");
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