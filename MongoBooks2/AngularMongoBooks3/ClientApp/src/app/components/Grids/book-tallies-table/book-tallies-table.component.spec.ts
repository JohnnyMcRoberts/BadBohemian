/// <reference path="../../../../../../node_modules/@types/jasmine/index.d.ts" />
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { BookTalliesTableComponent } from './book-tallies-table.component';

let component: BookTalliesTableComponent;
let fixture: ComponentFixture<BookTalliesTableComponent>;

describe('BookTalliesTable component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ BookTalliesTableComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(BookTalliesTableComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});