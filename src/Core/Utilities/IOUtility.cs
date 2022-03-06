using System;
using System.IO;
using System.Text;

namespace Thismaker.Core.Utilities
{
    /// <summary>
    /// Contains utility methods for performing common cosmetic IO functions.
    /// </summary>
    public static class IOUtility
    {
        /// <summary>
        /// Converts a Windows OS path to the Unix format.
        /// </summary>
        /// <param name="path">The path to convert to Unix</param>
        /// <returns>The path converted to a valid unix path, e.g \home\faith\kay becomes /home/faith/kay.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string UnixifyPath(string path)
        {
            if(path == null)throw new ArgumentNullException(nameof(path));
            return path.Replace("\\", "/");
        }

        /// <summary>
        /// Returns the next available name for a specific file path.
        /// </summary>
        /// <remarks>
        /// Useful if you do not wish to overwrite an exisiting file, but retain the name and number it. 
        /// For example if the file '/home/faith.kay' exists, the function returns '/home/faith (1).kay'
        /// The appended pattern is determined from the passed number pattern.
        /// </remarks>
        /// <param name="path">The path for which availability should be tested.</param>
        /// <param name="numberPattern">The pattern to be applied in the event that the file already exists</param>
        /// <returns></returns>
        public static string NextAvailableFileName(string path, string numberPattern =" ({0})")
        {
            // Short-cut if already available
            if (!File.Exists(path))
                return path;

            // If path has extension then insert the number pattern just before the extension and return next filename
            if (Path.HasExtension(path))
                return GetNextFilename(path.Insert(path.LastIndexOf(Path.GetExtension(path)), numberPattern));

            // Otherwise just append the pattern to the path and return next filename
            return GetNextFilename(path + numberPattern);
        }

        private static string GetNextFilename(string pattern)
        {
            string tmp = string.Format(pattern, 1);
            if (tmp == pattern)
                throw new ArgumentException("The pattern must include an index place-holder", "pattern");

            if (!File.Exists(tmp))
                return tmp; // short-circuit if no matches

            int min = 1, max = 2; // min is inclusive, max is exclusive/untested

            while (File.Exists(string.Format(pattern, max)))
            {
                min = max;
                max *= 2;
            }

            while (max != min + 1)
            {
                int pivot = (max + min) / 2;
                if (File.Exists(string.Format(pattern, pivot)))
                    min = pivot;
                else
                    max = pivot;
            }

            return string.Format(pattern, max);
        }

        /// <summary>
        /// Returns the next available name for a specific directory path.
        /// </summary>
        /// <remarks>
        /// Useful if you do not wish to overwrite an exisiting directory, but retain the name and number it. 
        /// For example if the directory '/home/faith' exists, the function returns '/home/faith (1)'
        /// The appended pattern is determined from the passed number pattern.
        /// </remarks>
        /// <param name="path">The path for which availability should be tested.</param>
        /// <param name="numberPattern">The pattern to be applied in the event that the directory already exists</param>
        /// <returns></returns>
        public static string NextAvailableDirectoryName(string path, string numberPattern = " ({0})")
        {
            // Short-cut if already available
            if (!Directory.Exists(path))
                return path;

            // If path has extension then insert the number pattern just before the extension and return next Directoryname
            if (Path.HasExtension(path))
                return GetNextDirectoryname(path.Insert(path.LastIndexOf(Path.GetExtension(path)), numberPattern));

            // Otherwise just append the pattern to the path and return next Directoryname
            return GetNextDirectoryname(path + numberPattern);
        }

        private static string GetNextDirectoryname(string pattern)
        {
            string tmp = string.Format(pattern, 1);
            if (tmp == pattern)
                throw new ArgumentException("The pattern must include an index place-holder", "pattern");

            if (!Directory.Exists(tmp))
                return tmp; // short-circuit if no matches

            int min = 1, max = 2; // min is inclusive, max is exclusive/untested

            while (Directory.Exists(string.Format(pattern, max)))
            {
                min = max;
                max *= 2;
            }

            while (max != min + 1)
            {
                int pivot = (max + min) / 2;
                if (Directory.Exists(string.Format(pattern, pivot)))
                    min = pivot;
                else
                    max = pivot;
            }

            return string.Format(pattern, max);
        }

        /// <summary>
        /// Returns a neatly formatted string representing the size of a file e.g 8MB.
        /// </summary>
        /// <param name="size">The size of the file in bytes</param>
        /// <param name="decimals">The number of decimal places that can be displayed</param>
        public static string GetFileSizeReadable(long size, uint decimals=2)
        {
            // Get absolute value
            long absolute_i = (size < 0 ? -size : size);
            // Determine the suffix and readable value
            string suffix;
            double readable;
            if (absolute_i >= 0x1000000000000000) // Exabyte
            {
                suffix = "EB";
                readable = (size >> 50);
            }
            else if (absolute_i >= 0x4000000000000) // Petabyte
            {
                suffix = "PB";
                readable = (size >> 40);
            }
            else if (absolute_i >= 0x10000000000) // Terabyte
            {
                suffix = "TB";
                readable = (size >> 30);
            }
            else if (absolute_i >= 0x40000000) // Gigabyte
            {
                suffix = "GB";
                readable = (size >> 20);
            }
            else if (absolute_i >= 0x100000) // Megabyte
            {
                suffix = "MB";
                readable = (size >> 10);
            }
            else if (absolute_i >= 0x400) // Kilobyte
            {
                suffix = "KB";
                readable = size;
            }
            else
            {
                return size.ToString("0 B"); // Byte
            }
            // Divide by 1024 to get fractional value
            readable /= 1024;
            // Return formatted number with suffix

            if (decimals == 0)
            {
                return $"{((int)readable)}{suffix}";
            }

            StringBuilder builder = new StringBuilder("0.");

            for (int i = 0; i < decimals; i++)
            {
                builder.Append('#');
            }
            return $"{readable.ToString(builder.ToString())}{suffix}";
        }

        ///<inheritdoc cref="CombinePath(string, string, string)"/>
        public static string CombinePath(string path1, string path2)
        {
            if (path1 == null) return path2;
            else if (path2 == null) return path1;
            else return path1.Trim().TrimEnd(Path.DirectorySeparatorChar)
               + Path.DirectorySeparatorChar
               + path2.Trim().TrimStart(Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// Combines the provided paths into one.
        /// </summary>
        /// <remarks>
        /// When combining paths using the usual <see cref="Path.Combine(string, string)"/> methods, if the second path begins with a directory separator character,
        /// the first path is often omitted. This is because the second path is taken to be absolute. While that may be ideal in some situations, it may not be what is needed in some situations.
        /// In those kinds of scenarios, this function provides the better alternative.
        /// </remarks>
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