using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace CKBS.Models.Services.Banking
{
    public class CurrencyExchange
    {
        private const string urlPattern = "http://rate-exchange-1.appspot.com/currency?from={0}&to={1}";
        public string CurrencyConversion(string fromCurrency, string toCurrency, decimal amount)
        {
            string url = string.Format(urlPattern, fromCurrency, toCurrency);
            using (var wc = new WebClient())
            {
                var json = wc.DownloadString(url);
                JToken token = JToken.Parse(json);

                decimal exchangeRate = (decimal)token.SelectToken("rate");
                return (amount * exchangeRate).ToString();
            }
        }

        public string Convert(CurrencyConvert currency, string from,  string to, double amount)
        {
            JToken json = JToken.FromObject(currency);
            var rate = double.Parse(json["Rate"].ToString());
            if (string.Compare(from, to, true) == 0)
            {
                rate = 1;
            }

            if (string.Compare(to, json["From"].ToString(), true) == 0)
            {
                rate = 1 / rate;               
            } 

            return (rate).ToString();
        }

    }

    public class CurrencyConvert
    {
        public string From { get; set; }
        public string To { get; set; }
        public decimal Rate { get; set; }
    }
    
}
