export interface IExportDataToEmailRequest {
    destinationEmail: string;
    format: string;
    note: string;
}

export class ExportDataToEmailRequest implements IExportDataToEmailRequest {
    static fromData(data: IExportDataToEmailRequest) {
        return new this(
            data.destinationEmail,
            data.format,
            data.note);
    }

    constructor(
        public destinationEmail: string = "",
        public format: string = "",
        public note: string = "") {

    }
}

export interface IExportDataToEmailResponse {
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


