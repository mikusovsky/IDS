using System;
using System.Collections.Generic;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;

namespace IDS
{
   //trieda na trackovanie pohybujucich sa vozidiel
   class TrackingVehicles
   {
      public List<Vehicle> CurrentVehicles;
      List<Vehicle> _tempCurrentVehicles;
      List<Vehicle> _prevVehicles;
      public int VehiclesCount;
      public string ConsoleText;
      int _maxPredictDistance;

      public TrackingVehicles(int laneWidth)
      {
         CurrentVehicles = new List<Vehicle>();
         _prevVehicles = new List<Vehicle>();
         _tempCurrentVehicles = new List<Vehicle>();
         VehiclesCount = 0;
         ConsoleText = "";
         _maxPredictDistance = (int)(0.1 * laneWidth);
      }

      //pridanie noveho vozidla do zoznamu
      public void AddCurrentVehicle(Rectangle bb, Image<Bgr, Byte> frameHD, double hdRatio)
      {
         Point p1 = new Point();
         Point p2 = new Point();
         p1.X = bb.X;
         p1.Y = bb.Y;
         p2.X = bb.X + bb.Width;
         p2.Y = bb.Y + bb.Height;
         Vehicle v = new Vehicle(p1, p2, frameHD, hdRatio);
         CurrentVehicles.Add(v);
      }

      //najdenie vozidiel na predchadzajucich snimkach 
      public void FindPrevBb()
      {
         double minDistance;
         int minId;
         double predictMinDistance;
         int predictMinId;
         double distance;
         _tempCurrentVehicles.Clear();

         for (int i = 0; i < _prevVehicles.Count; i++)
         {
            minDistance = Int32.MaxValue;
            predictMinDistance = Int32.MaxValue;
            minId = -1;
            predictMinId = -1;

            for (int j = 0; j < CurrentVehicles.Count; j++)
            {
               if (CurrentVehicles[j].IsUsed)
               {
                  continue;
               }
               distance = Math.Sqrt(Math.Pow((_prevVehicles[i].P1.X - CurrentVehicles[j].P1.X), 2) +
                   Math.Pow((_prevVehicles[i].P2.Y - CurrentVehicles[j].P2.Y), 2));
               if (distance < minDistance)
               {
                  minDistance = distance;
                  minId = j;
               }

               if (_prevVehicles[i].PredictionP.Y != 0 && _prevVehicles[i].PredictionP.X != 0)
               {
                  distance = Math.Sqrt(Math.Pow((_prevVehicles[i].PredictionP.X - CurrentVehicles[j].P1.X), 2)
                      + Math.Pow((_prevVehicles[i].PredictionP.Y - CurrentVehicles[j].P2.Y), 2));
                  if (distance < predictMinDistance)
                  {
                     predictMinDistance = distance;
                     predictMinId = j;
                  }
               }
            }

            if ((predictMinDistance < _maxPredictDistance))
            {
               _CopyVehicleProperties(i, predictMinId);
            }

            else if ((minDistance < _maxPredictDistance) && (_prevVehicles[i].NumberOfFrames == 1))
            {
               _CopyVehicleProperties(i, minId);
            }
            else if (_prevVehicles[i].NumberOfMissingFrames < 3)
            {
               _CopyMissingVehicleProperties(i);
            }
         }

         foreach (Vehicle v in CurrentVehicles)
         {
            if (!v.IsUsed)
            {
               _SetNewCarProperties(v);
            }
         }

         CurrentVehicles.Clear();
         CurrentVehicles.AddRange(_tempCurrentVehicles);
      }

      //vytvorenie noveho sledovaneho vozidla
      private void _SetNewCarProperties(Vehicle v)
      {
         v.NumberOfFrames = 1;
         v.Tracked = false;
         v.Counted = false;
         v.Size = 0;
         v.SetPredictionP(0, 0);
         v.SetSecondPredictionP(0, 0);
         v.FirstPosition = v.P1;
         _tempCurrentVehicles.Add(v);
      }

