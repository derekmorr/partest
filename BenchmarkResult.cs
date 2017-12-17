namespace AddTest
{
    public struct BenchmarkResult
    {
        public string MethodName;
        public long RunTimeMs;
        public double Total;

        public BenchmarkResult(string name, long time, double total)
        {
            MethodName = name;
            RunTimeMs = time;
            Total = total;
        }
    }
}