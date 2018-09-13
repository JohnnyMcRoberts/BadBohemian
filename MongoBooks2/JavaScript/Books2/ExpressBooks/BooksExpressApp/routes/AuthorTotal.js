"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
class AuthorTotal {
    constructor(book) {
        this.name = book.author;
        this.nation = book.nationality;
        this.language = book.originalLanguage;
        this.totalBooks = 1;
        this.totalPages = book.pages;
        this.books = new Array();
        this.books.push(book.title);
    }
    addBook(book) {
        this.totalBooks++;
        this.totalPages += book.pages;
        this.books.push(book.title);
    }
}
exports.AuthorTotal = AuthorTotal;
//# sourceMappingURL=AuthorTotal.js.map