using System;
using System.Collections.Generic;
using System.Drawing;

namespace IDS.IDS
{
   public class Range
   {
      public double MinX { get; private set; } = double.MaxValue;
      public double MinY { get; private set; } = double.MaxValue;
      public double MaxX { get; private set; } = double.MinValue;
      public double MaxY { get; private set; } = double.MinValue;

      public int MinXInt => Convert.ToInt32(Math.Round(MinX));
      public int MinYInt => Convert.ToInt32(Math.Round(MinY));
      public int MaxXInt => Convert.ToInt32(Math.Round(MaxX));
      public int MaxYInt => Convert.ToInt32(Math.Round(MaxY));

      public Range() { }

      public Range(List<Point> points)
      {
         foreach (Point p in points)
         {
            AddToRange(p);
         }
      }

      public void AddToRange(List<Point> points)
      {
         foreach (Point p in points)
         {
            AddToRange(p);
         }
      }

      public void AddToRange(Point p)
      {
         MinX = MinX > p.X ? p.X : MinX;
         MinY = MinY > p.Y ? p.Y : MinY;

         MaxX = MaxX < p.X ? p.X : MaxX;
         MaxY = MaxY < p.Y ? p.Y : MaxY;
      }

      public void Clear()
      {
         MinX = double.MaxValue;
         MinY = double.MaxValue;
         MaxX = double.MinValue;
         MaxY = double.MinValue;
      }
   }
}