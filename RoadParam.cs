using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Text.RegularExpressions;
using IDS.IDS;

namespace IDS
{
   //trieda na zistenie hranic vozovky a referencnej dlzky
   public partial class RoadParamForm : Form
   {
      List<Point> _roadPoints = new List<Point>();
      List<Point> _roadDistancePoints = new List<Point>();
      double _realDistance;
      bool _isSetAllRoadParam = false;
      bool _setRoadPoints = false;
      int _numberOfRoadLanes;
      string _fileName;

      Image<Bgr, Byte> _image;

      int _oldX, _oldY;
      int _index = -1;
      int _objectRadius = 3;

      int _videoNumber;
      double _resolution;
      double _rate;

      public RoadParamForm()
      {
         InitializeComponent();
      }

      public RoadParamForm(Image<Bgr, Byte> frame, string fileName)
      {
         InitializeComponent();
         Width = Math.Max(frame.Width + 200, 520);
         Height = Math.Max(frame.Height + 50, 300);
         _framePictureBox.Width = frame.Width;
         _framePictureBox.Height = frame.Height;
         _framePictureBox.Top = 5;
         _framePictureBox.Left = 5;
         _framePictureBox.Image = frame.ToBitmap();
         _image = frame;
         _fileName = fileName;

         _isSetAllRoadParam = false;
         _setRoadPoints = false;

         _saveRoadPointsButton.Enabled = true;
         _saveRoadDistancePointsButton.Enabled = false;
         _realDistanceTextBox.Enabled = false;

         _CheckFilename(fileName);

         _InitPoints();
      }

      //kontrola na meno suboru, ci je to vlastne video alebo nejake nezname
      private void _CheckFilename(string fileName)
      {
         string text;

         Match match = Regex.Match(fileName, @"video([0-9]+)-([0-9]+).([a-zA-Z0-9]{3})$", RegexOptions.IgnoreCase);

         if (match.Success)
         {
            text = fileName.Substring(5, fileName.Length - 9);
            string[] tmp = text.Split('-');
            _videoNumber = Convert.ToInt32(tmp[0]);
            _resolution = Convert.ToDouble(tmp[1]);
            _rate = _resolution / 320;
         }
      }

      //inicializacia bodov
      private void _InitPoints()
      {
         _InitRoadPoints();
         _InitRoadDistancePoints();
      }

