﻿using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SFA.DAS.Activities.Configuration;
using SFA.DAS.Activities.IntegrityChecker.Interfaces;

namespace SFA.DAS.Activities.IntegrityChecker.Repositories
{
    public class AzureBlobRepository : IAzureBlobRepository
    {
        private readonly IAzureBlobStorageConfiguration _config;
        private readonly Lazy<CloudBlobClient> _blobClient;
        private readonly Lazy<CloudBlobContainer> _logContainer;
        private readonly JsonSerializerSettings _serialiserSettings;

        public AzureBlobRepository(IAzureBlobStorageConfiguration config)
        {
            _config = config;
            _blobClient = new Lazy<CloudBlobClient>(InitialiseBlobCLient);
            _logContainer = new Lazy<CloudBlobContainer>(InitialiseBlobContainer);

            _serialiserSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
            _serialiserSettings.Converters.Add(new StringEnumConverter());
        }

        public Task SerialiseObjectToLog<T>(string blobname, T instance)
        {
            var fullName = $"{blobname ?? instance.GetType().Name}_{DateTime.UtcNow:yyyy-MM-dd_HHmmss}.json";
            var blobReference = _logContainer.Value.GetBlockBlobReference(fullName);
            blobReference.Properties.ContentType = "application/json";

            var serialisedContent = JsonConvert.SerializeObject(instance, _serialiserSettings);

            return blobReference.UploadTextAsync(serialisedContent);
        }

        private CloudBlobClient InitialiseBlobCLient()
        {
            var storageAccount = CloudStorageAccount.Parse(_config.LogStorageConnectionString);

            var blobClient = storageAccount.CreateCloudBlobClient();

            return blobClient;
        }

        private CloudBlobContainer InitialiseBlobContainer()
        {
            var container = _blobClient.Value.GetContainerReference("logs");
            container.CreateIfNotExists();
            return container;
        }
    }
}