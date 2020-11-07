using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SportsNewsApp.Models.Sites
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HighlightsPage : ContentPage
    {
        //An array of all delimiters, for HTML page parsing.
        internal Dictionary<string, string> Delimiters { get; set; }
        public List<Article> Vids { get; private set; }
        public ObservableCollection<Article> ObVids { get; private set; }

        public HighlightsPage()
        {
            InitializeComponent();
            FullHighlights fh = new FullHighlights();
            Vids = fh.ArticleList;
            ObVids = new ObservableCollection<Article>();
            Vids.ForEach(article => ObVids.Add(article));
            listView.ItemsSource = Vids;
            listView.ItemSelected += (object sender, SelectedItemChangedEventArgs e) =>
            {
                if (e.SelectedItem == null) return;
                if (sender is ListView lv) lv.SelectedItem = null;
                var item = (Article)e.SelectedItem;
                Browser.OpenAsync(new Uri(item.Address), BrowserLaunchMode.SystemPreferred);
            };
            BindingContext = this;
        }

        async void Share_Click(object sender, System.EventArgs e)
        {
            await ShareText(sender, e);
        }
        public async Task ShareText(object sender, EventArgs e)
        {
            var article = (Article)((ImageButton)sender).BindingContext;
            await Share.RequestAsync(new ShareTextRequest
            {
                Title = "Share Article",
                Uri = article.Address
            });
        }
    }
}