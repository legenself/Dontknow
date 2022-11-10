using DontKnow.ViewModel;
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
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Controls;
using Wpf.Ui.Mvvm.Contracts;

namespace DontKnow.Pages
{
    /// <summary>
    /// HomePage.xaml 的交互逻辑
    /// </summary>
    public partial class HomePage : INavigableView<HomeViewModel>
    {
        public INavigationService navigationService;
        public HomeViewModel ViewModel { get; }
        public HomePage(HomeViewModel homeViewModel, INavigationService navigationService)
        {
            ViewModel = homeViewModel;
            InitializeComponent();
            this.navigationService = navigationService;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            navigationService.Navigate(typeof(ImportPage));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            navigationService.Navigate(typeof(ReviewPage));
        }
    }
}
