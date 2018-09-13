"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
/*
 * allAuthorsMongo.ts
 * GET all authors mongo.
 */
const express = require("express");
const book_1 = require("./book");
const AuthorTotals_1 = require("./AuthorTotals");
const router = express.Router();
var hostname;
router.get('/', (req, res) => {
    console.log('router.get allAuthorsMongo / ');
    hostname = req.hostname;
    let books = book_1.default.find((err, books) => {
        if (err) {
            console.log('error in books find - ', err);
            res.render('allAuthorsMongo', {
                title: 'Express all books from DB error!',
                books: [
                    { author: "A bad thing", title: "but worse" },
                    { author: "is going", title: "To follow" }
                ]
            });
        }
        else {
            const authorTotals = new AuthorTotals_1.AuthorTotals(books);
            res.render('allAuthorsMongo', {
                title: 'Express got all books and authors from DB',
                authorTotals: authorTotals
            });
        }
    });
});
exports.default = router;
//# sourceMappingURL=allAuthorsMongo.js.map