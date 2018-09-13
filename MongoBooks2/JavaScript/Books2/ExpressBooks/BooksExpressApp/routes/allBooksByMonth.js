"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
/*
 * allBooksByMonth.ts
 * GET all books by month for reports.
 */
const express = require("express");
const book_1 = require("./book");
const MonthlyBooksReports_1 = require("./MonthlyBooksReports");
const router = express.Router();
router.get('/', (req, res) => {
    console.log('router.get allBooksByMonth / ');
    let books = book_1.default.find((err, books) => {
        if (err) {
            console.log('error in books find - ', err);
            res.send({
                title: 'Express all books from DB error!',
                books: [
                    { author: "A bad thing", title: "but worse" },
                    { author: "is going", title: "To follow" }
                ]
            });
        }
        else {
            let i = 0;
            const listBookItems = new Array();
            for (let book in books) {
                if (books.hasOwnProperty(book)) {
                    listBookItems.push(books[i]);
                    i++;
                }
            }
            let monthlyBooksReports = new MonthlyBooksReports_1.MonthlyBooksReports(listBookItems);
            res.send({
                title: 'Express got all monthly books reports from DB',
                monthlyBooksReports: monthlyBooksReports
            });
        }
    });
});
exports.default = router;
//# sourceMappingURL=allBooksByMonth.js.map