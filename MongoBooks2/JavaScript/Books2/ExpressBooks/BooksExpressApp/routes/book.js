"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
// book.ts
const mongoose = require("mongoose");
//const uri: string = 'mongodb://127.0.0.1:27017/local';
const uri = 'mongodb://127.0.0.1:27017/books_read';
mongoose.connect(uri, (err) => {
    if (err) {
        console.log(err.message);
    }
    else {
        console.log("Succesfully Connected!");
    }
});
;
exports.BookSchema = new mongoose.Schema({
    title: { type: String, required: true },
    author: { type: String, required: true },
});
//const Book = mongoose.model('Book', BookSchema);
const Book = mongoose.model('books', exports.BookSchema);
exports.default = Book;
//# sourceMappingURL=book.js.map