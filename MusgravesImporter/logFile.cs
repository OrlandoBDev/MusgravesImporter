using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MusgravesImporter
{
    public class LogFile
    {
        static readonly string BasePath = AppContext.BaseDirectory;

        public static void Write(string message)
        {

            var Date = $"{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Year}";
            var filePath = $@"{Settings.GetFileLocation()}/{Date}-LogFile.txt";


            using (var sw = new StreamWriter(filePath, true))
            {
                sw.WriteLine(message);
                sw.Close();
            };
        }


    }
}
