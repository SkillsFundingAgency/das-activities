using System;

namespace SFA.DAS.Activities.AcceptanceTests.Steps
{
    public class Account
    {
        public long Id { get; }
        public string Name { get; }
        public int MessageCount { get; private set; }

        private object _result;

        public Account(string name)
        {
            Id = long.Parse(DateTime.UtcNow.ToString("yyyyMMddhhmmssffff"));
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