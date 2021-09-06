import { IWeekScoreLog } from './../models/models';
import { ILeagueData, YearType } from '../models/models';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class DataService {

  public LeagueData$ = new ReplaySubject<ILeagueData>(1);
  public CurrentWeek$ = new ReplaySubject<number>(1);

  private year: number = new Date().getFullYear();
  public get Year(): number {
    return this.year;
  }
  public set Year(year: number) {
    this.year = year;
  }

  private week = 1;
  public get Week(): number {
    return this.week;
  }
  public set Week(week: number) {
    this.year = week;
  }

  private yearType: YearType = 'Regular';
  public get YearType(): YearType {
    return this.yearType;
  }
  public set YearType(value: YearType) {
    this.yearType = value;
  }

  constructor(private http: HttpClient) { }

  public GetLeagueData(year: number): void {
    this.year = year;
    this.http.get<ILeagueData>(`assets/${this.year}.Regular.Data.json`).subscribe(d => this.LeagueData$.next(d));
  }

  public SetWeek(week: number): void {
    this.week = week;
    this.CurrentWeek$.next(this.week);
  }


}
