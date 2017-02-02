using System;
using System.Collections.Generic;
using Emgu.CV;
using Emgu.CV.Structure;
using IDS.IDS.Classificator;

namespace IDS.IDS.Testing
{
   public class Test
   {
      public void Execute(IClassificator classificator)
      {
         classificator.LoadDb(Deffinitions.DbType.TrainingNormalized);
         List<CarModel> testingModels = Utils.GetAllCarModels(Deffinitions.DbType.Testing);
         TestInfo testInfo = new TestInfo();
         int countNullFound = 0;
         foreach (CarModel model in testingModels)
         {
            foreach (string imagePath in model.ImagesPath)
            {
               using (Image<Bgr, byte> image = new Image<Bgr, byte>(imagePath))
               using (Image<Gray, byte> grayNormalizedMask = Utils.Resize(Utils.ToGray(Utils.ExtractMask3(image)), Deffinitions.NORMALIZE_MASK_WIDTH, Deffinitions.NORMALIZE_MASK_HEIGHT))
               {
                  CarModel findModel = classificator.Match(grayNormalizedMask);
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