/// <reference path="../../../../../../node_modules/@types/jasmine/index.d.ts" />
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { BookAndPageRatesComponent } from './book-and-page-rates.component';

let component: BookAndPageRatesComponent;
let fixture: ComponentFixture<BookAndPageRatesComponent>;

describe('BookAndPageRates component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ BookAndPageRatesComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(BookAndPageRatesComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});