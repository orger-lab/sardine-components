using System.Diagnostics;

namespace Sardine.Utils.Windows
{
    public static class MemoryStatusFetcher
    {
        public static MemoryStatus GetMemoryStatus()
        {
            ProcessStartInfo info = new()
            {
                FileName = "wmic",
                Arguments = "OS get FreePhysicalMemory,TotalVisibleMemorySize /Value",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            };

            string output = "";
            using (Process? process = Process.Start(info))
            {
                if (process is null)
                    return new MemoryStatus();

                output = process.StandardOutput.ReadToEnd();
            }

            try
            {
                string[] lines = output.Trim().Split("\n");
                int freeMemoryParts = Convert.ToInt32(lines[0].Split("=", StringSplitOptions.RemoveEmptyEntries)[1]);
                int totalMemoryParts = Convert.ToInt32(lines[1].Split("=", StringSplitOptions.RemoveEmptyEntries)[1]);
                return new MemoryStatus(freeMemoryParts, totalMemoryParts);
            }
            catch
            {
                return new MemoryStatus();
            }
        }
    }
}
