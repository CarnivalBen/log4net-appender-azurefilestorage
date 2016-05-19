using System;
using System.Text;
using System.Text.RegularExpressions;
using log4net.Core;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using System.IO;

namespace log4net.Appender.AzureFileStorage {

    public class AzureFileAppender : AppenderSkeleton {

        #region Configuration parameters

        /// <summary>
        /// The connection string for the Azure storage account that you want to use.
        /// </summary>
        public string AzureStorageConnectionString { get; set; }

        /// <summary>
        /// The share name to create within the storage account.
        /// </summary>
        public string ShareName { get; set; }

        /// <summary>
        /// The path within the share to which log files will be written.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The log filename.
        /// </summary>
        public string File { get; set; }

        #endregion

        private static string _thisConnectionString;
        private static CloudStorageAccount _storageAccount;
        private static CloudFileClient _client;
        private static CloudFileShare _share;
        private static string _thisFolder;
        private static CloudFileDirectory _folder;
        private static CloudFile _file;
        private static string _thisFile;
       

        private void Initialise(LoggingEvent loggingEvent) {

            if (_storageAccount == null || AzureStorageConnectionString != _thisConnectionString) {
                _storageAccount = CloudStorageAccount.Parse(AzureStorageConnectionString);
                _thisConnectionString = AzureStorageConnectionString;
                _client = null;
                _share = null;
                _folder = null;
                _file = null;
            }

            if (_client == null) {
                _client = _storageAccount.CreateCloudFileClient();
                _share = null;
                _folder = null;
                _file = null;
            }

            if (_share == null || _share.Name != ShareName) {
                _share = _client.GetShareReference(ShareName);
                _share.CreateIfNotExists();
                _folder = null;
                _file = null;
            }

            if (_folder == null || Path != _thisFolder) {
                var pathElements = Path.Split(new[] {'\\', '/'}, StringSplitOptions.RemoveEmptyEntries);

                _folder = _share.GetRootDirectoryReference();
                foreach (var element in pathElements) {
                    _folder = _folder.GetDirectoryReference(element);
                    _folder.CreateIfNotExists();
                }

                _thisFolder = Path;
                _file = null;
            }

            var filename = Regex.Replace(File, @"\{(.+?)\}", _ => loggingEvent.TimeStamp.ToString(_.Result("$1")));
            if (_file == null || filename != _thisFile) {                
                _file = _folder.GetFileReference(filename);
                if (!_file.Exists()) _file.Create(0);
                _thisFile = filename;
            }
            
        }

        protected override void Append(LoggingEvent loggingEvent) {
            
            Initialise(loggingEvent);

            var buffer = Encoding.UTF8.GetBytes(RenderLoggingEvent(loggingEvent));

            _file.Resize(_file.Properties.Length + buffer.Length);

            using (var fileStream = _file.OpenWrite(null)) {
                fileStream.Seek(buffer.Length * -1, SeekOrigin.End);
                fileStream.Write(buffer, 0, buffer.Length);
            }
        }
    }
}
