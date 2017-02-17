using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Flann;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using IDS.IDS.IntervalTree;
using IntervalTree;

namespace IDS.IDS.Classificator
{
   public class SiftClassificator : IClassificator
   {
      private IList<IndecesMapping> m_imap;
      private Matrix<float> m_dbDescs;
      private Index m_index;
      private IntervalTree<CarModel, int> m_intervalTree;
      private Matrix<float> m_importanceMap;
      private List<CarModel> m_classificationModels;

      private IClassificator m_subset1Classificator = new SurfClassificator();
      private IClassificator m_subset2Classificator = new SurfClassificator();

      private HashSet<CarModel> m_subset1;
      private HashSet<CarModel> m_subset2;
      private static Deffinitions.DbType m_dbType;

      public Deffinitions.DbType DbType
      {
         get { return m_dbType; }
      }

      public List<CarModel> ClassificationModels
      {
         get { return m_classificationModels; }
      }

      public void LoadDb(Deffinitions.DbType dbType = Deffinitions.DbType.TrainingNormalized)
      {
         m_dbType = dbType;
         m_importanceMap = null; //Utils.CreateImportanceMap();
         m_classificationModels = Utils.GetAllCarModels(dbType);
         IList<Matrix<float>> dbDescsList = ComputeMultipleDescriptors(m_classificationModels, out m_imap, m_importanceMap);
         m_dbDescs = ConcatDescriptors(dbDescsList);
         foreach (Matrix<float> m in dbDescsList)
         {
            m.Dispose();
         }
         dbDescsList = null;
         GC.Collect();
         // create FLANN index with 4 kd-trees and perform KNN search over it look for 2 nearest neighbours
         m_index = new Index(m_dbDescs, 4);
         if (m_imap != null)
         {
            m_intervalTree = new IntervalTree<CarModel, int>();
            foreach (IndecesMapping indecesMapping in m_imap)
            {
               Interval<CarModel, int> interval = new Interval<CarModel, int>(indecesMapping.IndexStart, indecesMapping.IndexEnd, indecesMapping.CarModel);
               m_intervalTree.AddInterval(interval);
            }
         }
         
         /*
         m_subset1Classificator.LoadDb(Deffinitions.DbType.Subset1);
         m_subset1 = new HashSet<CarModel>(m_subset1Classificator.ClassificationModels);
         m_subset2Classificator.LoadDb(Deffinitions.DbType.Subset2);
         m_subset2 = new HashSet<CarModel>(m_subset2Classificator.ClassificationModels);
         */
         
      }

