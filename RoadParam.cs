using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

      private async void _CloseIfIsSetAllParameters()
      {
         await Task.Delay(100); //0.1 sec
         if (IsSetAllRoadParam)
         {
            Close();
         }
      }

      public void ShowAndClose()
      {
         _CloseIfIsSetAllParameters();
         ShowDialog();
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
         _isSetAllRoadParam = true;
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
            case 5:
               newPoint = new Point((int)(150 * _rate), (int)(47 * _rate));
               _roadPoints.Add(newPoint);
               newPoint = new Point((int)(203 * _rate), (int)(47 * _rate));
               _roadPoints.Add(newPoint);
               newPoint = new Point((int)(267 * _rate), (int)(205 * _rate));
               _roadPoints.Add(newPoint);
               newPoint = new Point((int)(87 * _rate), (int)(205 * _rate));
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
               NumberOfRoadLanes = 2;
               break;
            case 1200:
               newPoint = new Point(103, 35);
               _roadPoints.Add(newPoint);
               newPoint = new Point(177, 35);
               _roadPoints.Add(newPoint);
               newPoint = new Point(229, 110);
               _roadPoints.Add(newPoint);
               newPoint = new Point(55, 110);
               _roadPoints.Add(newPoint);
               NumberOfRoadLanes = 3;
               break;
            case 1201:
               newPoint = new Point(147, 5);
               _roadPoints.Add(newPoint);
               newPoint = new Point(222, 5);
               _roadPoints.Add(newPoint);
               newPoint = new Point(239, 84);
               _roadPoints.Add(newPoint);
               newPoint = new Point(111, 84);
               _roadPoints.Add(newPoint);
               NumberOfRoadLanes = 2;
               break;
            case 1202:
               newPoint = new Point(109, 10);
               _roadPoints.Add(newPoint);
               newPoint = new Point(182, 7);
               _roadPoints.Add(newPoint);
               newPoint = new Point(255, 98);
               _roadPoints.Add(newPoint);
               newPoint = new Point(101, 83);
               _roadPoints.Add(newPoint);
               NumberOfRoadLanes = 2;
               break;
            case 1203:
               newPoint = new Point(143, 4);
               _roadPoints.Add(newPoint);
               newPoint = new Point(231, 4);
               _roadPoints.Add(newPoint);
               newPoint = new Point(276, 97);
               _roadPoints.Add(newPoint);
               newPoint = new Point(115, 97);
               _roadPoints.Add(newPoint);
               NumberOfRoadLanes = 2;
               break;
            case 1204:
               newPoint = new Point(111, 11);
               _roadPoints.Add(newPoint);
               newPoint = new Point(190, 8);
               _roadPoints.Add(newPoint);
               newPoint = new Point(262, 101);
               _roadPoints.Add(newPoint);
               newPoint = new Point(115, 97);
               _roadPoints.Add(newPoint);
               NumberOfRoadLanes = 2;
               break;
            case 1205:
               newPoint = new Point(155, 6);
               _roadPoints.Add(newPoint);
               newPoint = new Point(215, 6);
               _roadPoints.Add(newPoint);
               newPoint = new Point(249, 93);
               _roadPoints.Add(newPoint);
               newPoint = new Point(109, 93);
               _roadPoints.Add(newPoint);
               NumberOfRoadLanes = 2;
               break;
            case 1206:
               newPoint = new Point(210, 10);
               _roadPoints.Add(newPoint);
               newPoint = new Point(301, 11);
               _roadPoints.Add(newPoint);
               newPoint = new Point(290, 86);
               _roadPoints.Add(newPoint);
               newPoint = new Point(117, 74);
               _roadPoints.Add(newPoint);
               _numberOfRoadLanes = 3;
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
               _isSetAllRoadParam = false;
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
            case 5:
               newPoint = new Point((int)(171 * _rate), (int)(98 * _rate));
               _roadDistancePoints.Add(newPoint);
               newPoint = new Point((int)(170 * _rate), (int)(154 * _rate));
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
            case 1200:
               newPoint = new Point(167, 112);
               _roadDistancePoints.Add(newPoint);
               newPoint = new Point(171, 129);
               _roadDistancePoints.Add(newPoint);
               _realDistanceTextBox.Text = "2";
               break;
            case 1201:
               newPoint = new Point(172, 101);
               _roadDistancePoints.Add(newPoint);
               newPoint = new Point(165, 164);
               _roadDistancePoints.Add(newPoint);
               _realDistanceTextBox.Text = "2";
               break;
            case 1202:
               newPoint = new Point(173, 85);
               _roadDistancePoints.Add(newPoint);
               newPoint = new Point(190, 126);
               _roadDistancePoints.Add(newPoint);
               _realDistanceTextBox.Text = "3.4";
               break;
            case 1203:
               newPoint = new Point(195, 67);
               _roadDistancePoints.Add(newPoint);
               newPoint = new Point(198, 93);
               _roadDistancePoints.Add(newPoint);
               _realDistanceTextBox.Text = "2";
               break;
            case 1204:
               newPoint = new Point(170, 59);
               _roadDistancePoints.Add(newPoint);
               newPoint = new Point(181, 87);
               _roadDistancePoints.Add(newPoint);
               _realDistanceTextBox.Text = "1.75";
               break;
            case 1205:
               newPoint = new Point(178, 43);
               _roadDistancePoints.Add(newPoint);
               newPoint = new Point(175, 78);
               _roadDistancePoints.Add(newPoint);
               _realDistanceTextBox.Text = "1.67";
               break;
            case 1206:
               newPoint = new Point(233, 67);
               _roadDistancePoints.Add(newPoint);
               newPoint = new Point(226, 80);
               _roadDistancePoints.Add(newPoint);
               _realDistanceTextBox.Text = "1";
               break;
            default:
               newPoint = new Point(20, 20);
               _roadDistancePoints.Add(newPoint);
               newPoint = new Point(20, _image.Height - 20);
               _roadDistancePoints.Add(newPoint);
               _isSetAllRoadParam = false;
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
         get { return _isSetAllRoadParam && _numberOfRoadLanes != 0; }
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
