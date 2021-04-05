using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace JPEG.Images
{
    unsafe class Matrix : IDisposable
    {
        public readonly int Height;
        public readonly int Width;
        private readonly Bitmap bitmap;
        private readonly BitmapData bitmapData;
        private readonly int bytesPerPixel;
        private readonly int widthInBytes;
        private readonly byte* ptrFirstPixel;
        private readonly int stride;

        public Matrix(int height, int width, Bitmap bitmap)
        {
            Height = height;
            Width = width;
            this.bitmap = bitmap;
            bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                        ImageLockMode.ReadWrite, bitmap.PixelFormat);

            bytesPerPixel = Bitmap.GetPixelFormatSize(bitmap.PixelFormat) / 8;
            widthInBytes = Width * bytesPerPixel;
            ptrFirstPixel = (byte*)bitmapData.Scan0;
            stride = bitmapData.Stride;
        }

        public Pixel this[int y, int x]
        {
            get
            {
                Pixel pixel;
                byte* currentLine = ptrFirstPixel + (y * stride);
                var byteOffset = x * bytesPerPixel;
                pixel = new Pixel(
                        currentLine[byteOffset + 2],
                        currentLine[byteOffset + 1],
                        currentLine[byteOffset], PixelFormat.RGB);
                return pixel;
            }

            set
            {
                byte* currentLine = ptrFirstPixel + (y * stride);
                var byteOffset = x * bytesPerPixel;
                var pixel = value;
                currentLine[byteOffset + 2] = (byte)ToByte(pixel.R);
                currentLine[byteOffset + 1] = (byte)ToByte(pixel.G);
                currentLine[byteOffset] = (byte)ToByte(pixel.B);
            }
        }


        public static explicit operator Matrix(Bitmap bmp)
        {
            var height = bmp.Height - bmp.Height % 8;
            var width = bmp.Width - bmp.Width % 8;
            var matrix = new Matrix(height, width, bmp);
            return matrix;
        }

        public static explicit operator Bitmap(Matrix matrix)
        {
            return matrix.bitmap;
        }

        public static int ToByte(double d)
        {
            var val = (int)d;
            if (val > byte.MaxValue)
                return byte.MaxValue;
            if (val < byte.MinValue)
                return byte.MinValue;
            return val;
        }

        public void Dispose()
        {
            bitmap.UnlockBits(bitmapData);
            bitmap.Dispose();
        }
    }
}