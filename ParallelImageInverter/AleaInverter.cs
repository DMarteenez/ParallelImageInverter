using Alea;
using Alea.Parallel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelImageInverter
{
    class AleaInverter
    {
        public static ImageWithName InvertImage(ImageWithName img)
        {

            Gpu gpu = Gpu.Default;

            gpu.For(0, img.height, (y) =>
            {
                gpu.For(0, img.width, (x) =>
                {
                    img.image[x, y] = ImageProcessor.InvertPixel(img.image[x, y]);
                });
            });
            return img;
        }
    }
}
