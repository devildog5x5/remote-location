using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IPManagementInterface.Shared.Services
{
    public class DeviceCommunicationService
    {
        private readonly HttpClient _httpClient;

        public DeviceCommunicationService()
        {
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(5)
            };
        }

        public async Task<bool> CheckDeviceStatusAsync(string ipAddress, int port, string protocol, CancellationToken cancellationToken = default)
        {
            try
            {
                var uri = $"{protocol}://{ipAddress}:{port}";
                using var response = await _httpClient.GetAsync(uri, cancellationToken);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string?> SendRequestAsync(string ipAddress, int port, string protocol, string? path = null, HttpMethod? method = null, string? content = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var uri = $"{protocol}://{ipAddress}:{port}{path ?? "/"}";
                method ??= HttpMethod.Get;
                
                using var request = new HttpRequestMessage(method, uri);
                if (content != null)
                {
                    request.Content = new StringContent(content);
                }

                using var response = await _httpClient.SendAsync(request, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch { }
            
            return null;
        }
    }
}
