using APP_Proyec.Views;
using Microsoft.Maui.Controls;

namespace APP_Proyec
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Registrar rutas para navegación si es necesario
            Routing.RegisterRoute("Products", typeof(ProductsPage));
            Routing.RegisterRoute("TopSelling", typeof(TopSellingPage));
            Routing.RegisterRoute("Orders", typeof(OrdersPage));
            Routing.RegisterRoute("PendingAccounts", typeof(PendingAccountsPage));
        }
    }
}
