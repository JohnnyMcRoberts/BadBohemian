"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
/*
 * GET all books listing.
 */
const express = require("express");
const router = express.Router();
var hostname;
router.get('/', (req, res) => {
    console.log('router.get allBooks / ');
    hostname = req.hostname;
    res.render('allBooks', { title: 'Express all books', extra: 'some extra content', user: hostname });
});
exports.default = router;
//# sourceMappingURL=allBooks.js.map