using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace JPEG
{
    public class FFT : IFFTTransform
    {
        private static Complex Exp(int k, int N, int multi)
        {
            if (k % N == 0) return 1;
            double arg = -2 * Math.PI * k * multi / N;
            return new Complex(Math.Cos(arg), Math.Sin(arg));
        }

        public Complex[] Fft(Complex[] input, int multi)
        {
            Complex[] X;
            int N = input.Length;
            if (N == 2)
            {
                X = new Complex[2];
                X[0] = input[0] + input[1];
                X[1] = input[0] - input[1];
            }
            else
            {
                Complex[] input_even = new Complex[N / 2];
                Complex[] input_odd = new Complex[N / 2];
                for (int i = 0; i < N / 2; i++)
                {
                    input_even[i] = input[2 * i];
                    input_odd[i] = input[2 * i + 1];
                }
                Complex[] X_even = Fft(input_even, multi);
                Complex[] X_odd = Fft(input_odd, multi);
                X = new Complex[N];
                for (int i = 0; i < N / 2; i++)
                {
                    X[i] = X_even[i] + Exp(i, N, multi) * X_odd[i];
                    X[i + N / 2] = X_even[i] - Exp(i, N, multi) * X_odd[i];
                }
            }
            return X;
        }

        public Complex[] InverseFFT(Complex[] x, int multi)
        {
            return Fft(x.Select(Complex.Conjugate).ToArray(), multi);
        }
    }
}
