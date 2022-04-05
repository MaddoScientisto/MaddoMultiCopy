using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multicopy.MAUI.Services
{
    public interface IFolderPicker
    {
        Task<string> PickFolder();
    }
}
