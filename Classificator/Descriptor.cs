using System;
using System.Collections.Generic;
using Emgu.CV;
using Emgu.CV.Structure;
using IDS.IDS.IntervalTree;

namespace IDS.IDS.Classificator
{
   public class Descriptor
   {
      private Enums.DbType m_dbType;
      private Enums.DescriptorType m_type;

      public Descriptor(Enums.DbType dbType, Enums.DescriptorType type)
      {
         m_dbType = dbType;
         m_type = type;
      }

      public Matrix<float> ComputeSingleDescriptors(Image<Gray, byte> image, Matrix<float> importanceMap)
      {
         return Cache.GetDescriptor(image, importanceMap, m_type);
      }

      public Matrix<float> ComputeSingleDescriptors(string fileName, Matrix<float> importanceMap, Enums.DescriptorType descriptorType)
      {
         return Cache.GetDescriptor(fileName, importanceMap, descriptorType, m_dbType);
      }

      public IList<Matrix<float>> ComputeMultipleDescriptors(List<CarModel> carModels, out IList<IndecesMapping> imap, Matrix<float> importanceMap)
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
               var desc = ComputeSingleDescriptors(imagesPath[j], importanceMap, m_type);
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

   }
}