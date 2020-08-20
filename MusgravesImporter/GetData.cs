using Spire.Xls;
using Spire.Xls.Collections;
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

            string[] files = System.IO.Directory.GetFiles(fileLocation, "*.csv");
            if (files.Length < 1) return 0;
            var reportingModel = new List<ReportingModel>();
            foreach (var file in files)
            {
                using (var stream = File.OpenRead(file))
                {
                    if (stream == null) return 0;
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {

                        var data = EnumerateLines(reader).ToList();
                        var startingRow = 9;
                        var category = data[0].Split(",")[0];
                        if (category.ToLower().Contains("household"))
                        {
                            startingRow = startingRow + 1;
                        }
                        for (int x = startingRow; x < data.Count; x++)
                        {
                            var weekRow = data[5].Split(",");
                            var locationRow = data[x].Split(",");
                            var location = locationRow[0];
                            var weekNumber = weekRow[2];
                            var productRow = data[startingRow-1].Split(",");
                           

                            for (int y = 1; y < locationRow.Count(); y++)
                            {
                                var product = productRow[y];
                                var sales = locationRow[y];
                                if (!string.IsNullOrWhiteSpace(location))
                                {
                                    var info = new ReportingModel
                                    {
                                        Location = location,
                                        Week = weekNumber,
                                        Product = product.Split("|")[0],
                                        ProductType = category,
                                        Sales = sales
                                    };
                                    reportingModel.Add(info);
                               }


                            }


                        }

                        try
                        {
                            CreateFile(reportingModel);
                            stream.Close();
                            MoveFile(files);
                            return reportingModel.Count;
                        }
                        catch (Exception ex)
                        {
                            LogFile.Write("An Error has accure white creating entries");
                            return 0;
                        }
                     

                    }
                     
           


                }






            }

            return 0;
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
            Console.WriteLine("Total of {0} lines found", lines.Count);
            return lines.ToArray();
        }
        private void MoveFile(string[] files)
        {
            var location = Settings.GetFileLocation() + "OrigianlProcessFiles//";
            foreach (var file in files)
            {
                var filename = file.Split("//")[2];
                if (!Directory.Exists(location))
                {
                    Directory.CreateDirectory(location);
                }

                File.Move(file, location + filename);
            }
        }

        private void CreateFile(List<ReportingModel> reportingModel)
        {
            var week = reportingModel.FirstOrDefault()?.Week;

            var csv = new StringBuilder();
            var header = "Week, Category, Location,Item, Quantity";

            var fileLocation = Settings.GetCreateFileLocation() + $"Processed//ProcessedFile{week}.csv";

            csv.AppendLine(header);
            //before your loop
            foreach (var item in reportingModel)
            {
                var newLine = string.Format("{0},{1},{2},{3},{4}",item.Week, item.ProductType, item.Location, item.Product, item.Sales);
                csv.AppendLine(newLine);
            }

         

            //after your loop
            File.WriteAllText(fileLocation, csv.ToString());


            //Workbook workbook = new Workbook();
            //Worksheet sheet = workbook.Worksheets[0];

            //sheet.Name = "Weekly Sales";
            //sheet.Range["A1"].Text = $"Musgraves Processed Daily Sales on {DateTime.Now} ";

            //sheet.Range["B3"].Text = "Product Type";
            //sheet.Range["D3"].Text = "Product";
            //sheet.Range["C3"].Text = "Location";
            //sheet.Range["A3"].Text = "Week Number";
            //sheet.Range["E3"].Text = "Sales";

            //var week = reportingModel.FirstOrDefault()?.Week;
            //int row = 70000;
            //foreach (var item in reportingModel)
            //{
            //    if (string.IsNullOrWhiteSpace(item.Sales))
            //    {
            //        var test = 2;
            //    }
            //    try
            //    {
            //        sheet.Range[$"B{row}"].Text = item.ProductType;
            //        sheet.Range[$"D{row}"].Text = item.Product;
            //        sheet.Range[$"C{row}"].Text = item.Location;
            //        sheet.Range[$"A{row}"].Text = item.Week;
            //        sheet.Range[$"E{row}"].Text = string.IsNullOrWhiteSpace(item.Sales) ? "0" : item.Sales;
            //    }
            //   catch(Exception ex)
            //    {
            //        var i = ex.Message;
            //    }

            //    row++;
            //}

           
            //workbook.SaveToFile($"{fileLocation}ProcessedFile {week}.csv ", ExcelVersion.Version2016);

            
        }
    }
}
