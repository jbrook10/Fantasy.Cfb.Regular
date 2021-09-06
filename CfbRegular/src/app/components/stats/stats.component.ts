import { ILeagueData, IOwner } from './../../models/models';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { DataService } from 'src/app/service/data.service';
import { combineLatest, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-stats',
  templateUrl: './stats.component.html',
  styleUrls: ['./stats.component.scss']
})
export class StatsComponent implements OnInit, OnDestroy {

  LeagueData!: ILeagueData;
  Owners: IOwner[] = [];
  loading = true;
  SmallScreen = false;
  Week = 0;
  $Destroyed = new Subject();


  constructor(private rosterService: DataService, breakpointObserver: BreakpointObserver) {
    breakpointObserver.observe([
      Breakpoints.Small,
      Breakpoints.XSmall,
    ]).subscribe(result => {
      this.SmallScreen = result.matches;
    });

  }

  ngOnInit(): void {

    combineLatest([this.rosterService.LeagueData$, this.rosterService.CurrentWeek$])
      .pipe(takeUntil(this.$Destroyed)).subscribe(([leagueData, week]) => {
        if (!!leagueData && !!week) {
          this.LeagueData = leagueData;
          this.Owners = this.LeagueData.Owners;
          this.Week = week;
          this.loading = false;
        }
      });

    this.SetWeek(1);
  }

  SetWeek(week: number): void {
    this.rosterService.SetWeek(week);
  }

  ngOnDestroy(): void {
    this.$Destroyed.next();
    this.$Destroyed.complete();
  }


}