      /// <summary>
      /// Convenience method for computing descriptors for multiple images.
      /// On return imap is filled with structures specifying which descriptor ranges in the concatenated matrix belong to what image. 
      /// </summary>
      /// <param name="fileNames">Filenames of images to process.</param>
      /// <param name="imap">List of IndecesMapping to hold descriptor ranges for each image.</param>
      /// <returns>List of descriptors for the given images.</returns>
      public static IList<Matrix<float>> ComputeMultipleDescriptors(List<CarModel> carModels, out IList<IndecesMapping> imap, Matrix<float> importanceMap = null)
      {
         imap = new List<IndecesMapping>();

         IList<Matrix<float>> descs = new List<Matrix<float>>();
         int imagesCount = 0;
         for (int i = 0; i < carModels.Count; i++)
         {
            imagesCount += carModels[i].ImagesPath.Count;
         }
         Utils.ProgressBarShow(imagesCount);
         int r = 0;
         int count = 0;
         for (int i = 0; i < carModels.Count; i++)
         {
            CarModel carModel = carModels[i];
            List<string> imagesPath = carModel.ImagesPath;
            for (int j = 0; j < imagesPath.Count; j++)
            {
               var desc = ComputeSingleDescriptors(imagesPath[j], importanceMap);
               if (desc != null)
               {
                  descs.Add(desc);

                  imap.Add(new IndecesMapping()
                  {
                     IndexStart = r,
                     IndexEnd = r + desc.Rows - 1,
                     CarModel = carModel,
                     ImageSrc = imagesPath[j]
                  });
                  Console.WriteLine($"{++count} of {imagesCount}");
                  r += desc.Rows;
               }
               Utils.ProgressBarIncrement();
            }
         }
         Console.WriteLine("loading complete");
         Utils.ProgressBarHide();
         return descs;
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

      /// <summary>
      /// Computes image descriptors.
      /// </summary>
      /// <param name="fileName">Image filename.</param>
      /// <returns>The descriptors for the given image.</returns>
      public static Matrix<float> ComputeSingleDescriptors(string fileName, Matrix<float> importanceMap = null)
      {
         return Cache.GetDescriptor(fileName, importanceMap, Deffinitions.DescriptorType.SIFT, m_dbType);
      }

      public static VectorOfKeyPoint GetKeyPoints(Image<Gray, byte> image, Deffinitions.DescriptorType descriptorType)
      {
         return Cache.GetKeyPoints(image, descriptorType);
      }

      /// <summary>
      /// Computes image descriptors.
      /// </summary>
      /// <param name="fileName">Image filename.</param>
      /// <returns>The descriptors for the given image.</returns>
      public static Matrix<float> ComputeSingleDescriptors(Image<Gray, byte> image, Matrix<float> importanceMap = null)
      {
         return Cache.GetDescriptor(image, importanceMap, Deffinitions.DescriptorType.SIFT);
      }

      public CarModel Match(Image<Bgr, byte> image, bool? onlyCarMaker = null)
      {
         return Match(Utils.ToGray(image));
      }

      public CarModel Match(Image<Gray, byte> image, bool? onlyCarMaker = null)
      {
         Matrix<float> queryDescriptors = ComputeSingleDescriptors(image, m_importanceMap);
         Utils.LogImage("normalized mask", image);
         CarModel matchesModel = _FindMatches(m_index, queryDescriptors, ref m_imap, m_intervalTree);
         return matchesModel;
      }

      /// <summary>
      /// Computes 'similarity' value (IndecesMapping.Similarity) for each image in the collection against our query image.
      /// </summary>
      /// <param name="dbDescriptors">Query image descriptor.</param>
      /// <param name="queryDescriptors">Consolidated db images descriptors.</param>
      /// <param name="images">List of IndecesMapping to hold the 'similarity' value for each image in the collection.</param>
      private CarModel _FindMatches(Index index, Matrix<float> queryDescriptors, ref IList<IndecesMapping> imap,
         IntervalTree<CarModel, int> carModelsInMatrix)
      {
         if (queryDescriptors == null)
         {
            return null;
         }
         Matrix<int> indices = new Matrix<int>(queryDescriptors.Rows, 2); // matrix that will contain indices of the 2-nearest neighbors found
         Matrix<float> dists = new Matrix<float>(queryDescriptors.Rows, 2); // matrix that will contain distances to the 2-nearest neighbors found

         index.KnnSearch(queryDescriptors, indices, dists, 2, 64);//24
         Dictionary<CarModel, int> carModelCount = new Dictionary<CarModel, int>();

         for (int i = 0; i < indices.Rows; i++)
         {
            // filter out all inadequate pairs based on distance between pairs
            //if (dists.Data[i, 0] < (0.6*dists.Data[i, 1]))
            {
               List<CarModel> modelsOnInterval = carModelsInMatrix.Get(indices[i, 0], StubMode.ContainsStartThenEnd);
               if (modelsOnInterval.Count == 0)
               {
                  continue; //TODO interval tree must contain indices values, probably StubMode.ContainsStartThenEnd it fixed
               }
               CarModel model = modelsOnInterval[0];
               if (!carModelCount.ContainsKey(model))
               {
                  carModelCount[model] = 0;
               }
               carModelCount[model]++;
            }
         }
         List<KeyValuePair<CarModel, int>> keyValues = carModelCount.Keys.Select(carModel => new KeyValuePair<CarModel, int>(carModel, carModelCount[carModel])).ToList();
         CarModel resutl = keyValues.OrderByDescending(o => o.Value).ToList()[0].Key;
         return resutl;
      }
   }
}