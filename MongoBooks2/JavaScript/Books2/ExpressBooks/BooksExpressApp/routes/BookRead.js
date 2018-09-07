"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
class BookRead {
    constructor(theBook) {
        this._id = theBook._id;
        this.dateString = theBook.dateString;
        this.date = theBook.date;
        this.title = theBook.title;
        this.author = theBook.author;
        this.pages = theBook.pages;
        this.note = theBook.note;
        this.nationality = theBook.nationality;
        this.originalLanguage = theBook.originalLanguage;
        this.image_url = theBook.image_url;
        this.tags = theBook.tags;
        this.format = theBook.format;
    }
    displayFormat() {
        const formats = ["undefined", "Book", "Comic", "Audio"];
        return formats[this.format % formats.length];
    }
    displayDate() {
        const months = ["January", "February", "March", "April", "May", "June",
            "July", "August", "September", "October", "November", "December"];
        const days = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];
        const weekday = days[this.date.getDay()];
        const displayDay = this.nthDay(this.date.getDate());
        const month = months[this.date.getMonth()];
        return weekday + " " + displayDay + " " + month + " " + this.date.getFullYear();
    }
    nthDay(day) {
        switch (day % 10) {
            case 1:
                return day + "st";
            case 2:
                return day + "nd";
            case 3:
                return day + "rd";
            default:
                return day + "th";
        }
    }
}
exports.BookRead = BookRead;
//# sourceMappingURL=BookRead.js.map