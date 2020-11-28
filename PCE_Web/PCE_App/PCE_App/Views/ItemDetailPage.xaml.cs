using System.ComponentModel;
using Xamarin.Forms;
using PCE_App.ViewModels;

namespace PCE_App.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}