/// <reference path="../../../../../../node_modules/@types/jasmine/index.d.ts" />
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { TextFileExportComponent } from './text-file-export.component';

let component: TextFileExportComponent;
let fixture: ComponentFixture<TextFileExportComponent>;

describe('TextFileExport component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ TextFileExportComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(TextFileExportComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});