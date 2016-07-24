using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using System.Threading;
using Emgu.CV.CvEnum;
using Emgu.CV.VideoSurveillance;
using System.Diagnostics;

namespace IDS
{
    public partial class MainForm : Form
    {
        System.Windows.Forms.Timer My_Timer;
        Capture _capture;

        Image<Bgr, Byte> frame;
        Image<Bgr, Byte> prevFrame;
        Image<Bgr, Byte> contoursImage;
        Image<Bgr, Byte> BBImage;
        Image<Bgr, Byte> foregroundFrame;
        Image<Bgr, Byte> backgroundFrame;
        Image<Bgr, Byte> tempBgrFrame;
        Image<Bgr, Byte> birdEye;
        
        Image<Gray, Byte> canny;
        Image<Gray, Byte> foregroundGrayFrame;
        Image<Gray, Byte> tempGrayFrame;
        Image<Gray, Byte> tempGrayBackgroundFrame;

        Image<Bgr, float> floatFrame;
        Image<Bgr, float> floatBackgroundFrame;
        
        Matrix<float> homogMatrix;

        MCvGaussBGStatModelParams mogParams;
        BGStatModel<Bgr> bgModel;
        
        bool startFrameCapture;
        bool initializedVariables = false;
        bool createBackgroundImage;
        bool showTmpImages;
        bool isDayScene;
        bool firstFrame;
        bool paused;

        int frameNumber;
        int minYTracking;
        int maxYTracking;
        int learningTime;
        int mergeDistance;

        List<Rectangle> boundingBoxes;
        BackgroundSubstractorMOG2 mogDetector;
        TrackingVehicles tracking;
        PairingHeadlights pairLights;

        List<Point> roadPoints;
        List<Point> roadDistancePoints;
        Point measurePoint1;
        Point measurePoint2;
        Point perspectiveMeasurePoint1;
        Point perspectiveMeasurePoint2;
        List<RoadLane> roadLanes;
        Stopwatch sw;
        
        int maxWidthOfRoadLane;
        int minBoundingBoxWidth;

        static public int FPS;
        static public double perspectiveMeasureDistance = 0;
        static public double realDistance = 1;
        static public int FRAME_NUMBER_TO_COUNT = 15;
        static public int BIRD_EYE_WIDTH = 300;
        static public int BIRD_EYE_HEIGHT = 400;
        static public int LEARNING_TIME_IN_SEC = 3;
        static public double LEARNING_RATE = 0.005;

        static public int FRAME_HEIGHT;
        static public int FRAME_WIDTH;
        static public int START_COUNTED_AREA;
        static public int END_COUNTED_AREA;
        static public int MAX_SIZE_OF_PRIVATE_CAR;

        public MainForm()
        {
            InitializeComponent();
            this.KeyPreview = true;
        }

        //inicializacia premennych
        private void setStartInitVariables()
        {
            if (!initializedVariables)
            {
                initVariables();
                initializedVariables = true;
            }

            //consoleText.Text = "sirka pruhu: " + maxWidthOfRoadLane.ToString();
            My_Timer.Interval = 1000 / FPS;
            //My_Timer.Interval = 1;

            My_Timer.Tick += new EventHandler(My_Timer_Tick);
            My_Timer.Start();

            paused = false;
        }
        
        private void initVariables()
        {
            My_Timer = new System.Windows.Forms.Timer();

            mogDetector = new BackgroundSubstractorMOG2(30, 50, false);
            tracking = new TrackingVehicles(roadLanes[0].MaxWidth);
            pairLights = new PairingHeadlights(roadLanes[0].MaxWidth);

            measurePoint1 = new Point();
            measurePoint2 = new Point(); ;
            perspectiveMeasurePoint1 = new Point();
            perspectiveMeasurePoint2 = new Point();

            boundingBoxes = new List<Rectangle>();

            homogMatrix = PerspectiveTransform.findHomographyMatrix(roadPoints);
            setPerspectiveMeasureDistance(homogMatrix);
            consoleText.Text = "";
            stopButton.Text = "Stop";

            initImage();
            initBool();
            initMogParams();
            initInt();
        }

