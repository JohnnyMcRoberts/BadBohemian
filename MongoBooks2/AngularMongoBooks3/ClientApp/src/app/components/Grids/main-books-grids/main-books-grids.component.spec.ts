/// <reference path="../../../../../../node_modules/@types/jasmine/index.d.ts" />
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { MainBooksGridsComponent } from './main-books-grids.component';

let component: MainBooksGridsComponent;
let fixture: ComponentFixture<MainBooksGridsComponent>;

describe('MainBooksGrids component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ MainBooksGridsComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(MainBooksGridsComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});