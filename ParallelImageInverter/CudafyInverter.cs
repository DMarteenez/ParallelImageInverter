using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelImageInverter
{
    class CudafyInverter
    {
        public static ImageWithName InvertImage(ImageWithName img)
        {


            Parallel.For(0, img.height, (y) =>
            {
                Parallel.For(0, img.width, (x) =>
                {
                    img.image[x, y] = ImageProcessor.InvertPixel(img.image[x, y]);
                });
            });
            return img;
        }
    }
}
