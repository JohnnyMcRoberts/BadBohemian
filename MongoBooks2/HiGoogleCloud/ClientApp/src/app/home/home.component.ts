import { Component, ViewChild } from '@angular/core';

import { MatTabChangeEvent, MatTabGroup } from '@angular/material/tabs';

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.scss']
})
export class HomeComponent {

    public readonly angularVersions =
        {
            angularCLI: "12.0.2",
            node: "12.16.2",
            packageManager: "npm 6.9.0",
            os: "win32 x64",

            angular:
            {
                version: "12.0.3",
                packages: [
                    "animations",
                    "common",
                    "compiler",
                    "compiler-cli",
                    "core",
                    "forms",
                    "language-service",
                    "platform-browser",
                    "platform-browser-dynamic",
                    "platform-server",
                    "router"
                ]
            },

            packageVersions:
                [
                    { package: "@angular-devkit/architect", version: "0.1200.5" },
                    { package: "@angular-devkit/build-angular", version: "12.0.5" },
                    { package: "@angular-devkit/build-ng-packagr", version: "0.901.15" },
                    { package: "@angular-devkit/core", version: "12.0.5" },
                    { package: "@angular-devkit/schematics", version: "12.0.2" },
                    { package: "@angular/cdk", version: "12.2.7" },
                    { package: "@angular/cli", version: "12.0.2" },
                    { package: "@angular/localize", version: "12.2.7" },
                    { package: "@angular/material", version: "12.2.7" },
                    { package: "@angular/service-worker", version: "12.0.0" },
                    { package: "@nguniversal/module-map-ngfactory-loader", version: "8.2.6" },
                    { package: "@schematics/angular", version: "12.0.2" },
                    { package: "ng-packagr", version: "12.2.2" },
                    { package: "rxjs", version: "6.6.3" },
                    { package: "typescript", version: "4.2.4" },
                    { package: "webpack", version: "5.54.0" }
                ]
        };

    // #region Child Component Data

    @ViewChild('tabGroup', { static: false }) tabGroup: MatTabGroup;

    // #endregion

    constructor() {
        // store the versions
        //this.appVersion = version;

        //console.log(" --> home App version = " + JSON.stringify(this.angularVersions, null, 4));
    }
}
