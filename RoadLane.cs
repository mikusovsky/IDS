using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu.CV.CvEnum;

namespace IDS
{
    //trieda na definovanie jazdneho pruhu
    public class RoadLane
    {
        List<Point> lanePoints;
                
        int numberOfPrivateCars;
        int numberOfTrucks;
        int numberOfCars;
       
        public RoadLane(Point lt, Point rt, Point lb, Point rb)
        {
            this.lanePoints = new List<Point>();
            this.lanePoints.Add(lt);
            this.lanePoints.Add(rt);
            this.lanePoints.Add(rb);
            this.lanePoints.Add(lb);
            
            numberOfPrivateCars = 0;
            numberOfTrucks = 0;
            numberOfCars = 0;

        }

        public int NumberOfPrivateCars
        {
            get { return numberOfPrivateCars; }
            set { this.numberOfPrivateCars = value; }
        }

        public int NumberOfTrucks
        {
            get { return numberOfTrucks; }
            set { this.numberOfTrucks = value; }
        }

        public int NumberOfCars
        {
            get { return this.numberOfPrivateCars + this.numberOfTrucks; }
        }

        public List<Point> LanePoints
        {
            get { return this.lanePoints; }
        }

        public int MaxWidth
        {
            get { return lanePoints[2].X - lanePoints[3].X; }
        }

    }
}
