using System;
using System.IO;
using CommandLine;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using TKOM.Exceptions;
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

            [Option('d', "declaration",
                HelpText = "Add HTML declaration on the top of the file")]
            public bool AddDeclaration { get; set; }
        }
        static void Main(string[] args)
        {
            string model = "";
            string templatePath = "";
            string outputPath = "";
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
                    else
                    {
                        throw new Exception($"File with model could not be read.");
                    }
                    if (File.Exists(opts.TemplatePath))
                    {
                        templatePath = opts.TemplatePath;
                    }
                    else
                    {
                        throw new Exception($"File with template could not be read.");
                    }
                    outputPath = opts.OutputPath;


                    Execute(JToken.Parse(model), templatePath, outputPath, opts.AddDeclaration);
                });






            // string json = @"[1, 2, 5]";
            // var deserialized = JToken.Parse(json);
            // Console.WriteLine(deserialized.Count());
        }

        private static void Execute(JToken model, string templatePath, string outputPath, bool addDeclaration)
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("TKOM.Program", LogLevel.Debug)
                    .AddConsole();
            });
            ILogger<Program> logger = loggerFactory.CreateLogger<Program>();

            var fileReader = new FileReader(templatePath);
            var scanner = new Scanner(fileReader);
            var parser = new Tools.Parser(scanner);
            try
            {
                logger.LogInformation("About to parse...");
                var tree = parser.Parse();

                var sem_checker = new SemanticsChecker(tree);
                var functionsDict = sem_checker.CheckAST();

                var executor = new Executor(functionsDict);
                executor.Execute(model, outputPath, addDeclaration);
            }
            catch (ParsingException e)
            {
                logger.LogError($"{scanner.Token.Line}:{scanner.Token.Column} Parsing error:\n{e.Message}\n\nBuild failed. Try to compile after fixing the error.");
                return;
            }
            catch (SemanticsException e)
            {
                logger.LogError($"Semantics exception: \n{e.Message}\n\nBuild failed. Try to compile after fixing the error.");
                return;
            }
            catch (RuntimeException e)
            {
                logger.LogError($"Runtime exception: \n{e.Message}\n\nBuild failed. Try to compile after fixing the error.");
                return;
            }
            logger.LogInformation("The file has been parsed successfully!");
        }

    }
}
