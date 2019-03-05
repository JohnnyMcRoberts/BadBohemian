/// <reference path="../../../../../../node_modules/@types/jasmine/index.d.ts" />
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { LanguageAuthorsTableComponent } from './language-authors-table.component';

let component: LanguageAuthorsTableComponent;
let fixture: ComponentFixture<LanguageAuthorsTableComponent>;

describe('LanguageAuthorsTable component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ LanguageAuthorsTableComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(LanguageAuthorsTableComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});