"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
/*
 * allBooks.ts
 * GET all books.
 */
const express = require("express");
const book_1 = require("./book");
const router = express.Router();
var hostname;
router.get('/', (req, res) => {
    console.log('router.get allBooks / ');
    hostname = req.hostname;
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
            res.send({ allBooks: books });
            //res.send(
            //  {
            //    title: 'Express got all books from DB',
            //    books: books
            //  });
        }
    });
});
exports.default = router;
//# sourceMappingURL=allBooks.js.map