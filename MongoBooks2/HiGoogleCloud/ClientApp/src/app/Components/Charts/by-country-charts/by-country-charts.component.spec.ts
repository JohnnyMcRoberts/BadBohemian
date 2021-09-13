/// <reference path="../../../../../../node_modules/@types/jasmine/index.d.ts" />
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { ByCountryChartsComponent } from './by-country-charts.component';

let component: ByCountryChartsComponent;
let fixture: ComponentFixture<ByCountryChartsComponent>;

describe('ByCountryCharts component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ ByCountryChartsComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(ByCountryChartsComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});