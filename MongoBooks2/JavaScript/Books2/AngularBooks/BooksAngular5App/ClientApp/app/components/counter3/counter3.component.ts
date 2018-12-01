import { Component, OnInit } from '@angular/core';
import { AuthorService, Author } from './../../services/allNames/allNames.service';
import { AllNames2Service } from './../../services/allNames2/allNames2.service';

@Component({
    selector: 'counter3',
    templateUrl: './counter3.component.html'
})
export class Counter3Component implements OnInit {

  currentAuthors: Author[];

  constructor(private namesService: AllNames2Service) {
    this.currentAuthors = namesService.allAuthors;
  }

  ngOnInit() {
    console.log(' Counter3Component  starting ngOnInit\n\n');
    //this.getHeroes();
    console.log(' Counter3Component  finished ngOnInit\n\n');
  }

  //getHeroes(): void {
  //  console.log(' Counter3Component  getting heroes\n\n');
  //  this.messageService.getAllAuthors()
  //    .subscribe(
  //    (data: Author[]) => {
  //      console.log(' Counter3Component got data OK\n\n');
  //      this.currentAuthors = data;

  //      for (let author of data) {
  //        console.log(' -> ' + author.name + '\n');
  //      }
  //    }
  //    //  , // success path
  //    //error => {
  //    //  console.log(' Counter3Component got data error\n\n' + error.toString() + ' \n\n\n');
  //    //} // error path
  //  );
  //}

  public currentCount = 0;

  public incrementCounter() {
    this.currentCount += 2;
    }
}