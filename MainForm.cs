using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV.VideoSurveillance;
using System.Diagnostics;
using CppWrapper;
using IDS.IDS;

namespace IDS
{
   public partial class MainForm : Form
   {
      System.Windows.Forms.Timer _myTimer;
      Capture _capture;

      Image<Bgr, Byte> _frameLow;
      Image<Bgr, Byte> _frameHD;
      Image<Bgr, Byte> _frameRoadLow;
      Image<Bgr, Byte> _frameRoadHD;
      Image<Bgr, Byte> _prevFrame;
      Image<Bgr, Byte> _contoursImage;
      Image<Bgr, Byte> _bbImage;
      Image<Bgr, Byte> _foregroundFrame;
      Image<Bgr, Byte> _backgroundFrame;
      Image<Bgr, Byte> _tempBgrFrame;
      Image<Bgr, Byte> _birdEye;

      Image<Gray, Byte> _canny;
      Image<Gray, Byte> _foregroundGrayFrame;
      Image<Gray, Byte> _tempGrayFrame;
      Image<Gray, Byte> _tempGrayBackgroundFrame;

      Image<Bgr, float> _floatFrame;
      Image<Bgr, float> _floatBackgroundFrame;

      Matrix<float> _homogMatrix;

      private double _hdRatio = 1;
      private double _roadHdRatio = 1;

      MCvGaussBGStatModelParams _mogParams;
      BGStatModel<Bgr> _bgModel;

      bool _startFrameCapture;
      bool _initializedVariables = false;
      bool _createBackgroundImage;
      bool _showTmpImages;
      bool _isDayScene;
      bool _firstFrame;
      bool _paused;

      int _frameNumber;
      int _minYTracking;
      int _maxYTracking;
      int _learningTime;
      int _mergeDistance;

      List<Rectangle> _boundingBoxes;
      BackgroundSubstractorMOG2 _mogDetector;
      TrackingVehicles _tracking;
      PairingHeadlights _pairLights;

      List<Point> _roadPoints;
      Range _roadRange = new Range();
      List<Point> _roadDistancePoints;
      Point _measurePoint1;
      Point _measurePoint2;
      Point _perspectiveMeasurePoint1;
      Point _perspectiveMeasurePoint2;
      List<RoadLane> _roadLanes;
      Stopwatch _sw;

      int _maxWidthOfRoadLane;
      int _minBoundingBoxWidth;

      static public int FPS;
      static public double PerspectiveMeasureDistance = 0;
      static public double RealDistance = 1;
      static public int FRAME_NUMBER_TO_COUNT = 15;
      static public int BIRD_EYE_WIDTH = 300;
      static public int BIRD_EYE_HEIGHT = 400;
      static public int LEARNING_TIME_IN_SEC = 3;
      static public double LEARNING_RATE = 0.005;

      static public int FRAME_HEIGHT;
      static public int FRAME_WIDTH;
      static public int START_COUNTED_AREA;
      static public int END_COUNTED_AREA;
      static public int MAX_SIZE_OF_PERSONAL_CAR;

      public MainForm()
      {
         InitializeComponent();
         KeyPreview = true;
      }

      //inicializacia premennych
      private void _SetStartInitVariables()
      {
         if (!_initializedVariables)
         {
            _InitVariables();
            _initializedVariables = true;
         }

         //ConsoleText.Text = "sirka pruhu: " + maxWidthOfRoadLane.ToString();
         _myTimer.Interval = 1000 / FPS;
         //My_Timer.Interval = 1;

         _myTimer.Tick += new EventHandler(_MyTimerTick);
         _myTimer.Start();

         _paused = false;
      }

      private void _InitVariables()
      {
         _myTimer = new System.Windows.Forms.Timer();
         _mogDetector = new BackgroundSubstractorMOG2(30, 50, false);
         _tracking = new TrackingVehicles(_roadLanes[0].MaxWidth);
         _pairLights = new PairingHeadlights(_roadLanes[0].MaxWidth);

         _measurePoint1 = new Point();
         _measurePoint2 = new Point();
         _perspectiveMeasurePoint1 = new Point();
         _perspectiveMeasurePoint2 = new Point();

         _boundingBoxes = new List<Rectangle>();

         _homogMatrix = PerspectiveTransform.FindHomographyMatrix(_roadPoints);
         //TODO create matrix rectangular to car
         _SetPerspectiveMeasureDistance(_homogMatrix);
         StopButton.Text = Resources.ButtonStop_Stop;

         _InitImage();
         _InitBool();
         _InitMogParams();
         _InitInt();
      }

