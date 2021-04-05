using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace JPEG
{
    public static class FFT2D
    {
        private static IFFTTransform FFTTransform;
        private static Complex[,] FFTColumn(double[,] input)
        {
            var rowLenght = input.GetLength(0);
            var columnLenght = input.GetLength(1);
            var matrix = new Complex[rowLenght, columnLenght];
            for (int column = 0; column < columnLenght; column++)
            {
                var line = new Complex[8] { input[0, column], input[1, column], input[2, column], input[3, column],
                    input[4, column], input[5, column],input[6, column],input[7, column]};
                var comlexLine = FFTTransform.Fft(line, column);
                for (int row = 0; row < rowLenght; row++)
                {
                    matrix[row, column] = comlexLine[row];
                }
            }
            return matrix;
        }

        private static Complex[,] FFTRow(Complex[,] input)
        {
            var rowLenght = input.GetLength(0);
            var columnLenght = input.GetLength(1);
            var matrix = new Complex[rowLenght, columnLenght];
            for (int row = 0; row < rowLenght; row++)
            {
                var line = new[] { input[row, 0], input[row, 1], input[row, 2], input[row, 3], input[row, 4],
                input[row, 5],input[row, 6],input[row, 7]};
                var comlexLine = FFTTransform.Fft(line, row);
                for (int column = 0; column < columnLenght; column++)
                {
                    matrix[row, column] = comlexLine[column];
                }
            }
            return matrix;
        }

        public static double[,] Fft2D(double[,] input, IFFTTransform transform)
        {
            FFTTransform = transform;
            var a = FFTColumn(input);
            var matrix = FFTRow(a);
            var rowLen = input.GetLength(0);
            var columnLen = input.GetLength(1);
            var res = new double[rowLen, columnLen];
            for (int row = 0; row < rowLen; row++)
                for (int column = 0; column < columnLen; column++)
                    res[row, column] = matrix[row, column].Real;
            return res;
        }

        public static double[,] IFft2D(double[,] input, double[,] res, IFFTTransform transform)
        {
            FFTTransform = transform;
            var a = IFFTRow(input);
            var matrix = IFFTColumn(a);
            var rowLen = input.GetLength(0);
            var columnLen = input.GetLength(1);
            for (int row = 0; row < rowLen; row++)
                for (int column = 0; column < columnLen; column++)
                    res[row, column] = matrix[row, column].Real / (rowLen * rowLen);
            return res;
        }

        private static Complex[,] IFFTColumn(Complex[,] input)
        {
            var rowLenght = input.GetLength(0);
            var columnLenght = input.GetLength(1);
            var matrix = new Complex[rowLenght, columnLenght];
            for (int column = 0; column < columnLenght; column++)
            {
                var line = new[] { input[0, column], input[1, column], input[2, column], input[3, column],
                    input[4, column], input[5, column],input[6, column],input[7, column]};
                var comlexLine = FFTTransform.InverseFFT(line, column);
                for (int row = 0; row < rowLenght; row++)
                {
                    matrix[row, column] = comlexLine[row];
                }
            }
            return matrix;
        }

        private static Complex[,] IFFTRow(double[,] input)
        {
            var rowLenght = input.GetLength(0);
            var columnLenght = input.GetLength(1);
            var matrix = new Complex[rowLenght, columnLenght];
            for (int row = 0; row < rowLenght; row++)
            {
                var line = new Complex[8] { input[row, 0], input[row, 1], input[row, 2], input[row, 3],
                    input[row, 4], input[row, 5],input[row, 6],input[row, 7]};
                var comlexLine = FFTTransform.InverseFFT(line, row);
                for (int column = 0; column < columnLenght; column++)
                {
                    matrix[row, column] = comlexLine[column];
                }
            }
            return matrix;
        }
    }
}
