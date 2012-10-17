﻿using NewsLib;
using NewsLib.Model;
using NewsLib.NewsUpdaters;
﻿using Bing.Maps;
using NewsLib.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Test3
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INewsUpdater
    {

        List<NewsItem> news = new List<NewsItem>();
        Geolocator geolocator;
        Pushpin me;

        public MainPage()
        {
            this.InitializeComponent();

            List<NewsItem> newsItems = new List<NewsItem>();
            NewsItem ni = new NewsItem();
            ni.Title = "testing...";
            ni.DateAdded = DateTime.Now.AddSeconds(-12);
            ni.ImgUri=new Uri("http://graphics8.nytimes.com/images/2012/09/26/world/26prexy2/26prexy2-articleLarge.jpg");
            newsItems.Add(ni);

            NewsItem ni2 = new NewsItem();
            ni2.Title = "Jasdev Singh";
            ni2.DateAdded = DateTime.Now.AddSeconds(-12);
            ni2.ImgUri = new Uri("http://i.imgur.com/RYOjN.gif");
            newsItems.Add(ni2);

            NewsItem ni3 = new NewsItem();
            ni3.Title = "Jasdev Singh";
            ni3.DateAdded = DateTime.Now.AddSeconds(-12);
            ni3.ImgUri = new Uri("http://i.imgur.com/RYOjN.gif");
            newsItems.Add(ni3);

        

            NewsListView.DataContext = newsItems;

            geolocator = new Geolocator();
            me = new Pushpin();
            me.Template = (Application.Current.Resources["MeTemplate"] as ControlTemplate);
            MainMap.Children.Add(me);

            geolocator.PositionChanged += new Windows.Foundation.TypedEventHandler<Geolocator, PositionChangedEventArgs>(geolocator_PositionChanged);
        }

        private void geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            // Need to set map view on UI thread.
            this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(
                () =>
                {
                    displayPosition(this, args);
                }));
        }

        private void displayPosition(object sender, PositionChangedEventArgs args)
        {
            Location location = new Location(args.Position.Coordinate.Latitude, args.Position.Coordinate.Longitude);
            MapLayer.SetPosition(me, location);
        }

        static void Sleep(int ms)
        {
            new System.Threading.ManualResetEvent(false).WaitOne(ms);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            INewsUpdater blekkoUpdater = new BlekkoUpdater();
            List<INewsUpdater> newsSources = new List<INewsUpdater>();
            newsSources.Add(blekkoUpdater);
            NewsReader nr = new NewsReader(newsSources);
            Update(nr);
        }

        public void Update(NewsReader nr)
        {
            nr.ReadAllNews(this);
        }

        public void RespondToUpdate(List<NewsItem> newsItems)
        {
            List<NewsItem> reallyNewItems = new List<NewsItem>();
            if (news.Count == 0)
            {
                news.AddRange(newsItems);
            }
            else
            {
                foreach (NewsItem ni in newsItems)
                {
                    Boolean found = false;
                    foreach (NewsItem ni2 in news)
                    {
                        if (ni.Title == ni2.Title)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        reallyNewItems.Add(ni);
                    }
                }
            }


            news.AddRange(reallyNewItems);


            this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(
                () =>
                {
                    NewsListView.DataContext = null;
                    NewsListView.DataContext = news;

                    MainMap.Children.Clear();
                }));


            foreach (NewsItem ni in newsItems)
            {
                AlchemyService.StartWebRequest(ni, this);
            }

            
        }

        public void RespondToAlchemyUpdate(NewsItem newni)
        {
            if (newni != null)
            {
                foreach (NewsItem ni in news)
                {
                    if (newni.Title == ni.Title)
                    {
                        ni.setLocation(newni.Latitude, newni.Longitude);

                        this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(
                           () =>
                           {
                               Pushpin newp = new Pushpin();
                               newp.Tag = ni;
                               MainMap.Children.Add(newp);
                               MapLayer.SetPosition(newp, new Location(ni.Latitude, ni.Longitude));
                           }));
                    }
                }
            }
        }




        public void RespondToUpdate(string rawNewsItems) { } // no implementation;

        private void FlipMap()
        {
            for (int i = 0; i < 1000; i++)
            {
                PlaneProjection pp = new PlaneProjection();
                pp.RotationX = i;
                MainMap.Projection = pp;
            }
        }

        private void Image_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            FlipMap();
        }
    }
}
