"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
/*
 * allBooksByMonthMongo.ts
 * GET all books by month from mongo.
 */
const express = require("express");
const book_1 = require("./book");
const MonthlyBooksReports_1 = require("./MonthlyBooksReports");
const router = express.Router();
var hostname;
var listBook;
var listBookItems;
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
            let i = 0;
            for (let book in books) {
                if (books.hasOwnProperty(book)) {
                    listBook = books[i];
                    i++;
                    listBookItems.push(listBook);
                }
            }
            let monthlyBooksReports = new MonthlyBooksReports_1.MonthlyBooksReports(listBookItems);
            let bookMonths = monthlyBooksReports.monthlyBooksRead;
            let monthIndex = randomIntFromInterval(0, bookMonths.length);
            let randomMonth = bookMonths[monthIndex];
            const totalDays = getDifferenceInDays(listBookItems[0].date, listBookItems[listBookItems.length - 1].date);
            res.render('allBooksByMonthMongo', {
                title: 'Express got a subset of books from DB for ' + randomMonth.monthDate + ' it has ' + randomMonth.monthDate.daysInMonth() + ' days. Overall ' + totalDays + ' Days',
                monthDate: randomMonth.monthDate,
                monthTotals: randomMonth.monthTotals,
                monthlyBreakdownTotals: randomMonth.breakdownTotals,
                overallTotals: monthlyBooksReports.overallTotals,
                books: randomMonth.listBooksRead,
                availableMonthsByYear: monthlyBooksReports.monthlyReportYears
            });
        }
    });
});
exports.default = router;
//# sourceMappingURL=allBooksByMonthMongo.js.map