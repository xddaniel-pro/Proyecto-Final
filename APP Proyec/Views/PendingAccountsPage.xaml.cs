using System.Collections.ObjectModel;
using APP_Proyec.Models;
using APP_Proyec.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Hosting;

namespace APP_Proyec.Views
{
    public partial class PendingAccountsPage : ContentPage
    {
        public ObservableCollection<Account> Accounts { get; } = new();
        private readonly IApiService _apiService;

        public PendingAccountsPage()
        {
            InitializeComponent();
            _apiService = App.Services.GetService<IApiService>();
            BindingContext = this;
        }

        public PendingAccountsPage(IApiService apiService) : this()
        {
            if (apiService != null) _apiService = apiService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var items = await _apiService.GetPendingAccountsAsync();
            Accounts.Clear();
            foreach (var a in items)
                Accounts.Add(a);
        }
    }
}
