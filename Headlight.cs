using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace IDS
{
    //trieda na definovanie predneho svetlometu vozidla
    class Headlight
    {
        bool paired;
        public Point p1;
        public Point p2;
        public int Height 
        {
            get { return p2.Y - p1.Y; }
        }
        public int Width  
        {
            get { return p2.X - p1.X; }
        }

        public Headlight(Point p1, Point p2)
        {
            this.p1 = p1;
            this.p2 = p2;
            paired = false;
        }
        public bool Paired
        {
            get { return paired; }
            set { paired = value; }
        }
    }
}
