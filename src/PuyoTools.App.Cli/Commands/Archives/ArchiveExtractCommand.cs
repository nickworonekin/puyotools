using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using PuyoTools.App.Tools;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
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
            AddOption(new Option<bool>("--decompress", "Extract compressed archives"));
            AddOption(new Option<bool>("--extract-source-folder", "Extract files to the same folder as the source archive."));
            AddOption(new Option<bool>("--extract-same-name", "Extract files to a folder with the same name as the source archive (and delete the source archive)."));
            AddOption(new Option<bool>("--delete", "Delete archive on successful extraction."));
            AddOption(new Option<bool>("--decompress-extracted", "Decompress extracted files."));
            AddOption(new Option<bool>("--file-number", "Use file number as filename."));
            AddOption(new Option<bool>("--prepend-file-number", "Prepend file number to filename."));
            AddOption(new Option<bool>("--extract-if-archive", "Extract extracted files that are archives."));
            AddOption(new Option<bool>("--decode-if-texture", "Decode extracted files that are textures."));
            AddOption(new Option<bool>("--verbose", "Show verbose output for archives being extracted"));

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
                DecompressSourceArchive = options.Decompress,
                ExtractToSourceDirectory = options.ExtractSourceFolder,
                ExtractToSameNameDirectory = options.ExtractSameName,
                DeleteSourceArchive = options.Delete,
                DecompressExtractedFiles = options.DecompressExtracted,
                FileNumberAsFilename = options.FileNumber,
                PrependFileNumber = options.PrependFileNumber,
                ExtractExtractedArchives = options.ExtractIfArchive,
                ConvertExtractedTextures = options.DecompressExtracted,
            };

            // Create the progress handler (only if the quiet option is not set)
            var progress = new SynchronousProgress<ArchiveExtractorProgress>(x =>
            {
                if (x.Entry is not null) // An archive entry is being extracted
                {
                    if (options.Verbose)
                    {
                        console.Out.Write($"-- Extracting {x.Entry} ... ({x.Progress:P0})\n");
                    }
                }
                else
                {
                    console.Out.Write($"Processing {x.File} ... ({x.Progress:P0})\n");
                }
            });

            // Execute the tool
            var tool = new ArchiveExtractor(toolOptions);
            tool.Execute(files, progress);
        }
    }
}
