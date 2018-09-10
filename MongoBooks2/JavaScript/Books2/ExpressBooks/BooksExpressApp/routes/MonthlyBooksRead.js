"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const MonthDate_1 = require("./MonthDate");
const BooksTotal_1 = require("./BooksTotal");
const BreakdownTotal_1 = require("./BreakdownTotal");
class MonthlyBooksRead {
    constructor(theDate) {
        this.monthDate = new MonthDate_1.MonthDate(theDate);
        this.listBooksRead = new Array();
    }
    updateTotals() {
        this.monthTotals = new BooksTotal_1.BooksTotal(this.monthDate.daysInMonth(), this.listBooksRead);
        this.breakdownTotals = new Array();
        this.breakdownTotals.push(new BreakdownTotal_1.BreakdownTotal(true, true, this.listBooksRead));
        this.breakdownTotals.push(new BreakdownTotal_1.BreakdownTotal(true, false, this.listBooksRead));
        this.breakdownTotals.push(new BreakdownTotal_1.BreakdownTotal(false, true, this.listBooksRead));
        this.breakdownTotals.push(new BreakdownTotal_1.BreakdownTotal(false, false, this.listBooksRead));
    }
}
exports.MonthlyBooksRead = MonthlyBooksRead;
//# sourceMappingURL=MonthlyBooksRead.js.map