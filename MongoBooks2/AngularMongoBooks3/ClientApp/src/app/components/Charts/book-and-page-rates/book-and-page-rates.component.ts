import { Component, OnInit, AfterViewInit } from '@angular/core';
import { BooksDataService } from './../../../Services/books-data.service';

import { SeriesColors } from './../../../Models/SeriesColors';
import { DeltaBooks } from './../../../Models/DeltaBooks';

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
    constructor(booksDataService: BooksDataService)
    {
      this.componentTitle = "Loading books charts from database...";
      this.booksDataService = booksDataService;
    }

    private booksDataService: BooksDataService;
    public componentTitle: string;

    public selectedItem: DeltaBooks;
    public deltaBooks: DeltaBooks[];

    //#region Component Implementation

    ngOnInit()
    {
      this.booksDataService.fetchAllDeltaBooksData().then(() =>
      {
        this.deltaBooks = new Array<DeltaBooks>();

        for (let item of this.booksDataService.deltaBooks)
        {
          var bookTally: DeltaBooks = item;
          this.deltaBooks.push(bookTally);
        }

        this.setupCharts();
      });
    }

    ngAfterViewInit()
    {
      this.setupCharts();
    }

    //#endregion

    //#region General Chart Data

    public plotlyConfig =
    {
      "displaylogo": false,
    }

    private readonly chartWidth: number = 1250;
    private readonly chartHeight: number = 650;

    public setupCharts(): void
    {
      this.setupOverallTalliesChart();
    }

    //#endregion

    //#region Overall Tallies Data

    public overallTalliesLayout: any;

    public overallTalliesData = null;

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
}
