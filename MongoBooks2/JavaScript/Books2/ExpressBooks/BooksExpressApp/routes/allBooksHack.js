"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
/*
 * GET all books listing.
 */
const express = require("express");
const router = express.Router();
var hostname;
router.get('/', (req, res) => {
    console.log('router.get allBooksHack / ');
    hostname = req.hostname;
    res.render('allBooksHack', {
        title: 'Express all books hack',
        extra: 'some extra content -poor',
        user: hostname,
        books: [
            { author: "Georges Simenon", title: "Maigret is Afraid" },
            { author: "Richard House", title: "The Kills" },
            { author: "Ann Quin", title: "The Unmapped Country" }
        ]
    });
});
exports.default = router;
//# sourceMappingURL=allBooksHack.js.map