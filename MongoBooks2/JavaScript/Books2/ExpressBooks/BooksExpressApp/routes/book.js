"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
// book.ts
const mongoose = require("mongoose");
//const uri: string = 'mongodb://127.0.0.1:27017/local';
const mongoIP = '127.0.0.1';
const mongoPort = '27017';
const databaseName = 'books_read';
const uri = 'mongodb://' + mongoIP + ':' + mongoPort + '/' + databaseName;
//const uri: string = 'mongodb://127.0.0.1:27017/books_read';
mongoose.connect(uri, (err) => {
    if (err) {
        console.log(err.message);
    }
    else {
        console.log("Succesfully Connected to " + uri + "!");
    }
});
var Format;
(function (Format) {
    Format[Format["Book"] = 1] = "Book";
    Format[Format["Comic"] = 2] = "Comic";
    Format[Format["Audio"] = 3] = "Audio";
})(Format = exports.Format || (exports.Format = {}));
;
;
;
exports.BookSchema = new mongoose.Schema({
    _id: { type: String, required: true },
    dateString: { type: String, required: true },
    date: { type: Date, required: true },
    title: { type: String, required: true },
    author: { type: String, required: true },
    pages: { type: Number, required: true },
    note: { type: String, required: false },
    nationality: { type: String, required: true },
    originalLanguage: { type: String, required: true },
    image_url: { type: String, required: true },
    tags: [{
            type: String
        }],
    format: { type: Number, required: true }
});
//const Book = mongoose.model('Book', BookSchema);
const Book = mongoose.model('books', exports.BookSchema);
exports.default = Book;
//# sourceMappingURL=book.js.map