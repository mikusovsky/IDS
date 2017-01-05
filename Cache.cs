using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace IDS.IDS
{
   public static class Cache
   {

      private const double SURF_HESSIAN_THRESH = 300;
      private const bool SURF_EXTENDED_FLAG = true;
      private static SURFDetector SurfDetector = new SURFDetector(SURF_HESSIAN_THRESH, SURF_EXTENDED_FLAG);
      private static Dictionary<string, Matrix<float>> SurfDescriptors = new Dictionary<string, Matrix<float>>();

      public static Matrix<float> GetSurfDescriptor(string imagePath)
      {
         if (SurfDescriptors.ContainsKey(imagePath))
         {
            return SurfDescriptors[imagePath];
         }

         Matrix<float> descs;
         string imageName = Path.GetFileName(imagePath);
         string cachePath = $"{Deffinitions.CACHE_PATH}\\{Path.GetFileName(imageName)}.cache";
         if (File.Exists(cachePath))
         {
            string[] lines = File.ReadAllLines(cachePath);
            descs = new Matrix<float>(lines.Length, lines[0].Split(' ').Length);
            for (int i = 0; i < lines.Length; i++)
            {
               float[] values = lines[i].Split(' ').ToList().Select(x => (float) Convert.ToDouble(x)).ToArray();
               for (int j = 0; j < values.Length; j++)
               {
                  descs[i, j] = values[j];
               }
            }
            return descs;
         }

         using (Image<Gray, Byte> img = new Image<Gray, byte>(imagePath))
         {
            VectorOfKeyPoint keyPoints = SurfDetector.DetectKeyPointsRaw(img, null);
            descs = SurfDetector.ComputeDescriptorsRaw(img, null, keyPoints);
         }

         using (TextWriter tw = new StreamWriter(cachePath))
         {
            for (int i = 0; i < descs.Rows; i++)
            {
               for (int j = 0; j < descs.Cols; j++)
               {
                  tw.Write($"{(j != 0 ? " " : "")}{descs[i, j]}");
               }
               tw.WriteLine();
            }
            tw.Flush();
         }
         return descs;
      }
      
      public static Matrix<float> GetSurfDescriptor(Image<Gray, byte> imagePath)
      {
         VectorOfKeyPoint keyPoints = SurfDetector.DetectKeyPointsRaw(imagePath, null);
         return SurfDetector.ComputeDescriptorsRaw(imagePath, null, keyPoints);
      }
   }
}