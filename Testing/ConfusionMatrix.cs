using System;
using System.Collections.Generic;
using System.Text;

namespace IDS.IDS.Testing
{
   public class ConfusionMatrix
   {
      private List<CarModel> m_models;
      private float[][] m_matrix;

      public ConfusionMatrix(List<CarModel> mModels)
      {
         m_models = mModels;
         _CreateMatrix();
      }

      public void AddModel(CarModel model, CarModel findedModel)
      {
         int modelIndex = _GetModelIndex(model);
         int findedModelIndex = _GetModelIndex(findedModel);
         if (findedModelIndex == -1)
         {
            _AddNewCarModel(findedModel);
            findedModelIndex = _GetModelIndex(findedModel);
         }
         m_matrix[modelIndex][findedModelIndex] += 1;
      }

      private void _CreateMatrix()
      {
         m_matrix = new float[m_models.Count][];
         for (int i = 0; i < m_matrix.Length; i++)
         {
            m_matrix[i] = new float[m_models.Count];
         }
      }

      private void _AddNewCarModel(CarModel newModel)
      {
         float[][] oldMatrix = m_matrix;
         m_models.Add(newModel);
         m_matrix = new float[m_models.Count][];
         for (int i = 0; i < m_matrix.Length; i++)
         {
            m_matrix[i] = new float[m_models.Count];
         }

         for (int i = 0; i < m_matrix.Length; i++)
         {
            for (int j = 0; j < m_matrix[i].Length; j++)
            {
               if (i < oldMatrix.Length && j < oldMatrix[i].Length )
               {
                  m_matrix[i][j] = oldMatrix[i][j];
               }
            }
         }
      }

      private int _GetModelIndex(CarModel carModel)
      {
         return m_models.IndexOf(carModel);
      }

      private float _GetModelSamplesCount(CarModel carModel)
      {
         int carModelIndex = _GetModelIndex(carModel);
         float count = 0;
         for (int i = 0; i < m_matrix[carModelIndex].Length; i++)
         {
            count += m_matrix[carModelIndex][i];
         }
         return count;
      }

      public string ToString(bool probability = true)
      {
         if (probability)
         {
            return _GetProbabilityMatrix();
         }
         return _GetCountMatrix();
      }

      public string _GetProbabilityMatrix()
      {
         StringBuilder sb = new StringBuilder();
         sb.Append(" ");
         foreach (CarModel model in m_models)
         {
            sb.Append($"\t{model.ID}");
         }
         sb.Append(Environment.NewLine);
         for (int i = 0; i < m_matrix.Length; i++)
         {
            CarModel model = m_models[i];
            sb.Append(model.ID);
            float samplesCount = _GetModelSamplesCount(model);
            for (int j = 0; j < m_matrix[i].Length; j++)
            {
               sb.Append($"\t{m_matrix[i][j] / samplesCount}");
            }
            sb.Append(Environment.NewLine);
         }
         return sb.ToString();
      }

      public string _GetCountMatrix()
      {
         StringBuilder sb = new StringBuilder();
         sb.Append(" ");
         foreach (CarModel model in m_models)
         {
            sb.Append($"\t{model.ID}");
         }
         sb.Append(Environment.NewLine);
         for (int i = 0; i < m_matrix.Length; i++)
         {
            CarModel model = m_models[i];
            sb.Append(model.ID);
            for (int j = 0; j < m_matrix[i].Length; j++)
            {
               sb.Append($"\t{m_matrix[i][j]}");
            }
            sb.Append(Environment.NewLine);
         }
         return sb.ToString();
      }
   }
}