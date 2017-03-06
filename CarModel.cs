using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace IDS.IDS
{
   public class CarModel
   {
      private const int CODE_MAKER = 2;
      private const int CODE_MODEL = 3;
      private const int CODE_GENERATION = 4;
      private const int CODE_FROM = 5;
      private const int CODE_TO = 6;
      private const int CODE_IMAGE_PATH = 7;
      private const int CODE_IMAGES_PATH = 8;
      private const int CODE_END = 10;

      public string Maker;
      public string Model;
      public string Generation;
      public int From;
      public int To;
      public string ImagePath;
      public List<string> ImagesPath = new List<string>();

      public string ID
      {
         get { return $"{Maker}_{Model}_{Generation}"; }
      }

      public CarModel()
      { }

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

      public override int GetHashCode()
      {
         return ID.GetHashCode();
      }

      public override bool Equals(object obj)
      {
         var other = obj as CarModel;
         if (other == null)
         {
            return false;
         }
         return ID == other.ID;
      }

      public void Encode(BinaryWriter writer)
      {
         writer.Write(CODE_MAKER);
         writer.Write(Maker);
         writer.Write(CODE_MODEL);
         writer.Write(Model);
         writer.Write(CODE_GENERATION);
         writer.Write(Generation);
         writer.Write(CODE_FROM);
         writer.Write(From);
         writer.Write(CODE_TO);
         writer.Write(To);
         writer.Write(CODE_IMAGE_PATH);
         writer.Write(ImagePath);
         if (ImagesPath.Count > 0)
         {
            writer.Write(CODE_IMAGES_PATH);
            writer.Write(ImagesPath.Count);
            foreach (string path in ImagesPath)
            {
               writer.Write(path);
            }
         }
         writer.Write(CODE_END);
      }

      public void Decode(BinaryReader reader)
      {
         var code = reader.ReadInt32();
         while (code != CODE_END)
         {
            switch (code)
            {
               case CODE_MAKER:
                  Maker = reader.ReadString();
                  break;

               case CODE_MODEL:
                  Model = reader.ReadString();
                  break;

               case CODE_GENERATION:
                  Generation = reader.ReadString();
                  break;

               case CODE_FROM:
                  From = reader.ReadInt32();
                  break;

               case CODE_TO:
                  To = reader.ReadInt32();
                  break;

               case CODE_IMAGE_PATH:
                  ImagePath = reader.ReadString();
                  break;

               case CODE_IMAGES_PATH:
                  List<string> paths = new List<string>();
                  int count = reader.ReadInt32();
                  for (int i = 0; i < count; i++)
                  {
                     string path = reader.ReadString();
                     paths.Add(path);
                  }
                  ImagesPath = paths;
                  break;
            }

            code = reader.ReadInt32();
         }
      }
   }
}