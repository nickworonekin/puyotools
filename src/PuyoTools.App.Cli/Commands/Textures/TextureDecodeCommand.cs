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

namespace PuyoTools.App.Cli.Commands.Textures
{
    class TextureDecodeCommand : Command
    {
        public TextureDecodeCommand()
            : base("decode", "Decode textures")
        {
            AddOption(new Option<string[]>(new string[] { "-i", "--input" }, "Files to decode (pattern matching supported).")
            {
                IsRequired = true,
            });
            AddOption(new Option<string[]>("--exclude", "Files to exclude from being decoded (pattern matching supported)."));
            AddOption(new Option<bool>("--compressed", "Decode compressed textures"));
            AddOption(new Option<bool>("--overwrite", "Overwrite source texture file with its decoded texture file."));
            AddOption(new Option<bool>("--delete", "Delete source texture file on successful decode."));

            Handler = CommandHandler.Create<TextureDecodeOptions, IConsole>(Execute);
        }

        private void Execute(TextureDecodeOptions options, IConsole console)
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
            var toolOptions = new TextureDecoderOptions
            {
                DecodeCompressedTextures = options.Compressed,
                OutputToSourceDirectory = options.Overwrite,
                DeleteSource = options.Delete,
            };

            // Create the progress handler (only if the quiet option is not set)
            var progress = new SynchronousProgress<ToolProgress>(x =>
            {
                console.Out.WriteLine($"Processing {x.File} ... ({x.Progress:P0})");
            });

            // Execute the tool
            var tool = new TextureDecoder(toolOptions);
            tool.Execute(files, progress);
        }
    }
}
