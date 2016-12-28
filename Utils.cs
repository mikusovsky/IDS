using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Structure;

namespace IDS.IDS
{
   public static class Utils
   {
      //private static readonly LicensePlateDetector m_licencePlateDetector = new LicensePlateDetector();

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
         _Resize(hdFrame.Bitmap, lowFrame.Bitmap);
      }

      private static Bitmap _Resize(Bitmap image, Bitmap newImage)
      {
         using (Graphics gr = Graphics.FromImage(newImage))
         {
            Rectangle rc = new Rectangle(0, 0, newImage.Width, newImage.Height);
            gr.SmoothingMode = SmoothingMode.HighQuality;
            gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
            gr.DrawImage(image, rc);
         }
         return newImage;
      }

      public static Image<Bgr, byte> ExtractMask2(Image<Bgr, byte> image)
      {
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
            CvInvoke.cvShowImage("sobel", sobel);
            CvInvoke.cvShowImage("cannyGray", cannyGray);
         }
         int width = rigntPos - leftPos;
         int height = shadowPos - windowStartPost;
         Image<Bgr, byte> retImg = Corp(image, leftPos, windowStartPost + height / 5, width, 4 * height / 5);
         CvInvoke.cvShowImage("mask", retImg);
         return null;
      }

      public static Image<Bgr, byte> Corp(Image<Bgr, byte> img, int x, int y, int width, int height)
      {
         Image<Bgr, Byte> roiImage = new Image<Bgr, byte>(img.Bitmap);
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

         CvInvoke.cvShowImage("car mask", imageMask);
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

         CvInvoke.cvShowImage("Plot", new Image<Rgb, byte>(bmp));
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
               Color color = image.Bitmap.GetPixel(i + x, j + y);
               ret += (color.R + color.G + color.B) / 3;
               count += 1;
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
   }
}