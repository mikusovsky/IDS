using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Structure;

namespace IDS.IDS.CarRecognition
{
   public static class SymetricFilter
   {
      private const int MASK_WIDHT = 300;
      private const int MASK_HEIGHT = 150;
      private static Image<Bgr, byte> m_image; 
      public static Image<Bgr, Byte> GetSymetricPart(Image<Bgr, Byte> image)
      {
         Utils.HdToLow(image, ref m_image);
         Image<Gray, Byte> grayImage = m_image.Convert<Gray, Byte>();
         Rectangle rect = _GetMostSymetricPart(grayImage);
         m_image.ROI = rect;
         //CvInvoke.cvShowImage("carPhoto", m_image);
         return image;
      }

      private static Rectangle _GetMostSymetricPart(Image<Gray, Byte> image)
      {
         Dictionary<int, int[]> dict = new Dictionary<int, int[]>();
         for (int i = 0; i + MASK_WIDHT < image.Width; i += 5)
         {
            for (int j = 0; j + MASK_HEIGHT < image.Height; j += 5)
            {
               int sum = _SumInWindow(image, i, j);
               dict[sum] = new int[] {i, j, i + MASK_WIDHT, j + MASK_HEIGHT};
            }
         }

         int min = dict.Keys.ToList().Min();
         int[] args = dict[min];
         Rectangle rect = new Rectangle(args[0], args[1], args[2], args[3]);
         return rect;
      }

      private static int _SumInWindow(Image<Gray, Byte> image, int x, int y)
      {
         int sum = 0;
         for (int i = y; i - y < MASK_HEIGHT; i++)
         {
            sum += _SumInRow(image, i, x, x + MASK_WIDHT);
         }
         return Math.Abs(sum);
      }

      private static int _SumInRow(Image<Gray, Byte> image, int row, int from, int to)
      {
         int ret = 0;
         int middle = (from + to) / 2;
         for (int i = from; i < middle && i < image.Width; i++)
         {
            ret += image.Bitmap.GetPixel(i, row).R;
         }
         for (int i = middle; i < to && i < image.Width; i++)
         {
            ret -= image.Bitmap.GetPixel(i, row).R;
         }
         return ret;
      }
   }
}