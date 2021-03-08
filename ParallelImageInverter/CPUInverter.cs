using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParallelImageInverter
{
    public class CPUInverter
    {
        public static ImageWithName InvertImage(ImageWithName img)
        {
            Parallel.For(0, img.height, (y) =>
            {
                Parallel.For(0, img.width, (x) =>
                {
                    img.image[x,y] = ImageProcessor.InvertPixel(img.image[x,y]);
                });
            });
            return img;
        }   
    }
}
