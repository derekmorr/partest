using System.Collections.Generic;

namespace TestHarness
{
    
    public delegate double PerformCalculation(double[] numbers);

    public struct Benchmark
    {
        public readonly string Name;
        public readonly PerformCalculation Function;
        public readonly List<BenchmarkResult> Results;

        public Benchmark(string n, PerformCalculation f)
        {
            Name = n;
            Function = f;
            Results = new List<BenchmarkResult>();
        }
    }
}