      private void _InitImage()
      {
         FRAME_HEIGHT = _frameLow.Height;
         FRAME_WIDTH = _frameLow.Width;
         _prevFrame = new Image<Bgr, Byte>(_frameLow.Size);
         _contoursImage = new Image<Bgr, Byte>(_frameLow.Size);
         _bbImage = new Image<Bgr, Byte>(_frameLow.Size);
         _foregroundFrame = new Image<Bgr, Byte>(_frameLow.Size);
         _backgroundFrame = new Image<Bgr, Byte>(_frameLow.Size);
         _foregroundGrayFrame = new Image<Gray, Byte>(_frameLow.Size);
         _tempBgrFrame = new Image<Bgr, Byte>(_frameLow.Size);
         _tempGrayFrame = new Image<Gray, Byte>(_frameLow.Size);
         _tempGrayBackgroundFrame = new Image<Gray, Byte>(_frameLow.Size);
         _birdEye = new Image<Bgr, byte>(BIRD_EYE_WIDTH, BIRD_EYE_HEIGHT);
         _canny = new Image<Gray, Byte>(_frameLow.Size);

         _floatFrame = new Image<Bgr, float>(_frameLow.Size);
         _floatBackgroundFrame = new Image<Bgr, float>(_frameLow.Size);
      }

      private void _InitInt()
      {
         _maxWidthOfRoadLane = _roadLanes[0].MaxWidth;
         _minYTracking = (int)Math.Round(0.05 * FRAME_HEIGHT);
         _maxYTracking = _roadLanes[0].LanePoints[3].Y;
         _learningTime = LEARNING_TIME_IN_SEC * FPS;
         _frameNumber = 0;
         _mergeDistance = (int)(_maxWidthOfRoadLane * 0.1); //15
         _minBoundingBoxWidth = (int)(0.2 * _maxWidthOfRoadLane);

         int laneHeight = _roadLanes[0].LanePoints[3].Y - _roadLanes[0].LanePoints[0].Y;
         START_COUNTED_AREA = (int)(_roadLanes[0].LanePoints[0].Y + (0.6 * laneHeight));
         END_COUNTED_AREA = (int)(_roadLanes[0].LanePoints[0].Y + (0.85 * laneHeight));
         MAX_SIZE_OF_PERSONAL_CAR = 8;
      }

      private void _InitMogParams()
      {
         _mogParams = new MCvGaussBGStatModelParams();
         _mogParams.win_size = 50;
         _mogParams.n_gauss = 2;
         _mogParams.bg_threshold = 0.6;
         _mogParams.std_threshold = 2.5;
         _mogParams.minArea = 5;
         _mogParams.weight_init = 0.05;
         _mogParams.variance_init = 30;
      }

      private void _InitBool()
      {
         _startFrameCapture = true;
         _createBackgroundImage = false;
         _paused = false;
         _firstFrame = true;
      }

      //inicializacia prveho framu
      private void _InicializationFirstFrame()
      {
         _startFrameCapture = false;
         CvInvoke.cvCopy(_frameLow, _prevFrame, IntPtr.Zero);
         _bgModel = new BGStatModel<Bgr>(_frameLow, ref _mogParams);
      }

      //nastavenie referencnej vzdialenosti vo vtacom pohlade
      private void _SetPerspectiveMeasureDistance(Matrix<float> matrix)
      {
         _SetMeasurePoints();
         _SetPerspectiveMeasurePoints(matrix);
         PerspectiveMeasureDistance = Math.Sqrt(Math.Pow(_perspectiveMeasurePoint1.X - _perspectiveMeasurePoint2.X, 2)
             + Math.Pow(_perspectiveMeasurePoint1.Y - _perspectiveMeasurePoint2.Y, 2));
      }

      private void _SetMeasurePoints()
      {
         _measurePoint1 = _roadDistancePoints[0];
         _measurePoint2 = _roadDistancePoints[1];
      }

      //vypocitanie pozicie referencnych bodov v vtacom pohlade
      private void _SetPerspectiveMeasurePoints(Matrix<float> matrix)
      {
         Matrix<float> tmp = PerspectiveTransform.PerspectiveTransformPoint(_measurePoint1.X, _measurePoint1.Y, matrix);
         _perspectiveMeasurePoint1 = new Point((int)Math.Round(tmp[0, 0]), (int)Math.Round(tmp[0, 1]));

         tmp = PerspectiveTransform.PerspectiveTransformPoint(_measurePoint2.X, _measurePoint2.Y, matrix);
         _perspectiveMeasurePoint2 = new Point((int)Math.Round(tmp[0, 0]), (int)Math.Round(tmp[0, 1]));
      }

