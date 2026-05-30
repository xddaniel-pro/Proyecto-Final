using System.Collections.ObjectModel;
using APP_Proyec.Models;
using APP_Proyec.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Hosting;

namespace APP_Proyec.Views
{
    public partial class OrdersPage : ContentPage
    {
        public ObservableCollection<Order> Orders { get; } = new();
        private readonly IApiService _apiService;

        public OrdersPage()
        {
            InitializeComponent();
            _apiService = App.Services.GetService<IApiService>();
            BindingContext = this;
        }

        public OrdersPage(IApiService apiService) : this()
        {
            if (apiService != null) _apiService = apiService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var items = await _apiService.GetOrdersAsync();
            Orders.Clear();
            foreach (var o in items)
                Orders.Add(o);
        }
    }
}
