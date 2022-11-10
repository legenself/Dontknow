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

namespace DontKnow.Pages
{
    /// <summary>
    /// SettingPage.xaml 的交互逻辑
    /// </summary>
    public partial class SettingPage : INavigableView<SettingViewModel>
    {
        public SettingViewModel ViewModel { get; }
        public SettingPage(SettingViewModel viewModel)
        {
            InitializeComponent();
            ViewModel= viewModel; 
        }
    }
}
