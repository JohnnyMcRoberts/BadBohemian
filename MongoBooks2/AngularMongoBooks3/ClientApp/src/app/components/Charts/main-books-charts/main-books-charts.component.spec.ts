/// <reference path="../../../../../../node_modules/@types/jasmine/index.d.ts" />
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { MainBooksChartsComponent } from './main-books-charts.component';

let component: MainBooksChartsComponent;
let fixture: ComponentFixture<MainBooksChartsComponent>;

describe('MainBooksCharts component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ MainBooksChartsComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(MainBooksChartsComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});