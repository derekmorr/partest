using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading;

namespace AddTest
{
  public class AddTest
  {
    public const int SITES_COUNT = 20000000; // 2000000;

    public static BenchmarkResult RunBenchmark(Benchmark benchmark, double[] numbers)
    {
      Stopwatch stopWatch = Stopwatch.StartNew();
      double result = benchmark.Function(numbers);
      stopWatch.Stop();
      int runtime = stopWatch.Elapsed.Milliseconds;

      return new BenchmarkResult(benchmark.Name, runtime, result);
    }

    public static IList<BenchmarkResult> RunAllBenchmarks(IList<Benchmark> benchmarks, double[] numbers)
    {      
      // shuffle the order of benchmarks
      Random rng = new Random();
      benchmarks = benchmarks.OrderBy( x => rng.Next()).ToList();

      return benchmarks.Select(benchmark => RunBenchmark(benchmark, numbers)).ToList();
    }
    
    public static void Main(string[] args)
    { 
      double[] numbers = MakeNumbers(SITES_COUNT);
      Console.WriteLine("generated {0} numbers", numbers.Count());
      Console.WriteLine("Vectors are h/w accelerated: {0}", Vector.IsHardwareAccelerated);
      Console.WriteLine("Vector<double>.Count: {0}", Vector<double>.Count);

      IList<Benchmark> benchmarks = new List<Benchmark>()
      {
        new Benchmark("Single-threaded for loop", AddImplementations.AddSingleThreadedFor),
        new Benchmark("Single-threaded LINQ",     AddImplementations.AddSingleThreadedLinq),
        new Benchmark("Single-threaded Vector",   AddImplementations.AddSingleThreadedVector),
        new Benchmark("Multi-threaded LINQ",      AddImplementations.AddMultiThreadedLinq),
        new Benchmark("Parallel.ForEach",         AddImplementations.AddMultiThreadForEach)
      };

      IList<BenchmarkResult> results = RunAllBenchmarks(benchmarks, numbers);
      
      // check that all implementations returned the same value
      double expectedTotal = results.First().Total; // arbitrary declare that the first result is correct.
      bool allEqual = results.All(result => result.Total == expectedTotal); 
      
      if (!allEqual)
      {
        Console.WriteLine("some implementations returned the wrong results!");
        var brokenImplementations = results.Where(result => result.Total != expectedTotal); 
        foreach (var broken in brokenImplementations)
        {
          Console.WriteLine("{0} is broken. returned {1} but expected {2}", broken.MethodName, broken.Total, expectedTotal);
        }
      }
      else
      {
        foreach (var result in results.OrderBy(result => result.RunTimeMs))
        {
          Console.WriteLine("result for: {0} \t {1} ms", result.MethodName, result.RunTimeMs);
        }
      }
    }

    private static double[] MakeNumbers(int count)
    {
      return Enumerable.Range(0, count).Select(i => Convert.ToDouble(i)).ToArray();
    }
  }
}