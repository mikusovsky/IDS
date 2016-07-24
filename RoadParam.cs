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
using System.Text.RegularExpressions;

namespace IDS
{
    //trieda na zistenie hranic vozovky a referencnej dlzky
    public partial class RoadParamForm : Form
    {
        List<Point> roadPoints = new List<Point>();
        List<Point> roadDistancePoints = new List<Point>();
        double realDistance;
        bool isSetAllRoadParam = false;
        bool setRoadPoints = false;
        int numberOfRoadLanes;
        string fileName;

        Image<Bgr, Byte> image;

        int oldX, oldY;
        int index = -1;
        int objectRadius = 3;

        int videoNumber;
        double resolution;
        double rate;

        public RoadParamForm()
        {
            InitializeComponent();
        }

        public RoadParamForm(Image<Bgr, Byte> frame, string fileName)
        {
            InitializeComponent();
            this.Width = Math.Max(frame.Width + 200, 520);
            this.Height = Math.Max(frame.Height + 50, 300);
            framePictureBox.Width = frame.Width;
            framePictureBox.Height = frame.Height;
            framePictureBox.Top = 5;
            framePictureBox.Left = 5;
            framePictureBox.Image = frame.ToBitmap();
            image = frame;
            this.fileName = fileName;

            isSetAllRoadParam = false;
            setRoadPoints = false;

            saveRoadPointsButton.Enabled = true;
            saveRoadDistancePointsButton.Enabled = false;
            realDistanceTextBox.Enabled = false;

            checkFilename(fileName);

            initPoints();
        }

        //kontrola na meno suboru, ci je to vlastne video alebo nejake nezname
        private void checkFilename(string fileName)
        {
            string text;

            Match match = Regex.Match(fileName, @"video([0-9]+)-([0-9]+).([a-zA-Z0-9]{3})$", RegexOptions.IgnoreCase);

            if (match.Success)
            {
                text = fileName.Substring(5, fileName.Length - 9);
                string[] tmp = text.Split('-');
                videoNumber = Convert.ToInt32(tmp[0]);
                resolution = Convert.ToDouble(tmp[1]);
                rate = resolution / 320;
            }
        }

        //inicializacia bodov
        private void initPoints()
        {
            initRoadPoints();
            initRoadDistancePoints();
        }

