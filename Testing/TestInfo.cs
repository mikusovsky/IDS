using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace IDS.IDS.Testing
{
   public class TestInfo
   {
      private bool m_onlyMakersCheck;

      public TestInfo(bool onlyMakers)
      {
         m_onlyMakersCheck = onlyMakers;
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
         sb.Append($"-------------------- {localDate.ToString(new CultureInfo("ru-RU"))} --------------------{Environment.NewLine}");
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
         sb.Append($"---------------------------------------------------------------{Environment.NewLine}");
         return sb.ToString();
      }
   }
}