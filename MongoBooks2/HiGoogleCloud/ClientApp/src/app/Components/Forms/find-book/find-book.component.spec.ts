/// <reference path="../../../../../../node_modules/@types/jasmine/index.d.ts" />
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { FindBookComponent } from './find-book.component';

let component: FindBookComponent;
let fixture: ComponentFixture<FindBookComponent>;

describe('FindBook component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ FindBookComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(FindBookComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});