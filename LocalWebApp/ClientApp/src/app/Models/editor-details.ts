export interface IEditorDetails {
    authorNames: string[];
    countryNames: string[];
    languages: string[];
    tags: string[];
};

export class EditorDetails implements IEditorDetails {
    constructor(
        public authorNames: string[] = new Array<string>(),
        public countryNames: string[] = new Array<string>(),
        public languages: string[] = new Array<string>(),
        public tags: string[] = new Array<string>()) {
    }
}
