using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewsLib.Model;

namespace NewsLib
{
    public class NewsReader
    {
        private List<INewsUpdater> newsSources;

        public NewsReader(List<INewsUpdater> newsSources)
        {
            this.newsSources = newsSources;
        }

        public List<NewsItem> ReadAllNews()
        {
            List<NewsItem> news = new List<NewsItem>();
            foreach (INewsUpdater nu in newsSources)
            {
                news.AddRange(nu.Update());
            }
            return news;
        }
    }
}
