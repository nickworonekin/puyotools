using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using PuyoTools.App.Formats.Textures;
using PuyoTools.App.Tools;

namespace PuyoTools.App.Cli.Commands.Textures
{
    class TextureFormatEncodeCommand : Command
    {
        private readonly ITextureFormat _format;

        private readonly Option<string[]> _inputOption;
        private readonly Option<string[]> _excludeOption;

        public TextureFormatEncodeCommand(ITextureFormat format)
            : base(format.CommandName, $"Create {format.Name} texture")
        {
            _format = format;

            _inputOption = new("--input", "-i")
            {
                Description = "Files to encode (pattern matching supported).",
                Required = true,
            };
            Options.Add(_inputOption);

            _excludeOption = new("--exclude")
            {
                Description = "Files to exclude from being encoded (pattern matching supported)."
            };
            Options.Add(_excludeOption);

            SetAction(parseResult => Execute(CreateOptions(parseResult), parseResult.Configuration.Output));
        }

        protected virtual TextureFormatEncodeOptions CreateOptions(ParseResult parseResult)
        {
            TextureFormatEncodeOptions options = new();
            SetBaseOptions(parseResult, options);
            return options;
        }

        protected void SetBaseOptions(ParseResult parseResult, TextureFormatEncodeOptions options)
        {
            options.Input = parseResult.GetRequiredValue(_inputOption);
            options.Exclude = parseResult.GetValue(_excludeOption);
        }

        protected void Execute(TextureFormatEncodeOptions options, TextWriter writer)
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
                writer.WriteLine($"Processing {x.File} ... ({x.Progress:P0})");
            });

            // Execute the tool
            var tool = new TextureEncoder(_format, toolOptions, options as ITextureFormatOptions);
            tool.Execute(files, progress);
        }
    }
}
