using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MusgravesImporter
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //  var connection = new DbContext();
            // connection.GetConnection();

            while (!stoppingToken.IsCancellationRequested)
            {
                //if (DateTime.Now.DayOfWeek.ToString().ToLower() == "wednesday" && DateTime.Now.TimeOfDay.Hours == 12)
                //{

                LogFile.Write($"Start Processing on {DateTime.Now}");
                _logger.LogInformation($"Start Processing on {DateTime.Now}", DateTimeOffset.Now);

                Process();
                await Task.Delay(360000, stoppingToken);
                //}
            }
        }

        private void Process()
        {
            ProcessFile process = new ProcessFile();

            string path = Settings.GetFileLocation();
            if (IfFileLocationPathExits(path))
            {
                var fileCount = process.ReadFile(path);
                LogFile.Write($"{fileCount} rows were processed on {DateTime.Now}");
                _logger.LogInformation($"{fileCount} rows were processed on {DateTime.Now}", DateTimeOffset.Now);

            }
        }

        private bool IfFileLocationPathExits(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    return true;
                }
                else
                {
                    LogFile.Write($"There is no file or the path {path} does not exits");
                    _logger.LogInformation($"There is no file or the path {path} does not exits", DateTimeOffset.Now);
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }


        }
    }
}
