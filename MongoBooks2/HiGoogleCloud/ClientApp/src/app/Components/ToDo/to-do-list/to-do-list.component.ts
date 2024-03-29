import { Component } from '@angular/core';

export interface Section
{
    name: string;
    updated: Date;
}

@Component({
    selector: 'app-to-do-list',
    templateUrl: './to-do-list.component.html',
    styleUrls: ['./to-do-list.component.scss']
})
/** ToDoList component*/
export class ToDoListComponent
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
        {
            name: 'Add the users login',
            updated: new Date('26/08/19'),
        },
        {
            name: 'Add the books editor',
            updated: new Date('28/08/19'),
        },
        {
            name: 'Add the other charts',
            updated: new Date('27/11/19'),
        },
        {
            name: 'Add the file export',
            updated: new Date('16/09/19'),
        },
        {
            name: 'Add the find book service',
            updated: new Date('25/05/21'),
        },
    ];

    public todos: Section[] =
    [
        {
            name: 'Add the file import',
            updated: new Date('16/09/19'),
        },
        {
            name: 'Add User verify e-mails',
            updated: new Date('26/05/21'),
        },
        {
            name: 'Add Export to e-mail',
            updated: new Date('26/05/21'),
        }
    ];

    /** ToDoList ctor */
    constructor()
    {

    }
}
