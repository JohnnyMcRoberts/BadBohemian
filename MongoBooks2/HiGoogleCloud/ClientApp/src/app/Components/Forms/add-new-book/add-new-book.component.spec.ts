/// <reference path="../../../../../../ClientApp/node_modules/@types/jasmine/index.d.ts" />
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { AddNewBookComponent } from './add-new-book.component';

let component: AddNewBookComponent;
let fixture: ComponentFixture<AddNewBookComponent>;

describe('AddNewBook component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ AddNewBookComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(AddNewBookComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});
