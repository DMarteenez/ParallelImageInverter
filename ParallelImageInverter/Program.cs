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
//using Cudafy;
//using Cudafy.Translator;
//using Cudafy.Host;

namespace ParallelImageInverter
{
    class Program
    {
        /// <summary>
        /// Директория с исходными изображениями.
        /// </summary>
        static string imageSourcePath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + @"\ImageSource\";
        /// <summary>
        /// Директория куда сохранять изображения.
        /// </summary>
        static string imageOutPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + @"\ImageOutput\";

        public static string[] imageProcessingData;

        public static void RunCPU(string[] imgNameList)
        {
            Console.WriteLine("Loading");
            ImageWithName[] SourceImageList = LoadImages(imageSourcePath, imgNameList);
            ImageWithName[] OutImageList = new ImageWithName[imgNameList.Length];

            //imageProcessingData = new string[SourceImageList.Length];

            Console.WriteLine("Processing");
            for (int i = 0; i < SourceImageList.Length; i++)
            {
                OutImageList[i] = CPUInverter.InvertImage(SourceImageList[i]);
            }
            Console.WriteLine("Saving");
            SaveImages(imageOutPath, OutImageList);
        }
        
        public static void RunILGPU(string[] imgNameList)
        {
            Console.WriteLine("Loading");
            ImageWithName[] SourceImageList = LoadImages(imageSourcePath, imgNameList);
            ImageWithName[] OutImageList = new ImageWithName[imgNameList.Length];

            //imageProcessingData = new string[SourceImageList.Length];

            Console.WriteLine("Processing");
            for (int i = 0; i < SourceImageList.Length; i++)
            {
                OutImageList[i] = ILGPUInverter.InvertImage(new ImageWithName());
            }
            Console.WriteLine("Saving");
            SaveImages(imageOutPath, OutImageList);
        }
        
        public static void RunAlea(string[] imgNameList)
        {
            Console.WriteLine("Loading");
            ImageWithName[] SourceImageList = LoadImages(imageSourcePath, imgNameList);
            ImageWithName[] OutImageList = new ImageWithName[imgNameList.Length];

            //imageProcessingData = new string[SourceImageList.Length];

            Console.WriteLine("Processing");
            for (int i = 0; i < SourceImageList.Length; i++)
            {
                OutImageList[i] = AleaInverter.InvertImage(SourceImageList[i]);
            }
            Console.WriteLine("Saving");
            SaveImages(imageOutPath, OutImageList);
            ILGPUInverter.Dispose();
        }

        public static ImageWithName[] LoadImages(string imageSourcePath, string[] imgNameList)
        {
            var SourceImageList = new ImageWithName[imgNameList.Length];
            Parallel.For(0, imgNameList.Length, (i) =>
            {
                Bitmap img = new Bitmap(imageSourcePath + imgNameList[i]);
                SourceImageList[i] = new ImageWithName(img, imgNameList[i]);
            });
            return SourceImageList;
        }

        public static void SaveImages(string imageOutPath, ImageWithName[] OutImageList)
        {
            Parallel.For(0, OutImageList.Length, (i) =>
            {
                OutImageList[i].GetBitmap().Save(imageOutPath + OutImageList[i].name);
            });
        }

        static string[] GetFileNames()
        {
            var imageNames = Directory.GetFiles(imageSourcePath);
            for (int i = 0; i < imageNames.Count(); i++)
            {
                imageNames[i] = Path.GetFileName(imageNames[i]);
            }
            return imageNames;
        }

        static void Main(string[] args)
        {
            var imageNames = GetFileNames();

            RunCPU(imageNames);
            ////RunILGPU(imageNames);
            ////RunAlea(imageNames);

            //ILGPUInverter.DoSomething();

            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }


}
