using System.Collections.Generic;

namespace SFA.DAS.Activities.AcceptanceTests.Steps
{
    public class Context
    {
        private readonly Dictionary<string, Account> _accounts = new Dictionary<string, Account>();
        private readonly Dictionary<string, object> _data = new Dictionary<string, object>();
        
        public T Get<T>()
        {
            return (T)_data[typeof(T).FullName];
        }

        public Account GetAccount(string accountName)
        {
            if (!_accounts.TryGetValue(accountName, out var account))
            {
                account = _accounts[accountName] = new Account(accountName);
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