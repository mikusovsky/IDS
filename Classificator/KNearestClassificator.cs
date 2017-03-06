using System.Collections.Generic;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Flann;
using IDS.IDS.IntervalTree;
using IntervalTree;

namespace IDS.IDS.Classificator
{
   class KNearestClassificator : IClassificator
   {
      private Index m_index;
      private Matrix<float> m_dbDescs;
      private IntervalTree<CarModel, int> m_intervalTree;

      public bool Train(Enums.DbType dbType, Enums.DescriptorType descriptorType, Matrix<float> trainData, IList<IndecesMapping> imap)
      {
         // create FLANN index with 4 kd-trees and perform KNN search over it look for 2 nearest neighbours
         m_dbDescs = trainData;
         m_index = new Index(m_dbDescs, 4);
         if (imap != null)
         {
            m_intervalTree = new IntervalTree<CarModel, int>();
            foreach (IndecesMapping indecesMapping in imap)
            {
               Interval<CarModel, int> interval = new Interval<CarModel, int>(indecesMapping.IndexStart, indecesMapping.IndexEnd, indecesMapping.CarModel);
               m_intervalTree.AddInterval(interval);
            }
            return true;
         }
         return false;
      }

      public CarModel GetClass(Matrix<float> data)
      {
         if (data == null)
         {
            return null;
         }
         Matrix<int> indices = new Matrix<int>(data.Rows, 2); // matrix that will contain indices of the 2-nearest neighbors found
         Matrix<float> dists = new Matrix<float>(data.Rows, 2); // matrix that will contain distances to the 2-nearest neighbors found

         m_index.KnnSearch(data, indices, dists, 2, 64);//24,64
         //index.RadiusSearch(queryDescriptors, indices, dists, 2, 64);
         Dictionary<CarModel, int> carModelCount = new Dictionary<CarModel, int>();

         for (int i = 0; i < indices.Rows; i++)
         {
            // filter out all inadequate pairs based on distance between pairs
            //if (dists.Data[i, 0] < (0.6*dists.Data[i, 1]))
            {
               List<CarModel> modelsOnInterval = m_intervalTree.Get(indices[i, 0], StubMode.ContainsStartThenEnd);
               if (modelsOnInterval.Count == 0)
               {
                  continue;
               }
               CarModel model = modelsOnInterval[0];
               if (!carModelCount.ContainsKey(model))
               {
                  carModelCount[model] = 0;
               }
               carModelCount[model]++;
            }
         }
         if (carModelCount.Count == 0)
         {
            return null;
         }
         List<KeyValuePair<CarModel, int>> keyValues = carModelCount.Keys.Select(carModel => new KeyValuePair<CarModel, int>(carModel, carModelCount[carModel])).ToList();
         CarModel resutl = keyValues.OrderByDescending(o => o.Value).ToList()[0].Key;
         return resutl;
      }
   }
}
