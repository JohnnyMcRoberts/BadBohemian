/// <reference path="../../../../../../node_modules/@types/jasmine/index.d.ts" />
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { BooksTagsTableComponent } from './books-tags-table.component';

let component: BooksTagsTableComponent;
let fixture: ComponentFixture<BooksTagsTableComponent>;

describe('BooksTagsTable component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ BooksTagsTableComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(BooksTagsTableComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});