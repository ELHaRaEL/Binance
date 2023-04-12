using Binance;

BinanceAPI binanceAPI = new();

await binanceAPI.GetAccountBalance();
binanceAPI.PrintAssetsBalance();
binanceAPI.PrintAssetsBalance(new List<Assets> { Assets.XMR, Assets.EUR });
/*   GetQuote getQuote =   */ await binanceAPI.GetQuote(Assets.XMR, Assets.EUR, 0.0065, 0);
//await binanceAPI.ConvertFromTo(Assets.XMR, Assets.EUR, 0.0065, 0);
await binanceAPI.GetConvertTradeHistory(DateTime.Now.AddDays(-72),DateTime.Now);

Console.ReadKey();

