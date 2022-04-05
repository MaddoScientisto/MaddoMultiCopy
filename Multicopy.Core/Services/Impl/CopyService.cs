using Humanizer;
using IOExtensions;
using Multicopy.Core.Services;
using Multicopy.MAUI.Core.Services;
using Multicopy.MAUI.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multicopy.MAUI.Services.Impl
{


    public class CopyService : ICopyService
    {
        private readonly IFolderPicker _folderPicker;
        private readonly IFileSystemService _fileSystem;
        public CopyService(IFolderPicker folderPicker, IFileSystemService fileSystem)
        {
            _folderPicker = folderPicker;
            _fileSystem = fileSystem;
        }
        public DestinationPathInfo Build()
        {
            return new DestinationPathInfo();
        }

        public void Progress(DestinationPathInfo dpi, TransferProgress obj)
        {

            //Console.WriteLine($"Progress: {obj.Transferred}/{obj.Total}");

            dpi.FilesToCopy = Convert.ToInt32(obj.Total);
            dpi.FilesCopied = Convert.ToInt32(obj.Transferred);
            dpi.CurrentFileName = new FileInfo(obj.ProcessedFile).Name;
            if (obj.BytesPerSecond != 0)
            {
                dpi.Speed = obj.GetDataPerSecondFormatted(SuffixStyle.Windows, 2);
            }

        }

        public async Task GetDestinationFolder(DestinationPathInfo dpi)
        {
            var path = await _folderPicker.PickFolder();

            if (!string.IsNullOrWhiteSpace(path))
            {
                dpi.DestinationPath = path;
            }


        }

        public void OpenFolder(DestinationPathInfo dpi)
        {
            _fileSystem.StartProcess("explorer.exe", dpi.DestinationPath);           
        }

        public async Task DoCopyAsync(DestinationPathInfo dpi, string sourcePath, CancellationToken cancellationToken, Action Tick)
        {
            
            int allFiles = _fileSystem.CountFilesInDirectory(sourcePath, "*", SearchOption.AllDirectories);
            dpi.FilesToCopy = allFiles;
            dpi.FilesCopied = 0;
            dpi.FilesOverwritten = 0;
            dpi.FilesRenamed = 0;
            dpi.FilesSkipped = 0;
            dpi.FilesDeleted = 0;

            if (dpi.CreateTopFolder)
            {
              

                var newStartFolder = _fileSystem.MakeNewStartFolder(_fileSystem.MakeFinalPathName(sourcePath), dpi.DestinationPath);

                _fileSystem.CreateNewDirectoryIfNotExists(newStartFolder);

                if (dpi.FileSeparation)
                {
                    await CopyWithSeparationAsync(dpi, sourcePath, newStartFolder, cancellationToken, Tick);
                }
                else
                {
                    await CopyFolderAsync(dpi, sourcePath, newStartFolder, cancellationToken, Tick);
                }


            }
            else
            {
                if (dpi.FileSeparation)
                {
                    await CopyWithSeparationAsync(dpi, sourcePath, dpi.DestinationPath, cancellationToken, Tick);
                }
                else
                {
                    await CopyFolderAsync(dpi, sourcePath, dpi.DestinationPath, cancellationToken, Tick);
                }

            }




        }

        private async Task CopyWithSeparationAsync(DestinationPathInfo dpi, string source, string destination, CancellationToken cancellationToken, Action Tick)
        {

            string[] files = _fileSystem.GetFiles(source, "*", SearchOption.AllDirectories);
            
            var extsDic = SeparateByExtension(files);

            foreach (var ext in extsDic)
            {

                var newPath = System.IO.Path.Combine(destination, ext.Key.Remove(0, 1)); // dotless extension

                _fileSystem.CreateNewDirectoryIfNotExists(newPath);
                
                foreach (var file in ext.Value)
                {
                    var destName = TryGetDestinationFileName(dpi, file, newPath, out var exists);

                    if (exists && dpi.ConflictType == FileConflictType.Skip)
                    {
                        dpi.FilesCopied++;
                        dpi.CurrentFileName = destName;
                        Tick?.Invoke();
                        continue;
                    }

                    await CopyFileAsync(dpi, file, destName, cancellationToken, Tick);
                    //await CopyFileAsync(ext.)
                }


            }
        }

        private Dictionary<string, List<string>> SeparateByExtension(string[] paths)
        {
            Dictionary<string, List<string>> dicktionary = new Dictionary<string, List<string>>();

            foreach (var path in paths)
            {
                var ext = System.IO.Path.GetExtension(path);
                if (ext != null)
                {
                    if (dicktionary.ContainsKey(ext))
                    {
                        dicktionary[ext].Add(path);

                    }
                    else
                    {
                        dicktionary.Add(ext, new List<string>() { path });
                    }
                }


            }
            return dicktionary;
        }

        private string TryGetDestinationFileName(DestinationPathInfo dpi, string source, string destinationFolder, out bool exists)
        {
            string name = new FileInfo(source).Name;// .GetFileName( file );
            string dest = System.IO.Path.Combine(destinationFolder, name);

            // Check if file exists
            if (_fileSystem.FileExists(dest))
            {
                exists = true;
                switch (dpi.ConflictType)
                {
                    case FileConflictType.Overwrite:
                        _fileSystem.DeleteFile(dest);
                        dpi.FilesOverwritten++;
                        dpi.FilesDeleted++;
                        return dest;
                    case FileConflictType.Rename:
                        {
                            dpi.FilesRenamed++;

                            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(dest);
                            var ext = Path.GetExtension(dest);
                            int i = 1;
                            while (i < 1000) // arbitrary number
                            {
                                var newName = $"{fileNameWithoutExtension} ({i}){ext}";
                                var newPath = Path.Combine(destinationFolder, newName);
                                if (!_fileSystem.FileExists(newPath))
                                {
                                    dest = newPath;
                                    break;
                                }
                                i++;
                            }
                            return dest;

                            // 1: try to add (1)
                            // 2: check if file with (1) exists
                            // 3: if exists make it (n+1)
                            // 4: try until it does not exist
                        }
                    case FileConflictType.Skip:
                        dpi.FilesSkipped++;
                        return dest;
                    default:
                        return dest;
                }
            }


            exists = false;
            return dest;

        }

        private async Task CopyFolderAsync(DestinationPathInfo dpi, string source, string destinationFolder, CancellationToken cancellationToken, Action? Tick)
        {
            _fileSystem.CreateNewDirectoryIfNotExists(destinationFolder);
            
            string[] files = _fileSystem.GetFiles(source);
            foreach (string file in files)
            {


                var destName = TryGetDestinationFileName(dpi, file, destinationFolder, out var fileExists);
                if (fileExists && dpi.ConflictType == FileConflictType.Skip)
                {
                    dpi.FilesCopied++;
                    dpi.CurrentFileName = destName;
                    Tick?.Invoke();
                    continue;
                }


                await CopyFileAsync(dpi, file, destName, cancellationToken, Tick);
            }
            string[] folders = _fileSystem.GetDirectories(source);
            foreach (string folder in folders)
            {
                string name = new DirectoryInfo(folder).Name;// Path.GetFileName(folder);
                string dest = System.IO.Path.Combine(destinationFolder, name);// Path.Combine(destFolder, name);
                                                                              //CopyFolder(folder, dest);
                await CopyFolderAsync(dpi, folder, dest, cancellationToken, Tick);
            }
        }




        private async Task CopyFileAsync(DestinationPathInfo dpi, string sourceFile, string destrinationFile, CancellationToken token, Action? Tick)
        {
            var bufferSize = 0x10000;
            var fileOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;
            var timer = new Stopwatch();
            timer.Start();
            await using Stream sourceStream = _fileSystem.GetStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize);
            await using Stream destinationStream = _fileSystem.GetStream(destrinationFile, FileMode.CreateNew, FileAccess.Write, FileShare.None, bufferSize, fileOptions);
            var size = sourceStream.Length;
            await sourceStream.CopyToAsync(destinationStream, bufferSize, token).ConfigureAwait(false);
            if (dpi.DoMove)
            {
                _fileSystem.DeleteFile(sourceFile);             
                dpi.FilesDeleted++;
            }
            timer.Stop();

            dpi.CurrentCopyTime = timer.Elapsed.Humanize();

            dpi.Speed = size.Bytes().Per(timer.Elapsed).Humanize("#.##");

            dpi.FilesCopied++;
            dpi.CurrentFileName = destrinationFile;
            Tick?.Invoke();

        }

    }
}
