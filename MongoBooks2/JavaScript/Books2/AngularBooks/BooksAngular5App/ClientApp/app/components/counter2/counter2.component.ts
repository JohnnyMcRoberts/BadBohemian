import { Component } from '@angular/core';

@Component({
    selector: 'counter2',
    templateUrl: './counter2.component.html'
})
export class Counter2Component {
    public currentCount = 0;

    public incrementCounter() {
        this.currentCount++;
    }
}
