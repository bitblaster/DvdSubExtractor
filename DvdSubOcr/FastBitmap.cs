﻿//Downloaded from Visual C# Kicks - http://www.vcskicks.com/
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace DvdSubOcr
{
    unsafe public class FastBitmap
    {
        private struct PixelData
        {
            public byte Blue;
            public byte Green;
            public byte Red;
            public byte Alpha;

            public override string ToString()
            {
                return "(" + Alpha + ", " + Red + ", " + Green + ", " + Blue + ")";
            }
        }

        public int Width { get; set; }
        public int Height { get; set; }

        private readonly Bitmap _workingBitmap;
        private int _width;
        private BitmapData _bitmapData;
        private Byte* _pBase = null;

        public FastBitmap(Bitmap inputBitmap)
        {
            _workingBitmap = inputBitmap;

            Width = inputBitmap.Width;
            Height = inputBitmap.Height;
        }

        public void LockImage()
        {
            var bounds = new Rectangle(Point.Empty, _workingBitmap.Size);

            _width = bounds.Width * sizeof(PixelData);
            if (_width % 4 != 0) _width = 4 * (_width / 4 + 1);

            //Lock Image
            _bitmapData = _workingBitmap.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            _pBase = (Byte*)_bitmapData.Scan0.ToPointer();
        }

        private PixelData* _pixelData = null;

        public Color GetPixel(int x, int y)
        {
            _pixelData = (PixelData*)(_pBase + y * _width + x * sizeof(PixelData));
            return Color.FromArgb(_pixelData->Alpha, _pixelData->Red, _pixelData->Green, _pixelData->Blue);
        }

        public Color GetPixelNext()
        {
            _pixelData++;
            return Color.FromArgb(_pixelData->Alpha, _pixelData->Red, _pixelData->Green, _pixelData->Blue);
        }

        public void SetPixel(int x, int y, Color color)
        {
            var data = (PixelData*)(_pBase + y * _width + x * sizeof(PixelData));
            data->Alpha = color.A;
            data->Red = color.R;
            data->Green = color.G;
            data->Blue = color.B;
        }

        public Bitmap GetBitmap()
        {
            return _workingBitmap;
        }

        public void UnlockImage()
        {
            _workingBitmap.UnlockBits(_bitmapData);
            _bitmapData = null;
            _pBase = null;
        }
    }
}
