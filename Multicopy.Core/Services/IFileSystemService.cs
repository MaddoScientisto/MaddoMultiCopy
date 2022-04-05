using Multicopy.MAUI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multicopy.Core.Services
{
    public interface IFileSystemService
    {
        int CountFilesInDirectory(string path, string searchPattern, SearchOption searchOption);
        void StartProcess(string path, string arguments);
        string? MakeFinalPathName(string sourcePath);
        string MakeNewStartFolder(string? finalPathName, string destinationPath);

        void CreateNewDirectoryIfNotExists(string path);
        string[] GetFiles(string path, string searchString, SearchOption searchOption);
        bool FileExists(string path);
        void DeleteFile(string path);
        string[] GetFiles(string path);
        string[] GetDirectories(string path);
        Stream GetStream(string sourceFile, FileMode fileMode, FileAccess fileAccess, FileShare fileShare, int bufferSize);
        Stream GetStream(string sourceFile, FileMode fileMode, FileAccess fileAccess, FileShare fileShare, int bufferSize, FileOptions fileOptions);
    }
}