      //inicializacia bodov vozovky
      private void _InitRoadPoints()
      {
         Point newPoint;

         switch (_videoNumber)
         {
            case 1:
               newPoint = new Point((int)(155 * _rate), (int)(71 * _rate));
               _roadPoints.Add(newPoint);
               newPoint = new Point((int)(206 * _rate), (int)(71 * _rate));
               _roadPoints.Add(newPoint);
               newPoint = new Point((int)(250 * _rate), (int)(206 * _rate));
               _roadPoints.Add(newPoint);
               newPoint = new Point((int)(84 * _rate), (int)(206 * _rate));
               _roadPoints.Add(newPoint);
               break;
            case 2:
               newPoint = new Point((int)(128 * _rate), (int)(65 * _rate));
               _roadPoints.Add(newPoint);
               newPoint = new Point((int)(189 * _rate), (int)(65 * _rate));
               _roadPoints.Add(newPoint);
               newPoint = new Point((int)(250 * _rate), (int)(206 * _rate));
               _roadPoints.Add(newPoint);
               newPoint = new Point((int)(72 * _rate), (int)(206 * _rate));
               _roadPoints.Add(newPoint);
               break;
            case 102:
               newPoint = new Point((int)(152 * _rate), (int)(29 * _rate));
               _roadPoints.Add(newPoint);
               newPoint = new Point((int)(173 * _rate), (int)(29 * _rate));
               _roadPoints.Add(newPoint);
               newPoint = new Point((int)(223 * _rate), (int)(144 * _rate));
               _roadPoints.Add(newPoint);
               newPoint = new Point((int)(106 * _rate), (int)(144 * _rate));
               _roadPoints.Add(newPoint);
               break;
            case 3:
            case 4:
            case 5:
            case 6:
               newPoint = new Point((int)(150 * _rate), (int)(47 * _rate));
               _roadPoints.Add(newPoint);
               newPoint = new Point((int)(203 * _rate), (int)(47 * _rate));
               _roadPoints.Add(newPoint);
               newPoint = new Point((int)(267 * _rate), (int)(205 * _rate));
               _roadPoints.Add(newPoint);
               newPoint = new Point((int)(87 * _rate), (int)(205 * _rate));
               _roadPoints.Add(newPoint);
               break;
            case 7:
               newPoint = new Point((int)(138 * _rate), (int)(48 * _rate));
               _roadPoints.Add(newPoint);
               newPoint = new Point((int)(180 * _rate), (int)(48 * _rate));
               _roadPoints.Add(newPoint);
               newPoint = new Point((int)(239 * _rate), (int)(205 * _rate));
               _roadPoints.Add(newPoint);
               newPoint = new Point((int)(81 * _rate), (int)(205 * _rate));
               _roadPoints.Add(newPoint);
               break;
            case 8:
               newPoint = new Point((int)(138 * _rate), (int)(42 * _rate));
               _roadPoints.Add(newPoint);
               newPoint = new Point((int)(193 * _rate), (int)(47 * _rate));
               _roadPoints.Add(newPoint);
               newPoint = new Point((int)(255 * _rate), (int)(205 * _rate));
               _roadPoints.Add(newPoint);
               newPoint = new Point((int)(74 * _rate), (int)(205 * _rate));
               _roadPoints.Add(newPoint);
               break;
            case 108:
               newPoint = new Point((int)(159 * _rate), (int)(29 * _rate));
               _roadPoints.Add(newPoint);
               newPoint = new Point((int)(179 * _rate), (int)(29 * _rate));
               _roadPoints.Add(newPoint);
               newPoint = new Point((int)(223 * _rate), (int)(144 * _rate));
               _roadPoints.Add(newPoint);
               newPoint = new Point((int)(106 * _rate), (int)(144 * _rate));
               _roadPoints.Add(newPoint);
               break;
            case 50:
               newPoint = new Point((int)(99 * _rate), (int)(43 * _rate));
               _roadPoints.Add(newPoint);
               newPoint = new Point((int)(155 * _rate), (int)(43 * _rate));
               _roadPoints.Add(newPoint);
               newPoint = new Point((int)(245 * _rate), (int)(220 * _rate));
               _roadPoints.Add(newPoint);
               newPoint = new Point((int)(43 * _rate), (int)(220 * _rate));
               _roadPoints.Add(newPoint);
               break;
            case 51:
               newPoint = new Point((int)(106 * _rate), (int)(48 * _rate));
               _roadPoints.Add(newPoint);
               newPoint = new Point((int)(182 * _rate), (int)(48 * _rate));
               _roadPoints.Add(newPoint);
               newPoint = new Point((int)(297 * _rate), (int)(220 * _rate));
               _roadPoints.Add(newPoint);
               newPoint = new Point((int)(72 * _rate), (int)(220 * _rate));
               _roadPoints.Add(newPoint);
               break;
            case 9:
               newPoint = new Point((int)(109 * _rate), (int)(122 * _rate));
               _roadPoints.Add(newPoint);
               newPoint = new Point((int)(163 * _rate), (int)(122 * _rate));
               _roadPoints.Add(newPoint);
               newPoint = new Point((int)(129 * _rate), (int)(205 * _rate));
               _roadPoints.Add(newPoint);
               newPoint = new Point((int)(5 * _rate), (int)(205 * _rate));
               _roadPoints.Add(newPoint);
               break;
            case 90:
               _numberOfLanesTextBox.Text = "1";
               newPoint = new Point(159, 16);
               _roadPoints.Add(newPoint);
               newPoint = new Point(175, 16);
               _roadPoints.Add(newPoint);
               newPoint = new Point(193, 207);
               _roadPoints.Add(newPoint);
               newPoint = new Point(104, 207);
               _roadPoints.Add(newPoint);
               break;
            case 91:
               _numberOfLanesTextBox.Text = "1";
               newPoint = new Point(159, 16);
               _roadPoints.Add(newPoint);
               newPoint = new Point(173, 16);
               _roadPoints.Add(newPoint);
               newPoint = new Point(196, 207);
               _roadPoints.Add(newPoint);
               newPoint = new Point(108, 207);
               _roadPoints.Add(newPoint);
               break;
            case 92:
               _numberOfLanesTextBox.Text = "1";
               newPoint = new Point(178, 25);
               _roadPoints.Add(newPoint);
               newPoint = new Point(195, 25);
               _roadPoints.Add(newPoint);
               newPoint = new Point(203, 207);
               _roadPoints.Add(newPoint);
               newPoint = new Point(121, 207);
               _roadPoints.Add(newPoint);
               break;
            case 93:
               _numberOfLanesTextBox.Text = "1";
               newPoint = new Point(163, 16);
               _roadPoints.Add(newPoint);
               newPoint = new Point(177, 16);
               _roadPoints.Add(newPoint);
               newPoint = new Point(198, 207);
               _roadPoints.Add(newPoint);
               newPoint = new Point(114, 207);
               _roadPoints.Add(newPoint);
               break;
            case 94:
               _numberOfLanesTextBox.Text = "1";
               newPoint = new Point(159, 16);
               _roadPoints.Add(newPoint);
               newPoint = new Point(175, 16);
               _roadPoints.Add(newPoint);
               newPoint = new Point(193, 207);
               _roadPoints.Add(newPoint);
               newPoint = new Point(104, 207);
               _roadPoints.Add(newPoint);
               break;
            case 200:
               newPoint = new Point((int)(140 * _rate), (int)(65 * _rate));
               _roadPoints.Add(newPoint);
               newPoint = new Point((int)(189 * _rate), (int)(65 * _rate));
               _roadPoints.Add(newPoint);
               newPoint = new Point((int)(250 * _rate), (int)(206 * _rate));
               _roadPoints.Add(newPoint);
               newPoint = new Point((int)(85 * _rate), (int)(206 * _rate));
               _roadPoints.Add(newPoint);
               break;
            case 201:
               newPoint = new Point(120, 44);
               _roadPoints.Add(newPoint);
               newPoint = new Point(178, 44);
               _roadPoints.Add(newPoint);
               newPoint = new Point(264, 221);
               _roadPoints.Add(newPoint);
               newPoint = new Point(61, 221);
               _roadPoints.Add(newPoint);
               break;
            case 1047:
            case 1048:
            case 1049:
            case 1051:
            case 1052:
            case 1053:
            case 1054:
            case 1055:
            case 1056:
            case 1057:
            case 1058:
            case 1059:
            case 1060:
            case 1061:
            case 1062:
            case 1063:
            case 1064:
            case 1065:
            case 1066:
            case 1067:
            case 1068:
            case 1069:
            case 1070:
               newPoint = new Point(76, 57);
               _roadPoints.Add(newPoint);
               newPoint = new Point(147, 57);
               _roadPoints.Add(newPoint);
               newPoint = new Point(193, 161);
               _roadPoints.Add(newPoint);
               newPoint = new Point(32, 161);
               _roadPoints.Add(newPoint);
               break;
            default:
               newPoint = new Point((int)(0.1 * _image.Width), (int)(0.1 * _image.Height));
               _roadPoints.Add(newPoint);
               newPoint = new Point((int)(_image.Width - (0.1 * _image.Width)), (int)(0.1 * _image.Height));
               _roadPoints.Add(newPoint);
               newPoint = new Point((int)(_image.Width - (0.1 * _image.Width)), (int)(_image.Height - (0.1 * _image.Height)));
               _roadPoints.Add(newPoint);
               newPoint = new Point((int)(0.1 * _image.Width), (int)(_image.Height - (0.1 * _image.Height)));
               _roadPoints.Add(newPoint);
               break;
         }
      }

