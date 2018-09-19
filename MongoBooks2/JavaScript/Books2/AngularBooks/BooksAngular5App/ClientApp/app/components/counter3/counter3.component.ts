import { Component } from '@angular/core';
import { AuthorService, Author } from './../../services/allNames/allNames.service';

@Component({
    selector: 'counter3',
    templateUrl: './counter3.component.html'
})
export class Counter3Component {

  constructor(private messageService: AuthorService) { }

  public currentCount = 0;

  public currentAuthors: Author[];

  public incrementCounter() {
    let authors = this.messageService.getAllAuthors().subscribe(
      result => {
        console.log(' got a response from the observable' + result.toString());
        this.currentAuthors = result;
      },
      error => console.error(error));
    this.currentCount += 2;
    }
}