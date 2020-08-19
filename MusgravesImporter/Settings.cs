using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace MusgravesImporter
{
    public static class Settings
    {
        public static string GetFileLocation()
        {
            string basePath = AppContext.BaseDirectory;
            string jsonFilePath = $"{basePath}/settings.json";
            try
            {
                var jsonString = File.ReadAllText(jsonFilePath);
                var objData = JsonConvert.DeserializeObject<List<LocationString>>(jsonString);
               
                return objData?.FirstOrDefault()?.PickupFileLocation ; //.PickupFileLocation;

            }


            catch (Exception ex)
            {
                LogFile.Write($"Pick up file location: {ex.Message}");
            }

            return null;
        }

        public static string GetCreateFileLocation()
        {

            string basePath = AppContext.BaseDirectory;
            string jsonFilePath = $"{basePath}/settings.json";
            try
            {
                var jsonString = File.ReadAllText(jsonFilePath);
                var objData = JsonConvert.DeserializeObject<List<LocationString>>(jsonString);

                return objData?.FirstOrDefault()?.CreateFileLocation; //.PickupFileLocation;

            }


            catch (Exception ex)
            {
                LogFile.Write($"Pick up file location: {ex.Message}");
            }

            return null;
        }
    }
}
