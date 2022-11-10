using DontKnow.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
    /// ReviewPage.xaml 的交互逻辑
    /// </summary>
    public partial class ReviewPage : INavigableView<ReviewViewModel>
    {
        public ReviewViewModel ViewModel { get; }
        public ReviewPage(ReviewViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
        }

        private void Pass_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CurrentKeywords.Status = KeywordStatus.Show;
            ViewModel.CurrentKeywords.Reviews.Add(DateTime.Now);
            ViewModel.DataService.SaveChanges();
            ViewModel.DataService.Update();
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CurrentKeywords.Status = KeywordStatus.Hide;
            ViewModel.CurrentKeywords.Reviews.Add(DateTime.Now);
            ViewModel.DataService.SaveChanges();
            ViewModel.DataService.Update();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CurrentKeywords.Status = KeywordStatus.Delete;
            ViewModel.CurrentKeywords.Reviews.Add(DateTime.Now);
            ViewModel.DataService.SaveChanges();
            ViewModel.DataService.Update();
        }





        private void ListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            eventArg.RoutedEvent = UIElement.MouseWheelEvent;
            eventArg.Source = sender;
          ( (ListBox)sender).RaiseEvent(eventArg);
        }

        private void RelationList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item=(Position) ((ListBox)sender).SelectedItem;
            //Process.Start($"explorer {item.FileName}");

            using Process fileopener = new Process();

            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = "\"" + item.FileName + "\"";
            fileopener.Start();
        }
    }
}
