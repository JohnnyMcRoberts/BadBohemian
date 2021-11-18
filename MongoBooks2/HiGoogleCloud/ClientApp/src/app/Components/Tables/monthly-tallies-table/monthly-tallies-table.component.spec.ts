/// <reference path="../../../../../../ClientApp/node_modules/@types/jasmine/index.d.ts" />
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { MonthlyTalliesTableComponent } from './monthly-tallies-table.component';

let component: MonthlyTalliesTableComponent;
let fixture: ComponentFixture<MonthlyTalliesTableComponent>;

describe('MonthlyTalliesTable component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ MonthlyTalliesTableComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(MonthlyTalliesTableComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});
