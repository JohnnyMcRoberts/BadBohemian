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
const MonthDate_1 = require("./MonthDate");
const BooksTotal_1 = require("./BooksTotal");
const MonthlyBooksRead_1 = require("./MonthlyBooksRead");
const router = express.Router();
var hostname;
var listBook;
var listBookItems;
function getMonthlyBooksRead(books) {
    const dict = new Collections.Dictionary();
    for (let book of books) {
        const month = new MonthDate_1.MonthDate(book.date);
        if (dict.containsKey(month)) {
            const monthSet = dict.getValue(month);
            monthSet.listBooksRead.push(new BookRead_1.BookRead(book));
            dict.setValue(month, monthSet);
        }
        else {
            const monthOfBooks = new MonthlyBooksRead_1.MonthlyBooksRead(book.date);
            monthOfBooks.listBooksRead.push(new BookRead_1.BookRead(book));
            dict.setValue(month, monthOfBooks);
        }
    }
    return dict.values();
}
function randomIntFromInterval(min, max) {
    return Math.floor(Math.random() * (max - min + 1) + min);
}
function getDifferenceInDays(start, end) {
    let diff = Math.abs(start.getTime() - end.getTime());
    let diffDays = Math.ceil(diff / (1000 * 3600 * 24));
    return diffDays;
}
router.get('/', (req, res) => {
    hostname = req.hostname;
    console.log('router.get allBooksByMonthMongo for host ' + hostname + ' / ');
    listBookItems = new Array();
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
            console.log('successfully got the books');
            //console.log('successfully got the books -> ', books.toString().slice(0, 50));
            let i = 0;
            for (let book in books) {
                if (books.hasOwnProperty(book)) {
                    listBook = books[i];
                    i++;
                    listBookItems.push(listBook);
                }
            }
            let bookMonths = getMonthlyBooksRead(listBookItems);
            let monthIndex = randomIntFromInterval(0, bookMonths.length);
            let randomMonth = bookMonths[monthIndex];
            randomMonth.updateTotals();
            const totalDays = getDifferenceInDays(listBookItems[0].date, listBookItems[listBookItems.length - 1].date);
            const overallTotals = new BooksTotal_1.BooksTotal(totalDays, listBookItems);
            res.render('allBooksByMonthMongo', {
                title: 'Express got a subset of books from DB for ' + randomMonth.monthDate + ' it has ' + randomMonth.monthDate.daysInMonth() + ' days. Overall ' + totalDays + ' Days',
                monthDate: randomMonth.monthDate,
                monthTotals: randomMonth.monthTotals,
                monthlyBreakdownTotals: randomMonth.breakdownTotals,
                overallTotals: overallTotals,
                books: randomMonth.listBooksRead
            });
        }
    });
});
exports.default = router;
//# sourceMappingURL=allBooksByMonthMongo.js.map