using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multicopy.MAUI.Data
{
    public class DestinationPathInfo
    {
        public string DestinationPath { get; set; }
        public string TotalTimeFormatted { get; set; }
        public double FilesToCopy { get; set; }
        public double FilesCopied { get; set; }
        public string CurrentFileName { get; set; }
        public string Speed { get; set; }
        public string CurrentCopyTime { get; set; }
        public bool DoMove { get; set; }
        public bool FileSeparation { get; set; }
        public bool CreateTopFolder { get; set; }
        public FileConflictType ConflictType { get; set; }

        public int FilesOverwritten { get; set; }
        public int FilesRenamed { get; set; }
        public int FilesSkipped { get; set; }
        public int FilesDeleted { get; set; }
    }
}
