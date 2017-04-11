using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Structure;

namespace IDS.IDS
{
   public partial class MaskParamFrom : Form
   {
      private PictureBox pictureBox1;
      private Button buttonOk;
      Point[] m_maskPoints = new Point[4];

      int _oldX, _oldY;
      int _index = -1;
      int _objectRadius = 3;

      int _videoNumber;
      double _resolution;
      double _rate;

      private bool _firstSet = true;
      
      public MaskParamFrom(Image<Bgr, byte> frame)
      {
         InitializeComponent();
         Width = Math.Max(frame.Width + 200, 520);
         Height = Math.Max(frame.Height + 50, 300);
         pictureBox1.Width = frame.Width;
         pictureBox1.Height = frame.Height;
         pictureBox1.Top = 5;
         pictureBox1.Left = 5;
         pictureBox1.Image = frame.ToBitmap();
      }
      public MaskParamFrom(Image<Gray, byte> frame)
      {
         InitializeComponent();
         Width = Math.Max(frame.Width + 200, 520);
         Height = Math.Max(frame.Height + 50, 300);
         pictureBox1.Width = frame.Width;
         pictureBox1.Height = frame.Height;
         pictureBox1.Top = 5;
         pictureBox1.Left = 5;
         pictureBox1.Image = frame.ToBitmap();
      }

      public List<Point> GetRoadMaskPoints()
      {
         return m_maskPoints.ToList();
      }

      public int[] GetMaskSize()
      {
         return new int[] { Math.Abs(m_maskPoints[0].X - m_maskPoints[3].X), Math.Abs(m_maskPoints[0].Y - m_maskPoints[1].Y) };
      }

      private void InitializeComponent()
      {
         this.pictureBox1 = new System.Windows.Forms.PictureBox();
         this.buttonOk = new System.Windows.Forms.Button();
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
         this.SuspendLayout();
         // 
         // pictureBox1
         // 
         this.pictureBox1.Location = new System.Drawing.Point(0, 0);
         this.pictureBox1.Name = "PictureObx";
         this.pictureBox1.Size = new System.Drawing.Size(309, 234);
         this.pictureBox1.TabIndex = 0;
         this.pictureBox1.TabStop = false;
         this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this._FramePictureBox_Paint);
         this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FramePictureBox_MouseDown);
         this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this._FramePictureBox_MouseMove);
         this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FramePictureBox_MouseUp);
         // 
         // button1
         // 
         this.buttonOk.Location = new System.Drawing.Point(234, 240);
         this.buttonOk.Name = "buttonOk";
         this.buttonOk.Size = new System.Drawing.Size(75, 23);
         this.buttonOk.TabIndex = 1;
         this.buttonOk.Text = Resources.Ok;
         this.buttonOk.UseVisualStyleBackColor = true;
         this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
         // 
         // MaskParamFrom
         // 
         this.ClientSize = new System.Drawing.Size(321, 268);
         this.Controls.Add(this.buttonOk);
         this.Controls.Add(this.pictureBox1);
         this.Name = "MaskParamFrom";
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
         this.ResumeLayout(false);

         m_maskPoints[0] = new Point(10, 10);
         m_maskPoints[1] = new Point(90, 10);
         m_maskPoints[2] = new Point(90, 50);
         m_maskPoints[3] = new Point(10, 50);
      }

      private void buttonOk_Click(object sender, System.EventArgs e)
      {
         this.Close();
      }

      private void _FramePictureBox_Paint(object sender, PaintEventArgs e)
      {
         _DrawRoadPoints(e);
      }

      private void FramePictureBox_MouseDown(object sender, MouseEventArgs e)
      {
         _index = _FindSelectPoint(e.Location);
         if (_index != -1)
         {
            _oldX = m_maskPoints[_index].X;
            _oldY = m_maskPoints[_index].Y;
         }
      }

      private void _FramePictureBox_MouseMove(object sender, MouseEventArgs e)
      {
         if (e.Button == MouseButtons.Left && _index != -1)
         {
            int dX = _oldX - e.X;
            int dY = _oldY - e.Y;

            Point tmp = new Point(_oldX - dX, _oldY - dY);
            m_maskPoints[_index] = tmp;
            if (_index % 2 == 0)
            {
               m_maskPoints[(_index + 1) % 4].X = tmp.X;
               int mod = (_index - 1) % 4;
               m_maskPoints[mod < 0 ? 3 : mod].Y = tmp.Y;
            }
            else
            {
               m_maskPoints[(_index + 1) % 4].Y = tmp.Y;
               int mod = (_index - 1) % 4;
               m_maskPoints[mod < 0 ? 3 : mod].X = tmp.X;
            }

            if (_firstSet)
            {
               for (int i = _index + 1; i % 4 != _index; i++)
               {
                  if (i % 2 == 0)
                  {
                     m_maskPoints[(i + 1) % 4].X = tmp.X;
                     int mod = (i - 1) % 4;
                     m_maskPoints[mod < 0 ? 3 : mod].Y = tmp.Y;
                  }
                  else
                  {
                     m_maskPoints[(i + 1) % 4].Y = tmp.Y;
                     int mod = (i - 1) % 4;
                     m_maskPoints[mod < 0 ? 3 : mod].X = tmp.X;
                  }
               }
               _firstSet = false;
            }
         }
         pictureBox1.Invalidate();
      }

      private void FramePictureBox_MouseUp(object sender, MouseEventArgs e)
      {
         _index = -1;
      }

      private void _DrawRoadPoints(PaintEventArgs e)
      {
         Pen myPen = new Pen(Color.Yellow, 2);
         e.Graphics.DrawPolygon(myPen, m_maskPoints.ToArray());

         foreach (Point corner in m_maskPoints)
         {
            _DrawCorner(e, corner);
         }
      }

      private void _DrawCorner(PaintEventArgs e, Point corner)
      {
         Rectangle rect = new Rectangle(
             corner.X - _objectRadius, corner.Y - _objectRadius,
             2 * _objectRadius + 1, 2 * _objectRadius + 1);
         e.Graphics.FillEllipse(Brushes.White, rect);
         e.Graphics.DrawEllipse(Pens.Black, rect);
      }

      private int _FindSelectPoint(Point mousePoint)
      {
         for (int i = 0; i < m_maskPoints.Length; i++)
         {
            if (_FindDistanceToPointSquared(m_maskPoints[i], mousePoint) < 15)
            {
               return i;
            }
         }

         return -1;
      }

      private int _FindDistanceToPointSquared(Point pt1, Point pt2)
      {
         int dx = pt1.X - pt2.X;
         int dy = pt1.Y - pt2.Y;
         return dx * dx + dy * dy;
      }
   }
}