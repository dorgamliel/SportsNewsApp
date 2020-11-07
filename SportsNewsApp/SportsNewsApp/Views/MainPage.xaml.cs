using NavigationMasterDetail.MenuItems;
using SportsNewsApp.Models;
using SportsNewsApp.Models.Sites;
using SportsNewsApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;

namespace SportsNewsApp
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : Xamarin.Forms.TabbedPage
    {
        public List<Article> Articles { get; private set; }
        public ObservableCollection<Article> PartialArticles { get; private set; }
        //Is loading more list when dragging page to bottom.
        private bool isLoading;
        public List<MasterPageItem> menuList { get; set; }
        public MainPage()
        {
            InitializeComponent();
            //On<Xamarin.Forms.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
            /*            ////////////////////
                        menuList = new List<MasterPageItem>();
                        var page1 = new MasterPageItem() { Title = "Item 1", Icon = "hamburger.png", TargetType = typeof(TestPage1) };
                        menuList.Add(page1);
                        navigationDrawerList.ItemsSource = menuList;
                        ///////////////////*/
            Articles = AllSites.GetInfoFromSites();
            List<Article> temp = new List<Article>();
            temp = Articles.GetRange(0, 10);
            PartialArticles = new ObservableCollection<Article>();
            temp.ForEach(article => PartialArticles.Add(article));
            Articles.RemoveRange(0, 10);
            listView.ItemsSource = PartialArticles;
            listView.ItemSelected += (object sender, SelectedItemChangedEventArgs e) =>
            {
                if (e.SelectedItem == null) return;
                if (sender is Xamarin.Forms.ListView lv) lv.SelectedItem = null;
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
            var article = (Article)((Xamarin.Forms.ImageButton)sender).BindingContext;
            await Share.RequestAsync(new ShareTextRequest
            {
                Title = "Share Article",
                Uri = article.Address
            });
        }


        private void Load_Articles(object sender, System.EventArgs e)
        {
            int i;
            for (i = 0; i < 10; i++)
                PartialArticles.Add(Articles[i]);
            Articles.RemoveRange(0, 10);
        }

        [System.Obsolete]
        private void Article_Clicked(object sender, SelectedItemChangedEventArgs e)
        {
            Device.OpenUri(new System.Uri("https://goolge.com"));
        }

        private void List_Scrolled(object sender, ScrolledEventArgs e)
        {
            ScrollView sv = sender as ScrollView;

        }

        async void ViewList_Loading(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                Articles = AllSites.GetInfoFromSites();
                List<Article> temp = new List<Article>();
                temp = Articles.GetRange(0, 10);
                PartialArticles.Clear();
                temp.ForEach(article => PartialArticles.Add(article));
                Articles.RemoveRange(0, 10);
            });
            listView.EndRefresh();
        }


        /*private async void OpenMenuOnCLick(object sender, EventArgs e)
        {
            await menu_button.TranslateTo(100, 0, 1000, Easing.CubicInOut);
            await menu_button.TranslateTo(0, 0, 1000, Easing.CubicInOut);
            await menu_button.TranslateTo(-100, 0, 1000, Easing.CubicInOut);
            await menu_button.TranslateTo(0, 0, 1000, Easing.CubicInOut);
        }*/
    }
}
