using System.Collections;
using System.Collections.Generic;
using Emgu.CV;
using Emgu.CV.Structure;

namespace IDS.IDS.Classificator
{
   public interface IClassificator
   {
      List<CarModel> ClassificationModels { get; }

      void LoadDb(Deffinitions.DbType dbType);
      CarModel Match(Image<Bgr, byte> image);
      CarModel Match(Image<Gray, byte> image);
   }
}
