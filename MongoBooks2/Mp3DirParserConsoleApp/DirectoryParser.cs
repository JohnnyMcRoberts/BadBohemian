namespace Mp3DirParserConsoleApp
{
    public class DirectoryParser
    {
        private const string Extension = @"*.mp3";

        private readonly string _sourceDirectory;

        public DirectoryParser(string sourceDirectory)
        {
            _sourceDirectory = sourceDirectory;
        }

        public List<Mp3Details> GetItemsInDirectory(bool display = false)
        {
            // Get the info for the mp3 files in the directory
            DirectoryInfo directoryInfo = new DirectoryInfo(_sourceDirectory);
            FileInfo[] fileInfos = directoryInfo.GetFiles(Extension);

            // Loop through adding the file details
            List<Mp3Details> mp3Items = new List<Mp3Details>();
            foreach (FileInfo file in fileInfos)
            {
                // Try to get file file tags
                string source = _sourceDirectory + file.Name;
                TagLib.File taggedFile = TagLib.File.Create(source);
                if (taggedFile != null)
                {
                    // Extract the tag details and add to the list
                    Mp3Details mp3Item = new Mp3Details(taggedFile, source);
                    mp3Items.Add(mp3Item);

                    // if displaying show the details
                    if (display)
                    {
                        Console.WriteLine(mp3Item.DisplayString());
                    }
                }
            }

            return mp3Items;
        }
    }
}
