using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

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

      public string GetGroupsSorted()
      {
         StringBuilder sb = new StringBuilder();
         var numberGroups = m_matches.GroupBy(o => o.ID);
         numberGroups = numberGroups.OrderByDescending(o => o.Count());
         foreach (var grp in numberGroups)
         {
            string modelId = grp.Key;
            int matchesCount = grp.Count();
            if (grp.Key != CarModel.ID)
            {
               sb.Append($"{modelId}\t{matchesCount}/{m_matches.Count}\t");
            }
         }
         return sb.ToString();
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
