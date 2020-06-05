export interface GoogleBooksApiInterface
{
    kind: string;
    totalItems: number;
    items: GoogleBook[];
};

export interface GoogleBook
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
