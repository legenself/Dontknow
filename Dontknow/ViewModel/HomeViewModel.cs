using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DontKnow.ViewModel
{
    public class HomeViewModel:ObservableObject
    {
        public DataService DataService { get; set; }
        public HomeViewModel(DataService dataService ) {
            DataService = dataService;
            DataService.OnDataChange += () =>
            {
                OnPropertyChanged(nameof(Projects));
                OnPropertyChanged(nameof(Keywords));
                OnPropertyChanged(nameof(HasNew));
                OnPropertyChanged(nameof(HasData));
                OnPropertyChanged(nameof(Left));
                OnPropertyChanged(nameof(NewCount));
            };
        }
        public List<Project> Projects => DataService.Get(c => c.Projects);
        public List<Keyword> Keywords => DataService.Context.TotalKeywords.ToList();
        public bool HasData =>  DataService.Context.TotalKeywords.Count()!=0;
        public int Left => DataService.Context.TodayNeedReviewsKeywords.Count();
        public int NewCount => DataService.Context.NewCount;
        public bool HasNew => DataService.Context.NewCount!=0;
        //public List<Keyword> NewKeywords => DataService.Context.TodayNeedReviewsKeywords.ToList();
        //public List<Keyword> ReviewKeywords => Projects.SelectMany(p => p.Keywords).Where(p => p.Status == KeywordStatus.Show).ToList();
    }
}
