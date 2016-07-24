using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace IDS
{
    //trieda na parovanie prednych svetlometov vozidiel
    class PairingHeadlights
    {
        List<Headlight> headlight;
        List<Rectangle> vehiclesBB;
        int widthOfRoadLane;

        public PairingHeadlights(int widthOfRoadLane)
        {
            headlight = new List<Headlight>();
            vehiclesBB = new List<Rectangle>();
            this.widthOfRoadLane = widthOfRoadLane;
        }

        //pridanie noveho svetlometu
        public void addHeadlight(Rectangle bb)
        {
            Point p1 = new Point();
            Point p2 = new Point();
            p1.X = bb.X;
            p1.Y = bb.Y;
            p2.X = bb.X + bb.Width;
            p2.Y = bb.Y + bb.Height;
            Headlight tmp = new Headlight(p1, p2);
            headlight.Add(tmp);
        }

        //hladanie vyslednych parov svetlometov
        public void findPairs()
        {

            int CH = (int)Math.Ceiling(widthOfRoadLane * 0.1);
            int CW = (int)Math.Ceiling(widthOfRoadLane * 0.1);
            int TJ = (int)Math.Ceiling(widthOfRoadLane * 0.05); //5
            int TXl = (int)Math.Ceiling(widthOfRoadLane * 0.15); //20
            int TXh = (int)Math.Ceiling(widthOfRoadLane * 0.5); //50
            int TW = (int)Math.Ceiling(widthOfRoadLane * 0.05); //5
            int TH = (int)Math.Ceiling(widthOfRoadLane * 0.05); //5
            bool foundPair;

            for (int i = 0; i < headlight.Count-1; i++)
            {
                if (headlight[i].Paired)
                {
                    continue;
                }
                
                foundPair = false;
                for (int j = i + 1; j < headlight.Count; j++)
                {
                    if (headlight[j].Paired)
                    {
                        continue;
                    }
                    
                    if (!(Math.Abs(headlight[i].p1.Y - headlight[j].p1.Y) <= TJ))
                    {
                        continue;
                    }

                    int value = Math.Abs(headlight[i].p1.X - headlight[j].p1.X);
                    if (!(TXl<=value && value<=TXh))
                    {
                        continue;
                    }

                    if (!(Math.Abs(headlight[i].Width - headlight[j].Width) <= TW))
                    {
                        continue;
                    }

                    if(!(Math.Abs(headlight[i].Height - headlight[j].Height) <= TH))
                    {
                        continue;
                    }

                    addVehicleBB(i, j);
                    foundPair = true;
                    headlight[j].Paired = true;

                }

               
                if ((!foundPair) && (headlight[i].Width <= CW) && (headlight[i].Height <= CH))
                {
                    addOnlyHeadlightBB(i);
                }

                    
               
            }
        }

        //pridanie vozidla iba s jednym svetlometom
        private void addOnlyHeadlightBB(int i)
        {
            Rectangle bb = new Rectangle();
            bb.X = headlight[i].p1.X;
            bb.Y = headlight[i].p1.Y;
            bb.Width = headlight[i].Width;
            bb.Height = headlight[i].Height;
            vehiclesBB.Add(bb);
        }

        //pridanie vozidla s 2 svetlometmi
        private void addVehicleBB(int i, int j)
        {
            Rectangle bb = new Rectangle();
            bb.X = Math.Min(headlight[i].p1.X, headlight[j].p1.X);
            bb.Y = Math.Min(headlight[i].p1.Y, headlight[j].p1.Y);
            bb.Width = Math.Abs(headlight[i].p1.X - headlight[j].p1.X) + Math.Max(headlight[i].Width, headlight[j].Width);
            bb.Height = Math.Max(headlight[i].Height, headlight[j].Height);
            vehiclesBB.Add(bb);
        }

        //kontrola a vymazanie viacerych svetlometov toho isteho vozidla
        private void deleteRedundantBB()
        {
            List<Rectangle> newBB = new List<Rectangle>();
            int TX;
            int TY;
            bool deleted;

            TX = (int)Math.Ceiling(widthOfRoadLane * 0.1);
            TY = (int)Math.Ceiling(widthOfRoadLane * 0.3);

            for (int i = 0; i < vehiclesBB.Count; i++ )
            {
                deleted = false;
                for (int j = i+1; j < vehiclesBB.Count; j++)
                {
                    int height = Math.Max(vehiclesBB[i].Height, vehiclesBB[j].Height);

                    if ((Math.Abs(vehiclesBB[i].X - vehiclesBB[j].X) <= TX) && (Math.Abs(vehiclesBB[i].Y - vehiclesBB[j].Y) <= TY))
                    {
                        deleted = true;
                        if (vehiclesBB[i].Y > vehiclesBB[j].Y)
                        {
                            newBB.Add(vehiclesBB[j]);
                        }
                        else
                        {
                            newBB.Add(vehiclesBB[i]);
                        }
                    }

                }
                if (!deleted)
                {
                    newBB.Add(vehiclesBB[i]);
                }
            }

            vehiclesBB = newBB;
        }
        
        //vytvorenie vozidiel pomocou sparovania svetlometov
        public List<Rectangle> createVehiclesBB()
        {
            findPairs();
            deleteRedundantBB();

            return vehiclesBB;
        }

        public void clearList()
        {
            headlight.Clear();
            vehiclesBB.Clear();
        }

    }
}
