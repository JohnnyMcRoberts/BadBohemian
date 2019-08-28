import { Component } from '@angular/core';
import { CurrentLoginService } from './../../../Services/current-login.service';

@Component({
    selector: 'app-main-import-export',
    templateUrl: './main-import-export.component.html',
    styleUrls: ['./main-import-export.component.scss']
})
/** MainImportExport component*/
export class MainImportExportComponent {
    /** MainImportExport ctor */
  constructor(private currentLoginService: CurrentLoginService) {

    }
}
