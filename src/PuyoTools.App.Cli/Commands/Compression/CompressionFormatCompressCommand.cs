using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using PuyoTools.App.Formats.Compression;
using PuyoTools.App.Tools;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.IO;
using System.CommandLine.NamingConventionBinder;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.App.Cli.Commands.Compression
{
    class CompressionFormatCompressCommand : Command
    {
        private readonly ICompressionFormat format;

        public CompressionFormatCompressCommand(ICompressionFormat format)
            : base(format.CommandName, $"Compress using {format.Name} compression")
        {
            this.format = format;

            AddOption(new Option<string[]>(new string[] { "-i", "--input" }, "Files to compress (pattern matching supported).")
            {
                IsRequired = true,
            });
            AddOption(new Option<string[]>("--exclude", "Files to exclude from being compressed (pattern matching supported)."));
            AddOption(new Option<bool>("--overwrite", "Overwrite source file with its compressed file."));
            AddOption(new Option<bool>("--delete", "Delete source file on successful compression."));

            Handler = CommandHandler.Create<CompressionFormatCompressOptions, IConsole>(Execute);
        }

        protected virtual void Execute(CompressionFormatCompressOptions options, IConsole console)
        {
            // Get the files to process by the tool
            var matcher = new Matcher();
            matcher.AddIncludePatterns(options.Input);
            if (options.Exclude?.Any() == true)
            {
                matcher.AddExcludePatterns(options.Exclude);
            }

            var files = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(Environment.CurrentDirectory)))
                .Files
                .Select(x => x.Path)
                .ToArray();

            // Create options in the format the tool uses
            var toolOptions = new CompressionCompressorOptions
            {
                OverwriteSourceFile = options.Overwrite,
                DeleteSourceFile = options.Delete,
            };

            // Create the progress handler (only if the quiet option is not set)
            var progress = new SynchronousProgress<ToolProgress>(x =>
            {
                console.Out.WriteLine($"Processing {x.File} ... ({x.Progress:P0})");
            });

            // Execute the tool
            var tool = new CompressionCompressor(format, toolOptions);
            tool.Execute(files, progress);
        }
    }
}
