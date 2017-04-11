using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using Emgu.CV;
using Emgu.CV.ML;
using Emgu.CV.Structure;
using IDS.IDS.IntervalTree;

namespace IDS.IDS.Classificator
{
   public class SVMClassificator : IClassificator
   {
      private SVM m_model;
      private SVMParams m_parameters;
      private Dictionary<int, CarModel> IdToModel = new Dictionary<int, CarModel>();
      private Enums.DbType m_dbType;
      private Enums.DescriptorType m_descriptorType;

      public string Id => $"{m_dbType}_{m_descriptorType}_{m_parameters.KernelType}";
      public string ConfigPath => $"{Deffinitions.CACHE_PATH_CLASSIFIER}\\{Id}.xml";

      public SVMClassificator()
      {
         m_parameters = new SVMParams();
         m_parameters.SVMType = Emgu.CV.ML.MlEnum.SVM_TYPE.C_SVC;
         m_parameters.KernelType = Emgu.CV.ML.MlEnum.SVM_KERNEL_TYPE.LINEAR;
         m_parameters.TermCrit = new MCvTermCriteria(50, 0.01);
         m_parameters.Gamma = 64;
         m_parameters.C = 8;
         m_parameters.Nu = 0.5;
         m_parameters.Degree = 10;

         m_model = new SVM();

         /*
         int trainSampleCount = 10000;
         int sigma = 60;

         #region Generate the training data and classes

         Matrix<float> trainData = new Matrix<float>(trainSampleCount, 2);
         Matrix<float> trainClasses = new Matrix<float>(trainSampleCount, 1);

         Image<Bgr, Byte> img1 = new Image<Bgr, byte>(500, 500);
         Image<Bgr, Byte> img2 = new Image<Bgr, byte>(500, 500);

         Matrix<float> sample = new Matrix<float>(1, 2);

         Matrix<float> trainData1 = trainData.GetRows(0, trainSampleCount / 3, 1);
         trainData1.GetCols(0, 1).SetRandNormal(new MCvScalar(100), new MCvScalar(sigma));
         trainData1.GetCols(1, 2).SetRandNormal(new MCvScalar(300), new MCvScalar(sigma));

         Matrix<float> trainData2 = trainData.GetRows(trainSampleCount / 3, 2 * trainSampleCount / 3, 1);
         trainData2.SetRandNormal(new MCvScalar(400), new MCvScalar(sigma));

         Matrix<float> trainData3 = trainData.GetRows(2 * trainSampleCount / 3, trainSampleCount, 1);
         trainData3.GetCols(0, 1).SetRandNormal(new MCvScalar(300), new MCvScalar(sigma));
         trainData3.GetCols(1, 2).SetRandNormal(new MCvScalar(100), new MCvScalar(sigma));

         Matrix<float> trainClasses1 = trainClasses.GetRows(0, trainSampleCount / 3, 1);
         trainClasses1.SetValue(1);
         Matrix<float> trainClasses2 = trainClasses.GetRows(trainSampleCount / 3, 2 * trainSampleCount / 3, 1);
         trainClasses2.SetValue(2);
         Matrix<float> trainClasses3 = trainClasses.GetRows(2 * trainSampleCount / 3, trainSampleCount, 1);
         trainClasses3.SetValue(3);

         #endregion

         using (SVM model = new SVM())
         {
            SVMParams p = new SVMParams();
            p = new SVMParams();
            p.SVMType = Emgu.CV.ML.MlEnum.SVM_TYPE.C_SVC;
            p.KernelType = Emgu.CV.ML.MlEnum.SVM_KERNEL_TYPE.RBF;
            p.TermCrit = new MCvTermCriteria(50, 0.01);
            p.Gamma = 64;
            p.C = 8;
            p.Nu = 0.5;
            p.Degree = 10;

            Stopwatch watch = Stopwatch.StartNew();
            bool trained = model.TrainAuto(trainData, trainClasses, null, null, p.MCvSVMParams, 5);
            watch.Stop();
            Console.WriteLine($"Swm learn - {watch.ElapsedMilliseconds}ms");
            
            watch.Restart();
            model.Save("SVM_Model.xml");
            Console.WriteLine($"Swm save - {watch.ElapsedMilliseconds}ms");
            
            watch.Restart();
            Emgu.CV.ML.SVM model_loaded = new Emgu.CV.ML.SVM();
            model_loaded.Load("SVM_Model.xml");
            Console.WriteLine($"Swm load - {watch.ElapsedMilliseconds}ms");

            for (int i = 0; i < img1.Height; i++)
            {
               for (int j = 0; j < img1.Width; j++)
               {
                  sample.Data[0, 0] = j;
                  sample.Data[0, 1] = i;

                  float response = model.Predict(sample);

                  img1[i, j] =
                     response == 1 ? new Bgr(90, 0, 0) :
                     response == 2 ? new Bgr(0, 90, 0) :
                     new Bgr(0, 0, 90);

                  img2[i, j] =
                     response == 1 ? new Bgr(90, 0, 0) :
                     response == 2 ? new Bgr(0, 90, 0) :
                     new Bgr(0, 0, 90);
               }
            }

            int c = model.GetSupportVectorCount();
            for (int i = 0; i < c; i++)
            {
               float[] v1 = model.GetSupportVector(i);
               PointF p1 = new PointF(v1[0], v1[1]);
               img1.Draw(new CircleF(p1, 4), new Bgr(128, 128, 128), 2);

               float[] v2 = model_loaded.GetSupportVector(i);
               PointF p2 = new PointF(v2[0], v2[1]);
               img2.Draw(new CircleF(p2, 4), new Bgr(128, 128, 128), 2);
            }
         }
         // display the original training samples
         for (int i = 0; i < (trainSampleCount / 3); i++)
         {
            PointF p3 = new PointF(trainData1[i, 0], trainData1[i, 1]);
            img1.Draw(new CircleF(p3, 2.0f), new Bgr(255, 100, 100), -1);
            PointF p4 = new PointF(trainData2[i, 0], trainData2[i, 1]);
            img1.Draw(new CircleF(p4, 2.0f), new Bgr(100, 255, 100), -1);
            PointF p5 = new PointF(trainData3[i, 0], trainData3[i, 1]);
            img1.Draw(new CircleF(p5, 2.0f), new Bgr(100, 100, 255), -1);

            PointF p6 = new PointF(trainData1[i, 0], trainData1[i, 1]);
            img2.Draw(new CircleF(p6, 2.0f), new Bgr(255, 100, 100), -1);
            PointF p7 = new PointF(trainData2[i, 0], trainData2[i, 1]);
            img2.Draw(new CircleF(p7, 2.0f), new Bgr(100, 255, 100), -1);
            PointF p8 = new PointF(trainData3[i, 0], trainData3[i, 1]);
            img2.Draw(new CircleF(p8, 2.0f), new Bgr(100, 100, 255), -1);
         }

         Utils.LogImage("model", img1);
         Utils.LogImage("loaded", img2);
         */
      }
      
