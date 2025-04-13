using LiveAppsOverlay.Updater.Interfaces;
using LiveAppsOverlay.Updater.Services;
using LiveAppsOverlay.Updater.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using NLog.Extensions.Logging;
using System.Configuration;
using System.Data;
using System.Windows;

namespace LiveAppsOverlay.Updater
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

            // Logging
            services.AddLogging(loggingBuilder => loggingBuilder.AddNLog(configFileRelativePath: "Config/NLog-updater.config"));

            // Services
            services.AddSingleton<IDownloadManager, DownloadManager>();
            services.AddSingleton<IHttpClientHandler, HttpClientHandler>();

            // ViewModels
            services.AddTransient<MainWindowViewModel>();

            return services.BuildServiceProvider();
        }
    }

}
