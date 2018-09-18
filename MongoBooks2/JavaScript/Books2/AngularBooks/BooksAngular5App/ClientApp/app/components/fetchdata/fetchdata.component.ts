import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';
import { IBookRead } from './../../interfaces/IBookRead';
import { AllBooksService } from './../../services/allBooks/allBooks.service';
import { AuthorService } from './../../services/allNames/allNames.service';

@Component({
    selector: 'fetchdata',
    templateUrl: './fetchdata.component.html'
})
export class FetchDataComponent {
  public forecasts: WeatherForecast[];
  public numberOfBooks: number;
  public allBooksRead: IBookRead[];
  public firstBooksRead: IBookRead;
  public allBooksService: AllBooksService;

  constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
    this.allBooksService = new AllBooksService(http);
    //this.numberOfBooks = this.allBooksService.getNumberBooks();

    console.log(" start FetchDataComponent\n\n\n\n\n\n");
    

    http.get('http://localhost:3000/allBooks').subscribe(
      result => {
        let res: IBookRead[] = result.json() as IBookRead[];
        console.log(" res = " + res.toString());
        this.numberOfBooks = res.length;
        this.allBooksRead = res;
        if (this.numberOfBooks > 0) {
          this.firstBooksRead = res[0];
        }
      },
      error => {
        console.error('\n\n\n This is an error \n\n\n');
        console.error(error);
        console.error('\n\n\n This Was an error \n\n\n');
      });

    
    http.get(baseUrl + 'api/SampleData/WeatherForecasts').subscribe(
      result =>
      {
          this.forecasts = result.json() as WeatherForecast[];
      },
      error => console.error(error));
  }

  //async ngOnInit() {
  //  this.numberOfBooks = await this.allBooksService.getTotalNumber();
  //}
}

interface WeatherForecast {
    dateFormatted: string;
    temperatureC: number;
    temperatureF: number;
    summary: string;
}
