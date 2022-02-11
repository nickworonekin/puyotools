using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using PuyoTools.App.Formats.Textures;
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

namespace PuyoTools.App.Cli.Commands.Textures
{
    class TextureFormatEncodeCommand : Command
    {
        private readonly ITextureFormat format;

        public TextureFormatEncodeCommand(ITextureFormat format)
            : base(format.CommandName, $"Create {format.Name} texture")
        {
            this.format = format;

            AddOption(new Option<string[]>(new string[] { "-i", "--input" }, "Files to encode (pattern matching supported).")
            {
                IsRequired = true,
            });
            AddOption(new Option<string[]>("--exclude", "Files to exclude from being encoded (pattern matching supported)."));

            Handler = CommandHandler.Create<TextureFormatEncodeOptions, IConsole>(Execute);
        }

        protected void Execute(TextureFormatEncodeOptions options, IConsole console)
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
            var toolOptions = new TextureEncoderOptions
            {
            };

            // Create the progress handler (only if the quiet option is not set)
            var progress = new SynchronousProgress<ToolProgress>(x =>
            {
                console.Out.WriteLine($"Processing {x.File} ... ({x.Progress:P0})");
            });

            // Execute the tool
            var tool = new TextureEncoder(format, toolOptions, options as ITextureFormatOptions);
            tool.Execute(files, progress);
        }
    }
}
