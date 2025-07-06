using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using PuyoTools.App.Tools;
using System;
using System.Collections.Generic;
using System.CommandLine;
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
            Option<string[]> inputOption = new("--input", "-i")
            {
                Description = "Archives to extract (pattern matching supported).",
                Required = true,
            };
            Add(inputOption);

            Option<string[]> excludeOption = new("--exclude")
            {
                Description = "Archives to exclude from being extracted (pattern matching supported)."
            };
            Add(excludeOption);

            Option<bool> decompressOption = new("--decompress")
            {
                Description = "Extract compressed archives"
            };
            Add(decompressOption);

            Option<bool> extractSourceFolderOption = new("--extract-source-folder")
            {
                Description = "Extract files to the same folder as the source archive."
            };
            Add(extractSourceFolderOption);

            Option<bool> extractSameNameOption = new("--extract-same-name")
            {
                Description = "Extract files to a folder with the same name as the source archive (and delete the source archive)."
            };
            Add(extractSameNameOption);

            Option<bool> deleteOption = new("--delete")
            {
                Description = "Delete archive on successful extraction."
            };
            Add(deleteOption);

            Option<bool> decompressExtractedOption = new("--decompress-extracted")
            {
                Description = "Decompress extracted files."
            };
            Add(decompressExtractedOption);

            Option<bool> fileNumberOption = new("--file-number")
            {
                Description = "Use file number as filename."
            };
            Add(fileNumberOption);

            Option<bool> prependFileNumberOption = new("--prepend-file-number")
            {
                Description = "Prepend file number to filename."
            };
            Add(prependFileNumberOption);

            Option<bool> extractIfArchiveOption = new("--extract-if-archive")
            {
                Description = "Extract extracted files that are archives."
            };
            Add(extractIfArchiveOption);

            Option<bool> decodeIfTextureOption = new("--decode-if-texture")
            {
                Description = "Decode extracted files that are textures."
            };
            Add(decodeIfTextureOption);

            Option<bool> verboseOption = new("--verbose")
            {
                Description = "Show verbose output for archives being extracted"
            };
            Add(verboseOption);

            SetAction(parseResult =>
            {
                ArchiveExtractOptions options = new()
                {
                    Input = parseResult.GetValue(inputOption),
                    Exclude = parseResult.GetValue(excludeOption),
                    Decompress = parseResult.GetValue(decompressOption),
                    ExtractSourceFolder = parseResult.GetValue(extractSourceFolderOption),
                    ExtractSameName = parseResult.GetValue(extractSameNameOption),
                    Delete = parseResult.GetValue(deleteOption),
                    DecompressExtracted = parseResult.GetValue(decompressExtractedOption),
                    FileNumber = parseResult.GetValue(fileNumberOption),
                    PrependFileNumber = parseResult.GetValue(prependFileNumberOption),
                    //ExtractIfArchiveNumber = parseResult.GetValue(extractIfArchiveOption),
                    //DecodeIfTextureNumber = parseResult.GetValue(decodeIfTextureOption),
                    Verbose = parseResult.GetValue(verboseOption),
                };

                Execute(options, parseResult.Configuration.Output);
            });
        }

        private void Execute(ArchiveExtractOptions options, TextWriter writer)
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
                        writer.Write($"-- Extracting {x.Entry} ... ({x.Progress:P0})\n");
                    }
                }
                else
                {
                    writer.Write($"Processing {x.File} ... ({x.Progress:P0})\n");
                }
            });

            // Execute the tool
            var tool = new ArchiveExtractor(toolOptions);
            tool.Execute(files, progress);
        }
    }
}
