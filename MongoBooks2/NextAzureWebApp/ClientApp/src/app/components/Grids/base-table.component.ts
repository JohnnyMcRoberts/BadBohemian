import { ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';

/** LanguageAuthorsTable component*/
export abstract class BaseTableComponent
{
  public componentTitle: string;

  public items: any[];
  public itemsDisplayedColumns: string[] = this.getItemsDisplayedColumns();

  @ViewChild('itemsTablePaginator') public itemsTablePaginator: MatPaginator;
  @ViewChild('itemsTableSort') public itemsTableSort: MatSort;
  public itemsDataSource: MatTableDataSource<any>;


  public setupItemsPagingAndSorting(): void
  {
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

  public applyItemsFilter(filterValue: string)
  {
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
