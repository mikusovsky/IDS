using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Structure;
using IDS.IDS.CarRecognition;

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

      public static Image<Bgr, byte> ExtractMask(Image<Bgr, byte> image)
      {
         /*
         Image<Gray, byte> gray1 = image.PyrDown().PyrUp();
         Image<Gray, byte> cannyGray = gray1.Canny(new Gray(120), new Gray(180));
         image = cannyGray;

         LineSegment2D[] lines = image.HoughLinesBinary(
                                 1,                  //Distance resolution in pixel-related units
                                 Math.PI / 45.0,     //Angle resolution measured in radians.
                                 50,                 //threshold
                                 100,                //min Line width
                                 1                   //gap between lines
                                 )[0];               //Get the lines from the first channel
      
         foreach (LineSegment2D line in lines)
         {
            image.Draw(line, new Gray(200), 5);
         }
         CvInvoke.cvShowImage("carPhoto", image);
         */
         List<Image<Gray, byte>> licencePlateList = new List<Image<Gray, byte>>();
         List<Image<Gray, byte>> filteredLicensePlateList = new List<Image<Gray, byte>>();
         List<MCvBox2D> boxList = new List<MCvBox2D>();
         //m_licencePlateDetector.DetectLicensePlate(image, licencePlateList, filteredLicensePlateList, boxList);
         return null;
      }

      public static Image<Gray, byte> ToGray(Image<Bgr, byte> image)
      {
         return image.Convert<Gray, Byte>();
      }

      private static Random random = new Random();
      public static string RandomString(int length)
      {
         const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
         return new string(Enumerable.Repeat(chars, length)
           .Select(s => s[random.Next(s.Length)]).ToArray());
      }
   }
}