using System.Collections.ObjectModel;
using APP_Proyec.Models;
using APP_Proyec.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Hosting;

namespace APP_Proyec.Views
{
    public partial class TopSellingPage : ContentPage
    {
        public ObservableCollection<Product> Products { get; } = new();
        private readonly IApiService _apiService;

        public TopSellingPage()
        {
            InitializeComponent();
            _apiService = App.Services.GetService<IApiService>();
            BindingContext = this;
        }

        public TopSellingPage(IApiService apiService) : this()
        {
            if (apiService != null) _apiService = apiService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var items = await _apiService.GetTopSellingAsync();
            Products.Clear();
            foreach (var p in items)
                Products.Add(p);
        }
    }
}
