//using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace BitMEX
{
    public enum OrderType
    {
        Buy,
        Sell
    }

    public class OrderBookItem
    {
        public string Symbol { get; set; }
        public int Level { get; set; }
        public int BidSize { get; set; }
        public decimal BidPrice { get; set; }
        public int AskSize { get; set; }
        public decimal AskPrice { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class BitMEXApi
    {
        private const string domain = "https://www.bitmex.com";// "https://testnet.bitmex.com";
        private static string apiKey;
        private static string apiSecret;
        private int rateLimit;

        public BitMEXApi(string bitmexKey = "", string bitmexSecret = "", int rateLimit = 5000)
        {
            Debug.Log("Init API");
            apiKey = bitmexKey;
            apiSecret = bitmexSecret;
            this.rateLimit = rateLimit;
        }

        private static string BuildQueryData(Dictionary<string, string> param)
        {
            if (param == null)
                return "";

            StringBuilder b = new StringBuilder();
            foreach (var item in param)
            {
                b.Append(string.Format("&{0}={1}", item.Key, UnityWebRequest.EscapeURL(item.Value)));// WebUtility.UrlEncode(item.Value)));
            }

            try { return b.ToString().Substring(1); }
            catch (Exception) { return ""; }
        }

        private static string BuildJSON(Dictionary<string, string> param)
        {
            if (param == null)
                return "";

            var entries = new List<string>();
            foreach (var item in param)
                entries.Add(string.Format("\"{0}\":\"{1}\"", item.Key, item.Value));

            return "{" + string.Join(",", entries.ToArray()) + "}";
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        private static long GetExpires()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 3600; // set expires one hour in the future
        }

        private static long GetNonce()
        {
            DateTime yearBegin = new DateTime(1990, 1, 1);
            return DateTime.UtcNow.Ticks - yearBegin.Ticks;
        }

        public static string Query(string method, string function, Dictionary<string, string> param = null, bool auth = false, bool json = false)
        {
            //Debug.Log("GOT QUERY : " + method + "  " + function + "    params : " + param.Count + "    auth : " + auth + "    json : " + json);

            string paramData = json ? BuildJSON(param) : BuildQueryData(param);
            string url = "/api/v1" + function + ((method == "GET" && paramData != "") ? "?" + paramData : "");
            string postData = (method != "GET") ? paramData : "";

            //Debug.Log("QUERY a) " + postData);

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(domain + url);
            webRequest.Method = method;

            if (auth)
            {
                string nonce = GetExpires().ToString();
                string message = method + url + nonce + postData;
                byte[] signatureBytes = hmacsha256(Encoding.UTF8.GetBytes(apiSecret), Encoding.UTF8.GetBytes(message));
                string signatureString = ByteArrayToString(signatureBytes);

                webRequest.Headers.Add("api-nonce", nonce);
                webRequest.Headers.Add("api-key", apiKey);
                webRequest.Headers.Add("api-signature", signatureString);

                //Debug.Log( "URL : " + url + "   NONCE :" + nonce + "   KEY : " + apiKey + "   SECRET :" + apiSecret);
            }

            try
            {
                if (postData != "")
                {
                    webRequest.ContentType = json ? "application/json" : "application/x-www-form-urlencoded";
                    var data = Encoding.UTF8.GetBytes(postData);
                    using (var stream = webRequest.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }

                using (WebResponse webResponse = webRequest.GetResponse())
                using (Stream str = webResponse.GetResponseStream())
                using (StreamReader sr = new StreamReader(str))
                {
                    string line = sr.ReadToEnd();
                   // Debug.Log("Line :" + line);
                    return line;
                }
            }
            catch (WebException wex)
            {
                using (HttpWebResponse response = (HttpWebResponse)wex.Response)
                {
                    if (response == null)
                        throw;

                    using (Stream str = response.GetResponseStream())
                    {
                        using (StreamReader sr = new StreamReader(str))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }

        //public List<OrderBookItem> GetOrderBook(string symbol, int depth)
        //{
        //    var param = new Dictionary<string, string>();
        //    param["symbol"] = symbol;
        //    param["depth"] = depth.ToString();
        //    string res = Query("GET", "/orderBook", param);
        //    return JsonSerializer.DeserializeFromString<List<OrderBookItem>>(res);
        //}

        public string GetOrders()
        {
            var param = new Dictionary<string, string>();
            param["symbol"] = "XBTUSD";
            //param["filter"] = "{\"open\":true}";
            //param["columns"] = "";
            param["count"] = 100.ToString();
            //param["start"] = 0.ToString();
            //param["reverse"] = false.ToString();
            //param["startTime"] = "";
            //param["endTime"] = "";
            return Query("GET", "/order", param, true);
        }

        public Position GetPosition()
        {
            var param = new Dictionary<string, string>();
            //param["symbol"] = "XBTUSD";
            //param["filter"] = "";
            //param["count"] = "1";
            string res = Query("GET", "/position", param, true);
            //Debug.Log("position resolution string : " + res);
            string JSONToParse = res.Remove(0, 1);
            JSONToParse = JSONToParse.Remove(JSONToParse.Length - 1, 1);
            Position position = JsonUtility.FromJson<Position>(JSONToParse);
          //  Debug.Log("Position : " + JSONToParse);
            return position;
            //return Query("GET", "/position", param, true);
        }

        public string MakeOrder(OrderType orderType, int Qty)
        {
            var param = new Dictionary<string, string>();
            param["symbol"] = "XBTUSD";
            param["side"] = orderType.ToString();
            param["orderQty"] = Qty.ToString(); ;
            param["ordType"] = "Market";
            return Query("POST", "/order", param, true);
        }
        
        /// <summary>
        /// Gets wallet details
        /// </summary>
        /// <returns></returns>
        public Wallet GetWallet()
        {
            string res = BitMEX.BitMEXApi.Query("GET", "/user/wallet", new Dictionary<string, string>(), true);;
            Wallet wallet = JsonUtility.FromJson<Wallet>(res);
            return wallet;
        }

        public string DeleteOrders()
        {
            var param = new Dictionary<string, string>();
            param["orderID"] = "de709f12-2f24-9a36-b047-ab0ff090f0bb";
            param["text"] = "cancel order by ID";
            return Query("DELETE", "/order", param, true, true);
        }

        public Instrument GetInstrument(string symbol)
        {
            var param = new Dictionary<string, string>();
            param["symbol"] = symbol;
            string res = Query("GET", "/instrument", param);
            string JSONToParse = res.Remove(0, 1);
            JSONToParse = JSONToParse.Remove(JSONToParse.Length - 1, 1);

            return JsonUtility.FromJson<Instrument>(JSONToParse);
        }

        
        public List<Candle> GetCandleHistory(string symbol, int count, string size)
        {
            List<Candle> candles = new List<Candle>();
            if (true)
            {
                var param = new Dictionary<string, string>();
                param["symbol"] = symbol;
                param["count"] = count.ToString();
                param["reverse"] = true.ToString();
                param["partial"] = false.ToString();
                param["binSize"] = size;
                string res = Query("GET", "/trade/bucketed", param);

                string JSONToParse = res.Remove(0, 1);
                JSONToParse = JSONToParse.Remove(JSONToParse.Length - 1, 1);

                // parse the list to build individual json strings for each candle...
                string[] data = res.Split(new string[] { "{", "}" }, StringSplitOptions.RemoveEmptyEntries);
                

                for (int sLoop = 1; sLoop < data.Length - 1; sLoop++)
                {
                    if (data[sLoop].Length > 10)
                    {
                        // add brackets either side so json recognises it.
                        StringBuilder sb = new StringBuilder();
                        sb.Append("{");
                        sb.Append(data[sLoop]);
                        sb.Append("}");

                        Candle c = JsonUtility.FromJson<Candle>(sb.ToString());

                        string timeString = sb.ToString(14, 19);

                        //2018-04-03T02:00:00.000Z
                        c.timeStamp = new DateTime(int.Parse(timeString.Substring(0, 4)), int.Parse(timeString.Substring(5, 2)), int.Parse(timeString.Substring(8, 2)), int.Parse(timeString.Substring(11, 2)), int.Parse(timeString.Substring(14, 2)), int.Parse(timeString.Substring(17, 2)));

                        candles.Add(c);
                    }
                }

                //our candles are back to front so lets reverse them
                // candles.Reverse();

                //sort by date time ascending
                candles.Sort((a, b) => a.timeStamp.CompareTo(b.timeStamp));
            }

            return candles;// JsonUtility.FromJson<List<Candle>>(JSONToParse);
        }

        private static byte[] hmacsha256(byte[] keyByte, byte[] messageBytes)
        {
            using (var hash = new HMACSHA256(keyByte))
            {
                return hash.ComputeHash(messageBytes);
            }
        }

        #region RateLimiter

        private long lastTicks = 0;
        private object thisLock = new object();

        private void RateLimit()
        {
            lock (thisLock)
            {
                long elapsedTicks = DateTime.Now.Ticks - lastTicks;
                var timespan = new TimeSpan(elapsedTicks);
                if (timespan.TotalMilliseconds < rateLimit)
                    Thread.Sleep(rateLimit - (int)timespan.TotalMilliseconds);
                lastTicks = DateTime.Now.Ticks;
            }
        }

        #endregion RateLimiter
    }
}