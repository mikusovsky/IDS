namespace IDS.IDS
{
   public class Enums
   {
      public enum TypeOfRatio
      {
         None = 0,
         Frame = 1,
         Road = 2
      }

      public enum TypeOfBlur
      {
         Blur = 0,
         Median = 1,
         Bilatral = 2,
         Gaussian = 3
      }

      public enum DbType
      {
         None = 0,
         Training = 1,
         TrainingNormalized = 2,
         Testing = 3,
         TestingMask = 4,
         Subset1 = 5,
         Subset2 = 6,
         TrainingBrand = 7,
         TrainingAudi = 8,
         TrainingBMW = 9,
         TrainingSkoda = 10,
         TrainingVolkswagen = 11,
         TestingBrand = 12,
         TrainingDbForBrand = 13,
         TrainingDbForBrandNormalized = 14,
         TestingBrandMask = 15
      }

      public enum DescriptorType
      {
         SURF = 1,
         SIFT = 2,
         ORB = 3
      }

      public enum ClassificatorType
      {
         KNearest = 1,
         SVM = 2
      }
   }
}