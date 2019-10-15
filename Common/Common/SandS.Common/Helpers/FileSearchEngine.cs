using System.Collections.Generic;
using System.IO;
using System.Linq;
using SandS.Common.Pathes;

namespace SandS.Common.Helpers
{
    internal sealed class FileSearchEngine : AbstractSearchEngine<FilePath>
    {
        public override FilePath[] FindAbove(FilePath path, string pattern)
        {
            var current = new DirectoryInfo(path);

            bool isRootFound;

            var files = new List<FilePath>();

            do
            {
                var file = current.EnumerateFiles(pattern, SearchOption.TopDirectoryOnly)
                                  .FirstOrDefault();

                if (file != default)
                {
                    files.Add(file.ToFilePath());
                }

                isRootFound = current.Parent is null || current.Root.FullName == current.Parent.FullName;
                current = current.Parent;
            }
            while (!isRootFound);

            return files.ToArray();
        }

        public override FilePath[] FindBelow(FilePath path, string pattern, SearchOption searchOption)
        {
            return Directory.EnumerateFiles(path,
                                            pattern,
                                            searchOption)
                            .Select(x => x.ToFilePath())
                            .ToArray();
        }
    }
}