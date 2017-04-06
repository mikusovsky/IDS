using System;
using Emgu.CV;
using System.Drawing;
using System.IO;
using Emgu.CV.Structure;
using IDS.IDS;

namespace IDS
{
   class Vehicle
   {
      public Point P1;
      public Point P2;
      public bool Counted;
      public double Size;
      public Point PredictionP;
      public Point SecondPredictionP;
      public int NumberOfFrames;
      public bool Tracked;
      public Point FirstPosition;
      public Point PerspectiveStartCountedAreaPoint;
      Point _positionOnStartCountedArea;
      public Point PerspectiveFirstPosition;
      public Point CountPosition;
      public Point PerspectiveCountPosition;
      public double Speed;
      public int Type;
      public Rectangle CountBB;
      public Rectangle PerspectiveCountBB;
      private int _numberOfMissingFrames;
      private bool _used;
      private int _numberOfFrameStartCountedArea;
      private Image<Bgr, Byte> _carPhoto;
      private Image<Bgr, Byte> _carMask;

      public bool WasHandled { get; set; }

      public Vehicle(Point p1, Point p2, Image<Bgr, Byte> frameHD, double hdRatio)
      {
         P1 = p1;
         P2 = p2;
         Counted = false;
         Size = 0;
         PredictionP = new Point();
         SecondPredictionP = new Point();
         FirstPosition = new Point();
         _positionOnStartCountedArea = new Point();
         CountPosition = new Point();
         NumberOfFrames = 0;
         Tracked = false;
         Speed = 0;
         Type = 0;
         _numberOfMissingFrames = 0;
         _used = false;
         _numberOfFrameStartCountedArea = 0;
         _CreateVehiclePhoto(frameHD, hdRatio);
         WasHandled = false;
      }

      public void SetPredictionP(int x, int y)
      {
         PredictionP.X = x;
         PredictionP.Y = y;
      }

      public void SetSecondPredictionP(int x, int y)
      {
         SecondPredictionP.X = x;
         SecondPredictionP.Y = y;
      }

      public void SetCountBB()
      {
         int height = Math.Abs(P1.Y - P2.Y);
         int width = Math.Abs(P1.X - P2.X);
         CountBB = new Rectangle(P1.X, P1.Y, width, height);
      }

      public void SetCountPosition()
      {
         CountPosition = P2;
      }

      public void SetPerspectivePoints(Matrix<float> matrix)
      {
         _SetPerspectiveCountPoint(matrix);
         _SetPerspectiveStartCountedArea(matrix);
         _SetPerspectiveCountBB(matrix);
      }

      public void SetSpeed()
      {
         double tmpDistance = Math.Sqrt(Math.Pow(PerspectiveStartCountedAreaPoint.Y - PerspectiveCountPosition.Y, 2));
         tmpDistance = PerspectiveTransform.ConvertDistanceToPerspective(tmpDistance);
         double time = (1 / (double)MainForm.FPS) * (double)(NumberOfFrames - NumberOfFrameStartCountedArea);
         Speed = Math.Round((tmpDistance / time) * 3.6, 2);
      }

      public void SetSize()
      {
         Size = Math.Round(PerspectiveTransform.ConvertDistanceToPerspective(PerspectiveCountBB.Height) * 0.4, 2);
      }

