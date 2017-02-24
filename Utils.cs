using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Xml;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Flann;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using IDS.IDS.Classificator;
using IDS.IDS.DataAugmentation;
using IDS.IDS.IntervalTree;

namespace IDS.IDS
{
   public static class Utils
   {
      private static System.Windows.Forms.ProgressBar m_progressBar;

      //private static readonly LicensePlateDetector m_licencePlateDetector = new LicensePlateDetector();

      public static string CurentVideoPath { get; set; }
      public static Dictionary<CarModel, Matrix<float>> ImportanceMaps = new Dictionary<CarModel, Matrix<float>>();

      public static void HdToLow(Image<Bgr, Byte> hdFrame, ref Image<Bgr, Byte> lowFrame)
      {
         if (hdFrame == null || hdFrame.Width == 0)
         {
            return;
         }
         double hdWidth = Convert.ToDouble(hdFrame.Width);
         double hdHeight = Convert.ToDouble(hdFrame.Height);
         double ratio = hdWidth / hdHeight;
         int lowWidth = Deffinitions.LOW_FRAME_WIDTH;
         int lowHeight = Convert.ToInt32(Math.Round(Convert.ToDouble(lowWidth) / ratio));

         if (lowFrame == null)
         {
            Bitmap bitmap = new Bitmap(lowWidth, lowHeight);
            bitmap.SetResolution(bitmap.HorizontalResolution, bitmap.VerticalResolution);
            lowFrame = new Image<Bgr, byte>(bitmap);
         }
         Bitmap newBitmap = lowFrame.Bitmap;
         _Resize(hdFrame.Bitmap, ref newBitmap);
         lowFrame.Bitmap = newBitmap;
      }

      public static Image<Bgr, byte> Resize(Image<Bgr, byte> image, int newWidth, int newHeight)
      {
         Bitmap bitmap = new Bitmap(newWidth, newHeight);
         Image<Bgr, byte> ret = new Image<Bgr, byte>(bitmap);
         _Resize(image.Bitmap, ref bitmap);
         ret.Bitmap = bitmap;
         return ret;
      }

      public static Image<Gray, byte> Resize(Image<Gray, byte> image, int newWidth, int newHeight)
      {
         Bitmap bitmap = new Bitmap(newWidth, newHeight);
         Image<Gray, byte> ret = new Image<Gray, byte>(bitmap);
         _Resize(image.Bitmap, ref bitmap);
         ret.Bitmap = bitmap;
         return ret;
      }

      private static void _Resize(Bitmap image, ref Bitmap newImage)
      {
         using (Graphics gr = Graphics.FromImage(newImage))
         {
            Rectangle rc = new Rectangle(0, 0, newImage.Width, newImage.Height);
            gr.SmoothingMode = SmoothingMode.HighQuality;
            gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
            gr.DrawImage(image, rc);
         }
      }

      public static Image<Bgr, byte> ExtractMask2(Image<Bgr, byte> image)
      {
         if (image == null)
         {
            return null;
         }
         Image<Gray, byte> grayImage = ToGray(image);
         Image<Gray, float> sobel = grayImage.Sobel(0, 1, 5);

         double maxRowSum = float.MinValue;
         int shadowPos = -1;
         List<int> values = new List<int>();
         for (int i = 0; i < sobel.Rows; i++)
         {
            int actualRow = 0;
            for (int j = 0; j < sobel.Cols; j++)
            {
               actualRow += sobel[i, j].Intensity > 0 ? 1 : 0;
            }
            values.Add(actualRow);
            if (maxRowSum < actualRow)
            {
               shadowPos = i;
               maxRowSum = actualRow;
            }
         }

         Image<Gray, byte> cannyGray = grayImage.Canny(new Gray(10), new Gray(60));
         //CvInvoke.cvShowImage("cannyGray", cannyGray);
         SetStrongestEdges(cannyGray, 7);
         List<int> histogram = GetHistogramY(cannyGray);
         int windowStartPost = _GetMaxDif(histogram, 20, shadowPos);
         histogram = GetHistogramX(cannyGray);
         int leftPos = _GetMinFromLeft(histogram);
         int rigntPos = _GetMinFromRight(histogram);

         if (Deffinitions.DEBUG_MODE)
         {
            _DrawLineY(sobel, shadowPos);
            _DrawLineY(sobel, windowStartPost);
            _DrawLineX(sobel, leftPos);
            _DrawLineX(sobel, rigntPos);
            Utils.LogImage("sobel", sobel);
            Utils.LogImage("cannyGray", cannyGray);
         }
         int width = rigntPos - leftPos;
         int height = shadowPos - windowStartPost;
         Image<Bgr, byte> retImg = CropImage(image, leftPos, windowStartPost + height / 5, width, 4 * height / 5);
         Utils.LogImage("mask", retImg);
         return retImg;
      }

