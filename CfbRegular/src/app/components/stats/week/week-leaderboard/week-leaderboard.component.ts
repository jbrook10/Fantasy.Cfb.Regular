import { AfterViewInit, Component, Input, ViewChild, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { IOwner } from 'src/app/models/models';

@Component({
  selector: 'app-week-leaderboard',
  templateUrl: './week-leaderboard.component.html',
  styleUrls: ['./week-leaderboard.component.scss']
})
export class WeekLeaderboardComponent implements OnChanges {

  @Input() Owners!: IOwner[];
  @Input() Week!: number;

  // sortedOwners: IOwner[] = [];

  dataSource = new MatTableDataSource<IOwner>();


  displayedColumns = ['Name', 'Count', 'Score'];

  constructor() {
  }
  ngOnChanges(changes: SimpleChanges): void {
    if (changes.Owners || changes.Week) {
      this.Owners.forEach(owner => {
        owner.Weeks[this.Week - 1].PlayerCount = owner.Players.flatMap(p => p.Logs).filter(l => l.Week === this.Week).length;
      });
      this.dataSource.data = this.Owners.sort((a, b) => b.Weeks[this.Week - 1].Score - a.Weeks[this.Week - 1].Score);
    }
  }

  scrollTo(name: string): void {
    const el = document.getElementById(name);
    if (!!el) {
      el.scrollIntoView();
    }
  }
}