      private void _SetPerspectiveCountBB(Matrix<float> matrix)
      {
         Matrix<float> tmp = PerspectiveTransform.PerspectiveTransformPoint(CountBB.X, CountBB.Y, matrix);
         Point tmpLT = new Point((int)Math.Round(tmp[0, 0]), (int)Math.Round(tmp[0, 1]));

         /*
         tmp = PerspectiveTransform.PerspectiveTransformPoint(CountBB.X + CountBB.Width, CountBB.Y, matrix);
         Point tmpRT = new Point((int)Math.Round(tmp[0, 0]), (int)Math.Round(tmp[0, 1]));
         */

         tmp = PerspectiveTransform.PerspectiveTransformPoint(CountBB.X, CountBB.Y + CountBB.Height, matrix);
         Point tmpLB = new Point((int)Math.Round(tmp[0, 0]), (int)Math.Round(tmp[0, 1]));

         tmp = PerspectiveTransform.PerspectiveTransformPoint(CountBB.X + CountBB.Width, CountBB.Y + CountBB.Height, matrix);
         Point tmpRB = new Point((int)Math.Round(tmp[0, 0]), (int)Math.Round(tmp[0, 1]));

         int height = tmpLB.Y - tmpLT.Y;
         int width = tmpRB.X - tmpLB.X;
         PerspectiveCountBB = new Rectangle(tmpLT.X, tmpLT.Y, width, height);
      }

      private void _SetPerspectiveCountPoint(Matrix<float> matrix)
      {
         Matrix<float> tmp = PerspectiveTransform.PerspectiveTransformPoint(CountPosition.X, CountPosition.Y, matrix);
         PerspectiveCountPosition = new Point((int)Math.Round(tmp[0, 0]), (int)Math.Round(tmp[0, 1]));
      }

      private void _SetPerspectiveStartCountedArea(Matrix<float> matrix)
      {
         Matrix<float> tmp = PerspectiveTransform.PerspectiveTransformPoint(_positionOnStartCountedArea.X, _positionOnStartCountedArea.Y, matrix);
         PerspectiveStartCountedAreaPoint = new Point((int)Math.Round(tmp[0, 0]), (int)Math.Round(tmp[0, 1]));
      }

      public int NumberOfMissingFrames
      {
         get { return _numberOfMissingFrames; }
         set { _numberOfMissingFrames = value; }
      }

      public bool IsUsed
      {
         get { return _used; }
         set { _used = value; }
      }

      public int NumberOfFrameStartCountedArea
      {
         get { return _numberOfFrameStartCountedArea; }
         set { _numberOfFrameStartCountedArea = value; }
      }

      public Point PositionOnStartCountedArea
      {
         get { return _positionOnStartCountedArea; }
         set { _positionOnStartCountedArea = value; }
      }

      public void _CreateVehiclePhoto(Image<Bgr, Byte> frame, double hdRatio)
      {
         _carPhoto = new Image<Bgr, byte>(frame.Size);
         CvInvoke.cvCopy(frame, _carPhoto, IntPtr.Zero);
         int minX = Convert.ToInt32(Math.Round(hdRatio * Convert.ToDouble(P1.X) - hdRatio)); 
         int minY = Convert.ToInt32(Math.Round(hdRatio * Convert.ToDouble(P1.Y) - (hdRatio)));
         int width = Convert.ToInt32(Math.Round(hdRatio * Convert.ToDouble(P2.X - P1.X) + (hdRatio)));
         int height = Convert.ToInt32(Math.Round(hdRatio * Convert.ToDouble(P2.Y - P1.Y) + hdRatio));
         Rectangle rect = new Rectangle(minX, minY, width, height);
         _carPhoto.ROI = rect;
         //CvInvoke.cvShowImage("carPhoto", _carPhoto);
      }

      public void ClassifiCar()
      {
         //_carMask = Utils.ExtractMask3(_carPhoto);
         //CvInvoke.cvShowImage("carPhoto", _carMask);
      }

      public Image<Bgr, byte> GetCarPhoto()
      {
         string path = $"{Deffinitions.OUTPUT_PATH_IMAGES}\\{Path.GetFileName(Utils.CurentVideoPath).Split('.')[0]}";
         if (!Directory.Exists(path))
         {
            Directory.CreateDirectory(path);
         }
         string filePath = $"{path}\\{DateTime.Now.ToString(@"MMddyyyy_h_mm_tt_")}{Utils.RandomString(3)}.png";

         _carPhoto.Bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
         return _carPhoto;
      }
   }
}
