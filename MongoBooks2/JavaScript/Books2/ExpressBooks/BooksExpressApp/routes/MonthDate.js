"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
class MonthDate {
    constructor(theDate) {
        this.year = theDate.getFullYear();
        this.month = theDate.getMonth();
    }
    toString() {
        return this.year.toString() + '/' + (1 + this.month).toString();
    }
    daysInMonth() {
        return new Date(this.year, this.month + 1, 0).getDate();
    }
    displayDate() {
        const months = ["January", "February", "March", "April", "May", "June",
            "July", "August", "September", "October", "November", "December"];
        const month = months[this.month];
        return month + " " + this.year;
    }
}
exports.MonthDate = MonthDate;
//# sourceMappingURL=MonthDate.js.map