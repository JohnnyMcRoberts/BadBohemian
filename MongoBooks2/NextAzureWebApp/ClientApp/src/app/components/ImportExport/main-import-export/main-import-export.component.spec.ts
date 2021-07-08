/// <reference path="../../../../../../node_modules/@types/jasmine/index.d.ts" />
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { MainImportExportComponent } from './main-import-export.component';

let component: MainImportExportComponent;
let fixture: ComponentFixture<MainImportExportComponent>;

describe('MainImportExport component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ MainImportExportComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(MainImportExportComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});