        private void initImage()
        {
            FRAME_HEIGHT = frame.Height;
            FRAME_WIDTH = frame.Width;
            prevFrame = new Image<Bgr, Byte>(frame.Size);
            contoursImage = new Image<Bgr, Byte>(frame.Size);
            BBImage = new Image<Bgr, Byte>(frame.Size);
            foregroundFrame = new Image<Bgr, Byte>(frame.Size);
            backgroundFrame = new Image<Bgr, Byte>(frame.Size);
            foregroundGrayFrame = new Image<Gray, Byte>(frame.Size);
            tempBgrFrame = new Image<Bgr, Byte>(frame.Size);
            tempGrayFrame = new Image<Gray, Byte>(frame.Size);
            tempGrayBackgroundFrame = new Image<Gray, Byte>(frame.Size);
            birdEye = new Image<Bgr, byte>(BIRD_EYE_WIDTH, BIRD_EYE_HEIGHT);
            canny = new Image<Gray, Byte>(frame.Size);

            floatFrame = new Image<Bgr, float>(frame.Size);
            floatBackgroundFrame = new Image<Bgr, float>(frame.Size);
        }

        private void initInt()
        {
            maxWidthOfRoadLane = roadLanes[0].MaxWidth;
            minYTracking = (int)Math.Round(0.05 * FRAME_HEIGHT);
            maxYTracking = roadLanes[0].LanePoints[3].Y;
            learningTime = LEARNING_TIME_IN_SEC * FPS;
            frameNumber = 0;
            mergeDistance = (int)(maxWidthOfRoadLane * 0.1); //15
            minBoundingBoxWidth = (int)(0.2 * maxWidthOfRoadLane);
            
            int laneHeight = roadLanes[0].LanePoints[3].Y - roadLanes[0].LanePoints[0].Y;
            START_COUNTED_AREA = (int)(roadLanes[0].LanePoints[0].Y + (0.6 * laneHeight));
            END_COUNTED_AREA = (int)(roadLanes[0].LanePoints[0].Y + (0.85 * laneHeight));
            MAX_SIZE_OF_PRIVATE_CAR = 8;
        }

        private void initMogParams()
        {
            mogParams = new MCvGaussBGStatModelParams();
            mogParams.win_size = 50;
            mogParams.n_gauss = 2;
            mogParams.bg_threshold = 0.6;
            mogParams.std_threshold = 2.5;
            mogParams.minArea = 5;
            mogParams.weight_init = 0.05;
            mogParams.variance_init = 30;
        }

        private void initBool()
        {
            startFrameCapture = true;
            createBackgroundImage = false;
            paused = false;
            firstFrame = true;
        }

        //inicializacia prveho framu
        private void inicializationFirstFrame()
        {
            startFrameCapture = false;
            CvInvoke.cvCopy(frame, prevFrame, IntPtr.Zero);
            bgModel = new BGStatModel<Bgr>(frame, ref mogParams);
        }

        //nastavenie referencnej vzdialenosti vo vtacom pohlade
        private void setPerspectiveMeasureDistance(Matrix<float> matrix)
        {
            setMeasurePoints();
            setPerspectiveMeasurePoints(matrix);
            perspectiveMeasureDistance = Math.Sqrt(Math.Pow(perspectiveMeasurePoint1.X - perspectiveMeasurePoint2.X, 2)
                + Math.Pow(perspectiveMeasurePoint1.Y - perspectiveMeasurePoint2.Y, 2));
        }

        private void setMeasurePoints()
        {
            measurePoint1 = roadDistancePoints[0];
            measurePoint2 = roadDistancePoints[1];
        }

