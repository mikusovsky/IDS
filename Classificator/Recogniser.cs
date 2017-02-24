﻿using System;
using System.Collections.Generic;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Structure;
using IDS.IDS.IntervalTree;

namespace IDS.IDS.Classificator
{
   public class Recogniser : IRecogniser
   {
      private List<CarModel> m_classificationModels;

      public List<CarModel> ClassificationModels
      {
         get { return m_classificationModels; }
      }

      private Enums.DescriptorType m_descriptorType;
      private Descriptor m_descriptor;
      private Enums.ClassificatorType m_classificatorType;
      private IClassificator m_classificator;

      private Matrix<float> m_importanceMap;
      private IList<IndecesMapping> m_imap;

      public void LoadDb(Enums.DbType dbType, Enums.DescriptorType descriptorType, Enums.ClassificatorType classificatorType)
      {
         m_descriptorType = descriptorType;
         m_classificatorType = classificatorType;
         m_importanceMap = null; //Utils.CreateImportanceMap();
         m_descriptor = new Descriptor(dbType, m_descriptorType);

         m_classificationModels = Utils.GetAllCarModels(dbType);
         IList<Matrix<float>> dbDescsList = m_descriptor.ComputeMultipleDescriptors(m_classificationModels, out m_imap, m_importanceMap);
         Matrix<float> dbDescs = ConcatDescriptors(dbDescsList);
         foreach (Matrix<float> m in dbDescsList)
         {
            m.Dispose();
         }
         dbDescsList = null;
         GC.Collect();

         if (Enums.ClassificatorType.KMeans == m_classificatorType)
         {
            m_classificator = new KMeansClassificator();
         }
         else
         {
            m_classificator = new SVMClassificator();
         }
         m_classificator.Train(dbDescs, m_imap);
      }

      public CarModel Match(Image<Bgr, byte> image, bool? onlyCarMaker = null)
      {
         return Match(Utils.ToGray(image), onlyCarMaker);
      }

      public CarModel Match(Image<Gray, byte> image, bool? onlyCarMaker = null)
      {
         Matrix<float> queryDescriptors = m_descriptor.ComputeSingleDescriptors(image, m_importanceMap);
         Utils.LogImage("tested Image", image);
         CarModel matchesModel = m_classificator.GetClass(queryDescriptors);
         return matchesModel;
      }

      /// <summary>
      /// Concatenates descriptors from different sources (images) into single matrix.
      /// </summary>
      /// <param name="descriptors">Descriptors to concatenate.</param>
      /// <returns>Concatenated matrix.</returns>
      public static Matrix<float> ConcatDescriptors(IList<Matrix<float>> descriptors)
      {
         int cols = descriptors[0].Cols;
         int rows = descriptors.Sum(a => a.Rows);

         float[,] concatedDescs = new float[rows, cols];

         int offset = 0;

         foreach (var descriptor in descriptors)
         {
            // append new descriptors
            Buffer.BlockCopy(descriptor.ManagedArray, 0, concatedDescs, offset, sizeof(float) * descriptor.ManagedArray.Length);
            offset += sizeof(float) * descriptor.ManagedArray.Length;
         }

         return new Matrix<float>(concatedDescs);
      }
   }
}