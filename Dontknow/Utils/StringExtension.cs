using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DontKnow
{
    public static class StringExtension
    {
        public const char NUL = (char)0; // Null char
        public const char BS = (char)8; // Back Space
        public const char CR = (char)13; // Carriage Return
        public const char SUB = (char)26; // Substitute
        public static bool isControlChar(this int ch)
        {
            return (ch >NUL && ch < BS)
                || (ch >CR && ch < SUB);
        }
        public static bool IsDirectory(this string path)
        {
            FileInfo fi = new FileInfo(path);
            if ((fi.Attributes & FileAttributes.Directory) != 0)
            {
                return true;
            }
            return false;
        }
        public static bool isBinaryFile(this string path)
        {
            long length = new FileInfo(path).Length;
            if (length == 0) return false;

            using (StreamReader stream = new StreamReader(path))
            {
                int ch;
                while ((ch = stream.Read()) != -1)
                {
                    if (ch.isControlChar())
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static Encoding GetEncoding(this string filename)
        {
            // Read the BOM
            var bom = new byte[4];
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                file.Read(bom, 0, 4);
            }

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe && bom[2] == 0 && bom[3] == 0) return Encoding.UTF32; //UTF-32LE
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return new UTF32Encoding(true, true);  //UTF-32BE

            // We actually have no idea what the encoding is if we reach this point, so
            // you may wish to return null instead of defaulting to ASCII
            return Encoding.ASCII;
        }
    }
}
