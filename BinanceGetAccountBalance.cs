using System.Security.Cryptography;

namespace Binance
{
    internal class BinanceGetAccountBalance
    {
        public static async Task<AccountBalance> GetAccountBalance(RSA rsa, HttpClient httpClient, string api_cluster)
        {
            string api_call = "/api/v3/account";
            Dictionary<string, string> api_params = new();
            AccountBalance accountBalance = await BinancePostGetResponse.GetResponse<AccountBalance>(httpClient,rsa, api_cluster, api_call, api_params);
            return accountBalance;
        }
    }
}
