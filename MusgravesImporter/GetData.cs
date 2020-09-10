
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MusgravesImporter
{
    public class ProcessFile
    {
        public int ReadFile(string fileLocation)
        {
            string category = null;
            string[] files = System.IO.Directory.GetFiles(fileLocation, "*.csv");
            if (files.Length < 1) return 0;
            var reportingModel = new List<ReportingModel>();
            foreach (var file in files)
            {
                var fileString = file.Split(" ");
                foreach (var filename in fileString)
                {
                    if(filename.ToLower().Contains("hous") || filename.ToLower().Contains("elect") || filename.ToLower().Contains("kit"))
                    {
                        category = filename;
                        using (var stream = File.OpenRead(file))
                        {
                            if (stream == null) return 0;
                            using (var reader = new StreamReader(stream, Encoding.UTF8))
                            {

                                var data = EnumerateLines(reader).ToList();
                                var startingRow = 9;

                                if (category.ToLower().Contains("hous"))
                                {
                                    startingRow = startingRow + 1;
                                }
                                for (int x = startingRow; x < data.Count; x++)
                                {
                                    var weekRow = data[5].Split(",");
                                    var locationRow = data[x].Split(",");
                                    var location = locationRow[0];
                                    var weekNumber = weekRow[2];
                                    var productRow = data[startingRow - 1].Split(",");


                                    for (int y = 1; y < locationRow.Count(); y++)
                                    {
                                        var product = productRow[y];
                                        var sales = locationRow[y];
                                        if (!string.IsNullOrWhiteSpace(location))
                                        {
                                            if(!string.IsNullOrWhiteSpace(sales))
                                            {
                                                var info = new ReportingModel
                                                {
                                                    Location = location,
                                                    Week = weekNumber,
                                                    Product = product.Split("|")[0].Replace("\"", " "),
                                                    ProductType = category,
                                                    Sales = sales,
                                                    FileName = fileString[0]
                                                };
                                                reportingModel.Add(info);
                                            }
                                     
                                        }


                                    }


                                }

                                stream.Close();

                            }




                        }

                    }
                }
                
                





            }
            try
            {
                reportingModel = reportingModel.OrderBy(a => a.Week).ThenBy(b => b.ProductType).ToList();
                CreateFile(reportingModel);
                var context = new DbContext();
                context.InsertIntoMusgrave(reportingModel);
             
                return reportingModel.Count;
            }
            catch (Exception ex)
            {
                LogFile.Write("An Error has accure white creating entries");
                return 0;
            }

      
        }

        public static IEnumerable<string> EnumerateLines(StreamReader reader)
        {
            var lines = new List<string>();
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                lines.Add(line);
                //yield return line;
            }
            
            return lines.ToArray();
        }
       
        private void CreateFile(List<ReportingModel> reportingModels)
        {
            var reportingModelgroupByWeek = reportingModels.GroupBy(a => a.Week).ToList();
            foreach(var reportingModel in reportingModelgroupByWeek)
            {

                var week = reportingModel.FirstOrDefault()?.Week;

                var csv = new StringBuilder();
                var header = "Week, Category, Location,Item, Quantity";

                var fileLocation = Settings.GetCreateFileLocation() + $"Processed";



                csv.AppendLine(header);
                //before your loop
                foreach (var item in reportingModel)
                {
                    if (string.IsNullOrWhiteSpace(item.Sales))
                    {
                        item.Sales = "0";
                    }
                    var newLine = string.Format("{0},{1},{2},{3},{4}", item.Week, item.ProductType, item.Location, item.Product, item.Sales);
                    csv.AppendLine(newLine);
                }
                var file = "";

                if (!Directory.Exists(fileLocation))
                {
                    Directory.CreateDirectory(fileLocation);
                }
              
                    var filename = $"Radius ProcessedFile-{week}.csv";
                     file = Path.Combine(fileLocation, filename);
                 
               

             
         
                File.WriteAllText(file, csv.ToString());
               

            }

        }
    }
}
