using System.ComponentModel;
using AForge;

namespace IDS.IDS
{
   public static class Deffinitions
   {
      public const bool DEBUG_MODE = false;
      public const string SUM_CARS = "sumCars";
      public const string SUM_TRUCKS = "sumTrucks";
      public const string TOTAL_SUM = "totalSum";
      
      public const string SOLUTION_PATH = "D:\\Skola\\UK\\DiplomovaPraca\\PokracovaniePoPredchodcovi\\zdrojové kódy";

      public const string OUTPUT_PATH = SOLUTION_PATH + "\\Output";
      public const string OUTPUT_PATH_IMAGES = OUTPUT_PATH + "\\Images";
      public const string OUTPUT_PATH_LOG = OUTPUT_PATH + "\\Log";
      public const string OUTPUT_PATH_IMAGES_TEMP = OUTPUT_PATH + "\\Temp";

      public const string CACHE_PATH = SOLUTION_PATH + "\\Cache";
      public const string CACHE_PATH_DESCRIPTOR = CACHE_PATH + "\\Descriptor";
      public const string CACHE_PATH_CLASSIFIER = CACHE_PATH + "\\Classifier";

      public const string CONFIGURATION_PATH = SOLUTION_PATH + "\\CarModelRecognition\\configuration";
      public const string TRAINING_DB_CONFIG_PATH = CONFIGURATION_PATH + "\\LoadDb.xml";
      public const string TRAINING_DB_NORMALIZED_CONFIG_PATH = CONFIGURATION_PATH + "\\LoadDbNormalized.xml";
      public const string TESTING_DB_CONFIG_PATH = CONFIGURATION_PATH + "\\LoadDbTesting.xml";
      public const string TESTING_DB_FOR_BRAND_CONFIG_PATH = CONFIGURATION_PATH + "\\LoadDbTestingForBrand.xml";
      public const string TESTING_DB_FOR_BRAND_NORMALIZED_CONFIG_PATH = CONFIGURATION_PATH + "\\LoadDbTestingForBrandNormalized.xml";
      public const string TESTING_DB_BRAND_CONFIG_PATH = CONFIGURATION_PATH + "\\LoadDbTestingBrand.xml";


      public const string TESTING_DB_BRAND_MASK_CONFIG_PATH = CONFIGURATION_PATH + "\\LoadDbTestingBrandMask.xml";
      public const string TESTING_DB_BRAND_MASK360_CONFIG_PATH = CONFIGURATION_PATH + "\\LoadDbTestingBrandMask360.xml";
      public const string TESTING_DB_BRAND_MASK540_CONFIG_PATH = CONFIGURATION_PATH + "\\LoadDbTestingBrandMask540.xml";
      public const string TESTING_DB_BRAND_MASK576_CONFIG_PATH = CONFIGURATION_PATH + "\\LoadDbTestingBrandMask576.xml";
      public const string TESTING_DB_BRAND_MASK720_CONFIG_PATH = CONFIGURATION_PATH + "\\LoadDbTestingBrandMask720.xml";
      public const string TESTING_DB_BRAND_MASK900_CONFIG_PATH = CONFIGURATION_PATH + "\\LoadDbTestingBrandMask900.xml";


      public const string TESTING_MASK_DB_CONFIG_PATH = CONFIGURATION_PATH + "\\LoadDbTestingMask.xml";
      public const string TESTING_MASK_DB360_CONFIG_PATH = CONFIGURATION_PATH + "\\LoadDbTestingMask360.xml";
      public const string TESTING_MASK_DB540_CONFIG_PATH = CONFIGURATION_PATH + "\\LoadDbTestingMask540.xml";
      public const string TESTING_MASK_DB576_CONFIG_PATH = CONFIGURATION_PATH + "\\LoadDbTestingMask576.xml";
      public const string TESTING_MASK_DB720_CONFIG_PATH = CONFIGURATION_PATH + "\\LoadDbTestingMask720.xml";
      public const string TESTING_MASK_DB900_CONFIG_PATH = CONFIGURATION_PATH + "\\LoadDbTestingMask900.xml";


      public const string TRAINING_DB_NORMALIZED_CONFIG_PATH_PODSKUPINA1 = CONFIGURATION_PATH + "\\Podskupina1.xml";
      public const string TRAINING_DB_NORMALIZED_CONFIG_PATH_PODSKUPINA2 = CONFIGURATION_PATH + "\\Podskupina2.xml";
      public const string TRAINING_DB_BRAND_CONFIG_PATH = CONFIGURATION_PATH + "\\LoadDbBrand.xml";
      public const string TRAINING_DB_AUDI_CONFIG_PATH = CONFIGURATION_PATH + "\\LoadDbAudi.xml";
      public const string TRAINING_DB_BMW_CONFIG_PATH = CONFIGURATION_PATH + "\\LoadDbBMW.xml";
      public const string TRAINING_DB_SKODA_CONFIG_PATH = CONFIGURATION_PATH + "\\LoadDbSkoda.xml";
      public const string TRAINING_DB_VOLKSWAGEN_CONFIG_PATH = CONFIGURATION_PATH + "\\LoadDbVolkswagen.xml";

      public const int LOW_FRAME_WIDTH = 320;

      public const double MASK_WIDTH_FACTOR = 2.7;
      public const double MASK_HEIGHT_FACTOR = 2.5;

      public const double HEIGHT_TO_WIDTH = 1.5;

      public const int NORMALIZE_MASK_WIDTH = 128;
      public const int NORMALIZE_MASK_HEIGHT = 128;

      public const int MASK_WIDTH_FROM_FRAME = 170;
      public const int MASK_HEIGHT_FROM_FRAME = 80;

      public const int MODELS_COUNT = 25;

      public static int[] USER_MASK_SIZE;
   }
}
