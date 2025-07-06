using PuyoTools.App.Formats.Archives;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.App.Cli.Commands.Archives
{
    class SvmArchiveCreateCommand : ArchiveFormatCreateCommand
    {
        private readonly Option<bool> _filenamesOption;
        private readonly Option<bool> _globalIndexesOption;
        private readonly Option<bool> _formatsOption;
        private readonly Option<bool> _dimensionsOption;

        public SvmArchiveCreateCommand(SvmFormat format)
            : base(format)
        {
            _filenamesOption = new("--filenames")
            {
                Description = "Include filenames"
            };
            Add(_filenamesOption);

            _globalIndexesOption = new("--global-indexes")
            {
                Description = "Include global indexes"
            };
            Add(_globalIndexesOption);

            _formatsOption = new("--formats")
            {
                Description = "Include pixel & data formats"
            };
            Add(_formatsOption);

            _dimensionsOption = new("--dimensions")
            {
                Description = "Include texture dimensions"
            };
            Add(_dimensionsOption);
        }

        protected override ArchiveCreateOptions CreateOptions(ParseResult parseResult)
        {
            SvmArchiveCreateOptions options = new()
            {
                Filenames = parseResult.GetValue(_filenamesOption),
                GlobalIndexes = parseResult.GetValue(_globalIndexesOption),
                Formats = parseResult.GetValue(_formatsOption),
                Dimensions = parseResult.GetValue(_dimensionsOption),
            };

            SetBaseOptions(parseResult, options);

            return options;
        }
    }
}
