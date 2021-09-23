using PuyoTools.App.Cli.Commands.Archives;
using PuyoTools.App.Cli.Commands.Compression;
using PuyoTools.App.Cli.Commands.Textures;
using System;
using System.CommandLine;
using System.Threading.Tasks;

namespace PuyoTools.App.Cli
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand
            {
                new Command("compression", "Perform compression-based actions on files")
                {
                    new CompressionCompressCommand(),
                    new CompressionDecompressCommand(),
                },
                new Command("archive", "Perform actions on archives")
                {
                    new ArchiveCreateCommand(),
                    new ArchiveExtractCommand(),
                },
                new Command("texture", "Perform actions on textures")
                {
                    new TextureEncodeCommand(),
                    new TextureDecodeCommand(),
                },
            };

            return await rootCommand.InvokeAsync(args);
        }
    }
}
