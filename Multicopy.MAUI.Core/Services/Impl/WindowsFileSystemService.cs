using Humanizer;
using Multicopy.MAUI.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multicopy.Core.Services.Impl
{
    public class WindowsFileSystemService : IFileSystemService
    {
        public int CountFilesInDirectory(string path, string searchPattern, SearchOption searchOption)
        {
            return Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories).Count();
        }

        public void StartProcess(string path, string arguments)
        {
            Process.Start(path, arguments);
        }

        public string? MakeFinalPathName(string sourcePath)
        {
            return System.IO.Path.EndsInDirectorySeparator(sourcePath) ? System.IO.Path.GetDirectoryName(sourcePath) : System.IO.Path.GetDirectoryName($"{sourcePath}{System.IO.Path.DirectorySeparatorChar}"); ;
        }

        public string MakeNewStartFolder(string? finalPathName, string destinationPath)
        {
            return finalPathName == null ? destinationPath : System.IO.Path.Combine(destinationPath, finalPathName.Split(System.IO.Path.DirectorySeparatorChar).Last());
        }

        public void CreateNewDirectoryIfNotExists(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public string[] GetFiles(string path, string searchString, SearchOption searchOption)
        {
            return Directory.GetFiles(path, "*", SearchOption.AllDirectories);
        }

        public string[] GetFiles(string path)
        {
            return Directory.GetFiles(path);
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public void DeleteFile(string path)
        {
            File.Delete(path);
        }

        public string[] GetDirectories(string path)
        {
            return Directory.GetDirectories(path);
        }

       public Stream GetStream(string sourceFile, FileMode fileMode, FileAccess fileAccess, FileShare fileShare, int bufferSize)
        {
            return new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize);
        }

        public Stream GetStream(string sourceFile, FileMode fileMode, FileAccess fileAccess, FileShare fileShare, int bufferSize, FileOptions fileOptions)
        {
            return new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, fileOptions);
        }
    }
}
