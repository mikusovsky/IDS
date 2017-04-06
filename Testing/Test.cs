using System;
using System.Collections.Generic;
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
         Enums.DescriptorType makerDescriptorType = Enums.DescriptorType.SIFT;
         Enums.DescriptorType modelDescriptorType = Enums.DescriptorType.SURF;
         recogniser.LoadDb(Enums.DbType.TrainingNormalized, makerDescriptorType, modelDescriptorType, Enums.ClassificatorType.KNearest);
         List<CarModel> testingModels = Utils.GetCarModelsForDb(testingDb);
         bool onlyMakers = onlyCarMaker ?? false;
         TestInfo testInfo = new TestInfo(testingModels, testingDb, makerDescriptorType, modelDescriptorType, onlyMakers);
         int countNullFound = 0;
         foreach (CarModel model in testingModels)
         {
            foreach (string imagePath in model.ImagesPath)
            {
               using (Image<Bgr, byte> image = new Image<Bgr, byte>(imagePath))
               using (Image<Gray, byte> normlized = Utils.Resize(Utils.ToGray(image), Deffinitions.NORMALIZE_MASK_WIDTH, Deffinitions.NORMALIZE_MASK_HEIGHT))
               using (Image<Gray, byte> grayNormalizedMask = onlyMakers ? normlized : normlized)//Utils.Resize(Utils.ToGray(Utils.ExtractMask3(image)), Deffinitions.NORMALIZE_MASK_WIDTH, Deffinitions.NORMALIZE_MASK_HEIGHT)))
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
         Console.Write(info + Environment.NewLine + countNullFound);
         LogToFile(info, testingDb, makerDescriptorType, modelDescriptorType);
      }

      public void Execute(IRecogniser recogniser, List<Enums.DbType> testingDbs, bool? onlyCarMaker = null)
      {
         Enums.DescriptorType makerDescriptorType = Enums.DescriptorType.SIFT;
         Enums.DescriptorType modelDescriptorType = Enums.DescriptorType.SIFT;
         recogniser.LoadDb(Enums.DbType.TrainingNormalized, makerDescriptorType, modelDescriptorType, Enums.ClassificatorType.KNearest);
         foreach (Enums.DbType testingDb in testingDbs)
         {
            List<CarModel> testingModels = Utils.GetCarModelsForDb(testingDb);
            bool onlyMakers = onlyCarMaker ?? false;
            TestInfo testInfo = new TestInfo(testingModels, testingDb, makerDescriptorType, modelDescriptorType, onlyMakers);
            int countNullFound = 0;
            foreach (CarModel model in testingModels)
            {
               foreach (string imagePath in model.ImagesPath)
               {
                  using (Image<Bgr, byte> image = new Image<Bgr, byte>(imagePath))
                  using (Image<Gray, byte> normlized = Utils.Resize(Utils.ToGray(image), Deffinitions.NORMALIZE_MASK_WIDTH, Deffinitions.NORMALIZE_MASK_HEIGHT))
                  using (Image<Gray, byte> grayNormalizedMask = onlyMakers ? normlized : normlized)//Utils.Resize(Utils.ToGray(Utils.ExtractMask3(image)), Deffinitions.NORMALIZE_MASK_WIDTH, Deffinitions.NORMALIZE_MASK_HEIGHT)))
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
            Console.Write(info + Environment.NewLine + countNullFound);
            LogToFile(info, testingDb, makerDescriptorType, modelDescriptorType);
         }
      }

      public void LogToFile(string text, Enums.DbType testedDbType, Enums.DescriptorType makerDescriptorType, Enums.DescriptorType modelDescriptorType)
      {
         using (BinaryWriter writer = new BinaryWriter(File.Open(Utils.GetLogFilePath(testedDbType, makerDescriptorType, modelDescriptorType), FileMode.Append)))
         {
            writer.Write(text);
            writer.Write(Environment.NewLine);
            writer.Flush();
            writer.Close();
         }
      }
   }
}