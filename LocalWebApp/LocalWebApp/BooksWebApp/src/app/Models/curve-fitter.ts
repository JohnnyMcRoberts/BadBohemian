export interface ICurveFitter {
    evaluateYValueAtPoint(xVal: number): number;
}

export class LinearCurveFitter implements ICurveFitter {
    private _yIntercept: number;
    private _slope: number;
    private _rSquared: number;

    public evaluateYValueAtPoint(xVal: number): number {
        return this._yIntercept + (xVal * this._slope);
    }

    public constructor(xVals: number[], yVals: number[]) {
        // Set up the slope parameters
        let rsquared: number = 0;
        let yintercept: number = 0;
        let slope: number = 0;

        // Fit a line to a collection of (x,y) points.

        if (xVals.length !== yVals.length || xVals.length < 1) {
            // Invalid data - set everything to zero & stop
            this._yIntercept = yintercept;
            this._slope = slope;
            this._rSquared = rsquared;

            return;
        }

        let sumOfX: number = 0;
        let sumOfY: number = 0;
        let sumOfXSq: number = 0;
        let sumOfYSq = 0;
        let ssX: number;
        let ssY: number;
        let sumCodeviates: number = 0;
        let sCo: number;
        let count: number = xVals.length;

        for (let ctr = 0; ctr < count; ctr++) {
            let x: number = xVals[ctr];
            let y: number = yVals[ctr];
            sumCodeviates += x * y;
            sumOfX += x;
            sumOfY += y;
            sumOfXSq += x * x;
            sumOfYSq += y * y;
        }

        ssX = sumOfXSq - ((sumOfX * sumOfX) / count);
        ssY = sumOfYSq - ((sumOfY * sumOfY) / count);
        let rNumerator = (count * sumCodeviates) - (sumOfX * sumOfY);
        let rDenom =
            (count * sumOfXSq - (sumOfX * sumOfX))
            * (count * sumOfYSq - (sumOfY * sumOfY));
        sCo = sumCodeviates - ((sumOfX * sumOfY) / count);

        let meanX = sumOfX / count;
        let meanY = sumOfY / count;
        let dblR = rNumerator / Math.sqrt(rDenom);
        rsquared = dblR * dblR;
        yintercept = meanY - ((sCo / ssX) * meanX);
        slope = sCo / ssX;

        // Store the values
        this._yIntercept = yintercept;
        this._slope = slope;
        this._rSquared = rsquared;
    }
}

export class QuadraticCurveFitter implements ICurveFitter {
    //#region Private data

    private _a: number;
    private _b: number;
    private _c: number;

    /* instance variables */
    private _pointArray: Array<number[]> = new Array<number[]>();
    private _numOfEntries: number;
    private _pointPair: number[];

    //#endregion

    //#region ICurveFitter

    public evaluateYValueAtPoint(xVal: number): number {
        return (xVal * xVal * this._a) + (xVal * this._b) + this._c;
    }

    //#endregion

    //#region Utility Methods


    /*instance methods */

    // add point pairs
    private AddPoints(x: number, y: number): void {
        this._pointPair = new Array<number>(2);
        this._numOfEntries += 1;
        this._pointPair[0] = x;
        this._pointPair[1] = y;
        this._pointArray.push(this._pointPair);
    }


    // returns the a term of the equation ax^2 + bx + c
    private aTerm(): number {
        if (this._numOfEntries < 3) {
            throw new TypeError("Insufficient pairs of co-ordinates");
        }

        //notation sjk to mean the sum of x_i^j*y_i^k. 
        let s40 = this.GetSx4(); //sum of x^4
        let s30 = this.GetSx3(); //sum of x^3
        let s20 = this.GetSx2(); //sum of x^2
        let s10 = this.GetSx();  //sum of x
        let s00 = this._numOfEntries;
        //sum of x^0 * y^0  ie 1 * number of entries

        let s21 = this.GetSx2y(); //sum of x^2*y
        let s11 = this.GetSxy();  //sum of x*y
        let s01 = this.GetSy();   //sum of y

        //a = Da/D
        return (s21 * (s20 * s00 - s10 * s10) -
            s11 * (s30 * s00 - s10 * s20) +
            s01 * (s30 * s10 - s20 * s20))
            /
            (s40 * (s20 * s00 - s10 * s10) -
                s30 * (s30 * s00 - s10 * s20) +
                s20 * (s30 * s10 - s20 * s20));
    }

    // returns the b term of the equation ax^2 + bx + c
    private bTerm(): number {
        if (this._numOfEntries < 3) {
            throw new TypeError("Insufficient pairs of co-ordinates");
        }

        //notation sjk to mean the sum of x_i^j*y_i^k.
        let s40 = this.GetSx4(); //sum of x^4
        let s30 = this.GetSx3(); //sum of x^3
        let s20 = this.GetSx2(); //sum of x^2
        let s10 = this.GetSx();  //sum of x
        let s00 = this._numOfEntries;
        //sum of x^0 * y^0  ie 1 * number of entries

        let s21 = this.GetSx2y(); //sum of x^2*y
        let s11 = this.GetSxy();  //sum of x*y
        let s01 = this.GetSy();   //sum of y

        //b = Db/D
        return (s40 * (s11 * s00 - s01 * s10) -
            s30 * (s21 * s00 - s01 * s20) +
            s20 * (s21 * s10 - s11 * s20))
            /
            (s40 * (s20 * s00 - s10 * s10) -
                s30 * (s30 * s00 - s10 * s20) +
                s20 * (s30 * s10 - s20 * s20));
    }

