using System;
using System.Collections.Generic;
using System.Linq;


namespace MongoDbBooks.Models
{
    using MongoDbBooks.Models.Database;

    public class AuthorCountry
    {
        private readonly MainBooksModel _mainModel;

        public string Country { get; set; }

        public UInt32 TotalPagesReadFromCountry
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

        public UInt32 TotalPagesWorldWide { get; set; }
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

                return string.IsNullOrEmpty(imageUri) ? new Uri("pack://application:,,,/Images/camera_image_cancel-32.png") : new Uri(imageUri);
            }
        }

        public AuthorCountry(MainBooksModel mainModel)
        {
            _mainModel = mainModel;
            Country = "";
            AuthorsFromCountry = new List<BookAuthor>();
            TotalPagesWorldWide = 1;
            TotalBooksWorldWide = 1;
        }
    }
}
