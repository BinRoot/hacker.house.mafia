using NewsLib.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Test3;

namespace NewsLib
{
    public static class AlchemyService
    {
        private static String apiurl = "http://access.alchemyapi.com/calls/url/URLGetRankedConcepts?apikey=2605a2c3d5548b446ed09bfbdefe41ff4555aa2a&outputMode=json&url=";

        static MainPage mp = null;

        public static void StartWebRequest(NewsItem ni, MainPage _mp)
        {
            mp = _mp;
            WebRequest webRequest = HttpWebRequest.Create( apiurl+ni.LinkUri.ToString() );

            Tuple<WebRequest, NewsItem> myTuple = new Tuple<WebRequest, NewsItem>(webRequest, ni);
            webRequest.BeginGetResponse(new AsyncCallback(FinishWebRequest), myTuple);
        }

        private static void FinishWebRequest(IAsyncResult result)
        {
            Tuple<WebRequest, NewsItem> myTuple = result.AsyncState as Tuple<WebRequest, NewsItem>;
            WebRequest req = myTuple.Item1;
            WebResponse response = req.EndGetResponse(result) as HttpWebResponse;
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);

            string responseFromServer = reader.ReadToEnd();


            // finds lat long from responseFromServer
            JObject jo = JObject.Parse(responseFromServer);
            JArray ja = (JArray)jo["concepts"];

            NewsItem ni = ((NewsItem)myTuple.Item2);

            Boolean updated = false;
            for (int i = 0; i < ja.Count; i++)
            {
                JObject jao = (JObject)ja[i];
                String geo = (String)jao["geo"];
                if (geo != null)
                {
                    double latd = Double.Parse(geo.Split(' ')[0]);
                    double lond = Double.Parse(geo.Split(' ')[1]);
                    updated = true;
                    ni.setLocation(latd, lond);
                    break;
                }
            }

            // update ni with lat long
            mp.RespondToAlchemyUpdate(ni);
        }

    }
}
