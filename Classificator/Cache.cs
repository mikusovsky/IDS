using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using Emgu.CV;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using IDS.IDS.IntervalTree;

namespace IDS.IDS
{
   public static class Cache
   {
      private static SURFDetector SurfDetector = new SURFDetector(300, true, 4, 5);
      private static SIFTDetector SiftDetector = new SIFTDetector(75, 6, 0.04, 10, 1.6); // pridate parametre
      private static ORBDetector OrbDetector = new ORBDetector(500, (float)1.2, 8, 31, 0, 2, ORBDetector.ScoreType.Harris, 31);
      private static HOGDescriptor HogDescriptor = new HOGDescriptor(); // TODO 
      private static Dictionary<string, Matrix<float>> Descriptors = new Dictionary<string, Matrix<float>>();
      private static HashSet<string> Was = new HashSet<string>();
      private static Dictionary<string, XmlDocument> XmlDocuments = new Dictionary<string, XmlDocument>();
      private static Dictionary<string, Matrix<float>> AvarageModelMaps = new Dictionary<string, Matrix<float>>();
      private static Dictionary<string, CarModel> CachedCarModels = new Dictionary<string, CarModel>();  //string - savet path

      private const int CODE_INDEX_MAPING = 1;
      private const int CODE_END = 2;

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
         string cachePath = $"{Deffinitions.CACHE_PATH_DESCRIPTOR}\\{carModelName}";
         if (AvarageModelMaps.ContainsKey(carModelName))
         {
            return AvarageModelMaps[carModelName];
         }
         if (File.Exists(cachePath))
         {
            map = _LoadMatrix(cachePath);
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
         _SaveMatrix(cachePath, map);
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
         string cachePath = $"{Deffinitions.CACHE_PATH_DESCRIPTOR}\\{imageName}_descriptor{descriptor}_dbType{dbType}.cache";
         if (File.Exists(cachePath))
         {
            descs = _LoadMatrix(cachePath);
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
               case Enums.DescriptorType.ORB:
                  descs = _GetOrbDescriptor(img, importanceMap);
                  break;
            }
         }
         _SaveMatrix(cachePath, descs);
         Descriptors[imagePath] = descs;
         return descs;
      }

      public static VectorOfKeyPoint GetKeyPoints(Image<Gray, byte> image, Enums.DescriptorType descriptorType)
      {
         if (descriptorType == Enums.DescriptorType.SIFT)
         {
            return SiftDetector.DetectKeyPointsRaw(image, null);
         }
         else if (descriptorType == Enums.DescriptorType.ORB)
         {
            return OrbDetector.DetectKeyPointsRaw(image, null);
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

      private static Matrix<float> _GetOrbDescriptor(Image<Gray, byte> image, Matrix<float> importanceMap = null)
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
            VectorOfKeyPoint keyPoints = GetKeyPoints(edges, Enums.DescriptorType.ORB);
            Matrix<byte> ret = OrbDetector.ComputeDescriptorsRaw(edges, null, keyPoints);
            return null;
         }
      }

      public static bool TryGetFullDescriptor(Enums.DescriptorType descriptorType, Enums.DbType dbType, ref Matrix<float> descriptorMatrix, ref IList<IndecesMapping> imap)
      {
         string cachePathMatrix = $"{Deffinitions.CACHE_PATH_DESCRIPTOR}\\matrix_descriptor{descriptorType}_dbType{dbType}.cache";
         string cachePathImap = $"{Deffinitions.CACHE_PATH_DESCRIPTOR}\\imap_descriptor{descriptorType}_dbType{dbType}.cache";
         if (!File.Exists(cachePathMatrix) || !File.Exists(cachePathImap))
         {
            return false;
         }

         descriptorMatrix = _LoadMatrix(cachePathMatrix);
         imap = _Load(cachePathImap);
         return true;
      }

