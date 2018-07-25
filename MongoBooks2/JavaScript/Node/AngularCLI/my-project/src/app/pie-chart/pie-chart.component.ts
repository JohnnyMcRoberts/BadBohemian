import { Component, OnInit, Input } from '@angular/core';
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

  constructor()
  {
    google.charts.load("current", { "packages": ["corechart"] });
    google.charts.setOnLoadCallback(this.drawChart);
  }

  chartTitle: string;
  

  ngOnInit() {
    this.chartTitle = `Book Countries. construct = *${this.userId}* max items =  *${this.maxDisplayItems - 1}*  for pages =  *${!this.isForBooks}*`;
  }


// Draw the chart and set the chart values
  drawChart():void {
    var countryPagesPieChartData = google.visualization.arrayToDataTable([
      ["Country", "Pages"],
      ["USA", 1363],
      ["Scotland", 518],
      ["France", 351],
      ["Hungary", 312],
      ["Belgium", 154],
      ["England", 0]
    ]);

    // Optional; add a title and set the width and height of the chart
    var countryPagesPieChartOptions = { 'title': 'Pages by country', 'width': 550, 'height': 400 };

    // Display the chart inside the <div> element with id="countryPagesPieChart"
    var countryPagesPieChartChart = new google.visualization.PieChart(document.getElementById("countryPagesPieChart"));
    countryPagesPieChartChart.draw(countryPagesPieChartData, countryPagesPieChartOptions);


    var countryBooksBarChartData = google.visualization.arrayToDataTable([
      ["Country", "Books"],
      ["USA", 6],
      ["England", 2],
      ["Belgium", 1],
      ["Hungary", 1],
      ["France", 1],
      ["Scotland", 1]
    ]);

    // Optional; add a title and set the width and height of the chart
    var countryBooksBarChartOptions = { "title": "Books by country", "width": 550, "height": 400, legend: "none" };

    // Display the chart inside the <div> element with id="countryBooksBarChart"
    var countryBooksBarChartChart = new google.visualization.BarChart(document.getElementById("countryBooksBarChart"));
    countryBooksBarChartChart.draw(countryBooksBarChartData, countryBooksBarChartOptions);

  }

}
