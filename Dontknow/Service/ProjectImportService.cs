using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DontKnow
{
    public enum KeywordStatus
    {
        New,Hide,Show,Delete
    }
    public class Keyword
    {
        public Guid ProjectUid { get; set; }
        public string Content { get; set; }
        public string Memo { get; set; }
        public DateTime AddAt { get; set; }
        [JsonIgnore]
        public DateTime UpdateAt => Reviews.Count != 0 ? Reviews.Last() : AddAt;
        public List<Position> Positions { get; set; } = new List<Position>();
        public List<DateTime> Reviews { get; set; } = new List<DateTime>();
        public KeywordStatus Status { get; set; } = KeywordStatus.New;
        [JsonIgnore]
        public string BaiduSearch => $"https://www.baidu.com/s?wd={Content}";
        [JsonIgnore]
        public string BingSearch => $"https://cn.bing.com/search?q={Content}";

        [JsonIgnore]
        public string BaiduTranslate => $"https://fanyi.baidu.com/#zh/en/{Content}";
    }
    public class Position
    {
        public string Content { get; set; }
        [JsonIgnore]
        public string ShowContent => Content.Trim();
        [JsonIgnore]
        public int ShowLineNumber => LineNumber + 1;
        public string FileName { get; set; }


        public int LineNumber { get; set; }
    }
    public class Project
    {
        public Guid Uid { get; set; }
        public string Name { get; set; }
        

        public string Description { get; set; }
        public List<string> Path { get; set; }
  
        public List<Keyword> Keywords { get; set; } = new List<Keyword>();
        [JsonIgnore]
        public int Total => Keywords.Count;
        [JsonIgnore]
        public int Review => Keywords.Where(p => p.Status == KeywordStatus.New || p.Status == KeywordStatus.Show).Count();
        [JsonIgnore]
        public int Progress =>Total==0?100: (int)(100-Review / (decimal)Total * 100);
    }


    public class ProjectImportService
    {
        DataService DataService;
        public ProjectImportService(DataService dataService)
        {
            DataService = dataService;  
        }
        public void Import(Project project)
        {

            var tokens = DataService.GetTokens();
            var stopwords = DataService.GetStopwords();
            var textfiles = project.Path.Where(p => !p.isBinaryFile()).ToList();
            var alllines = textfiles.SelectMany((file) =>
            {
                try
                {
                    return File.ReadAllLines(file).Select((p, i) =>
                    {
                        if (string.IsNullOrEmpty(p.Trim()))
                        {
                            return null;
                        }
                        return new
                        {
                            Path =file,
                            Line = p,
                            LineNumber = i
                        };
                    });


                }
                catch (Exception ex)
                {
                    return null;
                }

            }).Where(p => p != null).ToList();
            var allwords = new List<dynamic>();
            foreach (var line in alllines)
            {
                var removeZh = Regex.Replace(line.Line, "[\u4E00-\u9FFF]", " ");

                if (string.IsNullOrWhiteSpace(removeZh))
                {
                    continue;
                }
                allwords.AddRange(removeZh.Split(tokens).Select((p) => p.Trim()).Where(p => !Decimal.TryParse(p, out _)).Where(p => p.Length > 2).Where(p => !stopwords.Contains(p)).Select(p =>
                {
                    return new
                    {
                        FileName = line.Path,
                        LineNumber = line.LineNumber,
                        Line = line.Line,
                        Content = p
                    };
                }).ToList());
            }
 
            var result = allwords.GroupBy(p => p.Content)
            .Select(p => new Keyword()
            {
                ProjectUid = project.Uid,
                Content = p.Key,
                AddAt=DateTime.Now,
                Positions = p.Select(p1 => new Position()
                {
                    Content=p1.Line,
                 
                    FileName = p1.FileName,
                    LineNumber = p1.LineNumber
                }).ToList()
            }).ToList();


            DataService.Set((c) => {
                project.Keywords.AddRange(result);
                c.Projects.Add(project);
         
                return true;
            });

            return ;
        }

    }
}
