import { Component, OnInit, AfterViewInit } from '@angular/core';
import { BooksDataService } from './../../../Services/books-data.service';

import { ChartUtilities } from './../../../Models/ChartUtilities';

import { DeltaBooks } from './../../../Models/DeltaBooks';
import { CountryAuthors } from './../../../Models/CountryAuthors';
import { CountryCodeLookup, ICountryCode } from './../../../Models/CountryCodeLookup';

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
export class CountryMapsComponent
implements OnInit, AfterViewInit
{
    /** CountryMaps ctor */
    constructor(booksDataService: BooksDataService)
    {
        this.componentTitle = "Loading books charts from database...";
        this.booksDataService = booksDataService;
    }

    private booksDataService: BooksDataService;
    public componentTitle: string;
    public countryAuthors: CountryAuthors[];
    public deltaBooks: DeltaBooks[];

    //#region Component Implementation

    ngOnInit()
    {
        this.booksDataService.fetchAllCountryAuthorsData().then(() =>
        {
            this.countryAuthors = new Array<CountryAuthors>();

            for (let item of this.booksDataService.countryAuthors)
            {
                var countryAuthor: CountryAuthors = item;
                this.countryAuthors.push(countryAuthor);
            }

            this.setupAllCharts();
        });
    }

    ngAfterViewInit() {
        this.setupAllCharts();
    }

    //#endregion

    //#region General Chart Data

    public plotlyConfig =
        {
            "displaylogo": false,
        };

    
    public plotlyMapLayout =
    {
        width: ChartUtilities.chartWidth,
        height: ChartUtilities.chartHeight,
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
        var countryMapData: any;
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
                    //autocolorscale: true,
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

    public booksReadByCountryMapData = null;

    public setupBooksReadByCountryMap(): void
    {
        const sortedByBooks: CountryAuthors[] = this.countryAuthors.sort((t1, t2) =>
        {
            const ttl1 = t1.totalBooksReadFromCountry;
            const ttl2 = t2.totalBooksReadFromCountry;

            if (ttl1 > ttl2) { return -1; }
            if (ttl1 < ttl2) { return 1; }
            return 0;
        });

        var locationCodes: string[] = new Array<string>();
        var zValues: number[] = new Array<number>();
        var locationNames: string[] = new Array<string>();

        var missingNames: string[] = new Array<string>();

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
                    let total = totalsByCountryCode.get(countryCode.code) + country.totalBooksReadFromCountry;
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
        var allCountryCodes: ICountryCode[] = CountryCodeLookup.getCountryCodes();

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

            locationCodes.push(code);
            locationNames.push(namesByCountryCode.get(code));
            zValues.push(totalsByCountryCode.get(code));
        }

        this.booksReadByCountryMapData = this.getCountryMapData(locationCodes, zValues, locationNames, "Total Books", ColorScale.Blue);
    }

    //#endregion

    //#region Pages Read by Country Map

    public pagesReadByCountryMapData = null;

    public setupPagesReadByCountryMap(): void
    {
        const sortedByBooks: CountryAuthors[] = this.countryAuthors.sort((t1, t2) =>
        {
            const ttl1 = t1.totalPagesReadFromCountry;
            const ttl2 = t2.totalPagesReadFromCountry;

            if (ttl1 > ttl2) { return -1; }
            if (ttl1 < ttl2) { return 1; }
            return 0;
        });

        var locationCodes: string[] = new Array<string>();
        var zValues: number[] = new Array<number>();
        var locationNames: string[] = new Array<string>();

        var missingNames: string[] = new Array<string>();

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
                    let total = totalsByCountryCode.get(countryCode.code) + country.totalPagesReadFromCountry;
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
        var allCountryCodes: ICountryCode[] = CountryCodeLookup.getCountryCodes();

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

            locationCodes.push(code);
            locationNames.push(namesByCountryCode.get(code));
            zValues.push(totalsByCountryCode.get(code));
        }

        this.pagesReadByCountryMapData = this.getCountryMapData(locationCodes, zValues, locationNames, "Pages Read", ColorScale.Red);
    }

    //#endregion

    //#region Authors Read by Country Map

    public authorsReadByCountryMapData = null;

    public setupAuthorsReadByCountryMap(): void
    {
        const sortedByBooks: CountryAuthors[] = this.countryAuthors.sort((t1, t2) =>
        {
            const ttl1 = t1.authors.length;
            const ttl2 = t2.authors.length;

            if (ttl1 > ttl2) { return -1; }
            if (ttl1 < ttl2) { return 1; }
            return 0;
        });

        var locationCodes: string[] = new Array<string>();
        var zValues: number[] = new Array<number>();
        var locationNames: string[] = new Array<string>();

        var missingNames: string[] = new Array<string>();

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
                    let total = totalsByCountryCode.get(countryCode.code) + country.authors.length;
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
        var allCountryCodes: ICountryCode[] = CountryCodeLookup.getCountryCodes();

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

            locationCodes.push(code);
            locationNames.push(namesByCountryCode.get(code));
            zValues.push(totalsByCountryCode.get(code));
        }

        this.authorsReadByCountryMapData = this.getCountryMapData(locationCodes, zValues, locationNames, "Authors", ColorScale.Purple);
    }

    //#endregion
}
