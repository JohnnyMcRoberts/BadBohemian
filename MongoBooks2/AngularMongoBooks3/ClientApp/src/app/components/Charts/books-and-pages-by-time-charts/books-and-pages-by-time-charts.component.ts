import { Component, OnInit, AfterViewInit } from '@angular/core';
import { BooksDataService } from './../../../Services/books-data.service';

import { SeriesColors } from './../../../Models/SeriesColors';
import { YearlyTally } from './../../../Models/YearlyTally';

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

    //#region Overall Tallies Data

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

}
