using System;
using System.Collections.Generic;
using System.Diagnostics;
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


            while (!stoppingToken.IsCancellationRequested)
            {
                //Set time of date when process should  run, this can bet set in the settings json if varies.
               // if (DateTime.Now.TimeOfDay.Hours == 21 && DateTime.Now.TimeOfDay.Minutes == 55)
               // {
                    LogFile.Write($"Start Processing on {DateTime.Now}");
                    Process();
               // }
            }
        }

        private void Process()
        {
            ProcessFile process = new ProcessFile();

            string path = Settings.GetFileLocation();
            var fileCount =process.ReadFile(path);
            LogFile.Write($"{fileCount} rows were processed on {DateTime.Now}");
        }
    }
}
