using DontKnow.ViewModel;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
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
using Wpf.Ui.Common;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;
using Wpf.Ui.Mvvm.Services;
using static System.Net.WebRequestMethods;

namespace DontKnow.Pages
{
    /// <summary>
    /// ImportPage.xaml 的交互逻辑
    /// </summary>
    public partial class ImportPage : INavigableView<ImportViewModel>
    {
        public ImportViewModel ViewModel { get; }
        private ISnackbarService SnackbarService { get; set; }
        private IDialogControl DialogControl { get; set; }
        private ProjectImportService ProjectImportService { get; set; }
        public ImportPage(ImportViewModel viewModel, ISnackbarService snackbarService,IDialogService dialogService, ProjectImportService projectImportService)
        {
            ViewModel = viewModel;
            InitializeComponent();
            ProjectImportService = projectImportService;
                   SnackbarService = snackbarService;
            DialogControl = dialogService.GetDialogControl();
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            Ookii.Dialogs.Wpf.VistaFolderBrowserDialog vistaFolderBrowserDialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();

           //System.Windows.Forms.FolderBrowserDialog openFileDialog = new System.Windows.Forms.FolderBrowserDialog();
           // openFileDialog.Multiselect = true;
            if (vistaFolderBrowserDialog.ShowDialog() == true)
            {
                //SnackbarService.Show("警告", "你拖动了多个文件，默认选中第一个", SymbolRegular.Warning28);

                //vm.KeyFile = openFileDialog.FileNames[0];

                using Ookii.Dialogs.Wpf.ProgressDialog progressDialog = GetProgressDialog("导入");
                progressDialog.DoWork += new DoWorkEventHandler((e, o) =>
                {
                    int total = vistaFolderBrowserDialog.SelectedPaths.Length;
                    int current = 0;
                    var filenames = vistaFolderBrowserDialog.SelectedPaths.SelectMany(p =>
                    {
                        progressDialog.ReportProgress(current / total, "正在扫描", p);
                        return Directory.GetFiles(p, "*", SearchOption.AllDirectories).Where(p => !p.isBinaryFile()).ToList();

                    }).ToList();

                    ViewModel.UpdatePath(p =>
                    {
                        return new System.Collections.ObjectModel.ObservableCollection<string>(ViewModel.Path.Concat(filenames).Distinct());
                    });
                });
                progressDialog.ShowDialog();
             


            }
            else
            {
               SnackbarService.Show("取消", "未选中任何文件", SymbolRegular.Warning28);
            }
        }

        private void ImportButton_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                using Ookii.Dialogs.Wpf.ProgressDialog progressDialog = GetProgressDialog("导入" );
                progressDialog.DoWork += new DoWorkEventHandler((e, o) =>
                {
                    int total=files.Length;
                    int current = 0;
                    files = files.Where(p => p.IsDirectory()).SelectMany(p =>
                    {
                        current++;

                        progressDialog.ReportProgress(current/total, "正在扫描", p);
                        return Directory.GetFiles(p, "*", SearchOption.AllDirectories).Where(p => !p.isBinaryFile()).ToList();



                    }).Concat(files.Where(p => !p.IsDirectory()).Where(p => !p.isBinaryFile())).ToArray();
                });
                progressDialog.ShowDialog();
          

                ViewModel.UpdatePath(p =>
                {
                   return new System.Collections.ObjectModel.ObservableCollection<string>(ViewModel.Path.Concat(files).Distinct());
                }) ;
                //using Ookii.Dialogs.Wpf.ProgressDialog progressDialog = GetProgressDialog("导入" + files[0]);
                //progressDialog.DoWork += new DoWorkEventHandler(   (e,o) =>
                //{
                //    for (var i = 0; i < 100; i++)
                //    {
                //        //if (progressDialog.CancellationPending)
                //        //    return;
                //        progressDialog.ReportProgress(i,DateTime.Now+"aaa"+i,"adad"+i);


                //        Thread.Sleep(500);
                //    }
                //});
                //progressDialog.ShowDialog();


            }

        }


        private Ookii.Dialogs.Wpf.ProgressDialog GetProgressDialog(string Title)
        {
            return new Ookii.Dialogs.Wpf.ProgressDialog()
            {

                WindowTitle = Title,
                Text = "准备导入",
                Description = "",
                ProgressBarStyle =ProgressBarStyle.MarqueeProgressBar,
                ShowTimeRemaining = true,
            }; 
        }

 

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var b=  e.Source as Wpf.Ui.Controls.Button;
            var path = (string)b.CommandParameter;

            ViewModel.UpdatePath(p =>
            {
                p.Remove(path);
                return p;
            });
        }

        private void CleanButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.UpdatePath(p =>
            {
                p.Clear();
                return p;
            });
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ViewModel.Name))
            {
                SnackbarService.Show("警告", "项目名不可为空");
                return;
            }
            if (ViewModel.Path.Count==0)
            {
                SnackbarService.Show("警告","未选择任何文件");
                return;
            }

            Project project = new Project()
            {
                Uid = Guid.NewGuid(),
                Name = ViewModel.Name,
                Description = ViewModel.Description,
                Path = ViewModel.Path.ToList()
            };
            ProjectImportService.Import(project);
            ViewModel.Description = "";
            ViewModel.Name = "";
            ViewModel.UpdatePath(p =>
            {
                p.Clear();
                return p;
            });
            SnackbarService.Show("操作完成","项目导入成功");
        }

        private void FileListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            eventArg.RoutedEvent = UIElement.MouseWheelEvent;
            eventArg.Source = sender;
            FileListBox.RaiseEvent(eventArg);
        }
    }
}
