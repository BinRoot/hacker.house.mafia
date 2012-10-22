using NewsLib;
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
using Windows.UI.Popups;

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

            //List<NewsItem> newsItems = new List<NewsItem>();
            //NewsItem ni = new NewsItem();
            //ni.Title = "testing...";
            //ni.DateAdded = DateTime.Now.AddSeconds(-12);
            //ni.ImgUri=new Uri("http://graphics8.nytimes.com/images/2012/09/26/world/26prexy2/26prexy2-articleLarge.jpg");
            //newsItems.Add(ni);

            //NewsItem ni2 = new NewsItem();
            //ni2.Title = "Jasdev Singh";
            //ni2.DateAdded = DateTime.Now.AddSeconds(-12);
            //ni2.ImgUri = new Uri("http://i.imgur.com/RYOjN.gif");
            //newsItems.Add(ni2);

            //NewsItem ni3 = new NewsItem();
            //ni3.Title = "Jasdev Singh";
            //ni3.DateAdded = DateTime.Now.AddSeconds(-12);
            //ni3.ImgUri = new Uri("http://i.imgur.com/RYOjN.gif");
            //newsItems.Add(ni3);

        

            //NewsListView.DataContext = newsItems;

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
            INewsUpdater blekkoChinaUpdater = new BlekkoChinaUpdater();
            INewsUpdater blekkoEuropeUpdater = new BlekkoEuropeUpdater();
            INewsUpdater blekkoAfricaUpdater = new BlekkoAfricaUpdater();
            INewsUpdater blekkoIndiaUpdater = new BlekkoIndiaUpdater();
            INewsUpdater blekkoSAUpdater = new BlekkoSAUpdater();
            INewsUpdater blekkoUSUpdater = new BlekkoUSUpdater();

            List<INewsUpdater> newsSources = new List<INewsUpdater>();
            newsSources.Add(blekkoChinaUpdater);
            newsSources.Add(blekkoEuropeUpdater);
            newsSources.Add(blekkoAfricaUpdater);
            newsSources.Add(blekkoIndiaUpdater);
            newsSources.Add(blekkoSAUpdater);
            newsSources.Add(blekkoUSUpdater);
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


            //this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(
            //    () =>
            //    {
            //        NewsListView.DataContext = null;
            //        NewsListView.DataContext = news;

            //        MainMap.Children.Clear();
            //    }));

            size = newsItems.Count;
            count = 1;
            foreach (NewsItem ni in newsItems)
            {
                AlchemyService.StartWebRequest(ni, this);
            }

            
        }

        int count = 1;
        int size = 0;
        List<NewsItem> thingsToRemove = new List<NewsItem>();

        public void RespondToAlchemyUpdate(NewsItem newni)
        {
            if (count >= size)
            {
                this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(
                   () =>
                   {

                       foreach (NewsItem ri in thingsToRemove)
                       {
                           news.Remove(ri);
                       }

                       NewsListView.DataContext = null;
                       NewsListView.DataContext = news;

                       thingsToRemove.Clear();
                   }));
            }
            count++;
            if (!((newni.Latitude == 0.0)&&(newni.Longitude == 0.0)))
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
                               newp.Tapped += newp_Tapped;
                               newp.Tag = ni;
                               MainMap.Children.Add(newp);
                               MapLayer.SetPosition(newp, new Location(ni.Latitude, ni.Longitude));
                           }));
                    }
                }
            }
            else
            {
                thingsToRemove.Add(newni);
            }
        }

        async void newp_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Pushpin v = (Pushpin)sender;
            // Create the message dialog and set its content; it will get a default "Close" button since there aren't any other buttons being added
            var messageDialog = new MessageDialog(((NewsItem)v.Tag).Title);

            // Show the message dialog and wait
            await messageDialog.ShowAsync();
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
            Image img = (Image)sender;
            NewsItem ni = (NewsItem)img.Tag;

            foreach (Pushpin p in MainMap.Children)
            {
                MainMap.Center = new Location(ni.Latitude, ni.Longitude);
                p.Background = new SolidColorBrush(Windows.UI.Colors.CornflowerBlue);
              
            }

            foreach (Pushpin p in MainMap.Children)
            {
                if (p.Tag == ni)
                {
                    p.Background = new SolidColorBrush(Windows.UI.Colors.HotPink);
                    break;
                }
            }
            
        }
    }
}