        //vypocitanie pozicie referencnych bodov v vtacom pohlade
        private void setPerspectiveMeasurePoints(Matrix<float> matrix)
        {
            Matrix<float> tmp = PerspectiveTransform.perspectiveTransformPoint(measurePoint1.X, measurePoint1.Y, matrix);
            perspectiveMeasurePoint1 = new Point((int)Math.Round(tmp[0, 0]), (int)Math.Round(tmp[0, 1]));

            tmp = PerspectiveTransform.perspectiveTransformPoint(measurePoint2.X, measurePoint2.Y, matrix);
            perspectiveMeasurePoint2 = new Point((int)Math.Round(tmp[0, 0]), (int)Math.Round(tmp[0, 1]));
        }

        //hlavna metoda na spracovanie kazdeho framu
        private void My_Timer_Tick(object sender, EventArgs e)
        {
            sw = Stopwatch.StartNew();

            frame = _capture.QueryFrame();

            if (frame != null)
            {
                if (startFrameCapture)
                {
                    inicializationFirstFrame();
                    return;
                }

                if (isDayScene)
                {
                    dayScene();
                }
                else
                {
                    nightScene();
                }

                //CvInvoke.cvCopy(frame.WarpPerspective(homogMatrix, BIRD_EYE_WIDTH, BIRD_EYE_HEIGHT, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC, Emgu.CV.CvEnum.WARP.CV_WARP_DEFAULT, new Bgr(0, 0, 0)), birdEye, IntPtr.Zero);
                //CvInvoke.cvShowImage("vtaci pohlad", birdEye);

                trackingVehicles();
                                
                updateStatisticalData();
                sw.Stop();

                int actualFPS = (int)(1000 / sw.Elapsed.TotalMilliseconds);
                fpsLabel.Text = actualFPS.ToString() + "fps";
                //Console.WriteLine(actualFPS+ "fps");

                frameNumber++;
            }
            else
            {
                My_Timer.Stop();
                MessageBox.Show("Koniec videa");
                openFileButton.Enabled = true;
            }
        }

        //aktualizovanie zobrazovanych statistickych udajov 
        private void updateStatisticalData()
        {
            string name;
            Label lb;
            int sumCars = 0;
            int sumTrucks = 0;

            for (int i = 0; i < roadLanes.Count; i++)
            {
                name = "lane" + (i + 1).ToString() + "Cars";
                lb = (Label)this.Controls[name];
                lb.Text = roadLanes[i].NumberOfPrivateCars.ToString();

                name = "lane" + (i + 1).ToString() + "Trucks";
                lb = (Label)this.Controls[name];
                lb.Text = roadLanes[i].NumberOfTrucks.ToString();

                name = "lane" + (i + 1).ToString() + "Sum";
                lb = (Label)this.Controls[name];
                lb.Text = roadLanes[i].NumberOfCars.ToString();

                sumCars += roadLanes[i].NumberOfPrivateCars;
                sumTrucks += roadLanes[i].NumberOfTrucks;

            }

            lb = (Label)this.Controls["sumCars"];
            lb.Text = sumCars.ToString();

            lb = (Label)this.Controls["sumTrucks"];
            lb.Text = sumTrucks.ToString();

            lb = (Label)this.Controls["totalSum"];
            lb.Text = (sumCars + sumTrucks).ToString();


        }

