using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System;
using BPC.CodeGenerator.Extensions;
using BPC.CodeGenerator.Options;

namespace BPC.CodeGenerator.Commands
{
    [Command("generate", "gen")]
    public class GenerateCommand : OptionsCommandBase
    {
        private readonly ICodeGenerator _codeGenerator;

        public GenerateCommand(ILoggerFactory logger, IConsole console, IGeneratorOptionsSerializer serializer, ICodeGenerator codeGenerator)
            : base(logger, console, serializer)
        {
            _codeGenerator = codeGenerator;
        }

        [Option("-p <Provider>", Description = "Database provider to reverse engineer")]
        public DatabaseProviders? Provider { get; set; }

        [Option("-c <ConnectionString>", Description = "Database connection string to reverse engineer")]
        public string ConnectionString { get; set; }

        public bool? Extensions { get; set; } = true;

        public bool? Models { get; set; } = true;

        public bool? Mapper { get; set; } = true;

        public bool? Validator { get; set; } = true;


        protected override int OnExecute(CommandLineApplication application)
        {
            var workingDirectory = WorkingDirectory ?? Environment.CurrentDirectory;
            var optionsFile = OptionsFile ?? GeneratorOptionsSerializer.OptionsFileName;

            var options = Serializer.Load(workingDirectory, optionsFile);
            if (options == null)
            {
                Logger.LogInformation("Using default options");
                options = new GeneratorOptions();
            }

            // override options
            if (ConnectionString.HasValue())
                options.Database.ConnectionString = ConnectionString;

            if (Provider.HasValue)
                options.Database.Provider = Provider.Value;

            if (Extensions.HasValue)
                options.Data.Query.Generate = Extensions.Value;


            if (Models.HasValue)
            {
                options.Model.Read.Generate = Models.Value;
                options.Model.Create.Generate = false;
                options.Model.Update.Generate = false;
            }

            if (Mapper.HasValue)
                options.Model.Mapper.Generate = Mapper.Value;

            if (Validator.HasValue)
                options.Model.Validator.Generate = Validator.Value;

            var result = _codeGenerator.Generate(options);

            return result ? 0 : 1;
        }

    }
}