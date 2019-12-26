using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

            string json = @"[1, 2, 5]";
            var deserialized = JToken.Parse(json);
            Console.WriteLine(deserialized.Count());
        }

    }
}