      private void _GetRoadHdFromFrame(Image<Bgr, Byte> hdFrame, ref Image<Bgr, Byte> roadHdFrame, Range range)
      {
         roadHdFrame = new Image<Bgr, byte>(hdFrame.Size);
         CvInvoke.cvCopy(hdFrame, roadHdFrame, IntPtr.Zero);
         int x = Convert.ToInt32(Math.Round(_hdRatio * range.MinX));
         int y = Convert.ToInt32(Math.Round(_hdRatio * range.MinY));
         int width = Convert.ToInt32(Math.Round(_hdRatio * (range.MaxX - range.MinX)));
         int height = Convert.ToInt32(Math.Round(_hdRatio * (range.MaxY - range.MinY)));
         Rectangle rect = new Rectangle(x, y, width, height);
         roadHdFrame.ROI = rect;
         CvInvoke.cvShowImage("roadHdPhoto", roadHdFrame);
      }

      //hlavna metoda na spracovanie kazdeho framu
      private void _MyTimerTick(object sender, EventArgs e)
      {
         _sw = Stopwatch.StartNew();

         _frameHD = _capture.QueryFrame();

         if (_frameHD != null)
         {
            //_frame = _frameHD;

            if (_startFrameCapture)
            {
               Utils.HdToLow(_frameHD, ref _frameLow);
               _hdRatio = Convert.ToDouble(_frameHD.Height) / Convert.ToDouble(_frameLow.Height);
               //_GetRoadHdFromFrame(_frameHD, ref _frameRoadHD, _roadRange);
               //_HdToLow(_frameRoadHD, ref _frameRoadLow, Enums.TypeOfRatio.Road);
               _InicializationFirstFrame();
               return;
            }

            Utils.HdToLow(_frameHD, ref _frameLow);
            //_GetRoadHdFromFrame(_frameHD, ref _frameRoadHD, _roadRange);
            //_HdToLow(_frameRoadHD, ref _frameRoadLow, Enums.TypeOfRatio.None);
            if (_isDayScene)
            {
               _DayScene();
            }
            else
            {
               _NightScene();
            }

            //CvInvoke.cvCopy(frame.WarpPerspective(homogMatrix, BIRD_EYE_WIDTH, BIRD_EYE_HEIGHT, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC, Emgu.CV.CvEnum.WARP.CV_WARP_DEFAULT, new Bgr(0, 0, 0)), birdEye, IntPtr.Zero);
            //CvInvoke.cvShowImage("vtaci pohlad", birdEye);

            _TrackingVehicles();

            _UpdateStatisticalData();
            _sw.Stop();

            int actualFPS = (int)(1000 / _sw.Elapsed.TotalMilliseconds);
            FpsLabel.Text = actualFPS.ToString() + "fps";
            //Console.WriteLine(actualFPS+ "fps");

            _frameNumber++;
         }
         else
         {
            _myTimer.Stop();
            MessageBox.Show("Koniec videa");
            OpenFileButton.Enabled = true;
         }
      }

      //aktualizovanie zobrazovanych statistickych udajov 
      private void _UpdateStatisticalData()
      {
         string name;
         Label lb;
         int sumCars = 0;
         int sumTrucks = 0;

         for (int i = 0; i < _roadLanes.Count; i++)
         {
            name = "lane" + (i + 1) + Resources.PersonalVehicles;
            lb = (Label)Controls[name];
            lb.Text = _roadLanes[i].NumberOfPersonalCars.ToString();

            name = "lane" + (i + 1) + Resources.Lorries;
            lb = (Label)Controls[name];
            lb.Text = _roadLanes[i].NumberOfTrucks.ToString();

            name = "lane" + (i + 1) + Resources.SumText;
            lb = (Label)Controls[name];
            lb.Text = _roadLanes[i].NumberOfCars.ToString();

            sumCars += _roadLanes[i].NumberOfPersonalCars;
            sumTrucks += _roadLanes[i].NumberOfTrucks;

         }

         lb = (Label)Controls[Deffinitions.SUM_CARS];
         lb.Text = sumCars.ToString();

         lb = (Label)Controls[Deffinitions.SUM_TRUCKS];
         lb.Text = sumTrucks.ToString();

         lb = (Label)Controls[Deffinitions.TOTAL_SUM];
         lb.Text = (sumCars + sumTrucks).ToString();
      }

