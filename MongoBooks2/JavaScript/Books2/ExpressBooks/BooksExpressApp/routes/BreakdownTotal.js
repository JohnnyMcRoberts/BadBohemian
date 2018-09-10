"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const Collections = require("typescript-collections");
class BreakdownElement {
    constructor(title, total) {
        this.title = title;
        this.total = total;
    }
}
exports.BreakdownElement = BreakdownElement;
class BreakdownTotal {
    constructor(isLanguage, isPages, books) {
        this.isLanguage = isLanguage;
        this.isPages = isPages;
        this.elements = new Array();
        if (isPages)
            this.source = "Pages";
        else
            this.source = "Books";
        if (isLanguage)
            this.division = "Language";
        else
            this.division = "Nation";
        this.title = this.source + " by " + this.division;
        const dict = new Collections.Dictionary();
        // Get the elements.
        let title;
        let total;
        for (let book of books) {
            if (isPages)
                total = book.pages;
            else
                total = 1;
            if (isLanguage)
                title = book.originalLanguage;
            else
                title = book.nationality;
            if (dict.containsKey(title)) {
                const breakdownElement = dict.getValue(title);
                breakdownElement.total += total;
                dict.setValue(title, breakdownElement);
            }
            else {
                const breakdownElement = new BreakdownElement(title, total);
                dict.setValue(title, breakdownElement);
            }
        }
        // Get the elements sorted highest to lowest.
        const elements = dict.values();
        elements.sort((a, b) => {
            if (a.total > b.total)
                return -1;
            if (a.total < b.total)
                return 1;
            return 0;
        });
        this.elements = elements;
    }
}
exports.BreakdownTotal = BreakdownTotal;
//# sourceMappingURL=BreakdownTotal.js.map