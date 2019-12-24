using System;
using Microsoft.Extensions.Logging;
using TKOM.Tools;

namespace TKOM
{
    class Program
    {
        static void Main(string[] args)
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("TKOM.Tools.Parser", LogLevel.Debug)
                    .AddConsole();
            });
            ILogger logger = loggerFactory.CreateLogger<Parser>();
        }

    }
}
