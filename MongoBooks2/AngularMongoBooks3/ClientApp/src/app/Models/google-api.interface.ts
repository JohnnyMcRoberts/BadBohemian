export interface GoogleBooksApiInterface
{
    kind: string;
    totalItems: number;
    items: GoogleBookInterface[];
};

export interface GoogleBookInterface
{
    id: string;
    volumeInfo:
    {
        title: string;
        subtitle: string;
        authors: string[];
        publisher: string;
        publishDate: string;
        description: string;
        averageRating: number;
        ratingsCount: number;
        imageLinks:
        {
            thumbnail: string;
            smallThumbnail: string;
        };
    };
};

export interface GoogleBookIndustryIdentifierInterface
{
    type: string;       // Identifier type. Possible values are ISBN_10, ISBN_13, ISSN and OTHER.
    identifier: string; // Industry specific volume identifier.
}

export class GoogleBookIndustryIdentifier implements GoogleBookIndustryIdentifierInterface
{
    static fromData(data: GoogleBookIndustryIdentifierInterface)
    {
        return new this(
            data.type,
            data.identifier);
    }

    constructor(
        public type: string = "",
        public identifier: string = ""
    )
    {
    }
}

export interface GoogleBookVolumeInfoImageLinksInterface
{
    thumbnail: string;
    smallThumbnail: string;
    small: string;
    medium: string;
    large: string;
    extraLarge: string;

}

export class GoogleBookVolumeInfoImageLinks implements GoogleBookVolumeInfoImageLinksInterface
{
    static getSafeString(value: string): string
    {
        return (value === undefined || value === null) ? "" : value;
    }

    static fromData(data: GoogleBookVolumeInfoImageLinksInterface)
    {
        return new this(
            this.getSafeString(data.thumbnail),
            this.getSafeString(data.smallThumbnail),
            this.getSafeString(data.small),
            this.getSafeString(data.medium),
            this.getSafeString(data.large),
            this.getSafeString(data.extraLarge));
    }

    constructor(
        public thumbnail: string = "",
        public smallThumbnail: string = "",
        public small: string = "",
        public medium: string = "",
        public large: string = "",
        public extraLarge: string = "")
    {
    }
}

export interface GoogleBookVolumeInfoInterface
{
    title: string;          //	Volume title. (In LITE projection.);
    subtitle: string;       //	Volume subtitle. (In LITE projection.)	
    authors: string[];      //	list	The names of the authors and/or editors for this volume. (In LITE projection)	
    publisher: string;      //	Publisher of this volume. (In LITE projection.)	
    publishedDate: string;  //	Date of publication. (In LITE projection.)	
    description: string;    //	A synopsis of the volume. The text of the description is formatted in HTML and includes simple formatting elements, such as b, i, and br tags. (in LITE projection)

    industryIdentifiers: GoogleBookIndustryIdentifierInterface[]; //Industry standard identifiers for this volume.

    pageCount: number;      // Total number of pages.
    printType: string;      //	Type of publication of this volume.Possible values are BOOK or MAGAZINE.
    categories: string[];   // 	A list of subject categories, such as "Fiction", "Suspense", etc.

    averageRating: number;  //	The mean review rating for this volume. (min = 1.0, max = 5.0)	
    ratingsCount: number;   //	The number of review ratings for this volume.
    contentVersion: string; //	An identifier for the version of the volume content(text & images). (In LITE projection)	

    imageLinks: GoogleBookVolumeInfoImageLinksInterface; // A list of image links for all the sizes that are available. (in LITE projection)	

    language: string;               //	Best language for this volume(based on content).It is the two - letter ISO 639 - 1 code such as 'fr', 'en', etc.	
    previewLink: string;            //	URL to preview this volume on the Google Books site.
    infoLink: string;               //	URL to view information about this volume on the Google Books site. (In LITE projection);
    canonicalVolumeLink: string;    //	Canonical URL for a volume. (In LITE projection.)	
}

export class GoogleBookVolumeInfo implements GoogleBookVolumeInfoInterface
{
    static getSafeString(value: string): string
    {
        return GoogleBookVolumeInfoImageLinks.getSafeString(value);
    }

    static getSafeNumber(value: number): number
    {
        return (value === undefined || value === null) ? 0 : value;
    }

    static getSafeStrings(values: string[]): string[]
    {
        const safeValues: string[] = new Array<string>();
        if (values === undefined || values === null || values.length === 0)
            return safeValues;

        for (let i = 0; i < values.length; i++) {
            safeValues.push(GoogleBookVolumeInfoImageLinks.getSafeString(values[i]));
        }

        return safeValues;
    }

    static getSafeGoogleBookIndustryIdentifiers(values: GoogleBookIndustryIdentifier[]): GoogleBookIndustryIdentifier[]
    {
        const safeValues: GoogleBookIndustryIdentifier[] = new Array<GoogleBookIndustryIdentifier>();
        if (values === undefined || values === null || values.length === 0)
            return safeValues;

        for (let i = 0; i < values.length; i++)
        {
            safeValues.push(GoogleBookIndustryIdentifier.fromData(values[i]));
        }

        return safeValues;
    }

    static getSafeGoogleBookVolumeInfoImageLinks(values: GoogleBookVolumeInfoImageLinks): GoogleBookVolumeInfoImageLinksInterface
    {
        if (values === undefined || values === null)
            return new GoogleBookVolumeInfoImageLinks();

        return GoogleBookVolumeInfoImageLinks.fromData(values);
    }

