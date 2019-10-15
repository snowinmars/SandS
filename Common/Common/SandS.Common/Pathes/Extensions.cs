using System;
using System.IO;

namespace SandS.Common.Pathes
{
    // ReSharper disable once AllowPublicClass
    public static class Extensions
    {
        public static AbsolutePath ToAbsolutePath(this string path)
        {
            if (AbsolutePath.IsExistingFile(path))
            {
                return path.ToFilePath();
            }

            if (AbsolutePath.IsExistingDirectory(path))
            {
                return path.ToDirectoryPath();
            }

            throw new
                InvalidOperationException($"Entity '{path}' is a file or a directory, but a third option was hit");
        }

        public static DirectoryPath ToDirectoryPath(this DirectoryInfo info)
        {
            return new DirectoryPath(info.FullName);
        }

        public static DirectoryPath ToDirectoryPath(this string path)
        {
            return new DirectoryPath(path);
        }

        public static FilePath ToFilePath(this FileInfo info)
        {
            return new FilePath(info.FullName);
        }

        public static FilePath ToFilePath(this string path)
        {
            return new FilePath(path);
        }
    }
}