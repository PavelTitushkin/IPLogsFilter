using IPLogsFilter.Abstractions.Entities;
using IPLogsFilter.Abstractions.Services;
using IPLogsFilterAPI.Models.ModelsConfig;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace IPLogsFilter.Bussines.Service
{
    public class WebLogService : IWebLogService
    {
        //private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;
        public WebLogsProviderConfig ProviderConfig { get; set; }
        public WebLogService(
            //IHttpClientFactory httpClientFactory,
            HttpClient httpClient, IOptionsMonitor<WebLogsProviderConfig> options)
        {
            ProviderConfig = options.CurrentValue;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(ProviderConfig.BasePath);
            //_httpClientFactory = httpClientFactory;
        }

        public async Task<LogFile?> GetLogAsync(string filename, CancellationToken cancellationToken)
        {
            var path = new Uri(_httpClient.BaseAddress + $"/files/{filename}");

            var response = await _httpClient.GetAsync(path, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var logFile = await response.Content.ReadFromJsonAsync<LogFile>(cancellationToken);
                
                return logFile;
            }

            return null;

            //var client = _httpClientFactory.CreateClient("WebLogsProvider");
            //var response = await client.GetAsync("/files/{filename}");
            //if (response.IsSuccessStatusCode)
            //{
            //    return new LogFile();
            //}

            //return null;
        }

        public async Task<IEnumerable<LogFile>?> GetLogFilesAsync(DateTime time, CancellationToken cancellationToken)
        {
            var path = new Uri(_httpClient.BaseAddress + $"/files?time={time}");

            var response = await _httpClient.GetAsync(path, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var logFiles = await response.Content.ReadFromJsonAsync<IEnumerable<LogFile>>(cancellationToken);

                return logFiles;
            }

            return null;
        }
    }
}
