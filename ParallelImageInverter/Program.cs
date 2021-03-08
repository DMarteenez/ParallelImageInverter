using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILGPU;
using ILGPU.Runtime;
using ILGPU.Runtime.CPU;
using System.Threading;
using System.Drawing;
using Cudafy;
using Cudafy.Translator;
using Cudafy.Host;

namespace ParallelImageInverter
{
    //class Program
    //{
    //    /// <summary>
    //    /// Директория с исходными изображениями.
    //    /// </summary>
    //    static string imageSourcePath = @"C:\_Znatok\POLYTECH\4 курс\НИР\ParallelImageInverter\ImageSource\";
    //    /// <summary>
    //    /// Директория куда сохранять изображения.
    //    /// </summary>
    //    static string imageOutPath = @"C:\_Znatok\POLYTECH\4 курс\НИР\ParallelImageInverter\ImageOutput\";

    //    public static string[] imageProcessingData;

    //    public static void RunCPU(string[] imgNameList)
    //    {
    //        Console.WriteLine("Loading");
    //        ImageWithName[] SourceImageList = LoadImages(imageSourcePath, imgNameList);
    //        ImageWithName[] OutImageList = new ImageWithName[imgNameList.Length];

    //        //imageProcessingData = new string[SourceImageList.Length];

    //        Console.WriteLine("Processing");
    //        for (int i = 0; i < SourceImageList.Length; i++)
    //        {
    //            OutImageList[i] = CPUInverter.InvertImage(SourceImageList[i]);
    //        }
    //        Console.WriteLine("Saving");
    //        SaveImages(imageOutPath, OutImageList);
    //    }
    //    public static void RunILGPU(string[] imgNameList)
    //    {
    //        Console.WriteLine("Loading");
    //        ImageWithName[] SourceImageList = LoadImages(imageSourcePath, imgNameList);
    //        ImageWithName[] OutImageList = new ImageWithName[imgNameList.Length];

    //        //imageProcessingData = new string[SourceImageList.Length];

    //        Console.WriteLine("Processing");
    //        for (int i = 0; i < SourceImageList.Length; i++)
    //        {
    //            OutImageList[i] = ILGPUInverter.InvertImage(new ImageWithName());
    //        }
    //        Console.WriteLine("Saving");
    //        SaveImages(imageOutPath, OutImageList);
    //    }

    //    public static void RunAlea(string[] imgNameList)
    //    {
    //        Console.WriteLine("Loading");
    //        ImageWithName[] SourceImageList = LoadImages(imageSourcePath, imgNameList);
    //        ImageWithName[] OutImageList = new ImageWithName[imgNameList.Length];

    //        //imageProcessingData = new string[SourceImageList.Length];

    //        Console.WriteLine("Processing");
    //        for (int i = 0; i < SourceImageList.Length; i++)
    //        {
    //            OutImageList[i] = AleaInverter.InvertImage(SourceImageList[i]);
    //        }
    //        Console.WriteLine("Saving");
    //        SaveImages(imageOutPath, OutImageList);
    //        ILGPUInverter.Dispose();
    //    }

    //    public static ImageWithName[] LoadImages(string imageSourcePath, string[] imgNameList)
    //    {
    //        var SourceImageList = new ImageWithName[imgNameList.Length];
    //        Parallel.For(0, imgNameList.Length, (i) =>
    //        {
    //            Bitmap img = new Bitmap(imageSourcePath + imgNameList[i]);
    //            SourceImageList[i] = new ImageWithName(img, imgNameList[i]);
    //        });
    //        return SourceImageList;
    //    }

    //    public static void SaveImages(string imageOutPath, ImageWithName[] OutImageList)
    //    {
    //        Parallel.For(0, OutImageList.Length, (i) =>
    //        {
    //            OutImageList[i].GetBitmap().Save(imageOutPath + OutImageList[i].name);
    //        });
    //    }

    //    static string[] GetFileNames()
    //    {
    //        var imageNames = Directory.GetFiles(imageSourcePath);
    //        for (int i = 0; i < imageNames.Count(); i++)
    //        {
    //            imageNames[i] = Path.GetFileName(imageNames[i]);
    //        }
    //        return imageNames;
    //    }

    //    static void Main(string[] args)
    //    {
    //        var imageNames = GetFileNames();

    //        RunAlea(imageNames);

    //        //ILGPUInverter.DoShit();

    //        Console.WriteLine("Done");
    //        Console.ReadKey();
    //    }
    //}

    class Program
    {
        public const int N = 10;
        public static void Main()
        {
            Console.WriteLine("CUDAfy Example\nCollecting necessary resources...");

            CudafyModes.Target = eGPUType.Cuda; // To use OpenCL, change this enum
            CudafyModes.DeviceId = 0;
            CudafyTranslator.Language = CudafyModes.Target == eGPUType.OpenCL ? eLanguage.OpenCL : eLanguage.Cuda;

            //Check for available devices
            if (CudafyHost.GetDeviceCount(CudafyModes.Target) == 0)
                throw new System.ArgumentException("No suitable devices found.", "original");

            //Init device
            GPGPU gpu = CudafyHost.GetDevice(CudafyModes.Target, CudafyModes.DeviceId);
            Console.WriteLine("Running example using {0}", gpu.GetDeviceProperties(false).Name);

            //Load module for GPU
            CudafyModule km = CudafyTranslator.Cudafy();
            gpu.LoadModule(km);

            //Define local arrays
            int[] a = new int[N];
            int[] b = new int[N];
            int[] c = new int[N];

            // allocate the memory on the GPU
            int[] dev_c = gpu.Allocate<int>(c);

            // fill the arrays 'a' and 'b' on the CPU
            for (int i = 0; i < N; i++)
            {
                a[i] = i;
                b[i] = i * i;
            }

            // copy the arrays 'a' and 'b' to the GPU
            int[] dev_a = gpu.CopyToDevice(a);
            int[] dev_b = gpu.CopyToDevice(b);

            gpu.Launch(1, N).add(dev_a, dev_b, dev_c);

            // copy the array 'c' back from the GPU to the CPU
            gpu.CopyFromDevice(dev_c, c);

            // display the results
            for (int i = 0; i < N; i++)
                Console.WriteLine("{0} + {1} = {2}", a[i], b[i], c[i]);

            // free the memory allocated on the GPU
            gpu.FreeAll();

            Console.WriteLine("Done!");
            Console.ReadKey();
        }

        [Cudafy]
        public static void add(GThread thread, int[] a, int[] b, int[] c)
        {
            int tid = thread.threadIdx.x;
            if (tid < N)
                c[tid] = a[tid] + b[tid];
        }
    }
}
