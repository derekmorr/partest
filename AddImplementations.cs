using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace AddTest
{
    public class AddImplementations
    {
        public static double AddSingleThreadedFor(double[] numbers)
        {
            double total = 0;
            foreach (var i in numbers)
            {
                total += i;
            }
            return total;
        }

        public static double AddSingleThreadedLinq(double[] numbers)
        {
            return numbers.Sum();
        }

        public static double AddSingleThreadedVector(double[] numbers)
        {
            int vectSize = Vector<double>.Count;
            Vector<double> vectTotal = Vector<double>.Zero;

            // compute vector sum
            for (int i = 0; i < numbers.Count(); i += vectSize)
            {
                Vector<double> vectA = new Vector<double>(numbers, i);
                vectTotal = Vector.Add(vectTotal, vectA);
            }

            // sum the elements of totalVector
            double total = 0;
            for(int i = 0; i < vectSize; i++)
            {
                total += vectTotal[i];
            }

            return total;
        }

        public static double AddMultiThreadedLinq(double[] numbers)
        {
            return numbers.AsParallel().Sum();
        }

        public static double AddMultiThreadForEach(double[] numbers)
        {
            double total = 0.0;
      
            Parallel.ForEach<double, double>(
                numbers,                  // source collection,
                () => 0.0,                // initializer for the thread-local variable,
                (n, loop, subTotal) =>    // invoked on each iteration
                {
                    subTotal += n;
                    return subTotal;
                },
                (threadLocalTotal) =>     // compute final value
                {
                    double initialValue, computedValue;
                    do
                    {
                        initialValue = total; // save current running total, since other threads will modify total
                        computedValue = initialValue + threadLocalTotal;
                    }
                    while (initialValue != Interlocked.CompareExchange(ref total, computedValue, initialValue));
                } 
            );

            return total;
        }
    }
}