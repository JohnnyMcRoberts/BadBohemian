/// <reference path="../../../../../../node_modules/@types/jasmine/index.d.ts" />
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { GoogleBookItemComponent } from './google-book-item.component';

let component: GoogleBookItemComponent;
let fixture: ComponentFixture<GoogleBookItemComponent>;

describe('GoogleBookItem component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ GoogleBookItemComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(GoogleBookItemComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});