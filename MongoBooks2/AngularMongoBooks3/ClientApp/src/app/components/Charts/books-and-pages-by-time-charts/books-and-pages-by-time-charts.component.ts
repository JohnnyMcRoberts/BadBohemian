import { Component, OnInit, AfterViewInit } from '@angular/core';
import { BooksDataService } from './../../../Services/books-data.service';

import { SeriesColors } from './../../../Models/SeriesColors';
import { YearlyTally } from './../../../Models/YearlyTally';
import { IDeltaBooks, DeltaBooks} from './../../../Models/DeltaBooks';
import { ICurveFitter, LinearCurveFitter, QuadraticCurveFitter } from './../../../Models/CurveFitter';

@Component({
    selector: 'app-books-and-pages-by-time-charts',
    templateUrl: './books-and-pages-by-time-charts.component.html',
    styleUrls: ['./books-and-pages-by-time-charts.component.scss']
})
/** BooksAndPagesByTimeCharts component*/
export class BooksAndPagesByTimeChartsComponent
    implements OnInit, AfterViewInit
{
  /** BooksAndPagesByTimeCharts ctor */
    constructor(booksDataService: BooksDataService)
    {
      this.componentTitle = "Loading books charts from database...";
      this.booksDataService = booksDataService;
    }

    private booksDataService: BooksDataService;
    public componentTitle: string;

    public yearlyTallies: YearlyTally[];
    public deltaBooks: DeltaBooks[];

    //#region Component Implementation

    ngOnInit()
    {
        this.booksDataService.fetchAllYearlyTalliesData().then(() =>
        {
            this.yearlyTallies = new Array<YearlyTally>();

            for (let item of this.booksDataService.yearlyTallies)
            {
                var yearlyTally: YearlyTally = item;
                this.yearlyTallies.push(yearlyTally);
            }

            this.setupAllCharts();
        });

        this.booksDataService.fetchAllDeltaBooksData().then(() =>
        {
            this.deltaBooks = new Array<DeltaBooks>();

            for (let item of this.booksDataService.deltaBooks)
            {
                let deltaBook: DeltaBooks = item;
                this.deltaBooks.push(deltaBook);
            }

            this.setupAllCharts();
        });
    }

    ngAfterViewInit()
    {
        this.setupAllCharts();
    }

    //#endregion

    //#region General Chart Data

    public plotlyConfig =
    {
      "displaylogo": false,
    }

    private readonly chartWidth: number = 1250;
    private readonly chartHeight: number = 650;
    private readonly maxDaysPerYear: number = 366;
    private readonly ticksPerDay: number = 86400000;

    private readonly monthNames =
      ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];

    public setupAllCharts(): void
    {
        this.setupYearlyTallyCharts();
        this.setupAllDailyCharts();
    }

    public setupAllDailyCharts(): void
    {
        console.log("starting setupAllDailyCharts");

        if (this.deltaBooks == null)
        {
            console.log("stop setupAllDailyCharts has NOT deltaBooks");
            return;
        }

        console.log("setupAllDailyCharts has deltaBooks");
        this.setupPagesPerDayChart();
        this.setupAverageDaysPerBookChart();
        console.log("setupAllDailyCharts completing");
    }

    public setupYearlyTallyCharts(): void
    {
        console.log("starting setupYearlyTallyCharts");

        if (this.yearlyTallies == null)
        {
            console.log("stop setupYearlyTallyCharts has NOT yearlyTallies");
            return;
        }

        console.log("setupYearlyTallyCharts has yearlyTallies");
        this.setupBooksAndPagesPerYearChart();
        console.log("setupYearlyTallyCharts completing");
    }

    //#endregion

    //#region Books and Pages per Year Data

    public booksAndPagesPerYearLayout: any;

    public booksAndPagesPerYearData = null;

    public setupBooksAndPagesPerYearChartLayout(): void
    {
        this.booksAndPagesPerYearLayout =
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

    public setupBooksAndPagesPerYearChart(): void
    {
        this.setupBooksAndPagesPerYearChartLayout();

        var bookTotals: number[] = new Array<number>();
        var pageTotals: number[] = new Array<number>();
        var bookDates: Date[] = new Array<Date>();

        for (var i = 365; i < this.yearlyTallies.length; i++)
        {
            var yearlyTally = this.yearlyTallies[i];

            if (yearlyTally.yearDeltaBooks >= 0)
            {
                bookTotals.push(yearlyTally.yearDeltaBooks);
                pageTotals.push(yearlyTally.yearDeltaPages);
                bookDates.push(yearlyTally.date);
            }
        }

        var bookTotalsSeries =
        {
            x: bookDates,
            y: bookTotals,
            name: 'Yearly Book Total',
            type: 'scatter',
            mode: 'lines',
            line:
            {
                color: SeriesColors.liveChartsColors[0],
                shape: 'spline'
            }
        };

        var pageTotalsSeries =
        {
            x: bookDates,
            y: pageTotals,
            name: 'Yearly Page Total',
            yaxis: 'y2',
            type: 'scatter',
            mode: 'lines',
            line:
            {
                color: SeriesColors.liveChartsColors[1],
                shape: 'spline'
            }
        };

        this.booksAndPagesPerYearData = [bookTotalsSeries, pageTotalsSeries];
    }

    //#endregion

    //#region Pages per Day Data

    public pagesPerDayLayout: any;

    public pagesPerDayData = null;

    public setupPagesPerDayLayout(): void
    {
        this.pagesPerDayLayout =
        {
            xaxis:
            {
                autorange: true,
                title: "Date"
            },
            yaxis:
            {
                autorange: true,
                title: "Pages per Day",
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

    public setupPagesPerDayChart(): void
    {
        this.setupPagesPerDayLayout();

        var daysSinceStart: number[] = new Array<number>();
        var pageRates: number[] = new Array<number>();
        var deltaDates: Date[] = new Array<Date>();

        var curveFitValues: number[] = new Array<number>();

        for (let i = 0; i < this.deltaBooks.length; i++)
        {
            let deltaBook: IDeltaBooks = this.deltaBooks[i];

            daysSinceStart.push(deltaBook.daysSinceStart);
            deltaDates.push(new Date(deltaBook.date));
            pageRates.push(deltaBook.overallTally.pageRate);
        }

        var curveFitter: ICurveFitter =
            new QuadraticCurveFitter(daysSinceStart, pageRates);

        for (let i = 0; i < daysSinceStart.length; i++)
        {
            let curveFitValue: number =
                curveFitter.evaluateYValueAtPoint(daysSinceStart[i]);

            curveFitValues.push(curveFitValue);
        }

        var pageRateSeries =
        {
            x: deltaDates,
            y: pageRates,
          name: 'Page Rate',
          type: 'scatter',
          mode: 'lines',
          line:
          {
            color: SeriesColors.liveChartsColors[0],
            shape: 'spline'
          }
        };

        var pageRateTrendSeries =
        {
            x: deltaDates,
            y: curveFitValues,
            name: 'Page Rate Trend',
            type: 'scatter',
            mode: 'lines',
            line:
            {
              color: SeriesColors.liveChartsColors[1],
              shape: 'spline'
            }
        };

        this.pagesPerDayData = [pageRateSeries, pageRateTrendSeries];
    }

    //#endregion

    //#region Average Days per Book Data

    public averageDaysPerBookLayout: any;

    public averageDaysPerBookData = null;

    public setupAverageDaysPerBookLayout(): void
    {
        this.averageDaysPerBookLayout =
        {
          xaxis:
          {
              autorange: true,
              title: "Date"
          },
          yaxis:
          {
              autorange: true,
              title: "Days per Book",
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

    public setupAverageDaysPerBookChart(): void
    {
        this.setupAverageDaysPerBookLayout();

        var daysSinceStart: number[] = new Array<number>();
        var daysPerBook: number[] = new Array<number>();
        var daysPerBookLastTen: number[] = new Array<number>();
        var deltaDates: Date[] = new Array<Date>();

        var curveFitValues: number[] = new Array<number>();

        for (let i = 0; i < this.deltaBooks.length; i++)
        {
            let deltaBook: IDeltaBooks = this.deltaBooks[i];

            daysSinceStart.push(deltaBook.daysSinceStart);
            deltaDates.push(new Date(deltaBook.date));
            daysPerBook.push(deltaBook.overallTally.daysPerBook);
            daysPerBookLastTen.push(deltaBook.lastTenTally.daysPerBook);
        }

        var curveFitter: ICurveFitter =
            new QuadraticCurveFitter(daysSinceStart, daysPerBook);

        for (let i = 0; i < daysSinceStart.length; i++)
        {
            let curveFitValue: number =
                curveFitter.evaluateYValueAtPoint(daysSinceStart[i]);

            curveFitValues.push(curveFitValue);
        }

        var daysPerBookSeries =
        {
            x: deltaDates,
            y: daysPerBook,
            name: 'Days Per Book',
            type: 'scatter',
            mode: 'lines',
            line:
            {
                color: SeriesColors.liveChartsColors[0],
                shape: 'spline'
            }
        };

        var daysPerBookTrendSeries =
        {
            x: deltaDates,
            y: curveFitValues,
            name: 'Days Per Book Trend',
            type: 'scatter',
            mode: 'lines',
            line:
            {
                color: SeriesColors.liveChartsColors[4],
                shape: 'spline'
            }
        };

        var daysPerBookLastTenSeries =
        {
            x: deltaDates,
            y: daysPerBookLastTen,
            name: 'Last 10 Days Per Book',
            type: 'scatter',
            mode: 'lines',
            line:
            {
                color: SeriesColors.liveChartsColors[1],
                shape: 'spline'
            }
        };

        this.averageDaysPerBookData = [daysPerBookSeries, daysPerBookTrendSeries, daysPerBookLastTenSeries];
    }

    //#endregion

}