      //inicializacia bodov referencnej dlzky
      private void _InitRoadDistancePoints()
      {
         Point newPoint;

         switch (_videoNumber)
         {
            case 1:
               newPoint = new Point((int)(176 * _rate), (int)(134 * _rate));
               _roadDistancePoints.Add(newPoint);
               newPoint = new Point((int)(173 * _rate), (int)(158 * _rate));
               _roadDistancePoints.Add(newPoint);
               _realDistanceTextBox.Text = "7";
               break;
            case 2:
               newPoint = new Point((int)(162 * _rate), (int)(108 * _rate));
               _roadDistancePoints.Add(newPoint);
               newPoint = new Point((int)(161 * _rate), (int)(165 * _rate));
               _roadDistancePoints.Add(newPoint);
               _realDistanceTextBox.Text = "10";
               break;
            case 102:
               newPoint = new Point((int)(165 * _rate), (int)(69 * _rate));
               _roadDistancePoints.Add(newPoint);
               newPoint = new Point((int)(166 * _rate), (int)(105 * _rate));
               _roadDistancePoints.Add(newPoint);
               _realDistanceTextBox.Text = "10";
               break;
            case 3:
            case 4:
            case 5:
            case 6:
               newPoint = new Point((int)(171 * _rate), (int)(98 * _rate));
               _roadDistancePoints.Add(newPoint);
               newPoint = new Point((int)(170 * _rate), (int)(154 * _rate));
               _roadDistancePoints.Add(newPoint);
               _realDistanceTextBox.Text = "10";
               break;
            case 7:
               newPoint = new Point((int)(160 * _rate), (int)(107 * _rate));
               _roadDistancePoints.Add(newPoint);
               newPoint = new Point((int)(161 * _rate), (int)(166 * _rate));
               _roadDistancePoints.Add(newPoint);
               _realDistanceTextBox.Text = "10";
               break;
            case 8:
               newPoint = new Point((int)(168 * _rate), (int)(93 * _rate));
               _roadDistancePoints.Add(newPoint);
               newPoint = new Point((int)(169 * _rate), (int)(142 * _rate));
               _roadDistancePoints.Add(newPoint);
               _realDistanceTextBox.Text = "10";
               break;
            case 108:
               newPoint = new Point((int)(163 * _rate), (int)(49 * _rate));
               _roadDistancePoints.Add(newPoint);
               newPoint = new Point((int)(164 * _rate), (int)(67 * _rate));
               _roadDistancePoints.Add(newPoint);
               _realDistanceTextBox.Text = "10";
               break;
            case 9:
               newPoint = new Point(83, 176);
               _roadDistancePoints.Add(newPoint);
               newPoint = new Point(69, 194);
               _roadDistancePoints.Add(newPoint);
               _realDistanceTextBox.Text = "6";
               break;
            case 50:
               newPoint = new Point(128, 117);
               _roadDistancePoints.Add(newPoint);
               newPoint = new Point(132, 168);
               _roadDistancePoints.Add(newPoint);
               _realDistanceTextBox.Text = "7";
               break;
            case 51:
               newPoint = new Point(161, 142);
               _roadDistancePoints.Add(newPoint);
               newPoint = new Point(171, 182);
               _roadDistancePoints.Add(newPoint);
               _realDistanceTextBox.Text = "4.5";
               break;
            case 90:
               newPoint = new Point(176, 103);
               _roadDistancePoints.Add(newPoint);
               newPoint = new Point(180, 138);
               _roadDistancePoints.Add(newPoint);
               _realDistanceTextBox.Text = "6.3";
               break;
            case 91:
               newPoint = new Point(179, 103);
               _roadDistancePoints.Add(newPoint);
               newPoint = new Point(183, 137);
               _roadDistancePoints.Add(newPoint);
               _realDistanceTextBox.Text = "6.3";
               break;
            case 92:
               newPoint = new Point(203, 132);
               _roadDistancePoints.Add(newPoint);
               newPoint = new Point(205, 185);
               _roadDistancePoints.Add(newPoint);
               _realDistanceTextBox.Text = "6.3";
               break;
            case 93:
               newPoint = new Point(183, 107);
               _roadDistancePoints.Add(newPoint);
               newPoint = new Point(187, 140);
               _roadDistancePoints.Add(newPoint);
               _realDistanceTextBox.Text = "6.3";
               break;
            case 94:
               newPoint = new Point(179, 103);
               _roadDistancePoints.Add(newPoint);
               newPoint = new Point(183, 137);
               _roadDistancePoints.Add(newPoint);
               _realDistanceTextBox.Text = "6.3";
               break;
            case 200:
               newPoint = new Point((int)(162 * _rate), (int)(108 * _rate));
               _roadDistancePoints.Add(newPoint);
               newPoint = new Point((int)(161 * _rate), (int)(165 * _rate));
               _roadDistancePoints.Add(newPoint);
               _realDistanceTextBox.Text = "10";
               break;
            case 201:
               newPoint = new Point(83, 176);
               _roadDistancePoints.Add(newPoint);
               newPoint = new Point(69, 194);
               _roadDistancePoints.Add(newPoint);
               _realDistanceTextBox.Text = "6";
               break;
            case 1047:
            case 1048:
            case 1049:
            case 1051:
            case 1052:
            case 1053:
            case 1054:
            case 1055:
            case 1056:
            case 1057:
            case 1058:
            case 1059:
            case 1060:
            case 1061:
            case 1062:
            case 1063:
            case 1064:
            case 1065:
            case 1066:
            case 1067:
            case 1068:
            case 1069:
            case 1070:
               newPoint = new Point(111, 114);
               _roadDistancePoints.Add(newPoint);
               newPoint = new Point(111, 135);
               _roadDistancePoints.Add(newPoint);
               _realDistanceTextBox.Text = "2";
               break;
            default:
               newPoint = new Point(20, 20);
               _roadDistancePoints.Add(newPoint);
               newPoint = new Point(20, _image.Height - 20);
               _roadDistancePoints.Add(newPoint);
               break;
         }
      }

