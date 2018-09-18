"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
/*
 * allBooks.ts
 * GET all books.
 */
const express = require("express");
const router = express.Router();
var hostname;
router.get('/', (req, res) => {
    console.log('router.get authors / ');
    hostname = req.hostname;
    res.send({
        authors: [{ name: 'Joyce' }, { name: 'Becket' }]
    });
});
exports.default = router;
//# sourceMappingURL=authors.js.map