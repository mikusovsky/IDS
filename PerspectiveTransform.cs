using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;
using System.Drawing;

namespace IDS
{
    //trieda na pomocne vypocty perspektivnej transformacie 
    class PerspectiveTransform
    {
       

        //vypocet pozicie bodu vo vtacom pohlade
        static public Matrix<float> perspectiveTransformPoint(int x, int y, Matrix<float> matrix)
        {
            float[,] scrp = { { x, y, 1 } };
            float[,] dstp = new float[2, 2];
            Matrix<float> scr = new Matrix<float>(scrp);
            Matrix<float> dst = matrix.Mul(scr.Transpose());
            dst = dst.Mul(1 / dst[2, 0]);
            return dst.Transpose();
        }

        //vypocet matice potrebnej na transformaciu obrayu do vtacieho pohladu
        static public Matrix<float> findHomographyMatrix(List<Point> roadPoints)
        {
            
            Point roadPointLT = roadPoints[0];
            Point roadPointRT = roadPoints[1];
            Point roadPointLB = roadPoints[2];
            Point roadPointRB = roadPoints[3];

            float[,] scrp = { { roadPointLT.X, roadPointLT.Y }, { roadPointRT.X, roadPointRT.Y }, 
                            { roadPointLB.X, roadPointLB.Y }, { roadPointRB.X, roadPointRB.Y } };
            float[,] dstp = { { 0, 0 }, { MainForm.BIRD_EYE_WIDTH, 0 }, 
                            { 0, MainForm.BIRD_EYE_HEIGHT }, { MainForm.BIRD_EYE_WIDTH, MainForm.BIRD_EYE_HEIGHT } };
            float[,] homog = new float[3, 3];

            Matrix<float> c1 = new Matrix<float>(scrp);
            Matrix<float> c2 = new Matrix<float>(dstp);
            Matrix<float> homogMatrix = new Matrix<float>(homog);

            CvInvoke.cvFindHomography(c1.Ptr, c2.Ptr, homogMatrix.Ptr, Emgu.CV.CvEnum.HOMOGRAPHY_METHOD.DEFAULT, 0, IntPtr.Zero);
            return homogMatrix;
        }

        //konvertovanie vzdialenosti do vzdialenosti v vtacom pohlade
        static public double convertDistanceToPerspective(double dist)
        {
            return (dist / MainForm.perspectiveMeasureDistance) * MainForm.realDistance;
        }
    }
}
