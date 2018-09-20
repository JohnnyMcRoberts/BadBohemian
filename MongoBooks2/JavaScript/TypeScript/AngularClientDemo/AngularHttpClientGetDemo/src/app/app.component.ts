import { Component } from '@angular/core';
import { Observable } from 'rxjs';

import { DataService } from './data.service';
import { BooksReadDataService } from './books-read-data.service';
import { Product } from './product';
import { Family } from './family';
import { Location } from './location';
import { Transaction } from './transaction';
import { Author } from './author';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'AngularHttpClientGetDemo';

  private products: Product[] = [];
  private families: Family[] = [];
  private locations: Location[] = [];
  private transactions: Transaction[] = [];
  private authors: Author[] = [];
  private authors2: Author[] = [];

  private productsObservable: Observable<Product[]>;

  constructor(private dataService: DataService, private booksReadDataService: BooksReadDataService) {

    //this.productsObservable = this.dataService.get_products();

    this.dataService.get_families().subscribe((res: Family[]) => {
      this.families = res;
    });
    this.dataService.get_locations().subscribe((res: Location[]) => {
      console.log(res);
      this.locations = res;
    });
    this.dataService.get_transactions().subscribe((res: Transaction[]) => {
      console.log(res);
      this.transactions = res;
    });
    this.booksReadDataService.get_authors().subscribe((res: Author[]) => {
      console.log(res);
      console.log('From booksReadDataService ');
      this.authors = res;
    });
    this.dataService.get_authors().subscribe((res: Author[]) => {
      console.log(res);
      this.authors2 = res;
    });
  }


}
