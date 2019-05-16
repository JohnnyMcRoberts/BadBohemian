import { Component, OnInit, Output, EventEmitter } from '@angular/core';

export class NavigationMenuItem
{
  public link: string;
  public text: string;
  public icon: string;
}


@Component({
    selector: 'app-side-navigation-list',
    templateUrl: './side-navigation-list.component.html',
    styleUrls: ['./side-navigation-list.component.scss']
})
/** SideNavigationList component*/
export class SideNavigationListComponent implements OnInit
{

  @Output() sidenavClose = new EventEmitter();

  @Output() selectedMenuItem = new EventEmitter<string>();

  public navigationMenuItems: NavigationMenuItem[] =
  [
    {
      link: "/to-do-list", text: "To Do List", icon: "list_alt"
    },
    {
      link: "/user-login", text: "User Login", icon: "lock_open"
    },
    {
      link: "/main-books-grids", text: "Tables", icon: "table_chart"
    },
    {
      link: "/main-books-charts", text: "Charts", icon: "timeline"
    },
    {
      link: "/main-books-forms", text: "Forms", icon: "library_add"
    }
  ];

  public static defaultMenuItemText: string = "To Do List";

    /** SideNavigationList ctor */
  constructor()
  {

  }

  ngOnInit()
  {
  }

  public onSidenavClose = (param: any) =>
  {
    console.log("closed using :" + param);
    this.sidenavClose.emit();
    this.selectedMenuItem.emit(param.toString());
  }
}
