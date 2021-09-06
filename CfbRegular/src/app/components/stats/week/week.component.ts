import { ILeagueData, IOwner, IWeek } from 'src/app/models/models';
import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';

@Component({
  selector: 'app-week',
  templateUrl: './week.component.html',
  styleUrls: ['./week.component.scss']
})
export class WeekComponent implements OnChanges {

  @Input() LeagueData!: ILeagueData;
  @Input() Week!: number;
  Owners!: IOwner[];
  CurrentWeek: IWeek | undefined;

  constructor() { }
  ngOnChanges(changes: SimpleChanges): void {
    if (changes.Week) {
      console.log(this.Week);
      this.CurrentWeek = this.LeagueData.Weeks.find(w => w.Number === this.Week);
    }
    this.Owners = this.LeagueData.Owners;
  }
}
