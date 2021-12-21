import { Component, OnInit, AfterViewInit } from '@angular/core';
import { ViewportRuler } from '@angular/cdk/scrolling';
import { BooksDataService } from './../../../Services/books-data.service';

import { ChartUtilities } from './../../../Models/chart-utilities';

import { CountryAuthors } from './../../../Models/country-authors';
import { CountryCodeLookup, ICountryCode } from './../../../Models/country-code-lookup';

export enum ColorScale
{
    Purple,
    Blue,
    Red
};

@Component({
    selector: 'app-country-maps',
    templateUrl: './country-maps.component.html',
    styleUrls: ['./country-maps.component.scss']
})
/** CountryMaps component*/
export class CountryMapsComponent implements OnInit, AfterViewInit
{
    /** CountryMaps ctor */
    constructor(
        private viewportRuler: ViewportRuler,
        booksDataService: BooksDataService)
    {
        this.componentTitle = "Loading books charts from database...";
        this.booksDataService = booksDataService;
    }

    private booksDataService: BooksDataService;
    public componentTitle: string;
    public countryAuthors: CountryAuthors[] | any;

    public get loadingChartData(): boolean
    {

        return (!this.countryAuthors);
    }

    //#region Component Implementation

    ngOnInit()
    {
        this.booksDataService.fetchAllCountryAuthorsData().then(() =>
        {
            this.countryAuthors = new Array<CountryAuthors>();

            for (let item of this.booksDataService.countryAuthors as CountryAuthors[]) {
                const countryAuthor: CountryAuthors = item;
                this.countryAuthors.push(countryAuthor);
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
            autosize: true,
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

        if (viewportSize.width > 1 && viewportSize.width > 1)
        {
            this.chartWidth = Math.floor(this.viewPortWidth * 0.95);
            this.chartHeight = Math.floor(this.viewPortHeight * 0.8);
        }
    }

    public plotlyBooksReadByCountryMapLayout =
        {
            width: this.chartWidth,
            height: this.chartHeight,
            geo:
            {
                showframe: true,
                showcoastlines: true,
                projection:
                {
                    type: 'equirectangular'
                }
            }
        };

    public plotlyPagesReadByCountryMapLayout =
        {
            width: this.chartWidth,
            height: this.chartHeight,
            geo:
            {
                showframe: true,
                showcoastlines: true,
                projection:
                {
                    type: 'robinson'
                }
            }
        };

    public plotlAuthorsReadByCountryMapLayout =
        {
            width: this.chartWidth,
            height: this.chartHeight,
            geo:
            {
                showframe: true,
                showcoastlines: true,
                projection:
                {
                    type: 'miller'
                }
            }
        };

    public setupAllCharts(): void
    {
        if (this.countryAuthors != null && this.countryAuthors.length > 0)
            this.setupAllMapsByCountry();
    }

    public setupAllMapsByCountry(): void
    {
        this.setupBooksReadByCountryMap();
        this.setupPagesReadByCountryMap();
        this.setupAuthorsReadByCountryMap();
    }

    public getCountryMapData(
        locationCodes: string[],
        zValues: number[],
        locationNames: string[],
        title: string,
        colorScale: ColorScale = ColorScale.Red): any[]
    {
        let countryMapData: any;
        if (colorScale === ColorScale.Blue)
        {
            countryMapData =
                [
                    {
                        type: 'choropleth',
                        locations: locationCodes,
                        z: zValues,
                        text: locationNames,
                        colorscale:
                            [
                                [0, 'rgb(5, 10, 172)'], [0.35, 'rgb(40, 60, 190)'],
                                [0.5, 'rgb(70, 100, 245)'], [0.6, 'rgb(90, 120, 245)'],
                                [0.7, 'rgb(106, 137, 247)'], [0.99, 'rgb(220, 220, 220)'],
                                [1, 'rgb(255, 255, 255)']
                            ],
                        autocolorscale: false,
                        reversescale: true,
                        marker:
                        {
                            line:
                            {
                                color: 'rgb(180,180,180)',
                                width: 0.5
                            }
                        },
                        tick0: 0,
                        zmin: 0,
                        dtick: 1000,
                        colorbar:
                        {
                            autotic: false,
                            tickprefix: '',
                            title: title
                        }
                    }
                ];
        }
        else if (colorScale === ColorScale.Purple)
        {
            countryMapData =
                [
                    {
                        type: 'choropleth',
                        locations: locationCodes,
                        z: zValues,
                        text: locationNames,
                        colorscale:
                            [
                                [0, 'rgb(255,255,255)'],
                                [0.01, 'rgb(242,240,247)'],
                                [0.2, 'rgb(218,218,235)'],
                                [0.4, 'rgb(188,189,220)'],
                                [0.6, 'rgb(158,154,200)'],
                                [0.8, 'rgb(117,107,177)'],
                                [1, 'rgb(84,39,143)']
                            ],
                        autocolorscale: false,
                        marker:
                        {
                            line:
                            {
                                color: 'rgb(180,180,180)',
                                width: 0.5
                            }
                        },
                        tick0: 0,
                        zmin: 0,
                        dtick: 1000,
                        colorbar:
                        {
                            autotic: false,
                            tickprefix: '',
                            title: title
                        }
                    }
                ];
        }
        else
        {
            countryMapData =
                [
                    {
                        type: 'choropleth',
                        locations: locationCodes,
                        z: zValues,
                        text: locationNames,
                        colorscale:
                            [
                                [0, 'rgb(172, 10, 5)'], [0.35, 'rgb(190, 60, 40)'],
                                [0.5, 'rgb(245, 100, 70)'], [0.6, 'rgb(245, 120, 90)'],
                                [0.7, 'rgb(247, 137, 106)'], [0.99, 'rgb(220, 220, 220)'],
                                [1, 'rgb(255, 255, 255)']
                            ],
                        autocolorscale: false,
                        reversescale: true,
                        marker:
                        {
                            line:
                            {
                                color: 'rgb(180,180,180)',
                                width: 0.5
                            }
                        },
                        tick0: 0,
                        zmin: 0,
                        dtick: 1000,
                        colorbar:
                        {
                            autotic: false,
                            tickprefix: '',
                            title: title
                        }
                    }
                ];
        }

        return countryMapData;
    }

    //#endregion

    //#region Books Read by Country Map

    public booksReadByCountryMapData: any = null;

    public setupBooksReadByCountryMap(): void
    {
        const sortedByBooks: CountryAuthors[] =
            this.countryAuthors.sort((t1: CountryAuthors, t2: CountryAuthors) =>
            {
                const ttl1 = t1.totalBooksReadFromCountry;
                const ttl2 = t2.totalBooksReadFromCountry;

                if (ttl1 > ttl2) { return -1; }
                if (ttl1 < ttl2) { return 1; }
                return 0;
            });

        const locationCodes: string[] = new Array<string>();
        const zValues: number[] = new Array<number>();
        const locationNames: string[] = new Array<string>();

        const missingNames: string[] = new Array<string>();

        let totalsByCountryCode: Map<string, number> = new Map<string, number>();
        let namesByCountryCode: Map<string, string> = new Map<string, string>();

        for (let i = 0; i < sortedByBooks.length; i++)
        {
            let country: CountryAuthors = sortedByBooks[i];

            let countryCode: ICountryCode = CountryCodeLookup.getCountryCode(country.name);

            if (countryCode.code !== CountryCodeLookup.unknownCountryCode)
            {
                if (totalsByCountryCode.has(countryCode.code))
                {
                    let total = totalsByCountryCode.get(countryCode.code) as number + country.totalBooksReadFromCountry;
                    totalsByCountryCode.set(countryCode.code, total);
                }
                else
                {
                    totalsByCountryCode.set(countryCode.code, country.totalBooksReadFromCountry);
                    namesByCountryCode.set(countryCode.code, country.name);
                }
            }
            else
            {
                missingNames.push(country.name);
            }
        }

        // fill in the blanks with zeros
        const allCountryCodes: ICountryCode[] = CountryCodeLookup.getCountryCodes();

        for (let i = 0; i < allCountryCodes.length; i++)
        {
            let countryCode = allCountryCodes[i];

            if (!namesByCountryCode.has(countryCode.code))
            {
                namesByCountryCode.set(countryCode.code, countryCode.country);
                totalsByCountryCode.set(countryCode.code, 0);
            }
        }

        let countryCodes = Array.from(totalsByCountryCode.keys());

        for (let i = 0; i < countryCodes.length; i++)
        {
            let code: string = countryCodes[i];

            if (!code || code.length !== 3)
            {
                // ignore invalid country codes
                continue;
            }

            locationCodes.push(code);
            locationNames.push(namesByCountryCode.get(code) as string);
            zValues.push(totalsByCountryCode.get(code) as number);
        }

        this.booksReadByCountryMapData =
            this.getCountryMapData(locationCodes, zValues, locationNames, "Total Books", ColorScale.Blue);
    }

    //#endregion

    //#region Pages Read by Country Map

    public pagesReadByCountryMapData: any = null;

    public setupPagesReadByCountryMap(): void
    {
        const sortedByBooks: CountryAuthors[] =
            this.countryAuthors.sort((t1: CountryAuthors, t2: CountryAuthors) =>
            {
                const ttl1 = t1.totalPagesReadFromCountry;
                const ttl2 = t2.totalPagesReadFromCountry;

                if (ttl1 > ttl2) { return -1; }
                if (ttl1 < ttl2) { return 1; }
                return 0;
            });

        const locationCodes: string[] = new Array<string>();
        const zValues: number[] = new Array<number>();
        const locationNames: string[] = new Array<string>();

        const missingNames: string[] = new Array<string>();

        let totalsByCountryCode: Map<string, number> = new Map<string, number>();
        let namesByCountryCode: Map<string, string> = new Map<string, string>();

        for (let i = 0; i < sortedByBooks.length; i++)
        {
            let country: CountryAuthors = sortedByBooks[i];

            let countryCode: ICountryCode = CountryCodeLookup.getCountryCode(country.name);

            if (countryCode.code !== CountryCodeLookup.unknownCountryCode)
            {
                if (totalsByCountryCode.has(countryCode.code))
                {
                    let total =
                        totalsByCountryCode.get(countryCode.code) as number + country.totalPagesReadFromCountry;
                    totalsByCountryCode.set(countryCode.code, total);
                }
                else
                {
                    totalsByCountryCode.set(countryCode.code, country.totalPagesReadFromCountry);
                    namesByCountryCode.set(countryCode.code, country.name);
                }
            }
            else
            {
                missingNames.push(country.name);
            }
        }

        // fill in the blanks with zeros
        const allCountryCodes: ICountryCode[] = CountryCodeLookup.getCountryCodes();

        for (let i = 0; i < allCountryCodes.length; i++)
        {
            let countryCode = allCountryCodes[i];

            if (!namesByCountryCode.has(countryCode.code))
            {
                namesByCountryCode.set(countryCode.code, countryCode.country);
                totalsByCountryCode.set(countryCode.code, 0);
            }
        }

        let countryCodes = Array.from(totalsByCountryCode.keys());

        for (let i = 0; i < countryCodes.length; i++)
        {
            let code: string = countryCodes[i];

            if (!code || code.length !== 3) {
                // ignore invalid country codes
                continue;
            }

            locationCodes.push(code);
            locationNames.push(namesByCountryCode.get(code) as string);
            zValues.push(totalsByCountryCode.get(code) as number);
        }

        this.pagesReadByCountryMapData =
            this.getCountryMapData(locationCodes, zValues, locationNames, "Pages Read", ColorScale.Red);
    }

    //#endregion

    //#region Authors Read by Country Map

    public authorsReadByCountryMapData: any = null;

    public setupAuthorsReadByCountryMap(): void
    {
        const sortedByBooks: CountryAuthors[] =
            this.countryAuthors.sort((t1: CountryAuthors, t2: CountryAuthors) =>
            {
                const ttl1 = t1.authors.length;
                const ttl2 = t2.authors.length;

                if (ttl1 > ttl2) { return -1; }
                if (ttl1 < ttl2) { return 1; }
                return 0;
            });

        const locationCodes: string[] = new Array<string>();
        const zValues: number[] = new Array<number>();
        const locationNames: string[] = new Array<string>();

        const missingNames: string[] = new Array<string>();

        let totalsByCountryCode: Map<string, number> = new Map<string, number>();
        let namesByCountryCode: Map<string, string> = new Map<string, string>();

        for (let i = 0; i < sortedByBooks.length; i++)
        {
            let country: CountryAuthors = sortedByBooks[i];

            let countryCode: ICountryCode = CountryCodeLookup.getCountryCode(country.name);

            if (countryCode.code !== CountryCodeLookup.unknownCountryCode)
            {
                if (totalsByCountryCode.has(countryCode.code))
                {
                    let total = totalsByCountryCode.get(countryCode.code) as number + country.authors.length;
                    totalsByCountryCode.set(countryCode.code, total);
                }
                else
                {
                    totalsByCountryCode.set(countryCode.code, country.authors.length);
                    namesByCountryCode.set(countryCode.code, country.name);
                }
            }
            else
            {
                missingNames.push(country.name);
            }
        }

        // fill in the blanks with zeros
        const allCountryCodes: ICountryCode[] = CountryCodeLookup.getCountryCodes();

        for (let i = 0; i < allCountryCodes.length; i++)
        {
            let countryCode = allCountryCodes[i];

            if (!namesByCountryCode.has(countryCode.code))
            {
                namesByCountryCode.set(countryCode.code, countryCode.country);
                totalsByCountryCode.set(countryCode.code, 0);
            }
        }

        let countryCodes = Array.from(totalsByCountryCode.keys());

        for (let i = 0; i < countryCodes.length; i++)
        {
            let code: string = countryCodes[i];

            if (!code || code.length !== 3) {
                // ignore invalid country codes
                continue;
            }

            locationCodes.push(code);
            locationNames.push(namesByCountryCode.get(code) as string);
            zValues.push(totalsByCountryCode.get(code) as number);
        }

        this.authorsReadByCountryMapData =
            this.getCountryMapData(locationCodes, zValues, locationNames, "Authors", ColorScale.Purple);
    }

    //#endregion
}
