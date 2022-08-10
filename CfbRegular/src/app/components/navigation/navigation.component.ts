import { DataService } from '../../service/data.service';
import { Component, OnInit } from '@angular/core';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Observable } from 'rxjs';
import { map, shareReplay } from 'rxjs/operators';

@Component({
  selector: 'app-navigation',
  templateUrl: './navigation.component.html',
  styleUrls: ['./navigation.component.scss']
})
export class NavigationComponent implements OnInit {

  Year = '';
  Years: number[] = [];


  isHandset$: Observable<boolean> = this.breakpointObserver.observe(Breakpoints.Handset)
    .pipe(
      map(result => result.matches),
      shareReplay()
    );

  constructor(private rosterService: DataService, private breakpointObserver: BreakpointObserver) {

    const today = new Date();
    if (today.getMonth() < 7) {
      this.Year = today.getFullYear() - 1 + '';
    } else {
      this.Year = new Date().getFullYear() + '';
    }

    for (let index = 2019; index <= today.getFullYear(); index++) {
      if (index !== 2020) { // covid sucks
        this.Years.push(index)
      }
    }

  }

  ngOnInit(): void {
    this.loadFile();
  }

  loadFile(): void {
    this.rosterService.Year = +this.Year;
    this.rosterService.GetLeagueData(this.rosterService.Year);
  }
}
