using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SandS.Common.Pathes;

namespace SandS.Common.Helpers
{
    internal class AsyncFileHelper
    {
         public AsyncFileHelper(Action<string> logAction)
        {
            this.logAction = logAction;
        }

        private readonly Action<string> logAction;

        public async Task CopyFileAsync(FilePath sourcePath, FilePath destinationPath)
        {
            logAction?.Invoke($"Copy {sourcePath}  ==>>  {destinationPath}");

            var buffer = new byte[4096];

            using (var source = new FileStream(sourcePath,
                                               FileMode.Open,
                                               FileAccess.Read,
                                               FileShare.None,
                                               buffer.Length,
                                               FileOptions.Asynchronous))
            {
                await WriteToFileAsync(source, destinationPath);
            }
        }

        public async Task<FileInfo> DownloadFileAsync(Uri fileUri,
            DirectoryPath directory,
            string fileName,
            bool overwriteExistingFile = false)
        {
            return await DownloadFileAsync(fileUri, directory.CombineFile(fileName), overwriteExistingFile);
        }

        public async Task<FileInfo> DownloadFileAsync(Uri fileUri,
            FilePath filePath,
            bool overwriteExistingFile = false)
        {
            if (File.Exists(filePath))
            {
                if (overwriteExistingFile)
                {
                    File.Delete(filePath);
                }
                else
                {
                    throw new InvalidOperationException($"File {filePath} already exists");
                }
            }

            logAction?.Invoke($"Send downloading request to {fileUri}");

            using (var client = new HttpClient())
            using (var content = await client.GetStreamAsync(fileUri))
            {
                logAction?.Invoke($"Downloading {fileUri}  ==>>  {filePath}");
                await WriteToFileAsync(content, filePath);
            }

            return new FileInfo(filePath);
        }

        public async Task MonitorFileTailAsync(FilePath path,
            Func<string, Task> onChange,
            TimeSpan period = default,
            CancellationToken cancellationToken = default)
        {
            if (period == default)
            {
                period = TimeSpan.FromSeconds(1);
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }

            if (onChange is null)
            {
                throw new ArgumentNullException(nameof(onChange));
            }

            const int bufferLength = 4096;

            var initialFileLength = new FileInfo(path).Length;
            var lastReadLength = initialFileLength - bufferLength;

            if (lastReadLength < 0)
            {
                lastReadLength = 0;
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                var fileLength = new FileInfo(path).Length;

                if (fileLength < lastReadLength)
                {
                    throw new
                        IOException("Something was deleted from the watched file. Deletion events doesn't support now");
                }

                if (fileLength > lastReadLength)
                {
                    using (var fileStream = new FileStream(path,
                                                           FileMode.Open,
                                                           FileAccess.Read,
                                                           FileShare.ReadWrite,
                                                           bufferLength,
                                                           FileOptions.Asynchronous))
                    {
                        if (!fileStream.CanSeek)
                        {
                            throw new InvalidOperationException("Can't seek the file watch stream");
                        }

                        fileStream.Seek(lastReadLength, SeekOrigin.Begin);

                        var buffer = new byte[bufferLength];

                        int bytesRead;

                        while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) >
                               0)
                        {
                            lastReadLength += bytesRead;

                            var text = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                            await onChange(text);
                        }
                    }
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                await Task.Delay(period, cancellationToken);
            }
        }

        public async Task<string> ReadFileAsync(FilePath path)
        {
            logAction?.Invoke($"Reading from file {path}");

            using (var reader = File.OpenText(path))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public async Task UnzipAsync(FilePath archivePath, DirectoryPath destination)
        {
            logAction?.Invoke($"Unzipping {archivePath}  ==>>  {destination}");

            await Task.Run(() => ZipFile.ExtractToDirectory(archivePath, destination));
        }

        public async Task WriteToFileAsync(string content, FilePath path)
        {
            logAction?.Invoke($"Writing string data to file {path}");

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
            {
                await WriteToFileAsync(stream, path);
            }
        }

        private async Task WriteToFileAsync(Stream content, FilePath path)
        {
            var buffer = new byte[4096];

            if (!content.CanRead)
            {
                throw new Exception();
            }

            using (var destination = new FileStream(path,
                                                    FileMode.OpenOrCreate,
                                                    FileAccess.ReadWrite,
                                                    FileShare.None,
                                                    buffer.Length,
                                                    FileOptions.Asynchronous))
            {
                int bytesRead;

                while ((bytesRead = await content.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await destination.WriteAsync(buffer, 0, bytesRead);
                }

                await destination.FlushAsync();
            }
        }
    }
}