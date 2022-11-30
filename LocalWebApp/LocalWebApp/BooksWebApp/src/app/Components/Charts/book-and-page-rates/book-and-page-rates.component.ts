import { Component, OnInit, AfterViewInit } from '@angular/core';
import { Validators, FormControl } from '@angular/forms';
import { ViewportRuler } from '@angular/cdk/scrolling';

import { BooksDataService } from './../../../Services/books-data.service';

import { SeriesColors } from './../../../Models/series-colors';
import { DeltaBooks } from './../../../Models/delta-books';
import { MonthlyTally } from './../../../Models/monthly-tally';
import { Book } from './../../../Models/book';
import { ChartUtilities } from './../../../Models/chart-utilities';

export class YearlyTally
{
    constructor(
        public year: number = 0,
        public dayOfYear: number = 0,
        public booksReadOnDay: number = 0,
        public pagesReadOnDay: number = 0,
        public booksReadThisYear: number = 0,
        public pagesReadThisYear: number = 0
    )
    {

    }
}

export class YearlyTallyDelta
{
    constructor(
        public date: Date = new Date(),
        public year: number = 0,
        public dayOfYear: number = 0,
        public daysSinceStart: number = 0,
        public totalPages: number = 0,
        public totalBooks: number = 0,
        public totalBookFormat: number = 0,
        public totalComicFormat: number = 0,
        public totalAudioFormat: number = 0,
        public daysInYear: number = 0,
        public yearDeltaPages: number = 0,
        public yearDeltaBooks: number = 0,
        public yearDeltaBookFormat: number = 0,
        public yearDeltaComicFormat: number = 0,
        public yearDeltaAudioFormat: number = 0,
        public yearDeltaPercentageInEnglish: number = 0,
        public yearDeltaPercentageInTranslation: number = 0,
        public yearDeltaPageRate: number = 0,
        public yearDeltaDaysPerBook: number = 0,
        public yearDeltaPagesPerBook: number = 0
    )
    { }
};

export class YearsAndMonthlyTallies
{
    public monthlyTalliesPerYear: Map<number, MonthlyTally[]> =
        new Map<number, MonthlyTally[]>();

    public years: number[] = new Array<number>();

    constructor() { }
}

export class NumericSelectionItem
{
    constructor(
        public value: string = "",
        public viewValue: string = "",
        public numericValue: number = 0
    ) { }
}

