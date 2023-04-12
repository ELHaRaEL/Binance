using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace Binance
{
    internal class BinanceConvert
    {
        //        public static async Task<GetQuote> GetQuote(HttpClient httpClient, RSA rsa, string api_cluster, Assets fromAsset, Assets toAsset, double fromAmount = 0, double toAmount = 0, EnumsTypes.WalletType walletType = 0, EnumsTypes.ValidTime validTime = (EnumsTypes.ValidTime)0, int recvWindow = 5000)
        public static async Task<GetQuote> GetQuote(HttpClient httpClient, RSA rsa, string api_cluster, Assets fromAsset, Assets toAsset, double fromAmount, double toAmount , EnumsTypes.WalletType walletType , EnumsTypes.ValidTime validTime , int recvWindow )
        {
            //Either fromAmount or toAmount should be sent
            //quoteId will be returned only if you have enough funds to convert
            //If you sent fromAmount and toAmount - then only fromAmount will be use
            
            recvWindow = recvWindow >= 0 && recvWindow <= 600000 ? recvWindow : 5000;
            double amount = fromAmount > 0 ? fromAmount : toAmount;
            string amountName = fromAmount > 0 ? "fromAmount" : "toAmount";

                Dictionary<string, string> api_params = new()
                {
                    { "fromAsset", fromAsset.ToString() },
                    { "toAsset" , toAsset.ToString() },
                    { amountName , amount.ToString(CultureInfo.InvariantCulture) },
                    { "walletType" , walletType.ToString() },
                    { "validTime" , validTime.ToString().Replace("_","") },
                    { "recvWindow" , recvWindow.ToString() }
                };
                string api_call = "/sapi/v1/convert/getQuote";
                GetQuote getQuote = await BinancePostGetResponse.PostResponse<GetQuote>(httpClient, rsa, api_cluster, api_call, api_params);
                return getQuote;
        }

        public static async Task<AcceptQuote> AcceptQuote(HttpClient httpClient, RSA rsa, string api_cluster, string quoteId, int recvWindow = 5000)
        {
            recvWindow = recvWindow >= 0 && recvWindow <= 600000 ? recvWindow : 5000;

            Dictionary<string, string> api_params = new()
            {
                { "quoteId", quoteId },
                { "recvWindow" , recvWindow.ToString() }
            };

            string api_call = "/sapi/v1/convert/acceptQuote";
            AcceptQuote acceptQuote = await BinancePostGetResponse.PostResponse<AcceptQuote>(httpClient, rsa, api_cluster, api_call, api_params);
            return acceptQuote;
        }

        public static async Task<StatusOrder> OrderStatus(HttpClient httpClient, RSA rsa, string api_cluster, string? quoteId = null, string? orderId = null)
        {
            Dictionary<string, string> api_params = new();
            if (quoteId != null && orderId == null)
                api_params = new() {{ "quoteId", quoteId }};
            
            if (quoteId == null && orderId != null)
                api_params = new() { { "orderId", orderId } };

            string api_call = "/sapi/v1/convert/orderStatus";

            StatusOrder statusOrder = await BinancePostGetResponse.GetResponse<StatusOrder>(httpClient, rsa, api_cluster, api_call, api_params);
            return statusOrder;
        }


        public static async Task<ConvertTradeHistory> GetConvertTradeHistory(HttpClient httpClient, RSA rsa, string api_cluster, DateTime startTime, DateTime endTime = default, int limit = 100, int recvWindow = 5000)
        {
            if (endTime == default)
            {
                endTime = DateTime.Now;
            }
            recvWindow = recvWindow >= 0 && recvWindow <= 600000 ? recvWindow : 5000;
            limit = limit > 0 && limit <= 1000 ? limit : 100;

            Dictionary<string, string> api_params = new()
            {
                { "startTime", new DateTimeOffset(startTime).ToUnixTimeMilliseconds().ToString()},
                { "endTime",  new DateTimeOffset(endTime).ToUnixTimeMilliseconds().ToString() },
                { "limit" , limit.ToString() },
                { "recvWindow", recvWindow.ToString() },
            };

            string api_call = "/sapi/v1/convert/tradeFlow";
            ConvertTradeHistory convertTradeHistory = await BinancePostGetResponse.GetResponse<ConvertTradeHistory>(httpClient, rsa, api_cluster, api_call, api_params);
            return convertTradeHistory;
        }

    }


}
