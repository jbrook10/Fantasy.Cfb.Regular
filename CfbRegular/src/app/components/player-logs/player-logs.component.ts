import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { IGameLog, IPlayer } from 'src/app/models/models';

@Component({
  selector: 'app-player-logs',
  templateUrl: './player-logs.component.html',
  styleUrls: ['./player-logs.component.scss']
})
export class PlayerLogsComponent implements OnInit {

  columns = ['Week', 'Opp', 'PassYds', 'RushYds', 'Rec', 'RecYds', 'Tds', 'Score'];

  dataSource = new MatTableDataSource<IGameLog>();

  constructor(@Inject(MAT_DIALOG_DATA) public data: IPlayer) { }

  ngOnInit(): void {
    this.dataSource.data = this.data.Logs;
  }

}
