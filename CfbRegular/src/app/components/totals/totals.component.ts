import { PlayerLogsComponent } from './../player-logs/player-logs.component';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { combineLatest, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { ILeagueData, IOwner, IPlayer } from 'src/app/models/models';
import { DataService } from 'src/app/service/data.service';

@Component({
  selector: 'app-totals',
  templateUrl: './totals.component.html',
  styleUrls: ['./totals.component.scss']
})
export class TotalsComponent implements OnInit {

  LeagueData!: ILeagueData;
  Owners: IOwner[] = [];
  loading = true;
  SmallScreen = false;
  $Destroyed = new Subject();

  columns = ['Owner', 'Name', 'Pos', 'School', 'Score', 'Average'];

  dataSource = new MatTableDataSource<IPlayer>();


  constructor(private rosterService: DataService, breakpointObserver: BreakpointObserver, public dialog: MatDialog) {
    breakpointObserver.observe([
      Breakpoints.Small,
      Breakpoints.XSmall,
    ]).subscribe(result => {
      this.SmallScreen = result.matches;
    });

  }

  ngOnInit(): void {
    combineLatest([this.rosterService.LeagueData$])
      .pipe(takeUntil(this.$Destroyed)).subscribe(([leagueData]) => {
        if (!!leagueData) {
          this.LeagueData = leagueData;
          this.Owners = this.LeagueData.Owners;
          this.loading = false;

          let allPlayers: IPlayer[] = [];
          this.LeagueData.Owners.forEach(owner => {
            const players = owner.Players.map(p => {
              p.FantasyOwner = owner.Name;
              return p;
            });
            allPlayers = allPlayers.concat(players);
          });

          this.dataSource.data = allPlayers.sort((a, b) => b.Avg - a.Avg || b.Score - a.Score);
        }
      });
  }

  showLogs(player: IPlayer): void {
    console.log(player);

    this.dialog.open(PlayerLogsComponent, {
      data: player,
      minWidth: '95%'
    });

  }

}
