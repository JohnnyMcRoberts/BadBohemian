"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const BookRead_1 = require("./BookRead");
const MonthDate_1 = require("./MonthDate");
const BooksTotal_1 = require("./BooksTotal");
const MonthlyBooksRead_1 = require("./MonthlyBooksRead");
const MonthlyReportAvailableYear_1 = require("./MonthlyReportAvailableYear");
const Collections = require("typescript-collections");
class MonthlyBooksReports {
    constructor(listBooksRead) {
        this.monthlyBooksRead = this.getMonthlyBooksRead(listBooksRead);
        for (let monthlyReport of this.monthlyBooksRead) {
            monthlyReport.updateTotals();
        }
        this.monthlyReportYears = this.getMonthlyReportYears();
        const totalDays = this.getDifferenceInDays(listBooksRead[0].date, listBooksRead[listBooksRead.length - 1].date);
        this.overallTotals = new BooksTotal_1.BooksTotal(totalDays, listBooksRead);
    }
    getMonthlyReportYears() {
        const monthlyReportAvailableYears = new Array();
        const dict = new Collections.Dictionary();
        for (let month of this.monthlyBooksRead) {
            if (!dict.containsKey(month.monthDate.year)) {
                dict.setValue(month.monthDate.year, month.monthDate.year);
            }
        }
        const years = dict.values().sort((a, b) => {
            if (a < b)
                return -1;
            if (a > b)
                return 1;
            return 0;
        });
        for (let year of years) {
            monthlyReportAvailableYears.push(new MonthlyReportAvailableYear_1.MonthlyReportAvailableYear(year, this.monthlyBooksRead));
        }
        return monthlyReportAvailableYears;
    }
    getMonthlyBooksRead(books) {
        const dict = new Collections.Dictionary();
        for (let book of books) {
            const month = new MonthDate_1.MonthDate(book.date);
            if (dict.containsKey(month)) {
                const monthSet = dict.getValue(month);
                monthSet.listBooksRead.push(new BookRead_1.BookRead(book));
                dict.setValue(month, monthSet);
            }
            else {
                const monthOfBooks = new MonthlyBooksRead_1.MonthlyBooksRead(book.date);
                monthOfBooks.listBooksRead.push(new BookRead_1.BookRead(book));
                dict.setValue(month, monthOfBooks);
            }
        }
        return dict.values();
    }
    getDifferenceInDays(start, end) {
        let diff = Math.abs(start.getTime() - end.getTime());
        let diffDays = Math.ceil(diff / (1000 * 3600 * 24));
        return diffDays;
    }
}
exports.MonthlyBooksReports = MonthlyBooksReports;
//# sourceMappingURL=MonthlyBooksReports.js.map