    static fromData(data: GoogleBookVolumeInfoInterface)
    {
        return new this(
            this.getSafeString(data.title),
            this.getSafeString(data.subtitle),
            this.getSafeStrings(data.authors),

            this.getSafeString(data.publisher),
            this.getSafeString(data.publishedDate),
            this.getSafeString(data.description),

            this.getSafeGoogleBookIndustryIdentifiers(data.industryIdentifiers),
            this.getSafeNumber(data.pageCount),
            this.getSafeString(data.printType),

            this.getSafeStrings(data.categories),
            this.getSafeNumber(data.averageRating),
            this.getSafeNumber(data.ratingsCount),

            this.getSafeString(data.contentVersion),
            this.getSafeGoogleBookVolumeInfoImageLinks(data.imageLinks),
            this.getSafeString(data.language),

            this.getSafeString(data.previewLink),
            this.getSafeString(data.infoLink),
            this.getSafeString(data.canonicalVolumeLink));
    }

    public get allAuthors(): string
    {
        if (this.authors === undefined || this.authors === null || this.authors.length === 0)
            return "";

        return this.authors.join(", ");
    }

    public get isbn10(): string {
        if (this.industryIdentifiers === undefined || this.industryIdentifiers === null || this.industryIdentifiers.length === 0)
            return "";

        let isbn = "";

        for (let i = 0; i < this.industryIdentifiers.length; i++)
        {
            if (this.industryIdentifiers[i].type === "ISBN_10")
                isbn = this.industryIdentifiers[i].identifier;
        }

        return isbn;
    }

    public get isbn13(): string
    {
        if (this.industryIdentifiers === undefined || this.industryIdentifiers === null || this.industryIdentifiers.length === 0)
            return "";

        let isbn = "";

        for (let i = 0; i < this.industryIdentifiers.length; i++)
        {
            if (this.industryIdentifiers[i].type === "ISBN_13")
                isbn = this.industryIdentifiers[i].identifier;
        }

        return isbn;
    }

    public get allCategories(): string
    {
        if (this.categories === undefined || this.categories === null || this.categories.length === 0)
            return "None Specified";

        return this.categories.join(", ");
    }

    public get imageLinkFullPath(): string
    {
        if (this.imageLinks === undefined || this.imageLinks === null)
            return "";

        if (this.imageLinks.extraLarge.length > 0)
            return this.imageLinks.extraLarge;
        if (this.imageLinks.large.length > 0)
            return this.imageLinks.large;
        if (this.imageLinks.medium.length > 0)
            return this.imageLinks.medium;
        if (this.imageLinks.thumbnail.length > 0)
            return this.imageLinks.thumbnail;
        if (this.imageLinks.small.length > 0)
            return this.imageLinks.small;

        return this.imageLinks.smallThumbnail;
    }

    get imageLinkThumbnail(): string
    {
        return this.imageLinkFullPath
            ? this.imageLinkFullPath.replace("http:", "https:")
            : "";
    }

    constructor(
        public title: string = "",
        public subtitle: string = "",
        public authors: string[] = new Array<string>(),

        public publisher: string = "",
        public publishedDate: string = "",
        public description: string = "",

        public industryIdentifiers: GoogleBookIndustryIdentifierInterface[] = new Array<GoogleBookIndustryIdentifier>(),
        public pageCount: number = 0,
        public printType: string = "",

        public categories: string[] = new Array<string>(),
        public averageRating: number = 0,
        public ratingsCount: number = 0,

        public contentVersion: string = "",
        public imageLinks: GoogleBookVolumeInfoImageLinksInterface = new GoogleBookVolumeInfoImageLinks(),
        public language: string = "",

        public previewLink: string = "",
        public infoLink: string = "",
        public canonicalVolumeLink: string = ""){}
}

export interface GoogleBookDetailInterface
{
    kind: string;       // Resource type for a volume. (In LITE projection.)	
    id: string;         // Unique identifier for a volume. (In LITE projection.)	
    etag: string;       // Opaque identifier for a specific version of a volume resource. (In LITE projection);
    selfLink: string;   // URL to this resource. (In LITE projection.)	

    volumeInfo: GoogleBookVolumeInfoInterface;
};

export class GoogleBookDetail implements GoogleBookDetailInterface
{
    static fromData(data: GoogleBookDetailInterface)
    {
        let volumeInfo: GoogleBookVolumeInfoInterface;
        if (data.volumeInfo === undefined || data.volumeInfo === null)
            volumeInfo = new GoogleBookVolumeInfo();
        else
            volumeInfo = GoogleBookVolumeInfo.fromData(data.volumeInfo);

        return new this(
            GoogleBookVolumeInfoImageLinks.getSafeString(data.kind),
            GoogleBookVolumeInfoImageLinks.getSafeString(data.id),
            GoogleBookVolumeInfoImageLinks.getSafeString(data.etag),
            GoogleBookVolumeInfoImageLinks.getSafeString(data.selfLink),
            volumeInfo);
    }

    constructor(
        public kind: string = "",
        public id: string = "",
        public etag: string = "",
        public selfLink: string = "",
        public volumeInfo: GoogleBookVolumeInfoInterface = new GoogleBookVolumeInfo())
    {}
}
