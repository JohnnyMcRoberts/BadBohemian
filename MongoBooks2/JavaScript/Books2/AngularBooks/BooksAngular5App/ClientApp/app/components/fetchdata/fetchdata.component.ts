import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';
import { IBookRead } from './../../interfaces/IBookRead';
import { AllBooksService } from './../../services/allBooks/allBooks.service';

@Component({
    selector: 'fetchdata',
    templateUrl: './fetchdata.component.html'
})
export class FetchDataComponent {
  public forecasts: WeatherForecast[];

  constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
    //this.numberOfBooks = this.allBooksService.getNumberBooks();

    let forecastsUrl = baseUrl + 'api/SampleData/WeatherForecasts';

    console.log(" start FetchDataComponent from " + forecastsUrl +"\n\n\n\n\n\n");
    
    http.get(forecastsUrl).subscribe(
      result =>
      {
          this.forecasts = result.json() as WeatherForecast[];
      },
      error => console.error(error));
  }
}

interface WeatherForecast {
    dateFormatted: string;
    temperatureC: number;
    temperatureF: number;
    summary: string;
}
