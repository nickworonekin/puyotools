using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using PuyoTools.App.Formats.Archives;
using PuyoTools.App.Formats.Compression;
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
                .Select(x => new ArchiveCreatorFileEntry
                {
                    SourceFile = x,
                    Filename = Path.GetFileName(x),
                    FilenameInArchive = Path.GetFileName(x),
                })
                .ToArray();

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
