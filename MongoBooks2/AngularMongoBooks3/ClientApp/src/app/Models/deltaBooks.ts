import { TallyDelta } from './TallyDelta'

export interface IDeltaBooks
{
  date: Date;
  startDate: Date;
  daysSinceStart: number;
  overallTally: TallyDelta;
  lastTenTally: TallyDelta;
};

export class DeltaBooks implements IDeltaBooks
{
  constructor(
    public date: Date = new Date(),
    public startDate: Date = new Date(),
    public daysSinceStart: number = 1,
    public overallTally: TallyDelta = new TallyDelta(),
    public lastTenTally: TallyDelta = new TallyDelta())
  {
  }
}
