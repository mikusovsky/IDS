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
         m_parameters.KernelType = Emgu.CV.ML.MlEnum.SVM_KERNEL_TYPE.RBF;
         m_parameters.TermCrit = new MCvTermCriteria(50, 0.01);
         m_parameters.Gamma = 64;
         m_parameters.C = 8;
         m_parameters.Nu = 0.5;
         m_parameters.Degree = 10;

         m_model = new SVM();

         */
      }
      
      public bool Train(Enums.DbType dbType, Enums.DescriptorType descriptorType, Matrix<float> trainData, IList<IndecesMapping> imap)
      {
         m_dbType = dbType;
         m_descriptorType = descriptorType;
         if (_TryLoad())
         {
            return true;
         }

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
         int predictedId = Convert.ToInt32(Math.Round(m_model.Predict(data)));
         return IdToModel[predictedId];
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