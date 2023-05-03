using System.Text.Json;
using Newtonsoft.Json;
using File = TagLib.File;

namespace Mp3DirParserConsoleApp
{
    internal class Program
    {

        private static readonly string SourceDir = @"C:\Users\jonathan.Mcroberts\Downloads\My 2007\";

        private static readonly string OuputDir = @"C:\Users\jonathan.Mcroberts\Downloads\Organised 2007\";

        static void Main(string[] args)
        {
            // See https://aka.ms/new-console-template for more information
            Console.WriteLine("Hello, MP3 Directory Parser");

            DirectoryParser parser = new DirectoryParser(SourceDir);
            List<Mp3Details> mp3Items = parser.GetItemsInDirectory();
            SongsOrderer orderer = new SongsOrderer(mp3Items, OuputDir);
            orderer.AddAlbumDirectories();
        }
    }
}

