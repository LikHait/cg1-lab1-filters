using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cg_lab1
{
    class MatrixFilter : Filters
    {
        protected float[,] kernel = null;
        protected MatrixFilter() { }

        public MatrixFilter(float[,] kernel)
        {
            this.kernel = kernel;
        }

        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;
            float resultR = 0;
            float resultG = 0;
            float resultB = 0;
            for (int l = -radiusY; l <= radiusY; l++)
            {
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = Clamp(x + k, 0, sourseImage.Width - 1);
                    int idY = Clamp(y + l, 0, sourseImage.Height - 1);
                    Color neighbotColor = sourseImage.GetPixel(idX, idY);
                    resultR += neighbotColor.R * kernel[k + radiusX, l + radiusY];
                    resultG += neighbotColor.G * kernel[k + radiusX, l + radiusY];
                    resultB += neighbotColor.B * kernel[k + radiusX, l + radiusY];
                }
            }
            return Color.FromArgb(Clamp((int)resultR, 0, 255), Clamp((int)resultG, 0, 255), Clamp((int)resultB, 0, 255));
        }
    }

    class BlurFilter : MatrixFilter
    {
        public BlurFilter()
        {
            int sizeX = 3;
            int sizeY = 3;
            kernel = new float[sizeX, sizeY];
            for (int i = 0; i < sizeX; i++)
                for (int j = 0; j < sizeY; j++)
                    kernel[i, j] = 1.0f / (float)(sizeX * sizeY);
        }
    }

    class GaussianFilter : MatrixFilter
    {
        public void createGaussianKernel(int radius, float sigma)
        {
            int size = 2 * radius + 1;
            kernel = new float[size, size];
            float norm = 0;
            for (int i = -radius; i <= radius; i++)
                for (int j = -radius; j <= radius; j++)
                {
                    kernel[i + radius, j + radius] = (float)(Math.Exp(-(i * i + j * j) / (sigma * sigma)));
                    norm += kernel[i + radius, j + radius];
                }
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    kernel[i, j] /= norm;
        }
        public GaussianFilter()
        {
            createGaussianKernel(3, 2);
        }
    }

    class SobelFilter : MatrixFilter
    {
        float[,] kernelX = new float[,] { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };
        float[,] kernelY = new float[,] { { -1, 0, 1 }, { -2, 0, 2 }, { 1, 0, -1 } };

        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            int radiusX = kernelX.GetLength(0) / 2;
            int radiusY = kernelX.GetLength(1) / 2;
            float resultRx = 0;
            float resultGx = 0;
            float resultBx = 0;
            float resultRy = 0;
            float resultGy = 0;
            float resultBy = 0;
            for (int l = -radiusY; l <= radiusY; l++)
            {
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = Clamp(x + k, 0, sourseImage.Width - 1);
                    int idY = Clamp(y + l, 0, sourseImage.Height - 1);
                    Color neighbotColor = sourseImage.GetPixel(idX, idY);
                    resultRx += neighbotColor.R * kernelX[k + radiusX, l + radiusY];
                    resultGx += neighbotColor.G * kernelX[k + radiusX, l + radiusY];
                    resultBx += neighbotColor.B * kernelX[k + radiusX, l + radiusY];
                    resultRy += neighbotColor.R * kernelY[k + radiusX, l + radiusY];
                    resultGy += neighbotColor.G * kernelY[k + radiusX, l + radiusY];
                    resultBy += neighbotColor.B * kernelY[k + radiusX, l + radiusY];
                }
            }
            return Color.FromArgb(Clamp((int)Math.Sqrt(resultRx * resultRx + resultRy * resultRy), 0, 255), Clamp((int)Math.Sqrt(resultRx * resultRx + resultRy * resultRy), 0, 255), Clamp((int)Math.Sqrt(resultBx * resultBx + resultBy * resultBy), 0, 255));
        }
    }
}