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
    const string Folder = @"C:\Users\ronald\code\nevo\Nevo-Online versie 2011_3.0.xls\";
    const string InputFilename = Folder + @"Engelse index NEVO-online 2011.txt";
    const string FoodListCsvFilename = Folder + @"Nevo-Online versie 2011_3.0_nutrient.csv";
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
      Regex FoodGroupRegex = new Regex(@"^Food group / Productgroep\: +\d+ (.+?) / (.+?)  code$");
      Regex FoodNameRegex = new Regex(@"^(.+?)  (.+?)  (\d+) *$");

      //var matchobj = FoodNameRegex.Match("Strawberries in syrup tinned                                      Aardbeien op siroop blik/glas                                        174");
      //if (matchobj.Success)
      //{
      //  var englishName = matchobj.Groups[1].ToString().Trim();
      //  var dutchName = matchobj.Groups[2].ToString().Trim();
      //  var code = matchobj.Groups[3].ToString().Trim();
      //  Console.WriteLine(englishName + "," + dutchName + "," + code);
      //}
      //else
      //{
      //  Console.WriteLine("failed!");
      //}
      //return;

      var EnglishNames = new Dictionary<string,EnglishInfo>();

      using (var sr = new StreamReader(InputFilename))
      {
        string line;
        string foodGroup = "Unknown";
        string productGroep = "Onbekend";

        while ((line = sr.ReadLine()) != null)
        {
          line = line.Trim();
          if (string.IsNullOrEmpty(line))
          {
            continue;
          }

          var match = FoodGroupRegex.Match(line);
          if (match.Success)
          {
            foodGroup = match.Groups[1].ToString().Trim();
            productGroep = match.Groups[2].ToString().Trim();
            //Console.WriteLine(foodGroup + " / " + productGroep);
            continue;
          }

          match = FoodNameRegex.Match(line);
          if (match.Success)
          {
            var englishName = match.Groups[1].ToString().Trim();
            var dutchName = match.Groups[2].ToString().Trim();
            var code = match.Groups[3].ToString().Trim();
            //outstream.WriteLine(foodGroup + sep + productGroep + sep + englishName + sep + dutchName + sep + code);
            EnglishNames[code] = new EnglishInfo { Name = englishName, FoodGroup = foodGroup };
            continue;
          }

          // unknown line
          //Console.WriteLine(line);
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