      public static void SaveFullDescriptor(Enums.DescriptorType descriptorType, Enums.DbType dbType, Matrix<float> descriptorMatrix, IList<IndecesMapping> imap)
      {
         string cachePathMatrix = $"{Deffinitions.CACHE_PATH_DESCRIPTOR}\\matrix_descriptor{descriptorType}_dbType{dbType}.cache";
         string cachePathImap = $"{Deffinitions.CACHE_PATH_DESCRIPTOR}\\imap_descriptor{descriptorType}_dbType{dbType}.cache";

         _SaveMatrix(cachePathMatrix, descriptorMatrix);
         _SaveList(cachePathImap, imap);
      }

      private static IList<IndecesMapping> _Load(string path)
      {
         List<IndecesMapping> list = new List<IndecesMapping>();
         if (File.Exists(path))
         {
            using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
            {
               var code = reader.ReadInt32();
               while (code != CODE_END)
               {
                  switch (code)
                  {
                     case CODE_INDEX_MAPING:
                        IndecesMapping indexMapping = new IndecesMapping();
                        indexMapping.Decode(reader);
                        list.Add(indexMapping);
                        break;
                  }
                  code = reader.ReadInt32();
               }
               reader.Close();
            }
         }
         return list;
      }

      public static void EncodeCarModel(BinaryWriter writer, CarModel carModel)
      {
         string writerPath = ((FileStream)(writer.BaseStream)).Name;
         string carModelPath = $"{Path.GetDirectoryName(writerPath)}\\{Path.GetFileNameWithoutExtension(writerPath)}_{carModel.ID}.cache";
         writer.Write(carModelPath);
         if (Cache.IsCachet(carModelPath))
         {
            return;
         }
         using (BinaryWriter writerCarModel = new BinaryWriter(File.Open(carModelPath, FileMode.Create)))
         {
            carModel.Encode(writerCarModel);
            CachedCarModels[carModelPath] = carModel;
         }
      }

      public static CarModel DecodeCarModel(BinaryReader reader)
      {
         string carModelPath = reader.ReadString();
         if (!CachedCarModels.ContainsKey(carModelPath))
         {
            if (File.Exists(carModelPath))
            {
               using (BinaryReader readerCarModel = new BinaryReader(File.Open(carModelPath, FileMode.Open)))
               {
                  CarModel carModel = new CarModel();
                  carModel.Decode(readerCarModel);
                  CachedCarModels[carModelPath] = carModel;
               }
            }
         }
         return CachedCarModels[carModelPath];
      }

      private static void _SaveList(string path, IList<IndecesMapping> list)
      {
         using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
         {
            foreach (IndecesMapping indexMapping in list)
            {
               writer.Write(CODE_INDEX_MAPING);
               indexMapping.Encode(writer);
            }
            writer.Write(CODE_END);
            writer.Flush();
            writer.Close();
         }
      }

      private static Matrix<float> _LoadMatrix(string path)
      {
         Matrix<float> matrix = null;
         if (!File.Exists(path))
         {
            return matrix;
         }
         using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
         {
            int rows = reader.ReadInt32();
            int cols = reader.ReadInt32();
            matrix = new Matrix<float>(rows, cols);
            for (int i = 0; i < rows; i++)
            {
               for (int j = 0; j < cols; j++)
               {
                  matrix[i, j] = reader.ReadSingle();
               }
            }
         }
         return matrix;
         /*
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
         */
      }

      private static void _SaveMatrix(string path, Matrix<float> matrix)
      {
         if (matrix == null)
         {
            return;
         }
         using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
         {
            writer.Write(matrix.Rows);
            writer.Write(matrix.Cols);
            for (int i = 0; i < matrix.Rows; i++)
            {
               for (int j = 0; j < matrix.Cols; j++)
               {
                  writer.Write(matrix[i, j]);
               }
            }
            writer.Flush();
            writer.Close();
         }
         /*
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
            */
      }

      public static bool IsCachet(string path)
      {
         return File.Exists(path);
      }
   }
}