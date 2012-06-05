using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Threading;

namespace DiskSnapshot
{
  public class Scanner
  {
    public static void Refresh(DirectoryStructure s, Action<string> progress, Action callback)
    {
      var dispatcher = Dispatcher.CurrentDispatcher;
      DateTime start = DateTime.Now;
      ThreadPool.QueueUserWorkItem(delegate
      {
        s.Refresh(d => { if (progress != null) dispatcher.BeginInvoke(new Action(() => progress(d))); });
        s.LastScanTimeInSeconds = (start - DateTime.Now).TotalSeconds;
        dispatcher.BeginInvoke(callback);
      });
    }
  }
}
