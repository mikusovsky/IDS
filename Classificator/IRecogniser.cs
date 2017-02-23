using System.Collections.Generic;
using Emgu.CV;
using Emgu.CV.Structure;

namespace IDS.IDS.Classificator
{
   public interface IRecogniser
   {
      List<CarModel> ClassificationModels { get; }

      void LoadDb(Deffinitions.DbType dbType, Deffinitions.DescriptorType descriptorType, Deffinitions.ClassificatorType classificator);
      CarModel Match(Image<Bgr, byte> image, bool? onlyCarMaker = null);
      CarModel Match(Image<Gray, byte> image, bool? onlyCarMaker = null);
   }
}