@Component({
    selector: 'app-book-and-page-rates',
    templateUrl: './book-and-page-rates.component.html',
    styleUrls: ['./book-and-page-rates.component.scss']
})
/** BookAndPageRates component*/
export class BookAndPageRatesComponent
  implements OnInit, AfterViewInit
{
    /** BookAndPageRates ctor */
    constructor(
        private viewportRuler: ViewportRuler,
        booksDataService: BooksDataService)
    {
      this.componentTitle = "Loading books charts from database...";
      this.booksDataService = booksDataService;
    }

    private booksDataService: BooksDataService;
    public componentTitle: string;

    public books: Book[] | any;

    public deltaBooks: DeltaBooks[] | any;

    public monthlyTallies: MonthlyTally[] | any;

    //#region Component Implementation

    ngOnInit()
    {
        this.booksDataService.fetchAllDeltaBooksData().then(() =>
        {
            this.deltaBooks = new Array<DeltaBooks>();

            for (let item of this.booksDataService.deltaBooks as DeltaBooks[])
            {
                const bookTally: DeltaBooks = item;
                this.deltaBooks.push(bookTally);
            }

            this.setupAllCharts();
        });

        this.booksDataService.fetchAllMonthlyTalliesData().then(() =>
        {
            this.monthlyTallies = new Array<MonthlyTally>();

            for (let item of this.booksDataService.monthlyTallies as MonthlyTally[])
            {
                const monthlyTally: MonthlyTally = item;
                this.monthlyTallies.push(monthlyTally);
            }

            this.setupAllCharts();
        });

        this.booksDataService.fetchAllBooksData().then(() =>
        {
            this.books = new Array<Book>();

            for (let item of this.booksDataService.books as Book[])
            {
                const book: Book = item;
                this.books.push(book);
            }

            this.setupAllCharts();
        });

        // subscribe to the resize event
        this.viewportRuler.change().subscribe(() => { this.setupChartSizeFromViewport(); });
    }

    ngAfterViewInit()
    {
        this.setupChartSizeFromViewport();

        this.setupAllCharts();
    }

    //#endregion

    //#region General Chart Data

    public plotlyConfig =
    {
      "displaylogo": false,
      responsive: true
    }

    public get loadingChartData(): boolean {

        return (!this.deltaBooks || !this.books || !this.monthlyTallies);
    }

    public chartWidth: number = ChartUtilities.chartWidth;
    public chartHeight: number = ChartUtilities.chartHeight;

    public viewPortWidth: number = 0;
    public viewPortHeight: number = 0;

    private setupChartSizeFromViewport(): void
    {
        const viewportSize = this.viewportRuler.getViewportSize();

        this.viewPortWidth = viewportSize.width;
        this.viewPortHeight = viewportSize.height;

        if (viewportSize.width > 1 && viewportSize.width > 1) {
            this.chartWidth = Math.floor(this.viewPortWidth * 0.95);
            this.chartHeight = Math.floor(this.viewPortHeight * 0.8);
        }
    }

    private readonly maxDaysPerYear: number = 366;
    private readonly ticksPerDay: number = 86400000;

    private readonly monthNames =
        ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];

    public setupAllCharts() : void
    {
        this.setupDeltaCharts();
        this.setupMonthlyCharts();
        this.setupYearlyCharts();
    }

    public setupDeltaCharts(): void
    {
        console.log("starting setupDeltaCharts");

        if (this.deltaBooks == null)
        {
            console.log("stop setupDeltaCharts has NOT deltaBooks");
            return;
        }

        console.log("setupDeltaCharts has deltaBooks");
        this.setupOverallTalliesChart();
        this.setupDaysPerBookChart();
        this.setupPageRateChart();
        this.setupPagesPerBookChart();
        console.log("setupDeltaCharts completing");
    }

    public setupYearlyCharts(): void
    {
        console.log("starting setupYearlyCharts");

        if (this.monthlyTallies == null)
        {
            console.log("stop setupYearlyCharts has NOT monthlyTallies");
            return;
        }

        console.log("setupYearlyCharts has deltaBooks");
        this.setupBookTalliesPerMonthByCalendarYearChart();
        this.setupPageTalliesPerMonthByCalendarYearChart();
        console.log("setupYearlyCharts completing");
    }

    public setupMonthlyCharts(): void
    {
        console.log("starting setupMonthlyCharts");

        if (this.books == null)
        {
            console.log("stop setupMonthlyCharts has NOT books");
            return;
        }
        
        console.log("setupMonthlyCharts has books");
        this.setupBookTalliesByCalendarYearChart();
        this.setupPageTalliesByCalendarYearChart();
        this.setupSelectedYearsMonthlyChart();
        console.log("setupMonthlyCharts completing");
    }

    //#endregion

    //#region Overall Tallies Data

    public overallTalliesLayout: any;

    public overallTalliesData: any = null;

    public setupOverallTalliesChartLayout(): void
    {
      this.overallTalliesLayout =
      {
        xaxis:
        {
          autorange: true,
          title: "Date"
        },
        yaxis:
        {
          autorange: true,
          title: "Books Read",
          titlefont: { color: SeriesColors.liveChartsColors[0] },
          tickfont: { color: SeriesColors.liveChartsColors[0] }
        },
        yaxis2:
        {
          autorange: true,
          title: "Pages Read",
          titlefont: { color: SeriesColors.liveChartsColors[1] },
          tickfont: { color: SeriesColors.liveChartsColors[1] },
          overlaying: 'y',
          side: 'right'
        },
        hovermode: 'closest',

        width: this.chartWidth,
        height: this.chartHeight,

        autosize: true, 
        
        showlegend: true,
        legend:
        {
          x: 0.1,
          y: 1.0
        },
        margin:
        {
          l: 55,
          r: 55,
          b: 55,
          t: 25,
          pad: 4
        },
      };
    }

    public setupOverallTalliesChart(): void
    {
        this.setupOverallTalliesChartLayout();

        var bookTotals: number[] = new Array<number>();
        var pageTotals: number[] = new Array<number>();
        var bookDates: Date[] = new Array<Date>();

        for (var i = 0; i < this.deltaBooks.length; i++)
        {
            var delta = this.deltaBooks[i];

            bookTotals.push(delta.overallTally.totalBooks);
            pageTotals.push(delta.overallTally.totalPages);
            bookDates.push(delta.date);
        }

        var bookTotalsSeries =
        {
            x: bookDates,
            y: bookTotals,
            name: 'Book Totals',
            type: 'scatter',
            mode: 'lines',
            line:
            {
                color: SeriesColors.liveChartsColors[0]
            }
        };

        var pageTotalsSeries =
        {
            x: bookDates,
            y: pageTotals,
            name: 'Page Totals',
            yaxis: 'y2',
            type: 'scatter',
            mode: 'lines',
            line:
            {
                color: SeriesColors.liveChartsColors[1]
            }
        };

        this.overallTalliesData = [bookTotalsSeries, pageTotalsSeries];
    }

    //#endregion

    //#region Days Per Book Data

    public daysPerBookLayout: any;

    public daysPerBookData: any = null;

    public setupDaysPerBookChartLayout(): void
    {
        this.daysPerBookLayout =
        {
            xaxis:
            {
                autorange: true,
                title: "Date"
            },
            yaxis:
            {
                autorange: true,
                title: "Days Per Book",
                titlefont: { color: SeriesColors.liveChartsColors[0] },
                tickfont: { color: SeriesColors.liveChartsColors[0] }
            },
            hovermode: 'closest',

            width: this.chartWidth,
            height: this.chartHeight,
            showlegend: true,
            legend:
            {
                x: 0.1,
                y: 1.0
            },
            margin:
            {
                l: 55,
                r: 55,
                b: 55,
                t: 25,
                pad: 4
            },
        };
    }

    public setupDaysPerBookChart(): void
    {
        this.setupDaysPerBookChartLayout();

        var overallDaysPerBook: number[] = new Array<number>();
        var lastTenDaysPerBook: number[] = new Array<number>();
        var bookDates: Date[] = new Array<Date>();

        for (var i = 0; i < this.deltaBooks.length; i++)
        {
            var delta = this.deltaBooks[i];

            overallDaysPerBook.push(delta.overallTally.daysPerBook);
            lastTenDaysPerBook.push(delta.lastTenTally.daysPerBook);
            bookDates.push(delta.date);
        }

        var overallDaysPerBookSeries =
        {
            x: bookDates,
            y: overallDaysPerBook,
            name: 'Overall Days per Book',
            type: 'scatter',
            mode: 'lines',
            line:
            {
                color: SeriesColors.liveChartsColors[0]
            }
        };

        var lastTenDaysPerBookSeries =
        {
            x: bookDates,
            y: lastTenDaysPerBook,
            name: 'Last 10 Days per Book',
            type: 'scatter',
            mode: 'lines',
            line:
            {
                color: SeriesColors.liveChartsColors[1]
            }
        };

        this.daysPerBookData = [overallDaysPerBookSeries, lastTenDaysPerBookSeries];
    }

    //#endregion

    //#region Page Rate Data

    public pageRateLayout: any;

    public pageRateData: any = null;

    public setupPageRateChartLayout(): void
    {
        this.pageRateLayout =
            {
                xaxis:
                {
                    autorange: true,
                    title: "Date"
                },
                yaxis:
                {
                    autorange: true,
                    title: "Page Rate",
                    titlefont: { color: SeriesColors.liveChartsColors[0] },
                    tickfont: { color: SeriesColors.liveChartsColors[0] }
                },
                hovermode: 'closest',

                width: this.chartWidth,
                height: this.chartHeight,
                showlegend: true,
                legend:
                {
                    x: 0.1,
                    y: 1.0
                },
                margin:
                {
                    l: 55,
                    r: 55,
                    b: 55,
                    t: 25,
                    pad: 4
                },
            };
    }

    public setupPageRateChart(): void
    {
        this.setupPageRateChartLayout();

        var overallPageRate: number[] = new Array<number>();
        var lastTenPageRate: number[] = new Array<number>();
        var bookDates: Date[] = new Array<Date>();

        for (var i = 0; i < this.deltaBooks.length; i++)
        {
            var delta = this.deltaBooks[i];

            overallPageRate.push(delta.overallTally.pageRate);
            lastTenPageRate.push(delta.lastTenTally.pageRate);
            bookDates.push(delta.date);
        }

        var overallPageRateSeries =
        {
            x: bookDates,
            y: overallPageRate,
            name: 'Overall Page Rate',
            type: 'scatter',
            mode: 'lines',
            line:
            {
                color: SeriesColors.liveChartsColors[0]
            }
        };

        var lastTenPageRateSeries =
        {
            x: bookDates,
            y: lastTenPageRate,
            name: 'Last 10 Page Rate',
            type: 'scatter',
            mode: 'lines',
            line:
            {
                color: SeriesColors.liveChartsColors[1]
            }
        };

        this.pageRateData = [overallPageRateSeries, lastTenPageRateSeries];
    }

    //#endregion

    //#region Pages Per Book Data

    public pagesPerBookLayout: any;

    public pagesPerBookData: any = null;

    public setupPagesPerBookChartLayout(): void
    {
        this.pagesPerBookLayout =
            {
                xaxis:
                {
                    autorange: true,
                    title: "Date"
                },
                yaxis:
                {
                    autorange: true,
                    title: "Page Rate",
                    titlefont: { color: SeriesColors.liveChartsColors[0] },
                    tickfont: { color: SeriesColors.liveChartsColors[0] }
                },
                hovermode: 'closest',

                width: this.chartWidth,
                height: this.chartHeight,
                showlegend: true,
                legend:
                {
                    x: 0.1,
                    y: 1.0
                },
                margin:
                {
                    l: 55,
                    r: 55,
                    b: 55,
                    t: 25,
                    pad: 4
                },
            };
    }

    public setupPagesPerBookChart(): void
    {
        this.setupPagesPerBookChartLayout();

        var overallPagesPerBook: number[] = new Array<number>();
        var lastTenPagesPerBook: number[] = new Array<number>();
        var bookDates: Date[] = new Array<Date>();

        for (var i = 0; i < this.deltaBooks.length; i++)
        {
            var delta = this.deltaBooks[i];

            overallPagesPerBook.push(delta.overallTally.pagesPerBook);
            lastTenPagesPerBook.push(delta.lastTenTally.pagesPerBook);
            bookDates.push(delta.date);
        }

        var overallPagesPerBookSeries =
        {
            x: bookDates,
            y: overallPagesPerBook,
            name: 'Overall Pages Per Book',
            type: 'scatter',
            mode: 'lines',
            line:
            {
                color: SeriesColors.liveChartsColors[0]
            }
        };

        var lastTenPagesPerBookSeries =
        {
            x: bookDates,
            y: lastTenPagesPerBook,
            name: 'Last 10 Pages Per Book',
            type: 'scatter',
            mode: 'lines',
            line:
            {
                color: SeriesColors.liveChartsColors[1]
            }
        };

        this.pagesPerBookData = [overallPagesPerBookSeries, lastTenPagesPerBookSeries];
    }

    //#endregion

    //#region Book Tallies Per Month By Calendar Year Data

    public bookTalliesPerMonthByCalendarYearLayout: any;

    public bookTalliesPerMonthByCalendarYearData: any = null;

    public setupBookTalliesPerMonthByCalendarYearChartLayout(): void
    {
        this.bookTalliesPerMonthByCalendarYearLayout =
            {
                hovermode: 'closest',

                width: this.chartWidth,
                height: this.chartHeight,
                showlegend: true,
                title: 'Book Tallies per Month by Calendar Year',
                xaxis:
                {
                    autorange: true,
                    title: "Month"
                },
                yaxis:
                {
                    autorange: true,
                    title: "Books Read",
                    titlefont: { color: SeriesColors.liveChartsColors[0] },
                    tickfont: { color: SeriesColors.liveChartsColors[0] }
                },
                barmode: 'group',
                bargap: 0.15,
                bargroupgap: 0.1
            };
    }

    public getYearsAndMonthlyTallies(): YearsAndMonthlyTallies
    {
        let tallies: YearsAndMonthlyTallies = new YearsAndMonthlyTallies();


        for (let i = 0; i < this.monthlyTallies.length; i++)
        {
            var monthlyTally = this.monthlyTallies[i];
            var date = new Date(monthlyTally.monthDate);
            var year: number = date.getFullYear();

            if (tallies.monthlyTalliesPerYear.has(year))
            {
                (tallies.monthlyTalliesPerYear.get(year) as MonthlyTally[]).push(monthlyTally);
            }
            else
            {
                var monthTallies: MonthlyTally[] = new Array<MonthlyTally>();
                monthTallies.push(monthlyTally);
                tallies.monthlyTalliesPerYear.set(year, monthTallies);

                tallies.years.push(year);
            }
        }

        return tallies;
    }

    public setupBookTalliesPerMonthByCalendarYearChart(): void
    {
        this.setupBookTalliesPerMonthByCalendarYearChartLayout();

        const tallies: YearsAndMonthlyTallies = this.getYearsAndMonthlyTallies();

        const monthlyTalliesPerYear: Map<number, MonthlyTally[]> = tallies.monthlyTalliesPerYear;
        const years: number[] = tallies.years;

        var yearSeries: any[] = new Array<any>();

        for (let i = 0; i < years.length; i++)
        {
            let year: number = years[i];
            let color = SeriesColors.liveChartsColors[(i % SeriesColors.liveChartsColors.length)];
            yearSeries.push(this.getBookTalliesBarSeries(year, monthlyTalliesPerYear.get(year) as MonthlyTally[], color));
        }

        this.bookTalliesPerMonthByCalendarYearData = yearSeries;
    }

    public getBookTalliesBarSeries(year: number, monthTallies: MonthlyTally[], color: string): any
    {
        const maxMonths = 12;
        var monthTotals: number[] = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];

        for (let i = 0; i < monthTallies.length; i++)
        {
            let monthTally = monthTallies[i];
            let date = new Date(monthTally.monthDate);
            let month: number = date.getMonth();
            let monthIndex = month;
            if (monthIndex < 0)
                monthIndex = 0;
            if (monthIndex >= maxMonths)
                monthIndex = maxMonths - 1;

            monthTotals[monthIndex] = monthTally.totalBooks;
        }

        var trace =
        {
            x: this.monthNames,
            y: monthTotals,
            name: year.toString(),
            marker: { color: color },
            type: 'bar'
        };

        return trace;
    }

    //#endregion

    //#region Page Tallies Per Month By Calendar Year Data

    public pageTalliesPerMonthByCalendarYearLayout: any;

    public pageTalliesPerMonthByCalendarYearData: any = null;

    public setupPageTalliesPerMonthByCalendarYearChartLayout(): void
    {
        this.pageTalliesPerMonthByCalendarYearLayout =
            {
                hovermode: 'closest',

                width: this.chartWidth,
                height: this.chartHeight,
                showlegend: true,
                title: 'Page Tallies per Month by Calendar Year',
                xaxis:
                {
                    autorange: true,
                    title: "Month"
                },
                yaxis:
                {
                    autorange: true,
                    title: "Pages Read",
                    titlefont: { color: SeriesColors.liveChartsColors[0] },
                    tickfont: { color: SeriesColors.liveChartsColors[0] }
                },
                barmode: 'group',
                bargap: 0.15,
                bargroupgap: 0.1
            };
    }

    public setupPageTalliesPerMonthByCalendarYearChart(): void
    {
        this.setupPageTalliesPerMonthByCalendarYearChartLayout();

        var monthlyTalliesPerYear: Map<number, MonthlyTally[]> =
            new Map<number, MonthlyTally[]>();
        var years: number[] = new Array<number>();

        for (let i = 0; i < this.monthlyTallies.length; i++)
        {
            var monthlyTally = this.monthlyTallies[i];
            var date = new Date(monthlyTally.monthDate);
            var year: number = date.getFullYear();

            if (monthlyTalliesPerYear.has(year))
            {
                (monthlyTalliesPerYear.get(year) as MonthlyTally[]).push(monthlyTally);
            }
            else
            {
                var monthTallies: MonthlyTally[] = new Array<MonthlyTally>();
                monthTallies.push(monthlyTally);
                monthlyTalliesPerYear.set(year, monthTallies);

                years.push(year);
            }
        }

        var yearSeries: any[] = new Array<any>();

        for (let i = 0; i < years.length; i++)
        {
            let year: number = years[i];
            let color = SeriesColors.liveChartsColors[(i % SeriesColors.liveChartsColors.length)];
            yearSeries.push(this.getPageTalliesBarSeries(year, (monthlyTalliesPerYear.get(year) as MonthlyTally[]), color));
        }

        this.pageTalliesPerMonthByCalendarYearData = yearSeries;
    }

    public getPageTalliesBarSeries(year: number, monthTallies: MonthlyTally[], color: string): any
    {
        const maxMonths = 12;
        var monthTotals: number[] = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];

        for (let i = 0; i < monthTallies.length; i++)
        {
            let monthTally = monthTallies[i];
            let date = new Date(monthTally.monthDate);
            let month: number = date.getMonth();
            let monthIndex = month;
            if (monthIndex < 0)
                monthIndex = 0;
            if (monthIndex >= maxMonths)
                monthIndex = maxMonths - 1;

            monthTotals[monthIndex] = monthTally.totalPagesRead;
        }

        var trace =
        {
            x: this.monthNames,
            y: monthTotals,
            name: year.toString(),
            marker: { color: color },
            type: 'bar'
        };

        return trace;
    }

    //#endregion

    //#region Book Tallies By Calendar Year Data

    public bookTalliesByCalendarYearLayout: any;

    public bookTalliesByCalendarYearData: any = null;

    public setupBookTalliesByCalendarYearChartLayout(): void
    {
        this.bookTalliesByCalendarYearLayout =
        {
            xaxis:
            {
                autorange: true,
                title: "Day of Year"
            },
            yaxis:
            {
                autorange: true,
                title: "Books Read"
            },
            hovermode: 'closest',

            width: this.chartWidth,
            height: this.chartHeight,
            showlegend: true,
            legend:
            {
                x: 0.1,
                y: 1.0
            },
            margin:
            {
                l: 55,
                r: 55,
                b: 55,
                t: 25,
                pad: 4
            },
        };
    }

    public setupBookTalliesByCalendarYearChart(): void
    {
        this.setupBookTalliesByCalendarYearChartLayout();

        var yearlyTallies: Map<number, YearlyTally[]> = this.getYearlyTallies();

        let years: number[] = Array.from(yearlyTallies.keys());

        var yearSeries: any[] = new Array<any>();

        for (let i = 0; i < years.length; i++)
        {
            let year: number = years[i];
            let color = SeriesColors.liveChartsColors[(i % SeriesColors.liveChartsColors.length)];
            yearSeries.push(this.getBookTalliesLineSeries(year, yearlyTallies.get(year) as YearlyTally[], color));
        }

        this.bookTalliesByCalendarYearData = yearSeries;
    }

    public getBookTalliesLineSeries(year: number, yearTallies: YearlyTally[], color: string): any
    {
        var days: number[] = new Array<number>();
        var booksRead: number[] = new Array<number>();

        for (let i = 0; i < yearTallies.length; i++)
        {
            let monthTally: YearlyTally = yearTallies[i];

            days.push(monthTally.dayOfYear);
            booksRead.push(monthTally.booksReadThisYear);
        }

        var trace =
        {
            x: days,
            y: booksRead,
            name: year.toString(),
            type: 'scatter',
            mode: 'lines',
            line:
            {
                color: color,
                shape: 'spline'
            }
        };

        return trace;
    }

    public getBooksPerYear(): Map<number, Book[]>
    {
        var booksPerYear: Map<number, Book[]> = new Map<number, Book[]>();

        for (let i = 0; i < this.books.length; i++)
        {
            let book = this.books[i];
            let date = new Date(book.date);
            let year: number = date.getFullYear();

            if (booksPerYear.has(year))
            {
                (booksPerYear.get(year) as Book[]).push(book);
            }
            else
            {
                var yearsBooks: Book[] = new Array<Book>();
                yearsBooks.push(book);
                booksPerYear.set(year, yearsBooks);
            }
        }

        return booksPerYear;
    }

    public getBlankTalliesForYear(year: number): YearlyTally[]
    {
        var yearlyTallies: YearlyTally[] = new Array<YearlyTally>();

        for (let i = 0; i < this.maxDaysPerYear; i++)
        {
            var blankTally: YearlyTally = new YearlyTally(year, i);
            yearlyTallies.push(blankTally);
        }

        return yearlyTallies;
    }

    public getDayOfYear(yearFirstDay: number, inputDate: Date): number
    {
        let date = new Date(inputDate);
        var day: number = Math.ceil((date.getTime()) / this.ticksPerDay);
        var dayOfYear: number = day - yearFirstDay;

        if (dayOfYear < 0)
            dayOfYear = 0;

        if (dayOfYear >= this.maxDaysPerYear)
            dayOfYear = this.maxDaysPerYear -1;

        return dayOfYear;
    }

    public getYearlyTallies(): Map<number, YearlyTally[]>
    {
        var yearlyTallies: Map<number, YearlyTally[]> = new Map<number, YearlyTally[]>();

        var booksPerYear: Map<number, Book[]> = this.getBooksPerYear();

        let years: number[] = Array.from(booksPerYear.keys());

        for (let i = 0; i < years.length; i++)
        {
            let year:number = years[i];
            var yearTallies = this.getBlankTalliesForYear(year);

            var booksForYear: Book[] = booksPerYear.get(year) as Book[];

            var timestamp: number = new Date().setFullYear(year, 0, 1);
            var yearFirstDay: number = Math.floor(timestamp / this.ticksPerDay);

            for (let j = 0; j < booksForYear.length; j++)
            {
                let book = booksForYear[j];
                var dayOfYear: number = this.getDayOfYear(yearFirstDay, book.date);

                yearTallies[dayOfYear].booksReadOnDay ++;
                yearTallies[dayOfYear].pagesReadOnDay += book.pages;
            }

            let booksReadThisYear: number = 0;
            let pagesReadThisYear: number = 0;

            for (let j = 0; j < yearTallies.length; j++)
            {
                booksReadThisYear += yearTallies[j].booksReadOnDay;
                pagesReadThisYear += yearTallies[j].pagesReadOnDay;

                yearTallies[j].booksReadThisYear = booksReadThisYear;
                yearTallies[j].pagesReadThisYear = pagesReadThisYear;
            }

            yearlyTallies.set(year, yearTallies);
        }


        return yearlyTallies;
    }

    //#endregion

    //#region Page Tallies By Calendar Year Data

    public pageTalliesByCalendarYearLayout: any;

    public pageTalliesByCalendarYearData: any= null;

    public setupPageTalliesByCalendarYearChartLayout(): void
    {
        this.pageTalliesByCalendarYearLayout =
            {
                xaxis:
                {
                    autorange: true,
                    title: "Day of Year"
                },
                yaxis:
                {
                    autorange: true,
                    title: "Pages Read"
                },
                hovermode: 'closest',

                width: this.chartWidth,
                height: this.chartHeight,
                showlegend: true,
                legend:
                {
                    x: 0.1,
                    y: 1.0
                },
                margin:
                {
                    l: 55,
                    r: 55,
                    b: 55,
                    t: 25,
                    pad: 4
                },
            };
    }

    public setupPageTalliesByCalendarYearChart(): void
    {
        this.setupPageTalliesByCalendarYearChartLayout();

        var yearlyTallies: Map<number, YearlyTally[]> = this.getYearlyTallies();

        let years: number[] = Array.from(yearlyTallies.keys());

        var yearSeries: any[] = new Array<any>();

        for (let i = 0; i < years.length; i++)
        {
            let year: number = years[i];
            let color = SeriesColors.liveChartsColors[(i % SeriesColors.liveChartsColors.length)];
            yearSeries.push(this.getPageTalliesLineSeries(year, yearlyTallies.get(year) as YearlyTally[], color));
        }

        this.pageTalliesByCalendarYearData = yearSeries;
    }

    public getPageTalliesLineSeries(year: number, yearTallies: YearlyTally[], color: string): any
    {
        var days: number[] = new Array<number>();
        var booksRead: number[] = new Array<number>();

        for (let i = 0; i < yearTallies.length; i++)
        {
            let monthTally: YearlyTally = yearTallies[i];

            days.push(monthTally.dayOfYear);
            booksRead.push(monthTally.pagesReadThisYear);
        }

        var trace =
        {
            x: days,
            y: booksRead,
            name: year.toString(),
            type: 'scatter',
            mode: 'lines',
            line:
            {
                color: color,
                shape: 'spline'
            }
        };

        return trace;
    }

    //#endregion

    //#region Years Monthly Reading

    public selectedYearsMonthlyLayout: any;

    public selectedYearsMonthlyData: any = null;

    public yearsMonthlyReadingYears: number[] | any;
    public yearsMonthlyReadingSelectedYear: number | any;

    public yearsMonthlyReadingOptions: NumericSelectionItem[] = new Array<NumericSelectionItem>();
    public yearsMonthlyReadingLookup: Map<string, NumericSelectionItem> | any = null;
    public selectedYear = 'format_2020';
    public selectFormControl = new FormControl('', Validators.required);
    public yearToDisplayChartFor: number = 2020;

    private setupYearsSelection(years: number[]): void
    {
        this.yearsMonthlyReadingOptions = new Array<NumericSelectionItem>();
        this.yearsMonthlyReadingLookup = new Map<string, NumericSelectionItem>();

        for (let i = 0; i < years.length; i++)
        {
            const viewValue: string = years[i].toString();
            const numericValue = years[i];
            const value = 'format_' + numericValue.toString();
            const yearOption = new NumericSelectionItem(value, viewValue, numericValue);
            this.yearsMonthlyReadingLookup.set(value, yearOption);
            this.yearsMonthlyReadingOptions.push(yearOption);
        }
    }

    public newSelectedYearsMonthlyReading(value: any)
    {
        console.log("newSelectedYearsMonthlyReading : " + value.toString());
        this.selectedYear = value;

        if (!this.yearsMonthlyReadingLookup)
        {
            return;
        }


        if (this.yearsMonthlyReadingLookup.has(this.selectedYear))
        {
            this.yearToDisplayChartFor =
                (this.yearsMonthlyReadingLookup.get(this.selectedYear) as NumericSelectionItem).numericValue;

            console.log("Going to display " + this.yearToDisplayChartFor);

            let selectedMonthlyTally: MonthlyTally[] | any = null;

            if (this.selectedYearsMonthlyChartTallies && this.selectedYearsMonthlyChartTallies.years)
            {

                console.log("Set up this.selectedYearsMonthlyChartTallies ")

                for (let i = 0; i < this.selectedYearsMonthlyChartTallies.years.length; i++)
                {
                    console.log(" checking year " + i + " aka " + this.selectedYearsMonthlyChartTallies.years[i]);

                    if (this.yearToDisplayChartFor === this.selectedYearsMonthlyChartTallies.years[i])
                    {
                        console.log(" have found data for year: " + this.yearToDisplayChartFor);

                        if (this.selectedYearsMonthlyChartTallies.monthlyTalliesPerYear.has(this.yearToDisplayChartFor)) {

                            selectedMonthlyTally =
                                this.selectedYearsMonthlyChartTallies.monthlyTalliesPerYear.get(this.yearToDisplayChartFor);
                            console.log(" selected data found");
                        }
                    }
                }
            }
            else
            {
                console.log( " not set up this.selectedYearsMonthlyChartTallies ")
            }

            const bookTotals: number[] = new Array<number>();
            const pageTotals: number[] = new Array<number>();
            const bookDates: Date[] = new Array<Date>();

            if (selectedMonthlyTally)
            {
                for (let i = 0; i < selectedMonthlyTally.length; i++)
                {
                    const month: MonthlyTally = selectedMonthlyTally[i];

                    bookTotals.push(month.totalBooks);
                    pageTotals.push(month.totalPagesRead);
                    bookDates.push(month.monthDate);
                }
            }

            var bookTotalsSeries =
            {
                x: bookDates,
                y: bookTotals,
                name: 'Monthly Book Totals',
                type: 'scatter',
                mode: 'lines',
                line:
                {
                    color: SeriesColors.liveChartsColors[0],
                    shape: 'spline',
                    smoothing: 1.3
                }
            };

            var pageTotalsSeries =
            {
                x: bookDates,
                y: pageTotals,
                name: 'Monthly Page Totals',
                yaxis: 'y2',
                type: 'scatter',
                mode: 'lines',
                line:
                {
                    color: SeriesColors.liveChartsColors[1],
                    shape: 'spline',
                    smoothing: 1.3
                }
            };

            this.selectedYearsMonthlyData = [bookTotalsSeries, pageTotalsSeries];
        }
    }

    public selectedYearsMonthlyChartTallies: YearsAndMonthlyTallies | any;

    public setupSelectedYearsMonthlyLayout(): void
    {
        this.selectedYearsMonthlyLayout =
        {
            xaxis:
            {
                autorange: true,
                title: "Date"
            },
            yaxis:
            {
                autorange: true,
                title: "Books Read",
                titlefont: { color: SeriesColors.liveChartsColors[0] },
                tickfont: { color: SeriesColors.liveChartsColors[0] }
            },
            yaxis2:
            {
                autorange: true,
                title: "Pages Read",
                titlefont: { color: SeriesColors.liveChartsColors[1] },
                tickfont: { color: SeriesColors.liveChartsColors[1] },
                overlaying: 'y',
                side: 'right'
            },
            hovermode: 'closest',

            width: this.chartWidth,
            height: this.chartHeight,

            autosize: true,

            showlegend: true,
            legend:
            {
                x: 0.1,
                y: 1.0
            },
            margin:
            {
                l: 55,
                r: 55,
                b: 55,
                t: 25,
                pad: 4
            },
        };
    }

    public setupSelectedYearsMonthlyChart(): void
    {
        this.setupSelectedYearsMonthlyLayout();

        const tallies: YearsAndMonthlyTallies = this.getYearsAndMonthlyTallies();
        this.selectedYearsMonthlyChartTallies = tallies;
        this.setupYearsSelection(tallies.years);

        this.newSelectedYearsMonthlyReading(tallies.years[0]);
    }

    //#endregion

}
