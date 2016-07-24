using System;
using System.Collections.Generic;
using System.Drawing;

namespace IDS
{
   //trieda na parovanie prednych svetlometov vozidiel
   class PairingHeadlights
   {
      List<Headlight> _headlight;
      List<Rectangle> _vehiclesBb;
      int widthOfRoadLane;

      public PairingHeadlights(int widthOfRoadLane)
      {
         _headlight = new List<Headlight>();
         _vehiclesBb = new List<Rectangle>();
         this.widthOfRoadLane = widthOfRoadLane;
      }

      //pridanie noveho svetlometu
      public void AddHeadlight(Rectangle bb)
      {
         Point p1 = new Point();
         Point p2 = new Point();
         p1.X = bb.X;
         p1.Y = bb.Y;
         p2.X = bb.X + bb.Width;
         p2.Y = bb.Y + bb.Height;
         Headlight tmp = new Headlight(p1, p2);
         _headlight.Add(tmp);
      }

      //hladanie vyslednych parov svetlometov
      public void FindPairs()
      {
         int CH = (int)Math.Ceiling(widthOfRoadLane * 0.1);
         int CW = (int)Math.Ceiling(widthOfRoadLane * 0.1);
         int TJ = (int)Math.Ceiling(widthOfRoadLane * 0.05); //5
         int TXl = (int)Math.Ceiling(widthOfRoadLane * 0.15); //20
         int TXh = (int)Math.Ceiling(widthOfRoadLane * 0.5); //50
         int TW = (int)Math.Ceiling(widthOfRoadLane * 0.05); //5
         int TH = (int)Math.Ceiling(widthOfRoadLane * 0.05); //5
         bool foundPair;

         for (int i = 0; i < _headlight.Count - 1; i++)
         {
            if (_headlight[i].Paired)
            {
               continue;
            }

            foundPair = false;
            for (int j = i + 1; j < _headlight.Count; j++)
            {
               if (_headlight[j].Paired)
               {
                  continue;
               }

               if (!(Math.Abs(_headlight[i].P1.Y - _headlight[j].P1.Y) <= TJ))
               {
                  continue;
               }

               int value = Math.Abs(_headlight[i].P1.X - _headlight[j].P1.X);
               if (!(TXl <= value && value <= TXh))
               {
                  continue;
               }

               if (!(Math.Abs(_headlight[i].Width - _headlight[j].Width) <= TW))
               {
                  continue;
               }

               if (!(Math.Abs(_headlight[i].Height - _headlight[j].Height) <= TH))
               {
                  continue;
               }

               _AddVehicleBB(i, j);
               foundPair = true;
               _headlight[j].Paired = true;
            }
            
            if ((!foundPair) && (_headlight[i].Width <= CW) && (_headlight[i].Height <= CH))
            {
               _AddOnlyHeadlightBB(i);
            }
         }
      }

      //pridanie vozidla iba s jednym svetlometom
      private void _AddOnlyHeadlightBB(int i)
      {
         Rectangle bb = new Rectangle();
         bb.X = _headlight[i].P1.X;
         bb.Y = _headlight[i].P1.Y;
         bb.Width = _headlight[i].Width;
         bb.Height = _headlight[i].Height;
         _vehiclesBb.Add(bb);
      }

      //pridanie vozidla s 2 svetlometmi
      private void _AddVehicleBB(int i, int j)
      {
         Rectangle bb = new Rectangle();
         bb.X = Math.Min(_headlight[i].P1.X, _headlight[j].P1.X);
         bb.Y = Math.Min(_headlight[i].P1.Y, _headlight[j].P1.Y);
         bb.Width = Math.Abs(_headlight[i].P1.X - _headlight[j].P1.X) + Math.Max(_headlight[i].Width, _headlight[j].Width);
         bb.Height = Math.Max(_headlight[i].Height, _headlight[j].Height);
         _vehiclesBb.Add(bb);
      }

      //kontrola a vymazanie viacerych svetlometov toho isteho vozidla
      private void _DeleteRedundantBB()
      {
         List<Rectangle> newBB = new List<Rectangle>();
         int TX;
         int TY;
         bool deleted;

         TX = (int)Math.Ceiling(widthOfRoadLane * 0.1);
         TY = (int)Math.Ceiling(widthOfRoadLane * 0.3);

         for (int i = 0; i < _vehiclesBb.Count; i++)
         {
            deleted = false;
            for (int j = i + 1; j < _vehiclesBb.Count; j++)
            {
               int height = Math.Max(_vehiclesBb[i].Height, _vehiclesBb[j].Height);

               if ((Math.Abs(_vehiclesBb[i].X - _vehiclesBb[j].X) <= TX) && (Math.Abs(_vehiclesBb[i].Y - _vehiclesBb[j].Y) <= TY))
               {
                  deleted = true;
                  if (_vehiclesBb[i].Y > _vehiclesBb[j].Y)
                  {
                     newBB.Add(_vehiclesBb[j]);
                  }
                  else
                  {
                     newBB.Add(_vehiclesBb[i]);
                  }
               }

            }
            if (!deleted)
            {
               newBB.Add(_vehiclesBb[i]);
            }
         }

         _vehiclesBb = newBB;
      }

      //vytvorenie vozidiel pomocou sparovania svetlometov
      public List<Rectangle> _CreateVehiclesBB()
      {
         FindPairs();
         _DeleteRedundantBB();

         return _vehiclesBb;
      }

      public void _ClearList()
      {
         _headlight.Clear();
         _vehiclesBb.Clear();
      }
   }
}
