using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace NewsLib.Model
{
    public class NewsItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Uri ImgUri { get; set; }
        public DateTime DateAdded { get; set; }

        public String DateAddedStr
        {
            get
            {
                String retStr = "";
                TimeSpan diff = (DateTime.Now).Subtract(DateAdded);
                retStr = diff.Minutes + " " + diff.Seconds;
                return retStr;
            }
        }

        public BitmapImage ImgBit
        {
            get
            {
                return new BitmapImage(ImgUri);
            }
        }

        public Uri LinkUri { get; set; }

        private double lat = 0.0;
        public double Latitude {
            get 
            {
                return lat;
            } 
        }

        private double lon;
        public double Longitude
        {
            get
            {
                return lon;
            }
        }

        public void setLocation(double lat, double lon)
        {
            this.lat = lat;
            this.lon = lon;
        }
    }
}
