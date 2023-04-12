using System.Net.Http.Json;
using System.Security.Cryptography;

namespace Binance
{

        internal class BinancePostGetResponse
    {

        public static async Task<T> PostResponse<T>(HttpClient httpClient,RSA rsa, string api_cluster, string api_call, Dictionary<string, string> api_params) where T : IPostResponse, new()
        {
            string timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            string api_params_with_timestamp = string.Join("&", api_params.Select(x => $"{x.Key}={x.Value}")) + $"&timestamp={timestamp}";    
            string signature = BinanceRSA.SignData(rsa, api_params_with_timestamp);
            string url = api_cluster + api_call + "?" + api_params_with_timestamp + "&signature=" + signature;
            HttpResponseMessage response = await httpClient.PostAsync(url, null);
            T? t = await response.Content.ReadFromJsonAsync<T>();
            return t ?? new T();
        }
        public static async Task<T> GetResponse<T>(HttpClient httpClient, RSA rsa, string api_cluster, string api_call, Dictionary<string, string> api_params) where T : IGetResponse, new()
        {
            string timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            string api_params_with_timestamp = string.Join("&", api_params.Select(x => $"{x.Key}={x.Value}")) + $"&timestamp={timestamp}";
            string signature = BinanceRSA.SignData(rsa, api_params_with_timestamp);
            string url = api_cluster + api_call + "?" + api_params_with_timestamp + "&signature=" + signature;
            T? t = await httpClient.GetFromJsonAsync<T>(url);
            return t ?? new T();
        }

    }
}