      //metoda na trackovanie pohybujucich sa objektov
      private void _TrackingVehicles()
      {
         _tracking.FindPrevBb();
         if (_showTmpImages)
         {
            CvInvoke.cvCopy(_frameLow, _contoursImage, IntPtr.Zero);
            // CvInvoke.cvShowImage("predikcia", tracking.DrawPrediction(contoursImage));
         }

         List<Vehicle> newCountedVehicles = _tracking.SetStatisticParam(_homogMatrix);

         foreach (RoadLane lane in _roadLanes)
         {
            _frameLow.DrawPolyline(lane.LanePoints.ToArray(), true, new Bgr(Color.Yellow), 2);
         }

         _ShowInfoAboutVehicles(newCountedVehicles);

         foreach (Vehicle v in _tracking.CurrentVehicles)
         {
            if ((v.P2.Y > v.FirstPosition.Y) && v.NumberOfFrames > 4)
            {
               Rectangle rect = new Rectangle((int)v.P1.X, (int)v.P1.Y, (int)v.P2.X - (int)v.P1.X, (int)v.P2.Y - (int)v.P1.Y);
               _frameLow.Draw(rect, new Bgr(Color.LightGreen), 2);
            }
         }

         _tracking.MoveListCurrentToPrev();

         //LineSegment2D line = new LineSegment2D(new Point(0, START_COUNTED_AREA), new Point(frame.Width, START_COUNTED_AREA));
         //frame.Draw(line, new Bgr(255, 255, 255), 1);
         //line = new LineSegment2D(new Point(0, END_COUNTED_AREA), new Point(frame.Width, END_COUNTED_AREA));
         //frame.Draw(line, new Bgr(255, 255, 255), 1);

         if (_createBackgroundImage)
         {
            string s = "VYTVARAM POZADIE";
            MCvFont font = new MCvFont(FONT.CV_FONT_HERSHEY_PLAIN, 1, 1);
            Point p = new Point(15, 15);
            _frameLow.Draw(s, ref font, p, new Bgr(Color.Yellow));
         }
         mainFrameViewer.Image = _frameLow.Bitmap;
         //CvInvoke.cvShowImage("Monitoring", _frame);
      }

      private void _ShowInfoAboutVehicles(List<Vehicle> vehicles)
      {
         List<string> infoVehicleList = new List<string>();
         foreach (Vehicle v in vehicles)
         {
            int centerPoint = (int)((v.P1.X + v.P2.X) / 2);
            foreach (RoadLane lane in _roadLanes)
            {
               if (centerPoint < lane.LanePoints[2].X && !v.WasHandled)
               {
                  infoVehicleList.Add("---");
                  infoVehicleList.Add("---");
                  infoVehicleList.Add("---");
                  infoVehicleList.Add("---");
                  infoVehicleList.Add(v.Speed.ToString());
                  infoVehicleList.Add(string.Empty);

                  v.GetCarType();
                  CarInfoViewImageList.Images.Add(v.GetCarPhoto().Bitmap);
                  CarInfoView.Items.Add(new ListViewItem(infoVehicleList.ToArray(), CarInfoViewImageList.Images.Count - 1));
                  CarInfoView.Items[CarInfoView.Items.Count - 1].EnsureVisible();
                  v.WasHandled = true;
               }
            }
         }
      }

      //metoda na identifikaciu objektu pocas dna
      private void _DayScene()
      {
         _CreateForegroundFrame("avg");   //frameDiff, mog, mix, MOG2, avg
         if (_createBackgroundImage)
         {
            if (_showTmpImages)
            {
               CvInvoke.cvShowImage("Popredie", _foregroundGrayFrame);
            }
            // spravit hlasku vytvara sa pozadie
            return;
         }

         if (_showTmpImages)
         {
            CvInvoke.cvShowImage("Popredie", _foregroundGrayFrame);
         }

         Image<Gray, Byte> tmpForeground = _foregroundGrayFrame.Clone();
         Image<Gray, Byte> tmpMask = new Image<Gray, byte>(_frameLow.Size);
         Image<Gray, Byte> tmpSmooth = new Image<Gray, byte>(_frameLow.Size);
         Image<Gray, Byte> tmpThresh = new Image<Gray, byte>(_frameLow.Size);
         Image<Gray, Byte> tmpMorp = new Image<Gray, byte>(_frameLow.Size);

         for (int i = 0; i < _roadLanes.Count; i++)
         {
            _foregroundGrayFrame = tmpForeground.Clone();

            _CreateMask2(i);
            tmpMask += _foregroundGrayFrame;

            if (_showTmpImages)
            {
               CvInvoke.cvShowImage("maska", tmpMask);
            }

            _CreateSmoothFrame();
            tmpSmooth += _foregroundGrayFrame;

            _CreateTresholdFrame();
            tmpThresh += _foregroundGrayFrame;

            _CreateMorphologyFrameOnDay();
            tmpMorp += _foregroundGrayFrame;

            if (_showTmpImages)
            {
               CvInvoke.cvShowImage("po morfologii", tmpMorp);
            }

            if (i == 0)
            {
               _CreateBoundingBox(true, true);
            }
            else
            {
               _CreateBoundingBox(true, false);
            }

            if (_showTmpImages)
            {
               CvInvoke.cvShowImage("Boundingboxy", _bbImage);
            }
         }
      }

