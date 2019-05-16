import { Component } from '@angular/core';

export interface Section
{
  name: string;
  updated: Date;
}

@Component({
  selector: 'app-home-component',
  templateUrl: './home.component.html'
})
export class HomeComponent
{
  public complete: Section[] = [
    {
      name: 'Get Material and Structure going',
      updated: new Date('21/02/19'),
    },
    {
      name: 'Add Main Grid headings',
      updated: new Date('21/02/19'),
    },
    {
      name: 'Read & display data from the Mongo DB',
      updated: new Date('21/02/19'),
    },
    {
      name: 'Add the other Grids',
      updated: new Date('20/03/19'),
    },
  ];

  public todos: Section[] =
  [
    {
      name: 'Add the users login',
      updated: new Date('26/08/19'),
    },
    {
      name: 'Add the books editor',
      updated: new Date('28/03/19'),
    },
    {
      name: 'Add the charts',
      updated: new Date('28/03/19'),
    },
  ];
}
