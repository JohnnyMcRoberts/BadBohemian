/// <reference path="../../../../../../ClientApp/node_modules/@types/jasmine/index.d.ts" />
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { BooksReadTableComponent } from './books-read-table.component';

let component: BooksReadTableComponent;
let fixture: ComponentFixture<BooksReadTableComponent>;

describe('BooksReadTable component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ BooksReadTableComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(BooksReadTableComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});
