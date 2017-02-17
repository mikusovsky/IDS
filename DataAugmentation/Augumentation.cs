using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography;
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
               ret = img.SmoothGaussian(1, 3, 34.3, 45.3);
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
            //images.Add(img.SmoothGaussian(1, 3, 34.3, 45.3));
            images.Add(img.SmoothGaussian(1, 5, 34.3, 45.3));
            images.Add(img.SmoothGaussian(1, 7, 34.3, 45.3));
            images.Add(img.SmoothGaussian(1, 9, 34.3, 45.3)); // TODO for brand recognition
            /*
            images.Add(ImageBlur(img, 7, 7));
            images.Add(ImageBlur(img, 11, 11));
            */
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

      public static Image<Gray, byte> GetPerspectiveTransform(Image<Gray, byte> image, int x, int y)
      {
         PointF[] srcs = new PointF[4];
         srcs[0] = new PointF(0, 0);
         srcs[1] = new PointF(0, Deffinitions.NORMALIZE_MASK_HEIGHT - y - 1);
         srcs[2] = new PointF(Deffinitions.NORMALIZE_MASK_WIDTH - x - 1, Deffinitions.NORMALIZE_MASK_HEIGHT - y - 1);
         srcs[3] = new PointF(Deffinitions.NORMALIZE_MASK_WIDTH - x - 1, 0);
         
         PointF[] dsts = new PointF[4];
         dsts[0] = new PointF(0, 0);
         dsts[1] = new PointF(0, Deffinitions.NORMALIZE_MASK_HEIGHT - 1);
         dsts[2] = new PointF(Deffinitions.NORMALIZE_MASK_WIDTH - 1, Deffinitions.NORMALIZE_MASK_HEIGHT - 1);
         dsts[3] = new PointF(Deffinitions.NORMALIZE_MASK_WIDTH - 1, 0);
         

         HomographyMatrix mywarpmat = CameraCalibration.GetPerspectiveTransform(srcs, dsts);
         Image<Gray, byte> newImage = image.WarpPerspective(mywarpmat, Emgu.CV.CvEnum.INTER.CV_INTER_NN, Emgu.CV.CvEnum.WARP.CV_WARP_FILL_OUTLIERS, new Gray(0));
         newImage = Utils.Resize(newImage, Deffinitions.NORMALIZE_MASK_WIDTH, Deffinitions.NORMALIZE_MASK_HEIGHT);
         return newImage;
      }
      
      public static void GeneratePersepectiveTransform(ref List<Image<Gray, byte>> images)
      {
         Image<Gray, byte>[] arrImages = images.ToArray();
         foreach (Image<Gray, byte> img in arrImages)
         {
            images.Add(GetPerspectiveTransform(img, 0, 5));
            images.Add(GetPerspectiveTransform(img, 0, 10));
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
         GeneratePersepectiveTransform(ref images);
         //GenerateChangeBrightness(ref images);
      }
   }
}