      //vypocet vzdialenosti 2 bodov
      private int _FindDistanceToPointSquared(Point pt1, Point pt2)
      {
         int dx = pt1.X - pt2.X;
         int dy = pt1.Y - pt2.Y;
         return dx * dx + dy * dy;
      }

      //kreslenie
      private void _FramePictureBox_Paint(object sender, PaintEventArgs e)
      {
         if (!_setRoadPoints)
         {
            _DrawRoadPoints(e);
         }
         else
         {
            _DrawRoadDistancePoints(e);
         }
      }

      //kreslenie bodov vozovky
      private void _DrawRoadPoints(PaintEventArgs e)
      {
         Pen myPen = new Pen(Color.Yellow, 2);
         e.Graphics.DrawPolygon(myPen, _roadPoints.ToArray());

         foreach (Point corner in _roadPoints)
         {
            _DrawCorner(e, corner);
         }
      }

      //kreslenie rohov
      private void _DrawCorner(PaintEventArgs e, Point corner)
      {
         Rectangle rect = new Rectangle(
             corner.X - _objectRadius, corner.Y - _objectRadius,
             2 * _objectRadius + 1, 2 * _objectRadius + 1);
         e.Graphics.FillEllipse(Brushes.White, rect);
         e.Graphics.DrawEllipse(Pens.Black, rect);
      }

