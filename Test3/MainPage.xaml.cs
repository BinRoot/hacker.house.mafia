using Bing.Maps;
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
    public sealed partial class MainPage : Page
    {

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

        }

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
