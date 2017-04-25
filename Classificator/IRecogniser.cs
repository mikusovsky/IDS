using System.Collections.Generic;
using Emgu.CV;
using Emgu.CV.Structure;

namespace IDS.IDS.Classificator
{
   public interface IRecogniser
   {
      List<CarModel> ClassificationModels { get; }

      void LoadDb(Enums.DbType dbType, Enums.DescriptorType makerDescriptorType, Enums.DescriptorType modelDescriptorType, Enums.ClassificatorType classificator);
      CarModel Match(Image<Bgr, byte> image, bool? onlyCarMaker = null);
      CarModel Match(Image<Gray, byte> image, bool? onlyCarMaker = null);

      string TimeStats();
   }
}
