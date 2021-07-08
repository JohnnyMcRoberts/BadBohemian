import { Component, OnInit, AfterViewInit } from '@angular/core';
import { ViewportRuler } from '@angular/cdk/scrolling';
import { BooksDataService } from './../../../Services/books-data.service';

import { SeriesColors } from './../../../Models/SeriesColors';
import { ChartUtilities } from './../../../Models/ChartUtilities';

import { DeltaBooks, ICategoryTotal } from './../../../Models/DeltaBooks';
import { CountryAuthors } from './../../../Models/CountryAuthors';

@Component({
    selector: 'app-by-country-charts',
    templateUrl: './by-country-charts.component.html',
    styleUrls: ['./by-country-charts.component.scss']
})
/** ByCountryCharts component*/
export class ByCountryChartsComponent
    implements OnInit, AfterViewInit
{
    /** ByCountryCharts ctor */
    constructor(
        private viewportRuler: ViewportRuler,
        booksDataService: BooksDataService)
    {
        this.componentTitle = "Loading books charts from database...";
        this.booksDataService = booksDataService;
    }

    private booksDataService: BooksDataService;
    public componentTitle: string;
    public countryAuthors: CountryAuthors[];
    public deltaBooks: DeltaBooks[];

    public get loadingChartData(): boolean
    {
        return (!this.deltaBooks || !this.countryAuthors);
    }

    //#region Component Implementation

    ngOnInit()
    {
        this.booksDataService.fetchAllCountryAuthorsData().then(() =>
        {
            this.countryAuthors = new Array<CountryAuthors>();

            for (let item of this.booksDataService.countryAuthors)
            {
                const countryAuthor: CountryAuthors = item;
                this.countryAuthors.push(countryAuthor);
            }

            this.setupAllCharts();
        });

        this.booksDataService.fetchAllDeltaBooksData().then(() =>
        {
            this.deltaBooks = new Array<DeltaBooks>();

            for (let item of this.booksDataService.deltaBooks)
            {
                const deltaBook: DeltaBooks = item;
                this.deltaBooks.push(deltaBook);
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
    };

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

    public setupAllCharts(): void
    {
        if (this.countryAuthors != null && this.countryAuthors.length > 0)
            this.setupBooksAndPagesReadByCountryCharts();

        if (this.deltaBooks != null && this.deltaBooks.length > 0)
            this.setupAllCountryAuthorsByTallyCharts();
    }

    public setupAllCountryAuthorsByTallyCharts(): void
    {
        this.setupPercentageOfBooksReadByCountryCharts();
        this.setupTotalBooksReadByCountryCharts();
        this.setupPercentageOfPagesReadByCountryCharts();
        this.setupTotalPagesReadByCountryCharts();
    }

    //#endregion

    //#region Percentage of Books Read by Country

    public percentageOfBooksReadByCountryLayout: any;

    public percentageOfBooksReadByCountryData = null;

    public setupPercentageOfBooksReadByCountryLayout(): void
    {
        this.percentageOfBooksReadByCountryLayout =
            {
                xaxis:
                {
                    autorange: true,
                    title: "Date"
                },
                yaxis:
                {
                    autorange: true,
                    title: "% Books Read",
                    titlefont: { color: SeriesColors.liveChartsColors[0] },
                    tickfont: { color: SeriesColors.liveChartsColors[0] }
                },
                hovermode: 'closest',

                width: this.chartWidth,
                height: this.chartHeight,
                showlegend: true,
                legend:
                {
                    "orientation": "h",
                    x: 0.1,
                    y: 1
                },
                margin:
                {
                    l: 55,
                    r: 55,
                    b: 55,
                    t: 45,
                    pad: 4
                },
            };
    }

    public setupPercentageOfBooksReadByCountryCharts(): void
    {
        this.setupPercentageOfBooksReadByCountryLayout();

        // Get the list of languages for the final language tally
        let numberCountryTallies = this.deltaBooks.length;
        let finalCountryTotals = this.deltaBooks[numberCountryTallies - 1].countryTotals;

        const sortedByBooks: ICategoryTotal[] = finalCountryTotals.sort((t1, t2) =>
        {
            const ttl1 = t1.percentageBooks;
            const ttl2 = t2.percentageBooks;

            if (ttl1 > ttl2) { return -1; }
            if (ttl1 < ttl2) { return 1; }
            return 0;
        });

        const maxCountries: number = SeriesColors.liveChartsColors.length;
        const includeOtherCountry: boolean = (sortedByBooks.length > maxCountries);

        let otherLabel: string = "Other";
        var displayedCountryPercentagesByTime: Map<string, number[]> = new Map<string, number[]>();
        let displayedCountries: string[] = new Array<string>();

        for (let i = 0; i < sortedByBooks.length; i++)
        {
            let categoryTotal: ICategoryTotal = sortedByBooks[i];

            if (i < maxCountries - 1)
            {
                displayedCountryPercentagesByTime.set(categoryTotal.name, new Array<number>());
                displayedCountries.push(categoryTotal.name);
            }
            else
            {
                displayedCountryPercentagesByTime.set(otherLabel, new Array<number>());
            }
        }

        // Go through the deltas adding the percentages to the appropriate languages for the dates
        let deltaDates: Date[] = new Array<Date>();
        for (let i = 0; i < numberCountryTallies; i++)
        {
            let deltaDate = new Date(this.deltaBooks[i].date);
            deltaDates.push(deltaDate);

            let countryTotals = this.deltaBooks[i].countryTotals;

            // if there are other languages set the default value for this delta to zero
            let otherTotal: number = 0;
            let countriesAdded: string[] = new Array<string>();

            for (let j = 0; j < countryTotals.length; j++)
            {
                let countryTotal = countryTotals[j];

                if (displayedCountryPercentagesByTime.has(countryTotal.name))
                {
                    displayedCountryPercentagesByTime.get(countryTotal.name).push(countryTotal.percentageBooks);
                    countriesAdded.push(countryTotal.name);
                }
                else
                {
                    otherTotal += countryTotal.percentageBooks;
                }
            }

            for (let j = 0; j < displayedCountries.length; j++)
            {
                let displayedCountry = displayedCountries[j];
                if (countriesAdded.indexOf(displayedCountry) === -1)
                {
                    displayedCountryPercentagesByTime.get(displayedCountry).push(0);
                }
            }

            if (includeOtherCountry)
                displayedCountryPercentagesByTime.get(otherLabel).push(otherTotal);
        }

        // Create a series per language & display the series on the plot
        this.percentageOfBooksReadByCountryData =
            ChartUtilities.getLineSeriesForCategories(displayedCountryPercentagesByTime, deltaDates);
    }

    //#endregion

    //#region Total Books Read by Country

    public totalBooksReadByCountryLayout: any;

    public totalBooksReadByCountryData = null;

    public setupTotalBooksReadByCountryLayout(): void
    {
        this.totalBooksReadByCountryLayout =
            {
                xaxis:
                {
                    autorange: true,
                    title: "Date"
                },
                yaxis:
                {
                    autorange: true,
                    title: "Total Books Read",
                },
                hovermode: 'closest',


                width: this.chartWidth,
                height: this.chartHeight,
                showlegend: true,
                legend:
                {
                    "orientation": "h",
                    x: 0.1,
                    y: 1
                },
                margin:
                {
                    l: 55,
                    r: 55,
                    b: 55,
                    t: 45,
                    pad: 4
                },
            };
    }

    public setupTotalBooksReadByCountryCharts(): void
    {
        this.setupTotalBooksReadByCountryLayout();

        // Get the list of languages for the final language tally
        let numberCountryTallies = this.deltaBooks.length;
        let finalCountryTotals = this.deltaBooks[numberCountryTallies - 1].countryTotals;

        const sortedByBooks: ICategoryTotal[] = finalCountryTotals.sort((t1, t2) =>
        {
            const ttl1 = t1.totalBooks;
            const ttl2 = t2.totalBooks;

            if (ttl1 > ttl2) { return -1; }
            if (ttl1 < ttl2) { return 1; }
            return 0;
        });

        const maxCountries: number = SeriesColors.liveChartsColors.length;
        const includeOtherCountry: boolean = (sortedByBooks.length > maxCountries);

        let otherLabel: string = "Other";
        var displayedCountryTotalsByTime: Map<string, number[]> = new Map<string, number[]>();
        let displayedCountries: string[] = new Array<string>();

        for (let i = 0; i < sortedByBooks.length; i++)
        {
            let categoryTotal: ICategoryTotal = sortedByBooks[i];

            if (i < maxCountries - 1)
            {
                displayedCountryTotalsByTime.set(categoryTotal.name, new Array<number>());
                displayedCountries.push(categoryTotal.name);
            }
            else
            {
                displayedCountryTotalsByTime.set(otherLabel, new Array<number>());
            }
        }

        // Go through the deltas adding the percentages to the appropriate languages for the dates
        let deltaDates: Date[] = new Array<Date>();
        for (let i = 0; i < numberCountryTallies; i++)
        {
            let deltaDate = new Date(this.deltaBooks[i].date);
            deltaDates.push(deltaDate);

            let countryTotals = this.deltaBooks[i].countryTotals;

            // if there are other languages set the default value for this delta to zero
            let otherTotal: number = 0;
            let languagesAdded: string[] = new Array<string>();

            for (let j = 0; j < countryTotals.length; j++)
            {
                let countryTotal = countryTotals[j];

                if (displayedCountryTotalsByTime.has(countryTotal.name))
                {
                    displayedCountryTotalsByTime.get(countryTotal.name).push(countryTotal.totalBooks);
                    languagesAdded.push(countryTotal.name);
                }
                else
                {
                    otherTotal += countryTotal.totalBooks;
                }
            }

            for (let j = 0; j < displayedCountries.length; j++)
            {
                let displayedCountry = displayedCountries[j];
                if (languagesAdded.indexOf(displayedCountry) === -1)
                {
                    displayedCountryTotalsByTime.get(displayedCountry).push(0);
                }
            }

            if (includeOtherCountry)
                displayedCountryTotalsByTime.get(otherLabel).push(otherTotal);
        }

        // Create a series per language & display the series on the plot
        this.totalBooksReadByCountryData =
            ChartUtilities.getStackedAreaSeriesForCategories(displayedCountryTotalsByTime, deltaDates);
    }

    //#endregion

    //#region Percentage of Books Read by Country

    public percentageOfPagesReadByCountryLayout: any;

    public percentageOfPagesReadByCountryData = null;

    public setupPercentageOfPagesReadByCountryLayout(): void
    {
        this.percentageOfPagesReadByCountryLayout =
            {
                xaxis:
                {
                    autorange: true,
                    title: "Date"
                },
                yaxis:
                {
                    autorange: true,
                    title: "% Pages Read",
                    titlefont: { color: SeriesColors.liveChartsColors[0] },
                    tickfont: { color: SeriesColors.liveChartsColors[0] }
                },
                hovermode: 'closest',


                width: this.chartWidth,
                height: this.chartHeight,
                showlegend: true,
                legend:
                {
                    "orientation": "h",
                    x: 0.1,
                    y: 1
                },
                margin:
                {
                    l: 55,
                    r: 55,
                    b: 55,
                    t: 45,
                    pad: 4
                },
            };
    }

    public setupPercentageOfPagesReadByCountryCharts(): void
    {
        this.setupPercentageOfPagesReadByCountryLayout();

        // Get the list of languages for the final language tally
        let numberCountryTallies = this.deltaBooks.length;
        let finalCountryTotals = this.deltaBooks[numberCountryTallies - 1].countryTotals;

        const sortedByPages: ICategoryTotal[] = finalCountryTotals.sort((t1, t2) =>
        {
            const ttl1 = t1.percentagePages;
            const ttl2 = t2.percentagePages;

            if (ttl1 > ttl2) { return -1; }
            if (ttl1 < ttl2) { return 1; }
            return 0;
        });

        const maxCountries: number = SeriesColors.liveChartsColors.length;
        const includeOtherCountry: boolean = (sortedByPages.length > maxCountries);

        let otherLabel: string = "Other";
        var displayedCountryPercentagesByTime: Map<string, number[]> = new Map<string, number[]>();

        for (let i = 0; i < sortedByPages.length; i++)
        {
            let categoryTotal: ICategoryTotal = sortedByPages[i];

            if (i < maxCountries - 1) {
                displayedCountryPercentagesByTime.set(categoryTotal.name, new Array<number>());
            }
            else
            {
                displayedCountryPercentagesByTime.set(otherLabel, new Array<number>());
            }
        }

        // Go through the deltas adding the percentages to the appropriate languages for the dates
        let deltaDates: Date[] = new Array<Date>();
        for (let i = 0; i < numberCountryTallies; i++)
        {
            let deltaDate = new Date(this.deltaBooks[i].date);
            deltaDates.push(deltaDate);

            let countryTotals = this.deltaBooks[i].countryTotals;

            // if there are other languages set the default value for this delta to zero
            let otherTotal: number = 0;

            for (let j = 0; j < countryTotals.length; j++)
            {
                let countryTotal = countryTotals[j];

                if (displayedCountryPercentagesByTime.has(countryTotal.name))
                {
                    displayedCountryPercentagesByTime.get(countryTotal.name).push(countryTotal.percentagePages);
                }
                else
                {
                    otherTotal += countryTotal.percentagePages;
                }
            }

            if (includeOtherCountry)
                displayedCountryPercentagesByTime.get(otherLabel).push(otherTotal);
        }

        // Create a series per language & display the series on the plot
        this.percentageOfPagesReadByCountryData =
            ChartUtilities.getLineSeriesForCategories(displayedCountryPercentagesByTime, deltaDates);
    }

    //#endregion

    //#region Total Pages Read by Country

    public totalPagesReadByCountryLayout: any;

    public totalPagesReadByCountryData = null;

    public setupTotalPagesReadByCountryLayout(): void
    {
        this.totalPagesReadByCountryLayout =
            {
                xaxis:
                {
                    autorange: true,
                    title: "Date"
                },
                yaxis:
                {
                    autorange: true,
                    title: "Total Pages Read",
                },
                hovermode: 'closest',


                width: this.chartWidth,
                height: this.chartHeight,
                showlegend: true,
                legend:
                {
                    "orientation": "h",
                    x: 0.1,
                    y: 1
                },
                margin:
                {
                    l: 55,
                    r: 55,
                    b: 55,
                    t: 45,
                    pad: 4
                },
            };
    }

    public setupTotalPagesReadByCountryCharts(): void
    {
        this.setupTotalPagesReadByCountryLayout();

        // Get the list of languages for the final language tally
        let numberCountryTallies = this.deltaBooks.length;
        let finalCountryTotals = this.deltaBooks[numberCountryTallies - 1].countryTotals;

        const sortedByPages: ICategoryTotal[] = finalCountryTotals.sort((t1, t2) =>
        {
            const ttl1 = t1.totalPages;
            const ttl2 = t2.totalPages;

            if (ttl1 > ttl2) { return -1; }
            if (ttl1 < ttl2) { return 1; }
            return 0;
        });

        const maxCountries: number = SeriesColors.liveChartsColors.length;
        const includeOtherCountry: boolean = (sortedByPages.length > maxCountries);

        let otherLabel: string = "Other";
        var displayedCountryTotalsByTime: Map<string, number[]> = new Map<string, number[]>();
        let displayedCountries: string[] = new Array<string>();

        for (let i = 0; i < sortedByPages.length; i++)
        {
            let categoryTotal: ICategoryTotal = sortedByPages[i];

            if (i < maxCountries - 1)
            {
                displayedCountryTotalsByTime.set(categoryTotal.name, new Array<number>());
                displayedCountries.push(categoryTotal.name);
            }
            else
            {
                displayedCountryTotalsByTime.set(otherLabel, new Array<number>());
            }
        }

        // Go through the deltas adding the percentages to the appropriate languages for the dates
        let deltaDates: Date[] = new Array<Date>();
        for (let i = 0; i < numberCountryTallies; i++)
        {
            let deltaDate = new Date(this.deltaBooks[i].date);
            deltaDates.push(deltaDate);

            let countryTotals = this.deltaBooks[i].countryTotals;

            // if there are other languages set the default value for this delta to zero
            let otherTotal: number = 0;
            let languagesAdded: string[] = new Array<string>();

            for (let j = 0; j < countryTotals.length; j++)
            {
                let countryTotal = countryTotals[j];

                if (displayedCountryTotalsByTime.has(countryTotal.name))
                {
                    displayedCountryTotalsByTime.get(countryTotal.name).push(countryTotal.totalPages);
                    languagesAdded.push(countryTotal.name);
                }
                else
                {
                    otherTotal += countryTotal.totalPages;
                }
            }

            for (let j = 0; j < displayedCountries.length; j++)
            {
                let displayedCountry = displayedCountries[j];
                if (languagesAdded.indexOf(displayedCountry) === -1)
                {
                    displayedCountryTotalsByTime.get(displayedCountry).push(0);
                }
            }

            if (includeOtherCountry)
                displayedCountryTotalsByTime.get(otherLabel).push(otherTotal);
        }

        // Create a series per language & display the series on the plot
        this.totalPagesReadByCountryData =
            ChartUtilities.getStackedAreaSeriesForCategories(displayedCountryTotalsByTime, deltaDates);
    }

    //#endregion

    //#region Books and Pages Read by Country

    public currentPieChartByCountryLayout: any;

    public booksReadByCountryData = null;
    public pagesReadByCountryData = null;

    public setupBooksAndPagesReadByCountryLayout(): void
    {
        this.currentPieChartByCountryLayout =
            {
                width: this.chartWidth / 2,
                height: this.chartHeight,
                showlegend: true,
                legend: { "orientation": "h" },
                margin:
                {
                    l: 25,
                    r: 25,
                    b: 25,
                    t: 25,
                    pad: 4
                },
            };
    }

    public setupBooksAndPagesReadByCountryCharts(): void
    {
        this.setupBooksAndPagesReadByCountryLayout();

        const sortedByBooks: CountryAuthors[] = this.countryAuthors.sort((t1, t2) =>
        {
            const ttl1 = t1.totalBooksReadFromCountry;
            const ttl2 = t2.totalBooksReadFromCountry;

            if (ttl1 > ttl2) { return -1; }
            if (ttl1 < ttl2) { return 1; }
            return 0;
        });

        const sortedByPages: CountryAuthors[] = this.countryAuthors.sort((t1, t2) =>
        {
            const ttl1 = t1.totalPagesReadFromCountry;
            const ttl2 = t2.totalPagesReadFromCountry;

            if (ttl1 > ttl2) { return -1; }
            if (ttl1 < ttl2) { return 1; }
            return 0;
        });

        const maxCategories: number = SeriesColors.liveChartsColors.length;
        const includeOtherCategory: boolean = (sortedByBooks.length > maxCategories);

        let otherLabel: string = "Other";
        let otherTotal: number = 0;
        let otherColor: string = SeriesColors.liveChartsColors[maxCategories - 1];

        let ttlLabels: string[] = new Array<string>();
        let ttlValues: number[] = new Array<number>();
        let colors: string[] = new Array<string>();

        for (let i = 0; i < sortedByBooks.length; i++)
        {
            let sortedItem = sortedByBooks[i];
            if (i >= (maxCategories - 1))
            {
                otherTotal += sortedItem.totalBooksReadFromCountry;
            }
            else
            {
                ttlLabels.push(sortedItem.name);
                ttlValues.push(sortedItem.totalBooksReadFromCountry);
                colors.push(SeriesColors.liveChartsColors[i]);
            }
        }

        if (includeOtherCategory)
        {
            ttlLabels.push(otherLabel);
            ttlValues.push(otherTotal);
            colors.push(otherColor);
        }

        this.booksReadByCountryData =
            ChartUtilities.getPiePlotData(ttlLabels, ttlValues, colors);

        otherTotal = 0;
        ttlLabels = new Array<string>();
        ttlValues = new Array<number>();
        colors = new Array<string>();

        for (let i = 0; i < sortedByPages.length; i++)
        {
            let sortedItem = sortedByPages[i];
            if (i >= (maxCategories - 1))
            {
                otherTotal += sortedItem.totalPagesReadFromCountry;
            }
            else
            {
                ttlLabels.push(sortedItem.name);
                ttlValues.push(sortedItem.totalPagesReadFromCountry);
                colors.push(SeriesColors.liveChartsColors[i]);
            }
        }

        if (includeOtherCategory)
        {
            ttlLabels.push(otherLabel);
            ttlValues.push(otherTotal);
            colors.push(otherColor);
        }

        this.pagesReadByCountryData =
            ChartUtilities.getPiePlotData(ttlLabels, ttlValues, colors);
    }

    //#endregion

}