    // returns the c term of the equation ax^2 + bx + c
    private cTerm(): number {
        if (this._numOfEntries < 3) {
            throw new TypeError("Insufficient pairs of co-ordinates");
        }

        //notation sjk to mean the sum of x_i^j*y_i^k.
        let s40 = this.GetSx4(); //sum of x^4
        let s30 = this.GetSx3(); //sum of x^3
        let s20 = this.GetSx2(); //sum of x^2
        let s10 = this.GetSx();  //sum of x
        let s00 = this._numOfEntries;
        //sum of x^0 * y^0  ie 1 * number of entries

        let s21 = this.GetSx2y(); //sum of x^2*y
        let s11 = this.GetSxy();  //sum of x*y
        let s01 = this.GetSy();   //sum of y

        //c = Dc/D
        return (s40 * (s20 * s01 - s10 * s11) -
            s30 * (s30 * s01 - s10 * s21) +
            s20 * (s30 * s11 - s20 * s21))
            /
            (s40 * (s20 * s00 - s10 * s10) -
                s30 * (s30 * s00 - s10 * s20) +
                s20 * (s30 * s10 - s20 * s20));
    }

    private rSquare(): number // get r-squared
    {
        if (this._numOfEntries < 3) {
            throw new TypeError("Insufficient pairs of co-ordinates");
        }

        // 1 - (residual sum of squares / total sum of squares)
        return 1 - this.GetSSerr() / this.GetSStot();
    }


    /*helper methods*/
    private GetSx(): number // get sum of x
    {
        let Sx = 0;
        for (let i = 0; i < this._pointArray.length; i++) {
            let ppair: number[] = this._pointArray[i];
            Sx += ppair[0];
        }
        return Sx;
    }

    private GetSy(): number // get sum of y
    {
        let Sy = 0;
        for (let i = 0; i < this._pointArray.length; i++) {
            let ppair: number[] = this._pointArray[i];
            Sy += ppair[1];
        }
        return Sy;
    }

    private GetSx2(): number // get sum of x^2
    {
        let Sx2 = 0;
        for (let i = 0; i < this._pointArray.length; i++) {
            let ppair: number[] = this._pointArray[i];
            Sx2 += Math.pow(ppair[0], 2); // sum of x^2
        }
        return Sx2;
    }

    private GetSx3(): number // get sum of x^3
    {
        let Sx3 = 0;
        for (let i = 0; i < this._pointArray.length; i++) {
            let ppair: number[] = this._pointArray[i];
            Sx3 += Math.pow(ppair[0], 3); // sum of x^3
        }
        return Sx3;
    }

    private GetSx4(): number // get sum of x^4
    {
        let Sx4 = 0;
        for (let i = 0; i < this._pointArray.length; i++) {
            let ppair: number[] = this._pointArray[i];
            Sx4 += Math.pow(ppair[0], 4); // sum of x^4
        }
        return Sx4;
    }

    private GetSxy(): number // get sum of x*y
    {
        let Sxy = 0;
        for (let i = 0; i < this._pointArray.length; i++) {
            let ppair: number[] = this._pointArray[i];
            Sxy += ppair[0] * ppair[1]; // sum of x*y
        }
        return Sxy;
    }

    private GetSx2y(): number // get sum of x^2*y
    {
        let Sx2y = 0;
        for (let i = 0; i < this._pointArray.length; i++) {
            let ppair: number[] = this._pointArray[i];
            Sx2y += Math.pow(ppair[0], 2) * ppair[1]; // sum of x^2*y
        }
        return Sx2y;
    }

    private GetYMean(): number // mean value of y
    {
        let y_tot = 0;
        for (let i = 0; i < this._pointArray.length; i++) {
            let ppair: number[] = this._pointArray[i];
            y_tot += ppair[1];
        }
        return y_tot / this._numOfEntries;
    }

    private GetSStot(): number // total sum of squares
    {
        //the sum of the squares of the differences between 
        //the measured y values and the mean y value
        let ss_tot = 0;
        for (let i = 0; i < this._pointArray.length; i++) {
            let ppair: number[] = this._pointArray[i];
            ss_tot += Math.pow(ppair[1] - this.GetYMean(), 2);
        }
        return ss_tot;
    }

    private GetSSerr(): number // residual sum of squares
    {
        //the sum of the squares of te difference between 
        //the measured y values and the values of y predicted by the equation
        let ss_err = 0;
        for (let i = 0; i < this._pointArray.length; i++) {
            let ppair: number[] = this._pointArray[i];
            ss_err += Math.pow(ppair[1] - this.GetPredictedY(ppair[0]), 2);
        }
        return ss_err;
    }

    private GetPredictedY(x: number): number {
        //returns value of y predicted by the equation for a given value of x
        return this.aTerm() * Math.pow(x, 2) + this.bTerm() * x + this.cTerm();
    }

    //#endregion

    public constructor(xVals: number[], yVals: number[]) {
        this._numOfEntries = 0;
        this._pointPair = new Array<number>(2);
        this._a = this._b = this._c = 0.0;

        for (let i = 0; i < xVals.length && i < yVals.length; i++) {
            this.AddPoints(xVals[i], yVals[i]);
        }

        this._a = this.aTerm();
        this._b = this.bTerm();
        this._c = this.cTerm();
    }

}