        //inicializacia bodov vozovky
        private void initRoadPoints()
        {
            Point newPoint;

            switch (videoNumber)
            {
                case 1:
                    {
                        newPoint = new Point((int)(155 * rate), (int)(71 * rate));
                        roadPoints.Add(newPoint);

                        newPoint = new Point((int)(206 * rate), (int)(71 * rate));
                        roadPoints.Add(newPoint);

                        newPoint = new Point((int)(250 * rate), (int)(206 * rate));
                        roadPoints.Add(newPoint);

                        newPoint = new Point((int)(84 * rate), (int)(206 * rate));
                        roadPoints.Add(newPoint);
                        break;
                    }

                case 2:
                    {
                        newPoint = new Point((int)(128 * rate), (int)(65 * rate));
                        roadPoints.Add(newPoint);

                        newPoint = new Point((int)(189 * rate), (int)(65 * rate));
                        roadPoints.Add(newPoint);

                        newPoint = new Point((int)(250 * rate), (int)(206 * rate));
                        roadPoints.Add(newPoint);

                        newPoint = new Point((int)(72 * rate), (int)(206 * rate));
                        roadPoints.Add(newPoint);
                        break;
                    }

                case 102:
                    {
                        newPoint = new Point((int)(152 * rate), (int)(29 * rate));
                        roadPoints.Add(newPoint);

                        newPoint = new Point((int)(173 * rate), (int)(29 * rate));
                        roadPoints.Add(newPoint);

                        newPoint = new Point((int)(223 * rate), (int)(144 * rate));
                        roadPoints.Add(newPoint);

                        newPoint = new Point((int)(106 * rate), (int)(144 * rate));
                        roadPoints.Add(newPoint);
                        break;
                    }

                case 3:
                case 4:
                case 5:
                case 6:
                    {
                        newPoint = new Point((int)(150 * rate), (int)(47 * rate));
                        roadPoints.Add(newPoint);

                        newPoint = new Point((int)(203 * rate), (int)(47 * rate));
                        roadPoints.Add(newPoint);

                        newPoint = new Point((int)(267 * rate), (int)(205 * rate));
                        roadPoints.Add(newPoint);

                        newPoint = new Point((int)(87 * rate), (int)(205 * rate));
                        roadPoints.Add(newPoint);
                        break;
                    }
                case 7:
                    {
                        newPoint = new Point((int)(138 * rate), (int)(48 * rate));
                        roadPoints.Add(newPoint);

                        newPoint = new Point((int)(180 * rate), (int)(48 * rate));
                        roadPoints.Add(newPoint);

                        newPoint = new Point((int)(239 * rate), (int)(205 * rate));
                        roadPoints.Add(newPoint);

                        newPoint = new Point((int)(81 * rate), (int)(205 * rate));
                        roadPoints.Add(newPoint);
                        break;
                    }

                case 8:
                    {
                        newPoint = new Point((int)(138 * rate), (int)(42 * rate));
                        roadPoints.Add(newPoint);

                        newPoint = new Point((int)(193 * rate), (int)(47 * rate));
                        roadPoints.Add(newPoint);

                        newPoint = new Point((int)(255 * rate), (int)(205 * rate));
                        roadPoints.Add(newPoint);

                        newPoint = new Point((int)(74 * rate), (int)(205 * rate));
                        roadPoints.Add(newPoint);
                        break;
                    }

                case 108:
                    {
                        newPoint = new Point((int)(159 * rate), (int)(29 * rate));
                        roadPoints.Add(newPoint);

                        newPoint = new Point((int)(179 * rate), (int)(29 * rate));
                        roadPoints.Add(newPoint);

                        newPoint = new Point((int)(223 * rate), (int)(144 * rate));
                        roadPoints.Add(newPoint);

                        newPoint = new Point((int)(106 * rate), (int)(144 * rate));
                        roadPoints.Add(newPoint);
                        break;
                    }


                case 50:
                    {
                        newPoint = new Point((int)(99 * rate), (int)(43 * rate));
                        roadPoints.Add(newPoint);

                        newPoint = new Point((int)(155 * rate), (int)(43 * rate));
                        roadPoints.Add(newPoint);

                        newPoint = new Point((int)(245 * rate), (int)(220 * rate));
                        roadPoints.Add(newPoint);

                        newPoint = new Point((int)(43 * rate), (int)(220 * rate));
                        roadPoints.Add(newPoint);

                        break;
                    }
                case 51:
                    {
                        newPoint = new Point((int)(106 * rate), (int)(48 * rate));
                        roadPoints.Add(newPoint);

                        newPoint = new Point((int)(182 * rate), (int)(48 * rate));
                        roadPoints.Add(newPoint);

                        newPoint = new Point((int)(297 * rate), (int)(220 * rate));
                        roadPoints.Add(newPoint);

                        newPoint = new Point((int)(72 * rate), (int)(220 * rate));
                        roadPoints.Add(newPoint);

                        break;
                    }
                case 9:
                    {
                        newPoint = new Point((int)(109 * rate), (int)(122 * rate));
                        roadPoints.Add(newPoint);

                        newPoint = new Point((int)(163 * rate), (int)(122 * rate));
                        roadPoints.Add(newPoint);

                        newPoint = new Point((int)(129 * rate), (int)(205 * rate));
                        roadPoints.Add(newPoint);

                        newPoint = new Point((int)(5 * rate), (int)(205 * rate));
                        roadPoints.Add(newPoint);
                        break;
                    }





                case 90:
                    {
                        numberOfLanesTextBox.Text = "1";
                        newPoint = new Point(159, 16);
                        roadPoints.Add(newPoint);

                        newPoint = new Point(175, 16);
                        roadPoints.Add(newPoint);

                        newPoint = new Point(193, 207);
                        roadPoints.Add(newPoint);

                        newPoint = new Point(104, 207);
                        roadPoints.Add(newPoint);
                        break;
                    }
                case 91:
                    {
                        numberOfLanesTextBox.Text = "1";
                        newPoint = new Point(159, 16);
                        roadPoints.Add(newPoint);

                        newPoint = new Point(173, 16);
                        roadPoints.Add(newPoint);

                        newPoint = new Point(196, 207);
                        roadPoints.Add(newPoint);

                        newPoint = new Point(108, 207);
                        roadPoints.Add(newPoint);
                        break;
                    }
                case 92:
                    {
                        numberOfLanesTextBox.Text = "1";
                        newPoint = new Point(178, 25);
                        roadPoints.Add(newPoint);

                        newPoint = new Point(195, 25);
                        roadPoints.Add(newPoint);

                        newPoint = new Point(203, 207);
                        roadPoints.Add(newPoint);

                        newPoint = new Point(121, 207);
                        roadPoints.Add(newPoint);
                        break;
                    }
                case 93:
                    {
                        numberOfLanesTextBox.Text = "1";
                        newPoint = new Point(163, 16);
                        roadPoints.Add(newPoint);

                        newPoint = new Point(177, 16);
                        roadPoints.Add(newPoint);

                        newPoint = new Point(198, 207);
                        roadPoints.Add(newPoint);

                        newPoint = new Point(114, 207);
                        roadPoints.Add(newPoint);
                        break;
                    }
                case 94:
                    {
                        numberOfLanesTextBox.Text = "1";
                        newPoint = new Point(159, 16);
                        roadPoints.Add(newPoint);

                        newPoint = new Point(175, 16);
                        roadPoints.Add(newPoint);

                        newPoint = new Point(193, 207);
                        roadPoints.Add(newPoint);

                        newPoint = new Point(104, 207);
                        roadPoints.Add(newPoint);
                        break;
                    }

                case 200:
                    {
                        newPoint = new Point((int)(140 * rate), (int)(65 * rate));
                        roadPoints.Add(newPoint);

                        newPoint = new Point((int)(189 * rate), (int)(65 * rate));
                        roadPoints.Add(newPoint);

                        newPoint = new Point((int)(250 * rate), (int)(206 * rate));
                        roadPoints.Add(newPoint);

                        newPoint = new Point((int)(85 * rate), (int)(206 * rate));
                        roadPoints.Add(newPoint);
                        break;
                    }
                case 201:
                    {
                        newPoint = new Point(120, 44);
                        roadPoints.Add(newPoint);

                        newPoint = new Point(178, 44);
                        roadPoints.Add(newPoint);

                        newPoint = new Point(264, 221);
                        roadPoints.Add(newPoint);

                        newPoint = new Point(61, 221);
                        roadPoints.Add(newPoint);
                        break;
                    }
                default:
                    {
                        newPoint = new Point((int)(0.1 * image.Width), (int)(0.1 * image.Height));
                        roadPoints.Add(newPoint);

                        newPoint = new Point((int)(image.Width - (0.1 * image.Width)), (int)(0.1 * image.Height));
                        roadPoints.Add(newPoint);

                        newPoint = new Point((int)(image.Width - (0.1 * image.Width)), (int)(image.Height - (0.1 * image.Height)));
                        roadPoints.Add(newPoint);

                        newPoint = new Point((int)(0.1 * image.Width), (int)(image.Height - (0.1 * image.Height)));
                        roadPoints.Add(newPoint);

                        break;
                    }
            }
        }

