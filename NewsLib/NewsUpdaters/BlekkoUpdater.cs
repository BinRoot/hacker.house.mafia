using NewsLib.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsLib.NewsUpdaters
{
    public class BlekkoUpdater : INewsUpdater
    {
        const string baseurl = "http://blekko.com/ws/";
        const string authkey = "f91397f0";

        // http://blekko.com/ws/?q=/topnews%20/json&auth=f91397f0

        const string apiUrl = baseurl + "?q=/topnews%20/json&auth=" + authkey;
        NewsReader nr = null;

        public void Update(NewsReader nr)
        {
            this.nr = nr;

            DataService.StartWebRequest(apiUrl, this);
        }

        public void RespondToUpdate(List<NewsItem> newsItems) { } // no implementation

        public void RespondToUpdate(string rawNewsItems) 
        {
            List<NewsItem> newsList = new List<NewsItem>();
            JObject jo = JObject.Parse(rawNewsItems);
            JArray ja = (JArray)jo["RESULT"];
            for (int i = 0; i < ja.Count; i++)
            {
                JObject jao = (JObject)ja[i];
                string snippet = (string)jao["snippet"];
                
                NewsItem ni = new NewsItem();
                ni.Description = snippet;
            }


          //  nr.RespondToUpdate(
        } 

        
    }
}
