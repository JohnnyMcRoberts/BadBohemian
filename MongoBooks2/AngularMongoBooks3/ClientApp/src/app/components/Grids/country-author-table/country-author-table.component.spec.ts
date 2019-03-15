/// <reference path="../../../../../../node_modules/@types/jasmine/index.d.ts" />
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { CountryAuthorTableComponent } from './country-author-table.component';

let component: CountryAuthorTableComponent;
let fixture: ComponentFixture<CountryAuthorTableComponent>;

describe('CountryAuthorTable component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ CountryAuthorTableComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(CountryAuthorTableComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});