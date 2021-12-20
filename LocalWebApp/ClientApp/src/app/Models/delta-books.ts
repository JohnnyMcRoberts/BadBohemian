import { TallyDelta } from './tally-delta'

export interface ICategoryTotal {
    name: string;
    totalPages: number;
    totalBooks: number;
    percentagePages: number;
    percentageBooks: number;
};

export class CategoryTotal implements ICategoryTotal {
    constructor(
        public name: string = '',
        public totalPages: number = 0,
        public totalBooks: number = 0,
        public percentagePages: number = 0,
        public percentageBooks: number = 0) {
    }
}

export interface IDeltaBooks {
    date: Date;
    startDate: Date;
    daysSinceStart: number;
    overallTally: TallyDelta;
    lastTenTally: TallyDelta;
    languageTotals: ICategoryTotal[];
    countryTotals: ICategoryTotal[];
};

export class DeltaBooks implements IDeltaBooks {
    constructor(
        public date: Date = new Date(),
        public startDate: Date = new Date(),
        public daysSinceStart: number = 1,
        public overallTally: TallyDelta = new TallyDelta(),
        public lastTenTally: TallyDelta = new TallyDelta(),
        public languageTotals: ICategoryTotal[] = new Array<CategoryTotal>(),
        public countryTotals: ICategoryTotal[] = new Array<CategoryTotal>()) {
    }
}
