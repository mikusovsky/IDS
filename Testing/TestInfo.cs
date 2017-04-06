using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace IDS.IDS.Testing
{
   public class TestInfo
   {
      private bool m_onlyMakersCheck;
      private List<CarModel> m_testingModels;
      private Enums.DescriptorType m_makerDescriptorType;
      private Enums.DescriptorType m_modelDescriptorType;
      private Enums.DbType m_testedDbType;

      public TestInfo(List<CarModel> testingModels, Enums.DbType testedDbType, Enums.DescriptorType makerDescriptorType, Enums.DescriptorType modelDescriptorType, bool onlyMakers)
      {
         m_testingModels = testingModels;
         m_onlyMakersCheck = onlyMakers;
         m_makerDescriptorType = makerDescriptorType;
         m_modelDescriptorType = modelDescriptorType;
         m_testedDbType = testedDbType;
      }

      Dictionary<string, TestCarModelInfo> m_testInfo = new Dictionary<string, TestCarModelInfo>(); 

      public void AddCouple(CarModel model, CarModel findModel)
      {
         if (!m_testInfo.ContainsKey(model.ID))
         {
            m_testInfo[model.ID] = new TestCarModelInfo(model);
         }
         m_testInfo[model.ID].Add(findModel);
      }

      public string GetTestInfo()
      {
         StringBuilder sb = new StringBuilder();
         DateTime localDate = DateTime.Now;
         double allTestingImages = 0;
         double correctImages = 0;
         sb.Append($"-------------------- {localDate.ToString(new CultureInfo("ru-RU"))} {m_testedDbType} {m_makerDescriptorType} {m_modelDescriptorType}--------------------{Environment.NewLine}");
         foreach (TestCarModelInfo modelInfo in m_testInfo.Values)
         {
            double allTesting = modelInfo.GetMatchesCount();
            double correctMatches = m_onlyMakersCheck ? modelInfo.GetCorrectMakersMatchesCount() : modelInfo.GetCorrectMatchCount();
            string carDescription = m_onlyMakersCheck ? modelInfo.CarModel.Maker : modelInfo.CarModel.ID;
            allTestingImages += allTesting;
            correctImages += correctMatches;
            sb.Append($"{100 * correctMatches / allTesting}% {correctMatches}/{allTesting} ------------------ {carDescription} --- wrong matches {modelInfo.GetGroupsSorted()}{Environment.NewLine}");
         }
         sb.Append($"{Math.Round(100 * correctImages / allTestingImages, 2)}% {correctImages}/{allTestingImages}{Environment.NewLine}");

         sb.Append(Environment.NewLine);
         sb.Append(GetConfusionMatrix());
         sb.Append(Environment.NewLine);
         sb.Append(Environment.NewLine);
         sb.Append(GetConfusionMatrix(false));
         sb.Append($"---------------------------------------------------------------{Environment.NewLine}");
         
         return sb.ToString();
      }

      public string GetConfusionMatrix(bool percentual = true)
      {
         List<CarModel> carModels = new List<CarModel>();
         foreach (TestCarModelInfo modelInfo in m_testInfo.Values)
         {
            carModels.Add(modelInfo.CarModel);
         }

         ConfusionMatrix confusionMatrix = new ConfusionMatrix(carModels);
         foreach (TestCarModelInfo modelInfo in m_testInfo.Values)
         {
            foreach (CarModel matechedModel in modelInfo.GetMatches())
            {
               confusionMatrix.AddModel(modelInfo.CarModel, matechedModel);
            }
         }

         return confusionMatrix.ToString(percentual);
      }
   }
}