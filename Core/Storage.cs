using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Thismaker.Core
{
    public static class Storage
    {
        public static string CombinePath(string path1, string path2)
        {
            if (path1 == null) return path2;
            else if (path2 == null) return path1;
            else return path1.Trim().TrimEnd(Path.DirectorySeparatorChar)
               + Path.DirectorySeparatorChar
               + path2.Trim().TrimStart(Path.DirectorySeparatorChar);
        }

        public static string CombinePath(string path1, string path2, string path3)
        {
            return CombinePath(CombinePath(path1, path2), path3);
        }

        public static void Rename(this FileInfo fileInfo, string newName)
        {
            fileInfo.MoveTo(fileInfo.Directory.FullName + Path.DirectorySeparatorChar + newName);
        }
    }
}
