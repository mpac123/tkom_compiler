using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TKOM.Readers;
using TKOM.Tools;

namespace TKOM
{
    class Program
    {
        public class Options
        {
            [Option('t', "template", Required = true,
                HelpText = "Path to the *.mpgl file with the template")]
            public string TemplatePath { get; set; }
            [Option('m', "model", Required = true,
                HelpText = "Path to the *.json file with the model")]
            public string ModelPath { get; set; }
            [Option('o', "output", Required = true,
                HelpText = "Path to the output file")]
            public string OutputPath { get; set; }
        }
        static void Main(string[] args)
        {
            string model;
            string templatePath = "";
            string outputPath;
            CommandLine.Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(opts =>
                {
                    if (File.Exists(opts.ModelPath))
                    {
                        using (var file = new StreamReader(opts.ModelPath))
                        {
                            model = file.ReadToEnd();
                        }
                    }
                    if (File.Exists(opts.TemplatePath))
                    {
                        templatePath = opts.TemplatePath;
                    }
                    outputPath = opts.OutputPath;
                });

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("TKOM.Tools.Parser", LogLevel.Debug)
                    .AddConsole();
            });
            ILogger<Tools.Parser> logger = loggerFactory.CreateLogger<Tools.Parser>();

            var fileReader = new FileReader(templatePath);
            var scanner = new Scanner(fileReader);
            var parser = new Tools.Parser(scanner, logger);

            var tree = parser.Parse();



            // string json = @"[1, 2, 5]";
            // var deserialized = JToken.Parse(json);
            // Console.WriteLine(deserialized.Count());
        }

    }
}
