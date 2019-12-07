using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using SandS.Common.Pathes;

namespace SandS.Common.Helpers
{
    internal class SyncFileHelper
    {
        public SyncFileHelper(Action<string> logAction)
        {
            this.logAction = logAction;
        }

        private readonly Action<string> logAction;

        public void CopyFile(FilePath sourcePath, FilePath destinationPath)
        {
            logAction?.Invoke($"Copy {sourcePath}  ==>>  {destinationPath}");

            var buffer = new byte[4096];

            using (var source = new FileStream(sourcePath,
                                               FileMode.Open,
                                               FileAccess.Read,
                                               FileShare.None,
                                               buffer.Length))
            {
                WriteToFile(source, destinationPath);
            }
        }

        public FileInfo DownloadFile(Uri fileUri,
            DirectoryPath directory,
            string fileName,
            bool overwriteExistingFile = false)
        {
            return DownloadFile(fileUri, directory.CombineFile(fileName), overwriteExistingFile);
        }

        public FileInfo DownloadFile(Uri fileUri,
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

            var request = (HttpWebRequest)WebRequest.Create(fileUri);

            using (var content = request.GetResponse().GetResponseStream())
            {
                logAction?.Invoke($"Downloading {fileUri}  ==>>  {filePath}");
                WriteToFile(content, filePath);
            }

            return new FileInfo(filePath);
        }

        public string ReadFile(FilePath path)
        {
            logAction?.Invoke($"Reading from file {path}");

            using (var reader = File.OpenText(path))
            {
                return reader.ReadToEnd();
            }
        }

        public void Unzip(FilePath archivePath, DirectoryPath destination)
        {
            logAction?.Invoke($"Unzipping {archivePath}  ==>>  {destination}");

            ZipFile.ExtractToDirectory(archivePath, destination);
        }

        public void WriteToFile(string content, FilePath path)
        {
            logAction?.Invoke($"Writing string data to file {path}");

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
            {
                WriteToFile(stream, path);
            }
        }

        private void WriteToFile(Stream content, FilePath path)
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

                while ((bytesRead = content.Read(buffer, 0, buffer.Length)) > 0)
                {
                    destination.Write(buffer, 0, bytesRead);
                }

                destination.Flush();
            }
        }
    }
}