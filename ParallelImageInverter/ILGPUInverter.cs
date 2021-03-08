using ILGPU;
using ILGPU.Runtime;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelImageInverter
{
    class ILGPUInverter
    {
        private static readonly Accelerator gpu = Accelerator.Create(
            new Context(),
            Accelerator.Accelerators.First(a => a.AcceleratorType == AcceleratorType.Cuda));
        private static readonly Action<Index1, ArrayView<SimplePixel>> kernel =
            gpu.LoadAutoGroupedStreamKernel<Index1, ArrayView<SimplePixel>>(ApplyKernel);

        private static void ApplyKernel(
           Index1 index, /* The global thread index (1D in this case) */
           ArrayView<SimplePixel> pixelArray /* A view to a chunk of memory (1D in this case)*/)
        {
            pixelArray[index] = ImageProcessor.InvertPixel(pixelArray[index]);
        }

        public static ImageWithName InvertImage(ImageWithName img)
        {
            Console.WriteLine("ooo");
            using (MemoryBuffer<SimplePixel> buffer = gpu.Allocate<SimplePixel>(img.image.Length))
            {
                buffer.CopyFrom(img.Get1DArr(), 0, Index1.Zero, img.image.Length);

                kernel(img.image.Length, buffer.View);

                // Wait for the kernel to finish...
                gpu.Synchronize();

                return new ImageWithName(buffer.GetAsArray(), img.width, img.height, img.name); ; 
            }
        }

        public static void Dispose()
        {
            gpu?.Dispose();
        }

        public static void DoShit()
        {
            Console.WriteLine("ooo");
        }
    }
}
