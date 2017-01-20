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
         classificator.LoadDb();
         List<CarModel> testingModels = Utils.GetAllCarModels(Deffinitions.DbType.Testing);
         TestInfo testInfo = new TestInfo();
         foreach (CarModel model in testingModels)
         {
            foreach (string imagePath in model.ImagesPath)
            {
               using (Image<Bgr, byte> image = new Image<Bgr, byte>(imagePath))
               {
                  CarModel findModel = classificator.Match(image);
                  testInfo.AddCouple(model, findModel);
               }
            }
         }

         string info = testInfo.GetTestInfo();
         Console.Write(info);
      }
   }
}