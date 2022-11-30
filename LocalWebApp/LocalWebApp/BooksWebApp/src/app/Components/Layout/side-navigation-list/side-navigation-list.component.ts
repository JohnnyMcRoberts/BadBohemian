import { Component, OnInit, Output, EventEmitter } from '@angular/core';

export class NavigationMenuItem {
    public link: string | any;
    public text: string | any;
    public icon: string | any;
}

@Component({
    selector: 'app-side-navigation-list',
    templateUrl: './side-navigation-list.component.html',
    styleUrls: ['./side-navigation-list.component.scss']
})
/** SideNavigationList component*/
export class SideNavigationListComponent implements OnInit {

    @Output() sidenavClose = new EventEmitter();

    @Output() selectedMenuItem = new EventEmitter<string>();

    public navigationMenuItems: NavigationMenuItem[] =
    [
        {
            link: "/user-login", text: "User Login", icon: "lock_open"
        },
        {
            link: "/tables", text: "Tables", icon: "table_chart"
        },
        {
            link: "/charts", text: "Charts", icon: "timeline"
        },
        {
            link: "/forms", text: "Forms", icon: "library_add"
        },
        {
            link: "/import-export", text: "Import/Export", icon: "import_export"
        },
        //{
        //    link: "/to-do", text: "To Do List", icon: "list_alt"
        //},
        {
            link: "/home", text: "Home", icon: "home"
        }
    ];

    public static defaultMenuItemText: string = "User Login";

    /** SideNavigationList ctor */
    constructor() {

    }

    ngOnInit() {
    }

    public onSidenavClose = (param: any) => {
        console.log("closed using :" + param);
        this.sidenavClose.emit();
        this.selectedMenuItem.emit(param.toString());
    };
}
