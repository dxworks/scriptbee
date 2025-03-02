import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LoadingResultsDialogComponent } from './loading-results-dialog.component';

describe('LoadingResultsDialogComponent', () => {
  let component: LoadingResultsDialogComponent;
  let fixture: ComponentFixture<LoadingResultsDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [LoadingResultsDialogComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LoadingResultsDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
