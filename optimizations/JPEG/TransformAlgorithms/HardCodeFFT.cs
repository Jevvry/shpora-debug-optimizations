using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace JPEG
{
    public class HardCodeFFT : IFFTTransform
    {
        private static Complex Exp(int k, int N, int multi)
        {
            if (k % N == 0) return 1;
            double arg = -2 * Math.PI * k * multi / N;
            return new Complex(Math.Cos(arg), Math.Sin(arg));
        }

        public Complex[] Fft(Complex[] input, int multi)
        {
            var a = input[0] + input[4];
            var b = input[0] - input[4];
            var c = input[2] + input[6];
            var e = input[2] - input[6];

            var k = input[1] + input[5];
            var d = input[1] - input[5];
            var f = input[3] + input[7];
            var m = input[3] - input[7];

            var x_e = new Complex[4]
            {
                a+ Exp(0,4,multi)*c,
                b+ Exp(1,4,multi)*e,
                a-Exp(0,4,multi)*c,
                b-Exp(1,4,multi)*e
                };

            var x_o = new Complex[4]
            {
                k+Exp(0,4,multi)*f,
                d+Exp(1,4,multi)*m,
                k-Exp(0,4,multi)*f,
                d-Exp(1,4,multi)*m
            };

            var res = new Complex[8]
            {
                x_e[0]+Exp(0,8,multi)*x_o[0],
                 x_e[1]+Exp(1,8,multi)*x_o[1],
                  x_e[2]+Exp(2,8,multi)*x_o[2],
                   x_e[3]+Exp(3,8,multi)*x_o[3],

                   x_e[0]-Exp(0,8,multi)*x_o[0],
                 x_e[1]-Exp(1,8,multi)*x_o[1],
                  x_e[2]-Exp(2,8,multi)*x_o[2],
                   x_e[3]-Exp(3,8,multi)*x_o[3]
            };
            return res;
        }

        public Complex[] InverseFFT(Complex[] x, int multi)
        {
            var array = new Complex[8]
            {
                Complex.Conjugate(x[0]),Complex.Conjugate(x[1]),Complex.Conjugate(x[2]),
                Complex.Conjugate(x[3]),Complex.Conjugate(x[4]),Complex.Conjugate(x[5]),
                Complex.Conjugate(x[6]),Complex.Conjugate(x[7]),
            };
            return Fft(array, multi);
        }
    }
}
