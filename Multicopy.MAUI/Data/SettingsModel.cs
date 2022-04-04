using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multicopy.MAUI.Data
{
    public class SettingsModel
    {
        public string SourcePath { get; set; }
        public List<string> DestinationPaths { get; set; }
        public bool DoMove { get; set; }
        public bool ParallelExecution { get; set; }

        public bool TurnOff { get; set; }
        public bool Exit { get; set; }
        public bool CreateTopFolder { get; set; }
        public bool FileSeparation { get; set; }

        public FileConflictType FileConflict { get; set; }
    }
}
