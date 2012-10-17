using NewsLib.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NewsLib
{
    public static class DataService
    {
        
        public static void StartWebRequest(string url, INewsUpdater newsSource)
        {
            WebRequest webRequest = HttpWebRequest.Create(url);

            //webRequest.BeginGetResponse(new AsyncCallback(FinishWebRequest), null);
            Tuple<WebRequest, INewsUpdater> myTuple = new Tuple<WebRequest, INewsUpdater>(webRequest, newsSource);
            webRequest.BeginGetResponse(new AsyncCallback(FinishWebRequest), myTuple);
        }

        private static void FinishWebRequest(IAsyncResult result)
        {
            Tuple<WebRequest, INewsUpdater> myTuple = result.AsyncState as Tuple<WebRequest, INewsUpdater>;
            WebRequest req = myTuple.Item1;
            WebResponse response = req.EndGetResponse(result) as HttpWebResponse;
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);

            string responseFromServer = reader.ReadToEnd();

            myTuple.Item2.RespondToUpdate(responseFromServer);
        }

    }
}
