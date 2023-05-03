using System.Text.RegularExpressions;

namespace Mp3DirParserConsoleApp
{
    public class SongsOrderer
    {
        private readonly List<Mp3Details> _mp3Items;
        private readonly string _outputDirectory;

        public SongsOrderer(List<Mp3Details> mp3Items, string outputDirectory)
        {
            _mp3Items = mp3Items;
            _outputDirectory = outputDirectory;
        }

        public void AddAlbumDirectories()
        {
            // Group the songs by album artist
            Dictionary<string, List<Mp3Details>> mp3ItemsByArtist = 
                _mp3Items.GroupBy(x => x.Artist).ToDictionary(
                    g => g.Key, g => g.Select(x => x).ToList());

            // Go through creating the directories for each artist
            List<string> artists = mp3ItemsByArtist.Keys.Distinct().ToList();

            foreach (string artist in artists)
            {
                Console.WriteLine("Artist => \t" + artist);
                string cleanArtist = CleanName(artist);

                string artistPath = _outputDirectory + cleanArtist;

                if (CreateDirectoryForValidPath(artistPath))
                {
                    List<Mp3Details> artistSongs = mp3ItemsByArtist[artist];

                    Dictionary<string, List<Mp3Details>> artistSongsByAlbum =
                        artistSongs.GroupBy(x => x.Album).ToDictionary(
                            g => g.Key, g => g.Select(x => x).ToList());

                    List<string> albums = artistSongsByAlbum.Keys.Distinct().ToList();

                    foreach (string album in albums)
                    {
                        Console.WriteLine("\tAlbum => \t" + album);
                        string cleanAlbum = CleanName(album);

                        string albumPath = artistPath+ "\\" + cleanAlbum;

                        if (CreateDirectoryForValidPath(albumPath))
                        {

                            // Loop though copying the files.

                            foreach (Mp3Details mp3Details in artistSongsByAlbum[album])
                            {
                                string outputFile = albumPath + "\\" + Path.GetFileName(mp3Details.SourcePath);

                                File.Copy(mp3Details.SourcePath, outputFile);
                            }
                        }
                    }
                }

            }

        }

        string CleanName(string source)
        {
            string fixedName = source.Replace('?', ' ');
            fixedName = fixedName.Replace('\\', ' ');
            fixedName = fixedName.Replace('/', ' ');
            fixedName = fixedName.Replace(':', ' ');
            fixedName = fixedName.Replace('*', ' ');
            fixedName = fixedName.Replace('"', ' ');
            fixedName = fixedName.Replace('>', ' ');
            fixedName = fixedName.Replace('<', ' ');


            return fixedName;
        }


        private bool CreateDirectoryForValidPath(string path)
        {
            Regex driveCheck = new Regex(@"^[a-zA-Z]:\\$");
            if (!driveCheck.IsMatch(path.Substring(0, 3))) return false;
            string strTheseAreInvalidFileNameChars = new string(Path.GetInvalidPathChars());
            strTheseAreInvalidFileNameChars += @":/?*" + "\"";
            Regex containsABadCharacter = new Regex("[" + Regex.Escape(strTheseAreInvalidFileNameChars) + "]");
            if (containsABadCharacter.IsMatch(path.Substring(3, path.Length - 3)))
                return false;

            DirectoryInfo dir = new DirectoryInfo(Path.GetFullPath(path));
            if (!dir.Exists)
                dir.Create();
            return true;
        }
    }
}