      //metoda na identifikaciu objektu pocas noci
      private void _NightScene()
      {
         _foregroundGrayFrame = _frameLow.Convert<Gray, Byte>();
         if (_showTmpImages)
         {
            CvInvoke.cvShowImage("Gray Image", _foregroundGrayFrame);
         }

         _foregroundGrayFrame = _foregroundGrayFrame.ThresholdBinary(new Gray(240), new Gray(255));

         Image<Gray, Byte> roadMask = new Image<Gray, Byte>(_frameLow.Size);

         roadMask.FillConvexPoly(_roadPoints.ToArray(), new Gray(255));
         //CvInvoke.cvAnd(foregroundGrayFrame, roadMask, foregroundGrayFrame, IntPtr.Zero);

         if (_showTmpImages)
         {
            CvInvoke.cvShowImage("Threshold Image", _foregroundGrayFrame);
         }

         int kernelSize = (int)Math.Ceiling(_maxWidthOfRoadLane * 0.03); //TODO
         StructuringElementEx kernel = new StructuringElementEx(kernelSize, kernelSize, 1, 1, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE);
         CvInvoke.cvMorphologyEx(_foregroundGrayFrame, _foregroundGrayFrame, IntPtr.Zero, kernel, CV_MORPH_OP.CV_MOP_CLOSE, 1);// TODO skusit vacsie cislo

         if (_showTmpImages)
         {
            CvInvoke.cvShowImage("Morfologia", _foregroundGrayFrame);
         }

         _CreateBoundingBox(false, true);

         if (_showTmpImages)
         {
            CvInvoke.cvShowImage("Boundingboxy", _bbImage);
         }
      }

      //vytvorenie masky pre popredie
      private void _CreateMask2(int indexRoadLane)
      {
         _foregroundGrayFrame._ThresholdBinary(new Gray(40), new Gray(255));

         Image<Gray, Byte> roadMask = new Image<Gray, byte>(_frameLow.Size);
         roadMask.FillConvexPoly(_roadLanes[indexRoadLane].LanePoints.ToArray(), new Gray(255));

         CvInvoke.cvAnd(_foregroundGrayFrame, roadMask, _foregroundGrayFrame, IntPtr.Zero);

         _canny = _CannyMask();
         if (_showTmpImages)
         {
            CvInvoke.cvShowImage("canny v pruhu" + indexRoadLane, _canny);
         }
      }

      //najdenie hran
      private Image<Gray, Byte> _CannyMask()
      {
         Image<Gray, Byte> gray = _foregroundGrayFrame;//.PyrDown().PyrUp();
         Image<Gray, Byte> cannyGray1 = gray.Canny(new Gray(50), new Gray(50));
         //Image<Gray, Byte> cannyGray1 = gray.Canny(50, 50);
         Image<Gray, Byte> cannyGray2 = cannyGray1.Clone();
         Image<Gray, Byte> cannyGray = cannyGray1 + cannyGray2;

         int[,] element = { { 0, 0, 0 }, { 1, 1, 1 }, { 1, 1, 1 } };
         StructuringElementEx kernel = new StructuringElementEx(element, 1, 1);
         CvInvoke.cvDilate(cannyGray1, cannyGray, kernel, 1);

         return cannyGray;
      }

      //vyhladenie obrazu
      private void _CreateSmoothFrame()
      {
         int size = (int)Math.Ceiling(_maxWidthOfRoadLane * 0.07);
         _foregroundGrayFrame = _foregroundGrayFrame.SmoothBlur(size, size);
      }

      //vykonanie morfologickych operacii na obraze
      private void _CreateMorphologyFrameOnDay()
      {
         int size = (int)Math.Ceiling(_maxWidthOfRoadLane * 0.05);
         int anchor = size / 2;
         StructuringElementEx kernel = new StructuringElementEx(size, size, anchor, anchor, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_RECT);
         CvInvoke.cvMorphologyEx(_foregroundGrayFrame, _foregroundGrayFrame, IntPtr.Zero, kernel, CV_MORPH_OP.CV_MOP_CLOSE, 1);
      }

      //prahovanie obrazu
      private void _CreateTresholdFrame()
      {
         _foregroundGrayFrame = _foregroundGrayFrame.ThresholdBinary(new Gray(80), new Gray(255));
      }

      //identifikovanie popredia
      private void _CreateForegroundFrame(string type)
      {
         if (type.CompareTo("mog") == 0)
         {
            _foregroundGrayFrame = _CreateMOG();
         }
         else if (type.CompareTo("frameDiff") == 0)
         {
            _foregroundFrame = _CreateDiffFrame();
            _foregroundGrayFrame = _foregroundFrame.Convert<Gray, Byte>();
            CvInvoke.cvCopy(_frameLow, _prevFrame, IntPtr.Zero);
         }
         else if (type.CompareTo("mix") == 0)
         {
            _tempGrayFrame = _CreateDiffFrame().Convert<Gray, Byte>();
            CvInvoke.cvCopy(_frameLow, _prevFrame, IntPtr.Zero);
            CvInvoke.cvAnd(_CreateMOG(), _tempGrayFrame, _foregroundGrayFrame, IntPtr.Zero);
         }
         else if (type.CompareTo("MOG2") == 0)
         {
            _foregroundGrayFrame = _CreateMOG2();
         }
         else if (type.CompareTo("avg") == 0)
         {
            _foregroundGrayFrame = _CreateAvg();
         }
      }

