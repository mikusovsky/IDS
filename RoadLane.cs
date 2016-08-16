using System.Collections.Generic;
using System.Drawing;

namespace IDS
{
   //trieda na definovanie jazdneho pruhu
   public class RoadLane
   {
      List<Point> _lanePoints;
      int _numberOfPersonalCars;
      int _numberOfTrucks;
      int _numberOfCars;

      public RoadLane(Point lt, Point rt, Point lb, Point rb)
      {
         _lanePoints = new List<Point>();
         _lanePoints.Add(lt);
         _lanePoints.Add(rt);
         _lanePoints.Add(rb);
         _lanePoints.Add(lb);

         _numberOfPersonalCars = 0;
         _numberOfTrucks = 0;
         _numberOfCars = 0;
      }

      public int NumberOfPersonalCars
      {
         get { return _numberOfPersonalCars; }
         set { _numberOfPersonalCars = value; }
      }

      public int NumberOfTrucks
      {
         get { return _numberOfTrucks; }
         set { _numberOfTrucks = value; }
      }

      public int NumberOfCars
      {
         get { return _numberOfPersonalCars + _numberOfTrucks; }
      }

      public List<Point> LanePoints
      {
         get { return _lanePoints; }
      }

      public int MaxWidth
      {
         get { return _lanePoints[2].X - _lanePoints[3].X; }
      }
   }
}
