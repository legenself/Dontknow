using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using DontKnow.Pages;
using DontKnow.ViewModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wpf.Ui.Demo.Services;
using Wpf.Ui.Mvvm.Contracts;
using Wpf.Ui.Mvvm.Services;

namespace DontKnow
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly IHost _host = Host
       .CreateDefaultBuilder()
       .ConfigureAppConfiguration(c => { c.SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)); })
       .ConfigureServices((context, services) =>
       {
           // App Host
           //services.AddHostedService<ApplicationHostService>();
           services.AddHostedService<ApplicationHostService>();
           services.AddSingleton<IPageService, PageService>();
           services.AddSingleton<DataService>();
           services.AddSingleton<ProjectImportService>();
           services.AddSingleton<INavigationService, NavigationService>();
           // Theme manipulation
           services.AddSingleton<IThemeService, ThemeService>();

           // Taskbar manipulation
           services.AddSingleton<ITaskBarService, TaskBarService>();

           // Snackbar service
           services.AddSingleton<ISnackbarService, SnackbarService>();

           // Dialog service
           services.AddSingleton<IDialogService, DialogService>();

           // Service containing navigation, same as INavigationWindow... but without window
           services.AddSingleton<INavigationService, NavigationService>();
           services.AddScoped<INavigationWindow,ContainerWindow>();
           services.AddScoped<HomePage>();
           services.AddScoped<AboutPage>();
           services.AddScoped<ImportPage>();
           services.AddScoped<ProjectPage>();
           services.AddScoped<ReviewPage>();
           services.AddScoped<SettingPage>();

           services.AddScoped<HomeViewModel>();
           services.AddScoped<ImportViewModel>();
           services.AddScoped<ProjectViewModel>();
           services.AddScoped<ReviewViewModel>();
           services.AddScoped<SettingViewModel>();
           // Main window container with navigation

       }).Build();

        /// <summary>
        /// Gets registered service.
        /// </summary>
        /// <typeparam name="T">Type of the service to get.</typeparam>
        /// <returns>Instance of the service or <see langword="null"/>.</returns>
        public static T GetService<T>()
            where T : class
        {
            return _host.Services.GetService(typeof(T)) as T;
        }
        /// <summary>
        /// Occurs when the application is loading.
        /// </summary>
        private async void OnStartup(object sender, StartupEventArgs e)
        {
            await _host.StartAsync();
        }
        /// <summary>
        /// Occurs when the application is closing.
        /// </summary>
        private async void OnExit(object sender, ExitEventArgs e)
        {
            await _host.StopAsync();

            _host.Dispose();
        }
    }
}
