using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using PuyoTools.App.Cli.Commands.Compression;
using PuyoTools.App.Tools;

namespace PuyoTools.App.Cli.Commands.Textures
{
    class TextureDecodeCommand : Command
    {
        public TextureDecodeCommand()
            : base("decode", "Decode textures")
        {
            Option<string[]> inputOption = new("--input", "-i")
            {
                Description = "Files to decode (pattern matching supported).",
                Required = true,
            };
            Options.Add(inputOption);

            Option<string[]> excludeOption = new("--exclude")
            {
                Description = "Files to exclude from being decoded (pattern matching supported)."
            };
            Options.Add(excludeOption);

            Option<bool> overwriteOption = new("--overwrite")
            {
                Description = "Overwrite source texture file with its decoded texture file."
            };
            Options.Add(overwriteOption);

            Option<bool> deleteOption = new("--delete")
            {
                Description = "Delete source texture file on successful decode."
            };
            Options.Add(deleteOption);

            SetAction(parseResult =>
            {
                TextureDecodeOptions options = new()
                {
                    Input = parseResult.GetValue(inputOption),
                    Exclude = parseResult.GetValue(excludeOption),
                    Overwrite = parseResult.GetValue(overwriteOption),
                    Delete = parseResult.GetValue(deleteOption),
                };

                Execute(options, parseResult.Configuration.Output);
            });
        }

        private void Execute(TextureDecodeOptions options, TextWriter writer)
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
                writer.WriteLine($"Processing {x.File} ... ({x.Progress:P0})");
            });

            // Execute the tool
            var tool = new TextureDecoder(toolOptions);
            tool.Execute(files, progress);
        }
    }
}
