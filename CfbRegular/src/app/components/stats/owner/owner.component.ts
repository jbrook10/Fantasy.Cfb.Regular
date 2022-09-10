import { IGameLog, IPlayer, IWeekScoreLog } from './../../../models/models';
import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { IOwner } from 'src/app/models/models';
import { MatTableDataSource } from '@angular/material/table';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Observable } from 'rxjs';
import { map, shareReplay } from 'rxjs/operators';

@Component({
  selector: 'app-owner',
  templateUrl: './owner.component.html',
  styleUrls: ['./owner.component.scss']
})
export class OwnerComponent implements OnChanges {

  @Input() Owner!: IOwner;
  @Input() Week!: number;

  players: IPlayer[] = [];

  playerLogs: IWeekScoreLog[] = [];

  columns = ['Name', 'Pos', 'Opp', 'PassYds', 'RushYds', 'Rec', 'RecYds', 'Tds', 'Score'];
  columnsDetail = ['OppLong'];

  columnsFooter = ['Score'];
  tempLogs: IGameLog[] = [];

  expanded = false;
  total = 0;

  isHandset$: Observable<boolean> = this.breakpointObserver.observe(Breakpoints.Handset)
    .pipe(
      map(result => result.matches),
      shareReplay()
    );


  dataSource = new MatTableDataSource<IWeekScoreLog>();


  isDescriptionRow = (index: number, element: IWeekScoreLog) => !element.IsDescription;


  constructor(private breakpointObserver: BreakpointObserver) {
  }
  ngOnChanges(changes: SimpleChanges): void {

    if (changes.Week) {
      this.playerLogs = [];
      this.players = this.Owner.Players.filter(p => p.Logs.some(l => l.Week === this.Week));

      const now = Date.now();

      this.players.forEach(player => {
        player.WorkingLogs = player.Logs.filter(l => l.Week === this.Week);
        player.WorkingLogs.sort((a, b) => b.Score - a.Score);

        player.WorkingLogs.forEach(log => {

          const gameDate = new Date(log.Date);
          const lvl = !log.IsLog ? '' : log.Score >= 40 ? 'lvl4' : log.Score >= 30 ? 'lvl3' : log.Score >= 20 ? 'lvl2' : 'lvl1';

          this.playerLogs.push(
            {
              Name: player.Name,
              Link: player.Link,
              Position: player.Position,
              Score: log.Score,
              PassYds: log.PassYds,
              RushYds: log.RushYds,
              RecYds: log.RecYds,
              Rec: log.Rec,
              Tds: log.Tds,
              Opponent: log.Opp,
              IsDescription: false,
              School: player.School,
              Date: log.Date,
              Vs: log.Vs,
              Complete: log.IsLog,
              css: lvl
            }
          );

          this.playerLogs.push(
            {
              Name: player.Name,
              Link: player.Link,
              Position: player.Position,
              Score: log.Score,
              PassYds: log.PassYds,
              RushYds: log.RushYds,
              RecYds: log.RecYds,
              Rec: log.Rec,
              Tds: log.Tds,
              Opponent: log.Opp,
              IsDescription: true,
              School: player.School,
              Date: log.Date,
              Vs: log.Vs,
              Complete: log.IsLog,
              css: lvl
            }
          );
        });
      });

      this.playerLogs.sort((a, b) => b.Score - a.Score ||
        (new Date(a.Date).valueOf() - new Date(b.Date).valueOf()) ||
        (b.Opponent < a.Opponent ? 1 : -1));

      this.total = this.Owner.Weeks.find(w => w.Number === this.Week)?.Score || 0;
      this.dataSource.data = this.playerLogs;
    }

  }

  scrollTo(name: string): void {
    const el = document.getElementById(name);
    if (!!el) {
      el.scrollIntoView();
    }
  }
}
