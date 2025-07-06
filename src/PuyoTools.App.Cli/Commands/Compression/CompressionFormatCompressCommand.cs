using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using PuyoTools.App.Formats.Compression;
using PuyoTools.App.Tools;
using System;
using System.CommandLine;
using System.IO;
using System.Linq;

namespace PuyoTools.App.Cli.Commands.Compression
{
    class CompressionFormatCompressCommand : Command
    {
        private readonly ICompressionFormat _format;

        public CompressionFormatCompressCommand(ICompressionFormat format)
            : base(format.CommandName, $"Compress using {format.Name} compression")
        {
            _format = format;

            Option<string[]> inputOption = new("--input", "-i")
            {
                Description = "Files to compress (pattern matching supported).",
                Required = true,
            };
            Options.Add(inputOption);

            Option<string[]> excludeOption = new("--exclude")
            {
                Description = "Files to exclude from being compressed (pattern matching supported)."
            };
            Options.Add(excludeOption);

            Option<bool> overwriteOption = new("--overwrite")
            {
                Description = "Overwrite source file with its compressed file."
            };
            Options.Add(overwriteOption);

            Option<bool> deleteOption = new("--delete")
            {
                Description = "Delete source file on successful compression."
            };
            Options.Add(deleteOption);

            SetAction(parseResult =>
            {
                CompressionFormatCompressOptions options = new()
                {
                    Input = parseResult.GetValue(inputOption),
                    Exclude = parseResult.GetValue(excludeOption),
                    Overwrite = parseResult.GetValue(overwriteOption),
                    Delete = parseResult.GetValue(deleteOption),
                };

                Execute(options, parseResult.Configuration.Output);
            });
        }

        protected virtual void Execute(CompressionFormatCompressOptions options, TextWriter writer)
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
                writer.WriteLine($"Processing {x.File} ... ({x.Progress:P0})");
            });

            // Execute the tool
            var tool = new CompressionCompressor(_format, toolOptions);
            tool.Execute(files, progress);
        }
    }
}
