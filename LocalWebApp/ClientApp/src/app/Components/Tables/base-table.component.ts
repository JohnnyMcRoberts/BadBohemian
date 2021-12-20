import { ViewChild, Injectable } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';

/** LanguageAuthorsTable component*/
@Injectable()
export abstract class BaseTableComponent {
    public componentTitle: string | any;

    public items: any[] | any;
    public itemsDisplayedColumns: string[] = this.getItemsDisplayedColumns();

    @ViewChild('itemsTablePaginator') public itemsTablePaginator: MatPaginator | any;
    @ViewChild('itemsTableSort') public itemsTableSort: MatSort | any;
    public itemsDataSource: MatTableDataSource<any> | any;


    public setupItemsPagingAndSorting(): void {
        if (this.items != null) {
            setTimeout(() => {
                this.itemsDataSource.paginator = this.itemsTablePaginator;
                this.itemsDataSource.sort = this.itemsTableSort;
                this.itemsTableSort.sortChange.subscribe(() => {
                    this.itemsTablePaginator.pageIndex = 0;
                    this.itemsTablePaginator._changePageSize(this.itemsTablePaginator.pageSize);
                });
            });
        }
    }

    public applyItemsFilter(eventTarget: any)
    {
        const filterValue: string = (eventTarget as HTMLInputElement).value;
        this.itemsDataSource.filter = filterValue.trim().toLowerCase();

        if (this.itemsDataSource.paginator)
        {
            this.itemsTablePaginator.pageIndex = 0;
            this.itemsTablePaginator._changePageSize(this.itemsTablePaginator.pageSize);
        }
    }

    public abstract getItemsDisplayedColumns(): string[];
    public abstract onItemsRowClicked(row: any): void;
}