      //kreslenie bodov referencnej dlzky
      private void _DrawRoadDistancePoints(PaintEventArgs e)
      {
         Pen myPen = new Pen(Color.Yellow, 2);
         e.Graphics.DrawLine(myPen, _roadDistancePoints[0], _roadDistancePoints[1]);

         foreach (Point corner in _roadDistancePoints)
         {
            _DrawCorner(e, corner);
         }
      }

      //najdenie oznaceneho bodu
      private int _FindSelectPoint(List<Point> points, Point mousePoint)
      {
         for (int i = 0; i < points.Count; i++)
         {
            if (_FindDistanceToPointSquared(points[i], mousePoint) < 15)
            {
               return i;
            }
         }

         return -1;
      }

      private void _FramePictureBox_MouseMove(object sender, MouseEventArgs e)
      {
         if (e.Button == MouseButtons.Left && _index != -1)
         {
            int dX = _oldX - e.X;
            int dY = _oldY - e.Y;

            Point tmp = new Point(_oldX - dX, _oldY - dY);
            if (!_setRoadPoints)
            {
               _roadPoints[_index] = tmp;
            }
            else
            {
               _roadDistancePoints[_index] = tmp;
            }
         }
         _framePictureBox.Invalidate();
      }

      private void FramePictureBox_MouseDown(object sender, MouseEventArgs e)
      {
         List<Point> points;
         if (!_setRoadPoints)
         {
            points = _roadPoints;
         }
         else
         {
            points = _roadDistancePoints;
         }

         _index = _FindSelectPoint(points, e.Location);
         if (_index != -1)
         {
            _oldX = points[_index].X;
            _oldY = points[_index].Y;
         }
      }

