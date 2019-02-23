/// <reference path="../../../../../../node_modules/@types/jasmine/index.d.ts" />
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { AuthorsReadTableComponent } from './authors-read-table.component';

let component: AuthorsReadTableComponent;
let fixture: ComponentFixture<AuthorsReadTableComponent>;

describe('AuthorsReadTable component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ AuthorsReadTableComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(AuthorsReadTableComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});