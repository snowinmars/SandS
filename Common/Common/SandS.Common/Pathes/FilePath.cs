using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SandS.Common.Pathes
{
   // ReSharper disable once AllowPublicClass
    public sealed class FilePath : AbsolutePath,
                                   IComparable,
                                   IComparable<AbsolutePath>,
                                   IEquatable<AbsolutePath>
    {
        public FilePath(string rawPath)
            : base(rawPath)
        {
            if (Exists && !IsExistingFile())
            {
                throw new InvalidOperationException($"File path was expected but '{rawPath}' was provided");
            }
        }

        public override string Name => Path.GetFileName(RawPath);

        public string Extension => Path.GetExtension(RawPath);

        [JsonIgnore]
        public override DirectoryPath Parent => Path.GetDirectoryName(RawPath).ToDirectoryPath();

        public override bool Exists => File.Exists(RawPath);

        public T GetContent<T>()
        {
            var data = GetContent();

            return JsonConvert.DeserializeObject<T>(data);
        }

        public string GetContent()
        {
            return File.ReadAllText(RawPath);
        }

        public string GetMd5()
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(RawPath))
                {
                    var hash = md5.ComputeHash(stream);

                    return BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
                }
            }
        }

        public void Touch()
        {
            if (!Parent.Exists)
            {
                Parent.Touch();
            }

            if (!Exists)
            {
                using (new FileStream(RawPath,
                                      FileMode.OpenOrCreate,
                                      FileAccess.Read,
                                      FileShare.ReadWrite,
                                      4096))
                {
                    // do nothing
                }
            }

            File.SetLastWriteTimeUtc(RawPath, DateTime.UtcNow);
        }

        public async Task TouchAsync()
        {
            if (!Parent.Exists)
            {
                await Parent.TouchAsync();
            }

            if (!Exists)
            {
                using (new FileStream(RawPath,
                                      FileMode.OpenOrCreate,
                                      FileAccess.Read,
                                      FileShare.ReadWrite,
                                      4096,
                                      FileOptions.Asynchronous))
                {
                    // do nothing
                }
            }

            File.SetLastWriteTimeUtc(RawPath, DateTime.UtcNow);
        }
    }
}