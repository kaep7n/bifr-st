using BenchmarkDotNet.Running;
using System;

namespace Bifröst.Benchmarking
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<RouterBenchmark>();
        }
    }
}