      //skopirovanie a dedenie vlastnosti od vozidla na predchadyajucej snimke
      private void _CopyVehicleProperties(int prev, int current)
      {
         CurrentVehicles[current].Counted = _prevVehicles[prev].Counted;
         CurrentVehicles[current].CountBB = _prevVehicles[prev].CountBB;
         CurrentVehicles[current].CountPosition = _prevVehicles[prev].CountPosition;
         CurrentVehicles[current].Size = _prevVehicles[prev].Size;
         CurrentVehicles[current].FirstPosition = _prevVehicles[prev].FirstPosition;
         CurrentVehicles[current].NumberOfFrames = _prevVehicles[prev].NumberOfFrames + 1;
         CurrentVehicles[current].NumberOfFrameStartCountedArea = _prevVehicles[prev].NumberOfFrameStartCountedArea;
         CurrentVehicles[current].PerspectiveCountPosition = _prevVehicles[prev].PerspectiveCountPosition;
         CurrentVehicles[current].PerspectiveCountBB = _prevVehicles[prev].PerspectiveCountBB;
         CurrentVehicles[current].PerspectiveFirstPosition = _prevVehicles[prev].PerspectiveFirstPosition;
         CurrentVehicles[current].PositionOnStartCountedArea = _prevVehicles[prev].PositionOnStartCountedArea;
         int dX = CurrentVehicles[current].P1.X - _prevVehicles[prev].P1.X;
         int dY = CurrentVehicles[current].P2.Y - _prevVehicles[prev].P2.Y;
         CurrentVehicles[current].SetPredictionP(CurrentVehicles[current].P1.X + dX,
             CurrentVehicles[current].P2.Y + dY);
         CurrentVehicles[current].SetSecondPredictionP(CurrentVehicles[current].P1.X + 2 * dX,
             CurrentVehicles[current].P2.Y + 2 * dY);
         CurrentVehicles[current].Speed = _prevVehicles[prev].Speed;
         CurrentVehicles[current].Tracked = false;
         CurrentVehicles[current].Type = _prevVehicles[prev].Type;
         CurrentVehicles[current].IsUsed = true;
         _prevVehicles[prev].Tracked = true;
         _tempCurrentVehicles.Add(CurrentVehicles[current]);
      }

      //skopirovanie a dedenie vlastnosti od vozidla na predchadyajucej snimke pri chybajucej snimke
      private void _CopyMissingVehicleProperties(int prev)
      {
         _prevVehicles[prev].NumberOfFrames++;
         int dX = Math.Max(_prevVehicles[prev].PredictionP.X - _prevVehicles[prev].P1.X, 0);
         int dY = Math.Max(_prevVehicles[prev].PredictionP.Y - _prevVehicles[prev].P2.Y, 1);
         _prevVehicles[prev].SetPredictionP(_prevVehicles[prev].PredictionP.X + dX,
             _prevVehicles[prev].PredictionP.Y + dY);
         _prevVehicles[prev].SetSecondPredictionP(_prevVehicles[prev].PredictionP.X + 2 * dX,
             _prevVehicles[prev].PredictionP.Y + 2 * dY);
         _prevVehicles[prev].NumberOfMissingFrames++;
         _prevVehicles[prev].P1.X += dX;
         _prevVehicles[prev].P1.Y += dY;
         _prevVehicles[prev].P2.X += dX;
         _prevVehicles[prev].P2.Y += dY;

         _tempCurrentVehicles.Add(_prevVehicles[prev]);
      }

      //vykreslenie predpovedanych pozicii
      public Image<Bgr, Byte> DrawPrediction(Image<Bgr, Byte> frame)
      {
         foreach (Vehicle v in CurrentVehicles)
         {
            Rectangle rect = new Rectangle((int)v.P1.X, (int)v.P1.Y, (int)v.P2.X - (int)v.P1.X, (int)v.P2.Y - (int)v.P1.Y);
            frame.Draw(rect, new Bgr(Color.LightGreen), 1);
         }

         foreach (Vehicle v in _prevVehicles)
         {
            Rectangle rect = new Rectangle((int)v.P1.X, (int)v.P1.Y, (int)v.P2.X - (int)v.P1.X, (int)v.P2.Y - (int)v.P1.Y);
            frame.Draw(rect, new Bgr(Color.Red), 1);

            PointF point = new PointF((float)v.PredictionP.X, (float)v.PredictionP.Y);
            frame.Draw(new CircleF(point, 3), new Bgr(Color.Black), 2);
         }

         return frame;
      }

      //presunutie aktualneho zoznamu vozidiel do predchadyajucich 
      public void MoveListCurrentToPrev()
      {
         _prevVehicles.Clear();
         _prevVehicles.AddRange(CurrentVehicles);
         CurrentVehicles.Clear();
      }

      //nastavenie statistickych parametrov pre jednotlive vozidla
      public List<Vehicle> SetStatisticParam(Matrix<float> homogMatrix)
      {
         List<Vehicle> countedVehicle = new List<Vehicle>();
         foreach (Vehicle v in CurrentVehicles)
         {
            if ((!v.Counted) && (v.PositionOnStartCountedArea.Y == 0) && (v.P2.Y >= MainForm.START_COUNTED_AREA))
            {
               v.PositionOnStartCountedArea = v.P2;
               v.NumberOfFrameStartCountedArea = v.NumberOfFrames;
            }

            else if ((!v.Counted) && (v.NumberOfFrames >= MainForm.FRAME_NUMBER_TO_COUNT) && (v.FirstPosition.Y < v.P1.Y) &&
                 (v.P2.Y >= MainForm.END_COUNTED_AREA))
            {
               VehiclesCount++;
               v.Counted = true;
               v.SetCountBB();
               v.SetCountPosition();
               v.SetPerspectivePoints(homogMatrix);
               v.SetSize();
               v.SetSpeed();
               countedVehicle.Add(v);
            }
         }
         return countedVehicle;
      }
   }
}
