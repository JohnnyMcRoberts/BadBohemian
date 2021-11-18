import { Component, OnInit, Output, EventEmitter, Input, SimpleChange } from '@angular/core';

@Component({
    selector: 'app-navigation-header',
    templateUrl: './navigation-header.component.html',
    styleUrls: ['./navigation-header.component.scss']
})
/** NavigationHeader component*/
export class NavigationHeaderComponent implements OnInit {
    @Output()
    public sidenavToggle = new EventEmitter();

    /** NavigationHeader ctor */
    constructor() {

    }

    ngOnInit() {
    }

    public onToggleSidenav = () => {
        this.sidenavToggle.emit();
    };

    //#region OnChanges implementation

    @Input()
    selectedMenuOption: string;

    private updates: number = 0;

    ngOnChanges(changes: { [propKey: string]: SimpleChange; }) {
        for (let propName in changes) {
            if (changes.hasOwnProperty(propName)) {
                let changedProp = changes[propName];
                var currentValue = changedProp.currentValue;

                this.updates++;

                console.log("Update menu to :" + currentValue + " update count is " + this.updates);
            }
        }
    }

    //#endregion
}
