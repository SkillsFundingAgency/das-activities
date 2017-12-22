﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using Microsoft.Azure;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;

namespace SFA.DAS.Activities.Configuration
{
    public class TableStorageProvider : ISettingsProvider
    {
        public IDictionary<string, object> Settings { get; } = new Dictionary<string, object>();

        private readonly string _connectionString;

        public TableStorageProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        public TableStorageProvider AddSection(string serviceName)
        {
            var environment = ConfigurationManager.AppSettings["Environment_Name"];

            if (string.IsNullOrEmpty(environment))
            {
                environment = CloudConfigurationManager.GetSetting("Environment_Name");
            }

            if (!string.IsNullOrEmpty(_connectionString) && !string.IsNullOrEmpty(environment))
            {
                var configurationRepository = GetConfigurationRepository();

                try
                {
                    var details = configurationRepository.Get(serviceName, environment.ToUpper(), "1.0");
                    var settings = JsonConvert.DeserializeObject<ExpandoObject>(details, new ExpandoObjectConverter());

                    AddToDictionary(settings);
                }
                catch (Exception)
                {
                    // Swallow exception
                }
            }

            return this;
        }

        private void AddToDictionary(ExpandoObject settings, string prefix = "")
        {
            if (!string.IsNullOrEmpty(prefix))
            {
                prefix += ":";
            }

            foreach (var pair in settings)
            {
                if (pair.Value is ExpandoObject)
                {
                    AddToDictionary((ExpandoObject) pair.Value, $"{prefix}{pair.Key}");
                }
                else if (pair.Value != null && !(pair.Value is string) || pair.Value is string && !string.IsNullOrEmpty((string) pair.Value))
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