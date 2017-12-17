using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace TestHarness
{
  public class TestRunner
  {
    public const int SITES_COUNT = 20000000; // 2000000;
    public const int ITER_COUNT = 20;

    public static Benchmark RunBenchmark(Benchmark benchmark, double[] numbers, int iterations)
    {
      Console.WriteLine("testing {0}", benchmark.Name);
      for (int i = 0; i < iterations; i++)
      {
        Stopwatch stopWatch = Stopwatch.StartNew();
        double result = benchmark.Function(numbers);
        stopWatch.Stop();
        int runtime = stopWatch.Elapsed.Milliseconds;

        benchmark.Results.Add(new BenchmarkResult(runtime, result));
      }

      return benchmark;
    }

    public static IList<Benchmark> RunAllBenchmarks(IList<Benchmark> benchmarks, double[] numbers)
    {      
      // shuffle the order of benchmarks
      Random rng = new Random();
      benchmarks = benchmarks.OrderBy( x => rng.Next()).ToList();

      return benchmarks.Select(benchmark => RunBenchmark(benchmark, numbers, ITER_COUNT)).ToList();
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
        new Benchmark("Multi-threaded ForEach",   AddImplementations.AddMultiThreadForEach)
      };

      IList<Benchmark> results = RunAllBenchmarks(benchmarks, numbers);
      
      // check that all implementations returned the same value
      // arbitrary declare that the first result is correct.
      double expectedTotal = results.First().Results.First().Total;

      // print bad results, if any
      List<Benchmark> badResults = FindBadResults(benchmarks, expectedTotal).ToList();
      if (badResults.Any())
      {
        Console.WriteLine("some implementations returned the wrong results!");
        foreach (var badBenchmark in badResults)
        {
          Console.WriteLine("{0} is broken.", badBenchmark.Name);
        }
      }

      // process good results
      var badNames = badResults.Select(benchmark => benchmark.Name);
      IList<Benchmark> goodResults = benchmarks.Where(result => !badNames.Contains(result.Name)).ToList();
      var orderedResults = goodResults.OrderBy(benchmark => Avg(benchmark));
      foreach (var result in orderedResults)
      {
        var avg = Avg(result);
        var min = Min(result);
        var max = Max(result);
        Console.WriteLine("result for: {0} \t avg: {1} ms \t min: {2} ms \t max: {3} ms", 
          result.Name, avg, min, max);
      }
      
    }

    private static IList<Benchmark> FindBadResults(IList<Benchmark> benchmarks, double expectedResult)
    {
      List<Benchmark> badResults = new List<Benchmark>();
      foreach (var benchmark in benchmarks)
      {
        if (benchmark.Results.Exists(result => result.Total != expectedResult))
        {
          badResults.Add(benchmark);
        }
      }

      return badResults;
    }

    private static double Avg(Benchmark benchmark)
    {
      double total = benchmark.Results.Select(r => r.RunTimeMs).Sum();
      double count = benchmark.Results.Count;

      return total / count;
    }

    private static double Min(Benchmark benchmark)
    {
      return benchmark.Results.Select(r => r.RunTimeMs).Min();
    }

    private static double Max(Benchmark benchmark)
    {
      return benchmark.Results.Select(r => r.RunTimeMs).Max();
    }
    
    private static double[] MakeNumbers(int count)
    {
      return Enumerable.Range(0, count).Select(i => Convert.ToDouble(i)).ToArray();
    }
  }
}