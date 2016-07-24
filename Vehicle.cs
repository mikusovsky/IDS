using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu.CV.CvEnum;
using System.Drawing;

namespace IDS
{
    class Vehicle
    {
        public Point p1;
        public Point p2;
        public bool counted;
        public double size;
        public Point predictionP;
        public Point secondPredictionP;
        public int numberOfFrames;
        public bool tracked;
        public Point firstPosition;
        public Point perspectiveStartCountedAreaPoint;
        Point positionOnStartCountedArea;
        public Point perspectiveFirstPosition;
        public Point countPosition;
        public Point perspectiveCountPosition;
        public double speed;
        public int type;
        public Rectangle countBB;
        public Rectangle perspectiveCountBB;
        int numberOfMissingFrames;
        bool used;
        int numberOfFrameStartCountedArea;
         

        public Vehicle(Point p1, Point p2)
        {
            this.p1 = p1;
            this.p2 = p2;
            counted = false;
            size = 0;
            predictionP = new Point();
            secondPredictionP = new Point();
            firstPosition = new Point();
            positionOnStartCountedArea = new Point();
            countPosition = new Point();
            numberOfFrames = 0;
            tracked = false;
            speed = 0;
            type = 0;
            numberOfMissingFrames = 0;
            used = false;
            numberOfFrameStartCountedArea = 0;
            
        }

        public void setPredictionP(int x, int y)
        {
            predictionP.X = x;
            predictionP.Y = y;
        }

        public void setSecondPredictionP(int x, int y)
        {
            secondPredictionP.X = x;
            secondPredictionP.Y = y;
        }
     
        public void setCountBB()
        {
            int height = Math.Abs(p1.Y - p2.Y);
            int width = Math.Abs(p1.X - p2.X);
            countBB = new Rectangle(p1.X, p1.Y, width, height);
        }

        public void setCountPosition()
        {
            countPosition = p2;
        }

        public void setPerspectivePoints(Matrix<float> matrix)
        {
            setPerspectiveCountPoint(matrix);
            setPerspectiveStartCountedArea(matrix);
            setPerspectiveCountBB(matrix);
        }

        public void setSpeed()
        {
            double tmpDistance = Math.Sqrt(Math.Pow(perspectiveStartCountedAreaPoint.Y - perspectiveCountPosition.Y, 2));
            tmpDistance = PerspectiveTransform.convertDistanceToPerspective(tmpDistance);
            double time = (1/(double)MainForm.FPS)*(double)(numberOfFrames - numberOfFrameStartCountedArea);
            speed = Math.Round((tmpDistance / time) * 3.6 , 2);
        }

        public void setSize()
        {
            size = Math.Round(PerspectiveTransform.convertDistanceToPerspective(perspectiveCountBB.Height) * 0.4, 2);
        }

        private void setPerspectiveCountBB(Matrix<float> matrix)
        {
            Matrix<float> tmp = PerspectiveTransform.perspectiveTransformPoint(countBB.X, countBB.Y, matrix);
            Point tmpLT = new Point((int)Math.Round(tmp[0, 0]), (int)Math.Round(tmp[0, 1]));

            tmp = PerspectiveTransform.perspectiveTransformPoint(countBB.X + countBB.Width, countBB.Y, matrix);
            Point tmpRT = new Point((int)Math.Round(tmp[0, 0]), (int)Math.Round(tmp[0, 1]));

            tmp = PerspectiveTransform.perspectiveTransformPoint(countBB.X, countBB.Y + countBB.Height, matrix);
            Point tmpLB = new Point((int)Math.Round(tmp[0, 0]), (int)Math.Round(tmp[0, 1]));

            tmp = PerspectiveTransform.perspectiveTransformPoint(countBB.X + countBB.Width, countBB.Y + countBB.Height, matrix);
            Point tmpRB = new Point((int)Math.Round(tmp[0, 0]), (int)Math.Round(tmp[0, 1]));

            int height = tmpLB.Y - tmpLT.Y;
            int width = tmpRB.X - tmpLB.X;
            perspectiveCountBB = new Rectangle(tmpLT.X, tmpLT.Y, width, height); 

        }

        private void setPerspectiveCountPoint(Matrix<float> matrix)
        {
            Matrix<float> tmp = PerspectiveTransform.perspectiveTransformPoint(countPosition.X, countPosition.Y, matrix);
            perspectiveCountPosition = new Point((int)Math.Round(tmp[0, 0]), (int)Math.Round(tmp[0, 1]));
        }

        private void setPerspectiveStartCountedArea(Matrix<float> matrix)
        {
            Matrix<float> tmp = PerspectiveTransform.perspectiveTransformPoint(positionOnStartCountedArea.X, positionOnStartCountedArea.Y, matrix);
            perspectiveStartCountedAreaPoint = new Point((int)Math.Round(tmp[0, 0]), (int)Math.Round(tmp[0, 1]));
        }

        public int NumberOfMissingFrames
        {
            get {return numberOfMissingFrames; }
            set { numberOfMissingFrames = value; }
        }

        public bool isUsed
        {
            get { return used; }
            set { used = value; }
        }

        public int NumberOfFrameStartCountedArea
        {
            get { return numberOfFrameStartCountedArea; }
            set { numberOfFrameStartCountedArea = value; }
        }

        public Point PositionOnStartCountedArea
        {
            get { return positionOnStartCountedArea; }
            set { positionOnStartCountedArea = value; }

        }

    }
    
}
