/// <reference path="../../../../../../ClientApp/node_modules/@types/jasmine/index.d.ts" />
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { BookDetailComponent } from './book-detail.component';

let component: BookDetailComponent;
let fixture: ComponentFixture<BookDetailComponent>;

describe('BookDetail component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ BookDetailComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(BookDetailComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});
