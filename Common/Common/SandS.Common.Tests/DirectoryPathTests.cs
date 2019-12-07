using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SandS.Common.Pathes;
using Xunit;

namespace SandS.Common.Tests
{
    // ReSharper disable once AllowPublicClass
    public sealed class DirectoryPathTests
    {
        private readonly Randomize.TextSettings directoryNameSettings = Randomize.TextSettings.AllowNumbers |
                                                                        Randomize.TextSettings.AllowPunctuation |
                                                                        Randomize.TextSettings.AllowSpace |
                                                                        Randomize.TextSettings.AllowBigLetters |
                                                                        Randomize.TextSettings.AllowSmallLetters;

        private FilePath[] GenerateFiles(DirectoryPath directory, int filesCount, int nestingCount)
        {
            directory.Touch();

            var files = new List<FilePath>();

            for (var i = 0; i < filesCount; i++)
            {
                var currentDirectory = directory.Clone();

                for (var j = 0; j < nestingCount; j++)
                {
                    var directoryName = Randomize.String(5, 10, directoryNameSettings);
                    currentDirectory = currentDirectory.CombineDirectory(directoryName);
                }

                var fileName = Randomize.String(5, 10, directoryNameSettings);
                currentDirectory.CombineFile(fileName);
            }

            return files.ToArray();
        }

        [Fact]
        public void CombineDirectory()
        {
            var directoryNames = Randomize.Array(() => Randomize.String(5, 10, directoryNameSettings), 10);

            foreach (var directoryName in directoryNames)
            {
                var newDirectory = DirectoryPath.Current.CombineDirectory(directoryName);

                Assert.False(newDirectory.Exists);
                Assert.Equal(directoryName, newDirectory.Name);
                Assert.Equal(DirectoryPath.Current, newDirectory.Parent);

                newDirectory.Touch();

                Assert.True(newDirectory.Exists);
                Assert.True(newDirectory.IsEmpty);
                Assert.Equal(directoryName, newDirectory.Name);
                Assert.Equal(DirectoryPath.Current, newDirectory.Parent);
            }
        }

        [Fact]
        public void CombineFile()
        {
            var settings = Randomize.TextSettings.AllowNumbers |
                           Randomize.TextSettings.AllowPunctuation |
                           Randomize.TextSettings.AllowSpace |
                           Randomize.TextSettings.AllowBigLetters |
                           Randomize.TextSettings.AllowSmallLetters;

            var fileNames = Randomize.Array(() => Randomize.String(5, 10, settings), 10);

            foreach (var fileName in fileNames)
            {
                var newFile = DirectoryPath.Current.CombineFile(fileName);

                Assert.False(newFile.Exists);
                Assert.Equal(fileName, newFile.Name);
                Assert.Equal(DirectoryPath.Current, newFile.Parent);

                newFile.Touch();

                Assert.True(newFile.Exists);
                Assert.Equal(fileName, newFile.Name);
                Assert.Equal(DirectoryPath.Current, newFile.Parent);
            }
        }

        [Fact]
        public void CurrentProperty()
        {
            Assert.Equal(DirectoryPath.Current, Directory.GetCurrentDirectory(), StringComparer.InvariantCulture);
            Assert.True(DirectoryPath.Current.Exists);

            Assert.Equal(DirectoryPath.Current.RawPath,
                         Directory.GetCurrentDirectory(),
                         StringComparer.InvariantCulture);

            Assert.Equal(DirectoryPath.Current.Name, new DirectoryInfo(Directory.GetCurrentDirectory()).Name);

            Assert.Equal(DirectoryPath.Current.Parent,
                         new DirectoryInfo(Directory.GetCurrentDirectory()).Parent?.FullName);
        }

        [Fact]
        public void Delete()
        {
            var directoryNames = Randomize.Array(() => Randomize.String(5, 10, directoryNameSettings), 10);

            foreach (var directoryName in directoryNames)
            {
                var newDirectory = DirectoryPath.Current.CombineDirectory(directoryName);

                Assert.False(newDirectory.Exists);

                newDirectory.Touch();

                Assert.True(newDirectory.Exists);

                newDirectory.Delete();

                Assert.False(newDirectory.Exists);
            }
        }

