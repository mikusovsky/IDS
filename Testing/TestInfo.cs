using System;
using System.Collections.Generic;
using System.Text;

namespace IDS.IDS.Testing
{
   public class TestInfo
   {
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
         foreach (TestCarModelInfo modelInfo in m_testInfo.Values)
         {
            sb.Append($"{modelInfo.CarModel.ID} ------------------ {modelInfo.GetPercentage()}% {modelInfo.GetCorrectMatchCount()}/{modelInfo.GetMatchesCount()}{Environment.NewLine}");
         }
         return sb.ToString();
      }
   }
}