using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Structure;

namespace IDS.IDS.Classificator
{
   public class ModelRecogniser : IRecogniser
   {
      private int m_classificatorType = 2;

      IRecogniser _mBrandRecogniser;
      readonly Dictionary<string, IRecogniser> m_modelRecogniser = new Dictionary<string, IRecogniser>();

      public List<CarModel> ClassificationModels { get; }

      public void LoadDb(Enums.DbType dbType, Enums.DescriptorType descriptorType, Enums.ClassificatorType classificatorType)
      {
         _mBrandRecogniser = new Recogniser();
         _mBrandRecogniser.LoadDb(Enums.DbType.TrainingBrand, Enums.DescriptorType.SIFT, Enums.ClassificatorType.SVM);
         var makers = _mBrandRecogniser.ClassificationModels.GroupBy(o => o.Maker);
         int i = 0;
         foreach (var g in makers)
         {
            foreach (CarModel maker in g)
            {
               IRecogniser recogniser = new Recogniser();
               recogniser.LoadDb(Utils.GetDbTypeForMakerString(maker.Maker), Enums.DescriptorType.SURF, Enums.ClassificatorType.SVM);
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
         Image<Gray, byte> brandMaskPart = _GetBrandPartOfMask(mask);
         //Utils.AdaptiveBrightnes(brandMaskPart);
         //Utils.SaveImage(brandMaskPart, $"D:\\Skola\\UK\\DiplomovaPraca\\PokracovaniePoPredchodcovi\\zdrojové kódy\\Output\\Images\\OnlyMask\\{(Utils.RandomString(5))}.jpg", ImageFormat.Jpeg);

         CarModel findedBrand = _mBrandRecogniser.Match(brandMaskPart);
         if (getOnlyMaker)
         {
            return findedBrand;
         }
         CarModel findedModel = null;
         if (findedBrand != null)
         {
            findedModel = m_modelRecogniser[findedBrand.Maker].Match(mask);
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