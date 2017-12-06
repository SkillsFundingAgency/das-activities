﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using Microsoft.Azure;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SFA.DAS.Activities.Application.Configurations;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;

namespace SFA.DAS.Activities.Infrastructure.Configuration
{
    public class TableStorageProvider : IProvideSettings
    {
        private readonly string _connectionString;

        public TableStorageProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDictionary<string, object> Settings { get; } = new Dictionary<string, object>();

        public TableStorageProvider AddSection(string serviceName)
        {
            var environment = CloudConfigurationManager.GetSetting("Environment_Name") ?? ConfigurationManager.AppSettings["Environment:Name"];

            var configurationRepository = GetConfigurationRepository();
            try
            {
                var details = configurationRepository.Get(serviceName, environment.ToUpper(), "1.0");
                var settings = JsonConvert.DeserializeObject<ExpandoObject>(details, new ExpandoObjectConverter());
                AddToDictionary(settings);
            }
            catch (MissingMethodException ex)
             {
                // config is missing from table storage
            }

             return this;
        }

        private void AddToDictionary(ExpandoObject settings, string prefix = "")
        {
            if (!string.IsNullOrEmpty(prefix))
            {
                prefix += ":";
            }

            foreach (var pair in ((IDictionary<string, object>) settings))
            {
                if (pair.Value is ExpandoObject)
                {
                    AddToDictionary((ExpandoObject) pair.Value, $"{prefix}{pair.Key}");
                }
                else
                {
                    Settings.Add($"{prefix}{pair.Key}", pair.Value);
                }
            }
        }
        private IConfigurationRepository GetConfigurationRepository()
        {
            return new AzureTableStorageConfigurationRepository(_connectionString);
        }
    }
}