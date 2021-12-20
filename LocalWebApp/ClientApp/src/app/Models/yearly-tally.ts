export interface IYearlyTally {
    date: Date;
    year: number;
    dayOfYear: number;
    daysSinceStart: number;

    totalPages: number;
    totalBooks: number;
    totaBooksInEnglish: number;
    totalBookFormat: number;
    totalComicFormat: number;
    totalAudioFormat: number;

    totalPageRate: number;
    totalDaysPerBook: number;
    totalPagesPerBook: number;
    totalBooksPercentageInEnglish: number;

    daysInYearDelta: number;

    yearDeltaPages: number;
    yearDeltaBooks: number;
    yearDeltaBooksInEnglish: number;
    yearDeltaBookFormat: number;
    yearDeltaComicFormat: number;
    yearDeltaAudioFormat: number;

    yearDeltaPageRate: number;
    yearDeltaDaysPerBook: number;
    yearDeltaPagesPerBook: number;
    yearDeltaBooksPercentageInEnglish: number;
};

export class YearlyTally implements IYearlyTally {
    constructor(
        public date: Date = new Date(),
        public year: number = 0,
        public dayOfYear: number = 0,
        public daysSinceStart: number = 0,

        public totalPages: number = 0,
        public totalBooks: number = 0,
        public totaBooksInEnglish: number = 0,
        public totalBookFormat: number = 0,
        public totalComicFormat: number = 0,
        public totalAudioFormat: number = 0,

        public totalPageRate: number = 0,
        public totalDaysPerBook: number = 0,
        public totalPagesPerBook: number = 0,
        public totalBooksPercentageInEnglish: number = 0,

        public daysInYearDelta: number = 0,

        public yearDeltaPages: number = 0,
        public yearDeltaBooks: number = 0,
        public yearDeltaBooksInEnglish: number = 0,
        public yearDeltaBookFormat: number = 0,
        public yearDeltaComicFormat: number = 0,
        public yearDeltaAudioFormat: number = 0,

        public yearDeltaPageRate: number = 0,
        public yearDeltaDaysPerBook: number = 0,
        public yearDeltaPagesPerBook: number = 0,
        public yearDeltaBooksPercentageInEnglish: number = 0) {
    }
}
