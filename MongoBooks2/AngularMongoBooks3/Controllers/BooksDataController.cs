using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularMongoBooks3.Controllers
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using AngularMongoBooks3.Controllers.DataClasses;
    using AngularMongoBooks3.Controllers.Settings;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;

    using BooksCore.Books;
    using BooksCore.Geography;
    using BooksCore.Interfaces;
    using BooksCore.Users;
    using BooksDatabase.Implementations;
    using BooksUtilities.ViewModels;

    [Route("api/[controller]")]
    public class BooksDataController : Controller
    {
        #region Constants

        /// <summary>
        /// The connection string for the database.
        /// </summary>
        public const string DatabaseConnectionString = "mongodb://localhost:27017";

        #endregion

        #region Private data

        /// <summary>
        /// The books read database.
        /// </summary>
        private readonly BooksReadDatabase _booksReadDatabase;

        /// <summary>
        /// The books read from database.
        /// </summary>
        private ObservableCollection<Book> _books;

        #endregion

        [HttpGet("[action]")]
        public IEnumerable<Book> GetAllBooks()
        {
            _booksReadDatabase.ConnectToDatabase();
            if (_booksReadDatabase.ReadFromDatabase)
            {
                _books = new ObservableCollection<Book>();

                foreach (var bookRead in _booksReadDatabase.LoadedItems)
                {
                    _books.Add(new Book(bookRead));
                }
            }
            return _books;
        }


        public BooksDataController(IOptions<MongoDbSettings> config)
        {
            MongoDbSettings dbSettings = config.Value;

            _books = new ObservableCollection<Book>();

            _booksReadDatabase = new BooksReadDatabase(dbSettings.DatabaseConnectionString);
        }
    }
}
