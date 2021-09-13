export interface IExportDataToEmailRequest
{
    destinationEmail: string;
    format: string;
    source: string;
}

export class ExportDataToEmailRequest implements IExportDataToEmailRequest
{
    static fromData(data: IExportDataToEmailRequest)
    {
        return new this(
            data.destinationEmail,
            data.format,
            data.source);
    }

    constructor(
        public destinationEmail: string = "",
        public format: string = "",
        public source: string = "")
    {

    }
}

export interface IExportDataToEmailResponse
{
    destinationEmail: string;
    sentSuccessfully: boolean;
    error: string;
}

export class ExportDataToEmailResponse implements IExportDataToEmailResponse {
    static fromData(data: IExportDataToEmailResponse) {
        return new this(
            data.destinationEmail,
            data.sentSuccessfully,
            data.error);
    }

    constructor(
        public destinationEmail: string = "",
        public sentSuccessfully: boolean = false,
        public error: string = "") {

    }
}


