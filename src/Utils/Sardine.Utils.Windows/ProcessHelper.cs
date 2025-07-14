using System.Diagnostics;

namespace Sardine.Utils.Windows
{
    public static class ProcessHelper
    {
        public static void KillProcess(string name)
        {
            if (name.EndsWith(".exe"))
                name = name[..^4];
            foreach (var process in Process.GetProcessesByName(name))
            {
                process.Kill();
            }
        }
    }
}
