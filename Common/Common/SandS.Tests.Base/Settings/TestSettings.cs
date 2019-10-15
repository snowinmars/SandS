namespace SandS.Tests.Base.Settings
{
    // ReSharper disable once AllowPublicClass
    public sealed class TestSettings
    {
        internal TestSettings(TestSettingsModel model)
        {
            AssemblyNamePrefix = model.AssemblyNamePrefix;
            NamespacePrefix = model.NamespacePrefix;
        }

        public string AssemblyNamePrefix { get; }

        public string NamespacePrefix { get; }
    }
}