      public static Image<Bgr, byte> ExtractMask3(Image<Bgr, byte> image)
      {
         if (image == null)
         {
            return null;
         }
         Image<Gray, byte> grayImage = ToGray(image);
         Image<Gray, float> sobel = grayImage.Sobel(0, 1, 5);

         double maxRowSum = float.MinValue;
         int shadowPos = -1;
         List<int> values = new List<int>();
         for (int i = 0; i < sobel.Rows; i++)
         {
            int actualRow = 0;
            for (int j = 0; j < sobel.Cols; j++)
            {
               actualRow += sobel[i, j].Intensity > 0 ? 1 : 0;
            }
            values.Add(actualRow);
            if (maxRowSum < actualRow)
            {
               shadowPos = i;
               maxRowSum = actualRow;
            }
         }

         int width = Deffinitions.MASK_WIDTH_FROM_FRAME;
         int height = Deffinitions.MASK_HEIGHT_FROM_FRAME;
         int halfWidth = Deffinitions.MASK_WIDTH_FROM_FRAME / 2;
         double minDiffSum = Int32.MaxValue;
         int minX = 0;
         using (Image<Gray, byte> edges = grayImage/*grayImage.Canny(new Gray(10), new Gray(60))*/)
         {
            for (int i = 0; i + Deffinitions.MASK_WIDTH_FROM_FRAME < edges.Width; i++)
            {
               if (i == 47)
               {
                  int g = 0;
                  g++;
               }
               using (Image<Gray, byte> subFrame = CropImage(edges, i, shadowPos - height, width, height))
               using (Image<Gray, byte> left = CropImage(subFrame, 0, 0, halfWidth, height))
               using (Image<Gray, byte> right = CropImage(subFrame, halfWidth, 0, halfWidth, height).Flip(FLIP.HORIZONTAL))
               using (Image<Gray, byte> difference = left.AbsDiff(right).ThresholdBinary(new Gray(60), new Gray(255)))
               {
                  //Console.WriteLine($"{i} - {subFrame.Rows},{subFrame.Cols} ==> {left.Rows}=={right.Rows}, {left.Cols}=={right.Cols}");

                  double diffSum = 0;
                  for (int j = 0; j < difference.Rows; j++)
                  {
                     for (int k = 0; k < difference.Cols; k++)
                     {
                        diffSum += difference[j, k].Intensity;
                     }
                  }
                  if (minDiffSum > diffSum)
                  {
                     minX = i;
                     minDiffSum = diffSum;
                  }

               }
            }
         }
         Image<Bgr, byte> retImg = CropImage(image, minX, shadowPos - height, width, height);
         Utils.LogImage("mask", retImg);
         return retImg;
      }

      public static Image<Gray, byte> ExtractMask3(Image<Gray, byte> grayImage)
      {
         if (grayImage == null)
         {
            return null;
         }
         Image<Gray, float> sobel = grayImage.Sobel(0, 1, 5);

         double maxRowSum = float.MinValue;
         int shadowPos = -1;
         List<int> values = new List<int>();
         for (int i = 0; i < sobel.Rows; i++)
         {
            int actualRow = 0;
            for (int j = 0; j < sobel.Cols; j++)
            {
               actualRow += sobel[i, j].Intensity > 0 ? 1 : 0;
            }
            values.Add(actualRow);
            if (maxRowSum < actualRow)
            {
               shadowPos = i;
               maxRowSum = actualRow;
            }
         }

         int width = Deffinitions.MASK_WIDTH_FROM_FRAME;
         int height = Deffinitions.MASK_HEIGHT_FROM_FRAME;
         int halfWidth = Deffinitions.MASK_WIDTH_FROM_FRAME / 2;
         double minDiffSum = Int32.MaxValue;
         int minX = 0;
         using (Image<Gray, byte> edges = grayImage/*grayImage.Canny(new Gray(10), new Gray(60))*/)
         {
            for (int i = 0; i + Deffinitions.MASK_WIDTH_FROM_FRAME < edges.Width; i++)
            {
               if (i == 47)
               {
                  int g = 0;
                  g++;
               }
               using (Image<Gray, byte> subFrame = CropImage(edges, i, shadowPos - height, width, height))
               using (Image<Gray, byte> left = CropImage(subFrame, 0, 0, halfWidth, height))
               using (Image<Gray, byte> right = CropImage(subFrame, halfWidth, 0, halfWidth, height).Flip(FLIP.HORIZONTAL))
               using (Image<Gray, byte> difference = left.AbsDiff(right).ThresholdBinary(new Gray(60), new Gray(255)))
               {
                  //Console.WriteLine($"{i} - {subFrame.Rows},{subFrame.Cols} ==> {left.Rows}=={right.Rows}, {left.Cols}=={right.Cols}");

                  double diffSum = 0;
                  for (int j = 0; j < difference.Rows; j++)
                  {
                     for (int k = 0; k < difference.Cols; k++)
                     {
                        diffSum += difference[j, k].Intensity;
                     }
                  }
                  if (minDiffSum > diffSum)
                  {
                     minX = i;
                     minDiffSum = diffSum;
                  }

               }
            }
         }
         Image<Gray, byte> retImg = CropImage(grayImage, minX, shadowPos - height, width, height);
         //Utils.LogImage("mask", retImg);
         return retImg;
      }

