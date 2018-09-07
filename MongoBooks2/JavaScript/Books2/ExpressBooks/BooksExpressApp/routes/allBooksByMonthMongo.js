"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
/*
 * allBooksByMonthMongo.ts
 * GET all books by month from mongo.
 */
const express = require("express");
const Collections = require("typescript-collections");
const book_1 = require("./book");
const BookRead_1 = require("./BookRead");
const MonthlyBooksRead_1 = require("./MonthlyBooksRead");
const router = express.Router();
var hostname;
var listBook;
var listBookItems;
var listBooksRead;
function getMonthlyBooksRead(books) {
    var dict = new Collections.Dictionary();
    for (let book of books) {
        const month = new MonthlyBooksRead_1.MonthDate(book.date);
        var monthName = month.year.toString() + '/' + month.month.toString();
        if (dict.containsKey(monthName)) {
            const monthSet = dict.getValue(monthName);
            monthSet.listBooksRead.push(new BookRead_1.BookRead(book));
            dict.setValue(monthName, monthSet);
            console.log('adding book ' + book.title + ' to existing month ' + month); // "4", "5", "6"
        }
        else {
            const monthOfBooks = new MonthlyBooksRead_1.MonthlyBooksRead(book.date);
            monthOfBooks.listBooksRead.push(new BookRead_1.BookRead(book));
            dict.setValue(monthName, monthOfBooks);
            console.log('adding book ' + book.title + ' to new month ' + month); // "4", "5", "6"
        }
    }
    return dict.values();
}
router.get('/', (req, res) => {
    console.log('router.get allBooksByMonthMongo / ');
    hostname = req.hostname;
    listBookItems = new Array();
    listBooksRead = new Array();
    let books = book_1.default.find((err, books) => {
        if (err) {
            console.log('error in books find - ', err);
            res.render('allBooksMongo', {
                title: 'Express all books from DB error!',
                books: [
                    { author: "A bad thing", title: "but worse" },
                    { author: "is going", title: "To follow" }
                ]
            });
            return;
        }
        else {
            console.log('successfully got the books -> ', books.toString().slice(0, 50));
            let i = 0;
            for (let book in books) {
                if (books.hasOwnProperty(book)) {
                    listBook = books[i];
                    //console.log('successfully got the book ' + i + ' -> ', listBook);
                    i++;
                    var bookRead = new BookRead_1.BookRead(listBook);
                    listBookItems.push(listBook);
                    if (i > 5)
                        continue;
                    listBooksRead.push(bookRead);
                }
            }
            var bookMonths = getMonthlyBooksRead(listBookItems);
            var fourthMonth = bookMonths[4];
            res.render('allBooksByMonthMongo', {
                title: 'Express got a subset of books from DB',
                books: fourthMonth.listBooksRead
            });
        }
    });
});
exports.default = router;
//# sourceMappingURL=allBooksByMonthMongo.js.map