      public bool Train(Enums.DbType dbType, Enums.DescriptorType descriptorType, Matrix<float> trainData, IList<IndecesMapping> imap)
      {
         m_dbType = dbType;
         m_descriptorType = descriptorType;
         
         Dictionary<string, Range> ranges = new Dictionary<string, Range>();
         var modelG = imap.GroupBy(x => x.CarModel.ID);
         foreach (var g in modelG)
         {
            foreach (IndecesMapping indexsMapping in g)
            {
               string id = indexsMapping.CarModel.ID;
               if (!ranges.ContainsKey(id))
               {
                  var range = new Range();
                  ranges[id] = range;
                  IdToModel[ranges.Count] = indexsMapping.CarModel;
               }
               ranges[id].AddToRange(new Point(indexsMapping.IndexStart, indexsMapping.IndexEnd));
            }
         }

         Matrix<float> trainClasses = new Matrix<float>(trainData.Rows, 1);
         int category = 1;
         foreach (Range range in ranges.Values)
         {
            for (int i = range.MinXInt; i <= range.MaxYInt; i++)
            {
               trainClasses[i, 0] = category;
            }
            category++;
         }
         Stopwatch watch = Stopwatch.StartNew();
         if (_TryLoad())
         {
            return true;
         }
         bool ret = m_model.TrainAuto(trainData, trainClasses, null, null, m_parameters.MCvSVMParams, 5);
         watch.Stop();
         Console.WriteLine($"Swm learn - {watch.ElapsedMilliseconds}ms");

         _Save();
         return ret;
      }

      public CarModel GetClass(Matrix<float> data)
      {
         if (data == null)
         {
            return null;
         }
         List<int> predictions = new List<int>();
         for (int i = 0; i < data.Rows; i++)
         {
            predictions.Add(Convert.ToInt32(Math.Round(m_model.Predict(data.GetRow(i)))));
         }
         int most = (from i in predictions
                     group i by i into grp
                     orderby grp.Count() descending
                     select grp.Key).First();
         
         return IdToModel[most];
      }

      private void _Save()
      {
         m_model.Save(ConfigPath);
      }

      private bool _TryLoad()
      {
         if (!File.Exists(ConfigPath))
         {
            return false;
         }
         m_model.Load(ConfigPath);
         return true;
      }
   }
}