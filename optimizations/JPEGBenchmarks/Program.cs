using BenchmarkDotNet.Running;
using System;

namespace JPEGBenchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<TransfromBenchmark>();
            //BenchmarkRunner.Run<HuffmanCodecBenchmark>();
        }
    }
}