        //metoda sa trackovanie pohybujucich sa objektov
        private void trackingVehicles()
        {
            tracking.findPrevBB();
            if (showTmpImages)
            {
                CvInvoke.cvCopy(frame, contoursImage, IntPtr.Zero);
                // CvInvoke.cvShowImage("predikcia", tracking.drawPrediction(contoursImage));
            }

            List<Vehicle> newCountedVehicles = tracking.setStatisticParam(homogMatrix);

            foreach (RoadLane lane in roadLanes)
            {
                frame.DrawPolyline(lane.LanePoints.ToArray(), true, new Bgr(Color.Yellow), 2);
            }
            
            foreach (Vehicle v in newCountedVehicles)
            {
                int centerPoint = (int)((v.p1.X + v.p2.X) / 2);
                string tmpText;
                foreach (RoadLane lane in roadLanes)
                {
                    if (centerPoint < lane.LanePoints[2].X)
                    {
                        if (v.size < MAX_SIZE_OF_PRIVATE_CAR)
                        {
                            lane.NumberOfPrivateCars++;
                            tmpText = tracking.vehiclesCount.ToString() + "- Osobné v., \t r: " + v.speed.ToString();
                        }
                        else
                        {
                            lane.NumberOfTrucks++;
                            tmpText = tracking.vehiclesCount.ToString() + "- Nákladné v.,\t r: " + v.speed.ToString();
                        }

                        consoleText.Text = tmpText + " km/h \r\n" + consoleText.Text;

                        break;
                    }
                }

            }

            foreach (Vehicle v in tracking.currentVehicles)
            {
                if ((v.p2.Y > v.firstPosition.Y) && v.numberOfFrames > 4)
                {
                    Rectangle rect = new Rectangle((int)v.p1.X, (int)v.p1.Y, (int)v.p2.X - (int)v.p1.X, (int)v.p2.Y - (int)v.p1.Y);
                    frame.Draw(rect, new Bgr(Color.LightGreen), 2);
                }
            }

            tracking.moveListCurrentToPrev();

            //LineSegment2D line = new LineSegment2D(new Point(0, START_COUNTED_AREA), new Point(frame.Width, START_COUNTED_AREA));
            //frame.Draw(line, new Bgr(255, 255, 255), 1);
            //line = new LineSegment2D(new Point(0, END_COUNTED_AREA), new Point(frame.Width, END_COUNTED_AREA));
            //frame.Draw(line, new Bgr(255, 255, 255), 1);

            if (createBackgroundImage)
            {
                string s = "VYTVARAM POZADIE";
                MCvFont font = new MCvFont(FONT.CV_FONT_HERSHEY_PLAIN, 1, 1);
                Point p = new Point(15, 15);
                frame.Draw(s, ref font, p, new Bgr(Color.Yellow));
            }
            
            CvInvoke.cvShowImage("Monitoring", frame);
        }

        //metoda na identifikaciu objektu pocas dna
        private void dayScene()
        {
            createForegroundFrame("avg");   //frameDiff, mog, mix, MOG2, avg
            if (createBackgroundImage)
            {
                if (showTmpImages)
                {
                    CvInvoke.cvShowImage("Popredie", foregroundGrayFrame);
                }
                // spravit hlasku vytvara sa pozadie
                return;
            }

            if (showTmpImages)
            {
                CvInvoke.cvShowImage("Popredie", foregroundGrayFrame);
            }

            Image<Gray, Byte> tmpForeground = foregroundGrayFrame.Clone();
            Image<Gray, Byte> tmpMask = new Image<Gray, byte>(frame.Size);
            Image<Gray, Byte> tmpSmooth = new Image<Gray, byte>(frame.Size);
            Image<Gray, Byte> tmpThresh = new Image<Gray, byte>(frame.Size);
            Image<Gray, Byte> tmpMorp = new Image<Gray, byte>(frame.Size);

            for (int i = 0; i < roadLanes.Count; i++)
            {
                foregroundGrayFrame = tmpForeground.Clone();

                createMask2(i);
                tmpMask += foregroundGrayFrame;

                if (showTmpImages)
                {
                    CvInvoke.cvShowImage("maska", tmpMask);
                }

                createSmoothFrame();
                tmpSmooth += foregroundGrayFrame;

                createTresholdFrame();
                tmpThresh += foregroundGrayFrame;

                createMorphologyFrameOnDay();
                tmpMorp += foregroundGrayFrame;

                if (showTmpImages)
                {
                    CvInvoke.cvShowImage("po morfologii", tmpMorp);
                }

                if (i == 0)
                {
                    createBoundingBox(true, true);
                }
                else
                {
                    createBoundingBox(true, false);
                }

                if (showTmpImages)
                {
                    CvInvoke.cvShowImage("Boundingboxy", BBImage);
                }
            }
        }

