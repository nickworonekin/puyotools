using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using PuyoTools.App.Formats.Archives;
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

namespace PuyoTools.App.Cli.Commands.Archives
{
    class ArchiveFormatCreateCommand : Command
    {
        private readonly IArchiveFormat format;

        public ArchiveFormatCreateCommand(IArchiveFormat format)
            : base(format.CommandName, $"Create {format.Name} archive")
        {
            this.format = format;

            AddOption(new Option<string[]>(new string[] { "-i", "--input" }, "Files to add to the archive (pattern matching supported).")
            {
                IsRequired = true,
                AllowMultipleArgumentsPerToken = true,
            });
            AddOption(new Option<string[]>("--exclude", "Files to exclude from being added to the archive (pattern matching supported)."));
            AddOption(new Option<string>(new string[] { "-o", "--output" }, "The name of the archive to create.")
            {
                IsRequired = true,
            });
            AddOption(new Option<string>("--compress", "Compress the archive")
                .FromAmong(CompressionFactory.EncoderFormats.Select(x => x.CommandName).ToArray()));

            Handler = CommandHandler.Create<ArchiveCreateOptions, IConsole>(Execute);
        }

        protected void Execute(ArchiveCreateOptions options, IConsole console)
        {
            var files = new List<ArchiveCreatorFileEntry>();
            
            foreach (var input in options.Input)
            {
                string filename = input;
                //string filenameInArchive = Path.GetFileName(filename);
                string? filenameInArchive = null;
                int seperatorIndex = input.IndexOf(',');
                if (seperatorIndex != -1)
                {
                    filename = input.Substring(0, seperatorIndex);
                    filenameInArchive = input.Substring(seperatorIndex + 1);
                }

                // Get the files to process by the tool
                // To ensure files are added in the order specified, they will be matched seperately.
                var matcher = new Matcher();
                matcher.AddInclude(filename);
                if (options.Exclude?.Any() == true)
                {
                    matcher.AddExcludePatterns(options.Exclude);
                }

                var matchedFiles = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(Environment.CurrentDirectory)))
                    .Files
                    .Select(x => x.Path)
                    .Select(x => new ArchiveCreatorFileEntry
                    {
                        SourceFile = x,
                        //Filename = filename,
                        //FilenameInArchive = filenameInArchive,
                        Filename = x,
                        FilenameInArchive = filenameInArchive ?? x,
                    })
                    .ToArray();

                files.AddRange(matchedFiles);
            }

            // Create options in the format the tool uses
            var toolOptions = new ArchiveCreatorOptions
            {
                CompressionFormat = options.Compress is not null
                    ? CompressionFactory.EncoderFormats.FirstOrDefault(x => x.CommandName == options.Compress)
                    : null,
            };

            // Create the progress handler (only if the quiet option is not set)
            var progress = new SynchronousProgress<ToolProgress>(x =>
            {
                console.Out.WriteLine($"Processing {x.File} ... ({x.Progress:P0})");
            });

            // Execute the tool
            var tool = new ArchiveCreator(format, toolOptions, options as IArchiveFormatOptions);
            tool.Execute(files, options.Output, progress);
        }
    }
}
