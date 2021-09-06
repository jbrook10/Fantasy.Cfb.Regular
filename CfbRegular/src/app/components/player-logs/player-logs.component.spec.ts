import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PlayerLogsComponent } from './player-logs.component';

describe('PlayerLogsComponent', () => {
  let component: PlayerLogsComponent;
  let fixture: ComponentFixture<PlayerLogsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PlayerLogsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PlayerLogsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
