using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SportsNewsApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class testMaster : ContentPage
    {
        public ListView ListView;

        public testMaster()
        {
            InitializeComponent();

            BindingContext = new testMasterViewModel();
            ListView = MenuItemsListView;
        }

        class testMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<testMasterMenuItem> MenuItems { get; set; }

            public testMasterViewModel()
            {
                MenuItems = new ObservableCollection<testMasterMenuItem>(new[]
                {
                    new testMasterMenuItem { Id = 0, Title = "Page 1" },
                    new testMasterMenuItem { Id = 1, Title = "Page 2" },
                    new testMasterMenuItem { Id = 2, Title = "Page 3" },
                    new testMasterMenuItem { Id = 3, Title = "Page 4" },
                    new testMasterMenuItem { Id = 4, Title = "Page 5" },
                });
            }

            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }
}