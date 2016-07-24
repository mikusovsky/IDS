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
        public List<Vehicle> currentVehicles;
        List<Vehicle> tempCurrentVehicles;
        List<Vehicle> prevVehicles;
        public int vehiclesCount;
        public string consoleText;
        int maxPredictDistance;

        public TrackingVehicles(int laneWidth)
        {
            currentVehicles = new List<Vehicle>();
            prevVehicles = new List<Vehicle>();
            tempCurrentVehicles = new List<Vehicle>();
            vehiclesCount = 0;
            consoleText = "";
            maxPredictDistance = (int)(0.1 * laneWidth);
        }

        //pridanie noveho vozidla do zoznamu
        public void addCurrentVehicle(Rectangle bb)
        {
            Point p1 = new Point();
            Point p2 = new Point();
            p1.X = bb.X;
            p1.Y = bb.Y;
            p2.X = bb.X + bb.Width;
            p2.Y = bb.Y + bb.Height;
            Vehicle tmp = new Vehicle(p1, p2);
            currentVehicles.Add(tmp);
        }

        //najdenie vozidiel na predchadzajucich snimkach 
        public void findPrevBB()
        {
            double minDistance;
            int minId;
            double predictMinDistance;
            int predictMinId;
            double distance;
            tempCurrentVehicles.Clear();

            for (int i = 0; i < prevVehicles.Count; i++)
            {
                minDistance = Int32.MaxValue;
                predictMinDistance = Int32.MaxValue;
                minId = -1;
                predictMinId = -1;

                for (int j = 0; j < currentVehicles.Count; j++)
                {
                    if (currentVehicles[j].isUsed)
                    {
                        continue;
                    }
                    distance = Math.Sqrt(Math.Pow((prevVehicles[i].p1.X - currentVehicles[j].p1.X), 2) +
                        Math.Pow((prevVehicles[i].p2.Y - currentVehicles[j].p2.Y), 2));
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        minId = j;
                    }

                    if (prevVehicles[i].predictionP.Y != 0 && prevVehicles[i].predictionP.X != 0)
                    {
                        distance = Math.Sqrt(Math.Pow((prevVehicles[i].predictionP.X - currentVehicles[j].p1.X), 2)
                            + Math.Pow((prevVehicles[i].predictionP.Y - currentVehicles[j].p2.Y), 2));
                        if (distance < predictMinDistance)
                        {
                            predictMinDistance = distance;
                            predictMinId = j;
                        }
                    }
                }

                if ((predictMinDistance < maxPredictDistance))
                {
                    copyVehicleProperties(i, predictMinId);
                }

                else if ((minDistance < maxPredictDistance) && (prevVehicles[i].numberOfFrames == 1))
                {
                    copyVehicleProperties(i, minId);
                }
                else if (prevVehicles[i].NumberOfMissingFrames < 3)
                {
                    copyMissingVehicleProperties(i);
                }
            }

            foreach (Vehicle v in currentVehicles)
            {
                if (!v.isUsed)
                {
                    setNewCarProperties(v);
                }
            }

            currentVehicles.Clear();
            currentVehicles.AddRange(tempCurrentVehicles);
        }

        //vytvorenie noveho sledovaneho vozidla
        private void setNewCarProperties(Vehicle v)
        {
            v.numberOfFrames = 1;
            v.tracked = false;
            v.counted = false;
            v.size = 0;
            v.setPredictionP(0, 0);
            v.setSecondPredictionP(0, 0);
            v.firstPosition = v.p1;
            tempCurrentVehicles.Add(v);
        }

        //skopirovanie a dedenie vlastnosti od vozidla na predchadyajucej snimke
        private void copyVehicleProperties(int prev, int current)
        {
            currentVehicles[current].counted = prevVehicles[prev].counted;
            currentVehicles[current].countBB = prevVehicles[prev].countBB;
            currentVehicles[current].countPosition = prevVehicles[prev].countPosition;
            currentVehicles[current].size = prevVehicles[prev].size;
            currentVehicles[current].firstPosition = prevVehicles[prev].firstPosition;
            currentVehicles[current].numberOfFrames = prevVehicles[prev].numberOfFrames + 1;
            currentVehicles[current].NumberOfFrameStartCountedArea = prevVehicles[prev].NumberOfFrameStartCountedArea;
            currentVehicles[current].perspectiveCountPosition = prevVehicles[prev].perspectiveCountPosition;
            currentVehicles[current].perspectiveCountBB = prevVehicles[prev].perspectiveCountBB;
            currentVehicles[current].perspectiveFirstPosition = prevVehicles[prev].perspectiveFirstPosition;
            currentVehicles[current].PositionOnStartCountedArea = prevVehicles[prev].PositionOnStartCountedArea;
            int dX = currentVehicles[current].p1.X - prevVehicles[prev].p1.X;
            int dY = currentVehicles[current].p2.Y - prevVehicles[prev].p2.Y;
            currentVehicles[current].setPredictionP(currentVehicles[current].p1.X + dX,
                currentVehicles[current].p2.Y + dY);
            currentVehicles[current].setSecondPredictionP(currentVehicles[current].p1.X + 2 * dX,
                currentVehicles[current].p2.Y + 2 * dY);
            currentVehicles[current].speed = prevVehicles[prev].speed;
            currentVehicles[current].tracked = false;
            currentVehicles[current].type = prevVehicles[prev].type;
            currentVehicles[current].isUsed = true;
            prevVehicles[prev].tracked = true;
            tempCurrentVehicles.Add(currentVehicles[current]);
        }

        //skopirovanie a dedenie vlastnosti od vozidla na predchadyajucej snimke pri chybajucej snimke
        private void copyMissingVehicleProperties(int prev)
        {
            prevVehicles[prev].numberOfFrames++;
            int dX = Math.Max(prevVehicles[prev].predictionP.X - prevVehicles[prev].p1.X, 0);
            int dY = Math.Max(prevVehicles[prev].predictionP.Y - prevVehicles[prev].p2.Y, 1);
            prevVehicles[prev].setPredictionP(prevVehicles[prev].predictionP.X + dX,
                prevVehicles[prev].predictionP.Y + dY);
            prevVehicles[prev].setSecondPredictionP(prevVehicles[prev].predictionP.X + 2 * dX,
                prevVehicles[prev].predictionP.Y + 2 * dY);
            prevVehicles[prev].NumberOfMissingFrames++;
            prevVehicles[prev].p1.X += dX;
            prevVehicles[prev].p1.Y += dY;
            prevVehicles[prev].p2.X += dX;
            prevVehicles[prev].p2.Y += dY;

            tempCurrentVehicles.Add(prevVehicles[prev]);
        }

        //vykreslenie predpovedanych pozicii
        public Image<Bgr, Byte> drawPrediction(Image<Bgr, Byte> frame)
        {
            foreach (Vehicle v in currentVehicles)
            {
                Rectangle rect = new Rectangle((int)v.p1.X, (int)v.p1.Y, (int)v.p2.X - (int)v.p1.X, (int)v.p2.Y - (int)v.p1.Y);
                frame.Draw(rect, new Bgr(Color.LightGreen), 1);
            }

            foreach (Vehicle v in prevVehicles)
            {
                Rectangle rect = new Rectangle((int)v.p1.X, (int)v.p1.Y, (int)v.p2.X - (int)v.p1.X, (int)v.p2.Y - (int)v.p1.Y);
                frame.Draw(rect, new Bgr(Color.Red), 1);

                PointF point = new PointF((float)v.predictionP.X, (float)v.predictionP.Y);
                frame.Draw(new CircleF(point, 3), new Bgr(Color.Black), 2);
            }

            return frame;
        }

        //presunutie aktualneho zoznamu vozidiel do predchadyajucich 
        public void moveListCurrentToPrev()
        {
            prevVehicles.Clear();
            prevVehicles.AddRange(currentVehicles);
            currentVehicles.Clear();
        }

        //nastavenie statistickych parametrov pre jednotlive vozidla
        public List<Vehicle> setStatisticParam(Matrix<float> homogMatrix)
        {
            List<Vehicle> countedVehicle = new List<Vehicle>();
            foreach (Vehicle v in currentVehicles)
            {
                if ((!v.counted) && (v.PositionOnStartCountedArea.Y == 0) && (v.p2.Y >= MainForm.START_COUNTED_AREA))
                {
                    v.PositionOnStartCountedArea = v.p2;
                    v.NumberOfFrameStartCountedArea = v.numberOfFrames;
                }

                else if ((!v.counted) && (v.numberOfFrames >= MainForm.FRAME_NUMBER_TO_COUNT) && (v.firstPosition.Y < v.p1.Y) &&
                     (v.p2.Y >= MainForm.END_COUNTED_AREA))
                {
                    vehiclesCount++;
                    v.counted = true;
                    v.setCountBB();
                    v.setCountPosition();
                    v.setPerspectivePoints(homogMatrix);
                    v.setSize();
                    v.setSpeed();
                    countedVehicle.Add(v);
                }
            }
            return countedVehicle;
        }
    }
}
