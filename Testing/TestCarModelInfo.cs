using System;
using System.Collections.Generic;
using System.Linq;

namespace IDS.IDS.Testing
{
   public class TestCarModelInfo
   {
      public CarModel CarModel;
      private List<CarModel> m_matches = new List<CarModel>();

      public TestCarModelInfo(CarModel model)
      {
         CarModel = model;
      }

      public void Add(CarModel model)
      {
         m_matches.Add(model);
      }

      public double GetPercentage()
      {
         return Math.Round(100 * GetCorrectMatchCount() / m_matches.Count, 2);
      }

      public double GetCorrectMatchCount()
      {
         var result = from model in m_matches
                      where model.ID == CarModel.ID
                      select model;

         double count = Convert.ToDouble(result.Count());
         return count;
      }

      public double GetMatchesCount()
      {
         return m_matches.Count;
      }
   }
}
