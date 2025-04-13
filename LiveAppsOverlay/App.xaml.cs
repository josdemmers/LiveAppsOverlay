using LiveAppsOverlay.Interfaces;
using LiveAppsOverlay.Services;
using LiveAppsOverlay.ViewModels;
using LiveAppsOverlay.ViewModels.Dialogs;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;

namespace LiveAppsOverlay
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            Services = ConfigureServices();
        }

        /// <summary>
        /// Gets the current <see cref="App"/> instance in use
        /// </summary>
        public new static App Current => (App)Application.Current;

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
        /// </summary>
        public IServiceProvider Services { get; }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Services
            services.AddSingleton<IDialogCoordinator, DialogCoordinator>();
            services.AddSingleton<ISettingsManager, SettingsManager>();
            services.AddSingleton<IThumbnailManager, ThumbnailManager>();

            // ViewModels
            services.AddTransient<AppsViewModel>();
            services.AddTransient<ConfigWindowViewModel>();
            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<ThumbnailWindowViewModel>();

            return services.BuildServiceProvider();
        }
    }

}