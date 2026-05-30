using Microsoft.Extensions.DependencyInjection;

namespace APP_Proyec
{
    public partial class App : Application
    {
        // Servicio global para resolver dependencias desde constructores XAML
        public static IServiceProvider Services { get; internal set; } = null!;

        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}