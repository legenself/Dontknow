using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DontKnow.Service
{
    public class FileItem
    {
        public string Name { get; set; }
        public string Ext { get; set; }
        public long Size { get; set; }
        public FileItem(string path)
        {
            Name = path;
            Ext = Path.GetExtension(Ext);
        }
    }
}
