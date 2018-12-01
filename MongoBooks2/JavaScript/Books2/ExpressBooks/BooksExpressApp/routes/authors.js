"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
/*
 * allBooks.ts
 * GET all books.
 */
const express = require("express");
const router = express.Router();
var hostname;
//export interface Author {
//  name: string;
//}
class BaseAuthor {
    constructor(name) {
        this.name = name;
    }
}
exports.BaseAuthor = BaseAuthor;
class Author {
    constructor() { }
}
exports.Author = Author;
router.get('/', (req, res) => {
    console.log('router.get authors / ');
    hostname = req.hostname;
    //res.send([{ name: 'Joyce' }, { name: 'Becket' }]);
    let items = new Array();
    let a1 = new Author();
    a1.name = "Elliot";
    let a2 = new Author();
    a2.name = "Pound";
    items.push(a1);
    items.push(a2);
    //res.json(items);//.send([{ name: 'Joyce' }, { name: 'Becket' }]);
    //res.status(200).send(items);
    var data = ([
        {
            "name": "Mann"
        },
        {
            "name": "Joyce"
        },
        {
            "name": "Wolff"
        }
    ]);
    res.setHeader("Access-Control-Allow-Credentials", "true");
    res.setHeader("Cache-Control", "no-cache");
    res.setHeader("Pragma", "no-cache");
    res.setHeader("Vary", "Origin, Accept-Encoding");
    res.setHeader("Expires", "-1");
    res.setHeader("X-Content-Type-Options", "nosniff");
    res.status(200).send(data);
});
exports.default = router;
//# sourceMappingURL=authors.js.map