using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Emgu.CV;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace IDS.IDS
{
   public static class Cache
   {
      private static SURFDetector SurfDetector = new SURFDetector(300, true, 4, 5);
      private static SIFTDetector SiftDetector = new SIFTDetector(75, 6, 0.04, 10, 1.6); // pridate parametre
      private static HOGDescriptor HogDescriptor = new HOGDescriptor(); // TODO 
      private static Dictionary<string, Matrix<float>> Descriptors = new Dictionary<string, Matrix<float>>();
      private static HashSet<string> Was = new HashSet<string>();
      private static Dictionary<string, XmlDocument> XmlDocuments = new Dictionary<string, XmlDocument>();
      private static Dictionary<string, Matrix<float>> AvarageModelMaps = new Dictionary<string, Matrix<float>>();

      public static XmlDocument GetXMLDocument(string path)
      {
         if (!XmlDocuments.ContainsKey(path))
         {
            XmlDocument config = new XmlDocument();
            config.Load(path);
            XmlDocuments[path] = config;
         }
         return XmlDocuments[path];
      }

      public static Matrix<float> GetModelAvarageMap(CarModel carModel)
      {
         int rows = Deffinitions.NORMALIZE_MASK_WIDTH;
         int cols = Deffinitions.NORMALIZE_MASK_HEIGHT;
         Matrix<float> map = new Matrix<float>(rows, cols);
         string carModelName = $"{carModel.Maker}-{carModel.Model}-{carModel.Generation}-AvarageMap.cache";
         string cachePath = $"{Deffinitions.CACHE_PATH}\\{carModelName}";
         if (AvarageModelMaps.ContainsKey(carModelName))
         {
            return AvarageModelMaps[carModelName];
         }
         if (File.Exists(cachePath))
         {
            map = LoadMatrix(cachePath);
            AvarageModelMaps[carModelName] = map;
            return map;
         }
         float maxValue = 0;
         for (int i = 0; i < carModel.ImagesPath.Count; i++)
         {
            using (Image<Gray, byte> img = new Image<Gray, byte>(carModel.ImagesPath[i]))
            using (Image<Gray, byte> normalizedImage = Utils.Resize(img, rows, cols))
            using (Image<Gray, byte> cannyGray = normalizedImage.Canny(new Gray(10), new Gray(60)))
            {
               for (int j = 0; j < cannyGray.Rows; j++)
               {
                  for (int k = 0; k < cannyGray.Cols; k++)
                  {
                     map[j, k] += Convert.ToSingle(cannyGray[j, k].Intensity);
                     if (i == carModel.ImagesPath.Count - 1 && maxValue < map[j, k])
                     {
                        maxValue = map[j, k];
                     }
                  }
               }
            }
         }

         for (int j = 0; j < rows; j++)
         {
            for (int k = 0; k < cols; k++)
            {
               map[j, k] *= 255 / maxValue;
               if (map[j, k] < 100)
               {
                  map[j, k] = 0;
               }
            }
         }
         SaveMatrix(cachePath, map);
         AvarageModelMaps[carModelName] = map;
         return map;
      }

      public static Matrix<float> GetDescriptor(Image<Gray, byte> image, Matrix<float> importanceMap, Enums.DescriptorType descriptor)
      {
         Matrix<float> descs = null;
         switch (descriptor)
         {
            case Enums.DescriptorType.SURF:
               descs = _GetSurfDescriptor(image, importanceMap);
               break;
            case Enums.DescriptorType.SIFT:
               descs = _GetSiftDescriptor(image, importanceMap);
               break;
         }
         return descs;
      }

      public static Matrix<float> GetDescriptor(string imagePath, Matrix<float> importanceMap, Enums.DescriptorType descriptor, Enums.DbType dbType)
      {
         string imagePathDesriptor = imagePath + descriptor + dbType;
         /*
         if (!Was.Add(Path.GetFileName(imagePathDesriptor)))
         {
            Console.WriteLine("duplicateName - " + imagePathDesriptor);
         }
         */
         if (Descriptors.ContainsKey(imagePathDesriptor))
         {
            return Descriptors[imagePathDesriptor];
         }

         Matrix<float> descs = null;
         string imageName = Path.GetFileName(imagePath);
         string cachePath = $"{Deffinitions.CACHE_PATH}\\{imageName}_descriptor{descriptor}_dbType{dbType}.cache";
         if (File.Exists(cachePath))
         {
            descs = LoadMatrix(cachePath);
            return descs;
         }

         using (Image<Gray, byte> img = new Image<Gray, byte>(imagePath))
         {
            switch (descriptor)
            {
               case Enums.DescriptorType.SURF:
                  descs = _GetSurfDescriptor(img, importanceMap);
                  break;
               case Enums.DescriptorType.SIFT:
                  descs = _GetSiftDescriptor(img, importanceMap);
                  break;
            }
         }
         SaveMatrix(cachePath, descs);
         Descriptors[imagePath] = descs;
         return descs;
      }

      public static VectorOfKeyPoint GetKeyPoints(Image<Gray, byte> image, Enums.DescriptorType descriptorType)
      {
         if (descriptorType == Enums.DescriptorType.SIFT)
         {
            return SiftDetector.DetectKeyPointsRaw(image, null);
         }
         return SurfDetector.DetectKeyPointsRaw(image, null);
      }

      private static Matrix<float> _GetSurfDescriptor(Image<Gray, byte> image, Matrix<float> importanceMap = null)
      {
         Image<Gray, byte> testImage = image;
         using (Matrix<float> map = Utils.ImageToMap(image))
         {
            if (importanceMap != null)
            {
               for (int i = 0; i < map.Rows; i++)
               {
                  for (int j = 0; j < map.Cols; j++)
                  {
                     map[i, j] *= (importanceMap[i, j] / 255);
                  }
               }
               testImage = Utils.MapToImage(map);
               Utils.LogImage("image after mask aplication", testImage);
            }
            Image<Gray, byte> edges = testImage; //Utils.ExtractEdges(image);
            VectorOfKeyPoint keyPoints = GetKeyPoints(edges, Enums.DescriptorType.SURF);
            return SurfDetector.ComputeDescriptorsRaw(edges, null, keyPoints);
         }
      }

      private static Matrix<float> _GetSiftDescriptor(Image<Gray, byte> image, Matrix<float> importanceMap = null)
      {
         Image<Gray, byte> testImage = image;
         using (Matrix<float> map = Utils.ImageToMap(image))
         {
            if (importanceMap != null)
            {
               for (int i = 0; i < map.Rows; i++)
               {
                  for (int j = 0; j < map.Cols; j++)
                  {
                     map[i, j] *= (importanceMap[i, j] / 255);
                  }
               }
               testImage = Utils.MapToImage(map);
               Utils.LogImage("image after mask aplication", testImage);
            }
            Image<Gray, byte> edges = testImage; //Utils.ExtractEdges(image);
            VectorOfKeyPoint keyPoints = GetKeyPoints(edges, Enums.DescriptorType.SIFT);
            return SiftDetector.ComputeDescriptorsRaw(edges, null, keyPoints);
         }
      }

      private static Matrix<float> LoadMatrix(string path)
      {
         if (!File.Exists(path))
         {
            return null;
         }
         string[] lines = File.ReadAllLines(path);
         Matrix<float> map = null;
         for (int i = 0; i < lines.Length; i++)
         {
            float[] values = lines[i].Split(' ').ToList().Select(x => (float)Convert.ToDouble(x)).ToArray();
            if (map == null)
            {
               map = new Matrix<float>(lines.Length, values.Length);
            }
            for (int j = 0; j < values.Length; j++)
            {
               map[i, j] = values[j];
            }
         }
         return map;
      }

      private static void SaveMatrix(string path, Matrix<float> matrix)
      {
         if (matrix == null)
         {
            return;
         }
         using (TextWriter tw = new StreamWriter(path))
         {
            for (int i = 0; i < matrix.Rows; i++)
            {
               for (int j = 0; j < matrix.Cols; j++)
               {
                  tw.Write($"{(j != 0 ? " " : "")}{matrix[i, j]}");
               }
               tw.WriteLine();
            }
            tw.Flush();
         }
      }
   }
}