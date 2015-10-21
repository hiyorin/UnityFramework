using System.IO;
using System.Collections.Generic;

namespace Framework
{
    public sealed class FileUtility
    {
        public static bool CreateDirectory (string path)
        {
            if (string.IsNullOrEmpty (path) == true)
                return false;
            
            string[] splitPath = path.Split (Path.PathSeparator);
            string buffer = string.Empty;
            foreach (var row in splitPath)
            {
                buffer = Path.Combine (buffer, row);
                if (Directory.Exists (path) == false)
                    Directory.CreateDirectory (path);
            }
            return true;
        }

        public static void GetChildrenFile (string path, List<string> result)
        {
            if (result == null)
                return;
            if (Directory.Exists (path) == false)
                return;
            
        }
    }
}