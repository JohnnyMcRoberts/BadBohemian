"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
class MonthDate {
    constructor(theDate) {
        this.year = theDate.getFullYear();
        this.month = theDate.getMonth();
    }
}
exports.MonthDate = MonthDate;
class MonthlyBooksRead {
    constructor(theDate) {
        this.monthDate = new MonthDate(theDate);
        this.listBooksRead = new Array();
    }
}
exports.MonthlyBooksRead = MonthlyBooksRead;
//# sourceMappingURL=MonthlyBooksRead.js.map