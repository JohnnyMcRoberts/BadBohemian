import { Component, OnInit, Input } from '@angular/core';
import ICountryTotal = books.ICountryTotal;
import { LibraryService } from './../Services/library.service';

declare var google: any;


@Component({
  selector: 'app-pie-chart',
  templateUrl: './pie-chart.component.html',
  styleUrls: ['./pie-chart.component.css']
})
export class PieChartComponent implements OnInit {
  @Input() userId: string;
  @Input() maxDisplayItems: number;
  @Input() isForBooks: boolean;

  constructor(private libraryService: LibraryService)
  {
    google.charts.load("current", { "packages": ["corechart"] });
  }

  chartTitle: string;

  getCountryPageTotals(): void {
    this.libraryService.GetCountryTotals(true)
      .subscribe(totals => this.countryPageTotals = totals );
  }

  getCountryBookTotals(): void {
    this.libraryService.GetCountryTotals(false)
      .subscribe(totals => this.countryBookTotals = totals);
  } 

  ngOnInit() {
    this.getCountryPageTotals();
    this.getCountryBookTotals();

    this.chartTitle = `Book Countries. construct = *${this.userId}* max items =  *${this.maxDisplayItems - 1}*  for pages =  *${!this.isForBooks}*`;

    google.charts.setOnLoadCallback(
      () => { drawChart(this.countryPageTotals, this.countryBookTotals); }
    );
  }
  
  public countryPageTotals: ICountryTotal[];
  public countryBookTotals: ICountryTotal[];
}

// Draw the chart and set the chart values
function drawChart(countryPages: ICountryTotal[], countryBooks: ICountryTotal[]): void
{
  var countryPagesPieChartData = new google.visualization.DataTable();
  countryPagesPieChartData.addColumn('string', 'Country');
  countryPagesPieChartData.addColumn('number', 'Pages');

  for (let bookData of countryPages)
  {
    countryPagesPieChartData.addRow([bookData.nationality, bookData.total]);
  }

  // Optional; add a title and set the width and height of the chart
  var countryPagesPieChartOptions = { 'title': 'Pages by country', 'width': 550, 'height': 400 };

  // Display the chart inside the <div> element with id="countryPagesPieChart"
  var countryPagesPieChartChart = new google.visualization.PieChart(document.getElementById("countryPagesPieChart"));
  countryPagesPieChartChart.draw(countryPagesPieChartData, countryPagesPieChartOptions);

  var countryBooksBarChartData = new google.visualization.DataTable();
  countryBooksBarChartData.addColumn('string', 'Country');
  countryBooksBarChartData.addColumn('number', 'Books');

  for (let bookData of countryBooks)
  {
    countryBooksBarChartData.addRow([bookData.nationality, bookData.total]);
  }

  // Optional; add a title and set the width and height of the chart
  var countryBooksBarChartOptions = { "title": "Books by country", "width": 550, "height": 400, legend: "none" };

  // Display the chart inside the <div> element with id="countryBooksBarChart"
  var countryBooksBarChartChart = new google.visualization.BarChart(document.getElementById("countryBooksBarChart"));
  countryBooksBarChartChart.draw(countryBooksBarChartData, countryBooksBarChartOptions);

}
