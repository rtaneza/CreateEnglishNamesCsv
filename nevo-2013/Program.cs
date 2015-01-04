using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using LumenWorks.Framework.IO.Csv;
using System.Diagnostics;

namespace CreateEnglishNamesCsv
{
  class Program
  {
    const string Folder = @"C:\Users\ronald\code\Carbs\nevo\Nevo-Online versie 2013_4.0.xls\";
    const string InputFilename = Folder + @"English index NEVO online 2013c.txt";
    const string FoodListCsvFilename = Folder + @"Nevo-Online versie 2013_4.csv";
    const string OutputCsvFilename = Folder + @"NevoFoodListWithEnglishNames.csv";
    const string sep = ",";

    struct EnglishInfo
    {
      public string Name;
      public string FoodGroup;
    }

    static string EscapeField(string field)
    {
      Trace.Assert(!field.Contains("\""));

      var escapedField = field;
      if (field.Contains(","))
      {
        escapedField = "\"" + field + "\"";
      }
      return escapedField;
    }

    static void Main(string[] args)
    {
      // Example: 
      // Abricots dried and soaked --- Abrikozen gedroogde geweekt                                      2685    Fruit 
      Regex FoodNameRegex = new Regex(@"^(.+)---(.+)\s+(\d+)\s+(.+)$");

      var EnglishNames = new Dictionary<string,EnglishInfo>();

      using (var sr = new StreamReader(InputFilename))
      {
        string line;

        while ((line = sr.ReadLine()) != null)
        {
          line = line.Trim();
          if (string.IsNullOrEmpty(line))
          {
            continue;
          }

          var match = FoodNameRegex.Match(line);
          if (match.Success)
          {
            var englishName = match.Groups[1].ToString().Trim();
            var dutchName = match.Groups[2].ToString().Trim();
            var code = match.Groups[3].ToString().Trim();
            var foodGroup = match.Groups[4].ToString().Trim();
            //outstream.WriteLine(foodGroup + sep + productGroep + sep + englishName + sep + dutchName + sep + code);
            EnglishNames[code] = new EnglishInfo { Name = englishName, FoodGroup = foodGroup };
            continue;
          }

          // unknown line
          Console.WriteLine(line);
        }
      }

      Console.WriteLine("Writing output file");
      using (CsvReader csv = new CsvReader(new StreamReader(FoodListCsvFilename), true))
      {
        const int ProductCodeColumnIndex = 2;
        int fieldCount = csv.FieldCount;

        var headers = csv.GetFieldHeaders();
        
        using (var outstream = new StreamWriter(OutputCsvFilename))
        {
          outstream.Write(string.Join(sep, headers));
          outstream.WriteLine(sep + "FoodGroup,EnglishName");

          while (csv.ReadNextRecord())
          {
            var code = csv[ProductCodeColumnIndex];

            var fields = new string[fieldCount];
            csv.CopyCurrentRecordTo(fields);
            var escapedFields = fields.Select(x => EscapeField(x));
            //Console.WriteLine(line + "\n");
          
            outstream.Write(string.Join(sep, escapedFields));
            
            var info = EnglishNames[code];
            outstream.WriteLine(sep + EscapeField(info.FoodGroup) + sep + EscapeField(info.Name));
          }          
        }
      }
    }
  }
}
