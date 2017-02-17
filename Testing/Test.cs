using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using Emgu.CV;
using Emgu.CV.Structure;
using IDS.IDS.Classificator;

namespace IDS.IDS.Testing
{
   public class Test
   {
      public void Execute(IClassificator classificator, Deffinitions.DbType testingDb, bool? onlyCarMaker = null)
      {
         classificator.LoadDb(Deffinitions.DbType.TrainingNormalized);
         List<CarModel> testingModels = Utils.GetAllCarModels(testingDb);
         bool onlyMakers = onlyCarMaker ?? false;
         TestInfo testInfo = new TestInfo(onlyMakers);
         int countNullFound = 0;
         int i = 1;
         foreach (CarModel model in testingModels)
         {
            foreach (string imagePath in model.ImagesPath)
            {
               using (Image<Bgr, byte> image = new Image<Bgr, byte>(imagePath))
               using (Image<Gray, byte> grayNormalizedMask = onlyMakers ? Utils.ToGray(image) :Utils.Resize(Utils.ToGray(Utils.ExtractMask3(image)), Deffinitions.NORMALIZE_MASK_WIDTH, Deffinitions.NORMALIZE_MASK_HEIGHT))
               {
                  CarModel findModel = classificator.Match(grayNormalizedMask, onlyCarMaker);
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
      }
   }
}