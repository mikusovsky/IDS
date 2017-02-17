using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Structure;

namespace IDS.IDS.Classificator
{
   public class ModelClassificator : IClassificator
   {
      private int m_classificatorType = 2;

      IClassificator m_brandClassificator;
      Dictionary<string, IClassificator> m_modelClassificators = new Dictionary<string, IClassificator>();

      public List<CarModel> ClassificationModels { get; }

      public void LoadDb(Deffinitions.DbType dbType)
      {
         m_brandClassificator = _GetCassifitacotr(Deffinitions.DescriptorType.SIFT);
         m_brandClassificator.LoadDb(Deffinitions.DbType.TrainingBrand);
         var makers = m_brandClassificator.ClassificationModels.GroupBy(o => o.Maker);
         
         foreach (var g in makers)
         {
            foreach (CarModel maker in g)
            {
               IClassificator classificator = _GetCassifitacotr(Deffinitions.DescriptorType.SURF);
               classificator.LoadDb(Utils.GetDbTypeForMakerString(maker.Maker));
               m_modelClassificators[maker.Maker] = classificator;
            }
         }
      }

      private IClassificator _GetCassifitacotr(Deffinitions.DescriptorType type)
      {
         if (Deffinitions.DescriptorType.SURF == type)
         {
            return new SurfClassificator();
         }
         return new SiftClassificator();
      }

      public CarModel Match(Image<Bgr, byte> image, bool? onlyCarMaker = null)
      {
         return Match(Utils.ToGray(image), onlyCarMaker);
      }

      public CarModel Match(Image<Gray, byte> image, bool? onlyCarMaker = null)
      {
         bool getOnlyMaker = onlyCarMaker ?? false;
         Image<Gray, byte> mask = image;//_GetNormalisedGrayMask(image);
         Image<Gray, byte> brandMaskPart = _GetBrandPartOfMask(mask);
         //Utils.AdaptiveBrightnes(brandMaskPart);
         //Utils.SaveImage(brandMaskPart, $"D:\\Skola\\UK\\DiplomovaPraca\\PokracovaniePoPredchodcovi\\zdrojové kódy\\Output\\Images\\OnlyMask\\{(Utils.RandomString(5))}.jpg", ImageFormat.Jpeg);

         CarModel findedBrand = m_brandClassificator.Match(brandMaskPart);
         if (getOnlyMaker)
         {
            return findedBrand;
         }
         CarModel findedModel = null;
         if (findedBrand != null)
         {
            findedModel = m_modelClassificators[findedBrand.Maker].Match(mask);
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
   }
}