using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Appearance;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;
using Wpf.Ui.TaskBar;
using MenuItem = Wpf.Ui.Controls.MenuItem;

namespace DontKnow
{
    /// <summary>
    /// Interaction logic for ContainerWindow.xaml
    /// </summary>
    public partial class ContainerWindow : INavigationWindow
    {
        private readonly IThemeService _themeService;

        private readonly ITaskBarService _taskBarService;

        public ContainerWindow(INavigationService navigationService, IPageService pageService, IThemeService themeService, ITaskBarService taskBarService, ISnackbarService snackbarService, IDialogService dialogService)
        {
            // Attach the theme service
            _themeService = themeService;

            // Attach the taskbar service
            _taskBarService = taskBarService;
             
            // Initial preparation of the window.
            InitializeComponent();

            // We define a page provider for navigation
            SetPageService(pageService);

            // If you want to use INavigationService instead of INavigationWindow you can define its navigation here.
            navigationService.SetNavigationControl(RootNavigation);

            // Allows you to use the Snackbar control defined in this window in other pages or windows
            snackbarService.SetSnackbarControl(RootSnackbar);

            // Allows you to use the Dialog control defined in this window in other pages or windows
            dialogService.SetDialogControl(RootDialog);
       
          Loaded += (_, _) => InvokeSplashScreen();
        }
        /// <summary>
        /// Raises the closed event.
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Make sure that closing this window will begin the process of closing the application.
            Application.Current.Shutdown();
        }
        public Frame GetFrame()
      => RootFrame;

        public INavigation GetNavigation()
            => RootNavigation;

        public bool Navigate(Type pageType)
            => RootNavigation.Navigate(pageType);

        public void SetPageService(IPageService pageService)
            => RootNavigation.PageService = pageService;

        public void ShowWindow()
            => Show();

        public void CloseWindow()
            => Close();

        private bool _initialized = false;
        private void InvokeSplashScreen()
        {
            if (_initialized)
                return;

            _initialized = true;
  
            RootWelcomeGrid.Visibility = Visibility.Hidden;
            RootMainGrid.Visibility = Visibility.Visible; 
            Navigate(typeof(Pages.HomePage));
          
        }

        private void NavigationButtonTheme_OnClick(object sender, RoutedEventArgs e)
        {
            _themeService.SetTheme(_themeService.GetTheme() == ThemeType.Dark ? ThemeType.Light : ThemeType.Dark);
        }

        private void TrayMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem menuItem)
                return; 
        }

        private void RootNavigation_OnNavigated(INavigation sender, RoutedNavigationEventArgs e)
        { 

            // This funky solution allows us to impose a negative
            // margin for Frame only for the Dashboard page, thanks
            // to which the banner will cover the entire page nicely.
            RootFrame.Margin = new Thickness(
                left: 0,
                top: sender?.Current?.PageTag == "dashboard" ? -69 : 0,
                right: 0,
                bottom: 0);
        }
    }
}
