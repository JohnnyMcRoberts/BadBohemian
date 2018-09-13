"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
class AvailableReportMonth {
    constructor(name, index) {
        this.name = name;
        this.index = index;
    }
}
exports.AvailableReportMonth = AvailableReportMonth;
class MonthlyReportAvailableYear {
    constructor(year, listBooksRead) {
        this.year = year;
        const months = ["January", "February", "March", "April", "May", "June",
            "July", "August", "September", "October", "November", "December"];
        this.availableMonths = new Array();
        for (let monthIndex in listBooksRead) {
            if (listBooksRead.hasOwnProperty(monthIndex)) {
                const monthDate = listBooksRead[monthIndex].monthDate;
                if (monthDate.year === year) {
                    this.availableMonths.push(new AvailableReportMonth(months[monthDate.month], +monthIndex));
                }
            }
        }
    }
}
exports.MonthlyReportAvailableYear = MonthlyReportAvailableYear;
//# sourceMappingURL=MonthlyReportAvailableYear.js.map