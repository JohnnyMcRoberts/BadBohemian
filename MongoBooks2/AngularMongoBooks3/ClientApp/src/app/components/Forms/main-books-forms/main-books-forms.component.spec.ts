/// <reference path="../../../../../../node_modules/@types/jasmine/index.d.ts" />
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { MainBooksFormsComponent } from './main-books-forms.component';

let component: MainBooksFormsComponent;
let fixture: ComponentFixture<MainBooksFormsComponent>;

describe('MainBooksForms component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ MainBooksFormsComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(MainBooksFormsComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});