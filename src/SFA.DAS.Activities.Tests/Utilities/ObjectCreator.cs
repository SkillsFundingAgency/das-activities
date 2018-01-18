using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Activities.Tests.Utilities
{
    public class ObjectCreator : IObjectCreator
    {
        private const string Chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        private readonly Dictionary<Type, Func<object>> _defaults;

        public ObjectCreator()
        {
            var now = DateTime.UtcNow;
            var sixMonthsAgo = now.AddMonths(-6);
            var daysSinceSixMonthsAgo = (now - sixMonthsAgo).Days;
            var random = new Random();

            _defaults = new Dictionary<Type, Func<object>>
            {
                [typeof(DateTime)] = () => sixMonthsAgo.AddDays(random.Next(daysSinceSixMonthsAgo)),
                [typeof(int)] = () => random.Next(1, 2000),
                [typeof(string)] = () => new string(Chars.Select(c => Chars[random.Next(Chars.Length)]).Take(8).ToArray())
            };
        }
        
        public object Create(Type type, object properties = null)
        {
            var message = Activator.CreateInstance(type);

            foreach (var to in type.GetProperties())
            {
                switch (to.Name)
                {
                    case "CreatorName":
                        to.SetValue(message, GetType().ToString());
                        break;
                    case "CreatorUserRef":
                        to.SetValue(message, GetType().Assembly.GetName().Name);
                        break;
                    default:
                        if (_defaults.ContainsKey(to.PropertyType))
                        {
                            to.SetValue(message, _defaults[to.PropertyType]());
                        }

                        break;
                }
            }

            if (properties != null)
            {
                foreach (var from in properties.GetType().GetProperties())
                {
                    var to = message.GetType().GetProperty(from.Name);

                    if (to == null)
                    {
                        throw new Exception($"Type '{type.Name}' does not have a property named '{from.Name}'.");
                    }

                    to.SetValue(message, from.GetValue(properties));
                }
            }

            return message;
        }
    }
}