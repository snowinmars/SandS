using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SandS.Common.Helpers;

namespace SandS.Common.Pathes
{
    // ReSharper disable once AllowPublicClass
    public sealed class DirectoryPath : AbsolutePath,
                                        IComparable,
                                        IComparable<AbsolutePath>,
                                        IEquatable<AbsolutePath>
    {
        public DirectoryPath(string rawPath)
            : base(rawPath)
        {
            if (Exists && !IsExistingDirectory())
            {
                throw new InvalidOperationException($"Directory path was expected but '{rawPath}' was provided");
            }
        }

        private static readonly FileSearchEngine FileSearchEngine = new FileSearchEngine();

        private static readonly DirectorySearchEngine DirectorySearchEngine = new DirectorySearchEngine();

        public static DirectoryPath Current => Assembly.GetExecutingAssembly().Location.ToFilePath().Parent;

        public override string Name => new DirectoryInfo(RawPath).Name;

        [JsonIgnore]
        public override DirectoryPath Parent => Directory.GetParent(RawPath)?.ToDirectoryPath();

        public override bool Exists => Directory.Exists(RawPath);

        public bool IsEmpty =>
            !FindChildFiles().Any() &&
            !FindChildDirectories().Any();

        public DirectoryPath Root => Directory.GetDirectoryRoot(RawPath).ToDirectoryPath();

        public DirectoryPath Clone()
        {
            return new DirectoryPath(RawPath);
        }

        public DirectoryPath CombineDirectory(string path1)
        {
            return Path.Combine(RawPath, path1).ToDirectoryPath();
        }

        public DirectoryPath CombineDirectory(string path1, string path2)
        {
            return Path.Combine(RawPath, path1, path2).ToDirectoryPath();
        }

        public AbsolutePath CombineDirectory(params string[] parts)
        {
            var current = this;

            foreach (var part in parts)
            {
                current = current.CombineDirectory(part);
            }

            return current;
        }

        public FilePath CombineFile(string filename)
        {
            return Path.Combine(RawPath, filename).ToFilePath();
        }

        public void Delete()
        {
            Directory.Delete(RawPath, true);
        }

        public DirectoryPath EnsureExists()
        {
            if (!Directory.Exists(RawPath))
            {
                Directory.CreateDirectory(RawPath);
            }

            return this;
        }

        public DirectoryPath[] FindChildDirectories(string pattern = "*")
        {
            return DirectorySearchEngine.FindBelow(RawPath.ToDirectoryPath(), pattern, SearchOption.AllDirectories);
        }

        public DirectoryPath FindChildDirectory(string pattern = "*",
            ActionOnNotFound actionOnNotFound = ActionOnNotFound.ReturnDefault)
        {
            var directories = FindChildDirectories(pattern);

            if (directories.Any())
            {
                return directories.First();
            }

            switch (actionOnNotFound)
            {
            case ActionOnNotFound.ReturnDefault:
                return default;

            case ActionOnNotFound.ThrowNewException:
                throw new DirectoryNotFoundException($"Can't find a child directory matching '{pattern}' in {RawPath}");

            default:
                throw new ArgumentOutOfRangeException(nameof(actionOnNotFound), actionOnNotFound, null);
            }
        }

        public FilePath FindChildFile(string pattern = "*",
            ActionOnNotFound actionOnNotFound = ActionOnNotFound.ReturnDefault)
        {
            var files = FindChildFiles(pattern);

            if (files.Any())
            {
                return files.First();
            }

            switch (actionOnNotFound)
            {
            case ActionOnNotFound.ReturnDefault:
                return default;

            case ActionOnNotFound.ThrowNewException:
                throw new FileNotFoundException($"Can't find a child file matching '{pattern}' in {RawPath}");

            default:
                throw new ArgumentOutOfRangeException(nameof(actionOnNotFound), actionOnNotFound, null);
            }
        }

        public FilePath[] FindChildFiles(string pattern = "*")
        {
            return FileSearchEngine.FindBelow(RawPath.ToFilePath(), pattern, SearchOption.AllDirectories);
        }

        public DirectoryPath[] FindParentDirectories(string pattern = "*")
        {
            return DirectorySearchEngine.FindAbove(RawPath.ToDirectoryPath(), pattern);
        }

        public DirectoryPath FindParentDirectory(string pattern = "*",
            ActionOnNotFound actionOnNotFound = ActionOnNotFound.ReturnDefault)
        {
            var directories = FindParentDirectories(pattern);

            if (directories.Any())
            {
                return directories.First();
            }

            switch (actionOnNotFound)
            {
            case ActionOnNotFound.ReturnDefault:
                return default;

            case ActionOnNotFound.ThrowNewException:
                throw
                    new
                        DirectoryNotFoundException($"Can't find a parent directory matching '{pattern}' in {RawPath}");

            default:
                throw new ArgumentOutOfRangeException(nameof(actionOnNotFound), actionOnNotFound, null);
            }
        }

        public FilePath FindParentFile(string pattern = "*",
            ActionOnNotFound actionOnNotFound = ActionOnNotFound.ReturnDefault)
        {
            var files = FindParentFiles(pattern);

            if (files.Any())
            {
                return files.First();
            }

            switch (actionOnNotFound)
            {
            case ActionOnNotFound.ReturnDefault:
                return default;

            case ActionOnNotFound.ThrowNewException:
                throw new FileNotFoundException($"Can't find a parent file matching '{pattern}' in {RawPath}");

            default:
                throw new ArgumentOutOfRangeException(nameof(actionOnNotFound), actionOnNotFound, null);
            }
        }

        public FilePath[] FindParentFiles(string pattern = "*")
        {
            return FileSearchEngine.FindAbove(RawPath.ToFilePath(), pattern);
        }

        public DirectoryPath[] FindSiblingDirectories(string pattern = "*")
        {
            return DirectorySearchEngine.FindBelow(RawPath.ToDirectoryPath(), pattern, SearchOption.TopDirectoryOnly);
        }

        public DirectoryPath FindSiblingDirectory(string pattern = "*",
            ActionOnNotFound actionOnNotFound = ActionOnNotFound.ReturnDefault)
        {
            var directories = FindSiblingDirectories(pattern);

            if (directories.Any())
            {
                return directories.First();
            }

            switch (actionOnNotFound)
            {
            case ActionOnNotFound.ReturnDefault:
                return default;

            case ActionOnNotFound.ThrowNewException:
                throw new
                    DirectoryNotFoundException($"Can't find a sibling directory matching '{pattern}' in {RawPath}");

            default:
                throw new ArgumentOutOfRangeException(nameof(actionOnNotFound), actionOnNotFound, null);
            }
        }

        public FilePath FindSiblingFile(string pattern = "*",
            ActionOnNotFound actionOnNotFound = ActionOnNotFound.ReturnDefault)
        {
            var files = FindSiblingFiles(pattern);

            if (files.Any())
            {
                return files.First();
            }

            switch (actionOnNotFound)
            {
            case ActionOnNotFound.ReturnDefault:
                return default;

            case ActionOnNotFound.ThrowNewException:
                throw new FileNotFoundException($"Can't find a sibling file matching '{pattern}' in {RawPath}");

            default:
                throw new ArgumentOutOfRangeException(nameof(actionOnNotFound), actionOnNotFound, null);
            }
        }

        public FilePath[] FindSiblingFiles(string pattern = "*")
        {
            return FileSearchEngine.FindBelow(RawPath.ToFilePath(), pattern, SearchOption.TopDirectoryOnly);
        }

        public DirectoryPath Touch()
        {
            if (Exists)
            {
                return this;
            }

            if (!Parent.Exists)
            {
                return Parent.Touch();
            }

            return Directory.CreateDirectory(RawPath).ToDirectoryPath();
        }

        public async Task<DirectoryPath> TouchAsync()
        {
            if (Exists)
            {
                return this;
            }

            if (!Parent.Exists)
            {
                return await Parent.TouchAsync();
            }

            return Directory.CreateDirectory(RawPath).ToDirectoryPath();
        }
    }
}
