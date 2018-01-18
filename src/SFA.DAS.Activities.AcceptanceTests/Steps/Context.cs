using SFA.DAS.NLog.Logger;
using System.Collections.Generic;
using SFA.DAS.Activities.AcceptanceTests.Models;

namespace SFA.DAS.Activities.AcceptanceTests.Steps
{
    public class Context
    {
        private readonly Dictionary<string, Account> _accounts = new Dictionary<string, Account>();
        private readonly Dictionary<string, object> _data = new Dictionary<string, object>();
        private readonly ILog _logger;

        public Context(ILog logger)
        {
            _logger = logger;
        }
        
        public T Get<T>()
        {
            return (T)_data[typeof(T).FullName];
        }

        public Account GetAccount(string accountName)
        {
            if (!_accounts.TryGetValue(accountName, out var account))
            {
                account = _accounts[accountName] = new Account(accountName, _logger);
            }

            return account;
        }

        public IEnumerable<Account> GetAccounts()
        {
            return _accounts.Values;
        }

        public void Set<T>(T data)
        {
            _data[typeof(T).FullName] = data;
        }
    }
}