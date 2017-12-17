namespace AddTest
{
    
    public delegate double PerformCalculation(double[] numbers);

    public struct Benchmark
    {
        public string Name;
        public PerformCalculation Function;

        public Benchmark(string n, PerformCalculation f)
        {
            Name = n;
            Function = f;
        }
    }
}