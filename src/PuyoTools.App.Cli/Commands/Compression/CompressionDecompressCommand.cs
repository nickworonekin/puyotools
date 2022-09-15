using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
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
    class CompressionDecompressCommand : Command
    {
        public CompressionDecompressCommand()
            : base("decompress", "Decompress files")
        {
            AddOption(new Option<string[]>(new string[] { "-i", "--input" }, "Files to decompress (pattern matching supported).")
            {
                IsRequired = true,
            });
            AddOption(new Option<string[]>("--exclude", "Files to exclude from being decompressed (pattern matching supported)."));
            AddOption(new Option<bool>("--overwrite", "Overwrite compressed file with its decompressed file."));
            AddOption(new Option<bool>("--delete", "Delete compressed file on successful decompression."));
            //AddOption(new Option("--quiet", "Do not produce console output"));

            Handler = CommandHandler.Create<CompressionDecompressOptions, IConsole>(Execute);
        }

        private void Execute(CompressionDecompressOptions options, IConsole console)
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
                console.Out.WriteLine($"Processing {x.File} ... ({x.Progress:P0})");
            });

            // Execute the tool
            var tool = new CompressionDecompressor(toolOptions);
            tool.Execute(files, progress);
        }
    }
}