        //inicializacia bodov referencnej dlzky
        private void initRoadDistancePoints()
        {
            Point newPoint;

            switch (videoNumber)
            {
                case 1:
                    {
                        newPoint = new Point((int)(176 * rate), (int)(134 * rate));
                        roadDistancePoints.Add(newPoint);

                        newPoint = new Point((int)(173 * rate), (int)(158 * rate));
                        roadDistancePoints.Add(newPoint);

                        realDistanceTextBox.Text = "7";

                        break;
                    }

                case 2:
                    {
                        newPoint = new Point((int)(162 * rate), (int)(108 * rate));
                        roadDistancePoints.Add(newPoint);

                        newPoint = new Point((int)(161 * rate), (int)(165 * rate));
                        roadDistancePoints.Add(newPoint);

                        realDistanceTextBox.Text = "10";
                        break;
                    }
                case 102:
                    {
                        newPoint = new Point((int)(165 * rate), (int)(69 * rate));
                        roadDistancePoints.Add(newPoint);

                        newPoint = new Point((int)(166 * rate), (int)(105 * rate));
                        roadDistancePoints.Add(newPoint);

                        realDistanceTextBox.Text = "10";
                        break;
                    }

                case 3:
                case 4:
                case 5:
                case 6:
                    {
                        newPoint = new Point((int)(171 * rate), (int)(98 * rate));
                        roadDistancePoints.Add(newPoint);

                        newPoint = new Point((int)(170 * rate), (int)(154 * rate));
                        roadDistancePoints.Add(newPoint);

                        realDistanceTextBox.Text = "10";
                        break;
                    }

                case 7:
                    {
                        newPoint = new Point((int)(160 * rate), (int)(107 * rate));
                        roadDistancePoints.Add(newPoint);

                        newPoint = new Point((int)(161 * rate), (int)(166 * rate));
                        roadDistancePoints.Add(newPoint);

                        realDistanceTextBox.Text = "10";
                        break;

                    }

                case 8:
                    {
                        newPoint = new Point((int)(168 * rate), (int)(93 * rate));
                        roadDistancePoints.Add(newPoint);

                        newPoint = new Point((int)(169 * rate), (int)(142 * rate));
                        roadDistancePoints.Add(newPoint);

                        realDistanceTextBox.Text = "10";
                        break;
                    }

                case 108:
                    {
                        newPoint = new Point((int)(163 * rate), (int)(49 * rate));
                        roadDistancePoints.Add(newPoint);

                        newPoint = new Point((int)(164 * rate), (int)(67 * rate));
                        roadDistancePoints.Add(newPoint);

                        realDistanceTextBox.Text = "10";
                        break;
                    }


                case 9:
                    {
                        newPoint = new Point(83, 176);
                        roadDistancePoints.Add(newPoint);

                        newPoint = new Point(69, 194);
                        roadDistancePoints.Add(newPoint);

                        realDistanceTextBox.Text = "6";
                        break;
                    }

                case 50:
                    {
                        newPoint = new Point(128, 117);
                        roadDistancePoints.Add(newPoint);

                        newPoint = new Point(132, 168);
                        roadDistancePoints.Add(newPoint);

                        realDistanceTextBox.Text = "7";

                        break;
                    }
                case 51:
                    {
                        newPoint = new Point(161, 142);
                        roadDistancePoints.Add(newPoint);

                        newPoint = new Point(171, 182);
                        roadDistancePoints.Add(newPoint);

                        realDistanceTextBox.Text = "4,5";

                        break;
                    }

                case 90:
                    {
                        newPoint = new Point(176, 103);
                        roadDistancePoints.Add(newPoint);

                        newPoint = new Point(180, 138);
                        roadDistancePoints.Add(newPoint);

                        realDistanceTextBox.Text = "6,3";
                        break;
                    }
                case 91:
                    {
                        newPoint = new Point(179, 103);
                        roadDistancePoints.Add(newPoint);

                        newPoint = new Point(183, 137);
                        roadDistancePoints.Add(newPoint);

                        realDistanceTextBox.Text = "6,3";
                        break;
                    }
                case 92:
                    {
                        newPoint = new Point(203, 132);
                        roadDistancePoints.Add(newPoint);

                        newPoint = new Point(205, 185);
                        roadDistancePoints.Add(newPoint);

                        realDistanceTextBox.Text = "6,3";
                        break;
                    }
                case 93:
                    {
                        newPoint = new Point(183, 107);
                        roadDistancePoints.Add(newPoint);

                        newPoint = new Point(187, 140);
                        roadDistancePoints.Add(newPoint);

                        realDistanceTextBox.Text = "6,3";
                        break;
                    }
                case 94:
                    {
                        newPoint = new Point(179, 103);
                        roadDistancePoints.Add(newPoint);

                        newPoint = new Point(183, 137);
                        roadDistancePoints.Add(newPoint);

                        realDistanceTextBox.Text = "6,3";
                        break;
                    }

                case 200:
                    {
                        newPoint = new Point((int)(162 * rate), (int)(108 * rate));
                        roadDistancePoints.Add(newPoint);

                        newPoint = new Point((int)(161 * rate), (int)(165 * rate));
                        roadDistancePoints.Add(newPoint);

                        realDistanceTextBox.Text = "10";
                        break;
                    }

                case 201:
                    {
                        newPoint = new Point(83, 176);
                        roadDistancePoints.Add(newPoint);

                        newPoint = new Point(69, 194);
                        roadDistancePoints.Add(newPoint);

                        realDistanceTextBox.Text = "6";
                        break;
                    }
                default:
                    {
                        newPoint = new Point(20, 20);
                        roadDistancePoints.Add(newPoint);

                        newPoint = new Point(20, image.Height - 20);
                        roadDistancePoints.Add(newPoint);

                        break;
                    }
            }
        }

