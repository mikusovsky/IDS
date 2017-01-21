namespace IDS.IDS
{
   public static class Deffinitions
   {
      public const bool DEBUG_MODE = true;
      public const string SUM_CARS = "sumCars";
      public const string SUM_TRUCKS = "sumTrucks";
      public const string TOTAL_SUM = "totalSum";

      public const string CACHE_PATH = "D:\\Skola\\UK\\DiplomovaPraca\\PokracovaniePoPredchodcovi\\zdrojové kódy\\Cache";
      public const string TRAINING_DB_CONFIG_PATH = "D:\\Skola\\UK\\DiplomovaPraca\\PokracovaniePoPredchodcovi\\zdrojové kódy\\CarModelRecognition\\configuration\\LoadDb.xml";
      public const string TRAINING_DB_NORMALIZED_CONFIG_PATH = "D:\\Skola\\UK\\DiplomovaPraca\\PokracovaniePoPredchodcovi\\zdrojové kódy\\CarModelRecognition\\configuration\\LoadDbNormalized.xml";
      public const string TESTING_DB_CONFIG_PATH = "D:\\Skola\\UK\\DiplomovaPraca\\PokracovaniePoPredchodcovi\\zdrojové kódy\\CarModelRecognition\\configuration\\LoadDbTesting.xml";

      public const int LOW_FRAME_WIDTH = 320;

      public const double MASK_WIDTH_FACTOR = 2.5;
      public const double MASK_HEIGHT_FACTOR = 1.8;

      public const double HEIGHT_TO_WIDTH = 1.5;

      public const int NORMALIZE_MASK_WIDTH = 128;
      public const int NORMALIZE_MASK_HEIGHT = 128;

      public enum DbType
      {
         Training = 1,
         TrainingNormalized = 2,
         Testing = 3
      }

      public enum DescriptorType
      {
         SURF = 1,
         SIFT = 2
      }
   }
}