        //metoda na identifikaciu objektu pocas noci
        private void nightScene()
        {
            foregroundGrayFrame = frame.Convert<Gray, Byte>();
            if (showTmpImages)
            {
                CvInvoke.cvShowImage("Gray Image", foregroundGrayFrame);
            }

            foregroundGrayFrame = foregroundGrayFrame.ThresholdBinary(new Gray(240), new Gray(255));

            Image<Gray, Byte> roadMask = new Image<Gray, Byte>(frame.Size);

            roadMask.FillConvexPoly(roadPoints.ToArray(), new Gray(255));
            //CvInvoke.cvAnd(foregroundGrayFrame, roadMask, foregroundGrayFrame, IntPtr.Zero);

            if (showTmpImages)
            {
                CvInvoke.cvShowImage("Threshold Image", foregroundGrayFrame);
            }

            int kernelSize = (int)Math.Ceiling(maxWidthOfRoadLane * 0.03); //TODO
            StructuringElementEx kernel = new StructuringElementEx(kernelSize, kernelSize, 1, 1, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE);
            CvInvoke.cvMorphologyEx(foregroundGrayFrame, foregroundGrayFrame, IntPtr.Zero, kernel, CV_MORPH_OP.CV_MOP_CLOSE, 1);// TODO skusit vacsie cislo

            if (showTmpImages)
            {
                CvInvoke.cvShowImage("Morfologia", foregroundGrayFrame);
            }

            createBoundingBox(false, true);

            if (showTmpImages)
            {
                CvInvoke.cvShowImage("Boundingboxy", BBImage);
            }
        }

        //vytvorenie masky pre popredie
        private void createMask2(int indexRoadLane)
        {
            foregroundGrayFrame._ThresholdBinary(new Gray(40), new Gray(255));

            Image<Gray, Byte> roadMask = new Image<Gray, byte>(frame.Size);
            roadMask.FillConvexPoly(roadLanes[indexRoadLane].LanePoints.ToArray(), new Gray(255));

            CvInvoke.cvAnd(foregroundGrayFrame, roadMask, foregroundGrayFrame, IntPtr.Zero);

            canny = cannyMask();
            if (showTmpImages)
            {
                CvInvoke.cvShowImage("canny v pruhu" + indexRoadLane, canny);
            }
        }

        //najdenie hran
        private Image<Gray, Byte> cannyMask()
        {
            Image<Gray, Byte> gray = foregroundGrayFrame.PyrDown().PyrUp();
            //Image<Gray, Byte> cannyGray1 = gray.Canny(new Gray(50), new Gray(50));
            Image<Gray, Byte> cannyGray1 = gray.Canny(50, 50);
            Image<Gray, Byte> cannyGray2 = cannyGray1.Clone();
            Image<Gray, Byte> cannyGray = cannyGray1 + cannyGray2;

            int[,] element = { { 0, 0, 0 }, { 1, 1, 1 }, { 1, 1, 1 } };
            StructuringElementEx kernel = new StructuringElementEx(element, 1, 1);
            CvInvoke.cvDilate(cannyGray1, cannyGray, kernel, 1);

            return cannyGray;
        }

        //vyhladenie obrazu
        private void createSmoothFrame()
        {
            int size = (int)Math.Ceiling(maxWidthOfRoadLane * 0.07);
            foregroundGrayFrame = foregroundGrayFrame.SmoothBlur(size, size);
        }

        //vykonanie morfologickych operacii na obraze
        private void createMorphologyFrameOnDay()
        {
            int size = (int)Math.Ceiling(maxWidthOfRoadLane * 0.05);
            int anchor = size / 2;
            StructuringElementEx kernel = new StructuringElementEx(size, size, anchor, anchor, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_RECT);
            CvInvoke.cvMorphologyEx(foregroundGrayFrame, foregroundGrayFrame, IntPtr.Zero, kernel, CV_MORPH_OP.CV_MOP_CLOSE, 1);
        }

        //prahovanie obrazu
        private void createTresholdFrame()
        {
            foregroundGrayFrame = foregroundGrayFrame.ThresholdBinary(new Gray(80), new Gray(255));
        }

