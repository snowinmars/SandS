using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NUnit.Framework;
using SandS.Common.Pathes;
using SandS.Tests.Base;
using SandS.Tests.Base.Settings;

namespace SandS.Tests.Architecture
{
    internal sealed class CsProj : BaseTest
    {
        private TestSettings settings;

        [OneTimeSetUp]
        public async Task InitAsync()
        {
            settings = await GetSettingsAsync();
        }

        [Test]
        public void Projects_ShouldHave_AssemblyPrefix_Async()
        {
            const string pattern = @".*?<AssemblyName>(.*?)<\/AssemblyName>";

            var solution = DirectoryPath.Current.FindParentFile("*.sln");

            if (solution is null)
            {
                return;
            }

            var projects = solution.Parent.FindChildFiles("*.csproj");
            var projectsWithWrongAssemblyPrefix = new List<Tuple<string, string>>(16);

            foreach (var project in projects)
            {
                var content = project.GetContent();

                var match = Regex.Match(content, pattern, RegexOptions.Compiled | RegexOptions.Singleline);

                if (match.Captures.Count == 0)
                {
                    projectsWithWrongAssemblyPrefix.Add(new Tuple<string, string>(project.Name, "REGEX NOT FOUND"));

                    continue;
                }

                foreach (Match capture in match.Captures)
                {
                    var group = capture.Groups[1];

                    if (!group.Value.StartsWith(settings.AssemblyNamePrefix))
                    {
                        projectsWithWrongAssemblyPrefix.Add(new Tuple<string, string>(project.Name, group.Value));
                    }
                }
            }

            if (projectsWithWrongAssemblyPrefix.Any())
            {
                var sep = $"{Environment.NewLine}   ";

                Assert.Fail($"{projectsWithWrongAssemblyPrefix.Count} " +
                            $"{(projectsWithWrongAssemblyPrefix.Count == 1 ? "project" : "projects")} {sep}" +
                            $"have wrong assembly prefix: {sep}" +
                            $"{string.Join($", {sep}", projectsWithWrongAssemblyPrefix.Select(x => $"{x.Item1} - {x.Item2}"))} {sep}");
            }
        }

        [Test]
        public void Projects_ShouldHave_NamespacePrefix_Async()
        {
            const string pattern = @".*?<RootNamespace>(.*?)<\/RootNamespace>";

            var solution = DirectoryPath.Current.FindParentFile("*.sln");

            if (solution is null)
            {
                return;
            }

            var projects = solution.Parent.FindChildFiles("*.csproj");
            var projectsWithWrongNamespacePrefix = new List<string>(16);

            foreach (var project in projects)
            {
                var content = project.GetContent();

                var match = Regex.Match(content, pattern, RegexOptions.Compiled | RegexOptions.Singleline);

                if (match.Captures.Count == 0)
                {
                    projectsWithWrongNamespacePrefix.Add(project.Name);

                    continue;
                }

                foreach (Match capture in match.Captures)
                {
                    var group = capture.Groups[1];

                    if (!group.Value.StartsWith(settings.NamespacePrefix))
                    {
                        projectsWithWrongNamespacePrefix.Add(project.Name);
                    }
                }
            }

            if (projectsWithWrongNamespacePrefix.Any())
            {
                var sep = $"{Environment.NewLine}   ";

                Assert.Fail($"{projectsWithWrongNamespacePrefix.Count} " +
                            $"{(projectsWithWrongNamespacePrefix.Count == 1 ? "project" : "projects")} {sep}" +
                            $"have wrong namespace prefix: {sep}" +
                            $"{string.Join($", {sep}", projectsWithWrongNamespacePrefix)} {sep}");
            }
        }

        [Test]
        public void Projects_ShouldLink_CommonAssemblyAttributes_Async()
        {
            const string pattern = @".*?<Link>AssemblyInfo\\CommonAssemblyAttributes.cs<\/Link>";

            var solution = DirectoryPath.Current.FindParentFile("*.sln");

            if (solution is null)
            {
                return;
            }

            var projects = solution.Parent.FindChildFiles("*.csproj");

            var projectsWithoutLink = new List<string>(16);

            foreach (var project in projects)
            {
                var content = project.GetContent();

                var match = Regex.Match(content, pattern, RegexOptions.Compiled | RegexOptions.Singleline);

                if (match.Captures.Count == 0)
                {
                    projectsWithoutLink.Add(project.Name);
                }
            }

            if (projectsWithoutLink.Any())
            {
                var sep = $"{Environment.NewLine}   ";

                Assert.Fail($"{projectsWithoutLink.Count} " +
                            $"{(projectsWithoutLink.Count == 1 ? "project" : "projects")} {sep}" +
                            $"don't have link to CommonAssemblyAttributes.cs file: {sep}" +
                            $"{string.Join($", {sep}", projectsWithoutLink)} {sep}");
            }
        }
    }
}