      public static Image<Bgr, byte> CropImage(Image<Bgr, byte> img, int x, int y, int width, int height)
      {
         Image<Bgr, byte> roiImage = new Image<Bgr, byte>(img.Bitmap);
         /*** EXTRACT BUTTON MIDDLE PART OF IMAGE FOR EXTACTION ***/
         using (Graphics gr = Graphics.FromImage(roiImage.Bitmap))
         {
            Rectangle rc = new Rectangle(0, 0, img.Width, img.Height);
            gr.SmoothingMode = SmoothingMode.HighQuality;
            gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
            gr.DrawImage(img.Bitmap, rc);
            roiImage.ROI = new Rectangle(x, y, width, height);
         }
         return roiImage;
      }

      public static Image<Gray, byte> CropImage(Image<Gray, byte> img, int x, int y, int width, int height)
      {
         Bitmap tempBitmap = new Bitmap(img.Width, img.Height);
         Image<Bgr, Byte> roiImage = new Image<Bgr, Byte>(tempBitmap);
         /*** EXTRACT BUTTON MIDDLE PART OF IMAGE FOR EXTACTION ***/
         using (Graphics gr = Graphics.FromImage(roiImage.Bitmap))
         {
            gr.DrawImage(img.Bitmap, 0, 0);
            Rectangle rc = new Rectangle(0, 0, img.Width, img.Height);
            gr.SmoothingMode = SmoothingMode.HighQuality;
            gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
            gr.DrawImage(img.Bitmap, rc);
            roiImage.ROI = new Rectangle(x, y, width, height);
         }
         return ToGray(roiImage);
      }

      private static int _GetMinFromLeft(List<int> values)
      {
         double avarage = values.Average();
         int size = 20;
         for (int i = 0; i < values.Count - size; i++)
         {
            List<int> localValues = new List<int>();
            for (int j = 0; j < size; j++)
            {
               localValues.Add(values[i + j]);
            }
            localValues.Sort();
            if (localValues[size / 2] > avarage)
            {
               return i;
            }
         }
         return 0;
      }

      private static int _GetMinFromRight(List<int> values)
      {
         double avarage = values.Average();
         int size = 20;
         for (int i = values.Count - 1; i >= size; i--)
         {
            List<int> localValues = new List<int>();
            for (int j = size - 1; j >= 0; j--)
            {
               localValues.Add(values[i - j]);
            }
            localValues.Sort();
            if (localValues[size / 2] > avarage)
            {
               return i;
            }
         }
         return values.Count - 1;
      }

      private static void _DrawLineX(Image<Gray, float> img, int x)
      {
         for (int i = 0; i < 5; i++)
         {
            for (int j = 0; j < img.Rows && x - i >= 0 && x - i < img.Cols; j++)
            {
               img[j, x - i] = new Gray(-255);
            }
         }
      }

      private static void _DrawLineY(Image<Gray, float> img, int y)
      {
         for (int i = 0; i < 5; i++)
         {
            for (int j = 0; j < img.Cols && y - i >= 0 && y - i <= img.Rows; j++)
            {
               img[y - i, j] = new Gray(-255);
            }
         }
      }

      /// <summary>
      /// Find rectangle wich cant corespont for licence plate
      /// </summary>
      public static Rectangle GetLocalizationOfLicencePlate(Image<Bgr, byte> image)
      {
         return GetLocalizationOfLicencePlate(ToGray(image), null);
      }

      /// <summary>
      /// Find rectangle wich cant corespont for licence plate
      /// </summary>
      public static Rectangle GetLocalizationOfLicencePlate(Image<Gray, byte> image, Image<Bgr, byte> color)
      {
         image._GammaCorrect(1.5d);
         Image<Gray, byte> cannyGray = image.Canny(new Gray(150), new Gray(180));
         Contour<Point> finalContour = null;
         double maxBrightness = 0;
         using (MemStorage storage = new MemStorage())
         {
            for (Contour<Point> contours = cannyGray.FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE, Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_LIST, storage); contours != null; contours = contours.HNext)
            {
               Contour<Point> currentContour = contours.ApproxPoly(contours.Perimeter * 0.015, storage);
               Rectangle rectangle = currentContour.BoundingRectangle;
               double ratio = Convert.ToDouble(rectangle.Height) / Convert.ToDouble(rectangle.Width);
               if (currentContour.BoundingRectangle.Width > 20)
               {
                  CvInvoke.cvDrawContours(color, contours, new MCvScalar(255), new MCvScalar(255), -1, 1, Emgu.CV.CvEnum.LINE_TYPE.EIGHT_CONNECTED, new Point(0, 0));
                  color.Draw(currentContour.BoundingRectangle, new Bgr(0, 255, 0), 1);
               }
               if (/*rectangle.Width > 30 && rectangle.Width < 70 && rectangle.Height > 10 && rectangle.Height < 20
                  &&*/ratio > 0.20 && ratio < 0.5)
               {
                  /*double brightness = AverageBrightness(image, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
                  if (maxBrightness < brightness)
                  {
                     finalContour = currentContour;
                     maxBrightness = brightness;
                  }
                  */
               }
            }
         }
         if (finalContour == null)
         {
            return Rectangle.Empty;
         }
         return finalContour.BoundingRectangle;
      }

