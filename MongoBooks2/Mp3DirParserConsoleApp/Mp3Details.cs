using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mp3DirParserConsoleApp
{
    public class Mp3Details
    {
        public string Artist { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Album { get; set; } = string.Empty;

        public string SourcePath { get; set; } = string.Empty;

        public int Year { get; set; } = 0;
        public int Track { get; set; } = 0;

        public Mp3Details(TagLib.File taggedFile, string path)
        {
            Artist =
                string.IsNullOrEmpty(taggedFile.Tag.FirstAlbumArtist)
                    ? taggedFile.Tag.FirstPerformer
                    : taggedFile.Tag.FirstAlbumArtist;
            Title = taggedFile.Tag.Title;
            Album = taggedFile.Tag.Album;
            Year = (int)taggedFile.Tag.Year;
            Track = (int)taggedFile.Tag.Track;
            SourcePath = path;
        }

        public string DisplayString()
        {
            string taggedFileDetails =
                "\n Source: " + SourcePath +
                "\n\t Artist: " + Artist +
                "\n\t Track: " + Title +
                "\n\t Album: " + Album +
                "\n\t Year: " + Year;

            return taggedFileDetails;
        }

        public Mp3Details()
        {
        }
    }
}
