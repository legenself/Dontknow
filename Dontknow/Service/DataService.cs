using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using DontKnow.ViewModel;

namespace DontKnow
{
    public class DataService
    {
        public string GetUserAppDataPath()
        {
         
        
            // Get the .EXE assembly
            Assembly assm = Assembly.GetEntryAssembly();
            // Get a 'Type' of the AssemblyCompanyAttribute
            Type at = typeof(AssemblyCompanyAttribute);
            // Get a collection of custom attributes from the .EXE assembly
            object[] r = assm.GetCustomAttributes(at, false);
            // Get the Company Attribute
            AssemblyCompanyAttribute ct =
                          ((AssemblyCompanyAttribute)(r[0]));
            // Build the User App Data Path
            string path = Environment.GetFolderPath(
                        Environment.SpecialFolder.ApplicationData);
            path += @"\" + ct.Company;
            path += @"\" + assm.GetName().Version.ToString();
            return path;
        }
        public string BaseFolder;
        private const string DataFile="data.json";
        private DataContext context;
        private DataContext previewChange;
        public DataContext Context => context;
        public DataService()
        {
            BaseFolder = GetUserAppDataPath();
            if (!File.Exists(BaseFolder))
            {
                Directory.CreateDirectory(BaseFolder);
            }
            if (!File.Exists(DataPath))
            {
                File.WriteAllText(DataPath, JsonConvert.SerializeObject(new { }));
            }

            if (!File.Exists(Path.Combine(BaseFolder, "configs")))
            {
                Directory.CreateDirectory(Path.Combine(BaseFolder, "configs"));
                File.WriteAllText(Path.Combine(BaseFolder, "configs", "token.txt"), File.ReadAllText("token.txt"));
                File.WriteAllText(Path.Combine(BaseFolder, "configs", "stopwords.txt"), File.ReadAllText("stopwords.txt"));
            }
            Reload();
            
        }
        public   string DataPath => Path.Combine(BaseFolder, DataFile);
        public string TokenPath => Path.Combine(BaseFolder, "configs", "token.txt");
        public string Stopwords=> Path.Combine(BaseFolder, "configs", "stopwords.txt");
        public Action OnDataChange;
        public char[] GetTokens()
        {
            return File.ReadAllText(TokenPath).ToCharArray();
        }
        public string[] GetStopwords()
        {
            return File.ReadAllLines(Stopwords);
        }
        public T Get<T>(Func<DataContext,T> func)
        {
            return func(context);
        }
        public   T DeepClone<T>(  T obj)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj));
        }
        public bool Set(Func<DataContext,bool> func)
        {
                previewChange = DeepClone(context);
                if (!func(previewChange))
                {

                    return false;
                }
            context = previewChange;
            File.WriteAllText(Path.Combine(BaseFolder, DataFile), JsonConvert.SerializeObject(context));

            Update();
                return true;
        }
        public void Update()
        {
            OnDataChange?.Invoke();
        }
        public void SaveChanges()
        {
            File.WriteAllText(Path.Combine(BaseFolder, DataFile), JsonConvert.SerializeObject(context));
        }
        public void Reload()
        {
            context= JsonConvert.DeserializeObject<DataContext>( File.ReadAllText(Path.Combine(BaseFolder, DataFile)));
        }

    }
    public class DataContext
    {
        public List<Project> Projects { get; set; }=new List<Project>();
        public IEnumerable<Keyword> TotalKeywords => Projects.SelectMany(p => p.Keywords).OrderBy(p => p.UpdateAt);
        public IEnumerable<Keyword> NeedReviewsKeywords => TotalKeywords.Where(p => p.Status != KeywordStatus.Hide && p.Status != KeywordStatus.Delete );
        public IEnumerable<Keyword> TodayNeedReviewsKeywords => NeedReviewsKeywords.Where(p => p.Status==KeywordStatus.New|| p.UpdateAt.Date!=DateTime.Today);
        public IEnumerable<Keyword> NewKeywords => TotalKeywords.Where(p => p.Status == KeywordStatus.New );
        public int NeedReviewsKeywordsCount => NeedReviewsKeywords.Count() ;
        public int NewCount => NewKeywords.Count();


    }
}
