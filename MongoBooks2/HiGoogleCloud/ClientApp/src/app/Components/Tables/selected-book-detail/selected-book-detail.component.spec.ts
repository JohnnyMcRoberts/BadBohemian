/// <reference path="../../../../../../node_modules/@types/jasmine/index.d.ts" />
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { SelectedBookDetailComponent } from './selected-book-detail.component';

let component: SelectedBookDetailComponent;
let fixture: ComponentFixture<SelectedBookDetailComponent>;

describe('SelectedBookDetail component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ SelectedBookDetailComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(SelectedBookDetailComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});