      public static Image<Bgr, byte> ExtractMask(Image<Bgr, byte> image)
      {
         int imageWidth = image.Width;
         int imageHeight = image.Height;
         int roiX = imageWidth / 4;
         int roiY = imageHeight / 2;
         int roiWidth = imageWidth / 2;
         int roiHight = imageHeight / 2;
         Bitmap bitmap = new Bitmap(imageWidth, imageHeight);
         bitmap.SetResolution(bitmap.HorizontalResolution, bitmap.VerticalResolution);
         Image<Bgr, Byte> roiImage = new Image<Bgr, byte>(bitmap);
         /*** EXTRACT BUTTON MIDDLE PART OF IMAGE FOR EXTACTION ***/
         using (Graphics gr = Graphics.FromImage(roiImage.Bitmap))
         {
            Rectangle rc = new Rectangle(0, 0, imageWidth, imageHeight);
            gr.SmoothingMode = SmoothingMode.HighQuality;
            gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
            gr.DrawImage(image.Bitmap, rc);
            roiImage.ROI = new Rectangle(roiX, roiY, roiWidth, roiHight);
         }
         //roiImage._EqualizeHist();
         roiImage._GammaCorrect(1.5d);
         //roiImage._ThresholdBinary(new Bgr(150,150,150), new Bgr(255,255,255));

         Image<Gray, byte> gray1 = ToGray(roiImage);//.PyrDown().PyrUp();
         Image<Gray, byte> cannyGray = gray1.Canny(new Gray(150), new Gray(180));

         Contour<Point> finalContour = null;
         double maxBrightness = 0;
         using (MemStorage storage = new MemStorage())
         {
            for (Contour<Point> contours = cannyGray.FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE, Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_LIST, storage); contours != null; contours = contours.HNext)
            {
               Contour<Point> currentContour = contours.ApproxPoly(contours.Perimeter * 0.015, storage);
               Rectangle rectangle = currentContour.BoundingRectangle;
               double ratio = Convert.ToDouble(rectangle.Height) / Convert.ToDouble(rectangle.Width);
               if (rectangle.Width > 30 && rectangle.Width < 70 && rectangle.Height > 10 && rectangle.Height < 20
                  && ratio > 0.20 && ratio < 0.5)
               {
                  double brightness = AverageBrightness(image, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
                  if (maxBrightness < brightness)
                  {
                     finalContour = currentContour;
                     maxBrightness = brightness;
                  }
               }
            }
         }

         if (finalContour == null)
         {
            return null;
         }

         int maskX = GetMaskMinXFromLP(finalContour.BoundingRectangle.X, finalContour.BoundingRectangle.Width);
         int maskY = roiY - GetMaskMinYFromLP(finalContour.BoundingRectangle.Y, finalContour.BoundingRectangle.Width);
         int maskWidth = GetMaskWidthFromLP(finalContour.BoundingRectangle.Width);
         int maskHeight = GetMaskHeightFromLP(finalContour.BoundingRectangle.Width);
         //Image<Bgr, Byte> imageMask = new Image<Bgr, byte>(new Bitmap(imageWidth, imageHeight));
         Image<Bgr, Byte> imageMask = new Image<Bgr, byte>(bitmap);

         /*** EXTRACT MASK ***/
         using (Graphics gr = Graphics.FromImage(imageMask.Bitmap))
         {
            Rectangle rc = new Rectangle(0, 0, imageWidth, imageHeight);
            gr.SmoothingMode = SmoothingMode.HighQuality;
            gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
            gr.DrawImage(image.Bitmap, rc);
            imageMask.ROI = new Rectangle(maskX, maskY, maskWidth, maskHeight);
         }

         Utils.LogImage("car mask", imageMask);
         return imageMask;
      }

