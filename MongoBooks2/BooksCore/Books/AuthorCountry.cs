// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorCountry.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The author country.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BooksCore.Books
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BooksCore.Geography;
    using BooksCore.Interfaces;

    public class AuthorCountry
    {
        private readonly IGeographyProvider _mainModel;

        public string Country { get; set; }

        public uint TotalPagesReadFromCountry
        {
            get
            {
                return AuthorsFromCountry.Aggregate<BookAuthor, uint>(0, (current, author) => current + author.TotalPages);
            }
        }

        public int TotalBooksReadFromCountry
        {
            get
            {
                return AuthorsFromCountry.Sum(author => author.TotalBooksReadBy);
            }
        }

        public List<BookAuthor> AuthorsFromCountry { get; set; }

        public uint TotalPagesWorldWide { get; set; }

        public int TotalBooksWorldWide { get; set; }

        public double PercentageOfBooksRead
        {
            get
            {
                if (TotalBooksWorldWide < 1) return 0.0;
                return 100.0 * (TotalBooksReadFromCountry / (double)TotalBooksWorldWide);
            }
        }

        public double PercentageOfPagesRead
        {
            get
            {
                if (TotalPagesWorldWide < 1) return 0.0;
                return 100.0 * (TotalPagesReadFromCountry / (double)TotalPagesWorldWide);
            }
        }

        /// <summary>
        /// Gets the image URI ready to be displayed.
        /// </summary>
        public Nation Nation
        {
            get
            {
                return _mainModel.Nations.FirstOrDefault(n => n.Name == Country);
            }
        }

        /// <summary>
        /// Gets the image URI ready to be displayed.
        /// </summary>
        public Uri DisplayImage
        {
            get
            {
                string imageUri = string.Empty;
                foreach (WorldCountry worldCountry in _mainModel.WorldCountries.Where(worldCountry => worldCountry.Country == Country))
                {
                    imageUri = worldCountry.FlagUrl;
                    break;
                }

                if (string.IsNullOrEmpty(imageUri) && !string.IsNullOrEmpty(Nation?.ImageUri))
                    imageUri = Nation.ImageUri;

                //var uri = new Uri("pack://application:,,,/Images/camera_image_cancel-32.png");
                var uri = new Uri("https://upload.wikimedia.org/wikipedia/commons/9/99/LEI0440_Leica_IIIf_chrom_-_Sn._580566_1951-52-M39_Blitzsynchron_front_view-6531_hf-.jpg");
                //var uri = new Uri("https://cdn.imgbin.com/15/23/19/imgbin-computer-icons-photography-cancel-button-wvfSEsjpZ5nx67nXqS23rS4N5.jpg");

                if (!string.IsNullOrEmpty(imageUri))
                {
                    try
                    {
                        uri = new Uri(imageUri);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        //throw;
                    }
                }

                return uri;
            }
        }

        public AuthorCountry(IGeographyProvider mainModel)
        {
            _mainModel = mainModel;
            Country = string.Empty;
            AuthorsFromCountry = new List<BookAuthor>();
            TotalPagesWorldWide = 1;
            TotalBooksWorldWide = 1;
        }
    }
}
