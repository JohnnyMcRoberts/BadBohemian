"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
/*
 * PUT add dummy book.
 */
const express = require("express");
const router = express.Router();
const book_1 = require("./book");
var hostname;
router.post('/', (req, res) => {
    console.log('router.put addDummyBook / ');
    hostname = req.hostname;
    //var book = new Book(req.body);
    //var displayDate = new Date().toLocaleDateString();
    var displayTime = new Date().toISOString();
    var book = new book_1.default({ title: "Title created on " + displayTime, author: "Dummy" });
    //book.save((err: any) => {
    //  if (err) {
    //    res.send(err);
    //  } else {
    //    res.send(book);
    //  }
    //});
});
exports.default = router;
//# sourceMappingURL=addDummyBook.js.map