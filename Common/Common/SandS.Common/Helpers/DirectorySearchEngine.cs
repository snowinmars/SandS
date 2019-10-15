using System.Collections.Generic;
using System.IO;
using System.Linq;
using SandS.Common.Pathes;

namespace SandS.Common.Helpers
{
    internal sealed class DirectorySearchEngine : AbstractSearchEngine<DirectoryPath>
    {
        public override DirectoryPath[] FindAbove(DirectoryPath path, string pattern)
        {
            var current = new DirectoryInfo(path);

            bool isRootFound;

            var directories = new List<DirectoryPath>();

            do
            {
                var directory = current.EnumerateDirectories(pattern, SearchOption.TopDirectoryOnly)
                                       .FirstOrDefault();

                if (directory != default)
                {
                    directories.Add(directory.ToDirectoryPath());
                }

                isRootFound = current.Parent is null || current.Root.FullName == current.Parent.FullName;
                current = current.Parent;
            }
            while (!isRootFound);

            return directories.ToArray();
        }

        public override DirectoryPath[] FindBelow(DirectoryPath path, string pattern, SearchOption searchOption)
        {
            return Directory.EnumerateDirectories(path,
                                                  pattern,
                                                  searchOption)
                            .Select(x => x.ToDirectoryPath())
                            .ToArray();
        }
    }
}