        //vypocet vzdialenosti 2 bodov
        private int FindDistanceToPointSquared(Point pt1, Point pt2)
        {
            int dx = pt1.X - pt2.X;
            int dy = pt1.Y - pt2.Y;
            return dx * dx + dy * dy;
        }

        //kreslenie
        private void framePictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (!setRoadPoints)
            {
                drawRoadPoints(e);
            }
            else
            {
                drawRoadDistancePoints(e);
            }
        }

        //kreslenie bodov vozovky
        private void drawRoadPoints(PaintEventArgs e)
        {
            Pen myPen = new Pen(Color.Yellow, 2);
            e.Graphics.DrawPolygon(myPen, roadPoints.ToArray());

            foreach (Point corner in roadPoints)
            {
                drawCorner(e, corner);
            }
        }

        //kreslenie rohov
        private void drawCorner(PaintEventArgs e, Point corner)
        {
            Rectangle rect = new Rectangle(
                corner.X - objectRadius, corner.Y - objectRadius,
                2 * objectRadius + 1, 2 * objectRadius + 1);
            e.Graphics.FillEllipse(Brushes.White, rect);
            e.Graphics.DrawEllipse(Pens.Black, rect);
        }

        //kreslenie bodov referencnej dlzky
        private void drawRoadDistancePoints(PaintEventArgs e)
        {
            Pen myPen = new Pen(Color.Yellow, 2);
            e.Graphics.DrawLine(myPen, roadDistancePoints[0], roadDistancePoints[1]);

            foreach (Point corner in roadDistancePoints)
            {
                drawCorner(e, corner);
            }
        }

