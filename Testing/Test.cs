using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Emgu.CV;
using Emgu.CV.Structure;
using IDS.IDS.Classificator;

namespace IDS.IDS.Testing
{
   public class Test
   {
      public void Execute(IRecogniser recogniser, Enums.DbType testingDb, bool? onlyCarMaker = null)
      {
         Enums.DescriptorType makerDescriptorType = Enums.DescriptorType.SURF;
         Enums.DescriptorType modelDescriptorType = Enums.DescriptorType.SURF;
         recogniser.LoadDb(Enums.DbType.TrainingNormalized, makerDescriptorType, modelDescriptorType,
            Enums.ClassificatorType.KNearest);
         List<CarModel> testingModels = Utils.GetCarModelsForDb(testingDb);
         bool onlyMakers = onlyCarMaker ?? false;
         TestInfo testInfo = new TestInfo(testingModels, testingDb, makerDescriptorType, modelDescriptorType, onlyMakers);
         int countNullFound = 0;
         foreach (CarModel model in testingModels)
         {
            foreach (string imagePath in model.ImagesPath)
            {
               using (Image<Bgr, byte> image = new Image<Bgr, byte>(imagePath))
               using (
                  Image<Gray, byte> normlized = Utils.Resize(Utils.ToGray(image), Deffinitions.NORMALIZE_MASK_WIDTH,
                     Deffinitions.NORMALIZE_MASK_HEIGHT))
               using (Image<Gray, byte> grayNormalizedMask = onlyMakers ? normlized : normlized)
                  //Utils.Resize(Utils.ToGray(Utils.ExtractMask3(image)), Deffinitions.NORMALIZE_MASK_WIDTH, Deffinitions.NORMALIZE_MASK_HEIGHT)))
               {
                  CarModel findModel = recogniser.Match(grayNormalizedMask, onlyCarMaker);
                  if (findModel != null)
                  {
                     testInfo.AddCouple(model, findModel);
                  }
                  else
                  {
                     countNullFound++;
                  }
               }
            }
         }

         string info = testInfo.GetTestInfo();
         Console.Write(info + Environment.NewLine + countNullFound + Environment.NewLine + recogniser.TimeStats());
         LogToFile(info, testingDb, makerDescriptorType, modelDescriptorType);
      }

      public void Execute(IRecogniser recogniser, List<Enums.DbType> testingDbs, bool? onlyCarMaker = null)
      {
         Enums.DescriptorType makerDescriptorType = Enums.DescriptorType.SIFT;
         Enums.DescriptorType modelDescriptorType = Enums.DescriptorType.SIFT;
         recogniser.LoadDb(Enums.DbType.TrainingNormalized, makerDescriptorType, modelDescriptorType,
            Enums.ClassificatorType.KNearest);
         foreach (Enums.DbType testingDb in testingDbs)
         {
            List<CarModel> testingModels = Utils.GetCarModelsForDb(testingDb);
            bool onlyMakers = onlyCarMaker ?? false;
            TestInfo testInfo = new TestInfo(testingModels, testingDb, makerDescriptorType, modelDescriptorType,
               onlyMakers);
            int countNullFound = 0;
            foreach (CarModel model in testingModels)
            {
               foreach (string imagePath in model.ImagesPath)
               {
                  using (Image<Bgr, byte> image = new Image<Bgr, byte>(imagePath))
                  using (
                     Image<Gray, byte> normlized = Utils.Resize(Utils.ToGray(image), Deffinitions.NORMALIZE_MASK_WIDTH,
                        Deffinitions.NORMALIZE_MASK_HEIGHT))
                  using (Image<Gray, byte> grayNormalizedMask = onlyMakers ? normlized : normlized)
                     //Utils.Resize(Utils.ToGray(Utils.ExtractMask3(image)), Deffinitions.NORMALIZE_MASK_WIDTH, Deffinitions.NORMALIZE_MASK_HEIGHT)))
                  {
                     CarModel findModel = recogniser.Match(grayNormalizedMask, onlyCarMaker);
                     if (findModel != null)
                     {
                        testInfo.AddCouple(model, findModel);
                     }
                     else
                     {
                        countNullFound++;
                     }
                  }
               }
            }

            string info = testInfo.GetTestInfo();
            Console.Write(info + Environment.NewLine + countNullFound + Environment.NewLine + recogniser.TimeStats());
            LogToFile(info, testingDb, makerDescriptorType, modelDescriptorType);
         }
      }

      public void LogToFile(string text, Enums.DbType testedDbType, Enums.DescriptorType makerDescriptorType,
         Enums.DescriptorType modelDescriptorType)
      {
         using (
            BinaryWriter writer =
               new BinaryWriter(File.Open(Utils.GetLogFilePath(testedDbType, makerDescriptorType, modelDescriptorType),
                  FileMode.Append)))
         {
            writer.Write(text);
            writer.Write(Environment.NewLine);
            writer.Flush();
            writer.Close();
         }
      }

      public void MesureMaskExtraction()
      {
         List<CarModel> testingModels = Utils.GetCarModelsForDb(Enums.DbType.Testing);
         List<Image<Bgr, byte>> images = new List<Image<Bgr, byte>>();
         foreach (CarModel model in testingModels)
         {
            foreach (string imagePath in model.ImagesPath)
            {
               images.Add(new Image<Bgr, byte>(imagePath));
            }
         }
         //_MesureMaskExtraction(images, Enums.MaskExtractorType.SPZ);
         //_MesureMaskExtraction(images, Enums.MaskExtractorType.BoudingBox);
         _MesureMaskExtraction(images, Enums.MaskExtractorType.SlidingWindow);
      }

      private void _MesureMaskExtraction(List<Image<Bgr, byte>> images, Enums.MaskExtractorType extractorType)
      {
         double minTime = 999999;
         double maxTime = 0;
         double allTime = 0;
         Stopwatch sw1 = new Stopwatch();
         int i = 0;
         List<Image<Bgr, byte>> maskImages = new List<Image<Bgr, byte>>();
         foreach (Image<Bgr, byte> image in images)
         {
            sw1.Restart();
            switch (extractorType)
            {
               case Enums.MaskExtractorType.SPZ:
                  maskImages.Add(Utils.ExtractMask(image));
                  break;

               case Enums.MaskExtractorType.BoudingBox:
                  maskImages.Add(Utils.ExtractMask2(image));
                  break;

               case Enums.MaskExtractorType.SlidingWindow:
                  maskImages.Add(Utils.ExtractMask3(image, true));
                  break;
            }
            sw1.Stop();
            if (i > 2)
            {
               double spendTime = Convert.ToDouble(sw1.ElapsedMilliseconds);
               minTime = minTime > spendTime ? spendTime : minTime;
               maxTime = maxTime < spendTime ? spendTime : maxTime;
               allTime += spendTime;
            }
            i++;
         }
         string outPath = $"{Deffinitions.OUTPUT_PATH_IMAGES_TEMP}\\{extractorType}";
         Directory.CreateDirectory(outPath);
         int k = 1;
         int wrong = 0;
         foreach (Image<Bgr, byte> image in maskImages)
         {
            if (image == null)
            {
               wrong ++;
               continue;
            }
            using (Bitmap normallized = (Utils.ChangePixelFormat(image.Bitmap, PixelFormat.Format32bppArgb)))
            {
               normallized.Save($"{outPath}\\{k++}.jpg", ImageFormat.Jpeg);
            }
         }
         Console.WriteLine(wrong);
         Console.WriteLine($"{minTime}\t{maxTime}\t{allTime}\t{images.Count}\t{allTime/images.Count}");
      }
   }
}