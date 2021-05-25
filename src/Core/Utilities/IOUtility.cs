using System.IO;

namespace Thismaker.Core.Utilities
{
    public static class IOUtility
    {

        public static string GetFileSizeReadable(long i)
        {
            // Get absolute value
            long absolute_i = (i < 0 ? -i : i);
            // Determine the suffix and readable value
            string suffix;
            double readable;
            if (absolute_i >= 0x1000000000000000) // Exabyte
            {
                suffix = "EB";
                readable = (i >> 50);
            }
            else if (absolute_i >= 0x4000000000000) // Petabyte
            {
                suffix = "PB";
                readable = (i >> 40);
            }
            else if (absolute_i >= 0x10000000000) // Terabyte
            {
                suffix = "TB";
                readable = (i >> 30);
            }
            else if (absolute_i >= 0x40000000) // Gigabyte
            {
                suffix = "GB";
                readable = (i >> 20);
            }
            else if (absolute_i >= 0x100000) // Megabyte
            {
                suffix = "MB";
                readable = (i >> 10);
            }
            else if (absolute_i >= 0x400) // Kilobyte
            {
                suffix = "KB";
                readable = i;
            }
            else
            {
                return i.ToString("0 B"); // Byte
            }
            // Divide by 1024 to get fractional value
            readable /= 1024;
            // Return formatted number with suffix
            return readable.ToString("0.### ") + suffix;
        }

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