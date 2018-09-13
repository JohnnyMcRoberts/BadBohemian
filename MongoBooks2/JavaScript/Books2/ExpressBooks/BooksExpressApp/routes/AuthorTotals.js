"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
/*
 * AuthorTotals.ts
 * the author totals
 */
const AuthorTotal_1 = require("./AuthorTotal");
const Collections = require("typescript-collections");
class AuthorTotals {
    constructor(listBooksRead) {
        const dict = new Collections.Dictionary();
        for (let book of listBooksRead) {
            const author = book.author;
            if (dict.containsKey(author)) {
                const authorTotal = dict.getValue(author);
                authorTotal.addBook(book);
                dict.setValue(author, authorTotal);
            }
            else {
                dict.setValue(author, new AuthorTotal_1.AuthorTotal(book));
            }
        }
        this.authors = dict.values();
    }
}
exports.AuthorTotals = AuthorTotals;
//# sourceMappingURL=AuthorTotals.js.map