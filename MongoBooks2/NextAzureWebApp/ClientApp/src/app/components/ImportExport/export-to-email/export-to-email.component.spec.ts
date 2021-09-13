/// <reference path="../../../../../../node_modules/@types/jasmine/index.d.ts" />
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { ExportToEmailComponent } from './export-to-email.component';

let component: ExportToEmailComponent;
let fixture: ComponentFixture<ExportToEmailComponent>;

describe('ExportToEmail component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ ExportToEmailComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(ExportToEmailComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});