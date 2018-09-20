"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
/*
 * allBooks.ts
 * GET all books.
 */
const express = require("express");
const router = express.Router();
var hostname;
class BaseAuthor {
    constructor(name) {
        this.name = name;
    }
}
exports.BaseAuthor = BaseAuthor;
router.get('/', (req, res) => {
    console.log('router.get authors / ');
    hostname = req.hostname;
    //res.send([{ name: 'Joyce' }, { name: 'Becket' }]);
    let items = new Array();
    items.push(new BaseAuthor('Joyce'));
    items.push(new BaseAuthor('Wolff'));
    res.json(items); //.send([{ name: 'Joyce' }, { name: 'Becket' }]);
});
exports.default = router;
//# sourceMappingURL=authors.js.map