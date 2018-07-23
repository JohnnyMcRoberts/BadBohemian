import { Component, Input, OnInit } from '@angular/core';

import { GoogleChartsBaseService } from './GoogleChartsBaseService';
import { Injectable } from '@angular/core';
import { PieChartConfig } from './../Models/PieChartConfig';

declare var google: any;

@Injectable()
export class GooglePieChartService extends GoogleChartsBaseService {

  constructor() { super(); }

  public BuildPieChart(elementId: String, data: any[], config: PieChartConfig): void {
    var chartFunc = () => { return new google.visualization.PieChart(document.getElementById(elementId)); };
    var options = {
      title: config.title,
      pieHole: config.pieHole,
    };

    this.buildChart(data, chartFunc, options);
  }
}