      public static void Plot(List<int> values)
      {
         // add 5 so the bars fit properly
         int x = values.Count; // the position of the X axis
         int y = values.Max(); // the position of the Y axis
         Bitmap bmp = new Bitmap(360, 290);
         Graphics g = Graphics.FromImage(bmp);
         g.DrawLine(new Pen(Color.Red, 2), 5, 5, 5, 250);
         g.DrawLine(new Pen(Color.Red, 2), 5, 250, 300, 250);

         int i = 0;
         foreach (int val in values)
         {
            g.DrawLine(new Pen(Color.Blue, 1), i, 250, i, 250 - val);
            i++;
         }

         // let's draw a coordinate equivalent to (20,30) (20 up, 30 across)
         g.DrawString("X", new Font("Calibri", 12), new SolidBrush(Color.Black), y + 30, x - 20);

         Utils.LogImage("Plot", new Image<Rgb, byte>(bmp));
      }

      private static int _GetMaxDif(List<int> histogram, int gap, int maxY)
      {
         int maxDif = int.MinValue;
         int maxDifIndex = -1;
         for (int i = 0; i + gap < histogram.Count && i + gap < maxY - gap; i++)
         {
            int dif = histogram[i] - histogram[i + gap];
            if (maxDif < dif)
            {
               maxDif = dif;
               maxDifIndex = i;
            }
         }
         return maxDifIndex;
      }

      public static List<int> GetHistogramY(Image<Gray, byte> img, bool show = false)
      {
         List<int> values = new List<int>();
         for (int i = 0; i < img.Rows; i++)
         {
            int actualRow = 0;
            for (int j = 0; j < img.Cols; j++)
            {
               actualRow += img[i, j].Intensity > 0 ? 1 : 0;
            }
            values.Add(actualRow);
         }
         if (show)
         {
            Plot(values);
         }
         return values;
      }

      public static List<int> GetHistogramX(Image<Gray, byte> img, bool show = false)
      {
         List<int> values = new List<int>();
         for (int i = 0; i < img.Cols; i++)
         {
            int actualRow = 0;
            for (int j = 0; j < img.Rows; j++)
            {
               actualRow += img[j, i].Intensity > 0 ? 1 : 0;
            }
            values.Add(actualRow);
         }
         if (show)
         {
            Plot(values);
         }
         return values;
      }

      public static void SetStrongestEdges(Image<Gray, byte> image, int width)
      {
         bool[][] toSet = new bool[image.Height][];
         for (int i = 0; i < toSet.Length; i++)
         {
            toSet[i] = new bool[image.Width];
         }

         for (int i = 0; i < image.Height; i++)
         {
            for (int j = 0; j < image.Width; j++)
            {
               if (image[i, j].Intensity > 0)
               {
                  for (int k = -width / 2; k < width / 2 && k + i > 0 && k + i < image.Height; k++)
                  {
                     toSet[i + k][j] = true;
                  }
               }
            }
         }

         for (int i = 0; i < toSet.Length; i++)
         {
            for (int j = 0; j < toSet[i].Length; j++)
            {
               if (toSet[i][j])
               {
                  image.Data[i, j, 0] = 255;
               }
            }
         }
      }

      public static double AverageBrightness(Image<Bgr, byte> image, int x, int y, int width, int height)
      {
         double ret = 0;
         double count = 0;
         for (int i = 0; i < width; i++)
         {
            for (int j = 0; j < height; j++)
            {
               ret += (image.Data[i, j, 0] + image.Data[i, j, 1] + image.Data[i, j, 2]) / 3;
               count += 1;
            }
         }
         return ret / count;
      }


      public static double AverageBrightness(Image<Gray, byte> image, int x, int y, int width, int height)
      {
         double ret = 0;
         double count = 0;
         for (int i = 0; i < width; i++)
         {
            for (int j = 0; j < height; j++)
            {
               count += image.Data[i, j, 0];
            }
         }
         return ret / count;
      }

      public static Image<Gray, byte> ToGray(Image<Bgr, byte> image)
      {
         return image.Convert<Gray, Byte>();
      }

      private static Random Random = new Random();

      public static string RandomString(int length)
      {
         const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
         return new string(Enumerable.Repeat(chars, length)
           .Select(s => s[Random.Next(s.Length)]).ToArray());
      }

      public static int GetMaskWidthFromLP(int width)
      {
         double dwidth = Convert.ToDouble(width);
         return Convert.ToInt32(GetMaskWidthFromLP(dwidth));
      }

      public static double GetMaskWidthFromLP(double width)
      {
         return width * Deffinitions.MASK_WIDTH_FACTOR;
      }

      public static int GetMaskHeightFromLP(int width)
      {
         double dwidth = Convert.ToDouble(width);
         return Convert.ToInt32(GetMaskHeightFromLP(dwidth));
      }

      public static double GetMaskHeightFromLP(double width)
      {
         return width * Deffinitions.MASK_HEIGHT_FACTOR;
      }

