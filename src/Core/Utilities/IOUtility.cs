using System.IO;

namespace Thismaker.Core.Utilities
{
    public static class IOUtility
    {
        /// <summary>
        /// Better at handling paths than <see cref="System.IO.Path.Combine(string, string)"/>
        /// This is because of a fundamental flaw that has been disccussed in StackOverflow.
        /// In fact this method has been constructed from the methods provided.
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <returns></returns>
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

        /// <summary>
        /// A quick way to perform a rename of a file.
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="newName"></param>
        public static void Rename(this FileInfo fileInfo, string newName)
        {
            fileInfo.MoveTo(fileInfo.Directory.FullName + Path.DirectorySeparatorChar + newName);
        }
    }
}
