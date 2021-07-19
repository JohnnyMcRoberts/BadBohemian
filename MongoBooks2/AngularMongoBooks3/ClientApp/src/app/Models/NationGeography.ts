export interface INationGeography
{
    id: string;
    name: string;
    capital: string;
    latitude: number;
    longitude: number;
    imageUri: string;
    geographyXml: string;
};

export class NationGeography implements INationGeography
{
    static fromData(data: INationGeography)
    {
        return new this(
            data.id,
            data.name,
            data.capital,
            data.latitude,
            data.longitude,
            data.imageUri,
            data.geographyXml);
    }

    constructor(
        public id: string = "",
        public name: string = "",
        public capital: string = "",
        public latitude: number = 0,
        public longitude: number = 0,
        public imageUri: string = "",
        public geographyXml: string = "")
    {
    }
}
