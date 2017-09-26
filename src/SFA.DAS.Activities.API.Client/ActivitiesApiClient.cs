using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Activities.API.Types.DTOs;
using SFA.DAS.Tasks.API.Client;

namespace SFA.DAS.Activities.API.Client
{
    public class ActivitiesApiClient : IActivitiesApiClient
    {
        private readonly IActivitiesApiConfiguration _configuration;
        private readonly SecureHttpClient _httpClient;

        public ActivitiesApiClient(IActivitiesApiConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new SecureHttpClient(configuration);
        }

        public async Task<IEnumerable<ActivityDto>> GetActivities(string ownerId)
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}api/activities/{ownerId}";

            var json = await _httpClient.GetAsync(url);
            return JsonConvert.DeserializeObject<IEnumerable<ActivityDto>>(json);
        }

        private string GetBaseUrl()
        {
            return _configuration.ApiBaseUrl.EndsWith("/")
                ? _configuration.ApiBaseUrl
                : _configuration.ApiBaseUrl + "/";
        }
    }
}