        //identifikovanie popredia
        private void createForegroundFrame(string type)
        {
            if (type.CompareTo("mog") == 0)
            {
                foregroundGrayFrame = createMOG();
            }
            else if (type.CompareTo("frameDiff") == 0)
            {
                foregroundFrame = createDiffFrame();
                foregroundGrayFrame = foregroundFrame.Convert<Gray, Byte>();
                CvInvoke.cvCopy(frame, prevFrame, IntPtr.Zero);
            }
            else if (type.CompareTo("mix") == 0)
            {
                tempGrayFrame = createDiffFrame().Convert<Gray, Byte>();
                CvInvoke.cvCopy(frame, prevFrame, IntPtr.Zero);
                CvInvoke.cvAnd(createMOG(), tempGrayFrame, foregroundGrayFrame, IntPtr.Zero);
            }
            else if (type.CompareTo("MOG2") == 0)
            {
                foregroundGrayFrame = createMOG2();
            }
            else if (type.CompareTo("avg") == 0)
            {
                foregroundGrayFrame = createAvg();
            }

        }

        private Image<Gray, Byte> createAvg()
        {
            Image<Bgr, Byte> temp = new Image<Bgr, Byte>(frame.Size);

            if (firstFrame)
            {
                firstFrame = false;
                floatFrame = frame.Convert<Bgr, float>();
                CvInvoke.cvCopy(floatFrame, floatBackgroundFrame, IntPtr.Zero);
                createBackgroundImage = true;
            }
            else
            {
                floatFrame = frame.Convert<Bgr, float>();

                if (frameNumber < learningTime)
                {
                    floatBackgroundFrame.RunningAvg(floatFrame, 0.01);
                    createBackgroundImage = true;
                }
                else
                {
                    floatBackgroundFrame.RunningAvg(floatFrame, LEARNING_RATE);
                    createBackgroundImage = false;
                }
                backgroundFrame = floatBackgroundFrame.Convert<Bgr, Byte>();
                CvInvoke.cvShowImage("pozadie", backgroundFrame);
                CvInvoke.cvAbsDiff(frame, backgroundFrame, temp);
            }

            return temp.Convert<Gray, Byte>();
        }

        private Image<Gray, Byte> createMOG()
        {
            if (frameNumber < learningTime)
            {
                createBackgroundImage = true;
            }
            else
            {
                createBackgroundImage = false;
            }
            bgModel.Update(frame, -1);
            return bgModel.ForgroundMask;

        }

        private Image<Gray, Byte> createMOG2()
        {
            if (frameNumber < learningTime)
            {
                mogDetector.Update(frame);
                createBackgroundImage = true;

            }
            else
            {
                mogDetector.Update(frame, 0.1);
                createBackgroundImage = false;
            }

            return mogDetector.ForgroundMask;
        }

        private Image<Bgr, Byte> createDiffFrame()
        {
            Image<Bgr, Byte> temp = new Image<Bgr, Byte>(frame.Size);
            CvInvoke.cvAbsDiff(prevFrame, frame, temp);

            return temp;
        }

