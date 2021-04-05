using BenchmarkDotNet.Attributes;
using JPEG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPEGBenchmarks
{
    [DisassemblyDiagnoser]
    public class HuffmanCodecBenchmark
    {
        private int[] frequences;

        [GlobalSetup]
        public void SetUp()
        {
            frequences = Enumerable.Range(0, 1000).ToArray();
        }

        [Benchmark]
        public void LegacyHuffmanBuildTree()
        {
            HuffmanCodec.SlowTreeBuild(frequences);
        }

        [Benchmark]
        public void RefactorHuffmanBuildTree()
        {
            HuffmanCodec.BuildHuffmanTree(frequences);
        }
    }
}
