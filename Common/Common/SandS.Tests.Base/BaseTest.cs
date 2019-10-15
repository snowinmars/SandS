using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SandS.Tests.Base.Settings;

namespace SandS.Tests.Base
{
    internal abstract class BaseTest
    {
        protected BaseTest()
        {
            SetWorkingDirectory();
        }

        public async Task<TestSettings> GetSettingsAsync()
        {
            var settingsFilename = "settings.json";
            var json = await ReadFileContentAsync(new FileInfo(settingsFilename));
            var model = JsonConvert.DeserializeObject<TestSettingsModel>(json);
            var settings = new TestSettings(model);

            return settings;
        }

        protected async Task<string> ReadFileContentAsync(FileInfo fileInfo)
        {
            using (var stream = fileInfo.OpenRead())
            using (var streamReader = new StreamReader(stream))
            {
                return await streamReader.ReadToEndAsync().ConfigureAwait(false);
            }
        }

        private void SetWorkingDirectory()
        {
            // Service default directory is System32, that doesn't make any sense to me
            // I set it to the main .exe file location
            var assemblyLocationFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (!string.Equals(Environment.CurrentDirectory,
                               assemblyLocationFolder,
                               StringComparison.OrdinalIgnoreCase))
            {
                Environment.CurrentDirectory = assemblyLocationFolder;
            }
        }
    }
}