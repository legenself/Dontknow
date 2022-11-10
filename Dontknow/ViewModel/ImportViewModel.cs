using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DontKnow.ViewModel
{
    public class ImportViewModel: ObservableObject
    {
        public bool IsEmpty =>Path.Count == 0;
        private string _name="默认项目名";
        public string Name { get=>_name; set=>SetProperty(ref _name, value); }
        private string _description="默认描述";
        public string Description { get=>_description; set=>SetProperty(ref _description,value); }
        private ObservableCollection<string> _path = new ObservableCollection<string>();
        public void UpdatePath(Func<ObservableCollection<string>, ObservableCollection<string>> action)
        {
            _path= action( _path);
            OnPropertyChanged(nameof(Path));
            OnPropertyChanged(nameof(IsEmpty));
        }
        public ObservableCollection<string> Path
        {
            get => _path;
        }
        
    }
    public class TokenConfiguration
    {
        public string Name { get; set; }
        public string TokenFile { get; set; }
        public string StopwordsFile{ get; set; }
    }
}
