using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace CreateEnglishNamesCsv
{
  class Program
  {
    const string Folder = @"C:\Users\ronald\code\nevo\Nevo-Online versie 2011_3.0.xls\";
    const string InputFilename = Folder + @"Engelse index NEVO-online 2011.txt";
    const string OutputFilename = Folder + @"NevoEnglishNames.csv";
    const string sep = ";";

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

      using (var sr = new StreamReader(InputFilename))
      {
        string line;
        string foodGroup = "Unknown";
        string productGroep = "Onbekend";

        using (var outstream = new StreamWriter(OutputFilename))
        {
          outstream.WriteLine("FoodGroup" + sep + "ProductGroep" + sep + "Name" + sep + "Naam" + sep + "Code");

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
              outstream.WriteLine(foodGroup + sep + productGroep + sep +
                englishName + sep + dutchName + sep + code);
              continue;
            }

            Console.WriteLine(line);
          }
        }
      }
    }
  }
}