      private Image<Gray, Byte> _CreateAvg()
      {
         Image<Bgr, Byte> temp = new Image<Bgr, Byte>(_frameLow.Size);

         if (_firstFrame)
         {
            _firstFrame = false;
            _floatFrame = _frameLow.Convert<Bgr, float>();
            CvInvoke.cvCopy(_floatFrame, _floatBackgroundFrame, IntPtr.Zero);
            _createBackgroundImage = true;
         }
         else
         {
            _floatFrame = _frameLow.Convert<Bgr, float>();

            if (_frameNumber < _learningTime)
            {
               _floatBackgroundFrame.RunningAvg(_floatFrame, 0.01);
               _createBackgroundImage = true;
            }
            else
            {
               _floatBackgroundFrame.RunningAvg(_floatFrame, LEARNING_RATE);
               _createBackgroundImage = false;
            }
            _backgroundFrame = _floatBackgroundFrame.Convert<Bgr, Byte>();
            //CvInvoke.cvShowImage("pozadie", _backgroundFrame);
            CvInvoke.cvAbsDiff(_frameLow, _backgroundFrame, temp);
         }
         return temp.Convert<Gray, Byte>();
      }

      private Image<Gray, Byte> _CreateMOG()
      {
         if (_frameNumber < _learningTime)
         {
            _createBackgroundImage = true;
         }
         else
         {
            _createBackgroundImage = false;
         }
         _bgModel.Update(_frameLow, -1);
         return _bgModel.ForgroundMask;

      }

      private Image<Gray, Byte> _CreateMOG2()
      {
         if (_frameNumber < _learningTime)
         {
            _mogDetector.Update(_frameLow);
            _createBackgroundImage = true;
         }
         else
         {
            _mogDetector.Update(_frameLow, 0.1);
            _createBackgroundImage = false;
         }
         return _mogDetector.ForgroundMask;
      }

      private Image<Bgr, Byte> _CreateDiffFrame()
      {
         Image<Bgr, Byte> temp = new Image<Bgr, Byte>(_frameLow.Size);
         CvInvoke.cvAbsDiff(_prevFrame, _frameLow, temp);

         return temp;
      }

      //vytvorenie bounding boxov pre pohybujuce sa objekty
      private void _CreateBoundingBox(bool dayScene, bool firstLane)
      {

         if (firstLane)
         {
            CvInvoke.cvCopy(_frameLow, _bbImage, IntPtr.Zero);
         }

         //vytvorenie kontur
         _boundingBoxes.Clear();
         using (MemStorage storage = new MemStorage())
            for (Contour<Point> contours = _foregroundGrayFrame.FindContours(CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE,
              RETR_TYPE.CV_RETR_EXTERNAL, storage); contours != null; contours = contours.HNext)
            {
               Seq<Point> tr = contours.GetConvexHull(ORIENTATION.CV_CLOCKWISE);
               _boundingBoxes.Add(CvInvoke.cvBoundingRect(tr, true));
               _bbImage.Draw(_boundingBoxes[_boundingBoxes.Count - 1], new Bgr(Color.Red), 1);
            }

         _MergeBoundingBoxes();

         foreach (Rectangle bb in _boundingBoxes)
         {
            if (dayScene)
            {
               if ((bb.Width < _minBoundingBoxWidth))
               {
                  _bbImage.Draw(bb, new Bgr(Color.AliceBlue), 1);
                  continue;
               }

               // spatne porovnanie hran
               Image<Gray, Byte> BBMask = new Image<Gray, byte>(_frameLow.Size);
               Point[] points = { new Point(bb.Left, bb.Top), new Point(bb.Right, bb.Top), new Point(bb.Right, bb.Bottom), new Point(bb.Left, bb.Bottom) };
               BBMask.FillConvexPoly(points, new Gray(255));
               CvInvoke.cvAnd(BBMask, _canny, BBMask, IntPtr.Zero);

               if (BBMask.CountNonzero()[0] <= 6 * (bb.Width + bb.Height))
               {
                  _bbImage.Draw(bb, new Bgr(Color.Orange), 1);
                  continue;
               }

               if ((bb.Y > _minYTracking) && ((bb.Y + bb.Height) < _maxYTracking))
               {
                  _tracking.AddCurrentVehicle(bb, _frameHD, _hdRatio);
               }
               _bbImage.Draw(bb, new Bgr(Color.Yellow), 1);
            }
            else
            {
               if ((bb.Width * bb.Height) < 7)
               {
                  _bbImage.Draw(bb, new Bgr(Color.AliceBlue), 1);
                  continue;
               }

               if ((bb.Y > _minYTracking) && ((bb.Y + bb.Height) < _maxYTracking))
               {
                  _pairLights.AddHeadlight(bb);
               }
               _bbImage.Draw(bb, new Bgr(Color.Yellow), 1);
            }
         }

         if (!dayScene)
         {
            List<Rectangle> vehiclesBB;
            vehiclesBB = _pairLights._CreateVehiclesBB();

            foreach (Rectangle rect in vehiclesBB)
            {
               _tracking.AddCurrentVehicle(rect, _frameHD, _hdRatio);
            }

            _pairLights._ClearList();
         }
      }

