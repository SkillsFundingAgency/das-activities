﻿using System;
using System.Collections.Generic;

namespace SFA.DAS.Activities.AcceptanceTests.Utilities
{
    public class ObjectCreator : IObjectCreator
    {
        private readonly Dictionary<Type, object> _defaults;

        public ObjectCreator()
        {
            var now = DateTime.UtcNow;
            var sixMonthsAgo = now.AddMonths(-6);
            var daysSinceSixMonthsAgo = (now - sixMonthsAgo).Days;
            var random = new Random();

            _defaults = new Dictionary<Type, object>
            {
                [typeof(int)] = random.Next(1, 2000),
                [typeof(long)] = random.Next(2001, 4000),
                [typeof(double)] = random.Next(4001, 6000),
                [typeof(float)] = random.Next(6001, 8000),
                [typeof(decimal)] = random.Next(8001, 10000),
                [typeof(DateTime)] = sixMonthsAgo.AddDays(random.Next(daysSinceSixMonthsAgo))
            };
        }
        
        public object Create(Type type, object properties)
        {
            var message = Activator.CreateInstance(type);

            foreach (var to in type.GetProperties())
            {
                if (to.PropertyType.IsValueType && _defaults.ContainsKey(to.PropertyType))
                {
                    to.SetValue(message, _defaults[to.PropertyType]);
                }
                else if (to.PropertyType == typeof(string))
                {
                    switch (to.Name)
                    {
                        case "CreatorName":
                            to.SetValue(message, type.Assembly.GetName().Name);
                            break;
                        case "CreatorUserRef":
                            to.SetValue(message, GetType().ToString());
                            break;
                        default:
                            to.SetValue(message, to.Name);
                            break;
                    }
                }
            }

            foreach (var from in properties.GetType().GetProperties())
            {
                var to = message.GetType().GetProperty(from.Name);

                if (to == null)
                {
                    throw new Exception($"Type '{type.Name}' does not have a property named '{from.Name}'.");
                }

                to.SetValue(message, from.GetValue(properties));
            }

            return message;
        }
    }
}