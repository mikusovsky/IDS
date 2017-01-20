using Emgu.CV;
using Emgu.CV.Structure;

namespace IDS.IDS.Classificator
{
   public interface IClassificator
   {
      void LoadDb();
      CarModel Match(Image<Bgr, byte> image);
   }
}
