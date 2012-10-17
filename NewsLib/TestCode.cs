using NewsLib.Model;
using NewsLib.NewsUpdaters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsLib
{
    public static class TestCode
    {
        public static void Test()
        {
            INewsUpdater blekkoUpdater = new BlekkoUpdater();
            List<INewsUpdater> newsSources = new List<INewsUpdater>();
            newsSources.Add(blekkoUpdater);
            NewsReader nr = new NewsReader(newsSources);

           // nr.ReadAllNews();
        }
    }
}
