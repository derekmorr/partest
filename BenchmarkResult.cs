namespace TestHarness
{
    public struct BenchmarkResult
    {
        public long RunTimeMs;
        public double Total;

        public BenchmarkResult(long time, double total)
        {
            RunTimeMs = time;
            Total = total;
        }
    }
}