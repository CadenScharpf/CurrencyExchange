using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using magic.http.services;

namespace Assignment06
{
    public class TrendingVendorsResponse
    {
        public Vendor[] exchanges;
    }

    public class IconGrabberResponse
    {
        public string domain { get; set; }
        public Icon[] icons;
    }

    public class Icon
    {
        public string sizes;
        public string src;
    }
    public class Vendor
    {
        public string name { get; set; }
        public string website { get; set; }
        public string volume_24h { get; set; }
    }

    public partial class MainPage : ContentPage
    {
        HttpClient client;
        public MainPage()
        {
            InitializeComponent();
            LastUpdateLabel.Text = $"(Last Updated: {DateTime.Now}).";
            
            client = new HttpClient();
            Task<TrendingVendorsResponse> trendingVendorTask = Task.Run(() => client.Get<TrendingVendorsResponse>("https://www.cryptingup.com/api/exchanges", null));
            trendingVendorTask.Wait();
            var trendingVendors = trendingVendorTask.Result;
            var table = new TableView();
            TableRoot trendingTableRoot = new TableRoot();
            for(int i = 0; i < 5; i++)
            {
                Vendor v = trendingVendors.exchanges[i];
                var layout = new StackLayout() { Orientation = StackOrientation.Horizontal };
                string domain = extractDomainFromUri(v.website);
                IconGrabberResponse icons = null;
                var IconGrabberTask = Task.Run(() => client.Get<IconGrabberResponse>($@"http://favicongrabber.com/api/grab/{domain}", null));
                try { IconGrabberTask.Wait(); icons = IconGrabberTask.Result; } catch { }
                
                string logoSrc = (icons != null && !(icons.icons.Length == 0)) ? icons.icons[0].src : "";
                layout.Children.Add(new Image() { Source = logoSrc });
                layout.Children.Add(new Label() { Text = v.name });
                layout.Children.Add(new Label() { Text = "$" + v.volume_24h });

                TableSection ts = new TableSection()
                {
                    new ViewCell(){View=layout}
                };
                trendingTableRoot.Add(ts);
            }
            
            TrendingTable.Root = trendingTableRoot;
        }

        private string extractDomainFromUri(string uri)
        {
            bool passedDotCom = false;
            int i;
            for(i = uri.Length-1; i >0; i--)
            {
                if(uri[i] == '.') 
                {
                    if (!passedDotCom) { passedDotCom = true; } 
                    else { break; }
                }
            }
            if(i != 0) { i++; }
            return uri.Substring(i, uri.Length-i);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            
        }


    }

}
