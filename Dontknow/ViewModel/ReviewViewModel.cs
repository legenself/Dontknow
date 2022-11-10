using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DontKnow.ViewModel
{
    public class ReviewViewModel: ObservableObject
    {
        public DataService DataService { get; set; }
        public ReviewViewModel(DataService dataService)
        {
            DataService = dataService;
            DataService.OnDataChange += () =>
            {
                Update();
            };
        }
        public void Update()
        { 
            OnPropertyChanged(nameof(Total));
            OnPropertyChanged(nameof(HasData));
            OnPropertyChanged(nameof(Progress));
            OnPropertyChanged(nameof(CurrentKeywords));
            OnPropertyChanged(nameof(CurrentProject));
            OnPropertyChanged(nameof(Left));
        } 
        public int Total => DataService.Context.TotalKeywords.Count();
        public bool HasData => Total > 0;
        public int Left => DataService.Context.TodayNeedReviewsKeywords.Count();
        public int Progress => Total==0?0: 100- Left * 100 / Total;
        public Keyword? CurrentKeywords => DataService.Context.TodayNeedReviewsKeywords.FirstOrDefault();
        public Project? CurrentProject => DataService.Context.Projects.FirstOrDefault(p => p.Uid == CurrentKeywords?.ProjectUid);

    }
}
