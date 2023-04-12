using System.Net.Http.Json;

namespace Binance
{
    internal class BinanceGetSymbolsPrices
    {

        private static string ReturnSymbolsPricesUrlParser(List<CurrienciesSymbols> currienciesSymbols)
        {
            if (currienciesSymbols.Count == 1)
            {
                string symbolsPrciesUrl = "/price?symbols=[%22" + currienciesSymbols[0].ToString() + " %22]";
                return symbolsPrciesUrl;
            }
            else
            {
                string symbolsPrciesUrl = "/price?symbols=[";
                for (int i = 0; i < currienciesSymbols.Count; i++)
                {
                    symbolsPrciesUrl += "%22" + currienciesSymbols[i].ToString() + "%22,";
                    if (i == currienciesSymbols.Count - 1)
                        symbolsPrciesUrl += "%22" + currienciesSymbols[i].ToString() + "%22]";
                }
                return symbolsPrciesUrl;
            }
        }

        public static async Task<List<SymbolPrice>> GetSymbolsPrices(HttpClient httpClient, string api_cluster, List<CurrienciesSymbols> currienciesSymbols)
        {
            string api_call = "/api/v3/ticker";
            string symbolsPrciesUrl = ReturnSymbolsPricesUrlParser(currienciesSymbols);
            List<SymbolPrice>? symbolsPrices = await httpClient.GetFromJsonAsync<List<SymbolPrice>>(api_cluster + api_call + symbolsPrciesUrl);
            return symbolsPrices ?? new List<SymbolPrice>();
        }



    }


}