      public static int GetMaskMinXFromLP(int x, int width)
      {
         double dx = Convert.ToDouble(x);
         double dwidth = Convert.ToDouble(width);
         return Convert.ToInt32(GetMaskMinXFromLP(dx, dwidth));
      }

      public static double GetMaskMinXFromLP(double x, double width)
      {
         return (2 * x + width - Deffinitions.MASK_WIDTH_FACTOR) / 2;
      }

      public static int GetMaskMinYFromLP(int y, int width)
      {
         double dy = Convert.ToDouble(y);
         double dwidth = Convert.ToDouble(width);
         return Convert.ToInt32(GetMaskMinYFromLP(dy, dwidth));
      }

      public static double GetMaskMinYFromLP(double y, double width)
      {
         return (2 * y - width + Deffinitions.MASK_WIDTH_FACTOR) / 2;
      }

      public static Image<Gray, byte> ExtractEdges(Image<Gray, byte> img)
      {
         //img._EqualizeHist();
         //img._GammaCorrect(1.5d);
         //roiImage._ThresholdBinary(new Bgr(150,150,150), new Bgr(255,255,255));
         Image<Gray, byte> edges = img.Canny(new Gray(150), new Gray(180));
         Utils.LogImage("edges", edges);
         return edges;
      }

      public static void SetProgressBar(System.Windows.Forms.ProgressBar progressBar)
      {
         m_progressBar = progressBar;
      }

      public static void ProgressBarShow(int maximum, int step = 1)
      {
         m_progressBar.Value = 0;
         m_progressBar.Maximum = maximum;
         m_progressBar.Step = step;
         m_progressBar.Visible = true;
      }

      public static void ProgressBarHide()
      {
         m_progressBar.Visible = false;
      }

      public static void ProgressBarIncrement()
      {
         m_progressBar.PerformStep();
      }

      public static List<CarModel> GetAllCarModels(Enums.DbType type = Enums.DbType.Training)
      {
         string configPath;
         switch (type)
         {
            case Enums.DbType.Training:
               configPath = Deffinitions.TRAINING_DB_CONFIG_PATH;
               break;

            case Enums.DbType.TrainingNormalized:
               configPath = Deffinitions.TRAINING_DB_NORMALIZED_CONFIG_PATH;
               break;

            case Enums.DbType.Testing:
               configPath = Deffinitions.TESTING_DB_CONFIG_PATH;
               break;

            case Enums.DbType.TestingBrand:
               configPath = Deffinitions.TESTING_DB_BRAND_CONFIG_PATH;
               break;

            case Enums.DbType.TestingMask:
               configPath = Deffinitions.TESTING_MASK_DB_CONFIG_PATH;
               break;

            case Enums.DbType.Subset1:
               configPath = Deffinitions.TRAINING_DB_NORMALIZED_CONFIG_PATH_PODSKUPINA1;
               break;

            case Enums.DbType.Subset2:
               configPath = Deffinitions.TRAINING_DB_NORMALIZED_CONFIG_PATH_PODSKUPINA2;
               break;

            case Enums.DbType.TrainingBrand:
               configPath = Deffinitions.TRAINING_DB_BRAND_CONFIG_PATH;
               break;

            case Enums.DbType.TrainingAudi:
               configPath = Deffinitions.TRAINING_DB_AUDI_CONFIG_PATH;
               break;

            case Enums.DbType.TrainingBMW:
               configPath = Deffinitions.TRAINING_DB_BMW_CONFIG_PATH;
               break;

            case Enums.DbType.TrainingSkoda:
               configPath = Deffinitions.TRAINING_DB_SKODA_CONFIG_PATH;
               break;

            case Enums.DbType.TrainingVolkswagen:
               configPath = Deffinitions.TRAINING_DB_VOLKSWAGEN_CONFIG_PATH;
               break;
               
            case Enums.DbType.TrainingDbForBrand:
               configPath = Deffinitions.TESTING_DB_FOR_BRAND_CONFIG_PATH;
               break;

            case Enums.DbType.TrainingDbForBrandNormalized:
               configPath = Deffinitions.TESTING_DB_FOR_BRAND_NORMALIZED_CONFIG_PATH;
               break;

            case Enums.DbType.TestingBrandMask:
               configPath = Deffinitions.TESTING_DB_BRAND_MASK_CONFIG_PATH;
               break;

            default:
               configPath = Deffinitions.TRAINING_DB_CONFIG_PATH;
               break;
         }
         List<CarModel> ret = _GetCarModelsFromConfig(configPath);
         return ret;
      }