      //spajanie urcitych boundingboxov
      private void _MergeBoundingBoxes()
      {
         //List<Rectangle> BB = new List<Rectangle>();
         for (int i = _boundingBoxes.Count - 1; i >= 0; i--)
         {
            for (int j = 0; j < _boundingBoxes.Count; j++)
            {
               if (i == j)
               {
                  continue;
               }

               //ak sa horizontalne prekryvaju
               if (_boundingBoxes[i].Left < _boundingBoxes[j].Right && _boundingBoxes[j].Left < _boundingBoxes[i].Right)
               {
                  //ak sa aj vertikalne prekryvaju
                  if (_boundingBoxes[i].Top < _boundingBoxes[j].Bottom && _boundingBoxes[j].Top < _boundingBoxes[i].Bottom)
                  {
                     Point p1 = new Point(Math.Min(_boundingBoxes[i].Left, _boundingBoxes[j].Left), Math.Min(_boundingBoxes[i].Top, _boundingBoxes[j].Top));
                     Point p2 = new Point(Math.Max(_boundingBoxes[i].Right, _boundingBoxes[j].Right), Math.Max(_boundingBoxes[i].Bottom, _boundingBoxes[j].Bottom));
                     int height = p2.Y - p1.Y;
                     int width = p2.X - p1.X;
                     Rectangle newBB = new Rectangle(p1.X, p1.Y, width, height);
                     _boundingBoxes[j] = newBB;
                     _boundingBoxes.RemoveAt(i);
                     break;
                  }

                  int minDist = Math.Min(Math.Abs(_boundingBoxes[i].Top - _boundingBoxes[j].Bottom),
                                          Math.Abs(_boundingBoxes[i].Bottom - _boundingBoxes[j].Top));

                  //ak su blizko seba
                  if (minDist < _mergeDistance)
                  {
                     Point p1 = new Point(Math.Min(_boundingBoxes[i].Left, _boundingBoxes[j].Left), Math.Min(_boundingBoxes[i].Top, _boundingBoxes[j].Top));
                     Point p2 = new Point(Math.Max(_boundingBoxes[i].Right, _boundingBoxes[j].Right), Math.Max(_boundingBoxes[i].Bottom, _boundingBoxes[j].Bottom));
                     int height = p2.Y - p1.Y;
                     int width = p2.X - p1.X;
                     Rectangle newBB = new Rectangle(p1.X, p1.Y, width, height);
                     _boundingBoxes[j] = newBB;
                     _boundingBoxes.RemoveAt(i);
                     break;
                  }
               }
            }
         }
         //return BB;
      }

      //otvorenie a nacitanie videa 
      private void _LoadVideoButton_Click(object sender, EventArgs e)
      {
         OpenFileDialog.Filter = "Media Files|*.avi;*.mp4;*.MOV";
         OpenFileDialog.FileName = "";
         //initializedVariables = false;
         if (OpenFileDialog.ShowDialog() == DialogResult.OK)
         {
            try
            {
               _capture = null;
               _capture = new Capture(OpenFileDialog.FileName);
               FPS = (int)_capture.GetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FPS);
               _initializedVariables = false;
            }
            catch (NullReferenceException excpt)
            {
               MessageBox.Show(excpt.Message);
            }
         }
         else
         {
            return;
         }


         if (DaySceneRadioButton.Checked)
         {
            _isDayScene = true;
         }
         else
         {
            _isDayScene = false;
         }

         if (ShowTmpImageCheckBox.Checked)
         {
            _showTmpImages = true;
         }
         else
         {
            _showTmpImages = false;
         }

