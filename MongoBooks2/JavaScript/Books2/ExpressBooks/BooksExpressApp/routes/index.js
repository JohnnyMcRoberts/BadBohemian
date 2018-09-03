"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
/*
 * GET home page.
 */
const express = require("express");
const router = express.Router();
router.get('/', (req, res) => {
    console.log('router.get / ');
    res.render('index', { title: 'Express (Not the jungle or Terrordome)' });
});
exports.default = router;
//# sourceMappingURL=index.js.map