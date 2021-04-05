using BenchmarkDotNet.Attributes;
using JPEG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace JPEGBenchmarks
{
    [DisassemblyDiagnoser]
    public class TransfromBenchmark
    {
        private readonly IFFTTransform native = new FFT();
        private readonly IFFTTransform hardCoded = new HardCodeFFT();
        private readonly IFFTTransform superFast = new SuperFastFFT();

        private Complex[] numbers;

        [GlobalSetup]
        public void SetUp()
        {
            numbers = new Complex[8] { 1, 2, 3, 4, 5, 6, 7, 8 };
        }

        [Benchmark]
        public void NativeFFT()
        {
            native.Fft(numbers, 1);
        }

        [Benchmark]
        public void HardCodedFFT()
        {
            hardCoded.Fft(numbers, 1);
        }

        [Benchmark]
        public void SuperFastFFT()
        {
            var numberss = new Complex[8] { 1, 2, 3, 4, 5, 6, 7, 8 };
            superFast.Fft(numberss, 1);
        }
    }
}
