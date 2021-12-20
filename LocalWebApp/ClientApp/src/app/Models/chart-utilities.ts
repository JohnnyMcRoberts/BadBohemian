import { SeriesColors } from './series-colors';

export class ChartUtilities {
    static readonly chartWidth: number = 1250;
    static readonly chartHeight: number = 650;

    public static getLineSeries(deltaDates: Date[], yValues: number[], seriesName: string, color: string): any {
        var deltaLineSeries =
        {
            x: deltaDates,
            y: yValues,
            name: seriesName,
            type: 'scatter',
            mode: 'lines',
            line:
            {
                color: color,
                shape: 'spline'
            }
        };

        return deltaLineSeries;
    }

    public static getStackedAreaSeries(deltaDates: Date[], yValues: number[], seriesName: string, color: string): any {
        var deltaLineSeries =
        {
            x: deltaDates,
            y: yValues,
            name: seriesName,
            stackgroup: 'one',
            line:
            {
                color: color,
            }
        };

        return deltaLineSeries;
    }

    public static getLineSeriesForCategories(displayedCategoriesByTime: Map<string, number[]>, deltaDates: Date[]): any[] {
        let categorySeries = new Array<any>();
        let categories = Array.from(displayedCategoriesByTime.keys());
        for (let i = 0; i < categories.length; i++) {
            let category = categories[i];
            let yValues = displayedCategoriesByTime.get(category) as number[];

            categorySeries.push(
                this.getLineSeries(deltaDates, yValues, category, SeriesColors.liveChartsColors[i]));
        }

        return categorySeries;
    }

    public static getStackedAreaSeriesForCategories(displayedCategoriesByTime: Map<string, number[]>, deltaDates: Date[]): any[] {
        let categorySeries = new Array<any>();
        let categories = Array.from(displayedCategoriesByTime.keys());

        for (let i = 0; i < categories.length; i++) {
            let category = categories[i];
            let yValues = displayedCategoriesByTime.get(category) as number[];

            categorySeries.push(
                this.getStackedAreaSeries(deltaDates, yValues, category, SeriesColors.liveChartsColors[i]));
        }

        return categorySeries;
    }

    public static getPiePlotData(ttlLabels: string[], ttlValues: number[], colors: string[]): any[] {
        var plotData: any[] =
            [
                {
                    values: ttlValues,
                    labels: ttlLabels,
                    marker:
                    {
                        colors: colors
                    },
                    opacity: 0.8,
                    type: 'pie',
                    direction: "clockwise",
                    rotation: 90,
                    hole: .4
                }
            ];

        return plotData;
    }
}
