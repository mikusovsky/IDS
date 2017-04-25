using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.Structure;

namespace IDS.IDS.Classificator
{
   public class ModelRecogniser : IRecogniser
   {
      private int m_classificatorType = 2;
      Recogniser _mBrandRecogniser;
      readonly Dictionary<string, Recogniser> m_modelRecogniser = new Dictionary<string, Recogniser>();

      /*statistics*/
      private double m_brandMinTime = 99999;
      private double m_brandMaxTime = 0;
      private double m_brandTotalTime = 0;
      private double m_brandCount = 0;
      private double m_modelMinTime = 99999;
      private double m_modelMaxTime = 0;
      private double m_modelTotalTime = 0;
      private double m_modelCount = 0;

      public List<CarModel> ClassificationModels { get; }

      public string TimeStats()
      {
         StringBuilder sb = new StringBuilder();
         sb.Append(m_brandMinTime + "\t");
         sb.Append(m_brandMaxTime + "\t");
         sb.Append(m_brandTotalTime + "\t");
         sb.Append(m_brandCount + "\t");
         sb.Append(m_brandTotalTime/m_brandCount + "\t");
         sb.Append(Environment.NewLine);
         sb.Append(m_modelMinTime + "\t");
         sb.Append(m_modelMaxTime + "\t");
         sb.Append(m_modelTotalTime + "\t");
         sb.Append(m_modelCount + "\t");
         sb.Append(m_modelTotalTime / m_modelCount + "\t");
         return sb.ToString();
      }

      public void LoadDb(Enums.DbType dbType, Enums.DescriptorType makerDescriptorType, Enums.DescriptorType modelDescriptorType, Enums.ClassificatorType classificatorType)
      {
         _mBrandRecogniser = new Recogniser();
         _mBrandRecogniser.LoadDb(Enums.DbType.TrainingBrand, makerDescriptorType, Enums.ClassificatorType.KNearest);

         var makers = _mBrandRecogniser.ClassificationModels.GroupBy(o => o.Maker);
         foreach (var g in makers)
         {
            foreach (CarModel maker in g)
            {
               Recogniser recogniser = new Recogniser();
               recogniser.LoadDb(Utils.GetDbTypeForMakerString(maker.Maker), modelDescriptorType, Enums.ClassificatorType.KNearest);
               m_modelRecogniser[maker.Maker] = recogniser;
            }
         }
      }

      public CarModel Match(Image<Bgr, byte> image, bool? onlyCarMaker = null)
      {
         return Match(Utils.ToGray(image), onlyCarMaker);
      }

      public CarModel Match(Image<Gray, byte> image, bool? onlyCarMaker = null)
      {
         bool getOnlyMaker = onlyCarMaker ?? false;
         Image<Gray, byte> mask = image;//_GetNormalisedGrayMask(image);

         Stopwatch sw1 = new Stopwatch();
         sw1.Start();

         Image<Gray, byte> brandMaskPart = _GetBrandPartOfMask(mask);
         //Utils.AdaptiveBrightnes(brandMaskPart);
         //Utils.SaveImage(brandMaskPart, $"D:\\Skola\\UK\\DiplomovaPraca\\PokracovaniePoPredchodcovi\\zdrojové kódy\\Output\\Images\\OnlyMask\\{(Utils.RandomString(5))}.jpg", ImageFormat.Jpeg);

         CarModel findedBrand = _mBrandRecogniser.Match(brandMaskPart);
         sw1.Stop();
         _MesureBrandTime(sw1.ElapsedMilliseconds);
         if (getOnlyMaker)
         {
            return findedBrand;
         }
         CarModel findedModel = null;
         if (findedBrand != null)
         {
            Stopwatch sw2 = new Stopwatch();
            sw2.Start();
            findedModel = m_modelRecogniser[findedBrand.Maker].Match(mask);
            sw2.Stop();
            _MesureModelTime(sw2.ElapsedMilliseconds);
         }
         return findedModel;
      }

      private Image<Gray, byte> _GetNormalisedGrayMask(Image<Gray, byte> image)
      {
         Image<Gray, byte> grayMask = Utils.ExtractMask3(image);
         Utils.LogImage("grayMask mask", grayMask);
         //grayMask._EqualizeHist();
         //grayMask._GammaCorrect(2.5d);
         Image<Gray, byte> normalisedGrayMask = Utils.Resize(/*Utils.ToGray(image)*/grayMask, Deffinitions.NORMALIZE_MASK_WIDTH, Deffinitions.NORMALIZE_MASK_HEIGHT);
         return normalisedGrayMask;
      }

      private Image<Gray, byte> _GetBrandPartOfMask(Image<Gray, byte> normisedMask)
      {
         return Utils.CropImage(normisedMask, 30, 15, 55, 55);
      }

      private void _MesureBrandTime(long time)
      {
         double dTime = Convert.ToDouble(time);
         m_brandMinTime = m_brandMinTime > dTime ? dTime : m_brandMinTime;
         m_brandMaxTime = m_brandMaxTime < dTime ? dTime : m_brandMaxTime;
         m_brandCount++;
         m_brandTotalTime += dTime;
      }

      private void _MesureModelTime(long time)
      {
         double dTime = Convert.ToDouble(time);
         m_modelMinTime = m_modelMinTime > dTime ? dTime : m_modelMinTime;
         m_modelMaxTime = m_modelMaxTime < dTime ? dTime : m_modelMaxTime;
         m_modelCount++;
         m_modelTotalTime += dTime;
      }
   }
}