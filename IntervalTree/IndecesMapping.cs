using System;
using System.IO;
using System.Runtime.Serialization;

namespace IDS.IDS.IntervalTree
{
   public class IndecesMapping
   {
      private const int CODE_INDEX_START = 2;
      private const int CODE_INDEX_END = 3;
      private const int CODE_SIMILARITY = 4;
      private const int CODE_CARMODEL = 5;
      private const int CODE_IMAGE_SRC = 6;
      private const int CODE_END = 7;

      public int IndexStart { get; set; }
      public int IndexEnd { get; set; }
      public int Similarity { get; set; }
      public CarModel CarModel { get; set; }
      public string ImageSrc { get; set; }

      public void Encode(BinaryWriter writer)
      {
         writer.Write(CODE_INDEX_START);
         writer.Write(IndexStart);
         writer.Write(CODE_INDEX_END);
         writer.Write(IndexEnd);
         writer.Write(CODE_SIMILARITY);
         writer.Write(Similarity);
         writer.Write(CODE_CARMODEL);
         Cache.EncodeCarModel(writer, CarModel);
         writer.Write(CODE_IMAGE_SRC);
         writer.Write(ImageSrc);
         writer.Write(CODE_END);
      }

      public void Decode(BinaryReader reader)
      {
         var code = reader.ReadInt32();
         while (code != CODE_END)
         {
            switch (code)
            {
               case CODE_INDEX_START:
                  IndexStart = reader.ReadInt32();
                  break;

               case CODE_INDEX_END:
                  IndexEnd = reader.ReadInt32();
                  break;

               case CODE_SIMILARITY:
                  Similarity = reader.ReadInt32();
                  break;

               case CODE_CARMODEL:
                  CarModel = Cache.DecodeCarModel(reader);
                  break;

               case CODE_IMAGE_SRC:
                  ImageSrc = reader.ReadString();
                  break;
            }
            code = reader.ReadInt32();
         }
      }
   }
}