      private static List<CarModel> _GetCarModelsFromConfig(string dbConfigPath)
      {
         List<CarModel> carModels = new List<CarModel>();
         XmlDocument config = Cache.GetXMLDocument(dbConfigPath);
         XmlNode body = config.SelectSingleNode("/body");
         XmlNodeList makers = body.SelectNodes("maker");
         foreach (XmlNode maker in makers)
         {
            string makerName = maker.Attributes.GetNamedItem("name").Value;
            XmlNodeList models = maker.SelectNodes("model");
            foreach (XmlNode model in models)
            {
               string modelName = model.Attributes.GetNamedItem("name").Value;
               XmlNodeList generations = model.SelectNodes("generation");
               foreach (XmlNode generation in generations)
               {
                  string generationName = generation.Attributes.GetNamedItem("name").Value;
                  int from = Int32.Parse(generation.SelectSingleNode("from").InnerText);
                  int to = Int32.Parse(generation.SelectSingleNode("to").InnerText);
                  string path = generation.SelectSingleNode("path").InnerText;
                  carModels.Add(new CarModel(makerName, modelName, generationName, from, to, path));
               }
            }
         }
         return carModels;
      }

      public static Matrix<float> GetModelAvarageMap(CarModel carModel)
      {
         return Cache.GetModelAvarageMap(carModel);
      }
      public static void AdaptiveBrightnes(Image<Gray, byte> image)
      {
         CvInvoke.cvAdaptiveThreshold(image, image, 255,
            Emgu.CV.CvEnum.ADAPTIVE_THRESHOLD_TYPE.CV_ADAPTIVE_THRESH_MEAN_C,
            Emgu.CV.CvEnum.THRESH.CV_THRESH_BINARY_INV, 5, 5);
      }

      public static Matrix<float> CreateImportanceMap()
      {
         List<CarModel> carModels = GetAllCarModels(Enums.DbType.TrainingNormalized);
         double minValue, maxValue;
         Point minLoc, maxLoc;
         int rows = Deffinitions.NORMALIZE_MASK_WIDTH;
         int cols = Deffinitions.NORMALIZE_MASK_HEIGHT;
         Matrix<float> importanceMap = new Matrix<float>(rows, cols);
         ProgressBarShow(carModels.Count);
         foreach (CarModel carModel in carModels)
         {
            Matrix<float> map = GetModelAvarageMap(carModel);
            importanceMap += map;
            ProgressBarIncrement();
         }
         importanceMap.MinMax(out minValue, out maxValue, out minLoc, out maxLoc);
         importanceMap *= 255 / maxValue;

         for (int i = 75; i < 100; i++)
         {
            for (int j = 40; j < 90; j++)
            {
               importanceMap[i, j] = 0;
            }
         }

         for (int i = 0; i < 30; i++)
         {
            for (int j = 30 - i; j >= 0; j--)
            {
               importanceMap[127 - i, j] = 0;
            }
         }

         for (int i = 0; i < 30; i++)
         {
            for (int j = 30 - i; j >= 0; j--)
            {
               importanceMap[127 - i, 127 - j] = 0;
            }
         }

         for (int i = 0; i < rows; i++)
         {
            for (int j = 0; j < cols; j++)
            {
               if (importanceMap[i, j] < 30)
               {
                  importanceMap[i, j] = 0;
               }
            }
         }

         using (Image<Gray, byte> img = MapToImage(importanceMap))
         {
            ConvolutionKernelF kernelF = new ConvolutionKernelF(
               new float[,] {
               {0, 1, 0},
               {1, -4, 1},
               {0, 1, 0}}
            );
            img.Convolution(kernelF);
            importanceMap = ImageToMap(img);
         }

         LogImage("ImportanceMap", MapToImage(importanceMap));
         ProgressBarHide();
         return importanceMap;
      }

      public static Matrix<float> ImageToMap(Image<Gray, byte> img)
      {
         Matrix<float> map = new Matrix<float>(img.Rows, img.Cols);
         for (int i = 0; i < img.Rows; i++)
         {
            for (int j = 0; j < img.Cols; j++)
            {
               map[i, j] = (float)img[i, j].Intensity;
            }
         }
         return map;
      }

      public static Image<Gray, byte> MapToImage(Matrix<float> map)
      {
         Image<Gray, byte> image = new Image<Gray, byte>(new byte[map.Rows, map.Cols, 1]);
         for (int i = 0; i < map.Rows; i++)
         {
            for (int j = 0; j < map.Cols; j++)
            {
               image[i, j] = new Gray(Convert.ToInt32(map[i, j]));
            }
         }
         return image;
      }

      public static void LogImage(string text, IntPtr image)
      {
         if (Deffinitions.DEBUG_MODE)
         {
            CvInvoke.cvShowImage(text, image);
         }
      }

