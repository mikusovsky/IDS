using System.Collections.Generic;
using Emgu.CV;
using Emgu.CV.Structure;
using IDS.IDS.IntervalTree;

namespace IDS.IDS.Classificator
{
   public class SVMClassificator : IClassificator
   {
      //private SVM m_model;
      //private SVMParams m_parameters;

      public SVMClassificator()
      {
         /*
         m_model = new SVM();
         m_parameters = new SVMParams();
         m_parameters.KernelType = Emgu.CV.ML.MlEnum.SVM_KERNEL_TYPE.LINEAR;
         m_parameters.SVMType = Emgu.CV.ML.MlEnum.SVM_TYPE.C_SVC;
         m_parameters.C = 1;
         m_parameters.TermCrit = new MCvTermCriteria(100, 0.00001);
         */
      }

      public bool Train(Matrix<float> trainData, IList<IndecesMapping> imap)
      {
         throw new System.NotImplementedException();
      }

      public CarModel GetClass(Matrix<float> data)
      {
         throw new System.NotImplementedException();
      }
   }
}