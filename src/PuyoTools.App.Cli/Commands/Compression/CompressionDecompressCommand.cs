using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using PuyoTools.App.Tools;
using System;
using System.CommandLine;
using System.IO;
using System.Linq;

namespace PuyoTools.App.Cli.Commands.Compression
{
    class CompressionDecompressCommand : Command
    {
        public CompressionDecompressCommand()
            : base("decompress", "Decompress files")
        {
            Option<string[]> inputOption = new("--input", "-i")
            {
                Description = "Files to decompress (pattern matching supported).",
                Required = true,
            };
            Options.Add(inputOption);

            Option<string[]> excludeOption = new("--exclude")
            {
                Description = "Files to exclude from being decompressed (pattern matching supported)."
            };
            Options.Add(excludeOption);

            Option<bool> overwriteOption = new("--overwrite")
            {
                Description = "Overwrite compressed file with its decompressed file."
            };
            Options.Add(overwriteOption);

            Option<bool> deleteOption = new("--delete")
            {
                Description = "Delete compressed file on successful decompression."
            };
            Options.Add(deleteOption);

            SetAction(parseResult =>
            {
                CompressionDecompressOptions options = new()
                {
                    Input = parseResult.GetValue(inputOption),
                    Exclude = parseResult.GetValue(excludeOption),
                    Overwrite = parseResult.GetValue(overwriteOption),
                    Delete = parseResult.GetValue(deleteOption),
                };

                Execute(options, parseResult.Configuration.Output);
            });
        }

        private void Execute(CompressionDecompressOptions options, TextWriter writer)
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
            var toolOptions = new CompressionDecompressorOptions
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
            var tool = new CompressionDecompressor(toolOptions);
            tool.Execute(files, progress);
        }
    }
}
