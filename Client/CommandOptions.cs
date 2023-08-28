using CommandLine;
using CommandLine.Text;

namespace Client
{
    public class CommandOptions
    {
        [Option('f', "fileGlobPattern", Required = true, HelpText = "Files to be searched.")]
        public string FileGlobPattern { get; set; }

        [Option('d', "destionationDirectoryPath", Required = true, HelpText = "Destination directory path")]
        public string DestinationDirectoryPath { get; set; }

        [Option('s', "sourceDirectoryPath", HelpText = "Parent directory where the files will be..")]
        public string SourceDirectoryPath { get; set; }

        [Option('o', "overwriteTargetFiles", Default = false, Required = false, HelpText ="If passed true, copier will overwrite existing files at the target location.")]
        public bool OverwriteTargetFiles { get; set; }

        [Option('v', "verbose", Default = false, Required = false, HelpText ="If passed true, more information will be outputted to the console")]
        public bool Verbose { get; set; }

        [Usage]
        public static IEnumerable<Example> Examples => new List<Example>()
        {
            new Example("Start the copier", new UnParserSettings(){PreferShortName = true},
                new CommandOptions
                {
                    SourceDirectoryPath = "C://Users/MyDocuments/Images",
                    FileGlobPattern = "*.jpg",
                    DestinationDirectoryPath = "C:/Users/MyDocuments/NewImages"
                })
        };


    }
}

