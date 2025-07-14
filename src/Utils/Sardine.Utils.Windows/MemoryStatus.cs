namespace Sardine.Utils.Windows
{
    public class MemoryStatus
    {
        public int Free { get; }
        public int Total { get; }
        public double FreeGB => (double)Free / (1024 * 1024);
        public double TotalGB => (double)Free / (1024 * 1024);
        public double UsedPercentage => (double)(Total - Free) / Total;

        internal MemoryStatus(int free = 0, int total = 0)
        {
            Free = free;
            Total = total;
        }
    }
}
