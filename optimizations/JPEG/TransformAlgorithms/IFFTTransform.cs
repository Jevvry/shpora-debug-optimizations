using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace JPEG
{
    public interface IFFTTransform
    {
        Complex[] Fft(Complex[] input, int multi);
        Complex[] InverseFFT(Complex[] input, int multi);
    }
}
