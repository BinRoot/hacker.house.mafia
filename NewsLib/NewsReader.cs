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
        INewsUpdater nr;

        public NewsReader(List<INewsUpdater> newsSources)
        {
            this.newsSources = newsSources;
        }

        public void ReadAllNews(INewsUpdater nr)
        {
            this.nr = nr;
            foreach (INewsUpdater nu in newsSources)
            {
                nu.Update(this);
            }
        }

        // FIXME: Will this code break when there are multiple news sources?
        public void RespondToUpdate(List<NewsItem> newsItems)
        {
            nr.RespondToUpdate(newsItems);
        }
    }
}
