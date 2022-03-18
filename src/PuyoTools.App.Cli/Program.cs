using PuyoTools.App.Cli.Commands.Archives;
using PuyoTools.App.Cli.Commands.Compression;
using PuyoTools.App.Cli.Commands.Textures;
using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
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

            var parser = new CommandLineBuilder(rootCommand)
                .UseDefaults()
                .UseExceptionHandler(ExceptionHandler)
                .Build();

            return await parser.InvokeAsync(args);
        }

        static void ExceptionHandler(Exception e, InvocationContext context)
        {
            if (e is System.Reflection.TargetInvocationException)
            {
                e = e.InnerException;
            }

            Console.ForegroundColor = ConsoleColor.Red;
#if DEBUG
            Console.Error.WriteLine(e);
#else
            Console.Error.WriteLine(e.Message);
#endif
            Console.ResetColor();

            context.ExitCode = e.HResult;
        }
    }
}
