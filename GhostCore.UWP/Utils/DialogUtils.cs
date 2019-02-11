using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace GhostCore.UWP.Utils
{
    public static class DialogUtils
    {
        public static async Task<IUICommand> ShowMessageBox(string content, string title)
        {
            var msgDlg = new MessageDialog(content, title);
            return await msgDlg.ShowAsync();
        }
    }
}
