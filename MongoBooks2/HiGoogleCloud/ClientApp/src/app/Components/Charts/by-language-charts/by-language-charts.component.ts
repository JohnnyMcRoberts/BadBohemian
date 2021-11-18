import { Component, OnInit, AfterViewInit } from '@angular/core';
import { ViewportRuler } from '@angular/cdk/scrolling';
import { BooksDataService } from './../../../Services/books-data.service';

import { SeriesColors } from './../../../Models/SeriesColors';
import { ChartUtilities } from './../../../Models/ChartUtilities';

import { DeltaBooks, ICategoryTotal } from './../../../Models/DeltaBooks';
import { LanguageAuthors } from './../../../Models/LanguageAuthors';
import { Book } from './../../../Models/Book';

export class NameCountPair
{
    constructor(
        public name: string = "",
        public count: number = 0)
    {}
}

@Component({
    selector: 'app-by-language-charts',
    templateUrl: './by-language-charts.component.html',
    styleUrls: ['./by-language-charts.component.scss']
})
/** ByLanguageCharts component*/
export class ByLanguageChartsComponent
    implements OnInit, AfterViewInit
{
    /** ByLanguageCharts ctor */
    constructor(
        private viewportRuler: ViewportRuler,
        booksDataService: BooksDataService)
    {
      this.componentTitle = "Loading books charts from database...";
      this.booksDataService = booksDataService;
    }

    private booksDataService: BooksDataService;
    public componentTitle: string;
    public languageAuthors: LanguageAuthors[];
    public deltaBooks: DeltaBooks[];
    public books: Book[];

    public get loadingChartData(): boolean
    {
        return (!this.deltaBooks || !this.books || !this.languageAuthors);
    }

    //#region Component Implementation

    ngOnInit()
    {
        this.booksDataService.fetchAllLanguageAuthorsData().then(() =>
        {
            this.languageAuthors = new Array<LanguageAuthors>();

            for (let item of this.booksDataService.languageAuthors)
            {
                const languageAuthor: LanguageAuthors = item;
                this.languageAuthors.push(languageAuthor);
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

        this.booksDataService.fetchAllBooksData().then(() =>
        {
            this.books = new Array<Book>();

            for (let item of this.booksDataService.books) {
                let book: Book = item;
                this.books.push(book);
            }

            this.setupBooksAndPagesReadByLanguageAndCountryCharts();
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
        if (this.languageAuthors != null && this.languageAuthors.length > 0)
            this.setupBooksAndPagesReadByLanguageCharts();

        if (this.deltaBooks != null && this.deltaBooks.length > 0)
            this.setupAllLanguageByTallyCharts();
    }

    public setupAllLanguageByTallyCharts(): void
    {
        this.setupPercentageOfBooksReadByLanguageCharts();
        this.setupTotalBooksReadByLanguageCharts();
        this.setupPercentageOfPagesReadByLanguageCharts();
        this.setupTotalPagesReadByLanguageCharts();
    }

    //#endregion

    //#region Percentage of Books Read by Language

    public percentageOfBooksReadByLanguageLayout: any;

    public percentageOfBooksReadByLanguageData = null;

    public setupPercentageOfBooksReadByLanguageLayout(): void
    {
        this.percentageOfBooksReadByLanguageLayout =
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

    public setupPercentageOfBooksReadByLanguageCharts(): void
    {
        this.setupPercentageOfBooksReadByLanguageLayout();

        // Get the list of languages for the final language tally
        let numberLanguageTallies = this.deltaBooks.length;
        let finalLanguageTotals = this.deltaBooks[numberLanguageTallies - 1].languageTotals;

        const sortedByBooks: ICategoryTotal[] = finalLanguageTotals.sort((t1, t2) =>
        {
            const ttl1 = t1.percentageBooks;
            const ttl2 = t2.percentageBooks;

            if (ttl1 > ttl2) { return -1; }
            if (ttl1 < ttl2) { return 1; }
            return 0;
        });

        const maxLanguages: number = SeriesColors.liveChartsColors.length;
        const includeOtherLanguage: boolean = (sortedByBooks.length > maxLanguages);

        let otherLabel: string = "Other";
        var displayedLanguagePercentagesByTime: Map<string, number[]> = new Map<string, number[]>();
        let displayedLanguages: string[] = new Array<string>();

        for (let i = 0; i < sortedByBooks.length; i++)
        {
            let categoryTotal: ICategoryTotal = sortedByBooks[i];

            if (i < maxLanguages - 1)
            {
                displayedLanguagePercentagesByTime.set(categoryTotal.name, new Array<number>());
                displayedLanguages.push(categoryTotal.name);
            }
            else
            {
                displayedLanguagePercentagesByTime.set(otherLabel, new Array<number>());
            }
        }

        // Go through the deltas adding the percentages to the appropriate languages for the dates
        let deltaDates: Date[] = new Array<Date>();
        for (let i = 0; i < numberLanguageTallies; i++)
        {
            let deltaDate = new Date(this.deltaBooks[i].date);
            deltaDates.push(deltaDate);

            let languageTotals = this.deltaBooks[i].languageTotals;

            // if there are other languages set the default value for this delta to zero
            let otherTotal: number = 0;
            let languagesAdded: string[] = new Array<string>();

            for (let j = 0; j < languageTotals.length; j++)
            {
                let languageTotal = languageTotals[j];

                if (displayedLanguagePercentagesByTime.has(languageTotal.name))
                {
                    displayedLanguagePercentagesByTime.get(languageTotal.name).push(languageTotal.percentageBooks);
                    languagesAdded.push(languageTotal.name);
                }
                else
                {
                   otherTotal += languageTotal.percentageBooks;
                }
            }

            for (let j = 0; j < displayedLanguages.length; j++)
            {
                let displayedLanguage = displayedLanguages[j];
                if (languagesAdded.indexOf(displayedLanguage) === -1)
                {
                    displayedLanguagePercentagesByTime.get(displayedLanguage).push(0);
                }
            }

            if (includeOtherLanguage)
                displayedLanguagePercentagesByTime.get(otherLabel).push(otherTotal);
        }

        // Create a series per language & display the series on the plot
        this.percentageOfBooksReadByLanguageData =
          ChartUtilities.getLineSeriesForCategories(displayedLanguagePercentagesByTime, deltaDates);
    }

    //#endregion

    //#region Total Books Read by Language

    public totalBooksReadByLanguageLayout: any;

    public totalBooksReadByLanguageData = null;

    public setupTotalBooksReadByLanguageLayout(): void
    {
        this.totalBooksReadByLanguageLayout =
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

    public setupTotalBooksReadByLanguageCharts(): void
    {
        this.setupTotalBooksReadByLanguageLayout();

        // Get the list of languages for the final language tally
        let numberLanguageTallies = this.deltaBooks.length;
        let finalLanguageTotals = this.deltaBooks[numberLanguageTallies - 1].languageTotals;

        const sortedByBooks: ICategoryTotal[] = finalLanguageTotals.sort((t1, t2) =>
        {
            const ttl1 = t1.totalBooks;
            const ttl2 = t2.totalBooks;

            if (ttl1 > ttl2) { return -1; }
            if (ttl1 < ttl2) { return 1; }
            return 0;
        });

        const maxLanguages: number = SeriesColors.liveChartsColors.length;
        const includeOtherLanguage: boolean = (sortedByBooks.length > maxLanguages);

        let otherLabel: string = "Other";
        var displayedLanguageTotalsByTime: Map<string, number[]> = new Map<string, number[]>();
        let displayedLanguages: string[] = new Array<string>();

        for (let i = 0; i < sortedByBooks.length; i++)
        {
            let categoryTotal: ICategoryTotal = sortedByBooks[i];

            if (i < maxLanguages - 1)
            {
                displayedLanguageTotalsByTime.set(categoryTotal.name, new Array<number>());
                displayedLanguages.push(categoryTotal.name);
            }
            else
            {
                displayedLanguageTotalsByTime.set(otherLabel, new Array<number>());
            }
        }

        // Go through the deltas adding the percentages to the appropriate languages for the dates
        let deltaDates: Date[] = new Array<Date>();
        for (let i = 0; i < numberLanguageTallies; i++)
        {
            let deltaDate = new Date(this.deltaBooks[i].date);
            deltaDates.push(deltaDate);

            let languageTotals = this.deltaBooks[i].languageTotals;

            // if there are other languages set the default value for this delta to zero
            let otherTotal: number = 0;
            let languagesAdded: string[] = new Array<string>();

            for (let j = 0; j < languageTotals.length; j++)
            {
                let languageTotal = languageTotals[j];

                if (displayedLanguageTotalsByTime.has(languageTotal.name))
                {
                    displayedLanguageTotalsByTime.get(languageTotal.name).push(languageTotal.totalBooks);
                    languagesAdded.push(languageTotal.name);
                }
                else
                {
                    otherTotal += languageTotal.totalBooks;
                }
            }

            for (let j = 0; j < displayedLanguages.length; j++)
            {
                let displayedLanguage = displayedLanguages[j];
                if (languagesAdded.indexOf(displayedLanguage) === -1)
                {
                    displayedLanguageTotalsByTime.get(displayedLanguage).push(0);
                }
            }

            if (includeOtherLanguage)
                displayedLanguageTotalsByTime.get(otherLabel).push(otherTotal);
        }

        // Create a series per language & display the series on the plot
        this.totalBooksReadByLanguageData =
            ChartUtilities.getStackedAreaSeriesForCategories(displayedLanguageTotalsByTime, deltaDates);
    }

    //#endregion

    //#region Percentage of Books Read by Language

    public percentageOfPagesReadByLanguageLayout: any;

    public percentageOfPagesReadByLanguageData = null;

    public setupPercentageOfPagesReadByLanguageLayout(): void
    {
        this.percentageOfPagesReadByLanguageLayout =
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

    public setupPercentageOfPagesReadByLanguageCharts(): void
    {
        this.setupPercentageOfPagesReadByLanguageLayout();

        // Get the list of languages for the final language tally
        let numberLanguageTallies = this.deltaBooks.length;
        let finalLanguageTotals = this.deltaBooks[numberLanguageTallies - 1].languageTotals;

        const sortedByPages: ICategoryTotal[] = finalLanguageTotals.sort((t1, t2) =>
        {
            const ttl1 = t1.percentagePages;
            const ttl2 = t2.percentagePages;

            if (ttl1 > ttl2) { return -1; }
            if (ttl1 < ttl2) { return 1; }
            return 0;
        });

        const maxLanguages: number = SeriesColors.liveChartsColors.length;
        const includeOtherLanguage: boolean = (sortedByPages.length > maxLanguages);

        let otherLabel: string = "Other";
        var displayedLanguagePercentagesByTime: Map<string, number[]> = new Map<string, number[]>();

        for (let i = 0; i < sortedByPages.length; i++)
        {
            let categoryTotal: ICategoryTotal = sortedByPages[i];

            if (i < maxLanguages - 1)
            {
                displayedLanguagePercentagesByTime.set(categoryTotal.name, new Array<number>());
            }
            else
            {
                displayedLanguagePercentagesByTime.set(otherLabel, new Array<number>());
            }
        }

        // Go through the deltas adding the percentages to the appropriate languages for the dates
        let deltaDates: Date[] = new Array<Date>();
        for (let i = 0; i < numberLanguageTallies; i++)
        {
            let deltaDate = new Date(this.deltaBooks[i].date);
            deltaDates.push(deltaDate);

            let languageTotals = this.deltaBooks[i].languageTotals;

            // if there are other languages set the default value for this delta to zero
            let otherTotal: number = 0;

            for (let j = 0; j < languageTotals.length; j++)
            {
                let languageTotal = languageTotals[j];

                if (displayedLanguagePercentagesByTime.has(languageTotal.name))
                {
                    displayedLanguagePercentagesByTime.get(languageTotal.name).push(languageTotal.percentagePages);
                }
                else
                {
                    otherTotal += languageTotal.percentagePages;
                }
            }

            if (includeOtherLanguage)
                displayedLanguagePercentagesByTime.get(otherLabel).push(otherTotal);
        }

        // Create a series per language & display the series on the plot
        this.percentageOfPagesReadByLanguageData =
            ChartUtilities.getLineSeriesForCategories(displayedLanguagePercentagesByTime, deltaDates);
    }

    //#endregion

    //#region Total Pages Read by Language

    public totalPagesReadByLanguageLayout: any;

    public totalPagesReadByLanguageData = null;

    public setupTotalPagesReadByLanguageLayout(): void
    {
        this.totalPagesReadByLanguageLayout =
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

                width: ChartUtilities.chartWidth,
                height: ChartUtilities.chartHeight,
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

    public setupTotalPagesReadByLanguageCharts(): void
    {
        this.setupTotalPagesReadByLanguageLayout();

        // Get the list of languages for the final language tally
        let numberLanguageTallies = this.deltaBooks.length;
        let finalLanguageTotals = this.deltaBooks[numberLanguageTallies - 1].languageTotals;

        const sortedByPages: ICategoryTotal[] = finalLanguageTotals.sort((t1, t2) =>
        {
            const ttl1 = t1.totalPages;
            const ttl2 = t2.totalPages;

            if (ttl1 > ttl2) { return -1; }
            if (ttl1 < ttl2) { return 1; }
            return 0;
        });

        const maxLanguages: number = SeriesColors.liveChartsColors.length;
        const includeOtherLanguage: boolean = (sortedByPages.length > maxLanguages);

        let otherLabel: string = "Other";
        var displayedLanguageTotalsByTime: Map<string, number[]> = new Map<string, number[]>();
        let displayedLanguages: string[] = new Array<string>();

        for (let i = 0; i < sortedByPages.length; i++)
        {
            let categoryTotal: ICategoryTotal = sortedByPages[i];

            if (i < maxLanguages - 1)
            {
                displayedLanguageTotalsByTime.set(categoryTotal.name, new Array<number>());
                displayedLanguages.push(categoryTotal.name);
            }
            else
            {
                displayedLanguageTotalsByTime.set(otherLabel, new Array<number>());
            }
        }

        // Go through the deltas adding the percentages to the appropriate languages for the dates
        let deltaDates: Date[] = new Array<Date>();
        for (let i = 0; i < numberLanguageTallies; i++)
        {
            let deltaDate = new Date(this.deltaBooks[i].date);
            deltaDates.push(deltaDate);

            let languageTotals = this.deltaBooks[i].languageTotals;

            // if there are other languages set the default value for this delta to zero
            let otherTotal: number = 0;
            let languagesAdded: string[] = new Array<string>();

            for (let j = 0; j < languageTotals.length; j++) {
                let languageTotal = languageTotals[j];

                if (displayedLanguageTotalsByTime.has(languageTotal.name))
                {
                    displayedLanguageTotalsByTime.get(languageTotal.name).push(languageTotal.totalPages);
                    languagesAdded.push(languageTotal.name);
                }
                else
                {
                    otherTotal += languageTotal.totalPages;
                }
            }

            for (let j = 0; j < displayedLanguages.length; j++)
            {
                let displayedLanguage = displayedLanguages[j];
                if (languagesAdded.indexOf(displayedLanguage) === -1)
                {
                    displayedLanguageTotalsByTime.get(displayedLanguage).push(0);
                }
            }

            if (includeOtherLanguage)
                displayedLanguageTotalsByTime.get(otherLabel).push(otherTotal);
        }

        // Create a series per language & display the series on the plot
        this.totalPagesReadByLanguageData =
            ChartUtilities.getStackedAreaSeriesForCategories(displayedLanguageTotalsByTime, deltaDates);
    }

    //#endregion

    //#region Books and Pages Read by Language

    public currentPieChartByLanguageLayout: any;

    public booksReadByLanguageData = null;
    public pagesReadByLanguageData = null;

    public setupBooksAndPagesReadByLanguageLayout(): void
    {
        this.currentPieChartByLanguageLayout =
        {
            width: this.chartWidth/2,
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

    public setupBooksAndPagesReadByLanguageCharts(): void
    {
        this.setupBooksAndPagesReadByLanguageLayout();

        const sortedByBooks: LanguageAuthors[] = this.languageAuthors.sort((t1, t2) =>
        {
            const ttl1 = t1.totalBooksReadInLanguage;
            const ttl2 = t2.totalBooksReadInLanguage;

            if (ttl1 > ttl2) { return -1; }
            if (ttl1 < ttl2) { return 1; }
            return 0;
        });

        const sortedByPages: LanguageAuthors[] = this.languageAuthors.sort((t1, t2) =>
        {
            const ttl1 = t1.totalPagesReadInLanguage;
            const ttl2 = t2.totalPagesReadInLanguage;

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
                otherTotal += sortedItem.totalBooksReadInLanguage;
            }
            else
            {
                ttlLabels.push(sortedItem.name);
                ttlValues.push(sortedItem.totalBooksReadInLanguage);
                colors.push(SeriesColors.liveChartsColors[i]);
            }
        }

        if (includeOtherCategory)
        {
            ttlLabels.push(otherLabel);
            ttlValues.push(otherTotal);
            colors.push(otherColor);
        }

        this.booksReadByLanguageData =
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
                otherTotal += sortedItem.totalPagesReadInLanguage;
            }
            else
            {
                ttlLabels.push(sortedItem.name);
                ttlValues.push(sortedItem.totalPagesReadInLanguage);
                colors.push(SeriesColors.liveChartsColors[i]);
            }
        }

        if (includeOtherCategory)
        {
            ttlLabels.push(otherLabel);
            ttlValues.push(otherTotal);
            colors.push(otherColor);
        }

        this.pagesReadByLanguageData =
            ChartUtilities.getPiePlotData(ttlLabels, ttlValues, colors);
    }

    //#endregion

    //#region Books and Pages Read by Language

    public currentStarburstChartBooksByLanguageAndCountryLayout: any;

    public booksReadByLanguageAndCountryData = null;
    public pagesReadByLanguageAndCountryData = null;

    public setupBooksAndPagesReadByLanguageAndCountryLayout(): void
    {
        this.currentStarburstChartBooksByLanguageAndCountryLayout =
        {
            width: this.chartWidth,
            height: this.chartHeight,
            showlegend: true,
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

    public setupBooksAndPagesReadByLanguageAndCountryCharts(): void {
        this.setupBooksAndPagesReadByLanguageAndCountryLayout();

        // get the language & country counts for books
        const booksByLanguage: Map<string, Array<Book>> = new Map<string, Array<Book>>();
        const booksByCountry: Map<string, Array<Book>> = new Map<string, Array<Book>>();

        for (let i = 0; i < this.books.length; i++) {

            const book: Book = this.books[i];

            if (booksByLanguage.has(book.originalLanguage)) {
                booksByLanguage.get(book.originalLanguage).push(book);
            } else {
                const booksForLanguage: Array<Book> = new Array<Book>();
                booksForLanguage.push(book);
                booksByLanguage.set(book.originalLanguage, booksForLanguage);
            }

            if (booksByCountry.has(book.nationality)) {
                booksByCountry.get(book.nationality).push(book);
            } else {
                const booksForCountry: Array<Book> = new Array<Book>();
                booksForCountry.push(book);
                booksByCountry.set(book.nationality, booksForCountry);
            }
        }

        // get the language counts
        let sortedByLanguages =
            ByLanguageChartsComponent.getBooksSortedNameCountPairs(booksByLanguage);

        const maxItems: number = SeriesColors.liveChartsColors.length;

        let labels: string[] = new Array<string>();
        let parents: string[] = new Array<string>();
        let values: number[] = new Array<number>();

        let labelNamesUsed: Map<string, string> = new Map<string, string>();

        // Add the top level
        const allLanguagesLabel: string = "All";

        if (!labelNamesUsed.has(allLanguagesLabel))
        {
            labels.push(allLanguagesLabel);
            parents.push("");
            values.push(this.books.length);
            labelNamesUsed.set(allLanguagesLabel, allLanguagesLabel);
        }

        // Add the main language nodes
        for (let i = 0; i < maxItems && i < sortedByLanguages.length; i++)
        {
            let languageCount = sortedByLanguages[i];
            let languageName = languageCount.name;

            if (!labelNamesUsed.has(languageName))
            {
                labels.push(languageName);
                parents.push(allLanguagesLabel);
                values.push(languageCount.count);
                labelNamesUsed.set(languageName, languageName);
            }

            // get the sorted countries for this language
            let booksByCountryMap =
                ByLanguageChartsComponent.getBooksByCountryMap(booksByLanguage.get(languageName));

            let sortedBooksByCountry =
                ByLanguageChartsComponent.getBooksSortedNameCountPairs(booksByCountryMap);

            // add the child languages

            // Add the main language nodes
            for (let j = 0; j < maxItems && j < sortedBooksByCountry.length; j++)
            {
                let countryCount = sortedBooksByCountry[j];
                let countryName = countryCount.name;

                if (!labelNamesUsed.has(countryName)) {
                    labels.push(countryName);
                    parents.push(languageName);
                    values.push(countryCount.count);
                    labelNamesUsed.set(countryName, countryName);
                }

            }

        }

        this.booksReadByLanguageAndCountryData =
            ByLanguageChartsComponent.getStarburstPlotData(labels, parents, values, SeriesColors.liveChartsColors);
    }

    public static getBooksByCountryMap(books: Book[]): Map<string, Array<Book>>
    {
        const booksByCountry: Map<string, Array<Book>> = new Map<string, Array<Book>>();

        for (let i = 0; i < books.length; i++)
        {
            const book: Book = books[i];

            if (booksByCountry.has(book.nationality)) {
                booksByCountry.get(book.nationality).push(book);
            }
            else
            {
                const booksForCountry: Array<Book> = new Array<Book>();
                booksForCountry.push(book);
                booksByCountry.set(book.nationality, booksForCountry);
            }
        }

        return booksByCountry;
    }

    public static getBooksSortedNameCountPairs(booksMap: Map<string, Array<Book>>): NameCountPair[]
    {
        // Get the key counts pairs
        const mapKeys: string[] = Array.from(booksMap.keys());
        const keyCounts: NameCountPair[] = new Array<NameCountPair>();
        for (let i = 0; i < mapKeys.length; i++) {
            const mapKey: string = mapKeys[i];
            keyCounts.push(new NameCountPair(mapKey, booksMap.get(mapKey).length));
        }

        // sort by key & return
        const sortedByKeys: NameCountPair[] = keyCounts.sort((t1, t2) => {
            const ttl1 = t1.count;
            const ttl2 = t2.count;

            if (ttl1 > ttl2) { return -1; }
            if (ttl1 < ttl2) { return 1; }
            return 0;
        });

        return sortedByKeys;
    }

    public static getStarburstPlotData(labels: string[], parents: string[], values: number[], colors: string[]): any[] {
        var plotData: any[] =
        [
            {
                type: "sunburst",
                labels: labels,
                parents: parents,
                values: values,
                outsidetextfont: { size: 20, color: "#377eb8" },
                leaf: { opacity: 0.4 },
                marker: { line: { width: 2 } },
            }
        ];

        return plotData;
    }

    public onResetSunburstChart(chartName: string)
    {

        // log the change
        console.log("onResetSunburstChart : " + chartName);

        this.setupBooksAndPagesReadByLanguageAndCountryCharts();
    }

    //#endregion


}
