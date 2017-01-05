using System.Collections.Generic;
using System.IO;

namespace IDS.IDS
{
   public class CarModel
   {
      public string Maker;
      public string Model;
      public string Generation;
      public int From;
      public int To;
      public string ImagePath;
      public List<string> ImagesPath = new List<string>();

      public CarModel(string maker, string model, string generation, int from, int to, string imagePath)
      {
         Maker = maker;
         Model = model;
         Generation = generation;
         From = from;
         To = to;
         ImagePath = imagePath;
         _SetImagesPaths();
      }

      private void _SetImagesPaths()
      {
         foreach (string file in Directory.EnumerateFiles(ImagePath, "*"))
         {
            ImagesPath.Add(file);
         }
      }
   }
}