        //vytvorenie bounding boxov pre pohybujuce sa objekty
        private void createBoundingBox(bool dayScene, bool firstLane)
        {

            if (firstLane)
            {
                CvInvoke.cvCopy(frame, BBImage, IntPtr.Zero);
            }

            //vytvorenie kontur
            boundingBoxes.Clear();
            using (MemStorage storage = new MemStorage())
                for (Contour<Point> contours = foregroundGrayFrame.FindContours(CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE,
                  RETR_TYPE.CV_RETR_EXTERNAL, storage); contours != null; contours = contours.HNext)
                {
                    Seq<Point> tr = contours.GetConvexHull(ORIENTATION.CV_CLOCKWISE);
                    boundingBoxes.Add(CvInvoke.cvBoundingRect(tr, true));
                    BBImage.Draw(boundingBoxes[boundingBoxes.Count - 1], new Bgr(Color.Red), 1);
                }

            mergeBoundingBoxes();

            foreach (Rectangle bb in boundingBoxes)
            {
                if (dayScene)
                {
                    if ((bb.Width < minBoundingBoxWidth))
                    {
                        BBImage.Draw(bb, new Bgr(Color.AliceBlue), 1);
                        continue;
                    }

                    // spatne porovnanie hran
                    Image<Gray, Byte> BBMask = new Image<Gray, byte>(frame.Size);
                    Point[] points = { new Point(bb.Left, bb.Top), new Point(bb.Right, bb.Top), new Point(bb.Right, bb.Bottom), new Point(bb.Left, bb.Bottom) };
                    BBMask.FillConvexPoly(points, new Gray(255));
                    CvInvoke.cvAnd(BBMask, canny, BBMask, IntPtr.Zero);

                    if (BBMask.CountNonzero()[0] <= 6 * (bb.Width + bb.Height))
                    {
                        BBImage.Draw(bb, new Bgr(Color.Orange), 1);
                        continue;
                    }

                    if ((bb.Y > minYTracking) && ((bb.Y + bb.Height) < maxYTracking))
                    {
                        tracking.addCurrentVehicle(bb);
                    }
                    BBImage.Draw(bb, new Bgr(Color.Yellow), 1);
                }
                else
                {
                    if ((bb.Width * bb.Height) < 7)
                    {
                        BBImage.Draw(bb, new Bgr(Color.AliceBlue), 1);
                        continue;
                    }

                    if ((bb.Y > minYTracking) && ((bb.Y + bb.Height) < maxYTracking))
                    {
                        pairLights.addHeadlight(bb);
                    }
                    BBImage.Draw(bb, new Bgr(Color.Yellow), 1);
                }
            }

            if (!dayScene)
            {
                List<Rectangle> vehiclesBB;
                vehiclesBB = pairLights.createVehiclesBB();

                foreach (Rectangle rect in vehiclesBB)
                {
                    tracking.addCurrentVehicle(rect);
                }

                pairLights.clearList();
            }

        }

        //spajanie urcitych boundingboxov
        private void mergeBoundingBoxes()
        {
            //List<Rectangle> BB = new List<Rectangle>();
            for (int i = boundingBoxes.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < boundingBoxes.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    //ak sa horizontalne prekryvaju
                    if (boundingBoxes[i].Left < boundingBoxes[j].Right && boundingBoxes[j].Left < boundingBoxes[i].Right)
                    {
                        //ak sa aj vertikalne prekryvaju
                        if (boundingBoxes[i].Top < boundingBoxes[j].Bottom && boundingBoxes[j].Top < boundingBoxes[i].Bottom)
                        {
                            Point p1 = new Point(Math.Min(boundingBoxes[i].Left, boundingBoxes[j].Left), Math.Min(boundingBoxes[i].Top, boundingBoxes[j].Top));
                            Point p2 = new Point(Math.Max(boundingBoxes[i].Right, boundingBoxes[j].Right), Math.Max(boundingBoxes[i].Bottom, boundingBoxes[j].Bottom));
                            int height = p2.Y - p1.Y;
                            int width = p2.X - p1.X;
                            Rectangle newBB = new Rectangle(p1.X, p1.Y, width, height);
                            boundingBoxes[j] = newBB;
                            boundingBoxes.RemoveAt(i);
                            break;
                        }

                        int minDist = Math.Min(Math.Abs(boundingBoxes[i].Top - boundingBoxes[j].Bottom),
                                                Math.Abs(boundingBoxes[i].Bottom - boundingBoxes[j].Top));

                        //ak su blizko seba
                        if (minDist < mergeDistance)
                        {
                            Point p1 = new Point(Math.Min(boundingBoxes[i].Left, boundingBoxes[j].Left), Math.Min(boundingBoxes[i].Top, boundingBoxes[j].Top));
                            Point p2 = new Point(Math.Max(boundingBoxes[i].Right, boundingBoxes[j].Right), Math.Max(boundingBoxes[i].Bottom, boundingBoxes[j].Bottom));
                            int height = p2.Y - p1.Y;
                            int width = p2.X - p1.X;
                            Rectangle newBB = new Rectangle(p1.X, p1.Y, width, height);
                            boundingBoxes[j] = newBB;
                            boundingBoxes.RemoveAt(i);
                            break;
                        }

                    }
                }
            }

