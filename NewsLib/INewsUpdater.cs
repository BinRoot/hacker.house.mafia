using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewsLib.Model;

namespace NewsLib
{
    public interface INewsUpdater
    {
        void Update(NewsReader nr);

        void RespondToUpdate(List<NewsItem> newsItems);

        void RespondToUpdate(string rawNewsItems);
    }
}
