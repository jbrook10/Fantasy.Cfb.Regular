import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MatSort, Sort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { ILeagueData, IOwner } from 'src/app/models/models';
import { DataService } from 'src/app/service/data.service';

@Component({
  selector: 'app-leaderboard',
  templateUrl: './leaderboard.component.html',
  styleUrls: ['./leaderboard.component.scss']
})
export class LeaderboardComponent implements OnInit, OnDestroy {

  SmallScreen = false;
  LeagueData!: ILeagueData;
  displayedColumns = ['Name', 'Score'];

  dataSource = new MatTableDataSource<IOwner>();

  @ViewChild(MatSort) sort: MatSort = new MatSort();


  $Destroyed = new Subject();

  constructor(private rosterService: DataService, breakpointObserver: BreakpointObserver) {
    breakpointObserver.observe([
      Breakpoints.Small,
      Breakpoints.XSmall,
    ]).subscribe(result => {
      this.SmallScreen = result.matches;
    });

  }
  ngOnDestroy(): void {
    this.$Destroyed.next();
    this.$Destroyed.complete();
  }


  ngOnInit(): void {
    this.rosterService.LeagueData$.pipe(takeUntil(this.$Destroyed)).subscribe(data => {
      this.LeagueData = data;

      this.LeagueData.Owners.forEach(o => {
        let ownerScore = 0;
        o.Weeks.forEach(w => ownerScore += w.Score);
        o.TotalScore = ownerScore;
      });

      this.dataSource.data = this.LeagueData.Owners;
      this.dataSource.sort = this.sort;

      const sortState: Sort = {active: 'TotalScore', direction: 'desc'};
      this.sort.active = sortState.active;
      this.sort.direction = sortState.direction;
      this.sort.sortChange.emit(sortState);


    });
  }

}
