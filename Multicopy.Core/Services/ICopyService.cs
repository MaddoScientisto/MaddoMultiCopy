using IOExtensions;
using Multicopy.MAUI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multicopy.MAUI.Core.Services
{
    public interface ICopyService
    {
        DestinationPathInfo Build();
        Task DoCopyAsync(DestinationPathInfo dpi, string sourcePath, CancellationToken cancellationToken, Action Tick);
        Task GetDestinationFolder(DestinationPathInfo dpi);
        void OpenFolder(DestinationPathInfo dpi);
        void Progress(DestinationPathInfo dpi, TransferProgress obj);
    }
}
