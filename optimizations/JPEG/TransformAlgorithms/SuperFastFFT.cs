using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace JPEG
{
    public class SuperFastFFT : IFFTTransform
    {
        private static int ReverseBits(int val, int width)
        {
            int result = 0;
            for (int i = 0; i < width; i++, val >>= 1)
                result = (result << 1) | (val & 1);
            return result;
        }

        public static void TransformRadix2(Complex[] vec, bool inverse = false)
        {
            int n = vec.Length;
            int levels = 0;
            for (int temp = n; temp > 1; temp >>= 1)
                levels++;

            Complex[] expTable = new Complex[n / 2];
            double coef = 2 * Math.PI / n * (inverse ? 1 : -1);
            for (int i = 0; i < n / 2; i++)
                expTable[i] = Complex.FromPolarCoordinates(1, i * coef);

            for (int i = 0; i < n; i++)
            {
                int j = ReverseBits(i, levels);
                if (j > i)
                {
                    Complex temp = vec[i];
                    vec[i] = vec[j];
                    vec[j] = temp;
                }
            }

            for (int size = 2; size <= n; size *= 2)
            {
                int halfsize = size / 2;
                int tablestep = n / size;
                for (int i = 0; i < n; i += size)
                {
                    for (int j = i, k = 0; j < i + halfsize; j++, k += tablestep)
                    {
                        Complex temp = vec[j + halfsize] * expTable[k];
                        vec[j + halfsize] = vec[j] - temp;
                        vec[j] += temp;
                    }
                }

                if (size == n)
                    break;
            }
        }

        public Complex[] Fft(Complex[] input, int multi)
        {
            TransformRadix2(input);
            return input;
        }

        public Complex[] InverseFFT(Complex[] input, int multi)
        {
            TransformRadix2(input, true);
            return input;
        }
    }
}
