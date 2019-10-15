using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using SandS.Common.Pathes;
using SandS.Tests.Base;

namespace SandS.Tests.Architecture
{
    internal sealed class Sln : BaseTest
    {
        [Test]
        public void AdobeDI_ShouldBe_Resolvable()
        {
            // Resolver.RegisterAdobe();
        }

        [Test]
        public void CommonAssemblyAttribute_ShouldBe_Unique()
        {
            const string fileName = "CommonAssemblyAttributes.cs";

            var solution = DirectoryPath.Current.FindParentFile("*.sln");

            if (solution is null)
            {
                return;
            }

            var file = solution.Parent
                               .FindChildFiles(fileName)
                               .SingleOrDefault();

            if (file == default)
            {
                Assert.Fail($"More that one {fileName} was found in the solution folder. The current approach assumes projects to link to a single {fileName} file");
            }
        }

        [Test]
        public void SolutionBuildDeclaration_ShouldMatch_ItConfiguration_Async()
        {
            // check two groupes                         guid          ...           Debug ...   =  Debug ...
            const string pattern = @"\{[A-F0-9]{8}(?:-[A-F0-9]{4}){3}-[A-F0-9]{12}\}\.(.*?)\|.*?\ =\ (.*?)\|";
            var solution = DirectoryPath.Current.FindParentFile("*.sln");

            if (solution is null)
            {
                return;
            }

            var solutions = solution.Parent.FindChildFiles("*.sln");
            var solutionsWithMisspelling = new List<string>(16);

            foreach (var sln in solutions)
            {
                var content = sln.GetContent();

                var match = Regex.Match(content, pattern, RegexOptions.Compiled | RegexOptions.Singleline);

                Assert.AreEqual(3, match.Groups.Count);

                var buildDeclaration = match.Groups[1].Value;
                var buildConfiguration = match.Groups[2].Value;

                if (buildDeclaration != buildConfiguration)
                {
                    solutionsWithMisspelling.Add(sln.RawPath);
                }
            }

            if (solutionsWithMisspelling.Any())
            {
                var sep = $"{Environment.NewLine}   ";
                Assert.Fail($"Solutions {sep} {string.Join($", {sep}", solutionsWithMisspelling)} {sep} don't match some projects' build declaration and build configuration. This is super critical");
            }
        }
    }
}