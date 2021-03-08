using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelImageInverter
{
    class BitmapPixelConverter
    {
        public static SimplePixel[,] BitmapToPixels(Bitmap img)
        {
            SimplePixel[,] pixels = new SimplePixel[img.Width, img.Height];
            for (int y = 0; (y <= (img.Height - 1)); y++)
            {
                for (int x = 0; (x <= (img.Width - 1)); x++)
                {
                    Color color = img.GetPixel(x, y);
                    pixels[x, y].R = color.R;
                    pixels[x, y].G = color.G;
                    pixels[x, y].B = color.B;
                    pixels[x, y].A = color.A;
                }
            }
            return pixels;
        }

        public static Bitmap PixelsToBitmap(SimplePixel[,] pixels, int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);
            for (int y = 0; (y <= (height - 1)); y++)
            {
                for (int x = 0; (x <= (width - 1)); x++)
                {
                    bmp.SetPixel(x, y, Color.FromArgb(pixels[x, y].A, pixels[x, y].R, pixels[x, y].G, pixels[x, y].B));
                }
            }
            return bmp;
        }
    }

    public struct ImageWithName
    {
        public SimplePixel[,] image;
        public int height;
        public int width;
        public string name;

        public ImageWithName(ImageWithName original)
        {
            image = original.image;
            name = original.name;
            height = original.height;
            width = original.width;
        }

        public ImageWithName(Bitmap img, string name)
        {
            image = BitmapPixelConverter.BitmapToPixels(img);
            this.name = name;
            height = img.Height;
            width = img.Width;
        }

        public ImageWithName(SimplePixel[,] img, string name)
        {
            image = img;
            this.name = name;
            height = img.GetLength(2);
            width = img.GetLength(1);
        }

        public ImageWithName(SimplePixel[] arr, int widht, int height, string name)
        {
            image = new SimplePixel[widht, height];

            int x = 0;
            int y = 0;
            for (int k = 0; k < arr.Length; k++)
            {
                if (x >= widht)
                {
                    x = 0;
                    y++;
                }
                else
                {
                    image[x, y] = arr[k];
                    x++;
                }

            }

            this.name = name;
            this.height = widht;
            this.width = height;
        }

        public Bitmap GetBitmap()
        {
            return BitmapPixelConverter.PixelsToBitmap(image, width, height);
        }

        public SimplePixel[] Get1DArr()
        {
            SimplePixel[] res = new SimplePixel[image.Length];
            int k = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    res[k++] = image[x, y];
                }
            }
            return res;
        }
    }
}
