/// <reference path="../../../../../../ClientApp/node_modules/@types/jasmine/index.d.ts" />
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { EditExistingBookComponent } from './edit-existing-book.component';

let component: EditExistingBookComponent;
let fixture: ComponentFixture<EditExistingBookComponent>;

describe('EditExistingBook component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ EditExistingBookComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(EditExistingBookComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});
