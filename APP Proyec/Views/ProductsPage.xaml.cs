using System.Collections.ObjectModel;
using System;
using APP_Proyec.Models;
using APP_Proyec.Services;
using Microsoft.Maui.Controls;

namespace APP_Proyec.Views
{
    public partial class ProductsPage : ContentPage
    {
        public ObservableCollection<Product> Products { get; } = new();

        private readonly IApiService _apiService;

        public ProductsPage()
        {
            InitializeComponent();
            _apiService = App.Services.GetService<IApiService>();
            BindingContext = this;
        }

        // For DI resolution if created from code
        public ProductsPage(IApiService apiService) : this()
        {
            if (apiService != null) _apiService = apiService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var items = await _apiService.GetProductsAsync();
            Products.Clear();
            foreach (var p in items)
                Products.Add(p);
        }
    }
}
