using Spire.Xls;
using Spire.Xls.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MusgravesImporter
{
    public class ProcessFile
    {
        public int ReadFile(string fileLocation)
        {

            var reportingModel = new List<ReportingModel>();
         
            int weekNumberRow = 5;
            Workbook workbook = new Workbook();
            workbook.LoadFromFile(fileLocation);
            WorksheetsCollection sheets = workbook.Worksheets;

          

            foreach (var sheet in sheets)
            {
                int startingRow = 9;
                var sheetName = sheet.CodeName;
                if (sheet.CodeName.ToLower().Contains("household"))
                {
                    startingRow = 10;
                }
                int columnCount = sheet.Columns.Length;
                int rowCount = sheet.Rows.Length;

                for (int start = startingRow; start < rowCount; start++)
                {
                    var location = sheet.Rows[start].Columns[0].Value;
                    var weekNumber = sheet.Rows[weekNumberRow].Columns[1].Value;



                    for (int c = 1; c < columnCount; c++)
                    {
                        var product = sheet.Rows[startingRow - 1].Columns[c].Value;

                       var sales =sheet.Rows[start].Columns[c].Value;


                        var data = new ReportingModel
                        {
                            Location = location,
                            Week = weekNumber,
                            Product = product.Split("|")[0],
                            ProductType = sheetName,
                            Sales = sales
                        };

                       
                        reportingModel.Add(data);



                     


                        //can choose to save file to data warehouse or to a file

                    }



                }
                CreateFile(reportingModel);


            }

            return reportingModel.Count;

        }

        private void CreateFile(List<ReportingModel> reportingModel)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet = workbook.Worksheets[0];

            sheet.Name = "Daily Sales";
            sheet.Range["A1"].Text = $"Musgraves Processed Daily Sales on {DateTime.Now} ";

            sheet.Range["A3"].Text = "Product Type";
            sheet.Range["B3"].Text = "Product";
            sheet.Range["C3"].Text = "Location";
            sheet.Range["D3"].Text = "Week Number";
            sheet.Range["E3"].Text = "Sales";

            var week = reportingModel.FirstOrDefault()?.Week;
            int row = 4;
            foreach (var item in reportingModel)
            {
                sheet.Range[$"A{row}"].Text = item.ProductType;
                sheet.Range[$"B{row}"].Text = item.Product;
                sheet.Range[$"C{row}"].Text = item.Location;
                sheet.Range[$"D{row}"].Text = item.Week;
                sheet.Range[$"E{row}"].Text = item.Sales??" ";

                row++;
            }

            var fileLocation = Settings.GetCreateFileLocation();
            workbook.SaveToFile($"{fileLocation}ProcessedFile {week}.xlsx ", ExcelVersion.Version2016);
        }
    }
}