      private void FramePictureBox_MouseUp(object sender, MouseEventArgs e)
      {
         _index = -1;
      }

      private void SaveRoadPointsButton_Click(object sender, EventArgs e)
      {
         if (int.TryParse(_numberOfLanesTextBox.Text, out _numberOfRoadLanes))
         {
            _setRoadPoints = true;
            _saveRoadPointsButton.Enabled = false;
            _numberOfLanesTextBox.Enabled = false;
            _saveRoadDistancePointsButton.Enabled = true;
            _realDistanceTextBox.Enabled = true;
            _framePictureBox.Invalidate();
         }
         else
         {
            MessageBox.Show(Resources.WrongInputNumerForCountOfRoudLines);
            _numberOfLanesTextBox.Text = "";
         }
      }

      private void SaveRoadDistancePointsButton_Click(object sender, EventArgs e)
      {
         if (double.TryParse(_realDistanceTextBox.Text, out _realDistance))
         {
            _isSetAllRoadParam = true;
            this.Close();
            double tmp = Math.Sqrt(Math.Pow(_roadDistancePoints[0].X - _roadDistancePoints[1].X, 2)
                + Math.Pow(_roadDistancePoints[0].Y - _roadDistancePoints[1].Y, 2));
         }
         else
         {
            MessageBox.Show(Resources.WrongNumberFormatForDistinct);
            _realDistanceTextBox.Text = "";
         }
      }

      //vytvorenie jednotlivych jazdnych pruhov
      public List<RoadLane> CreateRoadLanes()
      {
         int topRoadLaneWidth = (_roadPoints[1].X - _roadPoints[0].X) / _numberOfRoadLanes;
         int bottomRoadLaneWidth = (_roadPoints[2].X - _roadPoints[3].X) / _numberOfRoadLanes;
         Point lt, rt, lb, rb;
         List<RoadLane> roadLanes = new List<RoadLane>();
         for (int i = 0; i < _numberOfRoadLanes; i++)
         {
            lt = new Point(_roadPoints[0].X + i * topRoadLaneWidth, _roadPoints[0].Y);
            rt = new Point(_roadPoints[0].X + (i + 1) * topRoadLaneWidth, _roadPoints[0].Y);
            lb = new Point(_roadPoints[3].X + i * bottomRoadLaneWidth, _roadPoints[3].Y);
            rb = new Point(_roadPoints[3].X + (i + 1) * bottomRoadLaneWidth, _roadPoints[3].Y);
            roadLanes.Add(new RoadLane(lt, rt, lb, rb));
         }

         return roadLanes;
      }

      //usporiadanie bodov, aby sa s nimi dalo pracovat ako s polygonom
      private List<Point> SortRoadPoints()
      {
         List<Point> newList = new List<Point>();
         newList.Add(_roadPoints[0]);
         newList.Add(_roadPoints[1]);
         newList.Add(_roadPoints[3]);
         newList.Add(_roadPoints[2]);

         return newList;
      }

      public bool IsSetAllRoadParam
      {
         get { return _isSetAllRoadParam; }
      }

      public int NumberOfRoadLanes
      {
         get { return _numberOfRoadLanes; }
         set { this._numberOfRoadLanes = value; }
      }

      public List<Point> GetRoadPoints()
      {
         return SortRoadPoints();
      }

      public List<Point> GetRoadDistancePoints()
      {
         return _roadDistancePoints;
      }

      public double GetRealDistance()
      {
         return _realDistance;
      }
   }
}
