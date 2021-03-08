using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;

namespace ParallelImageInverter
{
    public class ImageProcessor
    {
        public static SimplePixel InvertPixel(SimplePixel pixel)
        {
            var inv = new SimplePixel();
            inv.R = (byte)(255 - pixel.R);
            inv.G = (byte)(255 - pixel.G);
            inv.B = (byte)(255 - pixel.B);
            inv.A = pixel.A;
            return inv;
        }
    }
}