         if (_capture != null)
         {
            _frameHD = _capture.QueryFrame();
            //_frame = _frameHD;
            Utils.HdToLow(_frameHD, ref _frameLow);
            _hdRatio = Convert.ToDouble(_frameHD.Height) / Convert.ToDouble(_frameLow.Height);

            string onlyfilename = OpenFileDialog.SafeFileName;
            RoadParamForm roadParam = new RoadParamForm(_frameLow, onlyfilename);

            roadParam.ShowDialog();

            if (roadParam.IsSetAllRoadParam)
            {
               OpenFileButton.Enabled = false;
               _roadPoints = roadParam.GetRoadPoints();
               _roadDistancePoints = roadParam.GetRoadDistancePoints();
               RealDistance = roadParam.GetRealDistance();
               _roadLanes = roadParam.CreateRoadLanes();

               _roadRange.Clear();
               _roadRange.AddToRange(_roadPoints);
               //_GetRoadHdFromFrame(_frameHD, ref _frameRoadHD, _roadRange);
               //_HdToLow(_frameRoadHD, ref _frameRoadLow, Enums.TypeOfRatio.Road);

               _SetStartInitVariables();
               _CreateStatisticLabels();
            }
         }
      }

      //vytvorenie GUI pre statisticke informacie
      private void _CreateStatisticLabels()
      {

         int x = 512;
         int y = 73;
         int dy = 24;


         for (int i = 1; i <= _roadLanes.Count; i++)
         {
            Label laneText = new Label();
            laneText.Location = new Point(x, y);
            laneText.Text = "Pruh č." + i;
            laneText.AutoSize = true;


            Label laneCars = new Label();
            laneCars.Location = new Point(x + 72, y);
            laneCars.Text = "0";
            laneCars.Name = "lane" + i + Resources.PersonalVehicles;
            laneCars.AutoSize = true;

            Label laneTrucks = new Label();
            laneTrucks.Location = new Point(x + 128, y);
            laneTrucks.Text = "0";
            laneTrucks.Name = "lane" + i + Resources.Lorries;
            laneTrucks.AutoSize = true;

            Label laneSum = new Label();
            laneSum.Location = new Point(x + 188, y);
            laneSum.Text = "0";
            laneSum.Name = "lane" + i + Resources.SumText;
            laneSum.AutoSize = true;

            Controls.Add(laneText);
            Controls.Add(laneCars);
            Controls.Add(laneTrucks);
            Controls.Add(laneSum);
            y += dy;
         }

         Label sumText = new Label();
         sumText.Location = new Point(x, y);
         sumText.Text = Resources.Together;
         sumText.AutoSize = true;


         Label sumCars = new Label();
         sumCars.Location = new Point(x + 72, y);
         sumCars.Text = "0";
         sumCars.Name = Deffinitions.SUM_CARS;
         sumCars.AutoSize = true;

         Label sumTrucks = new Label();
         sumTrucks.Location = new Point(x + 128, y);
         sumTrucks.Text = "0";
         sumTrucks.Name = Deffinitions.SUM_TRUCKS;
         sumTrucks.AutoSize = true;

         Label totalSum = new Label();
         totalSum.Location = new Point(x + 188, y);
         totalSum.Text = "0";
         totalSum.Name = Deffinitions.TOTAL_SUM;
         totalSum.AutoSize = true;

         Controls.Add(sumText);
         Controls.Add(sumCars);
         Controls.Add(sumTrucks);
         Controls.Add(totalSum);


      }


      private void _StopButton_Click(object sender, EventArgs e)
      {
         if (_paused)
         {
            StopButton.Text = Resources.ButtonStop_Stop;
            _paused = false;
            _myTimer.Start();
            OpenFileButton.Enabled = false;
         }
         else
         {
            StopButton.Text = Resources.ButtonStop_Start;
            _paused = true;
            _myTimer.Stop();
            OpenFileButton.Enabled = true;
         }
      }

      private void MainForm_KeyDown(object sender, KeyEventArgs e)
      {
         if (e.KeyCode == Keys.N)
         {
            _StopButton_Click(sender, new EventArgs());
         }
      }

      private void ButtonLoadDb_Click(object sender, EventArgs e)
      {
         int noElements = 1000;
         int[] myArray = new int[noElements];

         for (int i = 0; i < noElements; i++)
         {
            myArray[i] = i*10;
         }
         //Utils.DetectRectangle();
         /*
         unsafe
         {
            fixed (int* pmyArray = &myArray[0])
            {
               CppWrapperClass controlCpp = new CppWrapperClass(pmyArray, noElements);
               int sumOfArray = controlCpp.getSum();
               sumOfArray = controlCpp.sum;
               int adam = controlCpp.localizeLicencePlate();
            }
         }
         */
         using (OpenFileDialog dlg = new OpenFileDialog())
         {
            dlg.Title = "Open Image";
            dlg.Filter = "All files (*.*)|*.*";//"bmp files (*.jpg)|*.jpg|All files (*.*)|*.*";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
               Image<Bgr, byte> image = new Image<Bgr, byte>(new Bitmap(dlg.FileName));
               Utils.ExtractMask2(image);
            }
         }
      }
   }
}