      public static void NormalizeDb(Enums.DbType dbType)
      {
         Console.WriteLine("Normalizing");
         Stopwatch watch = Stopwatch.StartNew();
         List<CarModel> carModels = Utils.GetAllCarModels(dbType);
         int imageId = 1;
         Utils.ProgressBarShow(carModels.Count);
         for (int i = 0; i < carModels.Count; i++)
         {
            CarModel carModel = carModels[i];
            List<string> imagesPath = carModel.ImagesPath;
            for (int j = 0; j < imagesPath.Count; j++)
            {
               string imagePath = imagesPath[j];
               using (Image<Gray, byte> img = new Image<Gray, byte>(imagePath))
               {
                  string normalizedPath = Path.GetDirectoryName(imagePath)?.Replace("TrainingDbForBrand", "TrainingDbForBrandNormalized");
                  string fileExtension = Path.GetExtension(imagePath);
                  ImageFormat imageFormat = GetImageFormatFromFileExtension(fileExtension);

                  List<Image<Gray, byte>> augumentedImages = new List<Image<Gray, byte>>();
                  Image<Gray, byte> normalizedImage = Resize(img, Deffinitions.NORMALIZE_MASK_WIDTH, Deffinitions.NORMALIZE_MASK_HEIGHT);
                  augumentedImages.Add(normalizedImage);
                  Augumentation.MakeAugumentation(ref augumentedImages);

                  foreach (Image<Gray, byte> augumentedImage in augumentedImages)
                  {
                     string imageNameNormalized = $"{imageId++}_Normalized{fileExtension}";

                     string normalizedImagePath = $"{normalizedPath}\\{imageNameNormalized}";

                     if (!Directory.Exists(normalizedPath))
                     {
                        Directory.CreateDirectory(normalizedPath);
                     }
                     SaveImage(augumentedImage, normalizedImagePath, imageFormat);
                  }
               }
            }
            ProgressBarIncrement();
         }
         ProgressBarHide();
         watch.Stop();
         Console.WriteLine($"Normalized finished - {watch.ElapsedMilliseconds}ms");
      }

      public static void CreateBrandMaskDb()
      {
         Console.WriteLine("Create brand db");
         Stopwatch watch = Stopwatch.StartNew();
         List<CarModel> carModels = Utils.GetAllCarModels(Enums.DbType.TrainingDbForBrandNormalized);
         string dbPath = "D:\\Skola\\UK\\DiplomovaPraca\\PokracovaniePoPredchodcovi\\Database\\TrainingDbBrand";
         var groups = carModels.GroupBy(o => o.Maker);
         int imageId = 1;
         Utils.ProgressBarShow(carModels.Count);
         foreach (var maker in groups)
         {
            foreach (CarModel model in maker)
            {
               string modelDbPath = $"{dbPath}\\{model.Maker}\\1\\1";
               foreach (string imageSrc in model.ImagesPath)
               {
                  using (Image<Gray, byte> img = new Image<Gray, byte>(imageSrc))
                  using (Image<Gray, byte> roi1 = CropImage(img, 30, 15, 70, 55))
                  using (Image<Gray, byte> roi2 = CropImage(img, 30, 20, 70, 55))
                  {
                     if (!Directory.Exists(modelDbPath))
                     {
                        Directory.CreateDirectory(modelDbPath);
                     }
                     roi1.Bitmap.Save($"{modelDbPath}\\{imageId++}.jpg", ImageFormat.Jpeg);
                     roi2.Bitmap.Save($"{modelDbPath}\\{imageId++}.jpg", ImageFormat.Jpeg);
                  }
               }
               ProgressBarIncrement();
            }
         }
         ProgressBarHide();
         watch.Stop();
         Console.WriteLine($"Create brand db - {watch.ElapsedMilliseconds}ms");
      }

      public static void SaveImage(Image<Gray, byte> img, string imgName, ImageFormat format)
      {
         if (img == null || img.Bitmap == null)
         {
            return;
         }
         string directory = Path.GetDirectoryName(imgName);
         if (!Directory.Exists(directory))
         {
            Directory.CreateDirectory(directory);
         }
         img.Bitmap.Save(imgName, format);
      }

      public static Enums.DbType GetDbTypeForMakerString(string strMaker)
      {
         string lowMaker = strMaker.ToUpper();
         switch (lowMaker)
         {
            case "AUDI":
               return Enums.DbType.TrainingAudi;

            case "BMW":
               return Enums.DbType.TrainingBMW;

            case "SKODA":
               return Enums.DbType.TrainingSkoda;

            default:
               return Enums.DbType.TrainingVolkswagen;
         }
      }

      public static ImageFormat GetImageFormatFromFileExtension(string extension)
      {
         switch (extension.ToLower())
         {
            case @".bmp":
               return ImageFormat.Bmp;

            case @".gif":
               return ImageFormat.Gif;

            case @".ico":
               return ImageFormat.Icon;

            case @".jpg":
            case @".jpeg":
               return ImageFormat.Jpeg;

            case @".png":
               return ImageFormat.Png;

            case @".tif":
            case @".tiff":
               return ImageFormat.Tiff;

            case @".wmf":
               return ImageFormat.Wmf;

            default:
               throw new NotImplementedException();
         }
      }
   }
}
