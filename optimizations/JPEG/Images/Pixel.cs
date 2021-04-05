﻿using System;
using System.Linq;

namespace JPEG.Images
{
    public struct Pixel
    {
        public Pixel(double firstComponent, double secondComponent,
            double thirdComponent, PixelFormat pixelFormat) : this()
        {
            if (PixelFormat.RGB != pixelFormat && PixelFormat.YCbCr != pixelFormat)
                throw new FormatException("Unknown pixel format: " + pixelFormat);
            if (pixelFormat == PixelFormat.RGB)
            {
                r = firstComponent;
                g = secondComponent;
                b = thirdComponent;
                y = 16.0 + (65.738 * R + 129.057 * G + 24.064 * B) / 256.0;
                cb = 128.0 + (-37.945 * R - 74.494 * G + 112.439 * B) / 256.0;
                cr = 128.0 + (112.439 * R - 94.154 * G - 18.285 * B) / 256.0;
            }
            if (pixelFormat == PixelFormat.YCbCr)
            {
                y = firstComponent;
                cb = secondComponent;
                cr = thirdComponent;
                r = (298.082 * y + 408.583 * Cr) / 256.0 - 222.921;
                g = (298.082 * Y - 100.291 * Cb - 208.120 * Cr) / 256.0 + 135.576;
                b = (298.082 * Y + 516.412 * Cb) / 256.0 - 276.836;
            }
        }

        private readonly double r;
        private readonly double g;
        private readonly double b;

        private readonly double y;
        private readonly double cb;
        private readonly double cr;

        public double R => r;
        public double G => g;
        public double B => b;

        public double Y => y;
        public double Cb => cb;
        public double Cr => cr;
    }
}