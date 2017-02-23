using System.Collections.Generic;
using Emgu.CV;
using IDS.IDS.IntervalTree;

namespace IDS.IDS.Classificator
{
   public interface IClassificator
   {
      bool Train(Matrix<float> trainData, IList<IndecesMapping> imap);
      CarModel GetClass(Matrix<float> data);
   }
}