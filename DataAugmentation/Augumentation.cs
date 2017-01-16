using System;
using System.Collections.Generic;
using Emgu.CV;
using Emgu.CV.Structure;

namespace IDS.IDS.DataAugmentation
{
   public static class Augumentation
   {

      public static Image<Bgr, byte> ImageBlur(Image<Bgr, byte> img, Enums.TypeOfBlur type, List <int> parmeters = null)
      {
         Image<Bgr, byte> ret = null;
         switch (type)
         {
            case Enums.TypeOfBlur.Bilatral:
               ret = img.SmoothBilatral(7, 255, 34);
               break;
            case Enums.TypeOfBlur.Blur:
               ret = img.SmoothBlur(10, 10, true);
                  break;
            case Enums.TypeOfBlur.Gaussian:
               ret = img.SmoothGaussian(3, 3, 34.3, 45.3);
               break;
            case Enums.TypeOfBlur.Median:
               ret = img.SmoothMedian(15);
               break;
         }
         return ret;
      }

      public static Image<Bgr, byte> ImageBlur(Image<Bgr, byte> img, int width, int height)
      {
         return img.SmoothBlur(width, height, true);
      }

      public static Image<Gray, byte> ImageBlur(Image<Gray, byte> img, int width, int height)
      {
         return img.SmoothBlur(width, height, true);
      }

      public static void GenerateImageBlur(ref List<Image<Bgr, byte>> images)
      {
         Image<Bgr, byte>[] arrImages = images.ToArray();
         foreach (Image<Bgr, byte> img in arrImages)
         {
            images.Add(ImageBlur(img, 5, 5));
            images.Add(ImageBlur(img, 11, 11));
         }
      }

      public static void GenerateImageBlur(ref List<Image<Gray, byte>> images)
      {
         Image<Gray, byte>[] arrImages = images.ToArray();
         foreach (Image<Gray, byte> img in arrImages)
         {
            images.Add(ImageBlur(img, 7, 7));
            images.Add(ImageBlur(img, 11, 11));
         }
      }

      public static Image<Bgr, byte> ChangeBrightness(Image<Bgr, byte> img, double gamma, bool equalizeHist = true)
      {
         Image<Bgr, byte> ret = img.Clone();
         if (equalizeHist)
         {
            ret._EqualizeHist();
         }
         ret._GammaCorrect(gamma);
         return ret;
      }

      public static Image<Gray, byte> ChangeBrightness(Image<Gray, byte> img, double gamma, bool equalizeHist = true)
      {
         Image<Gray, byte> ret = img.Clone();
         if (equalizeHist)
         {
            ret._EqualizeHist();
         }
         ret._GammaCorrect(gamma);
         return ret;
      }

      public static void GenerateChangeBrightness(ref List<Image<Bgr, byte>> images)
      {
         Image<Bgr, byte>[] arrImages = images.ToArray();
         foreach (Image<Bgr, byte> img in arrImages)
         {
            images.Add(ChangeBrightness(img, 0.75));
            images.Add(ChangeBrightness(img, 1.5));
            images.Add(ChangeBrightness(img, 1.5, false));
         }
      }
      public static void GenerateChangeBrightness(ref List<Image<Gray, byte>> images)
      {
         Image<Gray, byte>[] arrImages = images.ToArray();
         foreach (Image<Gray, byte> img in arrImages)
         {
            images.Add(ChangeBrightness(img, 0.75));
            images.Add(ChangeBrightness(img, 1.5));
            images.Add(ChangeBrightness(img, 1.5, false));
         }
      }

      public static void MakeAugumentation(ref List<Image<Bgr, byte>> images)
      {
         GenerateImageBlur(ref images);
         GenerateChangeBrightness(ref images);
      }

      public static void MakeAugumentation(ref List<Image<Gray, byte>> images)
      {
         GenerateImageBlur(ref images);
         GenerateChangeBrightness(ref images);
      }
   }
}