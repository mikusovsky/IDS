using System.Drawing;

namespace IDS
{
   //trieda na definovanie predneho svetlometu vozidla
   class Headlight
   {
      bool _paired;
      public Point P1;
      public Point P2;
      public int Height
      {
         get { return P2.Y - P1.Y; }
      }
      public int Width
      {
         get { return P2.X - P1.X; }
      }

      public Headlight(Point p1, Point p2)
      {
         P1 = p1;
         P2 = p2;
         _paired = false;
      }
      public bool Paired
      {
         get { return _paired; }
         set { _paired = value; }
      }
   }
}
