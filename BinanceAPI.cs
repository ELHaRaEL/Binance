using System.Globalization;
using System.Security.Cryptography;
using System.Reflection;

namespace Binance
{


    internal class BinanceAPI
    {
        private static readonly HttpClient clientBinanceAPI = new();
        private static readonly RSA rsa = new RSACryptoServiceProvider();

        private readonly string apiKey = string.Empty;
        private readonly string apiKeyPath = "Keys/apiKey";
        private readonly string privateKeyPath = "Keys/Private_key_XMRSwapper";
        private readonly string[] arrayOfAPIClusters = new string[]
        {
        "https://api.binance.com",
        "https://api1.binance.com",
        "https://api2.binance.com",
        "https://api3.binance.com"
        };

        private readonly List<(AccountBalance, DateTime)> listOfAccountBalances = new();

        public BinanceAPI()
        {
            apiKey = File.ReadAllText(apiKeyPath);
            SetUserAgent(clientBinanceAPI);
            BinanceRSA.SetRSAImportPrivateKey(rsa, privateKeyPath);
        }
        private void SetUserAgent(HttpClient httpClient)
        {

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);
            httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
        }


        public async Task GetAccountBalance()
        {
            AccountBalance accountBalance = await BinanceGetAccountBalance.GetAccountBalance(rsa, clientBinanceAPI, arrayOfAPIClusters[0]);
            listOfAccountBalances.Insert(0, (accountBalance, DateTime.Now));
        }

        public void PrintAssetsBalance(List<Assets>? assets = null)
        {
            if (assets == null) // Print with coins content only
            {
                if (listOfAccountBalances.Count > 0)
                {
                    List<Balance>? balances = listOfAccountBalances[0].Item1.Balances;
                    if (balances != null)
                    {
                        PrintHorizontalLine("All assets balance with content:", "Time of last account balance check: {0}", listOfAccountBalances[0].Item2);
                        foreach (Balance balance in balances)
                        {
                            if ((double.TryParse(balance.Free, NumberStyles.Float, CultureInfo.InvariantCulture, out double free) && double.TryParse(balance.Locked, NumberStyles.Float, CultureInfo.InvariantCulture, out double locked)) && (free > 0 || locked > 0))
                            {
                                PrintProperties(balance);
                            }
                        }

                    }
                }
            }
            else
            {

                if (listOfAccountBalances.Count > 0)
                {
                    List<Balance>? balances = listOfAccountBalances[0].Item1.Balances;
                    if (balances != null)
                    {
                        PrintHorizontalLine("Assets Balance:", "Time of last account balance check: {0}", listOfAccountBalances[0].Item2);
                        foreach (Assets asset in assets)
                        {
                            foreach (Balance balance in balances)
                            {
                                if (asset.ToString() == balance.Asset)
                                {
                                    PrintProperties(balance);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void PrintHorizontalLine(string methodName = "", string lineString = "Print time: {0}", DateTime dateTime = default)
        {
            if (dateTime == default)
            {
                dateTime = DateTime.Now;
            }
            Console.WriteLine();
            for (int i = 0; i < 66; i++)
            {
                Console.Write("#");
            }
            Console.WriteLine();
            Console.WriteLine(lineString, dateTime);
            Console.WriteLine(methodName);
            Console.WriteLine();
        }

        private static void PrintProperties(object obj)
        {

            PropertyInfo[] properties = obj.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                string name = property.Name;
                object? value = property.GetValue(obj);
                if (name == "CreateTime" || name == "StartTime" || name == "EndTime")
                {
                    if (value is long time)
                    {
                        Console.WriteLine("{0} = {1} = {2} (UTCM)", name, DateTimeOffset.FromUnixTimeMilliseconds(time).ToString("dd.MM.yyyy HH:mm:ss"), value);
                    }
                }
                else
                {
                    Console.WriteLine("{0} = {1}", name, value);
                }
            }
        }


        public async Task<GetQuote> GetQuote(Assets fromAsset, Assets toAsset, double fromAmount = 0, double toAmount = 0, EnumsTypes.WalletType walletType = 0, EnumsTypes.ValidTime validTime = (EnumsTypes.ValidTime)0, int recvWindow = 5000)
        {
            PrintHorizontalLine("Get quote:");
            GetQuote getQuote = await BinanceConvert.GetQuote(clientBinanceAPI, rsa, arrayOfAPIClusters[0], fromAsset, toAsset, fromAmount, toAmount, walletType, validTime, recvWindow);
            Console.WriteLine("From: {0,10}  --> To: {1,-10}", fromAsset, toAsset);
            Console.WriteLine("From: {0,10}  --> To: {1,-10}", getQuote.FromAmount, getQuote.ToAmount);
            PrintProperties(getQuote);
            return getQuote;
        }



        private async Task<AcceptQuote> AcceptQuote(string quoteID)
        {
            PrintHorizontalLine("Accept quote:");
            AcceptQuote acceptQuote = await BinanceConvert.AcceptQuote(clientBinanceAPI, rsa, arrayOfAPIClusters[0], quoteID);
            PrintProperties(acceptQuote);
            return acceptQuote;
        }

        public async Task<StatusOrder> OrderStatus(string quoteID)
        {
            PrintHorizontalLine("Order status:");
            StatusOrder statusOrder = await BinanceConvert.OrderStatus(clientBinanceAPI, rsa, arrayOfAPIClusters[0], quoteID);
            PrintProperties(statusOrder);
            return statusOrder;
        }

        public async Task ConvertFromTo(Assets fromAsset, Assets toAsset, double fromAmount = 0, double toAmount = 0, EnumsTypes.WalletType walletType = 0, EnumsTypes.ValidTime validTime = (EnumsTypes.ValidTime)0, int recvWindow = 5000)
        {
            await GetAccountBalance();
            PrintAssetsBalance(new List<Assets> { fromAsset, toAsset });
            GetQuote getQuote = await GetQuote(Assets.XMR, Assets.EUR, fromAmount, toAmount, walletType, validTime, recvWindow);
            if (getQuote.QuoteId != null)
            {
                /* AcceptQuote acceptQuote =  */
                await AcceptQuote(getQuote.QuoteId);
                /* StatusOrder statusOrder =  */
                await OrderStatus(getQuote.QuoteId);
                await GetAccountBalance();
                PrintAssetsBalance(new List<Assets> { fromAsset, toAsset });
            }
            else
                Console.WriteLine("There are not enough funds in your account.");
        }

        public async Task GetConvertTradeHistory(DateTime startTime, DateTime endTime = default, int limit = 100, int recvWindow = 5000)
        {
            PrintHorizontalLine("Get convert trade history:");
            ConvertTradeHistory convertTradeHistory = await BinanceConvert.GetConvertTradeHistory(clientBinanceAPI, rsa, arrayOfAPIClusters[0], startTime, endTime, limit, recvWindow);
            if (convertTradeHistory.List != null)
            {
                foreach (List OrderStatus in convertTradeHistory.List)
                {
                    PrintProperties(OrderStatus);
                    Console.WriteLine();
                }
            }
            Console.WriteLine("\nLimit: {2}\nMoreData: {3}\nStartTime: {0}\n  EndTime: {1}", startTime, endTime, convertTradeHistory.Limit, convertTradeHistory.MoreData);

        }


    }
}
