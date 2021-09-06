import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NavigationComponent } from './components/navigation/navigation.component';
import { LayoutModule } from '@angular/cdk/layout';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatChipsModule } from '@angular/material/chips';
import { MatDialogModule } from '@angular/material/dialog';


import { LeaderboardComponent } from './components/leaderboard/leaderboard.component';
import { StatsComponent } from './components/stats/stats.component';
import { HttpClientModule } from '@angular/common/http';
import { OwnerComponent } from './components/stats/owner/owner.component';
import { FlexLayoutModule } from '@angular/flex-layout';
import { WeekComponent } from './components/stats/week/week.component';
import { WeekLeaderboardComponent } from './components/stats/week/week-leaderboard/week-leaderboard.component';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { TotalsComponent } from './components/totals/totals.component';
import { PlayerLogsComponent } from './components/player-logs/player-logs.component';

@NgModule({
  declarations: [
    AppComponent,
    NavigationComponent,
    LeaderboardComponent,
    StatsComponent,
    OwnerComponent,
    WeekComponent,
    WeekLeaderboardComponent,
    TotalsComponent,
    PlayerLogsComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    LayoutModule,
    HttpClientModule,
    MatToolbarModule,
    MatButtonModule,
    MatSidenavModule,
    MatIconModule,
    MatListModule,
    MatTableModule,
    MatCardModule,
    MatSelectModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    MatDialogModule,

    FlexLayoutModule,

    MatPaginatorModule,

    MatSortModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
