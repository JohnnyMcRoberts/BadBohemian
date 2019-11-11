export interface IExportText
{
    format: string;
    formattedText: string;
}

export class ExportText implements IExportText
{
    static fromData(data: IExportText)
    {
        return new this(
            data.format,
            data.formattedText);
    }

    constructor(
        public format: string = "",
        public formattedText: string = "")
    {

    }
}
