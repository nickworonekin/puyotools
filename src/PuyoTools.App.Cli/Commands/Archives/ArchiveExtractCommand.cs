using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using PuyoTools.App.Tools;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.App.Cli.Commands.Archives
{
    class ArchiveExtractCommand : Command
    {
        public ArchiveExtractCommand()
            : base("extract", "Extract archives")
        {
            AddOption(new Option<string[]>(new string[] { "-i", "--input" }, "Archives to extract (pattern matching supported).")
            {
                IsRequired = true,
            });
            AddOption(new Option<string[]>("--exclude", "Archives to exclude from being extracted (pattern matching supported)."));

            Handler = CommandHandler.Create<ArchiveExtractOptions, IConsole>(Execute);
        }

        private void Execute(ArchiveExtractOptions options, IConsole console)
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
            var toolOptions = new ArchiveExtractorOptions
            {
            };

            // Create the progress handler (only if the quiet option is not set)
            var progress = new Progress<ToolProgress>(x =>
            {
                if (x is ArchiveEntryProgress) // No need to cast as the same properties are used
                {
                    console.Out.WriteLine($"-- Extracting {x.File} ... ({x.Progress:P0})");
                }
                else
                {
                    console.Out.WriteLine($"Processing {x.File} ... ({x.Progress:P0})");
                }
            });

            // Execute the tool
            ArchiveExtractor.Execute(files, toolOptions, progress);
        }
    }
}