        //najdenie oznaceneho bodu
        private int findSelectPoint(List<Point> points, Point mousePoint)
        {
            for (int i = 0; i < points.Count; i++)
            {
                if (FindDistanceToPointSquared(points[i], mousePoint) < 15)
                {
                    return i;
                }
            }

            return -1;
        }

        private void framePictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && index != -1)
            {
                int dX = oldX - e.X;
                int dY = oldY - e.Y;

                Point tmp = new Point(oldX - dX, oldY - dY);
                if (!setRoadPoints)
                {
                    roadPoints[index] = tmp;
                }
                else
                {
                    roadDistancePoints[index] = tmp;
                }
            }
            framePictureBox.Invalidate();
        }

        private void framePictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            List<Point> points;
            if (!setRoadPoints)
            {
                points = roadPoints;
            }
            else
            {
                points = roadDistancePoints;
            }

            index = findSelectPoint(points, e.Location);
            if (index != -1)
            {
                oldX = points[index].X;
                oldY = points[index].Y;
            }
        }

        private void framePictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            index = -1;
        }

        private void saveRoadPointsButton_Click(object sender, EventArgs e)
        {
            if (int.TryParse(numberOfLanesTextBox.Text, out numberOfRoadLanes))
            {
                setRoadPoints = true;
                saveRoadPointsButton.Enabled = false;
                numberOfLanesTextBox.Enabled = false;
                saveRoadDistancePointsButton.Enabled = true;
                realDistanceTextBox.Enabled = true;
                framePictureBox.Invalidate();
            }
            else
            {
                MessageBox.Show("Zle zadané číslo počtu pruhov");
                numberOfLanesTextBox.Text = "";
            }
        }

        private void saveRoadDistancePointsButton_Click(object sender, EventArgs e)
        {
            if (double.TryParse(realDistanceTextBox.Text, out realDistance))
            {
                isSetAllRoadParam = true;
                this.Close();
                double tmp = Math.Sqrt(Math.Pow(roadDistancePoints[0].X - roadDistancePoints[1].X, 2)
                    + Math.Pow(roadDistancePoints[0].Y - roadDistancePoints[1].Y, 2));
            }
            else
            {
                MessageBox.Show("Zle zadané číslo vzdialenosti");
                realDistanceTextBox.Text = "";
            }
        }

        //vytvorenie jednotlivych jazdnych pruhov
        public List<RoadLane> createRoadLanes()
        {
            int topRoadLaneWidth = (roadPoints[1].X - roadPoints[0].X) / numberOfRoadLanes;
            int bottomRoadLaneWidth = (roadPoints[2].X - roadPoints[3].X) / numberOfRoadLanes;
            Point lt, rt, lb, rb;
            List<RoadLane> roadLanes = new List<RoadLane>();
            for (int i = 0; i < numberOfRoadLanes; i++)
            {
                lt = new Point(roadPoints[0].X + i * topRoadLaneWidth, roadPoints[0].Y);
                rt = new Point(roadPoints[0].X + (i + 1) * topRoadLaneWidth, roadPoints[0].Y);
                lb = new Point(roadPoints[3].X + i * bottomRoadLaneWidth, roadPoints[3].Y);
                rb = new Point(roadPoints[3].X + (i + 1) * bottomRoadLaneWidth, roadPoints[3].Y);
                roadLanes.Add(new RoadLane(lt, rt, lb, rb));
            }

            return roadLanes;
        }

        //usporiadanie bodov, aby sa s nimi dalo pracovat ako s polygonom
        private List<Point> sortRoadPoints()
        {
            List<Point> newList = new List<Point>();
            newList.Add(roadPoints[0]);
            newList.Add(roadPoints[1]);
            newList.Add(roadPoints[3]);
            newList.Add(roadPoints[2]);

            return newList;
        }

        public bool IsSetAllRoadParam
        {
            get { return isSetAllRoadParam; }
        }
        
        public int NumberOfRoadLanes
        {
            get { return numberOfRoadLanes; }
            set { this.numberOfRoadLanes = value; }
        }

        public List<Point> getRoadPoints()
        {
            return sortRoadPoints();
        }

        public List<Point> getRoadDistancePoints()
        {
            return roadDistancePoints;
        }

        public double getRealDistance()
        {
            return realDistance;
        }
    }
}