            //return BB;
        }

        //otvorenie a nacitanie videa 
        private void openFileButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Media Files|*.avi;*.mp4";
            openFileDialog1.FileName = "";
            //initializedVariables = false;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _capture = null;
                    _capture = new Capture(openFileDialog1.FileName);
                    FPS = (int)_capture.GetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FPS);
                    initializedVariables = false;
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

            
            if (daySceneRadioButton.Checked)
            {
                isDayScene = true;
            }
            else
            {
                isDayScene = false;
            }

            if (showTmpImageCheckBox.Checked)
            {
                showTmpImages = true;
            }
            else
            {
                showTmpImages = false;
            }

            if (_capture != null)
            {
                frame = _capture.QueryFrame();
                

                string onlyfilename = openFileDialog1.SafeFileName;
                RoadParamForm roadParam = new RoadParamForm(frame, onlyfilename);

                roadParam.ShowDialog();

                if (roadParam.IsSetAllRoadParam)
                {
                    openFileButton.Enabled = false;
                    roadPoints = roadParam.getRoadPoints();
                    roadDistancePoints = roadParam.getRoadDistancePoints();
                    realDistance = roadParam.getRealDistance();
                    roadLanes = roadParam.createRoadLanes();

                    setStartInitVariables();
                    createStatisticLabels();

                }
            }
        }

        //vytvorenie GUI pre statisticke informacie
        private void createStatisticLabels()
        {

            int x = 512;
            int y = 73;
            int dy = 24;


            for (int i = 1; i <= roadLanes.Count; i++)
            {
                Label laneText = new Label();
                laneText.Location = new Point(x, y);
                laneText.Text = "Pruh č." + i.ToString();
                laneText.AutoSize = true;


                Label laneCars = new Label();
                laneCars.Location = new Point(x + 72, y);
                laneCars.Text = "0";
                laneCars.Name = "lane" + i.ToString() + "Cars";
                laneCars.AutoSize = true;

                Label laneTrucks = new Label();
                laneTrucks.Location = new Point(x + 128, y);
                laneTrucks.Text = "0";
                laneTrucks.Name = "lane" + i.ToString() + "Trucks";
                laneTrucks.AutoSize = true;

                Label laneSum = new Label();
                laneSum.Location = new Point(x + 188, y);
                laneSum.Text = "0";
                laneSum.Name = "lane" + i.ToString() + "Sum";
                laneSum.AutoSize = true;

                this.Controls.Add(laneText);
                this.Controls.Add(laneCars);
                this.Controls.Add(laneTrucks);
                this.Controls.Add(laneSum);
                y += dy;
            }

            Label sumText = new Label();
            sumText.Location = new Point(x, y);
            sumText.Text = "SPOLU";
            sumText.AutoSize = true;


            Label sumCars = new Label();
            sumCars.Location = new Point(x + 72, y);
            sumCars.Text = "0";
            sumCars.Name = "sumCars";
            sumCars.AutoSize = true;

            Label sumTrucks = new Label();
            sumTrucks.Location = new Point(x + 128, y);
            sumTrucks.Text = "0";
            sumTrucks.Name = "sumTrucks";
            sumTrucks.AutoSize = true;

            Label totalSum = new Label();
            totalSum.Location = new Point(x + 188, y);
            totalSum.Text = "0";
            totalSum.Name = "totalSum";
            totalSum.AutoSize = true;

            this.Controls.Add(sumText);
            this.Controls.Add(sumCars);
            this.Controls.Add(sumTrucks);
            this.Controls.Add(totalSum);


        }

        
        private void stopButton_Click(object sender, EventArgs e)
        {
            if (paused)
            {
                stopButton.Text = "Stop";
                paused = false;
                My_Timer.Start();
                openFileButton.Enabled = false;
            }
            else
            {
                stopButton.Text = "Spustiť";
                paused = true;
                My_Timer.Stop();
                openFileButton.Enabled = true;
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.N)
            {
                stopButton_Click(sender, new EventArgs());
            }
        }
    }
}
