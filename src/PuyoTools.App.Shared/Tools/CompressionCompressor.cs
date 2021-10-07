using PuyoTools.App.Formats.Compression;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace PuyoTools.App.Tools
{
    class CompressionCompressor
    {
        private readonly ICompressionFormat format;
        private readonly CompressionCompressorOptions options;

        public CompressionCompressor(
            ICompressionFormat format,
            CompressionCompressorOptions options)
        {
            this.format = format;
            this.options = options;
        }

        public void Execute(
            IList<string> files,
            IProgress<ToolProgress> progress = null,
            CancellationToken cancellationToken = default)
        {
            for (int i = 0; i < files.Count; i++)
            {
                string file = files[i];

                // Report progress.
                progress?.Report(new ToolProgress((double)i / files.Count, file));

                // Let's open the file.
                // But, we're going to do this in a try catch in case any errors happen.
                try
                {
                    MemoryStream buffer = new MemoryStream();

                    using (FileStream source = File.OpenRead(file))
                    {
                        // Run it through the compressor.
                        format.GetCodec().Compress(source, buffer);
                    }

                    // Now that we have a decompressed file (we hope!), let's see what we need to do with it.
                    if (options.OverwriteSourceFile)
                    {
                        // Overwrite the source file. Ok, we can do that!
                        using (FileStream destination = File.Create(file))
                        {
                            buffer.WriteTo(destination);

                            // We are done here. Continue on with the next file
                            continue;
                        }
                    }

                    // Get the output path and create it if it does not exist.
                    string outPath = Path.Combine(Path.GetDirectoryName(file), "Compressed Files");
                    if (!Directory.Exists(outPath))
                    {
                        Directory.CreateDirectory(outPath);
                    }

                    // Time to write out the file
                    using (FileStream destination = File.Create(Path.Combine(outPath, Path.GetFileName(file))))
                    {
                        buffer.WriteTo(destination);
                    }

                    // Delete the source file if the user chose to
                    if (options.DeleteSourceFile)
                    {
                        File.Delete(file);
                    }
                }
                catch
                {
                    // Meh, just ignore the error.
                }
            }
        }
    }
}