        [Fact]
        public void FindChildDirectory()
        {
            var origin = DirectoryPath.Current.CombineDirectory("tmp").Touch();

            var files = GenerateFiles(origin, Randomize.Int(2, 13), Randomize.Int(0, 3));

            foreach (var directory in files.Select(x => x.Parent))
            {
                var foundDirectory = origin.FindChildDirectory(directory.Name, ActionOnNotFound.ThrowNewException);

                Assert.Equal(directory, foundDirectory);
                Assert.Equal(directory.Name, foundDirectory.Name);

                var subname = directory.Name.Substring(Randomize.Int(1, directory.Name.Length - 2));
                var foundDirectories = origin.FindChildDirectories(subname);

                Assert.True(foundDirectories.Any());
            }
        }

        [Fact]
        public void FindChildFile()
        {
            var origin = DirectoryPath.Current.CombineDirectory("tmp").Touch();

            var files = GenerateFiles(origin, Randomize.Int(2, 13), Randomize.Int(0, 3));

            foreach (var file in files)
            {
                var foundFile = origin.FindChildFile(file.Name, ActionOnNotFound.ThrowNewException);

                Assert.Equal(file, foundFile);
                Assert.Equal(file.Name, foundFile.Name);
                Assert.Equal(file.Extension, foundFile.Extension);

                var subname = file.Name.Substring(Randomize.Int(1, file.Name.Length - 2));
                var foundFiles = origin.FindChildFiles(subname);

                Assert.True(foundFiles.Any());
            }
        }

        [Fact]
        public void FindParentDirectory()
        {
            var origin = DirectoryPath.Current.CombineDirectory("t")
                                      .CombineDirectory("m")
                                      .CombineDirectory("p")
                                      .Touch();

            var files = GenerateFiles(DirectoryPath.Current, Randomize.Int(2, 13), Randomize.Int(1, 5));

            foreach (var directory in files.Select(x => x.Parent))
            {
                var foundDirectory = origin.FindParentDirectory(directory.Name, ActionOnNotFound.ThrowNewException);

                Assert.Equal(directory, foundDirectory);
                Assert.Equal(directory.Name, foundDirectory.Name);

                var subname = directory.Name.Substring(Randomize.Int(1, directory.Name.Length - 2));
                var foundDirectories = origin.FindParentDirectories(subname);

                Assert.True(foundDirectories.Any());
            }
        }

        [Fact]
        public void FindParentFile()
        {
            var origin = DirectoryPath.Current.CombineDirectory("t")
                                      .CombineDirectory("m")
                                      .CombineDirectory("p")
                                      .Touch();

            var files = GenerateFiles(DirectoryPath.Current, Randomize.Int(2, 13), Randomize.Int(1, 5));

            foreach (var file in files)
            {
                var foundFile = origin.FindParentFile(file.Name, ActionOnNotFound.ThrowNewException);

                Assert.Equal(file, foundFile);
                Assert.Equal(file.Name, foundFile.Name);
                Assert.Equal(file.Extension, foundFile.Extension);

                var subname = file.Name.Substring(Randomize.Int(1, file.Name.Length - 2));
                var foundFiles = origin.FindParentFiles(subname);

                Assert.True(foundFiles.Any());
            }
        }

        [Fact]
        public void FindSiblingDirectory()
        {
            var origin = DirectoryPath.Current.CombineDirectory("tmp").Touch();

            var files = GenerateFiles(origin, Randomize.Int(2, 13), 0);

            foreach (var directory in files.Select(x => x.Parent))
            {
                var foundDirectory = origin.FindSiblingDirectory(directory.Name, ActionOnNotFound.ThrowNewException);

                Assert.Equal(directory, foundDirectory);
                Assert.Equal(directory.Name, foundDirectory.Name);

                var subname = directory.Name.Substring(Randomize.Int(1, directory.Name.Length - 2));
                var foundDirectories = origin.FindSiblingDirectories(subname);

                Assert.True(foundDirectories.Any());
            }
        }

        [Fact]
        public void FindSiblingFile()
        {
            var origin = DirectoryPath.Current.CombineDirectory("tmp").Touch();

            var files = GenerateFiles(origin, Randomize.Int(2, 13), 0);

            foreach (var file in files)
            {
                var foundFile = origin.FindSiblingFile(file.Name, ActionOnNotFound.ThrowNewException);

                Assert.Equal(file, foundFile);
                Assert.Equal(file.Name, foundFile.Name);
                Assert.Equal(file.Extension, foundFile.Extension);

                var subname = file.Name.Substring(Randomize.Int(1, file.Name.Length - 2));
                var foundFiles = origin.FindSiblingFiles(subname);

                Assert.True(foundFiles.Any());
            }
        }

        [Fact]
        public void Root()
        {
            Assert.Null(DirectoryPath.Current.Root.Parent);
        }
    }
}
