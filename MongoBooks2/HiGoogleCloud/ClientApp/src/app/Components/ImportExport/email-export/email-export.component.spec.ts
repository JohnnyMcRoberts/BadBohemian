/// <reference path="../../../../../../node_modules/@types/jasmine/index.d.ts" />
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { EmailExportComponent } from './email-export.component';

let component: EmailExportComponent;
let fixture: ComponentFixture<EmailExportComponent>;

describe('EmailExport component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ EmailExportComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(EmailExportComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});